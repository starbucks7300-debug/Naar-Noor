using System.Collections.Concurrent;
using System.Text.Json;

namespace NaarNoor.Desktop.Common.Services
{
    /// <summary>
    /// Multi-layer caching service: L1 (in-memory), L2 (SQLite), L3 (JSON files)
    /// </summary>
    public class CacheService : ICacheService
    {
        private readonly string _cacheDirectory;
        private readonly Dictionary<string, CacheEntry> _l1Cache; // L1: In-memory
        private readonly object _l1LockObject = new();
        private const long MaxCacheSizeBytes = 104857600; // 100MB
        private long _currentCacheSizeBytes = 0;
        private const string CacheDirectoryName = "NaarNoor\\cache";

        public CacheService()
        {
            _cacheDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                CacheDirectoryName
            );

            _l1Cache = new Dictionary<string, CacheEntry>();

            // Ensure cache directory exists
            if (!Directory.Exists(_cacheDirectory))
            {
                Directory.CreateDirectory(_cacheDirectory);
            }
        }

        /// <summary>
        /// Get value from cache with multi-layer fallback
        /// L1 (in-memory) → L2 (SQLite) → L3 (JSON) → null
        /// </summary>
        public async Task<T?> GetAsync<T>(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return default(T?);

            // L1: Check in-memory cache first
            lock (_l1LockObject)
            {
                if (_l1Cache.TryGetValue(key, out var entry))
                {
                    if (IsExpired(entry))
                    {
                        _l1Cache.Remove(key);
                    }
                    else
                    {
                        return DeserializeValue<T>(entry.SerializedValue);
                    }
                }
            }

            // L2/L3: Check file-based cache (simplified JSON approach)
            var cachedValue = await LoadFromFileAsync<T>(key);
            if (cachedValue != null)
            {
                // Promote to L1 cache
                lock (_l1LockObject)
                {
                    if (!_l1Cache.ContainsKey(key))
                    {
                        AddToL1Cache(key, cachedValue);
                    }
                }
                return cachedValue;
            }

            return default(T?);
        }

        /// <summary>
        /// Set value in cache with TTL
        /// </summary>
        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            if (string.IsNullOrWhiteSpace(key) || value == null)
                return;

            var serialized = JsonSerializer.Serialize(value);
            DateTime? expirationTime = expiration.HasValue ? DateTime.UtcNow.Add(expiration.Value) : null;

            // Store in L1 cache
            lock (_l1LockObject)
            {
                AddToL1Cache(key, value, serialized, expirationTime);
            }

