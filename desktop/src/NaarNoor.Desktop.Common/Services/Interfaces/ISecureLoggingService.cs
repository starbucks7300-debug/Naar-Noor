namespace NaarNoor.Desktop.Common.Services.Interfaces;

/// <summary>
/// Service for secure logging that prevents logging of sensitive data.
/// Implements filtering and redaction of confidential information.
/// </summary>
public interface ISecureLoggingService
{
    /// <summary>
    /// Logs an information message with sensitive data filtering.
    /// </summary>
    void LogInfo(string message, params object?[] args);

    /// <summary>
    /// Logs a warning message with sensitive data filtering.
    /// </summary>
    void LogWarning(string message, params object?[] args);

    /// <summary>
    /// Logs an error message with sensitive data filtering.
    /// </summary>
    void LogError(string message, params object?[] args);

    /// <summary>
    /// Logs an error with exception details, filtering sensitive data from stack traces.
    /// </summary>
    void LogError(Exception exception, string? message = null);

    /// <summary>
    /// Logs a debug message (only in debug builds) with sensitive data filtering.
    /// </summary>
    void LogDebug(string message, params object?[] args);

    /// <summary>
    /// Adds a pattern for sensitive data that should be redacted from logs.
    /// </summary>
    /// <param name="pattern">Regex pattern for matching sensitive data</param>
    /// <param name="fieldName">Name of the sensitive field</param>
    void AddSensitiveDataPattern(string pattern, string fieldName);

    /// <summary>
    /// Redacts sensitive information from a string.
    /// </summary>
    string RedactSensitiveData(string input);
}
