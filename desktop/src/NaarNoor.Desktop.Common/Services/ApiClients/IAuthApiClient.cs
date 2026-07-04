using Refit;
using NaarNoor.Desktop.Common.DTOs;

namespace NaarNoor.Desktop.Common.Services.ApiClients
{
    /// <summary>
    /// Refit API client interface for authentication endpoints
    /// </summary>
    [Headers("Accept: application/json", "Content-Type: application/json")]
    public interface IAuthApiClient
    {
        /// <summary>
        /// Authenticate user with username and password
        /// </summary>
        [Post("/api/auth/login")]
        Task<LoginResponse> LoginAsync([Body] LoginRequest request);

        /// <summary>
        /// Refresh expired access token using refresh token
        /// </summary>
        [Post("/api/auth/refresh")]
        [Headers("Authorization: Bearer")]
        Task<TokenResponse> RefreshTokenAsync([Body] RefreshTokenRequest request);

        /// <summary>
        /// Logout user and invalidate refresh token
        /// </summary>
        [Post("/api/auth/logout")]
        [Headers("Authorization: Bearer")]
        Task<IApiResponse> LogoutAsync();
    }
}
