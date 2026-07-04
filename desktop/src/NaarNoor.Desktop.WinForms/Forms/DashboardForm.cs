using NaarNoor.Desktop.WinForms.ViewModels;
using NaarNoor.Desktop.WinForms.Utilities;

namespace NaarNoor.Desktop.WinForms.Forms
{
    /// <summary>
    /// Main application dashboard form.
    /// Serves as the primary container window with menu bar, navigation, and dashboard metrics.
    /// Manages tab control for feature navigation (Reservations, Menu, Staff, Reports).
    /// Displays user info and logout button in top bar.
    /// </summary>
    public partial class DashboardForm : Form
    {
        private readonly DashboardViewModel _viewModel;

        public DashboardForm(DashboardViewModel viewModel)
        {
            _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
            InitializeComponent();
            SetupDataBindings();
            SetupEventHandlers();
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
            switch (tabControl.SelectedIndex)
            {
                case 0:
                    lblCurrentSection.Text = "Reservations";
                    break;
                case 1:
                    lblCurrentSection.Text = "Menu Management";
                    break;
                case 2:
                    lblCurrentSection.Text = "Staff Management";
                    break;
                case 3:
                    lblCurrentSection.Text = "Reports & Analytics";
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
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _viewModel.StopAutoRefresh();
                components?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
