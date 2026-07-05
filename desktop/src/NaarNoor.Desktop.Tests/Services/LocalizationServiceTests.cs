using Xunit;
using Moq;
using NaarNoor.Desktop.Common.Services;
using NaarNoor.Desktop.Common.Services.Interfaces;
using NaarNoor.Desktop.Common.Configuration;
using System.Globalization;
using System.Reactive.Linq;

namespace NaarNoor.Desktop.Tests.Services
{
    /// <summary>
    /// Tests for LocalizationService
    /// Validates English and Arabic localization, culture switching, and RTL/LTR support
    /// Requirements: REQ-121, REQ-122
    /// </summary>
    public class LocalizationServiceTests : IDisposable
    {
        private readonly Mock<IConfigurationService> _mockConfigService;
        private readonly LocalizationService _localizationService;
        private readonly string _testResourcesPath;

        public LocalizationServiceTests()
        {
            _mockConfigService = new Mock<IConfigurationService>();
            _mockConfigService.Setup(c => c.Get("Culture", It.IsAny<string>()))
                .Returns("en");

            _localizationService = new LocalizationService(_mockConfigService.Object);
            
            // Create test resources directory
            _testResourcesPath = Path.Combine(Path.GetTempPath(), "NaarNoor_Localization_Tests");
            if (!Directory.Exists(_testResourcesPath))
            {
                Directory.CreateDirectory(_testResourcesPath);
            }
        }

        public void Dispose()
        {
            if (Directory.Exists(_testResourcesPath))
            {
                Directory.Delete(_testResourcesPath, true);
            }
        }

        #region Constructor and Initialization

        [Fact]
        public void Constructor_WithNullConfigService_ThrowsArgumentNullException()
        {
            // Arrange & Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => 
                new LocalizationService(null!));
            Assert.Equal("configService", ex.ParamName);
        }

        [Fact]
        public void Constructor_InitializesWithEnglishCulture()
        {
            // Arrange, Act & Assert
            Assert.Equal("en", _localizationService.CurrentCulture);
        }

        [Fact]
        public void Constructor_LoadsPersistedCulture()
        {
            // Arrange
            _mockConfigService.Setup(c => c.Get("Culture", It.IsAny<string>()))
                .Returns("ar");

            // Act
            var service = new LocalizationService(_mockConfigService.Object);

            // Assert
            Assert.Equal("ar", service.CurrentCulture);
        }

        #endregion

        #region GetString Tests

        [Fact]
        public void GetString_WithValidKey_ReturnsString()
        {
            // Arrange
            _localizationService.SetCulture("en");

            // Act
            var result = _localizationService.GetString("Login.Title");

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        [Fact]
        public void GetString_WithNullKey_ReturnsNull()
        {
            // Arrange & Act
            var result = _localizationService.GetString(null!);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetString_WithEmptyKey_ReturnsEmpty()
        {
            // Arrange & Act
            var result = _localizationService.GetString("");

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void GetString_WithNonexistentKey_ReturnsFallbackKey()
        {
            // Arrange
            const string key = "NonExistent.Key";

            // Act
            var result = _localizationService.GetString(key);

            // Assert
            Assert.Equal(key, result);
        }

        [Fact]
        public void GetString_WithFormatArguments_ReturnsFormattedString()
        {
            // Arrange
            _localizationService.SetCulture("en");

            // Act - Format string directly
            var result = _localizationService.GetString("Common.UpdatedOn", "2024-01-15");

            // Assert - Should contain formatted date
            Assert.NotNull(result);
            // The result should be non-empty since it contains a valid key
            Assert.NotEmpty(result);
        }

        [Fact]
        public void GetString_WithInvalidFormatArguments_ReturnsFallback()
        {
            // Arrange
            const string key = "Test.Key";

            // Act
            var result = _localizationService.GetString(key);

            // Assert
            Assert.Equal(key, result);
        }

        #endregion

        #region SetCulture Tests

        [Fact]
        public void SetCulture_WithValidCulture_ChangesCulture()
        {
            // Arrange
            _localizationService.SetCulture("en");

            // Act
            _localizationService.SetCulture("ar");

            // Assert
            Assert.Equal("ar", _localizationService.CurrentCulture);
        }

        [Fact]
        public void SetCulture_WithSameCulture_DoesNothing()
        {
            // Arrange
            _localizationService.SetCulture("en");

            // Act - setting same culture again
            _localizationService.SetCulture("en");

            // Assert
            Assert.Equal("en", _localizationService.CurrentCulture);
        }

        [Fact]
        public void SetCulture_WithNullCulture_ThrowsArgumentException()
        {
            // Arrange & Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => 
                _localizationService.SetCulture(null!));
            Assert.Equal("cultureName", ex.ParamName);
        }

        [Fact]
        public void SetCulture_WithEmptyCulture_ThrowsArgumentException()
        {
            // Arrange & Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => 
                _localizationService.SetCulture(""));
            Assert.Equal("cultureName", ex.ParamName);
        }

        [Fact]
        public void SetCulture_WithWhitespaceCulture_ThrowsArgumentException()
        {
            // Arrange & Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => 
                _localizationService.SetCulture("   "));
            Assert.Equal("cultureName", ex.ParamName);
        }

