namespace NaarNoor.Desktop.Common.Services.Interfaces
{
    using NaarNoor.Desktop.Common.Utilities;
    /// <summary>
    /// Service interface for synchronizing pending operations with the API server.
    /// Processes queued operations when network connectivity is restored.
    /// Implements conflict resolution and handles partial sync failures.
    /// </summary>
    public interface ISyncService
    {
        /// <summary>
        /// Gets whether the sync is currently in progress.
        /// </summary>
        bool IsSyncing { get; }

        /// <summary>
        /// Observable that fires when sync starts or completes.
        /// Emits SyncResult with status and summary information.
        /// </summary>
        IObservable<SyncResult> SyncCompleted { get; }

        /// <summary>
        /// Process all pending operations in the queue, sorted by creation time.
        /// Executes operations in order and handles conflicts with last-write-wins strategy.
        /// Returns summary of success/failure counts.
        /// </summary>
        Task<Result<SyncSummary>> SyncPendingOperationsAsync();

        /// <summary>
        /// Get count of pending operations waiting to be synced.
        /// </summary>
        Task<int> GetPendingOperationCountAsync();

        /// <summary>
        /// Manually clear all pending operations (use with caution).
        /// </summary>
        Task ClearPendingOperationsAsync();
    }

    /// <summary>
    /// Result of a sync operation containing success/failure counts.
    /// </summary>
    public class SyncSummary
    {
        /// <summary>
        /// Total operations that were in queue
        /// </summary>
        public int TotalOperations { get; set; }

        /// <summary>
        /// Operations successfully synced
        /// </summary>
        public int SuccessfulOperations { get; set; }

        /// <summary>
        /// Operations that failed and remain in queue
        /// </summary>
        public int FailedOperations { get; set; }

        /// <summary>
        /// Timestamp when sync completed
        /// </summary>
        public DateTime CompletedAt { get; set; }

        /// <summary>
        /// Summary message for user notification
        /// </summary>
        public string Message { get; set; } = string.Empty;
    }

    /// <summary>
    /// Result event emitted when sync completes.
    /// </summary>
    public class SyncResult
    {
        /// <summary>
        /// Whether the sync was successful
        /// </summary>
        public bool IsSuccessful { get; set; }

        /// <summary>
        /// Sync summary with counts and message
        /// </summary>
        public SyncSummary Summary { get; set; } = new();
    }
}
