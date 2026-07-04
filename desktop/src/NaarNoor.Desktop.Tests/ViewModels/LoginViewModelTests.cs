using Xunit;
using Moq;
using NaarNoor.Desktop.WinForms.ViewModels;
using NaarNoor.Desktop.Common.Services;
using NaarNoor.Desktop.Common.Utilities;
using NaarNoor.Desktop.Common.DTOs;

namespace NaarNoor.Desktop.Tests.ViewModels
{
    public class ViewModelBaseTests
    {
        private readonly Mock<IServiceProvider> _mockServiceProvider;
        private TestViewModel? _viewModel;

        public ViewModelBaseTests()
        {
            _mockServiceProvider = new Mock<IServiceProvider>();
        }

        private void InitializeViewModel()
        {
            _viewModel = new TestViewModel(_mockServiceProvider.Object);
        }

        [Fact]
        public void Constructor_InitializesErrorAndLoadingProperties()
        {
            // Act
            InitializeViewModel();

            // Assert
            Assert.NotNull(_viewModel);
            Assert.Null(_viewModel!.ErrorMessage);
            Assert.False(_viewModel.IsLoading);
        }

        [Fact]
        public void SetError_SetsErrorMessage()
        {
            // Arrange
            InitializeViewModel();
            var message = "Test error";

            // Act
            _viewModel!.SetError(message);

            // Assert
            Assert.Equal(message, _viewModel.ErrorMessage);
        }

        [Fact]
        public void ClearError_ClearsErrorMessage()
        {
            // Arrange
            InitializeViewModel();
            _viewModel!.SetError("Test error");

            // Act
            _viewModel.ClearError();

            // Assert
            Assert.Null(_viewModel.ErrorMessage);
        }

        [Fact]
        public void GetService_ReturnsServiceFromProvider()
        {
            // Arrange
            InitializeViewModel();
            var mockService = new Mock<IAuthenticationService>();
            _mockServiceProvider
                .Setup(sp => sp.GetService(typeof(IAuthenticationService)))
                .Returns(mockService.Object);

            // Act
            var service = _viewModel!.GetTestService();

            // Assert
            Assert.NotNull(service);
        }

        [Fact]
        public void GetService_ThrowsWhenServiceNotFound()
        {
            // Arrange
            InitializeViewModel();
            _mockServiceProvider
                .Setup(sp => sp.GetService(It.IsAny<Type>()))
                .Returns(null);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => _viewModel!.GetTestService());
        }

        // Test implementation of ViewModelBase for testing
        private class TestViewModel : ViewModelBase
        {
            public TestViewModel(IServiceProvider serviceProvider) : base(serviceProvider)
            {
            }

            public IAuthenticationService GetTestService()
            {
                return GetService<IAuthenticationService>();
            }
        }
    }
}

