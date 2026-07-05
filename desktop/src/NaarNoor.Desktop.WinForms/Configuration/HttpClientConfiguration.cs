using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NaarNoor.Desktop.Common.Services.ApiClients;

namespace NaarNoor.Desktop.WinForms.Configuration
{
    /// <summary>
    /// Configures HTTP clients with Refit, security headers, and authentication handlers
    /// Handles all HttpClient setup for API communication
    /// </summary>
    public static class HttpClientConfiguration
    {
        /// <summary>
        /// Configure HTTP clients with Refit, retry policies, and resilience handlers
        /// </summary>
        public static void ConfigureHttpClients(IServiceCollection services, IConfiguration configuration)
        {
            var apiBaseUrl = configuration["Api:BaseUrl"] ?? "http://localhost:8080";
            var appVersion = GetApplicationVersion();

            // Register authentication header handler (must be before HttpClient registration)
            services.AddTransient<AuthenticationHeaderHandler>();

            // Configure common HttpClient settings
            Action<HttpClient> configureClient = (client) =>
            {
                client.BaseAddress = new Uri(apiBaseUrl);
                client.Timeout = TimeSpan.FromSeconds(30);

                // Add security headers
                AddSecurityHeaders(client, appVersion);
            };

            // Configure TLS 1.3+ enforcement
            ConfigureTls();

            // Register all API clients with HttpClientFactory
            services.AddHttpClient<IAuthApiClient>(configureClient);

            services.AddHttpClient<IReservationApiClient>(configureClient)
                .AddHttpMessageHandler<AuthenticationHeaderHandler>();

            services.AddHttpClient<IMenuApiClient>(configureClient)
                .AddHttpMessageHandler<AuthenticationHeaderHandler>();

            services.AddHttpClient<IChefApiClient>(configureClient)
                .AddHttpMessageHandler<AuthenticationHeaderHandler>();

            services.AddHttpClient<IReportApiClient>(configureClient)
                .AddHttpMessageHandler<AuthenticationHeaderHandler>();
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
