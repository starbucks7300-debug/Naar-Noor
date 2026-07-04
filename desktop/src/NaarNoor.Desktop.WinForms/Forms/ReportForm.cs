using NaarNoor.Desktop.WinForms.ViewModels;
using NaarNoor.Desktop.WinForms.Utilities;

namespace NaarNoor.Desktop.WinForms.Forms
{
    /// <summary>
    /// Report and analytics form for displaying business metrics.
    /// Tabbed interface with Revenue, Orders, and Reservations analytics.
    /// Supports date range filtering and CSV export.
    /// Binds to ReportViewModel for analytics data.
    /// </summary>
    public partial class ReportForm : Form
    {
        private readonly ReportViewModel _viewModel;

        public ReportForm()
        {
            _viewModel = null!;
            InitializeComponent();
        }

        public ReportForm(ReportViewModel viewModel)
        {
            _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
            InitializeComponent();
            InitializeDataGrids();
            SetupDataBindings();
        }

        private void InitializeDataGrids()
        {
            // Initialize Revenue tab grid
            var dgvRevenue = new DataGridView
            {
                Name = "dgvRevenue",
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true
            };
            dgvRevenue.Columns.Add("Date", "Date");
            dgvRevenue.Columns.Add("Amount", "Amount");
            dgvRevenue.Columns.Add("OrderCount", "Orders");
            tabRevenue.Controls.Add(dgvRevenue);

            // Initialize Orders tab grid
            var dgvOrders = new DataGridView
            {
                Name = "dgvOrders",
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true
            };
            dgvOrders.Columns.Add("OrderId", "Order ID");
            dgvOrders.Columns.Add("Date", "Date");
            dgvOrders.Columns.Add("Items", "Items");
            dgvOrders.Columns.Add("Total", "Total");
            dgvOrders.Columns.Add("Status", "Status");
            tabOrders.Controls.Add(dgvOrders);

            // Initialize Reservations tab grid
            var dgvReservations = new DataGridView
            {
                Name = "dgvReservations",
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true
            };
            dgvReservations.Columns.Add("Date", "Date");
            dgvReservations.Columns.Add("Count", "Reservations");
            dgvReservations.Columns.Add("Covers", "Total Covers");
            dgvReservations.Columns.Add("Revenue", "Revenue");
            tabReservations.Controls.Add(dgvReservations);
        }

        private void SetupDataBindings()
        {
            // Date range binding
            dtpStartDate.DataBindings.Add("Value", _viewModel, "StartDate", true, DataSourceUpdateMode.OnPropertyChanged);
            dtpEndDate.DataBindings.Add("Value", _viewModel, "EndDate", true, DataSourceUpdateMode.OnPropertyChanged);

            // Metrics binding
            lblRevenueValue.DataBindings.Add("Text", _viewModel, "YearRevenue", true, DataSourceUpdateMode.OnPropertyChanged,
                new DecimalToCurrencyConverter());
            lblOrdersValue.DataBindings.Add("Text", _viewModel, "TotalOrders");
            lblReservationsValue.DataBindings.Add("Text", _viewModel, "TotalReservations");

            // Error display binding
            var errorLabel = new Label { Visible = false };
            errorLabel.DataBindings.Add("Text", _viewModel, "ErrorMessage", true);
            errorLabel.DataBindings.Add("Visible", _viewModel, "ErrorMessage", true, DataSourceUpdateMode.OnPropertyChanged,
                new ErrorMessageToVisibleConverter());
            this.Controls.Add(errorLabel);

            // Button commands
            btnFilter.Click += (s, e) => _viewModel.LoadReportCommand.Execute(null);
            btnExport.Click += (s, e) => _viewModel.ExportToCsvCommand.Execute(null);

            // Load initial data
            _viewModel.LoadReportCommand.Execute(null);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