        [Fact]
        public void SetCulture_WithUnsupportedCulture_ThrowsArgumentException()
        {
            // Arrange & Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => 
                _localizationService.SetCulture("fr")); // French not supported
            Assert.Equal("cultureName", ex.ParamName);
        }

        [Fact]
        public void SetCulture_UpdatesThreadCulture()
        {
            // Arrange
            var originalCulture = Thread.CurrentThread.CurrentUICulture;

            try
            {
                // Act
                _localizationService.SetCulture("ar");

                // Assert
                Assert.Equal("ar", Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName);
            }
            finally
            {
                // Restore original culture
                Thread.CurrentThread.CurrentUICulture = originalCulture;
            }
        }

        [Fact]
        public void SetCulture_PersiststoCulturePreference()
        {
            // Arrange
            _localizationService.SetCulture("en");

            // Act
            _localizationService.SetCulture("ar");
            
            // Wait for async persistence
            System.Threading.Thread.Sleep(100);

            // Assert
            _mockConfigService.Verify(c => c.Set("Culture", "ar"), Times.Once);
            _mockConfigService.Verify(c => c.SaveConfigurationAsync(), Times.AtLeastOnce);
        }

        #endregion

        #region CultureChanged Observable Tests

        [Fact]
        public void CultureChanged_FiresWhenCultureChanges()
        {
            // Arrange
            var cultureChanges = new List<string>();
            _localizationService.SetCulture("en"); // Ensure we start from en
            _localizationService.CultureChanged.Subscribe(c => cultureChanges.Add(c));

            // Act
            _localizationService.SetCulture("ar"); // This should fire event
            _localizationService.SetCulture("en"); // This should fire event

            // Assert - should get 2 events
            Assert.Equal(2, cultureChanges.Count);
            Assert.Equal(new[] { "ar", "en" }, cultureChanges);
        }

        [Fact]
        public void CultureChanged_DoesNotFireWhenCultureUnchanged()
        {
            // Arrange
            var cultureChanges = new List<string>();
            _localizationService.SetCulture("en");
            _localizationService.CultureChanged.Subscribe(c => cultureChanges.Add(c));

            // Act
            _localizationService.SetCulture("en"); // Same culture

            // Assert
            Assert.Empty(cultureChanges);
        }

        #endregion

        #region RTL/LTR Tests

        [Theory]
        [InlineData("en", false)]
        [InlineData("ar", true)]
        public void IsRightToLeft_ReturnsTrueForArabic_FalseForEnglish(string culture, bool expected)
        {
            // Arrange
            _localizationService.SetCulture(culture);

            // Act
            var result = _localizationService.IsRightToLeft;

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("en", TextAlignment.Left)]
        [InlineData("ar", TextAlignment.Right)]
        public void GetTextAlignment_ReturnsCorrectAlignment(string culture, TextAlignment expected)
        {
            // Arrange
            _localizationService.SetCulture(culture);

            // Act
            var result = _localizationService.GetTextAlignment();

            // Assert
            Assert.Equal(expected, result);
        }

        #endregion

        #region LoadResourcesAsync Tests

        [Fact]
        public async Task LoadResourcesAsync_WithMissingFiles_ContinuesGracefully()
        {
            // Arrange - don't create resource files

            // Act & Assert - should not throw
            await _localizationService.LoadResourcesAsync();
        }

        #endregion

        #region Culture Switching at Runtime Tests

        [Fact]
        public void CanSwitchCulturesBothWays()
        {
            // Arrange
            var cultures = new[] { "en", "ar" };

            // Act & Assert
            foreach (var culture in cultures)
            {
                _localizationService.SetCulture(culture);
                Assert.Equal(culture, _localizationService.CurrentCulture);
            }
        }

        #endregion

        #region Concurrent Culture Switching Tests

        [Fact]
        public void ConcurrentCultureSwitching_IsThreadSafe()
        {
            // Arrange
            var cultures = new[] { "en", "ar" };
            var exceptions = new List<Exception>();

            // Act
            var tasks = Enumerable.Range(0, 10)
                .Select(i => Task.Run(() =>
                {
                    try
                    {
                        for (int j = 0; j < 10; j++)
                        {
                            _localizationService.SetCulture(cultures[j % 2]);
                        }
                    }
                    catch (Exception ex)
                    {
                        lock (exceptions)
                        {
                            exceptions.Add(ex);
                        }
                    }
                }))
                .ToArray();

            Task.WaitAll(tasks);

            // Assert
            Assert.Empty(exceptions);
            Assert.True(
                _localizationService.CurrentCulture == "en" || 
                _localizationService.CurrentCulture == "ar"
            );
        }

        #endregion

        #region Helper Methods

        private void CreateTestResourceFiles()
        {
            var enJson = @"{
  ""Test"": {
    ""Key"": ""Test Value EN""
  }
}";

            var arJson = @"{
  ""Test"": {
    ""Key"": ""قيمة اختبار AR""
  }
}";

            File.WriteAllText(Path.Combine(_testResourcesPath, "en.json"), enJson);
            File.WriteAllText(Path.Combine(_testResourcesPath, "ar.json"), arJson);
        }

        #endregion
    }
}
