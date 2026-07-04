namespace NaarNoor.Desktop.Common.DTOs
{
    /// <summary>
    /// Generic paged response wrapper for list endpoints
    /// </summary>
    /// <typeparam name="T">Type of items in the page</typeparam>
    public class PagedResponse<T>
    {
        public required List<T> Data { get; set; }
        public required int Page { get; set; }
        public required int PageSize { get; set; }
        public required int Total { get; set; }

        public int TotalPages => (Total + PageSize - 1) / PageSize;
    }

    /// <summary>
    /// Generic API response wrapper for consistency (using Refit's ApiResponse)
    /// </summary>
    /// <typeparam name="T">Type of response data</typeparam>
    public class ServerApiResponse<T>
    {
        public required bool Success { get; set; }
        public T? Data { get; set; }
        public string? Message { get; set; }
    }

    /// <summary>
    /// Data transfer object for revenue reporting
    /// </summary>
    public class RevenueDto
    {
        public required decimal TodayRevenue { get; set; }
        public required decimal WeekRevenue { get; set; }
        public required decimal MonthRevenue { get; set; }
        public required decimal YearRevenue { get; set; }
        public required decimal AveragePerOrder { get; set; }
    }

    /// <summary>
    /// Data transfer object for order statistics
    /// </summary>
    public class OrderStatsDto
    {
        public required int TotalOrders { get; set; }
        public required int CompletedOrders { get; set; }
        public required int PendingOrders { get; set; }
        public required int CancelledOrders { get; set; }
        public required double AveragePreparationTime { get; set; }
    }

    /// <summary>
    /// Data transfer object for report data
    /// </summary>
    public class ReportDto
    {
        public required string ReportType { get; set; }
        public required DateTime StartDate { get; set; }
        public required DateTime EndDate { get; set; }
        public required Dictionary<string, object> Data { get; set; }
    }
}
