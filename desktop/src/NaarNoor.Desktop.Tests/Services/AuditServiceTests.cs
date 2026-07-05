using Xunit;
using Microsoft.EntityFrameworkCore;
using NaarNoor.Desktop.Common.Services;
using NaarNoor.Desktop.Common.Data;
using NaarNoor.Desktop.Common.Data.Models;

namespace NaarNoor.Desktop.Tests.Services
{
    /// <summary>
    /// Unit tests for AuditService
    /// Tests audit logging functionality for security events and compliance tracking per REQ-005
    /// </summary>
    public class AuditServiceTests : IAsyncLifetime
    {
        private readonly DatabaseContext _dbContext;
        private readonly AuditService _auditService;

        public AuditServiceTests()
        {
            // Create in-memory SQLite database for testing
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: $"AuditServiceTests-{Guid.NewGuid()}")
                .Options;

            _dbContext = new DatabaseContext(options);
            _auditService = new AuditService(_dbContext);
        }

        public async Task InitializeAsync()
        {
            await _dbContext.Database.EnsureCreatedAsync();
        }

        public async Task DisposeAsync()
        {
            await _dbContext.Database.EnsureDeletedAsync();
            _dbContext.Dispose();
        }

        // ========== LogSecurityEventAsync Tests ==========

        [Fact]
        public async Task LogSecurityEventAsync_SavesEventToDatabase()
        {
            // Arrange
            var userId = "user123";
            var action = "login";
            var resourceType = "Authentication";
            var status = "success";

            // Act
            await _auditService.LogSecurityEventAsync(userId, action, resourceType, status);

            // Assert
            var logged = await _dbContext.AuditLogs
                .Where(a => a.UserId == userId && a.Action == action)
                .FirstOrDefaultAsync();

            Assert.NotNull(logged);
            Assert.Equal(userId, logged.UserId);
            Assert.Equal(action, logged.Action);
            Assert.Equal(resourceType, logged.ResourceType);
            Assert.Equal(status, logged.Status);
        }

        [Fact]
        public async Task LogSecurityEventAsync_IncludesTimestamp()
        {
            // Arrange
            var beforeTime = DateTime.UtcNow;

            // Act
            await _auditService.LogSecurityEventAsync("user1", "action1", "resource1", "success");

            var afterTime = DateTime.UtcNow;

            // Assert
            var logged = await _dbContext.AuditLogs.FirstAsync();
            Assert.True(logged.Timestamp >= beforeTime && logged.Timestamp <= afterTime);
        }

        [Fact]
        public async Task LogSecurityEventAsync_IncludesResourceIdAndDetails()
        {
            // Arrange
            var resourceId = "resource-456";
            var details = "Additional context information";

            // Act
            await _auditService.LogSecurityEventAsync(
                "user1", "create", "MenuItem", "success", resourceId, details
            );

            // Assert
            var logged = await _dbContext.AuditLogs.FirstAsync();
            Assert.Equal(resourceId, logged.ResourceId);
            Assert.Equal(details, logged.Details);
        }

        [Fact]
        public async Task LogSecurityEventAsync_HandlesMissingResourceId()
        {
            // Arrange & Act
            await _auditService.LogSecurityEventAsync(
                "user1", "delete", "Reservation", "failure", null, "Item not found"
            );

            // Assert
            var logged = await _dbContext.AuditLogs.FirstAsync();
            Assert.Null(logged.ResourceId);
            Assert.NotNull(logged.Details);
        }

        // ========== LogUnauthorizedAccessAsync Tests ==========

        [Fact]
        public async Task LogUnauthorizedAccessAsync_LogsUnauthorizedAccessEvent()
        {
            // Arrange
            var userId = "user123";
            var feature = "MenuManagement";

            // Act
            await _auditService.LogUnauthorizedAccessAsync(userId, feature);

            // Assert
            var logged = await _dbContext.AuditLogs
                .Where(a => a.UserId == userId && a.Action == "unauthorized_access")
                .FirstOrDefaultAsync();

            Assert.NotNull(logged);
            Assert.Equal("Feature", logged.ResourceType);
            Assert.Equal("failure", logged.Status);
            Assert.Equal(feature, logged.ResourceId);
        }

        [Fact]
        public async Task LogUnauthorizedAccessAsync_IncludesContextInDetails()
        {
            // Arrange
            var context = "EditMenuItem456";

            // Act
            await _auditService.LogUnauthorizedAccessAsync("user1", "MenuManagement", context);

            // Assert
            var logged = await _dbContext.AuditLogs.FirstAsync();
            Assert.Contains(context, logged.Details!);
            Assert.Contains("MenuManagement", logged.Details!);
        }

