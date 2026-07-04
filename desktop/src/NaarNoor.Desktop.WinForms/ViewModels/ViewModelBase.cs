using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NaarNoor.Desktop.Common.Utilities;

namespace NaarNoor.Desktop.WinForms.ViewModels
{
    /// <summary>
    /// Base class for all ViewModels providing common functionality.
    /// Extends ObservableObject from CommunityToolkit.Mvvm for INotifyPropertyChanged support.
    /// Provides error handling, loading state management, and async command execution helpers.
    /// </summary>
    public abstract partial class ViewModelBase : ObservableObject
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Error message to display to user (null when no error)
        /// </summary>
        [ObservableProperty]
        private string? errorMessage;

        /// <summary>
        /// Whether an async operation is currently in progress
        /// </summary>
        [ObservableProperty]
        private bool isLoading;

        /// <summary>
        /// Gets the service provider for dependency resolution.
        /// </summary>
        public IServiceProvider ServiceProvider => _serviceProvider;

        protected ViewModelBase(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        /// <summary>
        /// Clear any error message currently displayed.
        /// </summary>
        public void ClearError()
        {
            ErrorMessage = null;
        }

        /// <summary>
        /// Set an error message to display to the user.
        /// </summary>
        public void SetError(string? message)
        {
            ErrorMessage = message;
        }

        /// <summary>
        /// Execute an async operation with automatic loading state and error handling.
        /// </summary>
        /// <typeparam name="T">Return type of the operation</typeparam>
        /// <param name="operation">Async operation to execute</param>
        /// <param name="onSuccess">Optional callback on success</param>
        /// <param name="onError">Optional callback on error (defaults to SetError)</param>
        /// <returns>Result of the operation or failure result</returns>
        public async Task<Result<T>> ExecuteAsync<T>(
            Func<Task<Result<T>>> operation,
            Action<T>? onSuccess = null,
            Action<string>? onError = null)
        {
            try
            {
                ClearError();
                IsLoading = true;

                var result = await operation();

                if (result.IsSuccess && result.Value != null)
                {
                    onSuccess?.Invoke(result.Value);
                    return result;
                }
                else
                {
                    var errorMsg = result.Error ?? "An unknown error occurred";
                    (onError ?? SetError)(errorMsg);
                    return Result<T>.Failure(errorMsg);
                }
            }
            catch (Exception ex)
            {
                var errorMsg = $"Operation failed: {ex.Message}";
                (onError ?? SetError)(errorMsg);
                return Result<T>.Failure(errorMsg);
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// Execute an async operation without return value.
        /// </summary>
        /// <param name="operation">Async operation to execute</param>
        /// <param name="onSuccess">Optional callback on success</param>
        /// <param name="onError">Optional callback on error (defaults to SetError)</param>
        /// <returns>Result of the operation</returns>
        public async Task<Result> ExecuteAsync(
            Func<Task<Result>> operation,
            Action? onSuccess = null,
            Action<string>? onError = null)
        {
            try
            {
                ClearError();
                IsLoading = true;

                var result = await operation();

                if (result.IsSuccess)
                {
                    onSuccess?.Invoke();
                    return result;
                }
                else
                {
                    var errorMsg = result.Error ?? "An unknown error occurred";
                    (onError ?? SetError)(errorMsg);
                    return Result.Failure(errorMsg);
                }
            }
            catch (Exception ex)
            {
                var errorMsg = $"Operation failed: {ex.Message}";
                (onError ?? SetError)(errorMsg);
                return Result.Failure(errorMsg);
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// Get a service from the service provider.
        /// </summary>
        /// <typeparam name="TService">Service interface type</typeparam>
        /// <returns>Service instance or throws if not found</returns>
        protected TService GetService<TService>() where TService : class
        {
            return _serviceProvider.GetService(typeof(TService)) as TService
                ?? throw new InvalidOperationException($"Service {typeof(TService).Name} not found in container");
        }

        /// <summary>
        /// Try to get a service from the service provider.
        /// </summary>
        /// <typeparam name="TService">Service interface type</typeparam>
        /// <returns>Service instance or null if not found</returns>
        protected TService? TryGetService<TService>() where TService : class
        {
            return _serviceProvider.GetService(typeof(TService)) as TService;
        }
    }
}
