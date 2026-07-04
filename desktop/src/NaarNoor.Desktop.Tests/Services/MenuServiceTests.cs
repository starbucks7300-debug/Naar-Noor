using Moq;
using Xunit;
using NaarNoor.Desktop.Common.DTOs;
using NaarNoor.Desktop.Common.Services;
using NaarNoor.Desktop.Common.Services.ApiClients;

namespace NaarNoor.Desktop.Tests.Services
{
    public class MenuServiceTests
    {
        private readonly Mock<IMenuApiClient> _mockApiClient;
        private readonly Mock<ICacheService> _mockCacheService;
        private readonly MenuService _service;

        public MenuServiceTests()
        {
            _mockApiClient = new Mock<IMenuApiClient>();
            _mockCacheService = new Mock<ICacheService>();
            _service = new MenuService(_mockApiClient.Object, _mockCacheService.Object);
        }

        [Fact]
        public async Task GetMenuItemsAsync_WithCachedData_ReturnsCachedResult()
        {
            // Arrange
            var expected = new List<MenuItemDto>
            {
                new() { Id = "1", NameEn = "Pasta", NameAr = "معكرونة", Category = "Main", Price = 12.99m, IsAvailable = true }
            };
            
            _mockCacheService
                .Setup(x => x.GetAsync<List<MenuItemDto>>("menu:all"))
                .ReturnsAsync(expected);

            // Act
            var result = await _service.GetMenuItemsAsync();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(expected, result.Value);
            _mockApiClient.Verify(x => x.GetMenuItemsAsync(), Times.Never);
        }

        [Fact]
        public async Task GetMenuItemsAsync_WithoutCache_CallsApiAndCaches()
        {
            // Arrange
            var expected = new List<MenuItemDto>
            {
                new() { Id = "1", NameEn = "Pasta", NameAr = "معكرونة", Category = "Main", Price = 12.99m, IsAvailable = true }
            };
            
            _mockCacheService
                .Setup(x => x.GetAsync<List<MenuItemDto>>("menu:all"))
                .ReturnsAsync((List<MenuItemDto>?)null);

            _mockApiClient
                .Setup(x => x.GetMenuItemsAsync())
                .ReturnsAsync(expected);

            // Act
            var result = await _service.GetMenuItemsAsync();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(expected, result.Value);
            _mockCacheService.Verify(
                x => x.SetAsync("menu:all", expected, It.IsAny<TimeSpan>()),
                Times.Once);
        }

