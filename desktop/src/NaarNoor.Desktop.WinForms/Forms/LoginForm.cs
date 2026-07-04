using NaarNoor.Desktop.WinForms.ViewModels;
using NaarNoor.Desktop.WinForms.Utilities;

namespace NaarNoor.Desktop.WinForms.Forms
{
    /// <summary>
    /// Login form for user authentication.
    /// Provides credential input (username, password) with validation and error display.
    /// Binds to LoginViewModel for authentication logic.
    /// </summary>
    public partial class LoginForm : Form
    {
        private readonly LoginViewModel _viewModel;

        // Parameterless constructor for designer/serialization
        public LoginForm()
        {
            _viewModel = null!; // Will be null - not used in designer context
            InitializeComponent();
        }

        public LoginForm(LoginViewModel viewModel)
        {
            _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
            InitializeComponent();
            SetupDataBindings();
            SubscribeToEvents();
        }

        /// <summary>
        /// Set up data bindings between UI controls and ViewModel properties.
        /// </summary>
        private void SetupDataBindings()
        {
            // Username binding
            txtUsername.DataBindings.Add("Text", _viewModel, "UsernameInput", true, DataSourceUpdateMode.OnPropertyChanged);

            // Password binding (special handling for masked input)
            txtPassword.DataBindings.Add("Text", _viewModel, "PasswordInput", true, DataSourceUpdateMode.OnPropertyChanged);

            // Error message binding (label visibility)
            lblError.DataBindings.Add("Text", _viewModel, "ErrorMessage", true);
            lblError.DataBindings.Add("Visible", _viewModel, "ErrorMessage", true, DataSourceUpdateMode.OnPropertyChanged, new ErrorMessageToVisibleConverter());

            // Loading state binding (button disabled, spinner visible)
            btnLogin.DataBindings.Add("Enabled", _viewModel, "IsLoading", true, DataSourceUpdateMode.OnPropertyChanged, new InverseBooleanConverter());
            progressSpinner.DataBindings.Add("Visible", _viewModel, "IsLoading");

            // Login command binding
            btnLogin.Click += async (s, e) => await _viewModel.LoginAsync();
        }

        /// <summary>
        /// Subscribe to ViewModel events (LoginSucceeded).
        /// </summary>
        private void SubscribeToEvents()
        {
            _viewModel.LoginSucceeded += ViewModel_LoginSucceeded;
        }

        /// <summary>
        /// Handle successful login - navigate to dashboard.
        /// </summary>
        private void ViewModel_LoginSucceeded(object? sender, EventArgs e)
        {
            try
            {
                var dashboardViewModel = _viewModel.ServiceProvider.GetService(typeof(DashboardViewModel)) as DashboardViewModel;
                if (dashboardViewModel == null)
                {
                    throw new InvalidOperationException("DashboardViewModel not found");
                }
                
                var dashboardForm = new DashboardForm(dashboardViewModel);
                Application.OpenForms[0]?.Hide();
                dashboardForm.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open dashboard: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _viewModel.LoginSucceeded -= ViewModel_LoginSucceeded;
                components?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
