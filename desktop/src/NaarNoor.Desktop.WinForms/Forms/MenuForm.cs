using NaarNoor.Desktop.WinForms.ViewModels;
using NaarNoor.Desktop.WinForms.Utilities;

namespace NaarNoor.Desktop.WinForms.Forms
{
    /// <summary>
    /// Menu management form for CRUD operations on menu items.
    /// Displays bilingual menu items (EN/AR) with filtering and search.
    /// Binds to MenuViewModel for menu item management logic.
    /// </summary>
    public partial class MenuForm : Form
    {
        private readonly MenuViewModel _viewModel;

        public MenuForm()
        {
            _viewModel = null!;
            InitializeComponent();
        }

        public MenuForm(MenuViewModel viewModel)
        {
            _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
            InitializeComponent();
            SetupDataBindings();
            SubscribeToEvents();
            InitializeDataGrid();
        }

        private void InitializeDataGrid()
        {
            dgvMenuItems.AutoGenerateColumns = false;
            dgvMenuItems.Columns.Clear();

            dgvMenuItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "NameEN",
                HeaderText = "Name (English)",
                DataPropertyName = "NameEN",
                Width = 150
            });

            dgvMenuItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "NameAR",
                HeaderText = "Name (Arabic)",
                DataPropertyName = "NameAR",
                Width = 150
            });

            dgvMenuItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Category",
                HeaderText = "Category",
                DataPropertyName = "Category",
                Width = 100
            });

            dgvMenuItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Price",
                HeaderText = "Price",
                DataPropertyName = "Price",
                Width = 80
            });

            dgvMenuItems.Columns.Add(new DataGridViewCheckBoxColumn
            {
                Name = "IsAvailable",
                HeaderText = "Available",
                DataPropertyName = "IsAvailable",
                Width = 80
            });
        }

        private void SetupDataBindings()
        {
            // Search binding
            txtSearch.DataBindings.Add("Text", _viewModel, "SearchName", true, DataSourceUpdateMode.OnPropertyChanged);

            // Category filter binding
            ddlCategory.DataBindings.Add("SelectedItem", _viewModel, "CategoryFilter", true, DataSourceUpdateMode.OnPropertyChanged);
            ddlCategory.Items.Clear();
            ddlCategory.Items.AddRange(new object[] { "all", "Appetizers", "Main", "Desserts", "Beverages" });

            // Menu items data binding
            dgvMenuItems.DataBindings.Add("DataSource", _viewModel, "MenuItems", true, DataSourceUpdateMode.OnPropertyChanged);

            // Form visibility binding
            pnlForm.DataBindings.Add("Visible", _viewModel, "ShowNewItemForm", true, DataSourceUpdateMode.OnPropertyChanged);

            // Form inputs binding
            txtNameEN.DataBindings.Add("Text", _viewModel, "FormNameEn", true, DataSourceUpdateMode.OnPropertyChanged);
            txtNameAR.DataBindings.Add("Text", _viewModel, "FormNameAr", true, DataSourceUpdateMode.OnPropertyChanged);
            numPrice.DataBindings.Add("Value", _viewModel, "FormPrice", true, DataSourceUpdateMode.OnPropertyChanged);
            chkAvailable.DataBindings.Add("Checked", _viewModel, "FormIsAvailable", true, DataSourceUpdateMode.OnPropertyChanged);

            // Error display binding
            var errorLabel = new Label { Visible = false };
            errorLabel.DataBindings.Add("Text", _viewModel, "ErrorMessage", true);
            errorLabel.DataBindings.Add("Visible", _viewModel, "ErrorMessage", true, DataSourceUpdateMode.OnPropertyChanged,
                new ErrorMessageToVisibleConverter());
            this.Controls.Add(errorLabel);

            // Button commands
            btnSearch.Click += (s, e) => _viewModel.LoadMenuItemsCommand.Execute(null);
            btnNew.Click += (s, e) => _viewModel.ToggleNewItemFormCommand.Execute(null);
            btnEdit.Click += (s, e) => _viewModel.EditSelectedMenuItemCommand.Execute(null);
            btnDelete.Click += (s, e) => _viewModel.DeleteMenuItemCommand.Execute(null);
            btnSave.Click += (s, e) => 
            {
                if (_viewModel.ShowNewItemForm)
                {
                    _viewModel.CreateMenuItemCommand.Execute(null);
                }
            };
            btnCancel.Click += (s, e) => _viewModel.ToggleNewItemFormCommand.Execute(null);

            // Load menu items on form load
            this.Load += (s, e) => _viewModel.InitializeCommand.Execute(null);
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
