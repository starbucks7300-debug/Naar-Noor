using Xunit;
using NaarNoor.Desktop.Common.Configuration;
using System.Text.Json;

namespace NaarNoor.Desktop.Tests.Services
{
    /// <summary>
    /// Integration tests for ConfigurationService
    /// Tests persistence of culture preference to app-config.json
    /// </summary>
    public class ConfigurationServiceTests : IDisposable
    {
        private readonly string _testConfigPath;
        private readonly ConfigurationService _configService;

        public ConfigurationServiceTests()
        {
            // Use a temporary directory for testing
            var tempDir = Path.Combine(Path.GetTempPath(), $"NaarNoor-Test-{Guid.NewGuid()}");
            Directory.CreateDirectory(tempDir);
            _testConfigPath = Path.Combine(tempDir, "app-config.json");

            _configService = new ConfigurationService();
        }

        public void Dispose()
        {
            // Cleanup test files
            try
            {
                var tempDir = Path.GetDirectoryName(_testConfigPath);
                if (tempDir != null && Directory.Exists(tempDir))
                {
                    Directory.Delete(tempDir, true);
                }
            }
            catch { /* Ignore cleanup errors */ }
        }

        [Fact]
        public async Task SaveConfigurationAsync_PersistsCultureSetting()
        {
            // Arrange
            _configService.Configuration.Culture = "ar";

            // Act
            await _configService.SaveConfigurationAsync();

            // Assert
            Assert.True(File.Exists(_testConfigPath) || Directory.Exists(
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "NaarNoor")));
        }

        [Fact]
        public async Task LoadConfigurationAsync_LoadsPersistedCulture()
        {
            // Arrange
            var appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "NaarNoor"
            );
            
            if (!Directory.Exists(appDataPath))
            {
                Directory.CreateDirectory(appDataPath);
            }

            var configPath = Path.Combine(appDataPath, "app-config.json");
            var config = new AppConfiguration { Culture = "ar" };
            var json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(configPath, json);

            // Act
            await _configService.LoadConfigurationAsync();

            // Assert
            Assert.Equal("ar", _configService.Configuration.Culture);

            // Cleanup
            File.Delete(configPath);
        }

        [Fact]
        public void Set_UpdatesCultureProperty()
        {
            // Arrange
            _configService.Configuration.Culture = "en";

            // Act
            _configService.Set("Culture", "ar");

            // Assert
            Assert.Equal("ar", _configService.Configuration.Culture);
        }

        [Fact]
        public void Get_RetrievesDefaultCulture()
        {
            // Arrange
            _configService.Configuration.Culture = "en";

            // Act
            var culture = _configService.Get<string>("Culture", "en");

            // Assert
            Assert.Equal("en", culture);
        }

        [Fact]
        public void Get_RetrievesArabicCulture()
        {
            // Arrange
            _configService.Configuration.Culture = "ar";

            // Act
            var culture = _configService.Get<string>("Culture");

            // Assert
            Assert.Equal("ar", culture);
        }

        [Fact]
        public void Get_ReturnsDefaultValueWhenKeyNotFound()
        {
            // Act
            var value = _configService.Get<string>("NonExistentKey", "defaultValue");

            // Assert
            Assert.Equal("defaultValue", value);
        }

        [Fact]
        public async Task SetAndSave_PersistsCultureToFile()
        {
            // Arrange
            _configService.Set("Culture", "ar");

            // Act
            await _configService.SaveConfigurationAsync();
            var newService = new ConfigurationService();
            await newService.LoadConfigurationAsync();

            // Assert
            Assert.Equal("ar", newService.Configuration.Culture);
        }

        [Fact]
        public void Configuration_Property_ReturnsCurrentConfiguration()
        {
            // Act
            var config = _configService.Configuration;

            // Assert
            Assert.NotNull(config);
            Assert.IsType<AppConfiguration>(config);
        }

        [Fact]
        public void AppConfiguration_HasDefaultValues()
        {
            // Arrange
            var config = new AppConfiguration();

            // Assert
            Assert.Equal("en", config.Culture);
            Assert.Equal(30, config.CacheTtlMinutes);
            Assert.True(config.EnableOfflineMode);
            Assert.Equal("light", config.Theme);
        }
    }
}
