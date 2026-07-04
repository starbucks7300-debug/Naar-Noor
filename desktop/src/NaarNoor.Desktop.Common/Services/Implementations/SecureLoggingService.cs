using System.Text.RegularExpressions;
using NaarNoor.Desktop.Common.Services.Interfaces;

namespace NaarNoor.Desktop.Common.Services.Implementations;

/// <summary>
/// Service for secure logging that prevents logging of sensitive information.
/// Implements automatic redaction of passwords, tokens, and other PII.
/// </summary>
public class SecureLoggingService : ISecureLoggingService
{
    private readonly Dictionary<string, Regex> _sensitivePatterns = new();
    private readonly object _lockObject = new();

    public SecureLoggingService()
    {
        // Initialize default sensitive patterns
        InitializeDefaultPatterns();
    }

    public void LogInfo(string message, params object?[] args)
    {
        var redactedMessage = RedactSensitiveData(message);
        var redactedArgs = RedactSensitiveArgs(args);
        System.Diagnostics.Debug.WriteLine($"[INFO] {string.Format(redactedMessage, redactedArgs)}");
    }

    public void LogWarning(string message, params object?[] args)
    {
        var redactedMessage = RedactSensitiveData(message);
        var redactedArgs = RedactSensitiveArgs(args);
        System.Diagnostics.Debug.WriteLine($"[WARNING] {string.Format(redactedMessage, redactedArgs)}");
    }

    public void LogError(string message, params object?[] args)
    {
        var redactedMessage = RedactSensitiveData(message);
        var redactedArgs = RedactSensitiveArgs(args);
        System.Diagnostics.Debug.WriteLine($"[ERROR] {string.Format(redactedMessage, redactedArgs)}");
    }

    public void LogError(Exception exception, string? message = null)
    {
        var errorMessage = message ?? "An error occurred";
        var redactedMessage = RedactSensitiveData(errorMessage);

        var stackTrace = exception.StackTrace ?? "No stack trace available";
        var redactedStackTrace = RedactSensitiveData(stackTrace);

        var errorLog = $"[ERROR] {redactedMessage}\n" +
                       $"Exception: {exception.GetType().Name}\n" +
                       $"Message: {RedactSensitiveData(exception.Message)}\n" +
                       $"StackTrace: {redactedStackTrace}";

        System.Diagnostics.Debug.WriteLine(errorLog);
    }

    public void LogDebug(string message, params object?[] args)
    {
#if DEBUG
        var redactedMessage = RedactSensitiveData(message);
        var redactedArgs = RedactSensitiveArgs(args);
        System.Diagnostics.Debug.WriteLine($"[DEBUG] {string.Format(redactedMessage, redactedArgs)}");
#endif
    }

    public void AddSensitiveDataPattern(string pattern, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(pattern))
            throw new ArgumentException("Pattern cannot be null or empty", nameof(pattern));

        if (string.IsNullOrWhiteSpace(fieldName))
            throw new ArgumentException("Field name cannot be null or empty", nameof(fieldName));

        lock (_lockObject)
        {
            try
            {
                _sensitivePatterns[fieldName] = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException($"Invalid regex pattern for {fieldName}", nameof(pattern), ex);
            }
        }
    }

    public string RedactSensitiveData(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        var result = input;

        lock (_lockObject)
        {
            foreach (var (fieldName, pattern) in _sensitivePatterns)
            {
                // Replace matched patterns with redacted version
                result = pattern.Replace(result, match =>
                {
                    // Keep the first 3 characters visible for debugging
                    if (match.Value.Length > 3)
                        return match.Value.Substring(0, 3) + new string('*', Math.Min(match.Value.Length - 3, 10));
                    return new string('*', match.Value.Length);
                });
            }
        }

        return result;
    }

    /// <summary>
    /// Redacts sensitive data from an array of objects.
    /// </summary>
    private object?[] RedactSensitiveArgs(object?[] args)
    {
        if (args == null || args.Length == 0)
            return args;

        var redactedArgs = new object?[args.Length];

        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] is string str)
            {
                redactedArgs[i] = RedactSensitiveData(str);
            }
            else if (args[i] != null && args[i]!.GetType().IsClass && args[i]!.GetType() != typeof(string))
            {
                // For complex objects, just redact their string representation
                redactedArgs[i] = RedactSensitiveData(args[i].ToString() ?? string.Empty);
            }
            else
            {
                redactedArgs[i] = args[i];
            }
        }

        return redactedArgs;
    }

    /// <summary>
    /// Initializes default sensitive data patterns.
    /// </summary>
    private void InitializeDefaultPatterns()
    {
        // Password patterns
        AddSensitiveDataPattern(@"(?:password|pwd)\s*[=:]\s*[""']?([^""'\s]+)[""']?", "password");

        // Bearer token patterns
        AddSensitiveDataPattern(@"(?:bearer|token)\s+[A-Za-z0-9\-._~+/]+=*", "token");

        // JWT token patterns
        AddSensitiveDataPattern(@"eyJ[A-Za-z0-9_-]*\.eyJ[A-Za-z0-9_-]*\.[A-Za-z0-9_-]*", "jwt_token");

        // API key patterns
        AddSensitiveDataPattern(@"(?:api[_-]?key|apikey)\s*[=:]\s*[""']?([^""'\s]+)[""']?", "api_key");

        // Credit card patterns (simple check for 16 digits)
        AddSensitiveDataPattern(@"\b\d{4}[\s-]?\d{4}[\s-]?\d{4}[\s-]?\d{4}\b", "credit_card");

        // Social Security Number patterns
        AddSensitiveDataPattern(@"\b\d{3}-\d{2}-\d{4}\b", "ssn");

        // Email addresses (partial redaction)
        AddSensitiveDataPattern(@"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}", "email");
    }
}
