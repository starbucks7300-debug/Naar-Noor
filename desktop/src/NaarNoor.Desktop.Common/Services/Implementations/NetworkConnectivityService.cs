using System.Net.NetworkInformation;
using System.Reactive.Subjects;
using Microsoft.Extensions.Logging;
using NaarNoor.Desktop.Common.Services.Interfaces;

namespace NaarNoor.Desktop.Common.Services.Implementations
{
    /// <summary>
    /// Service for detecting network connectivity state changes.
    /// Uses System.Net.NetworkInformation.NetworkChange to detect when network becomes available/unavailable.
    /// Caches current online state and emits observable events on state change.
    /// Implements REQ-016: Offline mode detection and sync queue processing
    /// </summary>
    public class NetworkConnectivityService : INetworkConnectivityService, IDisposable
    {
        private bool _isOnline;
        private readonly Subject<bool> _connectivityChanged = new();
        private readonly ILogger<NetworkConnectivityService>? _logger;
        private bool _disposed = false;

        /// <summary>
        /// Gets whether the device is currently connected to the network.
        /// </summary>
        public bool IsOnline => _isOnline;

        /// <summary>
        /// Observable that fires when connectivity state changes.
        /// Emits true when online, false when offline.
        /// </summary>
        public IObservable<bool> ConnectivityChanged => _connectivityChanged;

        public NetworkConnectivityService(ILogger<NetworkConnectivityService>? logger = null)
        {
            _logger = logger;
            
            // Initialize with current state
            _isOnline = CheckIsOnline();
            _logger?.LogInformation("NetworkConnectivityService initialized. Current state: {IsOnline}", _isOnline);

            // Subscribe to network change events
            NetworkChange.NetworkAddressChanged += OnNetworkAddressChanged;
            NetworkChange.NetworkAvailabilityChanged += OnNetworkAvailabilityChanged;
        }

        /// <summary>
        /// Manually check connectivity (useful for forcing a refresh).
        /// </summary>
        public async Task<bool> CheckConnectivityAsync()
        {
            // Run on thread pool to avoid blocking
            var newState = await Task.Run(() => CheckIsOnline());

            // If state changed, emit event
            if (newState != _isOnline)
            {
                _isOnline = newState;
                _logger?.LogInformation("Network connectivity changed: {IsOnline}", _isOnline);
                _connectivityChanged.OnNext(_isOnline);
            }

            return newState;
        }

        /// <summary>
        /// Determine if the device is online by checking available network interfaces.
        /// Validates that at least one non-loopback interface is up and has active traffic.
        /// </summary>
        private static bool CheckIsOnline()
        {
            try
            {
                // Check if any network interface is up and connected
                var interfaces = NetworkInterface.GetAllNetworkInterfaces();

                if (interfaces.Length == 0)
                {
                    return false;
                }

                return interfaces.Any(ni =>
                    ni.IsReceiveOnly == false
                    && ni.OperationalStatus == OperationalStatus.Up
                    && ni.NetworkInterfaceType != NetworkInterfaceType.Loopback
                    && HasActiveConnection(ni));
            }
            catch (Exception ex)
            {
                // If we can't determine status, assume online (fail open)
                // This prevents false offline states due to permission issues
                System.Diagnostics.Debug.WriteLine($"Error checking connectivity: {ex.Message}");
                return true;
            }
        }

        /// <summary>
        /// Check if a network interface has active connection by verifying it has IPv4 statistics.
        /// </summary>
        private static bool HasActiveConnection(NetworkInterface ni)
        {
            try
            {
                var ipv4Stats = ni.GetIPv4Statistics();
                return ipv4Stats.BytesReceived > 0 || ipv4Stats.BytesSent > 0;
            }
            catch
            {
                // If we can't get stats, assume the interface is active if it's up
                return true;
            }
        }

        /// <summary>
        /// Handle NetworkChange.NetworkAddressChanged event.
        /// Fired when IP address configuration changes (address added/removed).
        /// </summary>
        private void OnNetworkAddressChanged(object? sender, EventArgs e)
        {
            _logger?.LogDebug("NetworkAddressChanged event fired");
            CheckConnectivityAndEmit();
        }

        /// <summary>
        /// Handle NetworkChange.NetworkAvailabilityChanged event.
        /// Fired when network availability changes (connect/disconnect).
        /// </summary>
        private void OnNetworkAvailabilityChanged(object? sender, NetworkAvailabilityEventArgs e)
        {
            _logger?.LogDebug("NetworkAvailabilityChanged event fired. IsAvailable: {IsAvailable}", e.IsAvailable);
            
            // e.IsAvailable indicates if network is available
            var newState = e.IsAvailable && CheckIsOnline();

            if (newState != _isOnline)
            {
                _isOnline = newState;
                _logger?.LogInformation("Network state changed via NetworkAvailabilityChanged: {IsOnline}", _isOnline);
                _connectivityChanged.OnNext(_isOnline);
            }
        }

        /// <summary>
        /// Check connectivity and emit event if state changed.
        /// </summary>
        private void CheckConnectivityAndEmit()
        {
            var newState = CheckIsOnline();

            if (newState != _isOnline)
            {
                _isOnline = newState;
                _logger?.LogInformation("Network connectivity changed: {IsOnline}", _isOnline);
                _connectivityChanged.OnNext(_isOnline);
            }
        }

        /// <summary>
        /// Clean up resources (unsubscribe from network change events).
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            NetworkChange.NetworkAddressChanged -= OnNetworkAddressChanged;
            NetworkChange.NetworkAvailabilityChanged -= OnNetworkAvailabilityChanged;
            
            _connectivityChanged?.Dispose();
            _disposed = true;
        }
    }
}
