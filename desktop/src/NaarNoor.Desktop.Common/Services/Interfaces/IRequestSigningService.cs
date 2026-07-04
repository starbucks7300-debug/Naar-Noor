namespace NaarNoor.Desktop.Common.Services.Interfaces;

/// <summary>
/// Service for signing and verifying requests to ensure data integrity and authenticity.
/// Uses HMAC-SHA256 to sign request bodies and verify responses.
/// </summary>
public interface IRequestSigningService
{
    /// <summary>
    /// Signs a request body with HMAC-SHA256.
    /// </summary>
    /// <param name="requestBody">The request body to sign</param>
    /// <returns>The signature in base64 format</returns>
    string SignRequest(string requestBody);

    /// <summary>
    /// Verifies a response signature to ensure data integrity.
    /// </summary>
    /// <param name="responseBody">The response body</param>
    /// <param name="signature">The signature to verify (base64 format)</param>
    /// <returns>True if signature is valid, false otherwise</returns>
    bool VerifyResponse(string responseBody, string signature);

    /// <summary>
    /// Adds signature headers to an HTTP request.
    /// </summary>
    /// <param name="requestBody">The request body</param>
    /// <param name="requestHeaders">Dictionary to add signature headers to</param>
    void AddSignatureHeaders(string requestBody, Dictionary<string, string> requestHeaders);
}
