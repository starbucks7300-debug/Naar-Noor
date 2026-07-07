using Xunit;
using Moq;
using NaarNoor.Desktop.Common.Services;
using NaarNoor.Desktop.Common.Services.Implementations;
using NaarNoor.Desktop.Common.Services.Interfaces;

namespace NaarNoor.Desktop.Tests.Services
{
    /// <summary>
    /// Unit tests for SyncService.
    /// The API-first SyncService has no local DB queue — all ops go direct to API server.
    /// </summary>
    public class SyncServiceTests
    {
        private readonly Mock<IAuthenticationService> _mockAuthService;
        private readonly SyncService _service;

        public SyncServiceTests()
        {
            _mockAuthService = new Mock<IAuthenticationService>();
            _mockAuthService.Setup(a => a.CurrentUserId).Returns("test-user-123");
            _service = new SyncService(_mockAuthService.Object);
        }

        [Fact]
        public void Constructor_InitializesWithServices()
        {
            Assert.NotNull(_service);
            Assert.False(_service.IsSyncing);
        }

        [Fact]
        public void Constructor_ThrowsOnNullAuthService()
        {
            Assert.Throws<ArgumentNullException>(() => new SyncService(null!));
        }

        [Fact]
        public void IsSyncing_ReturnsFalseInitially()
        {
            Assert.False(_service.IsSyncing);
        }

        [Fact]
        public void SyncCompleted_ReturnsObservable()
        {
            Assert.NotNull(_service.SyncCompleted);
        }

        [Fact]
        public async Task GetPendingOperationCountAsync_AlwaysReturnsZero()
        {
            var count = await _service.GetPendingOperationCountAsync();
            Assert.Equal(0, count);
        }

        [Fact]
        public async Task ClearPendingOperationsAsync_CompletesSuccessfully()
        {
            await _service.ClearPendingOperationsAsync(); // Should not throw
        }

        [Fact]
        public async Task SyncPendingOperationsAsync_ReturnsSuccessWithZeroOps()
        {
            var result = await _service.SyncPendingOperationsAsync();

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(0, result.Value.TotalOperations);
            Assert.Equal(0, result.Value.SuccessfulOperations);
            Assert.Equal(0, result.Value.FailedOperations);
        }

        [Fact]
        public async Task SyncPendingOperationsAsync_EmitsSyncCompletedEvent()
        {
            SyncResult? emitted = null;
            var sub = _service.SyncCompleted.Subscribe(r => emitted = r);

            try
            {
                await _service.SyncPendingOperationsAsync();
                Assert.NotNull(emitted);
                Assert.True(emitted!.IsSuccessful);
            }
            finally
            {
                sub.Dispose();
            }
        }

        [Fact]
        public async Task SyncPendingOperationsAsync_IsSyncingFalseAfterCompletion()
        {
            await _service.SyncPendingOperationsAsync();
            Assert.False(_service.IsSyncing);
        }
    }
}
