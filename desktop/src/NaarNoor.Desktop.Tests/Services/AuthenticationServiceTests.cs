using Xunit;
using Moq;
using NaarNoor.Desktop.Common.Services;
using NaarNoor.Desktop.Common.Services.ApiClients;
using NaarNoor.Desktop.Common.DTOs;

namespace NaarNoor.Desktop.Tests.Services
{
    /// <summary>
    /// Unit tests for AuthenticationService.
    /// Validates Requirements REQ-001, REQ-002:
    /// REQ-001: User authentication with JWT tokens
    /// REQ-002: Automatic token refresh, resilience
    /// Target coverage: >90%
    /// </summary>
    public class AuthenticationServiceTests
    {
        private readonly Mock<IAuthApiClient> _mockAuthApiClient;

        public AuthenticationServiceTests()
        {
            _mockAuthApiClient = new Mock<IAuthApiClient>();
        }

        #region Successful Authentication Tests

        [Fact]
        public async Task AuthenticateAsync_WithValidCredentials_ReturnsSuccess()
        {
            // Arrange
            var authService = new AuthenticationService(_mockAuthApiClient.Object);
            var loginResponse = new LoginResponse
            {
                AccessToken = "test-token",
                RefreshToken = "refresh-token",
                UserId = "user123",
                Role = "Manager"
            };
            _mockAuthApiClient.Setup(x => x.LoginAsync(It.IsAny<LoginRequest>()))
                .ReturnsAsync(loginResponse);

            // Act
            var result = await authService.AuthenticateAsync("testuser", "password");

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("user123", authService.CurrentUserId);
            Assert.Equal("Manager", authService.CurrentUserRole);
        }

        [Fact]
        public async Task AuthenticateAsync_WithInvalidCredentials_ReturnsFailed()
        {
            // Arrange
            var authService = new AuthenticationService(_mockAuthApiClient.Object);
            _mockAuthApiClient.Setup(x => x.LoginAsync(It.IsAny<LoginRequest>()))
                .ThrowsAsync(new HttpRequestException("Invalid credentials"));

            // Act
            var result = await authService.AuthenticateAsync("testuser", "wrongpassword");

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Null(authService.CurrentUserId);
        }

        [Fact]
        public async Task AuthenticateAsync_WithEmptyCredentials_ReturnsFailed()
        {
            // Arrange
            var authService = new AuthenticationService(_mockAuthApiClient.Object);

            // Act
            var result = await authService.AuthenticateAsync("", "");

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Null(authService.CurrentUserId);
        }

        [Fact]
        public async Task AuthenticateAsync_IsAuthenticated_ReturnsTrue()
        {
            // Arrange
            var authService = new AuthenticationService(_mockAuthApiClient.Object);
            var loginResponse = new LoginResponse
            {
                AccessToken = "test-token",
                RefreshToken = "refresh-token",
                UserId = "user123",
                Role = "Manager"
            };
            _mockAuthApiClient.Setup(x => x.LoginAsync(It.IsAny<LoginRequest>()))
                .ReturnsAsync(loginResponse);

            // Act
            await authService.AuthenticateAsync("testuser", "password");

            // Assert
            Assert.True(authService.IsAuthenticated);
        }

        #endregion

        #region Token Refresh Tests

        [Fact]
        public async Task RefreshTokenAsync_WithValidToken_ReturnsNewToken()
        {
            // Arrange
            var authService = new AuthenticationService(_mockAuthApiClient.Object);
            
            // First, authenticate
            var loginResponse = new LoginResponse
            {
                AccessToken = "old-token",
                RefreshToken = "refresh-token",
                UserId = "user123",
                Role = "Manager"
            };
            _mockAuthApiClient.Setup(x => x.LoginAsync(It.IsAny<LoginRequest>()))
                .ReturnsAsync(loginResponse);

            await authService.AuthenticateAsync("testuser", "password");

            // Setup refresh response
            var refreshResponse = new TokenResponse
            {
                AccessToken = "new-token",
                RefreshToken = "refresh-token"
            };
            _mockAuthApiClient.Setup(x => x.RefreshTokenAsync(It.IsAny<RefreshTokenRequest>()))
                .ReturnsAsync(refreshResponse);

            // Act
            var result = await authService.RefreshTokenAsync();

            // Assert
            Assert.True(result.IsSuccess);
            _mockAuthApiClient.Verify(x => x.RefreshTokenAsync(It.IsAny<RefreshTokenRequest>()), Times.Once);
        }

        [Fact]
        public async Task RefreshTokenAsync_WithoutAuthentication_ReturnsFailed()
        {
            // Arrange
            var authService = new AuthenticationService(_mockAuthApiClient.Object);

            // Act
            var result = await authService.RefreshTokenAsync();

            // Assert
            Assert.False(result.IsSuccess);
        }

        #endregion

        #region Logout Tests