        [Fact]
        public async Task GetMenuItemByIdAsync_WithValidId_ReturnsCachedOrApiResult()
        {
            // Arrange
            var itemId = "pasta-001";
            var expected = new MenuItemDto 
            { 
                Id = itemId, 
                NameEn = "Pasta", 
                NameAr = "معكرونة",
                Category = "Main", 
                Price = 12.99m, 
                IsAvailable = true 
            };

            _mockCacheService
                .Setup(x => x.GetAsync<MenuItemDto>(It.IsAny<string>()))
                .ReturnsAsync((MenuItemDto?)null);

            _mockApiClient
                .Setup(x => x.GetMenuItemByIdAsync(itemId))
                .ReturnsAsync(expected);

            // Act
            var result = await _service.GetMenuItemByIdAsync(itemId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(expected.Id, result.Value.Id);
        }

        [Fact]
        public async Task CreateMenuItemAsync_WithValidRequest_InvalidatesCache()
        {
            // Arrange
            var request = new CreateMenuItemRequest
            {
                NameEn = "Pizza",
                NameAr = "بيتزا",
                Category = "Main",
                Price = 15.99m,
                IsAvailable = true
            };

            var expected = new MenuItemDto
            {
                Id = "pizza-001",
                NameEn = "Pizza",
                NameAr = "بيتزا",
                Category = "Main",
                Price = 15.99m,
                IsAvailable = true
            };

            _mockApiClient
                .Setup(x => x.CreateMenuItemAsync(request))
                .ReturnsAsync(expected);

            // Act
            var result = await _service.CreateMenuItemAsync(request);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(expected.Id, result.Value.Id);
            _mockCacheService.Verify(x => x.InvalidatePattern("menu:all"), Times.Once);
            _mockCacheService.Verify(x => x.InvalidatePattern("menu:category:"), Times.Once);
            _mockCacheService.Verify(x => x.InvalidatePattern("menu:available"), Times.Once);
        }

        [Fact]
        public async Task UpdateMenuItemAsync_WithValidRequest_InvalidatesCache()
        {
            // Arrange
            var itemId = "pasta-001";
            var request = new UpdateMenuItemRequest
            {
                Price = 14.99m
            };

            var expected = new MenuItemDto
            {
                Id = itemId,
                NameEn = "Pasta",
                NameAr = "معكرونة",
                Category = "Main",
                Price = 14.99m,
                IsAvailable = true
            };

            _mockApiClient
                .Setup(x => x.UpdateMenuItemAsync(itemId, request))
                .ReturnsAsync(expected);

            // Act
            var result = await _service.UpdateMenuItemAsync(itemId, request);

            // Assert
            Assert.True(result.IsSuccess);
            _mockCacheService.Verify(x => x.InvalidatePattern("menu:all"), Times.Once);
        }

        [Fact]
        public async Task DeleteMenuItemAsync_WithValidId_InvalidatesCache()
        {
            // Arrange
            var itemId = "pasta-001";
            var mockResponse = new Mock<Refit.IApiResponse>();
            mockResponse.Setup(x => x.IsSuccessStatusCode).Returns(true);

            _mockApiClient
                .Setup(x => x.DeleteMenuItemAsync(itemId))
                .ReturnsAsync(mockResponse.Object);

            // Act
            var result = await _service.DeleteMenuItemAsync(itemId);

            // Assert
            Assert.True(result.IsSuccess);
            _mockCacheService.Verify(x => x.InvalidatePattern("menu:all"), Times.Once);
        }

        [Fact]
        public async Task GetMenuItemsByCategoryAsync_FiltersByCategoryCorrectly()
        {
            // Arrange
            var allItems = new List<MenuItemDto>
            {
                new() { Id = "1", NameEn = "Pasta", NameAr = "معكرونة", Category = "Main", Price = 12.99m, IsAvailable = true },
                new() { Id = "2", NameEn = "Pizza", NameAr = "بيتزا", Category = "Main", Price = 15.99m, IsAvailable = true },
                new() { Id = "3", NameEn = "Salad", NameAr = "سلطة", Category = "Appetizer", Price = 8.99m, IsAvailable = true }
            };

            var mainItems = allItems.Where(x => x.Category == "Main").ToList();

            _mockCacheService
                .Setup(x => x.GetAsync<List<MenuItemDto>>(It.IsAny<string>()))
                .ReturnsAsync((List<MenuItemDto>?)null);

            _mockApiClient
                .Setup(x => x.GetMenuItemsAsync())
                .ReturnsAsync(allItems);

            // Act
            var result = await _service.GetMenuItemsByCategoryAsync("Main");

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(mainItems.Count, result.Value.Count);
            Assert.All(result.Value, item => Assert.Equal("Main", item.Category));
        }

        [Fact]
        public async Task GetAvailableMenuItemsAsync_FiltersAvailableOnly()
        {
            // Arrange
            var allItems = new List<MenuItemDto>
            {
                new() { Id = "1", NameEn = "Pasta", NameAr = "معكرونة", Category = "Main", Price = 12.99m, IsAvailable = true },
                new() { Id = "2", NameEn = "Pizza", NameAr = "بيتزا", Category = "Main", Price = 15.99m, IsAvailable = false },
                new() { Id = "3", NameEn = "Salad", NameAr = "سلطة", Category = "Appetizer", Price = 8.99m, IsAvailable = true }
            };

            var availableItems = allItems.Where(x => x.IsAvailable).ToList();

            _mockCacheService
                .Setup(x => x.GetAsync<List<MenuItemDto>>(It.IsAny<string>()))
                .ReturnsAsync((List<MenuItemDto>?)null);

            _mockApiClient
                .Setup(x => x.GetMenuItemsAsync())
                .ReturnsAsync(allItems);

            // Act
            var result = await _service.GetAvailableMenuItemsAsync();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(availableItems.Count, result.Value.Count);
            Assert.All(result.Value, item => Assert.True(item.IsAvailable));
        }
    }
}
