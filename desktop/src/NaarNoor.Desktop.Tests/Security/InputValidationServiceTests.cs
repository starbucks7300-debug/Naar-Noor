using NaarNoor.Desktop.Common.Services.Implementations;
using NaarNoor.Desktop.Common.Services.Interfaces;
using Xunit;

namespace NaarNoor.Desktop.Tests.Security;

/// <summary>
/// Tests for the InputValidationService covering OWASP input validation guidelines.
/// </summary>
public class InputValidationServiceTests
{
    private readonly IInputValidationService _validationService;

    public InputValidationServiceTests()
    {
        _validationService = new InputValidationService();
    }

    [Fact]
    public void ValidateText_WithValidText_ReturnsTrue()
    {
        // Arrange
        var input = "ValidTextInput123";

        // Act
        var result = _validationService.ValidateText(input);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ValidateText_WithSqlInjection_ReturnsFalse()
    {
        // Arrange
        var input = "'; DROP TABLE users; --";

        // Act
        var result = _validationService.ValidateText(input, allowSpecialChars: true);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ValidateText_WithExcessiveLength_ReturnsFalse()
    {
        // Arrange
        var input = new string('a', 300);

        // Act
        var result = _validationService.ValidateText(input, maxLength: 255);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ValidateEmail_WithValidEmail_ReturnsTrue()
    {
        // Arrange
        var email = "user@example.com";

        // Act
        var result = _validationService.ValidateEmail(email);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ValidateEmail_WithInvalidEmail_ReturnsFalse()
    {
        // Arrange
        var email = "not-an-email";

        // Act
        var result = _validationService.ValidateEmail(email);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ValidateEmail_WithSqlInjection_ReturnsFalse()
    {
        // Arrange
        var email = "user@example.com' OR '1'='1";

        // Act
        var result = _validationService.ValidateEmail(email);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ValidatePhoneNumber_WithValidPhone_ReturnsTrue()
    {
        // Arrange
        var phone = "+1234567890";

        // Act
        var result = _validationService.ValidatePhoneNumber(phone);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ValidatePhoneNumber_WithFormattedPhone_ReturnsTrue()
    {
        // Arrange
        var phone = "+1 (234) 567-8900";

        // Act
        var result = _validationService.ValidatePhoneNumber(phone);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ValidateUrl_WithValidUrl_ReturnsTrue()
    {
        // Arrange
        var url = "https://example.com/api/resource";

        // Act
        var result = _validationService.ValidateUrl(url);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ValidateUrl_WithInvalidUrl_ReturnsFalse()
    {
        // Arrange
        var url = "not a url";

        // Act
        var result = _validationService.ValidateUrl(url);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ValidateUrl_WithFileProtocol_ReturnsFalse()
    {
        // Arrange
        var url = "file:///etc/passwd";

        // Act
        var result = _validationService.ValidateUrl(url);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ValidateNumeric_WithValidNumber_ReturnsTrue()
    {
        // Arrange
        var input = "123.45";

        // Act
        var result = _validationService.ValidateNumeric(input);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ValidateNumeric_WithRange_ReturnsTrue()
    {
        // Arrange
        var input = "50";

        // Act
        var result = _validationService.ValidateNumeric(input, 0, 100);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ValidateNumeric_OutOfRange_ReturnsFalse()
    {
        // Arrange
        var input = "150";

        // Act
        var result = _validationService.ValidateNumeric(input, 0, 100);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void SanitizeText_RemovesControlCharacters()
    {
        // Arrange
        var input = "Hello\x00World\x08Test";

        // Act
        var result = _validationService.SanitizeText(input);

        // Assert
        Assert.Equal("HelloWorldTest", result);
    }

    [Fact]
    public void SanitizeText_TruncatesToMaxLength()
    {
        // Arrange
        var input = "This is a very long text that exceeds the maximum allowed length";

        // Act
        var result = _validationService.SanitizeText(input, maxLength: 20);

        // Assert
        Assert.True(result.Length <= 20);
    }

    [Fact]
    public void EscapeSpecialChars_SqlContext_EscapesSingleQuotes()
    {
        // Arrange
        var input = "O'Reilly";

        // Act
        var result = _validationService.EscapeSpecialChars(input, EscapeContext.Sql);

        // Assert
        Assert.Equal("O''Reilly", result);
    }

    [Fact]
    public void ValidateAgainstSqlInjection_WithSuspiciousPattern_ReturnsFalse()
    {
        // Arrange - Use a more obvious injection pattern
        var input = "1' UNION SELECT * FROM users; --";

        // Act
        var result = _validationService.ValidateAgainstSqlInjection(input);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ValidateAgainstSqlInjection_WithDropStatement_ReturnsFalse()
    {
        // Arrange
        var input = "'; DROP TABLE users; --";

        // Act
        var result = _validationService.ValidateAgainstSqlInjection(input);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ValidateAgainstXPathInjection_WithXPathInjection_ReturnsFalse()
    {
        // Arrange
        var input = "' or '1'='1";

        // Act
        var result = _validationService.ValidateAgainstXPathInjection(input);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ValidateAgainstXPathInjection_WithNormalInput_ReturnsTrue()
    {
        // Arrange
        var input = "normal text without injection";

        // Act
        var result = _validationService.ValidateAgainstXPathInjection(input);

        // Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void ValidateText_WithNullOrEmpty_ReturnsFalse(string? input)
    {
        // Act
        var result = _validationService.ValidateText(input);

        // Assert
        Assert.False(result);
    }
}
