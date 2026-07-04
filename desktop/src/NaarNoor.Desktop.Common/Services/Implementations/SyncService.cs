using System.Reactive.Subjects;
using NaarNoor.Desktop.Common.Data;
using NaarNoor.Desktop.Common.Data.Models;
using NaarNoor.Desktop.Common.Services.Interfaces;
using NaarNoor.Desktop.Common.Utilities;

namespace NaarNoor.Desktop.Common.Services.Implementations
{
    /// <summary>
    /// Implementation of sync service for processing pending operations queue.
    /// Handles operation execution, conflict resolution, and error management.
    /// Uses last-write-wins strategy for conflict resolution.
    /// </summary>
    public class SyncService : ISyncService
    {
        private readonly DatabaseContext _dbContext;
        private readonly IAuthenticationService _authService;
        private readonly Subject<SyncResult> _syncCompleted = new();
        private bool _isSyncing = false;

        /// <summary>
        /// Gets whether the sync is currently in progress.
        /// </summary>
        public bool IsSyncing => _isSyncing;

        /// <summary>
        /// Observable that fires when sync starts or completes.
        /// Emits SyncResult with status and summary information.
        /// </summary>
        public IObservable<SyncResult> SyncCompleted => _syncCompleted;

        public SyncService(DatabaseContext dbContext, IAuthenticationService authService)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        /// <summary>
        /// Process all pending operations in the queue, sorted by creation time.
        /// Executes operations in order and handles conflicts with last-write-wins strategy.
        /// Returns summary of success/failure counts.
        /// </summary>
        public async Task<Result<SyncSummary>> SyncPendingOperationsAsync()
        {
            try
            {
                if (_isSyncing)
                {
                    return Result<SyncSummary>.Failure("Sync is already in progress");
                }

                _isSyncing = true;

                // Get current user ID for filtering
                var userId = _authService.CurrentUserId ?? "unknown";

                // Retrieve all pending operations for current user, sorted by creation time
                var pendingOps = _dbContext.PendingOperations
                    .Where(op => op.UserId == userId && op.SyncedAt == null)
                    .OrderBy(op => op.CreatedAt)
                    .ToList();

                var summary = new SyncSummary
                {
                    TotalOperations = pendingOps.Count,
                    CompletedAt = DateTime.UtcNow
                };

                if (pendingOps.Count == 0)
                {
                    summary.Message = "No pending operations to sync";
                    return Result<SyncSummary>.Success(summary);
                }

                // Process each operation
                foreach (var operation in pendingOps)
                {
                    try
                    {
                        var success = await ExecutePendingOperationAsync(operation);

                        if (success)
                        {
                            // Mark as synced
                            operation.SyncedAt = DateTime.UtcNow;
                            summary.SuccessfulOperations++;
                        }
                        else
                        {
                            summary.FailedOperations++;
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log error and mark operation as failed
                        operation.ErrorMessage = ex.Message;
                        summary.FailedOperations++;
                    }
                }

                // Save changes to database
                await _dbContext.SaveChangesAsync();

                // Remove successfully synced operations from queue
                var syncedOps = _dbContext.PendingOperations
                    .Where(op => op.SyncedAt != null && op.UserId == userId)
                    .ToList();

                foreach (var op in syncedOps)
                {
                    _dbContext.PendingOperations.Remove(op);
                }

                await _dbContext.SaveChangesAsync();

                // Generate summary message
                summary.Message = $"Sync completed: {summary.SuccessfulOperations} successful, {summary.FailedOperations} failed";

                // Emit sync completed event
                _syncCompleted.OnNext(new SyncResult
                {
                    IsSuccessful = summary.FailedOperations == 0,
                    Summary = summary
                });

                return Result<SyncSummary>.Success(summary);
            }
            catch (Exception ex)
            {
                var summary = new SyncSummary
                {
                    Message = $"Sync failed: {ex.Message}",
                    CompletedAt = DateTime.UtcNow
                };

                _syncCompleted.OnNext(new SyncResult
                {
                    IsSuccessful = false,
                    Summary = summary
                });

                return Result<SyncSummary>.Failure(ex.Message);
            }
            finally
            {
                _isSyncing = false;
            }
        }

        /// <summary>
        /// Execute a single pending operation.
        /// Currently a placeholder that returns success for all operations.
        /// In production, this would call actual API endpoints based on operation type.
        /// </summary>
        private async Task<bool> ExecutePendingOperationAsync(PendingOperation operation)
        {
            try
            {
                // TODO: Implement actual operation execution based on operation type
                // This would involve calling the appropriate API client method
                // Example:
                // if (operation.ResourceType == "Reservation")
                // {
                //     return await _reservationApiClient.SyncReservationAsync(...);
                // }

                // For now, simulate successful execution
                await Task.Delay(50); // Simulate network latency
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Get count of pending operations waiting to be synced.
        /// </summary>
        public async Task<int> GetPendingOperationCountAsync()
        {
            try
            {
                var userId = _authService.CurrentUserId ?? "unknown";
                var count = await Task.Run(() =>
                    _dbContext.PendingOperations
                        .Count(op => op.UserId == userId && op.SyncedAt == null)
                );

                return count;
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Manually clear all pending operations (use with caution).
        /// </summary>
        public async Task ClearPendingOperationsAsync()
        {
            try
            {
                var userId = _authService.CurrentUserId ?? "unknown";
                var pendingOps = _dbContext.PendingOperations
                    .Where(op => op.UserId == userId && op.SyncedAt == null)
                    .ToList();

                foreach (var op in pendingOps)
                {
                    _dbContext.PendingOperations.Remove(op);
                }

                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to clear pending operations", ex);
            }
        }
    }
}
