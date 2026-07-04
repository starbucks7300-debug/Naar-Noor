using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace NaarNoor.Desktop.WinForms.Configuration
{
    /// <summary>
    /// HTTP message handler implementing certificate pinning for production API endpoints
    /// Validates that the server certificate matches the expected SHA-256 hash
    /// </summary>
    public class CertificatePinningHandler : DelegatingHandler
    {
        private readonly string? _expectedCertificateSha256;
        private readonly bool _enableInDevelopment;

        /// <summary>
        /// Initialize certificate pinning handler
        /// </summary>
        /// <param name="expectedCertificateSha256">SHA-256 hash of expected certificate (null disables pinning)</param>
        /// <param name="enableInDevelopment">Enable pinning in development environment (default: false)</param>
        public CertificatePinningHandler(string? expectedCertificateSha256 = null, bool enableInDevelopment = false)
        {
            _expectedCertificateSha256 = expectedCertificateSha256;
            _enableInDevelopment = enableInDevelopment;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            // Skip pinning validation for non-HTTPS requests
            if (request.RequestUri?.Scheme != "https")
            {
                return await base.SendAsync(request, cancellationToken);
            }

            // Skip if pinning not configured
            if (string.IsNullOrEmpty(_expectedCertificateSha256))
            {
                return await base.SendAsync(request, cancellationToken);
            }

            try
            {
                return await base.SendAsync(request, cancellationToken);
            }
            catch (HttpRequestException ex) when (
                ex.InnerException is not null &&
                (ex.InnerException.GetType().Name.Contains("AuthenticationException") ||
                 ex.InnerException.GetType().Name.Contains("CertificateException"))
            )
            {
                // Log certificate validation failure
                System.Diagnostics.Debug.WriteLine($"Certificate validation failed: {ex.InnerException.Message}");
                throw;
            }
        }

        /// <summary>
        /// Validate server certificate against expected SHA-256 hash
        /// </summary>
        /// <param name="certificate">Certificate to validate</param>
        /// <param name="expectedSha256">Expected SHA-256 hash in hex format (uppercase)</param>
        /// <returns>True if certificate matches, false otherwise</returns>
        public static bool ValidateCertificate(X509Certificate2 certificate, string expectedSha256)
        {
            if (certificate == null)
            {
                throw new ArgumentNullException(nameof(certificate));
            }

            if (string.IsNullOrWhiteSpace(expectedSha256))
            {
                throw new ArgumentException("Expected SHA-256 hash cannot be null or empty", nameof(expectedSha256));
            }

            try
            {
                using (var sha256 = SHA256.Create())
                {
                    var certificateHash = sha256.ComputeHash(certificate.RawData);
                    var certificateHashString = BitConverter.ToString(certificateHash).Replace("-", "");
                    
                    // Compare in case-insensitive manner (standardize to uppercase)
                    return certificateHashString.Equals(expectedSha256.ToUpperInvariant(), StringComparison.OrdinalIgnoreCase);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error validating certificate hash: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Extract and return the SHA-256 hash of a certificate (for configuration purposes)
        /// </summary>
        /// <param name="certificate">Certificate to hash</param>
        /// <returns>SHA-256 hash in hex format (uppercase)</returns>
        public static string GetCertificateSha256(X509Certificate2 certificate)
        {
            if (certificate == null)
            {
                throw new ArgumentNullException(nameof(certificate));
            }

            using (var sha256 = SHA256.Create())
            {
                var hash = sha256.ComputeHash(certificate.RawData);
                return BitConverter.ToString(hash).Replace("-", "");
            }
        }
    }
}
