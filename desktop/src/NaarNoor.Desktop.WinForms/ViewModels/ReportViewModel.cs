using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NaarNoor.Desktop.Common.DTOs;
using NaarNoor.Desktop.Common.Services;

namespace NaarNoor.Desktop.WinForms.ViewModels
{
    /// <summary>
    /// ViewModel for analytics and reporting.
    /// Displays revenue, order statistics, and reservation trends.
    /// </summary>
    public partial class ReportViewModel : ViewModelBase
    {
        private readonly IReportService _reportService;

        /// <summary>
        /// Start date for report date range
        /// </summary>
        [ObservableProperty]
        private DateTime startDate = DateTime.Today.AddDays(-30);

        /// <summary>
        /// End date for report date range
        /// </summary>
        [ObservableProperty]
        private DateTime endDate = DateTime.Today;

        /// <summary>
        /// Today's revenue
        /// </summary>
        [ObservableProperty]
        private decimal todayRevenue;

        /// <summary>
        /// Week revenue
        /// </summary>
        [ObservableProperty]
        private decimal weekRevenue;

        /// <summary>
        /// Month revenue
        /// </summary>
        [ObservableProperty]
        private decimal monthRevenue;

        /// <summary>
        /// Year-to-date revenue
        /// </summary>
        [ObservableProperty]
        private decimal yearRevenue;

        /// <summary>
        /// Total number of orders
        /// </summary>
        [ObservableProperty]
        private int totalOrders;

        /// <summary>
        /// Completed orders count
        /// </summary>
        [ObservableProperty]
        private int completedOrders;

        /// <summary>
        /// Pending orders count
        /// </summary>
        [ObservableProperty]
        private int pendingOrders;

        /// <summary>
        /// Cancelled orders count
        /// </summary>
        [ObservableProperty]
        private int cancelledOrders;

        /// <summary>
        /// Average preparation time in minutes
        /// </summary>
        [ObservableProperty]
        private double averagePreparationTime;

        /// <summary>
        /// Total number of reservations
        /// </summary>
        [ObservableProperty]
        private int totalReservations;

        /// <summary>
        /// Active/confirmed reservations
        /// </summary>
        [ObservableProperty]
        private int activeReservations;

        /// <summary>
        /// Report type for display (Revenue, Orders, Reservations)
        /// </summary>
        [ObservableProperty]
        private string reportType = "Revenue";

        public ReportViewModel(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _reportService = GetService<IReportService>();
        }

        /// <summary>
        /// Load report data for the current date range and report type.
        /// </summary>
        [RelayCommand]
        public async Task LoadReport()
        {
            // Validate date range
            if (StartDate > EndDate)
            {
                SetError("Start date must be before end date");
                return;
            }

            ClearError();

            var result = await ExecuteAsync(
                async () => await _reportService.GetDashboardSummaryAsync(),
                onSuccess: (summary) =>
                {
                    if (summary == null)
                    {
                        SetError("Failed to load report data");
                        return;
                    }

                    // Update revenue metrics
                    TodayRevenue = summary.Revenue.TodayRevenue;
                    WeekRevenue = summary.Revenue.WeekRevenue;
                    MonthRevenue = summary.Revenue.MonthRevenue;
                    YearRevenue = summary.Revenue.YearRevenue;

                    // Update order metrics
                    TotalOrders = summary.OrderStats.TotalOrders;
                    CompletedOrders = summary.OrderStats.CompletedOrders;
                    PendingOrders = summary.OrderStats.PendingOrders;
                    CancelledOrders = summary.OrderStats.CancelledOrders;
                    AveragePreparationTime = summary.OrderStats.AveragePreparationTime;

                    // Update reservation metrics
                    TotalReservations = summary.TotalReservations;
                    ActiveReservations = summary.ActiveReservations;
                }
            );
        }

        /// <summary>
        /// Export report data to CSV file.
        /// </summary>
        [RelayCommand]
        public async Task ExportToCsv()
        {
            try
            {
                // In real app, would prompt for file location
                var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                var csvPath = Path.Combine(
                    documentsPath,
                    $"NaarNoor_Report_{ReportType}_{DateTime.Now:yyyyMMdd_HHmmss}.csv"
                );

                var csvLines = new List<string>
                {
                    $"Naar-Noor Report - {ReportType}",
                    $"Date Range: {StartDate:yyyy-MM-dd} to {EndDate:yyyy-MM-dd}",
                    "",
                    "Metric,Value",
                    $"Today Revenue,{TodayRevenue:C}",
                    $"Week Revenue,{WeekRevenue:C}",
                    $"Month Revenue,{MonthRevenue:C}",
                    $"Year Revenue,{YearRevenue:C}",
                    $"Total Orders,{TotalOrders}",
                    $"Completed Orders,{CompletedOrders}",
                    $"Pending Orders,{PendingOrders}",
                    $"Cancelled Orders,{CancelledOrders}",
                    $"Average Prep Time (min),{AveragePreparationTime:F2}",
                    $"Total Reservations,{TotalReservations}",
                    $"Active Reservations,{ActiveReservations}"
                };

                await File.WriteAllLinesAsync(csvPath, csvLines);
                SetError($"Report exported successfully to {Path.GetFileName(csvPath)}");
            }
            catch (Exception ex)
            {
                SetError($"Export failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Handle report type change - reload report.
        /// </summary>
        partial void OnReportTypeChanged(string value)
        {
            LoadReportCommand.ExecuteAsync(null);
        }

        /// <summary>
        /// Handle start date change - reload report.
        /// </summary>
        partial void OnStartDateChanged(DateTime value)
        {
            if (value <= EndDate)
            {
                LoadReportCommand.ExecuteAsync(null);
            }
        }

        /// <summary>
        /// Handle end date change - reload report.
        /// </summary>
        partial void OnEndDateChanged(DateTime value)
        {
            if (StartDate <= value)
            {
                LoadReportCommand.ExecuteAsync(null);
            }
        }
    }
}
