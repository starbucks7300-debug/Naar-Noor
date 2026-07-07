using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using NaarNoor.Desktop.Common.Services;
using NaarNoor.Desktop.Common.Services.Interfaces;
using NaarNoor.Desktop.Common.Services.Implementations;
using NaarNoor.Desktop.Common.Services.ApiClients;
using NaarNoor.Desktop.Common.Configuration;
using NaarNoor.Desktop.WinForms.ViewModels;

namespace NaarNoor.Desktop.WinForms.Configuration
{
    /// <summary>
    /// Configures the DI container for the desktop application.
    /// All business data is accessed via the centralized API server (no local database).
    /// </summary>
    public static class ServiceConfiguration
    {
        public static async Task<IServiceProvider> ConfigureServicesAsync(IConfiguration configuration)
        {
            var services = new ServiceCollection();

            services.AddSingleton(configuration);
            services.AddSingleton<IConfigurationService, ConfigurationService>();

            // Configure HTTP clients (Refit + Polly + auth headers)
            HttpClientConfiguration.ConfigureHttpClients(services, configuration);

            // Core stateful services
            services.AddSingleton<IAuthenticationService, AuthenticationService>();
            services.AddSingleton<IAuditService, AuditService>();
            services.AddSingleton<IAuthorizationService, AuthorizationService>();
            services.AddSingleton<ILocalizationService, LocalizationService>();
            services.AddSingleton<ICacheService, CacheService>();
            services.AddSingleton<INetworkConnectivityService, NetworkConnectivityService>();

            // Security services
            services.AddSingleton<IInputValidationService, InputValidationService>();
            services.AddSingleton<ISecureLoggingService, SecureLoggingService>();
            services.AddSingleton<ITlsConfigurationService, TlsConfigurationService>();

            var signingKey = configuration["Security:RequestSigningKey"] ?? "DefaultSigningKeyChangeInProduction";
            services.AddSingleton<IRequestSigningService>(_ => new RequestSigningService(signingKey));

            // Business services — all delegate to API via Refit clients
            services.AddSingleton<IReservationService, ReservationService>();
            services.AddSingleton<IMenuService, MenuService>();
            services.AddSingleton<IChefService, ChefService>();
            services.AddSingleton<IReportService, ReportService>();
            services.AddSingleton<ISyncService, SyncService>();

            // ViewModels
            services.AddSingleton<LoginViewModel>();
            services.AddSingleton<DashboardViewModel>();
            services.AddSingleton<ReservationViewModel>();
            services.AddSingleton<MenuViewModel>();
            services.AddSingleton<StaffViewModel>();
            services.AddSingleton<ReportViewModel>();

            var serviceProvider = services.BuildServiceProvider();

            var configService = serviceProvider.GetRequiredService<IConfigurationService>();
            await configService.LoadConfigurationAsync();

            return serviceProvider;
        }

        public static IServiceProvider ConfigureServices(IConfiguration configuration)
            => ConfigureServicesAsync(configuration).GetAwaiter().GetResult();
    }
}