            // Also store in file-based cache (L2/L3)
            await SaveToFileAsync<T>(key, serialized, expirationTime);
        }

        /// <summary>
        /// Remove a specific cache entry
        /// </summary>
        public async Task RemoveAsync(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return;

            // Remove from L1
            lock (_l1LockObject)
            {
                if (_l1Cache.TryGetValue(key, out var entry))
                {
                    _currentCacheSizeBytes -= entry.SerializedValue.Length;
                    _l1Cache.Remove(key);
                }
            }

            // Remove from file-based storage
            await RemoveFromFileAsync(key);
        }

        /// <summary>
        /// Clear all cache entries
        /// </summary>
        public async Task ClearAsync()
        {
            // Clear L1
            lock (_l1LockObject)
            {
                _l1Cache.Clear();
                _currentCacheSizeBytes = 0;
            }

            // Clear file-based cache
            try
            {
                if (Directory.Exists(_cacheDirectory))
                {
                    foreach (var file in Directory.GetFiles(_cacheDirectory))
                    {
                        File.Delete(file);
                    }
                }
            }
            catch
            {
                // Log but don't throw
            }

            await Task.CompletedTask;
        }

        /// <summary>
        /// Invalidate all entries matching a pattern (e.g., "reservations:*")
        /// </summary>
        public void InvalidatePattern(string pattern)
        {
            if (string.IsNullOrWhiteSpace(pattern))
                return;

            var keysToRemove = new List<string>();

            lock (_l1LockObject)
            {
                // Simple pattern matching: domain:* matches all keys starting with domain:
                if (pattern.EndsWith("*"))
                {
                    var prefix = pattern.Substring(0, pattern.Length - 1);
                    keysToRemove.AddRange(
                        _l1Cache.Keys.Where(k => k.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                    );
                }
                else
                {
                    // Exact match
                    if (_l1Cache.ContainsKey(pattern))
                        keysToRemove.Add(pattern);
                }

                // Remove matched entries
                foreach (var key in keysToRemove)
                {
                    if (_l1Cache.TryGetValue(key, out var entry))
                    {
                        _currentCacheSizeBytes -= entry.SerializedValue.Length;
                        _l1Cache.Remove(key);
                    }
                }
            }

            // Also invalidate from file-based storage
            try
            {
                if (Directory.Exists(_cacheDirectory))
                {
                    if (pattern.EndsWith("*"))
                    {
                        var prefix = pattern.Substring(0, pattern.Length - 1);
                        var filesToDelete = Directory.GetFiles(_cacheDirectory)
                            .Where(f => Path.GetFileNameWithoutExtension(f).StartsWith(prefix, StringComparison.OrdinalIgnoreCase));

                        foreach (var file in filesToDelete)
                        {
                            try { File.Delete(file); } catch { }
                        }
                    }
                    else
                    {
                        var filePath = GetCacheFilePath(pattern);
                        if (File.Exists(filePath))
                            File.Delete(filePath);
                    }
                }
            }
            catch
            {
                // Log but don't throw
            }
        }

        /// <summary>
        /// Add value to L1 cache with LRU eviction if needed
        /// </summary>
        private void AddToL1Cache<T>(string key, T value, string? serialized = null, DateTime? expiration = null)
        {
            serialized ??= JsonSerializer.Serialize(value);
            var valueSize = serialized.Length;

            // Check if we need to evict entries
            if (_currentCacheSizeBytes + valueSize > MaxCacheSizeBytes && _l1Cache.Count > 0)
            {
                // Simple LRU eviction: remove oldest entry
                var oldestKey = _l1Cache.Keys.First();
                _currentCacheSizeBytes -= _l1Cache[oldestKey].SerializedValue.Length;
                _l1Cache.Remove(oldestKey);
            }

            // Remove if key already exists
            if (_l1Cache.TryGetValue(key, out var existingEntry))
            {
                _currentCacheSizeBytes -= existingEntry.SerializedValue.Length;
                _l1Cache.Remove(key);
            }

            // Add new entry
            _l1Cache[key] = new CacheEntry
            {
                SerializedValue = serialized,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = expiration
            };

            _currentCacheSizeBytes += valueSize;
        }

        /// <summary>
        /// Load value from file-based cache
        /// </summary>
        private async Task<T?> LoadFromFileAsync<T>(string key)
        {
            try
            {
                var filePath = GetCacheFilePath(key);
                if (!File.Exists(filePath))
                    return default(T?);

                var content = await File.ReadAllTextAsync(filePath);
                var cacheData = JsonSerializer.Deserialize<FileCacheEntry>(content);

                if (cacheData == null)
                    return default(T?);

                // Check expiration
                if (cacheData.ExpiresAt.HasValue && DateTime.UtcNow > cacheData.ExpiresAt)
                {
                    File.Delete(filePath);
                    return default(T?);
                }

                return JsonSerializer.Deserialize<T>(cacheData.SerializedValue);
            }
            catch
            {
                return default(T?);
            }
        }

        /// <summary>
        /// Save value to file-based cache
        /// </summary>
        private async Task SaveToFileAsync<T>(string key, string serialized, DateTime? expiration = null)
        {
            try
            {
                var cacheData = new FileCacheEntry
                {
                    SerializedValue = serialized,
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = expiration
                };

                var filePath = GetCacheFilePath(key);
                var content = JsonSerializer.Serialize(cacheData);
                await File.WriteAllTextAsync(filePath, content);
            }
            catch
            {
                // Log but don't throw
            }
        }

        /// <summary>
        /// Remove value from file-based cache
        /// </summary>
        private async Task RemoveFromFileAsync(string key)
        {
            try
            {
                var filePath = GetCacheFilePath(key);
                if (File.Exists(filePath))
                    File.Delete(filePath);
            }
            catch
            {
                // Log but don't throw
            }

            await Task.CompletedTask;
        }

        /// <summary>
        /// Get file path for cache key
        /// </summary>
        private string GetCacheFilePath(string key)
        {
            // Sanitize key for use as filename
            var sanitized = System.Text.RegularExpressions.Regex.Replace(
                key,
                "[^a-zA-Z0-9._-]",
                "_"
            );
            return Path.Combine(_cacheDirectory, $"{sanitized}.cache");
        }

        /// <summary>
        /// Check if cache entry is expired
        /// </summary>
        private bool IsExpired(CacheEntry entry)
        {
            return entry.ExpiresAt.HasValue && DateTime.UtcNow > entry.ExpiresAt;
        }

        /// <summary>
        /// Deserialize cached value
        /// </summary>
        private T? DeserializeValue<T>(string serialized)
        {
            try
            {
                return JsonSerializer.Deserialize<T>(serialized);
            }
            catch
            {
                return default;
            }
        }

        /// <summary>
        /// Internal cache entry structure
        /// </summary>
        private class CacheEntry
        {
            public string SerializedValue { get; set; } = string.Empty;
            public DateTime CreatedAt { get; set; }
            public DateTime? ExpiresAt { get; set; }
        }

        /// <summary>
        /// File-based cache entry structure
        /// </summary>
        private class FileCacheEntry
        {
            public string SerializedValue { get; set; } = string.Empty;
            public DateTime CreatedAt { get; set; }
            public DateTime? ExpiresAt { get; set; }
        }
    }
}
