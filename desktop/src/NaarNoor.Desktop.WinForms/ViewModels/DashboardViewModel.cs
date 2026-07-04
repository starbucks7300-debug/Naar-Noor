using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NaarNoor.Desktop.Common.DTOs;
using NaarNoor.Desktop.Common.Services;

namespace NaarNoor.Desktop.WinForms.ViewModels
{
    /// <summary>
    /// ViewModel for the main dashboard.
    /// Loads and displays key metrics from all services.
    /// Implements auto-refresh every 30 seconds and role-based widget visibility.
    /// </summary>
    public partial class DashboardViewModel : ViewModelBase
    {
        private readonly IReportService _reportService;
        private readonly IAuthenticationService _authService;
        private CancellationTokenSource? _autoRefreshCancellation;
        private Task? _autoRefreshTask;

        /// <summary>
        /// Today's revenue from dashboard summary
        /// </summary>
        [ObservableProperty]
        private decimal todayRevenue;

        /// <summary>
        /// Total orders today
        /// </summary>
        [ObservableProperty]
        private int totalOrders;

        /// <summary>
        /// Total active reservations
        /// </summary>
        [ObservableProperty]
        private int activeReservations;

        /// <summary>
        /// Completed orders count
        /// </summary>
        [ObservableProperty]
        private int completedOrders;

        /// <summary>
        /// Whether to show reservation widget based on user role
        /// </summary>
        [ObservableProperty]
        private bool showReservationWidget;

        /// <summary>
        /// Whether to show menu management widget based on user role
        /// </summary>
        [ObservableProperty]
        private bool showMenuWidget;

        /// <summary>
        /// Whether to show staff management widget based on user role
        /// </summary>
        [ObservableProperty]
        private bool showStaffWidget;

        /// <summary>
        /// Whether to show reports widget based on user role
        /// </summary>
        [ObservableProperty]
        private bool showReportWidget;

        public DashboardViewModel(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _reportService = GetService<IReportService>();
            _authService = GetService<IAuthenticationService>();
        }

        /// <summary>
        /// Initialize dashboard by loading data and determining widget visibility.
        /// Should be called when the dashboard form loads.
        /// </summary>
        [RelayCommand]
        public async Task Initialize()
        {
            DetermineWidgetVisibility();
            
            // Load initial data
            await RefreshDashboard();

            // Start auto-refresh
            StartAutoRefresh();
        }

        /// <summary>
        /// Determine which widgets should be visible based on user role.
        /// </summary>
        private void DetermineWidgetVisibility()
        {
            var currentRole = _authService.CurrentUserRole ?? "Staff";

            // Manager and Admin see all widgets
            var canSeeAll = currentRole == "Manager" || currentRole == "Admin";

            // Chef can see menu and staff
            // Staff can see reservations
            ShowReservationWidget = canSeeAll || currentRole == "Staff" || currentRole == "Manager";
            ShowMenuWidget = canSeeAll || currentRole == "Chef";
            ShowStaffWidget = canSeeAll || currentRole == "Manager";
            ShowReportWidget = canSeeAll || currentRole == "Manager";
        }

        /// <summary>
        /// Refresh all dashboard metrics from services.
        /// </summary>
        [RelayCommand]
        public async Task RefreshDashboard()
        {
            // Load dashboard summary with all metrics
            var result = await ExecuteAsync(
                async () => await _reportService.GetDashboardSummaryAsync(),
                onSuccess: (summary) =>
                {
                    if (summary != null)
                    {
                        // Update revenue metrics
                        TodayRevenue = summary.Revenue.TodayRevenue;
                        
                        // Update order metrics
                        TotalOrders = summary.OrderStats.TotalOrders;
                        CompletedOrders = summary.OrderStats.CompletedOrders;
                        
                        // Update reservation metrics
                        ActiveReservations = summary.ActiveReservations;
                    }
                }
            );
        }

        /// <summary>
        /// Start auto-refresh of dashboard every 30 seconds.
        /// </summary>
        private void StartAutoRefresh()
        {
            StopAutoRefresh();

            _autoRefreshCancellation = new CancellationTokenSource();
            _autoRefreshTask = Task.Run(async () =>
            {
                while (!_autoRefreshCancellation.Token.IsCancellationRequested)
                {
                    try
                    {
                        await Task.Delay(TimeSpan.FromSeconds(30), _autoRefreshCancellation.Token);
                        
                        if (!_autoRefreshCancellation.Token.IsCancellationRequested)
                        {
                            await RefreshDashboardCommand.ExecuteAsync(null);
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        // Auto-refresh was stopped
                        break;
                    }
                    catch (Exception ex)
                    {
                        // Log error but continue auto-refreshing
                        System.Diagnostics.Debug.WriteLine($"Auto-refresh error: {ex.Message}");
                    }
                }
            }, _autoRefreshCancellation.Token);
        }

        /// <summary>
        /// Stop auto-refresh of dashboard.
        /// Should be called when navigating away or closing.
        /// </summary>
        public void StopAutoRefresh()
        {
            _autoRefreshCancellation?.Cancel();
            _autoRefreshTask?.Wait(TimeSpan.FromSeconds(1));
            _autoRefreshCancellation?.Dispose();
            _autoRefreshCancellation = null;
            _autoRefreshTask = null;
        }
    }
}
