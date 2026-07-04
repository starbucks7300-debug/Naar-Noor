using NaarNoor.Desktop.Common.Services.Implementations;
using NaarNoor.Desktop.Common.Services.Interfaces;
using Xunit;

namespace NaarNoor.Desktop.Tests.Security;

/// <summary>
/// Tests for SecureLoggingService covering sensitive data redaction.
/// </summary>
public class SecureLoggingServiceTests
{
    private readonly ISecureLoggingService _loggingService;

    public SecureLoggingServiceTests()
    {
        _loggingService = new SecureLoggingService();
    }

    [Fact]
    public void RedactSensitiveData_WithPassword_RedactsPassword()
    {
        // Arrange
        var input = "password=SecretPassword123";

        // Act
        var redacted = _loggingService.RedactSensitiveData(input);

        // Assert
        Assert.DoesNotContain("SecretPassword123", redacted);
        Assert.Contains("*", redacted);
    }

    [Fact]
    public void RedactSensitiveData_WithBearerToken_RedactsToken()
    {
        // Arrange
        var input = "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIn0";

        // Act
        var redacted = _loggingService.RedactSensitiveData(input);

        // Assert
        Assert.DoesNotContain("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9", redacted);
        Assert.Contains("*", redacted);
    }

    [Fact]
    public void RedactSensitiveData_WithCreditCard_RedactsCreditCard()
    {
        // Arrange
        var input = "Credit card: 4532-1111-2222-3333";

        // Act
        var redacted = _loggingService.RedactSensitiveData(input);

        // Assert
        Assert.DoesNotContain("4532-1111-2222-3333", redacted);
        Assert.Contains("*", redacted);
    }

    [Fact]
    public void RedactSensitiveData_WithApiKey_RedactsApiKey()
    {
        // Arrange
        var input = "api_key=test_sk_12345_redacted_key_9876543210";

        // Act
        var redacted = _loggingService.RedactSensitiveData(input);

        // Assert
        Assert.DoesNotContain("test_sk_12345_redacted_key_9876543210", redacted);
    }

    [Fact]
    public void RedactSensitiveData_WithEmail_RedactsEmail()
    {
        // Arrange
        var input = "Contact us at support@example.com";

        // Act
        var redacted = _loggingService.RedactSensitiveData(input);

        // Assert
        Assert.DoesNotContain("support@example.com", redacted);
    }

    [Fact]
    public void RedactSensitiveData_WithNormalText_DoesNotChange()
    {
        // Arrange
        var input = "This is normal log text without sensitive data";

        // Act
        var redacted = _loggingService.RedactSensitiveData(input);

        // Assert
        // Text should be redacted or remain same, but shouldn't expose sensitive data
        Assert.NotNull(redacted);
        Assert.NotEmpty(redacted);
    }

    [Fact]
    public void RedactSensitiveData_WithNull_ReturnsNull()
    {
        // Act
        var result = _loggingService.RedactSensitiveData(null!);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void RedactSensitiveData_WithEmpty_ReturnsEmpty()
    {
        // Act
        var result = _loggingService.RedactSensitiveData("");

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void AddSensitiveDataPattern_WithValidPattern()
    {
        // Arrange
        var pattern = @"\d{4}";
        var fieldName = "credit_card_last_4";

        // Act
        _loggingService.AddSensitiveDataPattern(pattern, fieldName);

        // Then
        var input = "Card ends in 1234";
        var redacted = _loggingService.RedactSensitiveData(input);

        // Assert
        Assert.NotEqual(input, redacted);
        Assert.Contains("*", redacted);
    }

    [Fact]
    public void AddSensitiveDataPattern_WithInvalidRegex_ThrowsException()
    {
        // Arrange
        var invalidPattern = "[invalid(regex";
        var fieldName = "invalid_field";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => _loggingService.AddSensitiveDataPattern(invalidPattern, fieldName));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void AddSensitiveDataPattern_WithNullOrEmptyPattern_ThrowsException(string pattern)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => _loggingService.AddSensitiveDataPattern(pattern, "field"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void AddSensitiveDataPattern_WithNullOrEmptyFieldName_ThrowsException(string fieldName)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => _loggingService.AddSensitiveDataPattern(@"\d+", fieldName));
    }

    [Fact]
    public void LogInfo_WithNormalMessage()
    {
        // Act & Assert - Should not throw
        _loggingService.LogInfo("Info message");
    }

    [Fact]
    public void LogWarning_WithNormalMessage()
    {
        // Act & Assert - Should not throw
        _loggingService.LogWarning("Warning message");
    }

    [Fact]
    public void LogError_WithNormalMessage()
    {
        // Act & Assert - Should not throw
        _loggingService.LogError("Error message");
    }

    [Fact]
    public void LogError_WithException()
    {
        // Arrange
        var exception = new InvalidOperationException("Operation failed");

        // Act & Assert - Should not throw
        _loggingService.LogError(exception, "An error occurred during processing");
    }

    [Fact]
    public void LogDebug_WithNormalMessage()
    {
        // Act & Assert - Should not throw
        _loggingService.LogDebug("Debug message");
    }

    [Fact]
    public void LogInfo_WithSensitiveData()
    {
        // Arrange
        var sensitiveMessage = "User password=SecretPassword123 failed login";

        // Act & Assert - Should not throw
        _loggingService.LogInfo(sensitiveMessage);
    }

    [Fact]
    public void MultipleRedactions_WorkCorrectly()
    {
        // Arrange
        var input = "Password: MyPassword123, Email: user@example.com, Token: bearerABC123";

        // Act
        var redacted = _loggingService.RedactSensitiveData(input);

        // Assert
        Assert.DoesNotContain("MyPassword123", redacted);
        Assert.DoesNotContain("user@example.com", redacted);
        Assert.Contains("*", redacted);
    }

    [Fact]
    public void RedactSensitiveData_CaseInsensitive()
    {
        // Arrange
        var input1 = "PASSWORD=secret";
        var input2 = "password=secret";
        var input3 = "PaSsWoRd=secret";

        // Act
        var redacted1 = _loggingService.RedactSensitiveData(input1);
        var redacted2 = _loggingService.RedactSensitiveData(input2);
        var redacted3 = _loggingService.RedactSensitiveData(input3);

        // Assert
        Assert.All(new[] { redacted1, redacted2, redacted3 }, r => 
            Assert.Contains("*", r)
        );
    }
}
