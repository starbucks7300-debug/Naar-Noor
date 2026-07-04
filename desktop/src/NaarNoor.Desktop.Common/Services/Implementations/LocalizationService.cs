using System.Globalization;
using System.Reactive.Subjects;
using System.Text.Json;
using System.Text.Json.Serialization;
using NaarNoor.Desktop.Common.Configuration;
using NaarNoor.Desktop.Common.Services.Interfaces;

namespace NaarNoor.Desktop.Common.Services
{
    /// <summary>
    /// Implementation of localization service for multilingual support
    /// Handles runtime culture switching without application restart
    /// Loads resources from JSON files (Resources/en.json, Resources/ar.json)
    /// Requirements: REQ-121, REQ-122
    /// </summary>
    public class LocalizationService : ILocalizationService
    {
        private readonly Subject<string> _cultureChanged = new();
        private readonly IConfigurationService _configService;
        private string _currentCulture = "en";
        private Dictionary<string, Dictionary<string, object>> _resources = new();

        public string CurrentCulture => _currentCulture;
        public IObservable<string> CultureChanged => _cultureChanged;

        public LocalizationService(IConfigurationService configService)
        {
            _configService = configService ?? throw new ArgumentNullException(nameof(configService));
            
            // Initialize with English as default
            _resources["en"] = new Dictionary<string, object>();
            _resources["ar"] = new Dictionary<string, object>();
            
            // Load persisted culture preference
            _currentCulture = _configService.Get("Culture", "en") ?? "en";
            UpdateThreadCulture(_currentCulture);
        }

        /// <summary>
        /// Flatten a nested dictionary into dot-notation keys
        /// Example: { "Login": { "Button": "Login" } } => { "Login.Button": "Login" }
        /// </summary>
        private void FlattenDictionary(Dictionary<string, object> source, string prefix, Dictionary<string, object> result)
        {
            foreach (var kvp in source)
            {
                var key = string.IsNullOrEmpty(prefix) ? kvp.Key : $"{prefix}.{kvp.Key}";
                
                if (kvp.Value is JsonElement element)
                {
                    if (element.ValueKind == JsonValueKind.Object)
                    {
                        var nestedDict = JsonSerializer.Deserialize<Dictionary<string, object>>(element.GetRawText()) ?? new();
                        FlattenDictionary(nestedDict, key, result);
                    }
                    else if (element.ValueKind == JsonValueKind.String)
                    {
                        result[key] = element.GetString() ?? "";
                    }
                    else if (element.ValueKind == JsonValueKind.Number)
                    {
                        result[key] = element.GetRawText();
                    }
                }
                else if (kvp.Value is Dictionary<string, object> nested)
                {
                    FlattenDictionary(nested, key, result);
                }
                else if (kvp.Value != null)
                {
                    result[key] = kvp.Value;
                }
            }
        }

        /// <summary>
        /// Get localized string for a given key
        /// Supports dot notation (e.g., "Login.Button")
        /// </summary>
        public string GetString(string key)
        {
            if (string.IsNullOrEmpty(key))
                return key;

            if (_resources.TryGetValue(_currentCulture, out var dict) && dict.TryGetValue(key, out var value))
            {
                return value?.ToString() ?? key;
            }

            // Fallback to English
            if (_resources.TryGetValue("en", out var enDict) && enDict.TryGetValue(key, out var enValue))
            {
                return enValue?.ToString() ?? key;
            }

            // Return key as fallback
            return key;
        }

        /// <summary>
        /// Get localized string with format arguments
        /// </summary>
        public string GetString(string key, params object[] args)
        {
            try
            {
                return string.Format(GetString(key), args);
            }
            catch
            {
                return key;
            }
        }

        /// <summary>
        /// Set the current culture for the application (runtime switching without restart)
        /// Updates Thread.CurrentThread.CurrentUICulture and persists preference to app-config.json
        /// Triggers CultureChanged event to notify all UI elements to reload strings
        /// Requirements: REQ-121, REQ-122
        /// </summary>
        public void SetCulture(string cultureName)
        {
            if (string.IsNullOrWhiteSpace(cultureName))
            {
                throw new ArgumentException("Culture name cannot be null or empty", nameof(cultureName));
            }

            // Validate culture exists
            if (!_resources.ContainsKey(cultureName))
            {
                throw new ArgumentException($"Culture '{cultureName}' is not supported", nameof(cultureName));
            }

            if (_currentCulture == cultureName)
            {
                return; // Already set to this culture
            }

            // Update current culture
            _currentCulture = cultureName;

            // Update thread culture for date/time/number formatting
            UpdateThreadCulture(cultureName);

            // Persist culture preference to configuration
            _configService.Set("Culture", cultureName);

            // Fire and forget persistence of configuration
            _ = Task.Run(async () =>
            {
                try
                {
                    await _configService.SaveConfigurationAsync();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to persist culture preference: {ex.Message}");
                }
            });

            // Notify all subscribers of culture change
            // This allows UI elements to react and reload strings
            _cultureChanged.OnNext(cultureName);
        }

        /// <summary>
        /// Update the current thread's UI culture
        /// </summary>
        private void UpdateThreadCulture(string cultureName)
        {
            try
            {
                Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(cultureName);
            }
            catch (CultureNotFoundException)
            {
                // Fallback to en if the culture is not valid
                Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en");
            }
        }

        /// <summary>
        /// Gets whether the current culture is right-to-left (Arabic)
        /// </summary>
        /// <returns>True if current culture is RTL, false if LTR</returns>
        public bool IsRightToLeft => _currentCulture == "ar";

        /// <summary>
        /// Gets the text alignment for the current culture
        /// </summary>
        /// <returns>TextAlignment.Left for LTR (en), TextAlignment.Right for RTL (ar)</returns>
        public TextAlignment GetTextAlignment()
        {
            return IsRightToLeft 
                ? TextAlignment.Right
                : TextAlignment.Left;
        }

        /// <summary>
        /// Load all localization resources from JSON files
        /// Searches for Resources/en.json and Resources/ar.json
        /// Requirements: REQ-121, REQ-122
        /// </summary>
        public async Task LoadResourcesAsync()
        {
            try
            {
                // Try to load from application data directory first
                var appDataPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "NaarNoor"
                );

                // Fallback to current directory or embedded resources
                var basePath = appDataPath;
                if (!Directory.Exists(basePath))
                {
                    basePath = Path.Combine(AppContext.BaseDirectory, "Resources");
                }

                await LoadResourcesFromPath("en", basePath);
                await LoadResourcesFromPath("ar", basePath);

                // If resources weren't loaded, they'll remain as empty dictionaries
                // This allows the app to continue with fallback to key names
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading localization resources: {ex.Message}");
                // Continue without throwing - will use key names as fallback
            }
        }

        /// <summary>
        /// Load resources for a specific culture from JSON file
        /// </summary>
        private async Task LoadResourcesFromPath(string culture, string basePath)
        {
            var filePath = Path.Combine(basePath, $"{culture}.json");
            
            if (!File.Exists(filePath))
            {
                // Try alternate location
                filePath = Path.Combine(AppContext.BaseDirectory, "Resources", $"{culture}.json");
            }

            if (File.Exists(filePath))
            {
                var json = await File.ReadAllTextAsync(filePath);
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var rawResources = JsonSerializer.Deserialize<Dictionary<string, object>>(json, options);
                
                if (rawResources != null)
                {
                    // Flatten the nested structure to dot notation
                    var flatResources = new Dictionary<string, object>();
                    FlattenDictionary(rawResources, "", flatResources);
                    _resources[culture] = flatResources;
                }
            }
        }
    }
}