        // ========== LogLoginAsync Tests ==========

        [Fact]
        public async Task LogLoginAsync_LogsSuccessfulLogin()
        {
            // Arrange
            var userId = "user123";

            // Act
            await _auditService.LogLoginAsync(userId, success: true);

            // Assert
            var logged = await _dbContext.AuditLogs.FirstAsync();
            Assert.Equal("login", logged.Action);
            Assert.Equal("success", logged.Status);
        }

        [Fact]
        public async Task LogLoginAsync_LogsFailedLogin()
        {
            // Arrange
            var userId = "user123";
            var details = "Invalid password";

            // Act
            await _auditService.LogLoginAsync(userId, success: false, details);

            // Assert
            var logged = await _dbContext.AuditLogs.FirstAsync();
            Assert.Equal("login", logged.Action);
            Assert.Equal("failure", logged.Status);
            Assert.Equal(details, logged.Details);
        }

        // ========== LogLogoutAsync Tests ==========

        [Fact]
        public async Task LogLogoutAsync_LogsLogoutEvent()
        {
            // Arrange
            var userId = "user123";

            // Act
            await _auditService.LogLogoutAsync(userId);

            // Assert
            var logged = await _dbContext.AuditLogs.FirstAsync();
            Assert.Equal("logout", logged.Action);
            Assert.Equal("success", logged.Status);
        }

        // ========== GetUserAuditLogsAsync Tests ==========

        [Fact]
        public async Task GetUserAuditLogsAsync_ReturnsLogsForUser()
        {
            // Arrange
            var userId = "user123";
            var otherUserId = "user456";

            await _auditService.LogLoginAsync(userId, true);
            await _auditService.LogLoginAsync(otherUserId, true);
            await _auditService.LogLogoutAsync(userId);

            // Act
            var logs = await _auditService.GetUserAuditLogsAsync(userId);

            // Assert
            Assert.NotEmpty(logs);
            Assert.All(logs, log => Assert.Equal(userId, log.UserId));
            Assert.Equal(2, logs.Count(l => l.UserId == userId));
        }

        [Fact]
        public async Task GetUserAuditLogsAsync_ReturnsLogsWithinTimeRange()
        {
            // Arrange
            var userId = "user123";
            var now = DateTime.UtcNow;

            // Manually add old log (before 90 days ago)
            var oldLog = new AuditLog
            {
                UserId = userId,
                Action = "old_action",
                ResourceType = "Test",
                Status = "success",
                Timestamp = now.AddDays(-100)
            };
            _dbContext.AuditLogs.Add(oldLog);

            // Add recent log
            await _auditService.LogLoginAsync(userId, true);
            await _dbContext.SaveChangesAsync();

            // Act
            var logs = await _auditService.GetUserAuditLogsAsync(userId, days: 90);

            // Assert
            Assert.DoesNotContain(oldLog, logs.Cast<object>());
            Assert.Single(logs); // Only the recent one
        }

        [Fact]
        public async Task GetUserAuditLogsAsync_ReturnsEmptyWhenNoLogsForUser()
        {
            // Arrange
            await _auditService.LogLoginAsync("user123", true);

            // Act
            var logs = await _auditService.GetUserAuditLogsAsync("user999");

            // Assert
            Assert.Empty(logs);
        }

        [Fact]
        public async Task GetUserAuditLogsAsync_ReturnsSortedByTimestampDescending()
        {
            // Arrange
            var userId = "user123";
            var now = DateTime.UtcNow;

            // Add multiple logs with different timestamps
            for (int i = 0; i < 3; i++)
            {
                var log = new AuditLog
                {
                    UserId = userId,
                    Action = $"action{i}",
                    ResourceType = "Test",
                    Status = "success",
                    Timestamp = now.AddSeconds(-i * 10)
                };
                _dbContext.AuditLogs.Add(log);
            }
            await _dbContext.SaveChangesAsync();

            // Act
            var logs = await _auditService.GetUserAuditLogsAsync(userId);

            // Assert
            Assert.True(logs[0].Timestamp >= logs[1].Timestamp);
            Assert.True(logs[1].Timestamp >= logs[2].Timestamp);
        }

        // ========== GetUnauthorizedAccessAttemptsAsync Tests ==========

