using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace NaarNoor.Desktop.Common.Services
{
    /// <summary>
    /// Service for securely storing and retrieving authentication tokens using AES encryption
    /// Keys are derived from machine and user information for secure per-user encryption
    /// </summary>
    public class TokenStorageService
    {
        private readonly string _tokenDirectory;
        private readonly string _tokenFilePath;
        private const string TokenFileName = "tokens.bin";

        public TokenStorageService()
        {
            _tokenDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "NaarNoor"
            );
            _tokenFilePath = Path.Combine(_tokenDirectory, TokenFileName);
        }

        /// <summary>
        /// Save encrypted tokens to secure storage
        /// </summary>
        public async Task SaveTokenAsync(string? accessToken, string? refreshToken)
        {
            try
            {
                // Ensure directory exists
                if (!Directory.Exists(_tokenDirectory))
                {
                    Directory.CreateDirectory(_tokenDirectory);
                    SetDirectoryPermissions(_tokenDirectory);
                }

                // Create token data object
                var tokenData = new TokenData
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    SavedAt = DateTime.UtcNow
                };

                // Serialize to JSON
                var json = JsonSerializer.Serialize(tokenData);
                var plainBytes = Encoding.UTF8.GetBytes(json);

                // Encrypt using AES
                var encryptedBytes = EncryptData(plainBytes);

                // Write encrypted bytes to file
                await File.WriteAllBytesAsync(_tokenFilePath, encryptedBytes);

                // Set file permissions to current user only
                SetFilePermissions(_tokenFilePath);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to save tokens securely.", ex);
            }
        }

        /// <summary>
        /// Load and decrypt tokens from secure storage
        /// </summary>
        public async Task<(string? AccessToken, string? RefreshToken)> LoadTokenAsync()
        {
            try
            {
                // Check if token file exists
                if (!File.Exists(_tokenFilePath))
                {
                    return (null, null);
                }

                // Read encrypted bytes
                var encryptedBytes = await File.ReadAllBytesAsync(_tokenFilePath);

                // Decrypt using AES
                var plainBytes = DecryptData(encryptedBytes);

                // Deserialize from JSON
                var json = Encoding.UTF8.GetString(plainBytes);
                var tokenData = JsonSerializer.Deserialize<TokenData>(json);

                return (tokenData?.AccessToken, tokenData?.RefreshToken);
            }
            catch (CryptographicException ex)
            {
                // Handle decryption failure gracefully
                throw new InvalidOperationException(
                    "Failed to decrypt tokens. Tokens may be corrupted or created by a different user.",
                    ex
                );
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to load tokens.", ex);
            }
        }

        /// <summary>
        /// Delete stored tokens (used on logout)
        /// </summary>
        public async Task DeleteTokenAsync()
        {
            try
            {
                if (File.Exists(_tokenFilePath))
                {
                    // Overwrite file with zeros before deletion (secure deletion)
                    var fileInfo = new FileInfo(_tokenFilePath);
                    using (var fs = new FileStream(_tokenFilePath, FileMode.Open, FileAccess.Write))
                    {
                        byte[] zeros = new byte[fs.Length];
                        fs.Write(zeros, 0, zeros.Length);
                    }

                    // Delete the file
                    File.Delete(_tokenFilePath);
                }

                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to delete tokens.", ex);
            }
        }

        /// <summary>
        /// Encrypt data using AES
        /// </summary>
        private byte[] EncryptData(byte[] plainText)
        {
            using (var aes = Aes.Create())
            {
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.KeySize = 256;
                aes.BlockSize = 128;

                // Generate a random IV
                aes.GenerateIV();
                var iv = aes.IV;

                // Derive key from machine ID + current user
                aes.Key = DeriveKey();

                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                using (var ms = new MemoryStream())
                {
                    // Write IV to the beginning of the stream (needed for decryption)
                    ms.Write(iv, 0, iv.Length);

                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        cs.Write(plainText, 0, plainText.Length);
                        cs.FlushFinalBlock();
                    }

                    return ms.ToArray();
                }
            }
        }

        /// <summary>
        /// Decrypt data using AES
        /// </summary>
        private byte[] DecryptData(byte[] encryptedData)
        {
            using (var aes = Aes.Create())
            {
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.KeySize = 256;
                aes.BlockSize = 128;

                // Extract IV from the beginning of the data
                var ivLength = aes.IV.Length;
                var iv = new byte[ivLength];
                Array.Copy(encryptedData, 0, iv, 0, ivLength);
                aes.IV = iv;

                // Derive key from machine ID + current user
                aes.Key = DeriveKey();

                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                using (var ms = new MemoryStream(encryptedData, ivLength, encryptedData.Length - ivLength))
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                {
                    var plainText = new byte[encryptedData.Length - ivLength];
                    int bytesRead = cs.Read(plainText, 0, plainText.Length);
                    return plainText[..bytesRead];
                }
            }
        }

        /// <summary>
        /// Derive encryption key from machine and user information
        /// </summary>
        private byte[] DeriveKey()
        {
            var salt = Encoding.UTF8.GetBytes("NaarNoor-Desktop-Token-Salt");
            var keyInfo = $"{Environment.MachineName}:{Environment.UserName}";
            var keyBytes = Encoding.UTF8.GetBytes(keyInfo);

            using (var pbkdf2 = new Rfc2898DeriveBytes(keyBytes, salt, 10000, HashAlgorithmName.SHA256))
            {
                return pbkdf2.GetBytes(32); // 256-bit key
            }
        }

        /// <summary>
        /// Set directory permissions to current user only
        /// </summary>
        private void SetDirectoryPermissions(string directoryPath)
        {
            try
            {
                // Windows-specific: Set ACL for current user only
                if (OperatingSystem.IsWindows())
                {
                    var dirInfo = new DirectoryInfo(directoryPath);
                    var dirSecurity = dirInfo.GetAccessControl();
                    dirSecurity.SetAccessRuleProtection(true, false);
                    dirInfo.SetAccessControl(dirSecurity);
                }
            }
            catch
            {
                // Log warning but don't fail - encryption provides primary protection
            }
        }

        /// <summary>
        /// Set file permissions to current user only
        /// </summary>
        private void SetFilePermissions(string filePath)
        {
            try
            {
                // Windows-specific: Set ACL for current user only
                if (OperatingSystem.IsWindows())
                {
                    var fileInfo = new FileInfo(filePath);
                    var fileSecurity = fileInfo.GetAccessControl();
                    fileSecurity.SetAccessRuleProtection(true, false);
                    fileInfo.SetAccessControl(fileSecurity);
                }
            }
            catch
            {
                // Log warning but don't fail - encryption provides primary protection
            }
        }

        /// <summary>
        /// Internal data structure for storing tokens
        /// </summary>
        private class TokenData
        {
            public string? AccessToken { get; set; }
            public string? RefreshToken { get; set; }
            public DateTime SavedAt { get; set; }
        }
    }
}
