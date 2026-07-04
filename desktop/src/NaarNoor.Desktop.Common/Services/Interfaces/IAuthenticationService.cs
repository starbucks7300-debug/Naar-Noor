using NaarNoor.Desktop.Common.DTOs;
using NaarNoor.Desktop.Common.Utilities;

namespace NaarNoor.Desktop.Common.Services
{
    /// <summary>
    /// Interface for authentication service handling login, logout, and token management
    /// </summary>
    public interface IAuthenticationService
    {
        /// <summary>
        /// Authenticate user with credentials and obtain JWT tokens
        /// </summary>
        Task<Result<LoginResponse>> AuthenticateAsync(string username, string password);

        /// <summary>
        /// Refresh expired access token using refresh token
        /// </summary>
        Task<Result<TokenResponse>> RefreshTokenAsync();

        /// <summary>
        /// Logout user and clear all authentication state
        /// </summary>
        Task LogoutAsync();

        /// <summary>
        /// Get current access token for API requests
        /// </summary>
        Task<string> GetCurrentTokenAsync();

        /// <summary>
        /// Check if user is currently authenticated
        /// </summary>
        bool IsAuthenticated { get; }

        /// <summary>
        /// Get current authenticated user ID from JWT claims
        /// </summary>
        string? CurrentUserId { get; }

        /// <summary>
        /// Get current authenticated user role from JWT claims
        /// </summary>
        string? CurrentUserRole { get; }
    }
}
