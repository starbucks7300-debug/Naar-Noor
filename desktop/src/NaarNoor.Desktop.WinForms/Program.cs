using NaarNoor.Desktop.WinForms.Configuration;
using NaarNoor.Desktop.WinForms.Forms;
using NaarNoor.Desktop.WinForms.ViewModels;
using NaarNoor.Desktop.Common.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace NaarNoor.Desktop.WinForms;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();

        // Load configuration from appsettings.json
        var basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? Environment.CurrentDirectory;
        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
            .Build();

        // Set up dependency injection
        var serviceProvider = ServiceConfiguration.ConfigureServicesAsync(configuration).Result;

        // Initialize localization service with resource loading (REQ-121, REQ-122)
        var localizationService = serviceProvider.GetRequiredService<ILocalizationService>();
        localizationService.LoadResourcesAsync().GetAwaiter().GetResult();

        // Create and show login form
        var loginViewModel = serviceProvider.GetService(typeof(LoginViewModel)) as LoginViewModel
            ?? throw new InvalidOperationException("LoginViewModel not found in service container");

        var loginForm = new LoginForm(loginViewModel);
        Application.Run(loginForm);
    }    
}