        [Fact]
        public async Task LogoutAsync_ClearsAuthenticationState()
        {
            // Arrange
            var authService = new AuthenticationService(_mockAuthApiClient.Object);
            var loginResponse = new LoginResponse
            {
                AccessToken = "test-token",
                RefreshToken = "refresh-token",
                UserId = "user123",
                Role = "Manager"
            };
            _mockAuthApiClient.Setup(x => x.LoginAsync(It.IsAny<LoginRequest>()))
                .ReturnsAsync(loginResponse);
            
            var mockApiResponse = new Mock<Refit.IApiResponse>();
            _mockAuthApiClient.Setup(x => x.LogoutAsync())
                .ReturnsAsync(mockApiResponse.Object);

            // Authenticate first
            await authService.AuthenticateAsync("testuser", "password");
            Assert.True(authService.IsAuthenticated);

            // Act
            await authService.LogoutAsync();

            // Assert
            Assert.False(authService.IsAuthenticated);
            Assert.Null(authService.CurrentUserId);
        }

        [Fact]
        public async Task LogoutAsync_CallsAPIEndpoint()
        {
            // Arrange
            var authService = new AuthenticationService(_mockAuthApiClient.Object);
            var loginResponse = new LoginResponse
            {
                AccessToken = "test-token",
                RefreshToken = "refresh-token",
                UserId = "user123",
                Role = "Manager"
            };
            _mockAuthApiClient.Setup(x => x.LoginAsync(It.IsAny<LoginRequest>()))
                .ReturnsAsync(loginResponse);
            
            var mockApiResponse = new Mock<Refit.IApiResponse>();
            _mockAuthApiClient.Setup(x => x.LogoutAsync())
                .ReturnsAsync(mockApiResponse.Object);

            await authService.AuthenticateAsync("testuser", "password");

            // Act
            await authService.LogoutAsync();

            // Assert
            _mockAuthApiClient.Verify(x => x.LogoutAsync(), Times.Once);
        }

        #endregion

        #region Token Retrieval Tests

        [Fact]
        public async Task GetCurrentTokenAsync_WhenAuthenticated_ReturnsToken()
        {
            // Arrange
            var authService = new AuthenticationService(_mockAuthApiClient.Object);
            var loginResponse = new LoginResponse
            {
                AccessToken = "test-token",
                RefreshToken = "refresh-token",
                UserId = "user123",
                Role = "Manager"
            };
            _mockAuthApiClient.Setup(x => x.LoginAsync(It.IsAny<LoginRequest>()))
                .ReturnsAsync(loginResponse);

            await authService.AuthenticateAsync("testuser", "password");

            // Act
            var token = await authService.GetCurrentTokenAsync();

            // Assert
            Assert.Equal("test-token", token);
        }

        [Fact]
        public async Task GetCurrentTokenAsync_WhenNotAuthenticated_ThrowsException()
        {
            // Arrange
            var authService = new AuthenticationService(_mockAuthApiClient.Object);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => authService.GetCurrentTokenAsync()
            );
        }

        #endregion

        #region Error Handling Tests

        [Fact]
        public async Task AuthenticateAsync_WithNetworkError_ReturnsFailed()
        {
            // Arrange
            var authService = new AuthenticationService(_mockAuthApiClient.Object);
            _mockAuthApiClient.Setup(x => x.LoginAsync(It.IsAny<LoginRequest>()))
                .ThrowsAsync(new HttpRequestException("Network error"));

            // Act
            var result = await authService.AuthenticateAsync("testuser", "password");

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Error);
            Assert.Contains("error", result.Error.ToLower());
        }

        [Fact]
        public async Task RefreshTokenAsync_WithNetworkError_LogsOut()
        {
            // Arrange
            var authService = new AuthenticationService(_mockAuthApiClient.Object);
            var loginResponse = new LoginResponse
            {
                AccessToken = "test-token",
                RefreshToken = "refresh-token",
                UserId = "user123",
                Role = "Manager"
            };
            _mockAuthApiClient.Setup(x => x.LoginAsync(It.IsAny<LoginRequest>()))
                .ReturnsAsync(loginResponse);
            _mockAuthApiClient.Setup(x => x.RefreshTokenAsync(It.IsAny<RefreshTokenRequest>()))
                .ThrowsAsync(new HttpRequestException("Network error"));

            await authService.AuthenticateAsync("testuser", "password");
            Assert.True(authService.IsAuthenticated);

            // Act
            var result = await authService.RefreshTokenAsync();

            // Assert
            Assert.False(result.IsSuccess);
            Assert.False(authService.IsAuthenticated);
        }

        #endregion

        #region Concurrent Authentication Tests

        [Fact]
        public async Task ConcurrentAuthenticationRequests_AllSucceed()
        {
            // Arrange
            var authService = new AuthenticationService(_mockAuthApiClient.Object);
            var loginResponse = new LoginResponse
            {
                AccessToken = "test-token",
                RefreshToken = "refresh-token",
                UserId = "user123",
                Role = "Manager"
            };

            _mockAuthApiClient.Setup(x => x.LoginAsync(It.IsAny<LoginRequest>()))
                .ReturnsAsync(loginResponse);

            // Act - Make concurrent authentication requests
            var tasks = Enumerable.Range(0, 5)
                .Select(_ => authService.AuthenticateAsync("testuser", "password"))
                .ToList();

            var results = await Task.WhenAll(tasks);

            // Assert
            Assert.All(results, result => Assert.True(result.IsSuccess));
        }

        #endregion
    }
}
