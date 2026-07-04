namespace NaarNoor.Desktop.Common.Services
{
    /// <summary>
    /// Interface for multi-layer caching strategy (L1: in-memory, L2: SQLite, L3: JSON files)
    /// </summary>
    public interface ICacheService
    {
        /// <summary>
        /// Retrieve a value from cache asynchronously
        /// </summary>
        /// <typeparam name="T">Type of value to retrieve</typeparam>
        /// <param name="key">Cache key in format: domain:entity:id (e.g., reservations:today:2024-01-15)</param>
        /// <returns>Cached value or null if not found</returns>
        Task<T?> GetAsync<T>(string key);

        /// <summary>
        /// Store a value in cache asynchronously
        /// </summary>
        /// <typeparam name="T">Type of value to store</typeparam>
        /// <param name="key">Cache key in format: domain:entity:id</param>
        /// <param name="value">Value to cache</param>
        /// <param name="expiration">Time-to-live for cache entry (null = no expiration)</param>
        Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);

        /// <summary>
        /// Remove a specific cache entry asynchronously
        /// </summary>
        /// <param name="key">Cache key to remove</param>
        Task RemoveAsync(string key);

        /// <summary>
        /// Clear all cache entries asynchronously
        /// </summary>
        Task ClearAsync();

        /// <summary>
        /// Invalidate all cache entries matching a pattern (e.g., "reservations:*")
        /// </summary>
        /// <param name="pattern">Pattern to match (e.g., "reservations:*", "menu:*", "*")</param>
        void InvalidatePattern(string pattern);
    }
}
