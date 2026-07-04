using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace NaarNoor.Desktop.Common.Services.Interfaces;

/// <summary>
/// Service for configuring TLS/SSL security settings and enforcing security policies.
/// </summary>
public interface ITlsConfigurationService
{
    /// <summary>
    /// Gets the configured HTTP client handler with TLS 1.3 enforcement and certificate pinning.
    /// </summary>
    HttpClientHandler GetSecureHandler();

    /// <summary>
    /// Validates a certificate against pinned certificates.
    /// </summary>
    /// <param name="certificate">The certificate to validate</param>
    /// <param name="chain">The certificate chain</param>
    /// <returns>True if certificate is valid and pinned, false otherwise</returns>
    bool ValidateCertificate(X509Certificate2 certificate, X509Chain chain);

    /// <summary>
    /// Adds a certificate pin for certificate pinning.
    /// </summary>
    /// <param name="thumbprint">The certificate thumbprint (SHA256)</param>
    void AddCertificatePin(string thumbprint);

    /// <summary>
    /// Gets all configured certificate pins.
    /// </summary>
    IReadOnlyList<string> GetCertificatePins();
}
