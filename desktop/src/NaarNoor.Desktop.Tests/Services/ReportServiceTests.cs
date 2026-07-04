using Moq;
using Xunit;
using NaarNoor.Desktop.Common.DTOs;
using NaarNoor.Desktop.Common.Services;
using NaarNoor.Desktop.Common.Services.ApiClients;
using NaarNoor.Desktop.Common.Utilities;

namespace NaarNoor.Desktop.Tests.Services
{
    public class ReportServiceTests
    {
        private readonly Mock<IReportApiClient> _mockApiClient;
        private readonly Mock<IReservationService> _mockReservationService;
        private readonly Mock<ICacheService> _mockCacheService;
        private readonly ReportService _service;

        public ReportServiceTests()
        {
            _mockApiClient = new Mock<IReportApiClient>();
            _mockReservationService = new Mock<IReservationService>();
            _mockCacheService = new Mock<ICacheService>();
            _service = new ReportService(_mockApiClient.Object, _mockReservationService.Object, _mockCacheService.Object);
        }

        [Fact]
        public async Task GetRevenueAsync_WithCachedData_ReturnsCachedResult()
        {
            // Arrange
            var expected = new RevenueDto
            {
                TodayRevenue = 500m,
                WeekRevenue = 3500m,
                MonthRevenue = 15000m,
                YearRevenue = 180000m,
                AveragePerOrder = 45.50m
            };
            
            _mockCacheService
                .Setup(x => x.GetAsync<RevenueDto>(It.IsAny<string>()))
                .ReturnsAsync(expected);

            // Act
            var result = await _service.GetRevenueAsync();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(expected.TodayRevenue, result.Value.TodayRevenue);
            _mockApiClient.Verify(x => x.GetRevenueAsync(It.IsAny<DateTime?>(), It.IsAny<DateTime?>()), Times.Never);
        }

