using System.Globalization;
using System.Reactive.Subjects;
using NaarNoor.Desktop.Common.Configuration;
using NaarNoor.Desktop.Common.Services.Interfaces;

namespace NaarNoor.Desktop.Common.Services
{
    /// <summary>
    /// Implementation of localization service for multilingual support
    /// Handles runtime culture switching without application restart
    /// </summary>
    public class LocalizationService : ILocalizationService
    {
        private readonly Subject<string> _cultureChanged = new();
        private readonly IConfigurationService _configService;
        private string _currentCulture = "en";
        private Dictionary<string, Dictionary<string, string>> _resources = new();

        public string CurrentCulture => _currentCulture;
        public IObservable<string> CultureChanged => _cultureChanged;

        public LocalizationService(IConfigurationService configService)
        {
            _configService = configService ?? throw new ArgumentNullException(nameof(configService));
            
            // Initialize with English as default
            _resources["en"] = new Dictionary<string, string>();
            _resources["ar"] = new Dictionary<string, string>();
            LoadDefaultResources();
            
            // Load persisted culture preference
            _currentCulture = _configService.Get("Culture", "en") ?? "en";
            UpdateThreadCulture(_currentCulture);
        }

        /// <summary>
        /// Get localized string for a given key
        /// </summary>
        public string GetString(string key)
        {
            if (_resources.TryGetValue(_currentCulture, out var dict) && dict.TryGetValue(key, out var value))
            {
                return value;
            }

            // Fallback to English
            if (_resources.TryGetValue("en", out var enDict) && enDict.TryGetValue(key, out var enValue))
            {
                return enValue;
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

            // Note: We don't await here since SetCulture is synchronous.
            // In a real scenario, you might want to handle persistence errors gracefully.
            // For now, we'll fire and forget the persistence.
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
        /// Load all localization resources (no-op for this implementation as resources are loaded in constructor)
        /// </summary>
        public async Task LoadResourcesAsync()
        {
            // Resources are loaded in the constructor
            await Task.CompletedTask;
        }

        /// <summary>
        /// Load default English and Arabic resources
        /// </summary>
        private void LoadDefaultResources()
        {
            // English resources
            _resources["en"]["app.title"] = "Naar-Noor Restaurant Management";
            _resources["en"]["login.title"] = "Welcome";
            _resources["en"]["login.username"] = "Username";
            _resources["en"]["login.password"] = "Password";
            _resources["en"]["login.button"] = "Login";
            _resources["en"]["login.error"] = "Invalid credentials";
            _resources["en"]["dashboard.title"] = "Dashboard";
            _resources["en"]["menu.title"] = "Menu";
            _resources["en"]["reservations.title"] = "Reservations";
            _resources["en"]["staff.title"] = "Staff";
            _resources["en"]["reports.title"] = "Reports";
            _resources["en"]["logout"] = "Logout";
            _resources["en"]["error"] = "Error";
            _resources["en"]["success"] = "Success";
            _resources["en"]["confirm"] = "Confirm";
            _resources["en"]["cancel"] = "Cancel";
            _resources["en"]["delete"] = "Delete";
            _resources["en"]["edit"] = "Edit";
            _resources["en"]["save"] = "Save";
            _resources["en"]["close"] = "Close";
            _resources["en"]["ok"] = "OK";
            _resources["en"]["language"] = "Language";
            _resources["en"]["english"] = "English";
            _resources["en"]["arabic"] = "العربية";
            _resources["en"]["culture.switching"] = "Switching language...";
            _resources["en"]["culture.switched"] = "Language changed successfully";

            // Arabic resources
            _resources["ar"]["app.title"] = "إدارة مطعم نار نور";
            _resources["ar"]["login.title"] = "أهلا وسهلا";
            _resources["ar"]["login.username"] = "اسم المستخدم";
            _resources["ar"]["login.password"] = "كلمة المرور";
            _resources["ar"]["login.button"] = "تسجيل الدخول";
            _resources["ar"]["login.error"] = "بيانات اعتماد غير صحيحة";
            _resources["ar"]["dashboard.title"] = "لوحة التحكم";
            _resources["ar"]["menu.title"] = "القائمة";
            _resources["ar"]["reservations.title"] = "الحجوزات";
            _resources["ar"]["staff.title"] = "الموظفون";
            _resources["ar"]["reports.title"] = "التقارير";
            _resources["ar"]["logout"] = "تسجيل الخروج";
            _resources["ar"]["error"] = "خطأ";
            _resources["ar"]["success"] = "نجح";
            _resources["ar"]["confirm"] = "تأكيد";
            _resources["ar"]["cancel"] = "إلغاء";
            _resources["ar"]["delete"] = "حذف";
            _resources["ar"]["edit"] = "تعديل";
            _resources["ar"]["save"] = "حفظ";
            _resources["ar"]["close"] = "إغلاق";
            _resources["ar"]["ok"] = "حسناً";
            _resources["ar"]["language"] = "اللغة";
            _resources["ar"]["english"] = "English";
            _resources["ar"]["arabic"] = "العربية";
            _resources["ar"]["culture.switching"] = "جاري التبديل إلى اللغة...";
            _resources["ar"]["culture.switched"] = "تم تغيير اللغة بنجاح";
        }
    }
}
