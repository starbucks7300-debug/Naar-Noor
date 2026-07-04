using System.Security.Cryptography;
using System.Text.Json;
using NaarNoor.Desktop.Common.DTOs;
using NaarNoor.Desktop.Common.Services.ApiClients;
using NaarNoor.Desktop.Common.Utilities;

namespace NaarNoor.Desktop.Common.Services
{
    /// <summary>
    /// Authentication service handling user login, logout, and JWT token lifecycle management
    /// </summary>
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IAuthApiClient _authApiClient;
        private string? _accessToken;
        private string? _refreshToken;
        private string? _currentUserId;
        private string? _currentUserRole;
        private DateTime _tokenExpiration;

        public bool IsAuthenticated => !string.IsNullOrEmpty(_accessToken);
        public string? CurrentUserId => _currentUserId;
        public string? CurrentUserRole => _currentUserRole;

        /// <summary>
        /// Initialize authentication service with API client
        /// </summary>
        public AuthenticationService(IAuthApiClient authApiClient)
        {
            _authApiClient = authApiClient ?? throw new ArgumentNullException(nameof(authApiClient));
        }

        /// <summary>
        /// Authenticate user with username and password
        /// </summary>
        public async Task<Result<LoginResponse>> AuthenticateAsync(string username, string password)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                {
                    return Result<LoginResponse>.Failure("Username and password are required");
                }

                var request = new LoginRequest
                {
                    Username = username.Trim(),
                    Password = password
                };

                var response = await _authApiClient.LoginAsync(request);

                if (response == null)
                {
                    return Result<LoginResponse>.Failure("Authentication failed: no response from server");
                }

                // Store tokens and extract claims
                _accessToken = response.AccessToken;
                _refreshToken = response.RefreshToken;
                _currentUserId = response.UserId;
                _currentUserRole = response.Role;

                // Parse JWT to set expiration (typically 30 minutes from now in real scenarios)
                _tokenExpiration = DateTime.UtcNow.AddMinutes(30);

                return Result<LoginResponse>.Success(response);
            }
            catch (HttpRequestException ex)
            {
                return Result<LoginResponse>.Failure($"Authentication error: {ex.Message}");
            }
            catch (Exception ex)
            {
                return Result<LoginResponse>.Failure($"Authentication error: {ex.Message}");
            }
        }

        /// <summary>
        /// Refresh expired access token using refresh token
        /// </summary>
        public async Task<Result<TokenResponse>> RefreshTokenAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(_refreshToken))
                {
                    return Result<TokenResponse>.Failure("No refresh token available");
                }

                var request = new RefreshTokenRequest { RefreshToken = _refreshToken };
                var response = await _authApiClient.RefreshTokenAsync(request);

                if (response == null)
                {
                    // Refresh failed - need to re-authenticate
                    await LogoutAsync();
                    return Result<TokenResponse>.Failure("Token refresh failed: no response from server");
                }

                // Update tokens with new ones
                _accessToken = response.AccessToken;
                _refreshToken = response.RefreshToken;
                _tokenExpiration = DateTime.UtcNow.AddMinutes(30);

                return Result<TokenResponse>.Success(response);
            }
            catch (HttpRequestException ex)
            {
                await LogoutAsync();
                return Result<TokenResponse>.Failure($"Token refresh error: {ex.Message}");
            }
            catch (Exception ex)
            {
                await LogoutAsync();
                return Result<TokenResponse>.Failure($"Token refresh error: {ex.Message}");
            }
        }

        /// <summary>
        /// Logout user and clear all authentication state
        /// </summary>
        public async Task LogoutAsync()
        {
            try
            {
                if (IsAuthenticated)
                {
                    // Attempt to notify server of logout
                    try
                    {
                        await _authApiClient.LogoutAsync();
                    }
                    catch
                    {
                        // Logout endpoint may fail, but we should still clear local state
                    }
                }
            }
            finally
            {
                // Clear all authentication state
                _accessToken = null;
                _refreshToken = null;
                _currentUserId = null;
                _currentUserRole = null;
                _tokenExpiration = DateTime.MinValue;
            }
        }

        /// <summary>
        /// Get current access token for API requests
        /// </summary>
        public async Task<string> GetCurrentTokenAsync()
        {
            if (string.IsNullOrEmpty(_accessToken))
            {
                throw new InvalidOperationException("User is not authenticated");
            }

            // Check if token is expiring soon (within 5 minutes) and refresh if needed
            if (DateTime.UtcNow >= _tokenExpiration.AddMinutes(-5))
            {
                var refreshResult = await RefreshTokenAsync();
                if (!refreshResult.IsSuccess)
                {
                    throw new InvalidOperationException("Failed to refresh expired token");
                }
            }

            return _accessToken;
        }

        /// <summary>
        /// Parse JWT token to extract claims (simplified version)
        /// In production, use System.IdentityModel.Tokens.Jwt for proper validation
        /// </summary>
        private static Dictionary<string, object>? ParseJwtClaims(string token)
        {
            try
            {
                // Split JWT into parts
                var parts = token.Split('.');
                if (parts.Length != 3)
                {
                    return null;
                }

                // Decode the payload (second part)
                var payload = parts[1];
                // Add padding if needed
                var padded = payload.Length % 4 == 0 ? payload : payload + new string('=', 4 - payload.Length % 4);

                var jsonBytes = Convert.FromBase64String(padded);
                var jsonString = System.Text.Encoding.UTF8.GetString(jsonBytes);

                var claims = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonString);
                return claims;
            }
            catch
            {
                return null;
            }
        }
    }
}