        [Fact]
        public async Task GetUnauthorizedAccessAttemptsAsync_ReturnsUnauthorizedAccessLogs()
        {
            // Arrange
            await _auditService.LogUnauthorizedAccessAsync("user1", "MenuManagement");
            await _auditService.LogUnauthorizedAccessAsync("user2", "ReservationManagement");
            await _auditService.LogLoginAsync("user3", true);

            // Act
            var logs = await _auditService.GetUnauthorizedAccessAttemptsAsync();

            // Assert
            Assert.Equal(2, logs.Count);
            Assert.All(logs, log => Assert.Equal("unauthorized_access", log.Action));
            Assert.DoesNotContain(logs, l => l.Action == "login");
        }

        [Fact]
        public async Task GetUnauthorizedAccessAttemptsAsync_ReturnsAttemptsWithinTimeRange()
        {
            // Arrange
            var now = DateTime.UtcNow;

            // Add old unauthorized access attempt
            var oldLog = new AuditLog
            {
                UserId = "user1",
                Action = "unauthorized_access",
                ResourceType = "Feature",
                Status = "failure",
                Timestamp = now.AddDays(-40)
            };
            _dbContext.AuditLogs.Add(oldLog);

            // Add recent unauthorized access attempt
            await _auditService.LogUnauthorizedAccessAsync("user2", "MenuManagement");
            await _dbContext.SaveChangesAsync();

            // Act
            var logs = await _auditService.GetUnauthorizedAccessAttemptsAsync(days: 30);

            // Assert
            Assert.DoesNotContain(oldLog, logs.Cast<object>());
            Assert.Single(logs);
        }

        [Fact]
        public async Task GetUnauthorizedAccessAttemptsAsync_ReturnsEmptyWhenNoAttempts()
        {
            // Arrange
            await _auditService.LogLoginAsync("user1", true);

            // Act
            var logs = await _auditService.GetUnauthorizedAccessAttemptsAsync();

            // Assert
            Assert.Empty(logs);
        }

        // ========== Error Handling Tests ==========

        [Fact]
        public async Task LogSecurityEventAsync_HandlesNullUserIdGracefully()
        {
            // Arrange & Act - Should not throw
            await _auditService.LogSecurityEventAsync(null!, "action", "resource", "success");

            // Assert - Default "unknown" should be used
            var logged = await _dbContext.AuditLogs.FirstAsync();
            Assert.Equal("unknown", logged.UserId);
        }

        [Fact]
        public async Task LogSecurityEventAsync_HandlesEmptyStringsGracefully()
        {
            // Arrange & Act - Should not throw
            await _auditService.LogSecurityEventAsync("user1", "", "", "");

            // Assert
            var logged = await _dbContext.AuditLogs.FirstAsync();
            Assert.NotNull(logged);
        }

        // ========== Integration Tests ==========

        [Fact]
        public async Task AuditTrail_CreatesComprehensiveSecurityLog()
        {
            // Arrange & Act - Simulate a series of security events
            await _auditService.LogLoginAsync("user123", true);
            await _auditService.LogUnauthorizedAccessAsync("user123", "AdminPanel", "Attempted to access admin features");
            await _auditService.LogSecurityEventAsync(
                "user123", "create", "MenuItem", "success", "menu-item-789", "Menu item created"
            );
            await _auditService.LogLogoutAsync("user123");

            // Assert - All events should be logged
            var logs = await _auditService.GetUserAuditLogsAsync("user123", days: 1);
            
            Assert.Equal(4, logs.Count);
            Assert.Single(logs, l => l.Action == "login" && l.Status == "success");
            Assert.Single(logs, l => l.Action == "unauthorized_access" && l.Status == "failure");
            Assert.Single(logs, l => l.Action == "create" && l.ResourceType == "MenuItem");
            Assert.Single(logs, l => l.Action == "logout");
        }

        [Fact]
        public async Task AuditTrail_SeparatesUserLogs()
        {
            // Arrange & Act
            await _auditService.LogLoginAsync("user1", true);
            await _auditService.LogLoginAsync("user2", true);
            await _auditService.LogLoginAsync("user1", true);

            // Assert
            var user1Logs = await _auditService.GetUserAuditLogsAsync("user1");
            var user2Logs = await _auditService.GetUserAuditLogsAsync("user2");

            Assert.Equal(2, user1Logs.Count);
            Assert.Single(user2Logs);
        }
    }
}
