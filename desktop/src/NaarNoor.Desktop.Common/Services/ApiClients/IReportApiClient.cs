using Refit;
using NaarNoor.Desktop.Common.DTOs;

namespace NaarNoor.Desktop.Common.Services.ApiClients
{
    /// <summary>
    /// Refit API client interface for reporting and analytics endpoints
    /// </summary>
    [Headers("Accept: application/json", "Content-Type: application/json", "Authorization: Bearer")]
    public interface IReportApiClient
    {
        /// <summary>
        /// Get revenue statistics for a date range
        /// </summary>
        [Get("/api/reports/revenue")]
        Task<RevenueDto> GetRevenueAsync(
            [Query] DateTime? fromDate = null,
            [Query] DateTime? toDate = null);

        /// <summary>
        /// Get order statistics for a date range
        /// </summary>
        [Get("/api/reports/orders")]
        Task<OrderStatsDto> GetOrderStatsAsync(
            [Query] DateTime? fromDate = null,
            [Query] DateTime? toDate = null);

        /// <summary>
        /// Get general report data
        /// </summary>
        [Get("/api/reports/{reportType}")]
        Task<ReportDto> GetReportAsync(
            string reportType,
            [Query] DateTime? fromDate = null,
            [Query] DateTime? toDate = null);
    }
}
