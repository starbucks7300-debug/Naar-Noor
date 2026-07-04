using Moq;
using Xunit;
using NaarNoor.Desktop.Common.DTOs;
using NaarNoor.Desktop.Common.Services;
using NaarNoor.Desktop.Common.Services.ApiClients;

namespace NaarNoor.Desktop.Tests.Services
{
    public class ChefServiceTests
    {
        private readonly Mock<IChefApiClient> _mockApiClient;
        private readonly Mock<ICacheService> _mockCacheService;
        private readonly ChefService _service;

        public ChefServiceTests()
        {
            _mockApiClient = new Mock<IChefApiClient>();
            _mockCacheService = new Mock<ICacheService>();
            _service = new ChefService(_mockApiClient.Object, _mockCacheService.Object);
        }

        [Fact]
        public async Task GetStaffAsync_WithCachedData_ReturnsCachedResult()
        {
            // Arrange
            var expected = new List<StaffDto>
            {
                new() { Id = "1", Name = "Chef John", Role = "Chef", Status = "available" }
            };
            
            _mockCacheService
                .Setup(x => x.GetAsync<List<StaffDto>>("staff:all"))
                .ReturnsAsync(expected);

            // Act
            var result = await _service.GetStaffAsync();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(expected, result.Value);
            _mockApiClient.Verify(x => x.GetStaffAsync(), Times.Never);
        }

        [Fact]
        public async Task GetStaffAsync_WithoutCache_CallsApiAndCaches()
        {
            // Arrange
            var expected = new List<StaffDto>
            {
                new() { Id = "1", Name = "Chef John", Role = "Chef", Status = "available" }
            };
            
            _mockCacheService
                .Setup(x => x.GetAsync<List<StaffDto>>("staff:all"))
                .ReturnsAsync((List<StaffDto>?)null);

            _mockApiClient
                .Setup(x => x.GetStaffAsync())
                .ReturnsAsync(expected);

            // Act
            var result = await _service.GetStaffAsync();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(expected, result.Value);
            _mockCacheService.Verify(
                x => x.SetAsync("staff:all", expected, It.IsAny<TimeSpan>()),
                Times.Once);
        }

        [Fact]
        public async Task GetStaffByRoleAsync_FiltersStaffByRole()
        {
            // Arrange
            var allStaff = new List<StaffDto>
            {
                new() { Id = "1", Name = "Chef John", Role = "Chef", Status = "available" },
                new() { Id = "2", Name = "Waiter Jane", Role = "Waiter", Status = "busy" },
                new() { Id = "3", Name = "Chef Mike", Role = "Chef", Status = "available" }
            };

            var chefs = allStaff.Where(x => x.Role == "Chef").ToList();

            _mockCacheService
                .Setup(x => x.GetAsync<List<StaffDto>>(It.IsAny<string>()))
                .ReturnsAsync((List<StaffDto>?)null);

            _mockApiClient
                .Setup(x => x.GetStaffAsync())
                .ReturnsAsync(allStaff);

            // Act
            var result = await _service.GetStaffByRoleAsync("Chef");

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(chefs.Count, result.Value.Count);
            Assert.All(result.Value, staff => Assert.Equal("Chef", staff.Role));
        }

        [Fact]
        public async Task GetAvailableStaffAsync_FiltersAvailableStaffOnly()
        {
            // Arrange
            var allStaff = new List<StaffDto>
            {
                new() { Id = "1", Name = "Chef John", Role = "Chef", Status = "available" },
                new() { Id = "2", Name = "Waiter Jane", Role = "Waiter", Status = "busy" },
                new() { Id = "3", Name = "Chef Mike", Role = "Chef", Status = "break" },
                new() { Id = "4", Name = "Waiter Tom", Role = "Waiter", Status = "available" }
            };

            var available = allStaff.Where(x => x.Status == "available").ToList();

            _mockCacheService
                .Setup(x => x.GetAsync<List<StaffDto>>(It.IsAny<string>()))
                .ReturnsAsync((List<StaffDto>?)null);

            _mockApiClient
                .Setup(x => x.GetStaffAsync())
                .ReturnsAsync(allStaff);

            // Act
            var result = await _service.GetAvailableStaffAsync();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(available.Count, result.Value.Count);
            Assert.All(result.Value, staff => Assert.Equal("available", staff.Status));
        }

        [Fact]
        public async Task UpdateStaffStatusAsync_WithValidRequest_InvalidatesCache()
        {
            // Arrange
            var staffId = "1";
            var request = new UpdateStaffStatusRequest { Status = "busy" };

            var expected = new StaffDto
            {
                Id = staffId,
                Name = "Chef John",
                Role = "Chef",
                Status = "busy"
            };

            _mockApiClient
                .Setup(x => x.UpdateStaffStatusAsync(staffId, request))
                .ReturnsAsync(expected);

            // Act
            var result = await _service.UpdateStaffStatusAsync(staffId, request);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("busy", result.Value.Status);
            _mockCacheService.Verify(x => x.InvalidatePattern("staff:all"), Times.Once);
            _mockCacheService.Verify(x => x.InvalidatePattern("staff:role:"), Times.Once);
            _mockCacheService.Verify(x => x.InvalidatePattern("staff:available"), Times.Once);
        }

        [Fact]
        public async Task UpdateStaffStatusAsync_WithEmptyStaffId_ReturnsFailure()
        {
            // Act
            var result = await _service.UpdateStaffStatusAsync("", new UpdateStaffStatusRequest { Status = "busy" });

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("required", result.Error, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task UpdateStaffStatusAsync_WithNullRequest_ReturnsFailure()
        {
            // Act
            var result = await _service.UpdateStaffStatusAsync("1", null!);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("required", result.Error, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task StaffStatusUpdates_PublishesNotifications()
        {
            // Arrange
            var staffId = "1";
            var request = new UpdateStaffStatusRequest { Status = "busy" };

            var expected = new StaffDto
            {
                Id = staffId,
                Name = "Chef John",
                Role = "Chef",
                Status = "busy"
            };

            _mockApiClient
                .Setup(x => x.UpdateStaffStatusAsync(staffId, request))
                .ReturnsAsync(expected);

            var notifications = new List<StaffStatusNotification>();
            _service.StaffStatusUpdates.Subscribe(n => notifications.Add(n));

            // Act
            await _service.UpdateStaffStatusAsync(staffId, request);

            // Assert
            Assert.Single(notifications);
            Assert.Equal(staffId, notifications[0].StaffId);
            Assert.Equal("busy", notifications[0].Status);
            Assert.Equal("Chef John", notifications[0].Name);
        }

        [Fact]
        public async Task GetStaffByRoleAsync_WithEmptyRole_ReturnsFailure()
        {
            // Act
            var result = await _service.GetStaffByRoleAsync("");

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("required", result.Error, StringComparison.OrdinalIgnoreCase);
        }
    }
}
