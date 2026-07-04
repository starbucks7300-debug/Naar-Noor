using NaarNoor.Desktop.Common.DTOs;
using NaarNoor.Desktop.Common.Services.ApiClients;
using NaarNoor.Desktop.Common.Utilities;

namespace NaarNoor.Desktop.Common.Services
{
    /// <summary>
    /// Service for analytics and reporting with caching.
    /// Provides aggregated report data for dashboards and analytics pages.
    /// </summary>
    public class ReportService : IReportService
    {
        private readonly IReportApiClient _apiClient;
        private readonly IReservationService _reservationService;
        private readonly ICacheService _cacheService;

        private const string RevenueReportKeyPrefix = "report:revenue:";
        private const string OrderStatsKeyPrefix = "report:orders:";
        private const string ReportKeyPrefix = "report:";
        private const string DashboardSummaryKey = "report:dashboard:summary";
        private static readonly TimeSpan ReportCacheTtl = TimeSpan.FromHours(1);
        private static readonly TimeSpan DashboardCacheTtl = TimeSpan.FromMinutes(30);

        public ReportService(
            IReportApiClient apiClient,
            IReservationService reservationService,
            ICacheService cacheService)
        {
            _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
            _reservationService = reservationService ?? throw new ArgumentNullException(nameof(reservationService));
            _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        }

        public async Task<Result<RevenueDto>> GetRevenueAsync(DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                var cacheKey = $"{RevenueReportKeyPrefix}{fromDate:O}:{toDate:O}";

                // Try to get from cache first
                var cached = await _cacheService.GetAsync<RevenueDto>(cacheKey);
                if (cached != null)
                {
                    return Result<RevenueDto>.Success(cached);
                }

                // Fetch from API
                var result = await _apiClient.GetRevenueAsync(fromDate, toDate);

                // Cache the result
                await _cacheService.SetAsync(cacheKey, result, ReportCacheTtl);

                return Result<RevenueDto>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<RevenueDto>.Failure($"Failed to get revenue report: {ex.Message}");
            }
        }

        public async Task<Result<OrderStatsDto>> GetOrderStatsAsync(DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                var cacheKey = $"{OrderStatsKeyPrefix}{fromDate:O}:{toDate:O}";

                // Try to get from cache first
                var cached = await _cacheService.GetAsync<OrderStatsDto>(cacheKey);
                if (cached != null)
                {
                    return Result<OrderStatsDto>.Success(cached);
                }

                // Fetch from API
                var result = await _apiClient.GetOrderStatsAsync(fromDate, toDate);

                // Cache the result
                await _cacheService.SetAsync(cacheKey, result, ReportCacheTtl);

                return Result<OrderStatsDto>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<OrderStatsDto>.Failure($"Failed to get order statistics: {ex.Message}");
            }
        }

        public async Task<Result<ReportDto>> GetReportAsync(
            string reportType,
            DateTime? fromDate = null,
            DateTime? toDate = null)
        {
            if (string.IsNullOrWhiteSpace(reportType))
            {
                return Result<ReportDto>.Failure("Report type is required");
            }

            try
            {
                var cacheKey = $"{ReportKeyPrefix}{reportType}:{fromDate:O}:{toDate:O}";

                // Try to get from cache first
                var cached = await _cacheService.GetAsync<ReportDto>(cacheKey);
                if (cached != null)
                {
                    return Result<ReportDto>.Success(cached);
                }

                // Fetch from API
                var result = await _apiClient.GetReportAsync(reportType, fromDate, toDate);

                // Cache the result
                await _cacheService.SetAsync(cacheKey, result, ReportCacheTtl);

                return Result<ReportDto>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<ReportDto>.Failure($"Failed to get report: {ex.Message}");
            }
        }

        public async Task<Result<DashboardSummary>> GetDashboardSummaryAsync()
        {
            try
            {
                // Try to get from cache first
                var cached = await _cacheService.GetAsync<DashboardSummary>(DashboardSummaryKey);
                if (cached != null)
                {
                    return Result<DashboardSummary>.Success(cached);
                }

                // Fetch revenue and order stats (today)
                var today = DateTime.Today;
                var revenueResult = await GetRevenueAsync(today, today.AddDays(1));
                var orderStatsResult = await GetOrderStatsAsync(today, today.AddDays(1));

                if (!revenueResult.IsSuccess || !orderStatsResult.IsSuccess)
                {
                    return Result<DashboardSummary>.Failure(
                        revenueResult.IsSuccess ? orderStatsResult.Error : revenueResult.Error);
                }

                // Get reservation counts
                var reservationsResult = await _reservationService.GetReservationsAsync();
                int totalReservations = 0;
                int activeReservations = 0;

                if (reservationsResult.IsSuccess)
                {
                    totalReservations = reservationsResult.Value.Count;
                    activeReservations = reservationsResult.Value
                        .Count(r => r.Status.Equals("confirmed", StringComparison.OrdinalIgnoreCase)
                            || r.Status.Equals("pending", StringComparison.OrdinalIgnoreCase));
                }

                // Build summary
                var summary = new DashboardSummary
                {
                    Revenue = revenueResult.Value,
                    OrderStats = orderStatsResult.Value,
                    TotalReservations = totalReservations,
                    ActiveReservations = activeReservations,
                    GeneratedAt = DateTime.UtcNow
                };

                // Cache the result
                await _cacheService.SetAsync(DashboardSummaryKey, summary, DashboardCacheTtl);

                return Result<DashboardSummary>.Success(summary);
            }
            catch (Exception ex)
            {
                return Result<DashboardSummary>.Failure($"Failed to get dashboard summary: {ex.Message}");
            }
        }
    }
}