        [Fact]
        public async Task GetRevenueAsync_WithoutCache_CallsApiAndCaches()
        {
            // Arrange
            var expected = new RevenueDto
            {
                TodayRevenue = 500m,
                WeekRevenue = 3500m,
                MonthRevenue = 15000m,
                YearRevenue = 180000m,
                AveragePerOrder = 45.50m
            };
            
            _mockCacheService
                .Setup(x => x.GetAsync<RevenueDto>(It.IsAny<string>()))
                .ReturnsAsync((RevenueDto?)null);

            _mockApiClient
                .Setup(x => x.GetRevenueAsync(It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
                .ReturnsAsync(expected);

            // Act
            var result = await _service.GetRevenueAsync();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(expected.TodayRevenue, result.Value.TodayRevenue);
            _mockCacheService.Verify(
                x => x.SetAsync(It.IsAny<string>(), expected, It.IsAny<TimeSpan>()),
                Times.Once);
        }

        [Fact]
        public async Task GetOrderStatsAsync_WithCachedData_ReturnsCachedResult()
        {
            // Arrange
            var expected = new OrderStatsDto
            {
                TotalOrders = 100,
                CompletedOrders = 95,
                PendingOrders = 3,
                CancelledOrders = 2,
                AveragePreparationTime = 25.5
            };
            
            _mockCacheService
                .Setup(x => x.GetAsync<OrderStatsDto>(It.IsAny<string>()))
                .ReturnsAsync(expected);

            // Act
            var result = await _service.GetOrderStatsAsync();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(expected.TotalOrders, result.Value.TotalOrders);
        }

        [Fact]
        public async Task GetOrderStatsAsync_WithoutCache_CallsApiAndCaches()
        {
            // Arrange
            var expected = new OrderStatsDto
            {
                TotalOrders = 100,
                CompletedOrders = 95,
                PendingOrders = 3,
                CancelledOrders = 2,
                AveragePreparationTime = 25.5
            };
            
            _mockCacheService
                .Setup(x => x.GetAsync<OrderStatsDto>(It.IsAny<string>()))
                .ReturnsAsync((OrderStatsDto?)null);

            _mockApiClient
                .Setup(x => x.GetOrderStatsAsync(It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
                .ReturnsAsync(expected);

            // Act
            var result = await _service.GetOrderStatsAsync();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(expected.TotalOrders, result.Value.TotalOrders);
            _mockCacheService.Verify(
                x => x.SetAsync(It.IsAny<string>(), expected, It.IsAny<TimeSpan>()),
                Times.Once);
        }

        [Fact]
        public async Task GetReportAsync_WithValidReportType_ReturnsReport()
        {
            // Arrange
            var reportType = "sales";
            var expected = new ReportDto
            {
                ReportType = reportType,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(1),
                Data = new Dictionary<string, object> { { "total", 5000 } }
            };
            
            _mockCacheService
                .Setup(x => x.GetAsync<ReportDto>(It.IsAny<string>()))
                .ReturnsAsync((ReportDto?)null);

            _mockApiClient
                .Setup(x => x.GetReportAsync(reportType, It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
                .ReturnsAsync(expected);

            // Act
            var result = await _service.GetReportAsync(reportType);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(reportType, result.Value.ReportType);
        }

        [Fact]
        public async Task GetReportAsync_WithEmptyReportType_ReturnsFailure()
        {
            // Act
            var result = await _service.GetReportAsync("");

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("required", result.Error, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task GetDashboardSummaryAsync_AggregatesMetrics()
        {
            // Arrange
            var revenue = new RevenueDto
            {
                TodayRevenue = 500m,
                WeekRevenue = 3500m,
                MonthRevenue = 15000m,
                YearRevenue = 180000m,
                AveragePerOrder = 45.50m
            };

            var orderStats = new OrderStatsDto
            {
                TotalOrders = 100,
                CompletedOrders = 95,
                PendingOrders = 3,
                CancelledOrders = 2,
                AveragePreparationTime = 25.5
            };

            var reservations = new List<ReservationDto>
            {
                new() { Id = "1", CustomerName = "John", PartySize = 2, BookingTime = DateTime.UtcNow, Status = "confirmed" },
                new() { Id = "2", CustomerName = "Jane", PartySize = 4, BookingTime = DateTime.UtcNow, Status = "pending" },
                new() { Id = "3", CustomerName = "Bob", PartySize = 3, BookingTime = DateTime.UtcNow, Status = "completed" }
            };

            _mockCacheService
                .Setup(x => x.GetAsync<DashboardSummary>(It.IsAny<string>()))
                .ReturnsAsync((DashboardSummary?)null);

            _mockCacheService
                .Setup(x => x.GetAsync<RevenueDto>(It.IsAny<string>()))
                .ReturnsAsync((RevenueDto?)null);

            _mockCacheService
                .Setup(x => x.GetAsync<OrderStatsDto>(It.IsAny<string>()))
                .ReturnsAsync((OrderStatsDto?)null);

            _mockApiClient
                .Setup(x => x.GetRevenueAsync(It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
                .ReturnsAsync(revenue);

            _mockApiClient
                .Setup(x => x.GetOrderStatsAsync(It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
                .ReturnsAsync(orderStats);

            _mockReservationService
                .Setup(x => x.GetReservationsAsync(It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
                .ReturnsAsync(Result<List<ReservationDto>>.Success(reservations));

            // Act
            var result = await _service.GetDashboardSummaryAsync();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(revenue.TodayRevenue, result.Value.Revenue.TodayRevenue);
            Assert.Equal(orderStats.TotalOrders, result.Value.OrderStats.TotalOrders);
            Assert.Equal(3, result.Value.TotalReservations);
            Assert.Equal(2, result.Value.ActiveReservations); // confirmed + pending
        }

        [Fact]
        public async Task GetDashboardSummaryAsync_HandlesMissingReservations()
        {
            // Arrange
            var revenue = new RevenueDto
            {
                TodayRevenue = 500m,
                WeekRevenue = 3500m,
                MonthRevenue = 15000m,
                YearRevenue = 180000m,
                AveragePerOrder = 45.50m
            };

            var orderStats = new OrderStatsDto
            {
                TotalOrders = 100,
                CompletedOrders = 95,
                PendingOrders = 3,
                CancelledOrders = 2,
                AveragePreparationTime = 25.5
            };

            _mockCacheService
                .Setup(x => x.GetAsync<DashboardSummary>(It.IsAny<string>()))
                .ReturnsAsync((DashboardSummary?)null);

            _mockCacheService
                .Setup(x => x.GetAsync<RevenueDto>(It.IsAny<string>()))
                .ReturnsAsync((RevenueDto?)null);

            _mockCacheService
                .Setup(x => x.GetAsync<OrderStatsDto>(It.IsAny<string>()))
                .ReturnsAsync((OrderStatsDto?)null);

            _mockApiClient
                .Setup(x => x.GetRevenueAsync(It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
                .ReturnsAsync(revenue);

            _mockApiClient
                .Setup(x => x.GetOrderStatsAsync(It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
                .ReturnsAsync(orderStats);

            _mockReservationService
                .Setup(x => x.GetReservationsAsync(It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
                .ReturnsAsync(Result<List<ReservationDto>>.Failure("API error"));

            // Act
            var result = await _service.GetDashboardSummaryAsync();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(0, result.Value.TotalReservations);
            Assert.Equal(0, result.Value.ActiveReservations);
        }
    }
}
