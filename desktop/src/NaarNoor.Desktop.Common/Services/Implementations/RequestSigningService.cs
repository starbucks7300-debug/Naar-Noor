using System.Security.Cryptography;
using System.Text;
using NaarNoor.Desktop.Common.Services.Interfaces;

namespace NaarNoor.Desktop.Common.Services.Implementations;

/// <summary>
/// Service for signing and verifying requests using HMAC-SHA256.
/// Ensures data integrity and prevents tampering of API requests and responses.
/// </summary>
public class RequestSigningService : IRequestSigningService
{
    private readonly byte[] _signingKey;
    private const string SignatureHeaderName = "X-Request-Signature";
    private const string TimestampHeaderName = "X-Request-Timestamp";

    public RequestSigningService(string signingKey)
    {
        if (string.IsNullOrWhiteSpace(signingKey))
            throw new ArgumentException("Signing key cannot be null or empty", nameof(signingKey));

        // Use SHA256 to derive a consistent key from the provided key
        _signingKey = SHA256.HashData(Encoding.UTF8.GetBytes(signingKey));
    }

    public string SignRequest(string requestBody)
    {
        if (requestBody == null)
            throw new ArgumentNullException(nameof(requestBody));

        var signature = ComputeSignature(requestBody);
        return Convert.ToBase64String(signature);
    }

    public bool VerifyResponse(string responseBody, string signature)
    {
        if (string.IsNullOrEmpty(responseBody) || string.IsNullOrEmpty(signature))
            return false;

        try
        {
            var expectedSignature = ComputeSignature(responseBody);
            var providedSignature = Convert.FromBase64String(signature);

            // Use constant-time comparison to prevent timing attacks
            return ConstantTimeComparison(expectedSignature, providedSignature);
        }
        catch (FormatException)
        {
            return false;
        }
    }

    public void AddSignatureHeaders(string requestBody, Dictionary<string, string> requestHeaders)
    {
        if (requestHeaders == null)
            throw new ArgumentNullException(nameof(requestHeaders));

        var signature = SignRequest(requestBody);
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();

        requestHeaders[SignatureHeaderName] = signature;
        requestHeaders[TimestampHeaderName] = timestamp;
    }

    /// <summary>
    /// Computes HMAC-SHA256 signature of the request body.
    /// </summary>
    private byte[] ComputeSignature(string requestBody)
    {
        var bodyBytes = Encoding.UTF8.GetBytes(requestBody);

        using (var hmac = new HMACSHA256(_signingKey))
        {
            return hmac.ComputeHash(bodyBytes);
        }
    }

    /// <summary>
    /// Performs constant-time comparison to prevent timing attacks.
    /// </summary>
    private static bool ConstantTimeComparison(byte[] a, byte[] b)
    {
        if (a.Length != b.Length)
            return false;

        int result = 0;
        for (int i = 0; i < a.Length; i++)
        {
            result |= a[i] ^ b[i];
        }

        return result == 0;
    }
}
