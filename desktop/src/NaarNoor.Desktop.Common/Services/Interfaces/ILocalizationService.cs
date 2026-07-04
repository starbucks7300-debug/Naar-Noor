using System;

namespace NaarNoor.Desktop.Common.Services.Interfaces
{
    /// <summary>
    /// Text alignment for display (platform-agnostic)
    /// </summary>
    public enum TextAlignment
    {
        Left,
        Right,
        Center
    }

    /// <summary>
    /// Service for managing application localization and culture switching.
    /// Supports English and Arabic languages with runtime language switching.
    /// Requirements: REQ-121, REQ-122
    /// </summary>
    public interface ILocalizationService
    {
        /// <summary>
        /// Gets a localized string for the given key.
        /// </summary>
        /// <param name="key">The resource key (e.g., "LoginButton", "Welcome")</param>
        /// <returns>The localized string, or the key if not found</returns>
        string GetString(string key);

        /// <summary>
        /// Gets a localized string with formatted arguments.
        /// </summary>
        /// <param name="key">The resource key containing format placeholders</param>
        /// <param name="args">Format arguments</param>
        /// <returns>The formatted localized string</returns>
        string GetString(string key, params object[] args);

        /// <summary>
        /// Sets the current culture for the application.
        /// Triggers CultureChanged event and persists the preference.
        /// </summary>
        /// <param name="cultureName">The culture name (e.g., "en", "ar")</param>
        void SetCulture(string cultureName);

        /// <summary>
        /// Gets the current culture name.
        /// </summary>
        string CurrentCulture { get; }

        /// <summary>
        /// Observable that fires when culture changes.
        /// Provides the new culture name to subscribers.
        /// </summary>
        IObservable<string> CultureChanged { get; }

        /// <summary>
        /// Gets whether the current culture is right-to-left (Arabic).
        /// Used for RTL/LTR layout adjustment in UI forms.
        /// </summary>
        bool IsRightToLeft { get; }

        /// <summary>
        /// Gets the text alignment for the current culture.
        /// Returns TextAlignment.Right for Arabic (RTL), Left for English (LTR).
        /// </summary>
        /// <returns>TextAlignment appropriate for current culture</returns>
        TextAlignment GetTextAlignment();

        /// <summary>
        /// Loads all localization resources from embedded or external resource files.
        /// Should be called during application startup.
        /// </summary>
        Task LoadResourcesAsync();
    }
}
