using NaarNoor.Desktop.Common.Utilities;
using System;
using System.Threading.Tasks;

namespace NaarNoor.Desktop.WinForms.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private string _usernameInput = string.Empty;
        private string _passwordInput = string.Empty;

        public string UsernameInput
        {
            get => _usernameInput;
            set { if (_usernameInput != value) { _usernameInput = value; OnPropertyChanged(); } }
        }

        public string PasswordInput
        {
            get => _passwordInput;
            set { if (_passwordInput != value) { _passwordInput = value; OnPropertyChanged(); } }
        }

        public event EventHandler? LoginSucceeded;

        public LoginViewModel(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task LoginAsync()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;

                if (string.IsNullOrWhiteSpace(UsernameInput) || string.IsNullOrWhiteSpace(PasswordInput))
                {
                    ErrorMessage = "Username and password are required";
                    return;
                }

                await Task.Delay(500);
                PasswordInput = string.Empty;
                LoginSucceeded?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
