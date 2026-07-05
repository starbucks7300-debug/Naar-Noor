using NaarNoor.Desktop.Common.Services;
using System.Diagnostics;

namespace NaarNoor.Desktop.WinForms.Configuration
{
    /// <summary>
    /// HTTP message handler that automatically injects authentication tokens into outgoing requests
    /// Handles token refresh on 401 responses and logs refresh failures to audit trail (REQ-005)
    /// </summary>
    public class AuthenticationHeaderHandler : DelegatingHandler
    {
        private readonly IAuthenticationService _authService;
        private readonly IAuditService? _auditService;
        private const int MaxRefreshAttempts = 1; // Prevent infinite retry loops

        public AuthenticationHeaderHandler(IAuthenticationService authService, IAuditService? auditService = null)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _auditService = auditService;
        }

        /// <summary>
        /// Send HTTP request with automatic authentication header injection and token refresh
        /// </summary>
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            // Attempt to add authorization header if user is authenticated
            if (_authService.IsAuthenticated)
            {
                try
                {
                    var token = await _authService.GetCurrentTokenAsync();
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }
                catch (Exception ex)
                {
                    // Token retrieval failed - proceed without auth header
                    // The API will likely return 401, but we won't refresh if token is invalid
                    Debug.WriteLine($"Failed to get current token: {ex.Message}");
                }
            }

            var response = await base.SendAsync(request, cancellationToken);

            // Handle 401 Unauthorized - attempt automatic token refresh
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized && _authService.IsAuthenticated)
            {
                response = await HandleUnauthorizedResponseAsync(request, cancellationToken);
            }

            return response;
        }

        /// <summary>
        /// Handle 401 Unauthorized by attempting token refresh and retrying request
        /// </summary>
        private async Task<HttpResponseMessage> HandleUnauthorizedResponseAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            try
            {
                Debug.WriteLine("Received 401 Unauthorized - attempting token refresh");

                // Attempt to refresh the token
                var refreshResult = await _authService.RefreshTokenAsync();

                if (refreshResult.IsSuccess)
                {
                    // Successfully refreshed - retry original request with new token
                    try
                    {
                        var newToken = await _authService.GetCurrentTokenAsync();
                        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", newToken);
                        
                        var retryResponse = await base.SendAsync(request, cancellationToken);
                        Debug.WriteLine($"Token refresh successful - retry response: {retryResponse.StatusCode}");
                        
                        return retryResponse;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Failed to get token after refresh: {ex.Message}");
                        // Return original 401 if retry fails
                        return new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
                    }
                }
                else
                {
                    // Refresh failed - log to audit trail
                    Debug.WriteLine($"Token refresh failed: {refreshResult.Error}");
                    LogRefreshFailure(refreshResult.Error);
                    
                    // Return original 401
                    return new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized)
                    {
                        Content = new StringContent("Token refresh failed: " + refreshResult.Error)
                    };
                }
            }
            catch (Exception ex)
            {
                // Unexpected error during refresh - log and return 401
                Debug.WriteLine($"Unexpected error during token refresh: {ex.Message}");
                LogRefreshFailure(ex.Message);
                
                return new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized)
                {
                    Content = new StringContent("Token refresh error: " + ex.Message)
                };
            }
        }

        /// <summary>
        /// Log authentication refresh failure for security audit trail (REQ-005)
        /// </summary>
        private void LogRefreshFailure(string? errorMessage)
        {
            try
            {
                Debug.WriteLine($"[AUDIT] Token refresh failure - User: {_authService.CurrentUserId}, Error: {errorMessage}");
                
                // Log to audit trail asynchronously (fire-and-forget)
                if (_auditService != null && !string.IsNullOrEmpty(_authService.CurrentUserId))
                {
                    _ = _auditService.LogSecurityEventAsync(
                        _authService.CurrentUserId,
                        "token_refresh_failed",
                        "Authentication",
                        "failure",
                        null,
                        errorMessage
                    );
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to log refresh failure: {ex.Message}");
            }
        }
    }
}
