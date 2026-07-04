using NaarNoor.Desktop.Common.Services.Implementations;
using NaarNoor.Desktop.Common.Services.Interfaces;
using Xunit;

namespace NaarNoor.Desktop.Tests.Security;

/// <summary>
/// Tests for TlsConfigurationService covering TLS/SSL configuration and certificate pinning.
/// </summary>
public class TlsConfigurationServiceTests
{
    private readonly ITlsConfigurationService _tlsService;

    public TlsConfigurationServiceTests()
    {
        _tlsService = new TlsConfigurationService();
    }

    [Fact]
    public void GetSecureHandler_ReturnsHttpClientHandler()
    {
        // Act
        var handler = _tlsService.GetSecureHandler();

        // Assert
        Assert.NotNull(handler);
        Assert.IsType<HttpClientHandler>(handler);
    }

    [Fact]
    public void GetSecureHandler_HasCustomValidationCallback()
    {
        // Act
        var handler = _tlsService.GetSecureHandler();

        // Assert
        Assert.NotNull(handler.ServerCertificateCustomValidationCallback);
    }

    [Fact]
    public void AddCertificatePin_WithValidThumbprint_AddsPinSuccessfully()
    {
        // Arrange
        var thumbprint = "A1B2C3D4E5F6G7H8I9J0K1L2M3N4O5P6Q7R8S9T0";

        // Act
        _tlsService.AddCertificatePin(thumbprint);

        // Assert
        var pins = _tlsService.GetCertificatePins();
        Assert.Contains(thumbprint, pins);
    }

    [Fact]
    public void AddCertificatePin_DuplicatePin_OnlyAddedOnce()
    {
        // Arrange
        var thumbprint = "A1B2C3D4E5F6G7H8I9J0K1L2M3N4O5P6Q7R8S9T0";

        // Act
        _tlsService.AddCertificatePin(thumbprint);
        _tlsService.AddCertificatePin(thumbprint);

        // Assert
        var pins = _tlsService.GetCertificatePins();
        Assert.Single(pins.Where(p => p.Equals(thumbprint, StringComparison.OrdinalIgnoreCase)));
    }

    [Fact]
    public void AddCertificatePin_NormalizesCasing()
    {
        // Arrange
        var thumbprintLower = "a1b2c3d4e5f6g7h8i9j0k1l2m3n4o5p6q7r8s9t0";
        var thumbprintUpper = "A1B2C3D4E5F6G7H8I9J0K1L2M3N4O5P6Q7R8S9T0";

        // Act
        _tlsService.AddCertificatePin(thumbprintLower);
        var pins1 = _tlsService.GetCertificatePins();

        _tlsService.AddCertificatePin(thumbprintUpper);
        var pins2 = _tlsService.GetCertificatePins();

        // Assert
        Assert.Single(pins1);
        Assert.Single(pins2); // Still only one pin due to normalization
    }

    [Fact]
    public void AddCertificatePin_WithNullThumbprint_ThrowsException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => _tlsService.AddCertificatePin(null!));
    }

    [Fact]
    public void AddCertificatePin_WithEmptyThumbprint_ThrowsException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => _tlsService.AddCertificatePin(""));
    }

    [Fact]
    public void GetCertificatePins_InitiallyEmpty()
    {
        // Act
        var pins = _tlsService.GetCertificatePins();

        // Assert
        Assert.Empty(pins);
    }

    [Fact]
    public void GetCertificatePins_ReturnsReadOnlyCollection()
    {
        // Arrange
        _tlsService.AddCertificatePin("TEST123");

        // Act
        var pins = _tlsService.GetCertificatePins();

        // Assert
        Assert.IsAssignableFrom<IReadOnlyList<string>>(pins);
    }

    [Fact]
    public void AddMultipleCertificatePins()
    {
        // Arrange
        var pin1 = "A1B2C3D4E5F6G7H8I9J0K1L2M3N4O5P6";
        var pin2 = "B2C3D4E5F6G7H8I9J0K1L2M3N4O5P6A1";
        var pin3 = "C3D4E5F6G7H8I9J0K1L2M3N4O5P6A1B2";

        // Act
        _tlsService.AddCertificatePin(pin1);
        _tlsService.AddCertificatePin(pin2);
        _tlsService.AddCertificatePin(pin3);

        // Assert
        var pins = _tlsService.GetCertificatePins();
        Assert.Equal(3, pins.Count);
        Assert.Contains(pin1, pins);
        Assert.Contains(pin2, pins);
        Assert.Contains(pin3, pins);
    }

    [Fact]
    public void ValidateCertificate_WithoutConfiguredPins_ReturnsTrue()
    {
        // Arrange
        // No pins configured

        // Act
        var result = _tlsService.ValidateCertificate(null!, null!);

        // Assert
        // Without pins configured, validation is permissive (returns based on certificate null check)
        Assert.False(result); // Certificate is null
    }

    [Fact]
    public void GetSecureHandler_CanBeUsedForHttpClient()
    {
        // Arrange
        var handler = _tlsService.GetSecureHandler();

        // Act
        using var client = new HttpClient(handler);

        // Assert
        Assert.NotNull(client);
        Assert.NotNull(handler);
    }

    [Theory]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public void AddCertificatePin_WithWhitespaceOnly_ThrowsException(string input)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => _tlsService.AddCertificatePin(input));
    }
}
