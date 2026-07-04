using Moq;
using Xunit;
using NaarNoor.Desktop.Common.DTOs;
using NaarNoor.Desktop.Common.Services;
using NaarNoor.Desktop.Common.Services.ApiClients;

namespace NaarNoor.Desktop.Tests.Services
{
    public class ReservationServiceTests
    {
        private readonly Mock<IReservationApiClient> _mockApiClient;
        private readonly Mock<ICacheService> _mockCacheService;
        private readonly ReservationService _service;

        public ReservationServiceTests()
        {
            _mockApiClient = new Mock<IReservationApiClient>();
            _mockCacheService = new Mock<ICacheService>();
            _service = new ReservationService(_mockApiClient.Object, _mockCacheService.Object);
        }

        [Fact]
        public async Task GetReservationsAsync_WithCachedData_ReturnsCachedResult()
        {
            // Arrange
            var expected = new List<ReservationDto>
            {
                new() { Id = "1", CustomerName = "John", PartySize = 4, BookingTime = DateTime.UtcNow, Status = "confirmed" }
            };
            
            _mockCacheService
                .Setup(x => x.GetAsync<List<ReservationDto>>(It.IsAny<string>()))
                .ReturnsAsync(expected);

            // Act
            var result = await _service.GetReservationsAsync();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(expected, result.Value);
            _mockApiClient.Verify(x => x.GetReservationsAsync(It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>()), Times.Never);
        }

        [Fact]
        public async Task GetReservationsAsync_WithoutCache_CallsApiAndCaches()
        {
            // Arrange
            var expected = new List<ReservationDto>
            {
                new() { Id = "1", CustomerName = "John", PartySize = 4, BookingTime = DateTime.UtcNow, Status = "confirmed" }
            };
            
            var pagedResponse = new PagedResponse<ReservationDto>
            {
                Data = expected,
                Page = 1,
                PageSize = 1000,
                Total = 1
            };

            _mockCacheService
                .Setup(x => x.GetAsync<List<ReservationDto>>(It.IsAny<string>()))
                .ReturnsAsync((List<ReservationDto>?)null);

            _mockApiClient
                .Setup(x => x.GetReservationsAsync(It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
                .ReturnsAsync(pagedResponse);

            // Act
            var result = await _service.GetReservationsAsync();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(expected, result.Value);
            _mockCacheService.Verify(
                x => x.SetAsync(It.IsAny<string>(), expected, It.IsAny<TimeSpan>()),
                Times.Once);
        }

        [Fact]
        public async Task GetReservationByIdAsync_WithValidId_ReturnsCachedOrApiResult()
        {
            // Arrange
            var reservationId = "123";
            var expected = new ReservationDto 
            { 
                Id = reservationId, 
                CustomerName = "John", 
                PartySize = 4, 
                BookingTime = DateTime.UtcNow, 
                Status = "confirmed" 
            };

            _mockCacheService
                .Setup(x => x.GetAsync<ReservationDto>(It.IsAny<string>()))
                .ReturnsAsync((ReservationDto?)null);

            _mockApiClient
                .Setup(x => x.GetReservationByIdAsync(reservationId))
                .ReturnsAsync(expected);

            // Act
            var result = await _service.GetReservationByIdAsync(reservationId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(expected.Id, result.Value.Id);
        }

        [Fact]
        public async Task GetReservationByIdAsync_WithEmptyId_ReturnsFailure()
        {
            // Act
            var result = await _service.GetReservationByIdAsync("");

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("required", result.Error, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task CreateReservationAsync_WithValidRequest_InvalidatesCache()
        {
            // Arrange
            var request = new CreateReservationRequest
            {
                CustomerName = "Jane",
                PartySize = 2,
                BookingTime = DateTime.UtcNow.AddDays(1),
                CustomerPhone = "555-0123"
            };

            var expected = new ReservationDto
            {
                Id = "new-id",
                CustomerName = "Jane",
                PartySize = 2,
                BookingTime = request.BookingTime,
                Status = "pending"
            };

            _mockApiClient
                .Setup(x => x.CreateReservationAsync(request))
                .ReturnsAsync(expected);

            // Act
            var result = await _service.CreateReservationAsync(request);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(expected.Id, result.Value.Id);
            _mockCacheService.Verify(x => x.InvalidatePattern("reservations:all"), Times.Once);
        }

        [Fact]
        public async Task CreateReservationAsync_WithNullRequest_ReturnsFailure()
        {
            // Act
            var result = await _service.CreateReservationAsync(null!);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("required", result.Error, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task UpdateReservationAsync_WithValidRequest_InvalidatesCache()
        {
            // Arrange
            var reservationId = "123";
            var request = new UpdateReservationRequest
            {
                Status = "completed"
            };

            var expected = new ReservationDto
            {
                Id = reservationId,
                CustomerName = "John",
                PartySize = 4,
                BookingTime = DateTime.UtcNow,
                Status = "completed"
            };

            _mockApiClient
                .Setup(x => x.UpdateReservationAsync(reservationId, request))
                .ReturnsAsync(expected);

            // Act
            var result = await _service.UpdateReservationAsync(reservationId, request);

            // Assert
            Assert.True(result.IsSuccess);
            _mockCacheService.Verify(x => x.InvalidatePattern("reservations:all"), Times.Once);
            _mockCacheService.Verify(x => x.RemoveAsync($"reservations:{reservationId}"), Times.Once);
        }

        [Fact]
        public async Task DeleteReservationAsync_WithValidId_InvalidatesCache()
        {
            // Arrange
            var reservationId = "123";
            var mockResponse = new Mock<Refit.IApiResponse>();
            mockResponse.Setup(x => x.IsSuccessStatusCode).Returns(true);

            _mockApiClient
                .Setup(x => x.DeleteReservationAsync(reservationId))
                .ReturnsAsync(mockResponse.Object);

            // Act
            var result = await _service.DeleteReservationAsync(reservationId);

            // Assert
            Assert.True(result.IsSuccess);
            _mockCacheService.Verify(x => x.InvalidatePattern("reservations:all"), Times.Once);
            _mockCacheService.Verify(x => x.RemoveAsync($"reservations:{reservationId}"), Times.Once);
        }

        [Fact]
        public async Task ReservationUpdates_PublishesNotifications()
        {
            // Arrange
            var request = new CreateReservationRequest
            {
                CustomerName = "Jane",
                PartySize = 2,
                BookingTime = DateTime.UtcNow.AddDays(1)
            };

            var expected = new ReservationDto
            {
                Id = "new-id",
                CustomerName = "Jane",
                PartySize = 2,
                BookingTime = request.BookingTime,
                Status = "pending"
            };

            _mockApiClient
                .Setup(x => x.CreateReservationAsync(request))
                .ReturnsAsync(expected);

            var notifications = new List<ReservationNotification>();
            _service.ReservationUpdates.Subscribe(n => notifications.Add(n));

            // Act
            await _service.CreateReservationAsync(request);

            // Assert
            Assert.Single(notifications);
            Assert.Equal("created", notifications[0].EventType);
            Assert.Equal(expected.Id, notifications[0].ReservationId);
        }
    }
}
