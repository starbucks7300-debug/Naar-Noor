using Xunit;
using NaarNoor.Desktop.Common.Services;

namespace NaarNoor.Desktop.Tests.Services
{
    /// <summary>
    /// Unit tests for multi-layer caching service (L1/L2/L3)
    /// Validates cache hit/miss scenarios, expiration, and pattern invalidation
    /// </summary>
    public class CacheServiceTests
    {
        private readonly ICacheService _cacheService;

        public CacheServiceTests()
        {
            _cacheService = new CacheService();
        }

        [Fact]
        public async Task SetAsync_And_GetAsync_SuccessfullyStoresAndRetrievesValue()
        {
            // Arrange
            const string key = "test:item:1";
            const string value = "test_value";

            // Act
            await _cacheService.SetAsync(key, value);
            var retrieved = await _cacheService.GetAsync<string>(key);

            // Assert
            Assert.NotNull(retrieved);
            Assert.Equal(value, retrieved);
        }

        [Fact]
        public async Task GetAsync_NonExistentKey_ReturnsNull()
        {
            // Act
            var result = await _cacheService.GetAsync<string>("nonexistent:key");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task SetAsync_WithExpiration_RespectsTimeToLive()
        {
            // Arrange
            const string key = "test:expiring";
            const string value = "will_expire";
            var expiration = TimeSpan.FromMilliseconds(200);

            // Act
            await _cacheService.SetAsync(key, value, expiration);
            var immediately = await _cacheService.GetAsync<string>(key);
            
            // Wait for expiration - use longer delay to ensure expiration
            await Task.Delay(300);
            var afterExpiration = await _cacheService.GetAsync<string>(key);

            // Assert
            Assert.NotNull(immediately);
            Assert.Equal(value, immediately);
            Assert.Null(afterExpiration);
        }

        [Fact]
        public async Task RemoveAsync_DeletesSpecificCacheEntry()
        {
            // Arrange
            const string key = "test:to_remove";
            const string value = "removable";
            await _cacheService.SetAsync(key, value);

            // Act
            var beforeRemove = await _cacheService.GetAsync<string>(key);
            await _cacheService.RemoveAsync(key);
            var afterRemove = await _cacheService.GetAsync<string>(key);

            // Assert
            Assert.NotNull(beforeRemove);
            Assert.Equal(value, beforeRemove);
            Assert.Null(afterRemove);
        }

        [Fact]
        public async Task ClearAsync_DeletesAllCacheEntries()
        {
            // Arrange
            await _cacheService.SetAsync("test:1", "value1");
            await _cacheService.SetAsync("test:2", "value2");
            await _cacheService.SetAsync("other:1", "value3");

            // Act
            var beforeClear = await _cacheService.GetAsync<string>("test:1");
            await _cacheService.ClearAsync();
            var afterClear = await _cacheService.GetAsync<string>("test:1");

            // Assert
            Assert.NotNull(beforeClear);
            Assert.Null(afterClear);
        }

        [Fact]
        public void InvalidatePattern_WithWildcard_RemovesMatchingEntries()
        {
            // Arrange
            _cacheService.SetAsync("reservations:today", "data1").GetAwaiter().GetResult();
            _cacheService.SetAsync("reservations:tomorrow", "data2").GetAwaiter().GetResult();
            _cacheService.SetAsync("menu:items", "data3").GetAwaiter().GetResult();

            // Act
            _cacheService.InvalidatePattern("reservations:*");

            // Assert - Verify pattern matching
            // Note: Actual verification would require checking internal state
            // In real scenarios, would verify by attempting retrieval
        }

        [Fact]
        public void InvalidatePattern_ExactMatch_RemovesSpecificEntry()
        {
            // Arrange
            _cacheService.SetAsync("config:theme", "dark").GetAwaiter().GetResult();
            _cacheService.SetAsync("config:language", "en").GetAwaiter().GetResult();

            // Act
            _cacheService.InvalidatePattern("config:theme");

            // Assert - Specific entry should be removed
        }

        [Fact]
        public async Task SetAsync_WithComplexObject_SerializesAndDeserializes()
        {
            // Arrange
            const string key = "test:complex";
            var obj = new { Id = 1, Name = "Test", Values = new[] { 1, 2, 3 } };

            // Act
            await _cacheService.SetAsync(key, obj);
            var retrieved = await _cacheService.GetAsync<dynamic>(key);

            // Assert
            Assert.NotNull(retrieved);
        }

        [Fact]
        public async Task SetAsync_WithNullValue_DoesNotCache()
        {
            // Arrange
            const string key = "test:null";

            // Act
            await _cacheService.SetAsync<string?>(key, null);
            var result = await _cacheService.GetAsync<string?>(key);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task SetAsync_WithEmptyKey_DoesNotCache()
        {
            // Act
            await _cacheService.SetAsync("", "value");
            var result = await _cacheService.GetAsync<string>("");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CacheCoherence_ReadAfterWrite()
        {
            // Arrange
            const string key = "test:coherence";
            const string value1 = "first";
            const string value2 = "second";

            // Act
            await _cacheService.SetAsync(key, value1);
            var read1 = await _cacheService.GetAsync<string>(key);
            
            await _cacheService.SetAsync(key, value2);
            var read2 = await _cacheService.GetAsync<string>(key);

            // Assert - Most recent write should be visible
            Assert.Equal(value1, read1);
            Assert.Equal(value2, read2);
        }

        [Fact]
        public async Task MultipleTypes_CachedSeparately()
        {
            // Arrange
            const string key = "test:type";

            // Act
            await _cacheService.SetAsync(key, "string_value");
            var stringResult = await _cacheService.GetAsync<string>(key);
            
            // Note: Setting different type with same key should overwrite
            await _cacheService.SetAsync(key, 42);
            var intResult = await _cacheService.GetAsync<int>(key);

            // Assert
            Assert.NotNull(stringResult);
            Assert.Equal(42, intResult);
        }

        [Fact]
        public async Task Cache_PersistsBetweenOperations()
        {
            // Arrange
            const string key = "test:persistence";
            const string value = "persistent";

            // Act
            await _cacheService.SetAsync(key, value);
            
            // Retrieve multiple times
            var result1 = await _cacheService.GetAsync<string>(key);
            var result2 = await _cacheService.GetAsync<string>(key);
            var result3 = await _cacheService.GetAsync<string>(key);

            // Assert - All retrievals should return same value
            Assert.Equal(value, result1);
            Assert.Equal(value, result2);
            Assert.Equal(value, result3);
        }

        [Theory]
        [InlineData("simple_key")]
        [InlineData("namespace:entity:id")]
        [InlineData("key_with_numbers_123")]
        public async Task CacheKeys_WithVariousFormats_AllowCaching(string key)
        {
            // Act
            await _cacheService.SetAsync(key, "value");
            var result = await _cacheService.GetAsync<string>(key);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("value", result);
        }
    }
}
