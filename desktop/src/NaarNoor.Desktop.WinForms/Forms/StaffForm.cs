using NaarNoor.Desktop.WinForms.ViewModels;
using NaarNoor.Desktop.WinForms.Utilities;
using NaarNoor.Desktop.Common.DTOs;

namespace NaarNoor.Desktop.WinForms.Forms
{
    /// <summary>
    /// Staff management form for viewing and updating staff status.
    /// Displays staff members with role, availability status, and scheduled hours.
    /// Enables quick status changes (Available/Busy/Break/Off Duty).
    /// Binds to StaffViewModel for staff operations.
    /// </summary>
    public partial class StaffForm : Form
    {
        private readonly StaffViewModel _viewModel;

        public StaffForm()
        {
            _viewModel = null!;
            InitializeComponent();
        }

        public StaffForm(StaffViewModel viewModel)
        {
            _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
            InitializeComponent();
            SetupDataBindings();
            SubscribeToEvents();
            InitializeDataGrid();
        }

        private void InitializeDataGrid()
        {
            dgvStaff.AutoGenerateColumns = false;
            dgvStaff.Columns.Clear();

            dgvStaff.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Name",
                HeaderText = "Name",
                DataPropertyName = "Name",
                Width = 120
            });

            dgvStaff.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Role",
                HeaderText = "Role",
                DataPropertyName = "Role",
                Width = 100
            });

            dgvStaff.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Status",
                HeaderText = "Status",
                DataPropertyName = "Status",
                Width = 100
            });

            dgvStaff.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ScheduledHours",
                HeaderText = "Hours",
                DataPropertyName = "ScheduledHours",
                Width = 80
            });

            dgvStaff.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "CurrentShift",
                HeaderText = "Current Shift",
                DataPropertyName = "CurrentShift",
                Width = 100
            });
        }

        private void SetupDataBindings()
        {
            // Search binding
            txtSearch.DataBindings.Add("Text", _viewModel, "SearchName", true, DataSourceUpdateMode.OnPropertyChanged);

            // Role filter binding
            ddlStatusFilter.DataBindings.Add("SelectedItem", _viewModel, "RoleFilter", true, DataSourceUpdateMode.OnPropertyChanged);
            ddlStatusFilter.Items.Clear();
            ddlStatusFilter.Items.AddRange(_viewModel.RoleOptions.ToArray());

            // Staff list binding
            dgvStaff.DataBindings.Add("DataSource", _viewModel, "Staff", true, DataSourceUpdateMode.OnPropertyChanged);

            // Status dropdown for selected staff
            ddlStaffStatus.DataBindings.Add("SelectedItem", _viewModel, "StatusUpdate", true, DataSourceUpdateMode.OnPropertyChanged);
            ddlStaffStatus.Items.Clear();
            ddlStaffStatus.Items.AddRange(_viewModel.StatusOptions.ToArray());

            // Error display binding
            var errorLabel = new Label { Visible = false };
            errorLabel.DataBindings.Add("Text", _viewModel, "ErrorMessage", true);
            errorLabel.DataBindings.Add("Visible", _viewModel, "ErrorMessage", true, DataSourceUpdateMode.OnPropertyChanged,
                new ErrorMessageToVisibleConverter());
            this.Controls.Add(errorLabel);

            // Button commands
            btnSearch.Click += (s, e) => _viewModel.LoadStaffCommand.Execute(null);
            btnRefresh.Click += (s, e) => _viewModel.LoadStaffCommand.Execute(null);
            btnUpdateStatus.Click += (s, e) => _viewModel.UpdateStaffStatusCommand.Execute(null);

            // Refresh details when selection changes
            dgvStaff.SelectionChanged += (s, e) =>
            {
                if (dgvStaff.CurrentRow?.DataBoundItem is StaffDto staff)
                {
                    lblScheduleInfo.Text = staff.ScheduledHours ?? "No schedule";
                    lblShiftInfo.Text = staff.Role ?? "Unknown";
                    ddlStaffStatus.SelectedItem = staff.Status;
                }
            };

            // Initialize data
            _viewModel.InitializeCommand.Execute(null);
        }

        private void SubscribeToEvents()
        {
            // Events can be added here for real-time notifications
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
