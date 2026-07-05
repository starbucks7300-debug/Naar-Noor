using NaarNoor.Desktop.WinForms.ViewModels;
using NaarNoor.Desktop.WinForms.Utilities;
using NaarNoor.Desktop.WinForms.Theme;
using NaarNoor.Desktop.Common.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace NaarNoor.Desktop.WinForms.Forms
{
    /// <summary>
    /// Main application dashboard form.
    /// Serves as the primary container window with menu bar, navigation, and dashboard metrics.
    /// Manages tab control for feature navigation (Reservations, Menu, Staff, Reports).
    /// Displays user info and logout button in top bar.
    /// Supports RTL/LTR layout switching for bilingual support (English/Arabic)
    /// Requirements: REQ-121, REQ-122
    /// </summary>
    public partial class DashboardForm : Form
    {
        private readonly DashboardViewModel _viewModel;
        private readonly ILocalizationService _localizationService;
        private IDisposable? _cultureChangeSubscription;

        public DashboardForm(DashboardViewModel viewModel)
        {
            _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
            
            // Get localization service from service provider
            var serviceProvider = (IServiceProvider?)viewModel.GetType().GetProperty("ServiceProvider", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(viewModel);
            _localizationService = serviceProvider?.GetRequiredService<ILocalizationService>() 
                ?? throw new InvalidOperationException("LocalizationService not found");

            InitializeComponent();
            ThemeManager.Apply(this);
            SetupDataBindings();
            SetupEventHandlers();
            
            // Subscribe to culture changes to update layout (REQ-121, REQ-122)
            SubscribeToCultureChanges();
            
            // Apply initial layout direction based on current culture
            ApplyLayoutDirection();
        }

        /// <summary>
        /// Subscribe to culture change events to refresh layout
        /// </summary>
        private void SubscribeToCultureChanges()
        {
            _cultureChangeSubscription = _localizationService.CultureChanged.Subscribe(_ =>
            {
                // Update layout direction on culture change
                ApplyLayoutDirection();
                UpdateLocalizedText();
            });
        }

        /// <summary>
        /// Apply RTL/LTR layout direction to the form and all controls
        /// </summary>
        private void ApplyLayoutDirection()
        {
            LocalizationHelper.ApplyLayoutDirection(this, _localizationService);
        }

        /// <summary>
        /// Update all localized text after culture change
        /// </summary>
        private void UpdateLocalizedText()
        {
            // Update tab titles
            if (tabControl.TabPages.Count > 0)
            {
                tabControl.TabPages[0].Text = _localizationService.GetString("Reservations.Title");
                tabControl.TabPages[1].Text = _localizationService.GetString("Menu.Title");
                tabControl.TabPages[2].Text = _localizationService.GetString("Staff.Title");
                tabControl.TabPages[3].Text = _localizationService.GetString("Reports.Title");
            }

            // Update button text
            if (btnRefresh != null)
                btnRefresh.Text = _localizationService.GetString("Common.Refresh");
            
            if (btnLogout != null)
                btnLogout.Text = _localizationService.GetString("Common.Logout");

            // Update section label
            UpdateSectionLabel();
        }

        /// <summary>
        /// Called when the form loads. Initialize dashboard data and start auto-refresh.
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            _viewModel.InitializeCommand.Execute(null);
        }

        /// <summary>
        /// Set up data bindings between UI controls and ViewModel properties.
        /// </summary>
        private void SetupDataBindings()
        {
            // Revenue metric
            lblTodayRevenue.DataBindings.Add("Text", _viewModel, "TodayRevenue", true, DataSourceUpdateMode.OnPropertyChanged,
                new DecimalToCurrencyConverter());

            // Order metrics
            lblTotalOrders.DataBindings.Add("Text", _viewModel, "TotalOrders");
            lblCompletedOrders.DataBindings.Add("Text", _viewModel, "CompletedOrders");

            // Reservation metrics
            lblActiveReservations.DataBindings.Add("Text", _viewModel, "ActiveReservations");

            // Error message
            lblError.DataBindings.Add("Text", _viewModel, "ErrorMessage");
            lblError.DataBindings.Add("Visible", _viewModel, "ErrorMessage", true, DataSourceUpdateMode.OnPropertyChanged,
                new ErrorMessageToVisibleConverter());

            // Loading state
            lblLoading.DataBindings.Add("Visible", _viewModel, "IsLoading");
            
            // User info binding
            lblUserInfo.DataBindings.Add("Text", _viewModel, "CurrentUserDisplayName");
        }

        /// <summary>
        /// Set up event handlers for UI buttons and navigation.
        /// </summary>
        private void SetupEventHandlers()
        {
            // Tab navigation
            tabControl.SelectedIndexChanged += TabControl_SelectedIndexChanged;

            // Button handlers
            btnRefresh.Click += (s, e) => _viewModel.RefreshDashboardCommand.Execute(null);
            btnLogout.Click += (s, e) => HandleLogout();
        }

        /// <summary>
        /// Handle tab selection changes.
        /// </summary>
        private void TabControl_SelectedIndexChanged(object? sender, EventArgs e)
        {
            UpdateSectionLabel();
        }

        /// <summary>
        /// Update the current section label based on selected tab
        /// </summary>
        private void UpdateSectionLabel()
        {
            if (lblCurrentSection == null || tabControl == null)
                return;

            switch (tabControl.SelectedIndex)
            {
                case 0:
                    lblCurrentSection.Text = _localizationService.GetString("Reservations.Title");
                    break;
                case 1:
                    lblCurrentSection.Text = _localizationService.GetString("Menu.Title");
                    break;
                case 2:
                    lblCurrentSection.Text = _localizationService.GetString("Staff.Title");
                    break;
                case 3:
                    lblCurrentSection.Text = _localizationService.GetString("Reports.Title");
                    break;
                default:
                    lblCurrentSection.Text = _localizationService.GetString("Dashboard.Title");
                    break;
            }
        }

        /// <summary>
        /// Handle logout - stop auto-refresh and close the dashboard.
        /// </summary>
        private void HandleLogout()
        {
            _viewModel.StopAutoRefresh();
            this.Close();
            Application.Exit();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            _viewModel.StopAutoRefresh();
            _cultureChangeSubscription?.Dispose();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _viewModel.StopAutoRefresh();
                _cultureChangeSubscription?.Dispose();
                components?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
