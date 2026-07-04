namespace NaarNoor.Desktop.Common.Services.Interfaces;

/// <summary>
/// Service for validating user input and preventing injection attacks.
/// Implements defense-in-depth validation for all user-provided data.
/// </summary>
public interface IInputValidationService
{
    /// <summary>
    /// Validates and sanitizes a text input to prevent injection attacks.
    /// </summary>
    /// <param name="input">The input text to validate</param>
    /// <param name="maxLength">Maximum allowed length (default: 255)</param>
    /// <param name="allowSpecialChars">Whether to allow special characters</param>
    /// <returns>True if valid, false otherwise</returns>
    bool ValidateText(string? input, int maxLength = 255, bool allowSpecialChars = false);

    /// <summary>
    /// Validates an email address format.
    /// </summary>
    bool ValidateEmail(string? email);

    /// <summary>
    /// Validates a phone number format.
    /// </summary>
    bool ValidatePhoneNumber(string? phoneNumber);

    /// <summary>
    /// Validates a URL to prevent URL injection.
    /// </summary>
    bool ValidateUrl(string? url);

    /// <summary>
    /// Validates numeric input within a range.
    /// </summary>
    bool ValidateNumeric(string? input, decimal? minValue = null, decimal? maxValue = null);

    /// <summary>
    /// Sanitizes input by removing potentially dangerous characters.
    /// </summary>
    string SanitizeText(string? input, int maxLength = 255);

    /// <summary>
    /// Escapes special characters to prevent injection in SQL/XML/HTML contexts.
    /// </summary>
    string EscapeSpecialChars(string? input, EscapeContext context);

    /// <summary>
    /// Validates that input doesn't contain SQL injection patterns.
    /// </summary>
    bool ValidateAgainstSqlInjection(string? input);

    /// <summary>
    /// Validates that input doesn't contain XPath injection patterns.
    /// </summary>
    bool ValidateAgainstXPathInjection(string? input);
}

/// <summary>
/// Context for escaping special characters.
/// </summary>
public enum EscapeContext
{
    /// <summary>Escape for SQL context</summary>
    Sql,

    /// <summary>Escape for XML/HTML context</summary>
    Xml,

    /// <summary>Escape for JavaScript context</summary>
    JavaScript,

    /// <summary>Escape for LDAP context</summary>
    Ldap
}
