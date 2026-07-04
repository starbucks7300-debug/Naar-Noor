using System.Text.RegularExpressions;
using NaarNoor.Desktop.Common.Services.Interfaces;

namespace NaarNoor.Desktop.Common.Services.Implementations;

/// <summary>
/// Service for validating user input and preventing injection attacks.
/// Implements OWASP input validation guidelines.
/// </summary>
public class InputValidationService : IInputValidationService
{
    // Regex patterns for validation
    private static readonly Regex EmailPattern = new(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", RegexOptions.Compiled);
    private static readonly Regex PhonePattern = new(@"^\+?[1-9]\d{1,14}$", RegexOptions.Compiled);
    private static readonly Regex UrlPattern = new(@"^https?://[a-zA-Z0-9\-._~:/?#\[\]@!$&'()*+,;=]+$", RegexOptions.Compiled);
    private static readonly Regex NumericPattern = new(@"^-?\d+(\.\d+)?$", RegexOptions.Compiled);
    private static readonly Regex AlphanumericPattern = new(@"^[a-zA-Z0-9\s\-_]*$", RegexOptions.Compiled);

    // SQL injection keywords to detect
    private static readonly string[] SqlKeywords = 
    {
        "SELECT", "INSERT", "UPDATE", "DELETE", "DROP", "CREATE", "ALTER",
        "EXEC", "EXECUTE", "UNION", "OR", "AND", "DECLARE", "CAST", "CONVERT"
    };

    // XPath injection patterns
    private static readonly string[] XPathDangerousChars = { "'", "\"", "[", "]", "(", ")" };

    public bool ValidateText(string? input, int maxLength = 255, bool allowSpecialChars = false)
    {
        if (string.IsNullOrWhiteSpace(input))
            return false;

        if (input.Length > maxLength)
            return false;

        if (!allowSpecialChars && !AlphanumericPattern.IsMatch(input))
            return false;

        return ValidateAgainstSqlInjection(input) && ValidateAgainstXPathInjection(input);
    }

    public bool ValidateEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        if (email.Length > 254)  // RFC 5321
            return false;

        return EmailPattern.IsMatch(email) && ValidateAgainstSqlInjection(email);
    }

    public bool ValidatePhoneNumber(string? phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return false;

        // Remove common formatting characters
        var cleaned = Regex.Replace(phoneNumber, @"[\s\-().+]", "");

        return PhonePattern.IsMatch(cleaned) && ValidateAgainstSqlInjection(cleaned);
    }

    public bool ValidateUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return false;

        try
        {
            var uri = new Uri(url);
            // Only allow HTTP and HTTPS
            if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
                return false;

            return UrlPattern.IsMatch(url) && ValidateAgainstSqlInjection(url);
        }
        catch (UriFormatException)
        {
            return false;
        }
    }

    public bool ValidateNumeric(string? input, decimal? minValue = null, decimal? maxValue = null)
    {
        if (string.IsNullOrWhiteSpace(input))
            return false;

        if (!NumericPattern.IsMatch(input))
            return false;

        if (!decimal.TryParse(input, out var value))
            return false;

        if (minValue.HasValue && value < minValue)
            return false;

        if (maxValue.HasValue && value > maxValue)
            return false;

        return ValidateAgainstSqlInjection(input);
    }

    public string SanitizeText(string? input, int maxLength = 255)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        // Trim to max length
        var sanitized = input.Length > maxLength ? input[..maxLength] : input;

        // Remove null bytes
        sanitized = sanitized.Replace("\0", string.Empty);

        // Remove control characters except newline and tab
        sanitized = Regex.Replace(sanitized, @"[\x00-\x08\x0B\x0C\x0E-\x1F]", string.Empty);

        return sanitized.Trim();
    }

    public string EscapeSpecialChars(string? input, EscapeContext context)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        return context switch
        {
            EscapeContext.Sql => EscapeSql(input),
            EscapeContext.Xml => System.Web.HttpUtility.HtmlEncode(input),
            EscapeContext.JavaScript => EscapeJavaScript(input),
            EscapeContext.Ldap => EscapeLdap(input),
            _ => input
        };
    }

    public bool ValidateAgainstSqlInjection(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return true;

        // Convert to uppercase for case-insensitive comparison
        var upperInput = input.ToUpperInvariant();

        // Check for SQL keywords combined with common injection patterns
        var suspiciousPatterns = new[]
        {
            "'; DROP",
            "'; DELETE",
            "1' OR '1'='1",
            "1' UNION",
            "'; EXEC",
            "-- ",
            "/* ",
            "*/",
            "xp_",
            "sp_"
        };

        // Check for direct keyword presence with injection indicators
        foreach (var pattern in suspiciousPatterns)
        {
            if (upperInput.Contains(pattern.ToUpperInvariant(), StringComparison.OrdinalIgnoreCase))
                return false;
        }

        return true;
    }

    public bool ValidateAgainstXPathInjection(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return true;

        // Check for XPath injection patterns
        var suspiciousPatterns = new[]
        {
            "' or '1'='1",
            "' or \"1\"=\"1",
            "' or 1=1",
            "'] or [",
            "*",
            "or //"
        };

        foreach (var pattern in suspiciousPatterns)
        {
            if (input.Contains(pattern, StringComparison.OrdinalIgnoreCase))
                return false;
        }

        return true;
    }

    /// <summary>
    /// Escapes SQL special characters.
    /// </summary>
    private static string EscapeSql(string input)
    {
        // Replace single quotes with two single quotes (SQL standard)
        return input.Replace("'", "''");
    }

    /// <summary>
    /// Escapes JavaScript special characters.
    /// </summary>
    private static string EscapeJavaScript(string input)
    {
        return input
            .Replace("\\", "\\\\")
            .Replace("\"", "\\\"")
            .Replace("'", "\\'")
            .Replace("\n", "\\n")
            .Replace("\r", "\\r")
            .Replace("\t", "\\t")
            .Replace("\b", "\\b")
            .Replace("\f", "\\f");
    }

    /// <summary>
    /// Escapes LDAP special characters.
    /// </summary>
    private static string EscapeLdap(string input)
    {
        var ldapChars = new[] { '*', '(', ')', '\\', '\0' };
        var result = input;

        foreach (var @char in ldapChars)
        {
            result = result.Replace(@char.ToString(), $"\\{Convert.ToInt32(@char):x2}");
        }

        return result;
    }
}
