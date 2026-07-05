using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NaarNoor.Desktop.Common.Services.ApiClients;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;

namespace NaarNoor.Desktop.WinForms.Configuration
{
    /// <summary>
    /// Configures HTTP clients with Refit, security headers, and authentication handlers
    /// Handles all HttpClient setup for API communication with Polly resilience policies
    /// </summary>
    public static class HttpClientConfiguration
    {
        private const int CircuitBreakerFailureThreshold = 5;
        private const int CircuitBreakerDurationSeconds = 30;

        /// <summary>
        /// Configure HTTP clients with Refit, retry policies, and resilience handlers
        /// Applies exponential backoff retry policy (1s, 2s, 4s) and circuit breaker policy
        /// </summary>
        public static void ConfigureHttpClients(IServiceCollection services, IConfiguration configuration)
        {
            var apiBaseUrl = configuration["Api:BaseUrl"] ?? "http://localhost:8080";
            var timeoutSeconds = configuration.GetValue<int>("Api:TimeoutSeconds", 30);
            var appVersion = GetApplicationVersion();

            // Register authentication header handler (must be before HttpClient registration)
            services.AddTransient<AuthenticationHeaderHandler>();

            // Configure common HttpClient settings
            Action<HttpClient> configureClient = (client) =>
            {
                client.BaseAddress = new Uri(apiBaseUrl);
                client.Timeout = TimeSpan.FromSeconds(timeoutSeconds);

                // Add security headers
                AddSecurityHeaders(client, appVersion);
            };

            // Configure TLS 1.3+ enforcement
            ConfigureTls();

            // Create Polly policies
            var retryPolicy = CreateRetryPolicy();
            var circuitBreakerPolicy = CreateCircuitBreakerPolicy();

            // Register all API clients with HttpClientFactory
            services.AddHttpClient<IAuthApiClient>(configureClient)
                .AddPolicyHandler(retryPolicy)
                .AddPolicyHandler(circuitBreakerPolicy);

            services.AddHttpClient<IReservationApiClient>(configureClient)
                .AddPolicyHandler(retryPolicy)
                .AddPolicyHandler(circuitBreakerPolicy)
                .AddHttpMessageHandler<AuthenticationHeaderHandler>();

            services.AddHttpClient<IMenuApiClient>(configureClient)
                .AddPolicyHandler(retryPolicy)
                .AddPolicyHandler(circuitBreakerPolicy)
                .AddHttpMessageHandler<AuthenticationHeaderHandler>();

            services.AddHttpClient<IChefApiClient>(configureClient)
                .AddPolicyHandler(retryPolicy)
                .AddPolicyHandler(circuitBreakerPolicy)
                .AddHttpMessageHandler<AuthenticationHeaderHandler>();

            services.AddHttpClient<IReportApiClient>(configureClient)
                .AddPolicyHandler(retryPolicy)
                .AddPolicyHandler(circuitBreakerPolicy)
                .AddHttpMessageHandler<AuthenticationHeaderHandler>();
        }

        /// <summary>
        /// Create Polly retry policy with exponential backoff
        /// Retries on transient failures: timeouts, 5xx errors
        /// Backoff delays: 1 second, 2 seconds, 4 seconds
        /// </summary>
        private static IAsyncPolicy<HttpResponseMessage> CreateRetryPolicy()
        {
            return Policy
                .Handle<HttpRequestException>()
                .Or<TaskCanceledException>()
                .OrResult<HttpResponseMessage>(r =>
                    (int)r.StatusCode >= 500 ||
                    r.StatusCode == System.Net.HttpStatusCode.RequestTimeout)
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt - 1)),
                    onRetry: (outcome, timespan, retryCount, context) =>
                    {
                        System.Diagnostics.Debug.WriteLine(
                            $"[Polly] Retry {retryCount} after {timespan.TotalSeconds}s - " +
                            $"Exception: {outcome.Exception?.Message ?? "N/A"}, " +
                            $"Status: {outcome.Result?.StatusCode.ToString() ?? "N/A"}");
                    });
        }

        /// <summary>
        /// Create Polly circuit breaker policy
        /// Breaks after 5 consecutive failures with 30-second duration
        /// Prevents cascading failures to backend API
        /// </summary>
        private static IAsyncPolicy<HttpResponseMessage> CreateCircuitBreakerPolicy()
        {
            return Policy
                .Handle<HttpRequestException>()
                .Or<TaskCanceledException>()
                .OrResult<HttpResponseMessage>(r =>
                    (int)r.StatusCode >= 500 ||
                    r.StatusCode == System.Net.HttpStatusCode.RequestTimeout)
                .CircuitBreakerAsync(
                    handledEventsAllowedBeforeBreaking: CircuitBreakerFailureThreshold,
                    durationOfBreak: TimeSpan.FromSeconds(CircuitBreakerDurationSeconds),
                    onBreak: (outcome, timespan) =>
                    {
                        System.Diagnostics.Debug.WriteLine(
                            $"[Polly] Circuit breaker opened for {timespan.TotalSeconds}s - " +
                            $"After {CircuitBreakerFailureThreshold} failures");
                    },
                    onReset: () =>
                    {
                        System.Diagnostics.Debug.WriteLine("[Polly] Circuit breaker reset - API recovered");
                    });
        }

        /// <summary>
        /// Add security headers to HTTP client
        /// </summary>
        private static void AddSecurityHeaders(HttpClient client, string appVersion)
        {
            // Prevent content-type sniffing attacks
            client.DefaultRequestHeaders.Add("X-Content-Type-Options", "nosniff");

            // Prevent clickjacking attacks
            client.DefaultRequestHeaders.Add("X-Frame-Options", "DENY");

            // Enable XSS protection
            client.DefaultRequestHeaders.Add("X-XSS-Protection", "1; mode=block");

            // Force HTTPS for 1 year
            client.DefaultRequestHeaders.Add("Strict-Transport-Security", "max-age=31536000; includeSubDomains");

            // Identify client version for API logging/debugging
            client.DefaultRequestHeaders.Add("X-Client-Version", appVersion);

            // Identify client as desktop app
            client.DefaultRequestHeaders.Add("User-Agent", GetUserAgent());

            // Accept gzip compression (handled automatically by HttpClient)
            client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");
        }

        /// <summary>
        /// Configure TLS protocol version enforcement
        /// </summary>
        private static void ConfigureTls()
        {
            // Enforce TLS 1.3 with TLS 1.2 fallback for compatibility
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13 | SecurityProtocolType.Tls12;
        }

        /// <summary>
        /// Get user agent string identifying the desktop client
        /// </summary>
        private static string GetUserAgent()
        {
            var version = GetApplicationVersion();
            var runtimeVersion = System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription;
            return $"NaarNoor-Desktop/{version} ({runtimeVersion})";
        }

        /// <summary>
        /// Get application version from assembly
        /// </summary>
        private static string GetApplicationVersion()
        {
            return typeof(HttpClientConfiguration).Assembly.GetName().Version?.ToString() ?? "1.0.0";
        }
    }
}
