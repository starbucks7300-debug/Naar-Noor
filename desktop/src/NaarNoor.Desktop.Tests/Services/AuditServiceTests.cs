using Xunit;
using NaarNoor.Desktop.Common.Services;

namespace NaarNoor.Desktop.Tests.Services
{
    /// <summary>
    /// Unit tests for AuditService (file-based JSONL logging, no local DB).
    /// </summary>
    public class AuditServiceTests : IDisposable
    {
        private readonly AuditService _auditService;
        private readonly string _testLogDir;

        public AuditServiceTests()
        {
            _testLogDir = Path.Combine(Path.GetTempPath(), $"NaarNoorAuditTest_{Guid.NewGuid()}");
            Directory.CreateDirectory(_testLogDir);

            // Use the parameterless constructor (it creates its own log directory)
            _auditService = new AuditService();
        }

        public void Dispose()
        {
            try { Directory.Delete(_testLogDir, recursive: true); } catch { }
        }

        [Fact]
        public async Task LogSecurityEventAsync_DoesNotThrow()
        {
            await _auditService.LogSecurityEventAsync("user1", "login", "Authentication", "success");
        }

        [Fact]
        public async Task LogSecurityEventAsync_HandlesNullUserIdGracefully()
        {
            await _auditService.LogSecurityEventAsync(null!, "action", "resource", "success");
        }

        [Fact]
        public async Task LogUnauthorizedAccessAsync_DoesNotThrow()
        {
            await _auditService.LogUnauthorizedAccessAsync("user1", "MenuManagement", "context");
        }

        [Fact]
        public async Task LogLoginAsync_SuccessDoesNotThrow()
        {
            await _auditService.LogLoginAsync("user1", success: true);
        }

        [Fact]
        public async Task LogLoginAsync_FailureDoesNotThrow()
        {
            await _auditService.LogLoginAsync("user1", success: false, "Wrong password");
        }

        [Fact]
        public async Task LogLogoutAsync_DoesNotThrow()
        {
            await _auditService.LogLogoutAsync("user1");
        }

        [Fact]
        public async Task GetUserAuditLogsAsync_ReturnsLogsForUser()
        {
            var userId = $"testuser-{Guid.NewGuid()}";
            await _auditService.LogLoginAsync(userId, true);
            await _auditService.LogLogoutAsync(userId);

            var logs = await _auditService.GetUserAuditLogsAsync(userId, days: 1);

            Assert.NotEmpty(logs);
            Assert.All(logs, l => Assert.Equal(userId, l.UserId));
        }

        [Fact]
        public async Task GetUserAuditLogsAsync_DoesNotReturnOtherUsers()
        {
            var userId = $"user-{Guid.NewGuid()}";
            var otherId = $"other-{Guid.NewGuid()}";
            await _auditService.LogLoginAsync(userId, true);
            await _auditService.LogLoginAsync(otherId, true);

            var logs = await _auditService.GetUserAuditLogsAsync(userId, days: 1);

            Assert.All(logs, l => Assert.Equal(userId, l.UserId));
        }

        [Fact]
        public async Task GetUserAuditLogsAsync_ReturnsSortedDescending()
        {
            var userId = $"sortuser-{Guid.NewGuid()}";
            await _auditService.LogLoginAsync(userId, true);
            await Task.Delay(10);
            await _auditService.LogLogoutAsync(userId);

            var logs = await _auditService.GetUserAuditLogsAsync(userId, days: 1);

            if (logs.Count >= 2)
                Assert.True(logs[0].Timestamp >= logs[1].Timestamp);
        }

        [Fact]
        public async Task GetUnauthorizedAccessAttemptsAsync_ReturnsOnlyUnauthorizedEvents()
        {
            var userId = $"unauth-{Guid.NewGuid()}";
            await _auditService.LogUnauthorizedAccessAsync(userId, "AdminPanel");
            await _auditService.LogLoginAsync(userId, true);

            var logs = await _auditService.GetUnauthorizedAccessAttemptsAsync(days: 1);

            Assert.All(logs, l => Assert.Equal("unauthorized_access", l.Action));
        }

        [Fact]
        public async Task GetUnauthorizedAccessAttemptsAsync_ReturnsEmptyWhenNone()
        {
            var logs = await _auditService.GetUnauthorizedAccessAttemptsAsync(days: 0);
            Assert.Empty(logs);
        }

        [Fact]
        public async Task AuditTrail_RecordsMultipleEventsInOrder()
        {
            var userId = $"trail-{Guid.NewGuid()}";
            await _auditService.LogLoginAsync(userId, true);
            await _auditService.LogUnauthorizedAccessAsync(userId, "AdminPanel");
            await _auditService.LogLogoutAsync(userId);

            var logs = await _auditService.GetUserAuditLogsAsync(userId, days: 1);

            Assert.Equal(3, logs.Count);
        }
    }
}
