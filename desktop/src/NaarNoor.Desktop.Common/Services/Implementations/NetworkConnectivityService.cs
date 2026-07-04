using System.Net.NetworkInformation;
using System.Reactive.Subjects;
using NaarNoor.Desktop.Common.Services.Interfaces;

namespace NaarNoor.Desktop.Common.Services.Implementations
{
    /// <summary>
    /// Service for detecting network connectivity state changes.
    /// Uses System.Net.NetworkInformation.NetworkChange to detect when network becomes available/unavailable.
    /// Caches current online state and emits observable events on state change.
    /// </summary>
    public class NetworkConnectivityService : INetworkConnectivityService, IDisposable
    {
        private bool _isOnline;
        private readonly Subject<bool> _connectivityChanged = new();
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

        public NetworkConnectivityService()
        {
            // Initialize with current state
            _isOnline = CheckIsOnline();

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
                _connectivityChanged.OnNext(_isOnline);
            }

            return newState;
        }

        /// <summary>
        /// Determine if the device is online by checking available network interfaces.
        /// </summary>
        private static bool CheckIsOnline()
        {
            try
            {
                // Check if any network interface is up and connected
                return NetworkInterface
                    .GetAllNetworkInterfaces()
                    .Any(ni => ni.IsReceiveOnly == false
                        && ni.OperationalStatus == OperationalStatus.Up
                        && (ni.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                        && ni.GetIPv4Statistics().BytesReceived > 0);
            }
            catch
            {
                // If we can't determine status, assume online (fail open)
                return true;
            }
        }

        /// <summary>
        /// Handle NetworkChange.NetworkAddressChanged event.
        /// Fired when IP address configuration changes.
        /// </summary>
        private void OnNetworkAddressChanged(object? sender, EventArgs e)
        {
            CheckConnectivityAndEmit();
        }

        /// <summary>
        /// Handle NetworkChange.NetworkAvailabilityChanged event.
        /// Fired when network availability changes (connect/disconnect).
        /// </summary>
        private void OnNetworkAvailabilityChanged(object? sender, NetworkAvailabilityEventArgs e)
        {
            // e.IsAvailable indicates if network is available
            var newState = e.IsAvailable && CheckIsOnline();

            if (newState != _isOnline)
            {
                _isOnline = newState;
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
