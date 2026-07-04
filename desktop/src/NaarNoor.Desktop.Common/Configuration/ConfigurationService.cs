using System.Text.Json;

namespace NaarNoor.Desktop.Common.Configuration
{
    /// <summary>
    /// Service for managing application configuration persistence
    /// Loads and saves configuration to app-config.json in AppData folder
    /// </summary>
    public class ConfigurationService : IConfigurationService
    {
        private const string ConfigFileName = "app-config.json";
        private readonly string _configPath;
        private AppConfiguration _configuration;

        public AppConfiguration Configuration => _configuration;

        public ConfigurationService()
        {
            var appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "NaarNoor"
            );

            // Ensure directory exists
            if (!Directory.Exists(appDataPath))
            {
                Directory.CreateDirectory(appDataPath);
            }

            _configPath = Path.Combine(appDataPath, ConfigFileName);
            _configuration = new AppConfiguration();
        }

        /// <summary>
        /// Load configuration from app-config.json
        /// </summary>
        public async Task LoadConfigurationAsync()
        {
            try
            {
                if (File.Exists(_configPath))
                {
                    var json = await File.ReadAllTextAsync(_configPath);
                    var loadedConfig = JsonSerializer.Deserialize<AppConfiguration>(json);
                    
                    if (loadedConfig != null)
                    {
                        _configuration = loadedConfig;
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error but continue with defaults
                System.Diagnostics.Debug.WriteLine($"Error loading configuration: {ex.Message}");
                _configuration = new AppConfiguration();
            }
        }

        /// <summary>
        /// Save current configuration to app-config.json
        /// </summary>
        public async Task SaveConfigurationAsync()
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                var json = JsonSerializer.Serialize(_configuration, options);
                await File.WriteAllTextAsync(_configPath, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving configuration: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Get a configuration value by key
        /// </summary>
        public T? Get<T>(string key, T? defaultValue = default)
        {
            try
            {
                var property = typeof(AppConfiguration).GetProperty(key);
                if (property != null)
                {
                    var value = property.GetValue(_configuration);
                    return (T?)value ?? defaultValue;
                }
            }
            catch
            {
                // Ignore errors and return default
            }

            return defaultValue;
        }

        /// <summary>
        /// Set a configuration value by key
        /// </summary>
        public void Set<T>(string key, T value)
        {
            try
            {
                var property = typeof(AppConfiguration).GetProperty(key);
                if (property != null && property.CanWrite)
                {
                    property.SetValue(_configuration, value);
                }
            }
            catch
            {
                // Ignore errors
            }
        }
    }

    /// <summary>
    /// Interface for configuration service
    /// </summary>
    public interface IConfigurationService
    {
        /// <summary>
        /// Get the current configuration object
        /// </summary>
        AppConfiguration Configuration { get; }

        /// <summary>
        /// Load configuration from disk asynchronously
        /// </summary>
        Task LoadConfigurationAsync();

        /// <summary>
        /// Save configuration to disk asynchronously
        /// </summary>
        Task SaveConfigurationAsync();

        /// <summary>
        /// Get a configuration value
        /// </summary>
        T? Get<T>(string key, T? defaultValue = default);

        /// <summary>
        /// Set a configuration value
        /// </summary>
        void Set<T>(string key, T value);
    }
}
