namespace NaarNoor.Desktop.Common.Configuration
{
    /// <summary>
    /// Application configuration model for persisting user preferences
    /// </summary>
    public class AppConfiguration
    {
        /// <summary>
        /// Current culture/language preference (e.g., "en", "ar")
        /// </summary>
        public string Culture { get; set; } = "en";

        /// <summary>
        /// API base URL for the backend service
        /// </summary>
        public string? ApiBaseUrl { get; set; }

        /// <summary>
        /// Cache TTL in minutes
        /// </summary>
        public int CacheTtlMinutes { get; set; } = 30;

        /// <summary>
        /// Enable offline mode capability
        /// </summary>
        public bool EnableOfflineMode { get; set; } = true;

        /// <summary>
        /// Application theme preference (light/dark)
        /// </summary>
        public string Theme { get; set; } = "light";
    }
}
