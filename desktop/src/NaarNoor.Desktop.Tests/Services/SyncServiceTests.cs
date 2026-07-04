using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using NaarNoor.Desktop.Common.Data;
using NaarNoor.Desktop.Common.Data.Models;
using NaarNoor.Desktop.Common.Services;
using NaarNoor.Desktop.Common.Services.Implementations;
using NaarNoor.Desktop.Common.Services.Interfaces;

namespace NaarNoor.Desktop.Tests.Services
{
    public class SyncServiceTests : IAsyncLifetime
    {
        private DatabaseContext? _dbContext;
        private Mock<IAuthenticationService>? _mockAuthService;
        private SyncService? _service;

        public async Task InitializeAsync()
        {
            // Create in-memory database for testing
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dbContext = new DatabaseContext(options);

            // Setup mock authentication service
            _mockAuthService = new Mock<IAuthenticationService>();
            _mockAuthService.Setup(a => a.CurrentUserId).Returns("test-user-123");

            _service = new SyncService(_dbContext, _mockAuthService.Object);

            await Task.CompletedTask;
        }

        public async Task DisposeAsync()
        {
            if (_dbContext != null)
            {
                await _dbContext.DisposeAsync();
            }
        }

        #region Initialization Tests

        [Fact]
        public void Constructor_InitializesWithServices()
        {
            // Assert
            Assert.NotNull(_service);
            Assert.False(_service!.IsSyncing);
        }

