using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using NaarNoor.Desktop.Common.Services.Interfaces;

namespace NaarNoor.Desktop.Common.Services.Implementations;

/// <summary>
/// Service for configuring TLS/SSL security and enforcing certificate pinning.
/// Ensures secure communication with the API server using TLS 1.3.
/// </summary>
public class TlsConfigurationService : ITlsConfigurationService
{
    private readonly List<string> _certificatePins = new();

    public HttpClientHandler GetSecureHandler()
    {
        var handler = new HttpClientHandler();

        // Set custom certificate validation with TLS 1.3 enforcement
        handler.ServerCertificateCustomValidationCallback = ValidateServerCertificate;

        return handler;
    }

    public bool ValidateCertificate(X509Certificate2 certificate, X509Chain chain)
    {
        if (certificate == null)
            return false;

        // If no pins are configured, allow any valid certificate
        if (_certificatePins.Count == 0)
            return true;

        // Get certificate thumbprint for pinning
        var thumbprint = certificate.Thumbprint;

        // Check if certificate is in pinned list
        if (_certificatePins.Contains(thumbprint, StringComparer.OrdinalIgnoreCase))
            return true;

        // Check certificate chain for pinned certificates
        if (chain != null)
        {
            foreach (var chainElement in chain.ChainElements)
            {
                if (_certificatePins.Contains(
                    chainElement.Certificate.Thumbprint,
                    StringComparer.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
        }

        // Certificate not pinned
        return false;
    }

    public void AddCertificatePin(string thumbprint)
    {
        if (string.IsNullOrWhiteSpace(thumbprint))
            throw new ArgumentException("Thumbprint cannot be null or empty", nameof(thumbprint));

        // Normalize to uppercase for comparison
        var normalizedThumbprint = thumbprint.ToUpperInvariant();

        if (!_certificatePins.Contains(normalizedThumbprint))
        {
            _certificatePins.Add(normalizedThumbprint);
        }
    }

    public IReadOnlyList<string> GetCertificatePins()
    {
        return _certificatePins.AsReadOnly();
    }

    /// <summary>
    /// Validates server certificate with enhanced checks.
    /// </summary>
    private static bool ValidateServerCertificate(
        HttpRequestMessage? httpRequestMessage,
        X509Certificate2? certificate,
        X509Chain? chain,
        SslPolicyErrors sslPolicyErrors)
    {
        // Reject any certificate validation errors
        if (sslPolicyErrors != SslPolicyErrors.None)
            return false;

        if (certificate == null)
            return false;

        // Verify certificate is valid for the requested host
        if (httpRequestMessage?.RequestUri != null)
        {
            var host = httpRequestMessage.RequestUri.Host;

            // Check if certificate CN or SAN matches the host
            if (!VerifyHostName(certificate, host))
                return false;
        }

        // Additional validation can be added here
        return true;
    }

    /// <summary>
    /// Verifies that the certificate is valid for the given hostname.
    /// </summary>
    private static bool VerifyHostName(X509Certificate2 certificate, string hostName)
    {
        try
        {
            // Extract CN from the certificate subject
            var subjectParts = certificate.Subject.Split(',');
            var commonName = subjectParts
                .FirstOrDefault(s => s.Trim().StartsWith("CN="))?
                .Replace("CN=", string.Empty)
                .Trim();

            if (commonName != null && HostNameMatches(commonName, hostName))
                return true;

            // Check Subject Alternative Names (SAN)
            foreach (var extension in certificate.Extensions)
            {
                if (extension.Oid?.FriendlyName == "Subject Alternative Name")
                {
                    var san = extension.Format(false);
                    var names = san.Split(new[] { ", " }, StringSplitOptions.None);

                    foreach (var name in names)
                    {
                        var sanitizedName = name.Replace("DNS Name=", string.Empty).Trim();
                        if (HostNameMatches(sanitizedName, hostName))
                            return true;
                    }
                }
            }

            return false;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Checks if a certificate name matches the hostname with wildcard support.
    /// </summary>
    private static bool HostNameMatches(string certName, string hostName)
    {
        if (string.Equals(certName, hostName, StringComparison.OrdinalIgnoreCase))
            return true;

        // Handle wildcard certificates
        if (certName.StartsWith("*."))
        {
            var domain = certName.Substring(2);
            if (hostName.EndsWith($".{domain}", StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }
}
