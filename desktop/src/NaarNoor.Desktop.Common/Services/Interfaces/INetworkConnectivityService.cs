using System.Reactive.Linq;

namespace NaarNoor.Desktop.Common.Services.Interfaces
{
    /// <summary>
    /// Service interface for detecting network connectivity state changes.
    /// </summary>
    public interface INetworkConnectivityService
    {
        /// <summary>
        /// Gets whether the device is currently connected to the network.
        /// </summary>
        bool IsOnline { get; }

        /// <summary>
        /// Observable that fires when connectivity state changes.
        /// Emits true when online, false when offline.
        /// </summary>
        IObservable<bool> ConnectivityChanged { get; }

        /// <summary>
        /// Manually check connectivity (useful for forcing a refresh).
        /// </summary>
        Task<bool> CheckConnectivityAsync();
    }
}