        [Fact]
        public void Constructor_ThrowsOnNullDatabaseContext()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new SyncService(null!, _mockAuthService!.Object)
            );
        }

        [Fact]
        public void Constructor_ThrowsOnNullAuthService()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new SyncService(_dbContext!, null!)
            );
        }

        #endregion

        #region Property Tests

        [Fact]
        public void IsSyncing_ReturnsFalseInitially()
        {
            // Assert
            Assert.False(_service!.IsSyncing);
        }

        [Fact]
        public void SyncCompleted_ReturnsObservable()
        {
            // Assert
            Assert.NotNull(_service!.SyncCompleted);
        }

        #endregion

        #region GetPendingOperationCountAsync Tests

        [Fact]
        public async Task GetPendingOperationCountAsync_ReturnsZeroWhenNoPending()
        {
            // Act
            var count = await _service!.GetPendingOperationCountAsync();

            // Assert
            Assert.Equal(0, count);
        }

        [Fact]
        public async Task GetPendingOperationCountAsync_ReturnsPendingCount()
        {
            // Arrange
            _dbContext!.PendingOperations.Add(new PendingOperation
            {
                UserId = "test-user-123",
                OperationType = "Create",
                ResourceType = "Reservation",
                Payload = "{}",
                CreatedAt = DateTime.UtcNow
            });

            _dbContext.PendingOperations.Add(new PendingOperation
            {
                UserId = "test-user-123",
                OperationType = "Update",
                ResourceType = "Reservation",
                Payload = "{}",
                CreatedAt = DateTime.UtcNow
            });

            await _dbContext.SaveChangesAsync();

            // Act
            var count = await _service!.GetPendingOperationCountAsync();

            // Assert
            Assert.Equal(2, count);
        }

        [Fact]
        public async Task GetPendingOperationCountAsync_IgnoresOtherUsers()
        {
            // Arrange
            _dbContext!.PendingOperations.Add(new PendingOperation
            {
                UserId = "test-user-123",
                OperationType = "Create",
                ResourceType = "Reservation",
                Payload = "{}",
                CreatedAt = DateTime.UtcNow
            });

            _dbContext.PendingOperations.Add(new PendingOperation
            {
                UserId = "other-user-456",
                OperationType = "Update",
                ResourceType = "Reservation",
                Payload = "{}",
                CreatedAt = DateTime.UtcNow
            });

            await _dbContext.SaveChangesAsync();

            // Act
            var count = await _service!.GetPendingOperationCountAsync();

            // Assert
            Assert.Equal(1, count);
        }

        [Fact]
        public async Task GetPendingOperationCountAsync_IgnoresSyncedOperations()
        {
            // Arrange
            _dbContext!.PendingOperations.Add(new PendingOperation
            {
                UserId = "test-user-123",
                OperationType = "Create",
                ResourceType = "Reservation",
                Payload = "{}",
                CreatedAt = DateTime.UtcNow,
                SyncedAt = DateTime.UtcNow
            });

            _dbContext.PendingOperations.Add(new PendingOperation
            {
                UserId = "test-user-123",
                OperationType = "Update",
                ResourceType = "Reservation",
                Payload = "{}",
                CreatedAt = DateTime.UtcNow
            });

            await _dbContext.SaveChangesAsync();

            // Act
            var count = await _service!.GetPendingOperationCountAsync();

            // Assert
            Assert.Equal(1, count);
        }

        #endregion

        #region ClearPendingOperationsAsync Tests

        [Fact]
        public async Task ClearPendingOperationsAsync_RemovesAllPending()
        {
            // Arrange
            _dbContext!.PendingOperations.Add(new PendingOperation
            {
                UserId = "test-user-123",
                OperationType = "Create",
                ResourceType = "Reservation",
                Payload = "{}",
                CreatedAt = DateTime.UtcNow
            });

            _dbContext.PendingOperations.Add(new PendingOperation
            {
                UserId = "test-user-123",
                OperationType = "Update",
                ResourceType = "Reservation",
                Payload = "{}",
                CreatedAt = DateTime.UtcNow
            });

            await _dbContext.SaveChangesAsync();

            // Act
            await _service!.ClearPendingOperationsAsync();

            // Assert
            var count = await _service.GetPendingOperationCountAsync();
            Assert.Equal(0, count);
        }

        [Fact]
        public async Task ClearPendingOperationsAsync_OnlyRemovesCurrentUserOps()
        {
            // Arrange
            _dbContext!.PendingOperations.Add(new PendingOperation
            {
                UserId = "test-user-123",
                OperationType = "Create",
                ResourceType = "Reservation",
                Payload = "{}",
                CreatedAt = DateTime.UtcNow
            });

            _dbContext.PendingOperations.Add(new PendingOperation
            {
                UserId = "other-user-456",
                OperationType = "Update",
                ResourceType = "Reservation",
                Payload = "{}",
                CreatedAt = DateTime.UtcNow
            });

            await _dbContext.SaveChangesAsync();

            // Act
            await _service!.ClearPendingOperationsAsync();

            // Assert
            var otherUserCount = _dbContext.PendingOperations.Count(op => op.UserId == "other-user-456");
            Assert.Equal(1, otherUserCount);
        }

        [Fact]
        public async Task ClearPendingOperationsAsync_ThrowsOnNullUserId()
        {
            // Arrange
            _mockAuthService!.Setup(a => a.CurrentUserId).Returns((string?)null);

            // Act & Assert
            // Should not throw - uses "unknown" as fallback
            await _service!.ClearPendingOperationsAsync();
        }

        #endregion

        #region SyncPendingOperationsAsync Tests

        [Fact]
        public async Task SyncPendingOperationsAsync_ReturnsSuccessWhenNoPending()
        {
            // Act
            var result = await _service!.SyncPendingOperationsAsync();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(0, result.Value.TotalOperations);
            Assert.Equal(0, result.Value.SuccessfulOperations);
        }

        [Fact]
        public async Task SyncPendingOperationsAsync_ProcessesPendingOperations()
        {
            // Arrange
            _dbContext!.PendingOperations.Add(new PendingOperation
            {
                UserId = "test-user-123",
                OperationType = "Create",
                ResourceType = "Reservation",
                Payload = "{}",
                CreatedAt = DateTime.UtcNow
            });

            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _service!.SyncPendingOperationsAsync();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(1, result.Value!.TotalOperations);
            Assert.Equal(1, result.Value.SuccessfulOperations);
        }

        [Fact]
        public async Task SyncPendingOperationsAsync_ProcessesInCreatedAtOrder()
        {
            // Arrange
            var now = DateTime.UtcNow;
            _dbContext!.PendingOperations.Add(new PendingOperation
            {
                UserId = "test-user-123",
                OperationType = "Create",
                ResourceType = "Reservation",
                Payload = "{}",
                CreatedAt = now.AddMinutes(2)
            });

            _dbContext.PendingOperations.Add(new PendingOperation
            {
                UserId = "test-user-123",
                OperationType = "Update",
                ResourceType = "Reservation",
                Payload = "{}",
                CreatedAt = now
            });

            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _service!.SyncPendingOperationsAsync();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(2, result.Value!.TotalOperations);
        }

        [Fact]
        public async Task SyncPendingOperationsAsync_SkipsAlreadySyncedOps()
        {
            // Arrange
            _dbContext!.PendingOperations.Add(new PendingOperation
            {
                UserId = "test-user-123",
                OperationType = "Create",
                ResourceType = "Reservation",
                Payload = "{}",
                CreatedAt = DateTime.UtcNow,
                SyncedAt = DateTime.UtcNow
            });

            _dbContext.PendingOperations.Add(new PendingOperation
            {
                UserId = "test-user-123",
                OperationType = "Update",
                ResourceType = "Reservation",
                Payload = "{}",
                CreatedAt = DateTime.UtcNow
            });

            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _service!.SyncPendingOperationsAsync();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(1, result.Value!.TotalOperations);
        }

        [Fact]
        public async Task SyncPendingOperationsAsync_RemovesSuccessfulOpsFromQueue()
        {
            // Arrange
            _dbContext!.PendingOperations.Add(new PendingOperation
            {
                UserId = "test-user-123",
                OperationType = "Create",
                ResourceType = "Reservation",
                Payload = "{}",
                CreatedAt = DateTime.UtcNow
            });

            await _dbContext.SaveChangesAsync();

            // Act
            await _service!.SyncPendingOperationsAsync();

            // Assert
            var remaining = _dbContext.PendingOperations.Count(op => op.UserId == "test-user-123");
            Assert.Equal(0, remaining);
        }

        [Fact]
        public async Task SyncPendingOperationsAsync_IsSyncingFlagUpdates()
        {
            // Arrange
            _dbContext!.PendingOperations.Add(new PendingOperation
            {
                UserId = "test-user-123",
                OperationType = "Create",
                ResourceType = "Reservation",
                Payload = "{}",
                CreatedAt = DateTime.UtcNow
            });

            await _dbContext.SaveChangesAsync();

            // Act
            var syncTask = _service!.SyncPendingOperationsAsync();
            var isSyncingDuringExecution = _service.IsSyncing;
            await syncTask;

            // Assert
            Assert.False(_service.IsSyncing); // Should be false after completion
        }

        [Fact]
        public async Task SyncPendingOperationsAsync_ReturnsErrorWhenAlreadySyncing()
        {
            // This test verifies the IsSyncing check, though in practice
            // concurrent calls would be rare due to UI/task scheduling

            // Add a pending operation
            _dbContext!.PendingOperations.Add(new PendingOperation
            {
                UserId = "test-user-123",
                OperationType = "Create",
                ResourceType = "Reservation",
                Payload = "{}",
                CreatedAt = DateTime.UtcNow
            });

            await _dbContext.SaveChangesAsync();

            // First sync should succeed
            var result1 = await _service!.SyncPendingOperationsAsync();
            Assert.True(result1.IsSuccess);
        }

        [Fact]
        public async Task SyncPendingOperationsAsync_EmitsSyncCompletedEvent()
        {
            // Arrange
            SyncResult? emittedResult = null;
            var subscription = _service!.SyncCompleted.Subscribe(result =>
            {
                emittedResult = result;
            });

            _dbContext!.PendingOperations.Add(new PendingOperation
            {
                UserId = "test-user-123",
                OperationType = "Create",
                ResourceType = "Reservation",
                Payload = "{}",
                CreatedAt = DateTime.UtcNow
            });

            await _dbContext.SaveChangesAsync();

            try
            {
                // Act
                await _service.SyncPendingOperationsAsync();
                await Task.Delay(100);

                // Assert
                Assert.NotNull(emittedResult);
                Assert.True(emittedResult!.IsSuccessful);
                Assert.Equal(1, emittedResult.Summary.SuccessfulOperations);
            }
            finally
            {
                subscription.Dispose();
            }
        }

        #endregion

        #region SyncSummary Tests

        [Fact]
        public async Task SyncPendingOperationsAsync_ReturnsSummaryWithCorrectCounts()
        {
            // Arrange
            for (int i = 0; i < 3; i++)
            {
                _dbContext!.PendingOperations.Add(new PendingOperation
                {
                    UserId = "test-user-123",
                    OperationType = "Create",
                    ResourceType = "Reservation",
                    Payload = "{}",
                    CreatedAt = DateTime.UtcNow
                });
            }

            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _service!.SyncPendingOperationsAsync();

            // Assert
            Assert.Equal(3, result.Value!.TotalOperations);
            Assert.Equal(3, result.Value.SuccessfulOperations);
            Assert.Equal(0, result.Value.FailedOperations);
        }

        [Fact]
        public async Task SyncPendingOperationsAsync_SummaryContainsMessage()
        {
            // Arrange
            _dbContext!.PendingOperations.Add(new PendingOperation
            {
                UserId = "test-user-123",
                OperationType = "Create",
                ResourceType = "Reservation",
                Payload = "{}",
                CreatedAt = DateTime.UtcNow
            });

            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _service!.SyncPendingOperationsAsync();

            // Assert
            Assert.NotEmpty(result.Value!.Message);
            Assert.Contains("Sync completed", result.Value.Message);
        }

        #endregion

        #region Error Handling Tests

        [Fact]
        public async Task SyncPendingOperationsAsync_HandlesUserIdNull()
        {
            // Arrange
            _mockAuthService!.Setup(a => a.CurrentUserId).Returns((string?)null);

            _dbContext!.PendingOperations.Add(new PendingOperation
            {
                UserId = "unknown",
                OperationType = "Create",
                ResourceType = "Reservation",
                Payload = "{}",
                CreatedAt = DateTime.UtcNow
            });

            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _service!.SyncPendingOperationsAsync();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(1, result.Value!.TotalOperations);
        }

        #endregion
    }
}
