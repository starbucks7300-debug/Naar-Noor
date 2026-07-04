using NaarNoor.Desktop.Common.DTOs;
using NaarNoor.Desktop.Common.Utilities;

namespace NaarNoor.Desktop.Common.Services
{
    /// <summary>
    /// Service interface for analytics and reporting with caching.
    /// </summary>
    public interface IReportService
    {
        /// <summary>
        /// Get revenue statistics for a date range.
        /// Results are cached with 1-hour TTL for stability.
        /// </summary>
        Task<Result<RevenueDto>> GetRevenueAsync(DateTime? fromDate = null, DateTime? toDate = null);

        /// <summary>
        /// Get order statistics for a date range.
        /// Results are cached with 1-hour TTL for stability.
        /// </summary>
        Task<Result<OrderStatsDto>> GetOrderStatsAsync(DateTime? fromDate = null, DateTime? toDate = null);

        /// <summary>
        /// Get generic report data by type.
        /// Results are cached with 1-hour TTL for stability.
        /// </summary>
        Task<Result<ReportDto>> GetReportAsync(
            string reportType,
            DateTime? fromDate = null,
            DateTime? toDate = null);

        /// <summary>
        /// Get dashboard summary (aggregated key metrics for quick display).
        /// Results are cached with 30-minute TTL.
        /// </summary>
        Task<Result<DashboardSummary>> GetDashboardSummaryAsync();
    }

    /// <summary>
    /// Aggregated dashboard summary with key metrics.
    /// </summary>
    public class DashboardSummary
    {
        public required RevenueDto Revenue { get; set; }
        public required OrderStatsDto OrderStats { get; set; }
        public int TotalReservations { get; set; }
        public int ActiveReservations { get; set; }
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    }
}
