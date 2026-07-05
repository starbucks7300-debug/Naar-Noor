using NaarNoor.Desktop.WinForms.ViewModels;
using NaarNoor.Desktop.WinForms.Utilities;
using NaarNoor.Desktop.WinForms.Theme;

namespace NaarNoor.Desktop.WinForms.Forms
{
    /// <summary>
    /// Reservation management form with CRUD interface.
    /// Displays reservations in a data grid with filtering, searching, and pagination.
    /// Allows creating, editing, and deleting reservations.
    /// </summary>
    public partial class ReservationForm : Form
    {
        private readonly ReservationViewModel _viewModel;

        public ReservationForm(ReservationViewModel viewModel)
        {
            _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
            InitializeComponent();
            ThemeManager.Apply(this);
            SetupDataBindings();
            SetupEventHandlers();
        }

        /// <summary>
        /// Called when the form loads. Initialize data.
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            _viewModel.LoadReservationsCommand.Execute(null);
        }

        /// <summary>
        /// Set up data bindings between UI controls and ViewModel properties.
        /// </summary>
        private void SetupDataBindings()
        {
            // Search and filter bindings
            txtSearchCustomer.DataBindings.Add("Text", _viewModel, "SearchCustomerName", true, DataSourceUpdateMode.OnPropertyChanged);

            // Status filter
            var statuses = new[] { "all", "confirmed", "pending", "cancelled" };
            cmbStatusFilter.DataSource = statuses;
            cmbStatusFilter.DataBindings.Add("SelectedItem", _viewModel, "StatusFilter", true, DataSourceUpdateMode.OnPropertyChanged);

            // Data grid binding
            dataGridReservations.DataSource = _viewModel.Reservations;

            // Selected reservation
            dataGridReservations.DataBindings.Add("DataSource", _viewModel, "Reservations");

            // Pagination
            lblPageInfo.DataBindings.Add("Text", _viewModel, "CurrentPage", true, DataSourceUpdateMode.OnPropertyChanged,
                new PageInfoConverter(_viewModel));

            // New reservation form visibility
            pnlNewReservationForm.DataBindings.Add("Visible", _viewModel, "ShowNewReservationForm");

            // Form inputs binding
            txtFormCustomerName.DataBindings.Add("Text", _viewModel, "FormCustomerName", true, DataSourceUpdateMode.OnPropertyChanged);
            dtpFormBookingTime.DataBindings.Add("Value", _viewModel, "FormBookingTime", true, DataSourceUpdateMode.OnPropertyChanged);
            nudFormPartySize.DataBindings.Add("Value", _viewModel, "FormPartySize", true, DataSourceUpdateMode.OnPropertyChanged);
            txtFormCustomerEmail.DataBindings.Add("Text", _viewModel, "FormCustomerEmail", true, DataSourceUpdateMode.OnPropertyChanged);
            txtFormCustomerPhone.DataBindings.Add("Text", _viewModel, "FormCustomerPhone", true, DataSourceUpdateMode.OnPropertyChanged);
            txtFormSpecialRequests.DataBindings.Add("Text", _viewModel, "FormSpecialRequests", true, DataSourceUpdateMode.OnPropertyChanged);

            // Error message
            lblError.DataBindings.Add("Text", _viewModel, "ErrorMessage");
            lblError.DataBindings.Add("Visible", _viewModel, "ErrorMessage", true, DataSourceUpdateMode.OnPropertyChanged,
                new ErrorMessageToVisibleConverter());

            // Loading state
            lblLoading.DataBindings.Add("Visible", _viewModel, "IsLoading");
        }

        /// <summary>
        /// Set up event handlers for buttons and UI controls.
        /// </summary>
        private void SetupEventHandlers()
        {
            // Button handlers
            btnNewReservation.Click += (s, e) => _viewModel.ToggleNewReservationFormCommand.Execute(null);
            btnCreate.Click += (s, e) => _viewModel.CreateReservationCommand.Execute(null);
            btnUpdate.Click += (s, e) => _viewModel.UpdateReservationCommand.Execute(null);
            btnDelete.Click += (s, e) => ConfirmAndDelete();
            btnCancel.Click += (s, e) => _viewModel.ToggleNewReservationFormCommand.Execute(null);

            // Grid selection
            dataGridReservations.SelectionChanged += (s, e) =>
            {
                if (dataGridReservations.SelectedRows.Count > 0)
                {
                    var selectedRow = dataGridReservations.SelectedRows[0];
                    if (selectedRow.DataBoundItem is ReservationViewModel vm)
                    {
                        // Note: Simplified - in real implementation, bind through SelectedReservation property
                    }
                }
            };

            // Pagination
            btnPrevious.Click += (s, e) =>
            {
                if (_viewModel.CurrentPage > 1)
                {
                    _viewModel.CurrentPage--;
                    _viewModel.LoadReservationsCommand.Execute(null);
                }
            };

            btnNext.Click += (s, e) =>
            {
                var maxPages = (_viewModel.TotalCount + 49) / 50; // PageSize = 50
                if (_viewModel.CurrentPage < maxPages)
                {
                    _viewModel.CurrentPage++;
                    _viewModel.LoadReservationsCommand.Execute(null);
                }
            };
        }

        /// <summary>
        /// Show confirmation dialog before deleting a reservation.
        /// </summary>
        private void ConfirmAndDelete()
        {
            if (_viewModel.SelectedReservation == null)
            {
                MessageBox.Show("Please select a reservation to delete.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var result = MessageBox.Show(
                $"Are you sure you want to delete the reservation for {_viewModel.SelectedReservation.CustomerName}?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                _viewModel.DeleteReservationCommand.Execute(null);
            }
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

    /// <summary>
    /// Converter to display current page information.
    /// </summary>
    public class PageInfoConverter : IValueConverter
    {
        private readonly ReservationViewModel _viewModel;

        public PageInfoConverter(ReservationViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        public object? Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo? culture)
        {
            if (value is int currentPage)
            {
                var maxPages = (_viewModel.TotalCount + 49) / 50; // PageSize = 50
                return $"Page {currentPage} of {maxPages} ({_viewModel.TotalCount} total)";
            }
            return "Page 1 of 1";
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo? culture)
        {
            throw new NotImplementedException();
        }
    }
}
