using Xunit;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using NaarNoor.Desktop.WinForms.Configuration;

namespace NaarNoor.Desktop.Tests.Configuration
{
    /// <summary>
    /// Unit tests for certificate pinning validation
    /// Validates secure HTTPS certificate verification
    /// </summary>
    public class CertificatePinningTests
    {
        /// <summary>
        /// Generate a self-signed certificate for testing
        /// </summary>
        private static X509Certificate2 CreateTestCertificate()
        {
            using (var rsa = RSA.Create(2048))
            {
                var request = new CertificateRequest(
                    "cn=test.example.com",
                    rsa,
                    HashAlgorithmName.SHA256,
                    RSASignaturePadding.Pkcs1
                );

                var certificate = request.CreateSelfSigned(
                    DateTimeOffset.UtcNow,
                    DateTimeOffset.UtcNow.AddYears(1)
                );

                return certificate;
            }
        }

        [Fact]
        public void ValidateCertificate_WithMatchingHash_ReturnsTrue()
        {
            // Arrange
            var certificate = CreateTestCertificate();
            var correctHash = CertificatePinningHandler.GetCertificateSha256(certificate);

            // Act
            var result = CertificatePinningHandler.ValidateCertificate(certificate, correctHash);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void ValidateCertificate_WithMismatchedHash_ReturnsFalse()
        {
            // Arrange
            var certificate = CreateTestCertificate();
            var incorrectHash = "0000000000000000000000000000000000000000000000000000000000000000";

            // Act
            var result = CertificatePinningHandler.ValidateCertificate(certificate, incorrectHash);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ValidateCertificate_WithCaseInsensitiveHash_ReturnsTrue()
        {
            // Arrange
            var certificate = CreateTestCertificate();
            var hash = CertificatePinningHandler.GetCertificateSha256(certificate);
            var lowercaseHash = hash.ToLower();

            // Act
            var result = CertificatePinningHandler.ValidateCertificate(certificate, lowercaseHash);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void GetCertificateSha256_ReturnsValidHexString()
        {
            // Arrange
            var certificate = CreateTestCertificate();

            // Act
            var hash = CertificatePinningHandler.GetCertificateSha256(certificate);

            // Assert
            Assert.NotNull(hash);
            Assert.Equal(64, hash.Length); // SHA-256 = 32 bytes = 64 hex chars
            Assert.True(IsValidHexString(hash));
        }

        [Fact]
        public void ValidateCertificate_WithNullCertificate_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                CertificatePinningHandler.ValidateCertificate(null!, "somehash")
            );
        }

        [Fact]
        public void ValidateCertificate_WithNullHash_ThrowsArgumentException()
        {
            // Arrange
            var certificate = CreateTestCertificate();

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                CertificatePinningHandler.ValidateCertificate(certificate, null!)
            );
        }

        [Fact]
        public void ValidateCertificate_WithEmptyHash_ThrowsArgumentException()
        {
            // Arrange
            var certificate = CreateTestCertificate();

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                CertificatePinningHandler.ValidateCertificate(certificate, "")
            );
        }

        [Fact]
        public void GetCertificateSha256_WithNullCertificate_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                CertificatePinningHandler.GetCertificateSha256(null!)
            );
        }

        [Theory]
        [InlineData("AAAA")]
        [InlineData("ZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZ")]
        public void ValidateCertificate_WithInvalidHash_ReturnsFalse(string invalidHash)
        {
            // Arrange
            var certificate = CreateTestCertificate();

            // Act
            var result = CertificatePinningHandler.ValidateCertificate(certificate, invalidHash);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void CertificateHash_Deterministic_SameHashForSameCertificate()
        {
            // Arrange
            var certificate = CreateTestCertificate();

            // Act
            var hash1 = CertificatePinningHandler.GetCertificateSha256(certificate);
            var hash2 = CertificatePinningHandler.GetCertificateSha256(certificate);

            // Assert
            Assert.Equal(hash1, hash2);
        }

        /// <summary>
        /// Helper to validate that a string contains only valid hex characters
        /// </summary>
        private static bool IsValidHexString(string value)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(value, @"^[0-9A-F]+$", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        }
    }
}
