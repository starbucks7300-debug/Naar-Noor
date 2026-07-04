using NaarNoor.Desktop.Common.Services.Implementations;
using NaarNoor.Desktop.Common.Services.Interfaces;
using Xunit;

namespace NaarNoor.Desktop.Tests.Security;

/// <summary>
/// Tests for RequestSigningService covering HMAC-SHA256 signing and verification.
/// </summary>
public class RequestSigningServiceTests
{
    private readonly IRequestSigningService _signingService;
    private const string TestSigningKey = "TestSigningKeyForSecurityTests123";

    public RequestSigningServiceTests()
    {
        _signingService = new RequestSigningService(TestSigningKey);
    }

    [Fact]
    public void SignRequest_ReturnsBase64String()
    {
        // Arrange
        var requestBody = "{\"username\":\"testuser\",\"password\":\"testpass\"}";

        // Act
        var signature = _signingService.SignRequest(requestBody);

        // Assert
        Assert.NotNull(signature);
        Assert.NotEmpty(signature);
        // Verify it's valid base64
        var bytes = Convert.FromBase64String(signature);
        Assert.Equal(32, bytes.Length); // SHA256 produces 32 bytes
    }

    [Fact]
    public void SignRequest_SameInput_ProducesSameSignature()
    {
        // Arrange
        var requestBody = "{\"test\":\"data\"}";

        // Act
        var signature1 = _signingService.SignRequest(requestBody);
        var signature2 = _signingService.SignRequest(requestBody);

        // Assert
        Assert.Equal(signature1, signature2);
    }

    [Fact]
    public void SignRequest_DifferentInput_ProducesDifferentSignature()
    {
        // Arrange
        var body1 = "{\"test\":\"data1\"}";
        var body2 = "{\"test\":\"data2\"}";

        // Act
        var signature1 = _signingService.SignRequest(body1);
        var signature2 = _signingService.SignRequest(body2);

        // Assert
        Assert.NotEqual(signature1, signature2);
    }

    [Fact]
    public void VerifyResponse_WithValidSignature_ReturnsTrue()
    {
        // Arrange
        var responseBody = "{\"status\":\"success\"}";
        var signature = _signingService.SignRequest(responseBody);

        // Act
        var isValid = _signingService.VerifyResponse(responseBody, signature);

        // Assert
        Assert.True(isValid);
    }

    [Fact]
    public void VerifyResponse_WithInvalidSignature_ReturnsFalse()
    {
        // Arrange
        var responseBody = "{\"status\":\"success\"}";
        var invalidSignature = "invalid_signature_data";

        // Act
        var isValid = _signingService.VerifyResponse(responseBody, invalidSignature);

        // Assert
        Assert.False(isValid);
    }

    [Fact]
    public void VerifyResponse_WithTamperedBody_ReturnsFalse()
    {
        // Arrange
        var originalBody = "{\"status\":\"success\"}";
        var signature = _signingService.SignRequest(originalBody);
        var tamperedBody = "{\"status\":\"success\",\"admin\":true}";

        // Act
        var isValid = _signingService.VerifyResponse(tamperedBody, signature);

        // Assert
        Assert.False(isValid);
    }

    [Fact]
    public void AddSignatureHeaders_AddsRequiredHeaders()
    {
        // Arrange
        var requestBody = "{\"data\":\"test\"}";
        var headers = new Dictionary<string, string>();

        // Act
        _signingService.AddSignatureHeaders(requestBody, headers);

        // Assert
        Assert.Contains("X-Request-Signature", headers.Keys);
        Assert.Contains("X-Request-Timestamp", headers.Keys);
        Assert.NotEmpty(headers["X-Request-Signature"]);
        Assert.NotEmpty(headers["X-Request-Timestamp"]);
    }

    [Fact]
    public void AddSignatureHeaders_TimestampIsUnixTime()
    {
        // Arrange
        var requestBody = "{\"data\":\"test\"}";
        var headers = new Dictionary<string, string>();
        var beforeTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        // Act
        _signingService.AddSignatureHeaders(requestBody, headers);

        var afterTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var timestamp = long.Parse(headers["X-Request-Timestamp"]);

        // Assert
        Assert.True(timestamp >= beforeTime && timestamp <= afterTime + 1);
    }

    [Theory]
    [InlineData(null)]
    public void SignRequest_WithNull_ThrowsException(string? input)
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _signingService.SignRequest(input!));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void VerifyResponse_WithNullOrEmptySignature_ReturnsFalse(string? signature)
    {
        // Act
        var result = _signingService.VerifyResponse("response body", signature ?? "");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void DifferentServiceInstances_WithSameKey_ProduceSameSignatures()
    {
        // Arrange
        var service1 = new RequestSigningService(TestSigningKey);
        var service2 = new RequestSigningService(TestSigningKey);
        var requestBody = "{\"consistent\":\"signing\"}";

        // Act
        var signature1 = service1.SignRequest(requestBody);
        var signature2 = service2.SignRequest(requestBody);

        // Assert
        Assert.Equal(signature1, signature2);
    }

    [Fact]
    public void DifferentServiceInstances_WithDifferentKeys_ProduceDifferentSignatures()
    {
        // Arrange
        var service1 = new RequestSigningService("Key1");
        var service2 = new RequestSigningService("Key2");
        var requestBody = "{\"test\":\"data\"}";

        // Act
        var signature1 = service1.SignRequest(requestBody);
        var signature2 = service2.SignRequest(requestBody);

        // Assert
        Assert.NotEqual(signature1, signature2);
    }

    [Fact]
    public void ConstructorWithNullKey_ThrowsException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new RequestSigningService(null!));
    }

    [Fact]
    public void ConstructorWithEmptyKey_ThrowsException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new RequestSigningService(string.Empty));
    }

    [Fact]
    public void AddSignatureHeaders_WithNullDictionary_ThrowsException()
    {
        // Arrange
        var requestBody = "{\"data\":\"test\"}";

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _signingService.AddSignatureHeaders(requestBody, null!));
    }
}
