namespace NaarNoor.Desktop.Common.Data.Models
{
    /// <summary>
    /// Represents a cached data entry in the local SQLite cache
    /// </summary>
    public class CacheEntry
    {
        /// <summary>
        /// Primary key for the cache entry
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Cache key identifier (e.g., "reservations:today:2024-01-15")
        /// </summary>
        public required string Key { get; set; }

        /// <summary>
        /// JSON serialized cache value
        /// </summary>
        public required string Value { get; set; }

        /// <summary>
        /// When this cache entry was created
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// When this cache entry was last updated (null if not updated)
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// When this cache entry expires (null if no expiration)
        /// </summary>
        public DateTime? ExpiresAt { get; set; }
    }
}
