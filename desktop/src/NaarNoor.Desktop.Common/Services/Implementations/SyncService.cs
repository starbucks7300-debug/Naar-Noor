using System.Reactive.Subjects;
using NaarNoor.Desktop.Common.Services.Interfaces;
using NaarNoor.Desktop.Common.Utilities;

namespace NaarNoor.Desktop.Common.Services.Implementations
{
    /// <summary>
    /// Sync service for the API-first architecture.
    /// All operations go directly to the centralized API server — no local DB queue needed.
    /// This service is retained for interface compatibility and future offline support.
    /// </summary>
    public class SyncService : ISyncService
    {
        private readonly IAuthenticationService _authService;
        private readonly Subject<SyncResult> _syncCompleted = new();
        private bool _isSyncing = false;

        public bool IsSyncing => _isSyncing;
        public IObservable<SyncResult> SyncCompleted => _syncCompleted;

        public SyncService(IAuthenticationService authService)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        /// <summary>
        /// No pending operations in the API-first model — everything is sent directly to the server.
        /// Returns an immediate success with 0 operations.
        /// </summary>
        public Task<Result<SyncSummary>> SyncPendingOperationsAsync()
        {
            if (_isSyncing)
                return Task.FromResult(Result<SyncSummary>.Failure("Sync already in progress"));

            _isSyncing = true;
            try
            {
                var summary = new SyncSummary
                {
                    TotalOperations      = 0,
                    SuccessfulOperations = 0,
                    FailedOperations     = 0,
                    CompletedAt          = DateTime.UtcNow,
                    Message              = "All operations are sent directly to the API — no pending queue.",
                };

                _syncCompleted.OnNext(new SyncResult { IsSuccessful = true, Summary = summary });
                return Task.FromResult(Result<SyncSummary>.Success(summary));
            }
            finally
            {
                _isSyncing = false;
            }
        }

        public Task<int> GetPendingOperationCountAsync() => Task.FromResult(0);

        public Task ClearPendingOperationsAsync() => Task.CompletedTask;
    }
}
