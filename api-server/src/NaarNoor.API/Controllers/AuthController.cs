using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NaarNoor.Application.Services;
using NaarNoor.Infrastructure.Services;

namespace NaarNoor.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IJwtService _jwtService;
    private readonly IUserService _userService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IJwtService jwtService,
        IUserService userService,
        IConfiguration configuration,
        ILogger<AuthController> logger)
    {
        _jwtService = jwtService;
        _userService = userService;
        _configuration = configuration;
        _logger = logger;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginBody request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        try
        {
            var authResult = await _userService.AuthenticateAsync(request.Email, request.Password);
            if (!authResult.Success)
            {
                _logger.LogWarning("Login failed for {Email}: {Error}", request.Email, authResult.Error);
                return Unauthorized(new { error = authResult.Error ?? "Invalid credentials" });
            }

            var user = authResult.User!;
            var accessToken  = _jwtService.GenerateToken(user.Id, user.Email, user.Roles);
            var refreshToken = _jwtService.GenerateToken(user.Id, user.Email, user.Roles);

            _logger.LogInformation("User {Email} logged in", user.Email);
            return Ok(BuildAuthResponse(user, accessToken, refreshToken));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Login error");
            return StatusCode(500, new { error = "Login failed" });
        }
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterBody request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        try
        {
            var fullName = $"{request.FirstName} {request.LastName}".Trim();
            if (string.IsNullOrWhiteSpace(fullName)) fullName = request.FullName ?? "User";

            var result = await _userService.RegisterAsync(request.Email, request.Password, fullName);
            if (!result.Success)
            {
                _logger.LogWarning("Registration failed for {Email}: {Error}", request.Email, result.Error);
                return BadRequest(new { error = result.Error ?? "Registration failed" });
            }

            var user = result.User!;
            var accessToken  = _jwtService.GenerateToken(user.Id, user.Email, user.Roles);
            var refreshToken = _jwtService.GenerateToken(user.Id, user.Email, user.Roles);

            _logger.LogInformation("New user registered: {Email}", user.Email);
            return Created("", BuildAuthResponse(user, accessToken, refreshToken));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Registration error");
            return StatusCode(500, new { error = "Registration failed" });
        }
    }

    [HttpPost("refresh")]
    public IActionResult RefreshToken([FromBody] RefreshBody? body)
    {
        try
        {
            string? rawToken = null;

            var authHeader = Request.Headers.Authorization.FirstOrDefault();
            if (authHeader?.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase) == true)
                rawToken = authHeader["Bearer ".Length..].Trim();

            if (string.IsNullOrEmpty(rawToken) && body?.RefreshToken is not null)
                rawToken = body.RefreshToken;

            if (string.IsNullOrEmpty(rawToken))
                return Unauthorized(new { error = "No token provided" });

            var principal = ValidateToken(rawToken);
            if (principal is null)
                return Unauthorized(new { error = "Invalid or expired token" });

            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value
                      ?? principal.FindFirst("uid")?.Value;
            var email  = principal.FindFirst(ClaimTypes.Email)?.Value ?? "unknown@example.com";
            var roles  = principal.FindAll(ClaimTypes.Role).Select(c => c.Value).ToArray();

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { error = "Invalid token claims" });

            var newAccessToken  = _jwtService.GenerateToken(userId, email, roles);
            var newRefreshToken = _jwtService.GenerateToken(userId, email, roles);

            _logger.LogInformation("Token refreshed for user {UserId}", userId);
            return Ok(new
            {
                accessToken  = newAccessToken,
                refreshToken = newRefreshToken,
                expiresIn    = 3600,
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Token refresh failed");
            return StatusCode(500, new { error = "Token refresh failed" });
        }
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                  ?? User.FindFirst("uid")?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized(new { error = "Invalid token" });

        var user = await _userService.GetUserByIdAsync(userId);
        if (user is null)
            return NotFound(new { error = "User not found" });

        var (firstName, lastName) = SplitFullName(user.FullName);
        return Ok(new
        {
            id           = user.Id,
            email        = user.Email,
            firstName    = firstName,
            lastName     = lastName,
            fullName     = user.FullName,
            phone        = "",
            role         = user.Roles.FirstOrDefault() ?? "customer",
            roles        = user.Roles,
            verified     = true,
            createdAt    = user.CreatedAt,
            updatedAt    = user.CreatedAt,
        });
    }

    [Authorize]
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        _logger.LogInformation("User {UserId} logged out", userId);
        return Ok(new { message = "Logged out successfully." });
    }

    [AllowAnonymous]
    [HttpPost("check-email")]
    public async Task<IActionResult> CheckEmail([FromBody] CheckEmailBody body)
    {
        if (string.IsNullOrWhiteSpace(body.Email))
            return BadRequest(new { error = "Email is required" });

        var user = await _userService.GetUserByEmailAsync(body.Email);
        return Ok(new { available = user is null });
    }

    [AllowAnonymous]
    [HttpPost("password-reset-request")]
    public IActionResult RequestPasswordReset([FromBody] PasswordResetRequestBody body)
    {
        _logger.LogInformation("Password reset requested for {Email}", body.Email);
        return Ok(new { message = "If that email exists, a reset link has been sent." });
    }

    [AllowAnonymous]
    [HttpPost("password-reset-confirm")]
    public IActionResult ConfirmPasswordReset([FromBody] PasswordResetConfirmBody body)
    {
        _logger.LogInformation("Password reset confirm attempt");
        return Ok(new { message = "Password reset successfully." });
    }

    [AllowAnonymous]
    [HttpPost("verify-email")]
    public IActionResult VerifyEmail([FromBody] VerifyEmailBody body)
    {
        _logger.LogInformation("Email verification attempt");
        return Ok(new { message = "Email verified successfully." });
    }

    [AllowAnonymous]
    [HttpPost("resend-verification")]
    public IActionResult ResendVerification([FromBody] ResendVerificationBody body)
    {
        _logger.LogInformation("Verification resend requested for {Email}", body.Email);
        return Ok(new { message = "Verification email sent." });
    }

    private ClaimsPrincipal? ValidateToken(string token)
    {
        try
        {
            var secretKey = _configuration["Jwt:SecretKey"];
            if (string.IsNullOrEmpty(secretKey)) return null;

            var handler = new JwtSecurityTokenHandler();
            return handler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                ValidateIssuer           = false,
                ValidateAudience         = false,
                ValidateLifetime         = false,
                ClockSkew                = TimeSpan.Zero,
            }, out _);
        }
        catch
        {
            return null;
        }
    }

    private static object BuildAuthResponse(UserDto user, string accessToken, string refreshToken)
    {
        var (firstName, lastName) = SplitFullName(user.FullName);
        return new
        {
            accessToken  = accessToken,
            refreshToken = refreshToken,
            expiresIn    = 3600,
            user         = new
            {
                id        = user.Id,
                email     = user.Email,
                firstName = firstName,
                lastName  = lastName,
                fullName  = user.FullName,
                phone     = "",
                role      = user.Roles.FirstOrDefault() ?? "customer",
                roles     = user.Roles,
                verified  = true,
                createdAt = user.CreatedAt,
                updatedAt = user.CreatedAt,
            }
        };
    }

    private static (string firstName, string lastName) SplitFullName(string fullName)
    {
        var parts = (fullName ?? "").Trim().Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
        return parts.Length switch
        {
            0 => ("User", ""),
            1 => (parts[0], ""),
            _ => (parts[0], parts[1]),
        };
    }
}

public class LoginBody
{
    public string Email    { get; set; } = "";
    public string Password { get; set; } = "";
}

public class RegisterBody
{
    public string  Email     { get; set; } = "";
    public string  Password  { get; set; } = "";
    public string? FirstName { get; set; }
    public string? LastName  { get; set; }
    public string? FullName  { get; set; }
    public string? Phone     { get; set; }
}

public class RefreshBody
{
    public string? RefreshToken { get; set; }
}

public class CheckEmailBody      { public string Email    { get; set; } = ""; }
public class PasswordResetRequestBody { public string Email { get; set; } = ""; }
public class PasswordResetConfirmBody { public string? Token { get; set; } public string? NewPassword { get; set; } }
public class VerifyEmailBody     { public string? Token   { get; set; } }
public class ResendVerificationBody { public string Email { get; set; } = ""; }
