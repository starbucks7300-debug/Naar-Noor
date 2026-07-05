using NaarNoor.Desktop.Common.Services.Interfaces;
using System.Windows.Forms;

namespace NaarNoor.Desktop.WinForms.Utilities
{
    /// <summary>
    /// Helper class for applying localization and RTL/LTR layout adjustments to WinForms controls
    /// Provides utilities for culture-aware UI configuration
    /// Requirements: REQ-121, REQ-122
    /// </summary>
    public static class LocalizationHelper
    {
        /// <summary>
        /// Apply RTL/LTR layout settings to a form based on current culture
        /// For Arabic (RTL): mirrors layout, sets RightToLeft to Yes, adjusts control alignment
        /// For English (LTR): normal layout, sets RightToLeft to No
        /// Requirements: REQ-121, REQ-122
        /// </summary>
        public static void ApplyLayoutDirection(Control control, ILocalizationService localizationService)
        {
            if (control == null || localizationService == null)
                return;

            var isRTL = localizationService.IsRightToLeft;
            
            if (control is Form form)
            {
                form.RightToLeft = isRTL ? RightToLeft.Yes : RightToLeft.No;
                form.RightToLeftLayout = isRTL;
            }
            else
            {
                control.RightToLeft = isRTL ? RightToLeft.Yes : RightToLeft.No;
            }

            // Recursively apply to all child controls
            ApplyLayoutDirectionRecursive(control, isRTL);
        }

        /// <summary>
        /// Recursively apply RTL/LTR settings to all child controls
        /// </summary>
        private static void ApplyLayoutDirectionRecursive(Control parent, bool isRTL)
        {
            foreach (Control child in parent.Controls)
            {
                child.RightToLeft = isRTL ? RightToLeft.Yes : RightToLeft.No;

                // Adjust text alignment for labels and other text controls
                if (child is Label label)
                {
                    label.TextAlign = isRTL ? ContentAlignment.TopRight : ContentAlignment.TopLeft;
                }
                else if (child is Button button)
                {
                    button.TextAlign = isRTL ? ContentAlignment.MiddleRight : ContentAlignment.MiddleLeft;
                }
                else if (child is TextBox textBox)
                {
                    textBox.TextAlign = isRTL ? HorizontalAlignment.Right : HorizontalAlignment.Left;
                }
                else if (child is RichTextBox richTextBox)
                {
                    richTextBox.RightToLeft = isRTL ? RightToLeft.Yes : RightToLeft.No;
                }

                // Recursively apply to container controls
                if (child.HasChildren)
                {
                    ApplyLayoutDirectionRecursive(child, isRTL);
                }
            }
        }

        /// <summary>
        /// Get localized text for a control from the localization service
        /// </summary>
        public static string GetLocalizedText(string resourceKey, ILocalizationService localizationService)
        {
            return localizationService?.GetString(resourceKey) ?? resourceKey;
        }

        /// <summary>
        /// Update all localized text in a control and its children
        /// Should be called after culture change
        /// </summary>
        public static void UpdateLocalizedStrings(Control control, ILocalizationService localizationService, Dictionary<Control, string>? controlResourceKeys = null)
        {
            if (control == null || localizationService == null)
                return;

            // Update the control itself if it has a resource key
            if (controlResourceKeys?.TryGetValue(control, out var key) == true)
            {
                control.Text = GetLocalizedText(key, localizationService);
            }

            // Update all child controls
            UpdateLocalizedStringsRecursive(control, localizationService, controlResourceKeys);
        }

        /// <summary>
        /// Recursively update localized strings for all child controls
        /// </summary>
        private static void UpdateLocalizedStringsRecursive(Control parent, ILocalizationService localizationService, Dictionary<Control, string>? controlResourceKeys = null)
        {
            foreach (Control child in parent.Controls)
            {
                if (controlResourceKeys?.TryGetValue(child, out var key) == true)
                {
                    child.Text = GetLocalizedText(key, localizationService);
                }

                if (child.HasChildren)
                {
                    UpdateLocalizedStringsRecursive(child, localizationService, controlResourceKeys);
                }
            }
        }

        /// <summary>
        /// Apply FlowLayoutPanel for flexible RTL/LTR layouts
        /// For Arabic: uses RightToLeft flow
        /// For English: uses LeftToRight flow
        /// </summary>
        public static void ConfigureFlowPanel(FlowLayoutPanel panel, ILocalizationService localizationService)
        {
            if (panel == null || localizationService == null)
                return;

            var isRTL = localizationService.IsRightToLeft;
            panel.FlowDirection = isRTL ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
            panel.RightToLeft = isRTL ? RightToLeft.Yes : RightToLeft.No;
        }
    }
}
