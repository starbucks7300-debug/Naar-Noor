namespace NaarNoor.Desktop.Common.DTOs
{
    /// <summary>
    /// Request DTO for user login
    /// </summary>
    public class LoginRequest
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
    }

    /// <summary>
    /// Response DTO for successful login, contains tokens and user info
    /// </summary>
    public class LoginResponse
    {
        public required string AccessToken { get; set; }
        public required string RefreshToken { get; set; }
        public required string UserId { get; set; }
        public required string Role { get; set; }
    }

    /// <summary>
    /// Request DTO for token refresh
    /// </summary>
    public class RefreshTokenRequest
    {
        public required string RefreshToken { get; set; }
    }

    /// <summary>
    /// Response DTO for token refresh operation
    /// </summary>
    public class TokenResponse
    {
        public required string AccessToken { get; set; }
        public required string RefreshToken { get; set; }
    }
}
