using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using NaarNoor.Desktop.Common.Services;
using NaarNoor.Desktop.Common.Services.Interfaces;
using NaarNoor.Desktop.Common.Services.Implementations;
using NaarNoor.Desktop.Common.Services.ApiClients;
using NaarNoor.Desktop.Common.Data;
using NaarNoor.Desktop.Common.Configuration;
using NaarNoor.Desktop.WinForms.ViewModels;

namespace NaarNoor.Desktop.WinForms.Configuration
{
    /// <summary>
    /// Configures the dependency injection container for the desktop application
    /// Delegates HTTP client setup to separate HttpClientConfiguration class
    /// </summary>
    public static class ServiceConfiguration
    {
        /// <summary>
        /// Configure all services, ViewModels, and dependencies for the application
        /// </summary>
        public static async Task<IServiceProvider> ConfigureServicesAsync(IConfiguration configuration)
        {
            var services = new ServiceCollection();

            // Add configuration
            services.AddSingleton(configuration);

            // Configure application configuration service
            services.AddSingleton<IConfigurationService, ConfigurationService>();

            // Configure database (SQLite)
            ConfigureDatabase(services, configuration);

            // Configure HTTP clients with security headers and authentication
            HttpClientConfiguration.ConfigureHttpClients(services, configuration);

            // Register singleton services (stateful, shared across application)
            services.AddSingleton<IAuthenticationService, AuthenticationService>();
            services.AddSingleton<IAuditService, AuditService>();
            services.AddSingleton<IAuthorizationService, AuthorizationService>();
            services.AddSingleton<ILocalizationService, LocalizationService>();
            services.AddSingleton<ICacheService, CacheService>();
            services.AddSingleton<INetworkConnectivityService, NetworkConnectivityService>();

            // Register security services
            services.AddSingleton<IInputValidationService, InputValidationService>();
            services.AddSingleton<ISecureLoggingService, SecureLoggingService>();
            services.AddSingleton<ITlsConfigurationService, TlsConfigurationService>();

            // Register request signing service with API signing key from configuration
            var signingKey = configuration["Security:RequestSigningKey"] ?? "DefaultSigningKeyChangeInProduction";
            services.AddSingleton<IRequestSigningService>(_ => new RequestSigningService(signingKey));

            // Register business services as singletons
            services.AddSingleton<IReservationService, ReservationService>();
            services.AddSingleton<IMenuService, MenuService>();
            services.AddSingleton<IChefService, ChefService>();
            services.AddSingleton<IReportService, ReportService>();
            services.AddSingleton<ISyncService, SyncService>();

            // Register database initializer as singleton
            services.AddSingleton<DatabaseInitializer>();

            // Register ViewModels as singletons
            services.AddSingleton<LoginViewModel>();
            services.AddSingleton<DashboardViewModel>();
            services.AddSingleton<ReservationViewModel>();
            services.AddSingleton<MenuViewModel>();
            services.AddSingleton<StaffViewModel>();
            services.AddSingleton<ReportViewModel>();

            // Build the service provider
            var serviceProvider = services.BuildServiceProvider();

            // Load application configuration asynchronously
            var configService = serviceProvider.GetRequiredService<IConfigurationService>();
            await configService.LoadConfigurationAsync();

            return serviceProvider;
        }

        /// <summary>
        /// Synchronous version for backward compatibility
        /// </summary>
        public static IServiceProvider ConfigureServices(IConfiguration configuration)
        {
            return ConfigureServicesAsync(configuration).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Configure local SQLite database for caching and offline storage
        /// </summary>
        private static void ConfigureDatabase(IServiceCollection services, IConfiguration configuration)
        {
            var dbPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "NaarNoor",
                "naaranoor.db"
            );

            // Ensure directory exists
            var dbDirectory = Path.GetDirectoryName(dbPath);
            if (!Directory.Exists(dbDirectory))
            {
                Directory.CreateDirectory(dbDirectory);
            }

            // Register DbContext
            services.AddDbContext<DatabaseContext>(options =>
                options.UseSqlite($"Data Source={dbPath}"),
                ServiceLifetime.Singleton
            );
        }
    }
}
