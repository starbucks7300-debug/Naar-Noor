using NaarNoor.Desktop.Common.DTOs;
using NaarNoor.Desktop.Common.Services.ApiClients;
using NaarNoor.Desktop.Common.Utilities;

namespace NaarNoor.Desktop.Common.Services
{
    /// <summary>
    /// Service for managing menu items with caching and bilingual support.
    /// Implements CRUD operations with automatic cache invalidation.
    /// </summary>
    public class MenuService : IMenuService
    {
        private readonly IMenuApiClient _apiClient;
        private readonly ICacheService _cacheService;

        private const string AllMenuItemsCacheKey = "menu:all";
        private const string MenuItemCacheKeyPrefix = "menu:item:";
        private const string MenuCategoryPrefix = "menu:category:";
        private const string AvailableMenuCacheKey = "menu:available";
        private static readonly TimeSpan CacheTtl = TimeSpan.FromHours(2);

        public MenuService(
            IMenuApiClient apiClient,
            ICacheService cacheService)
        {
            _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
            _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        }

        public async Task<Result<List<MenuItemDto>>> GetMenuItemsAsync()
        {
            try
            {
                // Try to get from cache first
                var cached = await _cacheService.GetAsync<List<MenuItemDto>>(AllMenuItemsCacheKey);
                if (cached != null)
                {
                    return Result<List<MenuItemDto>>.Success(cached);
                }

                // Fetch from API
                var items = await _apiClient.GetMenuItemsAsync();

                // Cache the result
                await _cacheService.SetAsync(AllMenuItemsCacheKey, items, CacheTtl);

                return Result<List<MenuItemDto>>.Success(items);
            }
            catch (Exception ex)
            {
                return Result<List<MenuItemDto>>.Failure($"Failed to get menu items: {ex.Message}");
            }
        }

        public async Task<Result<MenuItemDto>> GetMenuItemByIdAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return Result<MenuItemDto>.Failure("Menu item ID is required");
            }

            try
            {
                var cacheKey = $"{MenuItemCacheKeyPrefix}{id}";

                // Try to get from cache first
                var cached = await _cacheService.GetAsync<MenuItemDto>(cacheKey);
                if (cached != null)
                {
                    return Result<MenuItemDto>.Success(cached);
                }

                // Fetch from API
                var item = await _apiClient.GetMenuItemByIdAsync(id);

                // Cache the result
                await _cacheService.SetAsync(cacheKey, item, CacheTtl);

                return Result<MenuItemDto>.Success(item);
            }
            catch (Exception ex)
            {
                return Result<MenuItemDto>.Failure($"Failed to get menu item: {ex.Message}");
            }
        }

        public async Task<Result<MenuItemDto>> CreateMenuItemAsync(CreateMenuItemRequest request)
        {
            if (request == null)
            {
                return Result<MenuItemDto>.Failure("Menu item request is required");
            }

            try
            {
                // Create menu item via API
                var result = await _apiClient.CreateMenuItemAsync(request);

                // Invalidate list and category caches
                _cacheService.InvalidatePattern(AllMenuItemsCacheKey);
                _cacheService.InvalidatePattern(MenuCategoryPrefix);
                _cacheService.InvalidatePattern(AvailableMenuCacheKey);

                return Result<MenuItemDto>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<MenuItemDto>.Failure($"Failed to create menu item: {ex.Message}");
            }
        }

        public async Task<Result<MenuItemDto>> UpdateMenuItemAsync(
            string id,
            UpdateMenuItemRequest request)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return Result<MenuItemDto>.Failure("Menu item ID is required");
            }

            if (request == null)
            {
                return Result<MenuItemDto>.Failure("Update request is required");
            }

            try
            {
                // Update menu item via API
                var result = await _apiClient.UpdateMenuItemAsync(id, request);

                // Invalidate caches
                _cacheService.InvalidatePattern(AllMenuItemsCacheKey);
                _cacheService.InvalidatePattern(MenuCategoryPrefix);
                _cacheService.InvalidatePattern(AvailableMenuCacheKey);
                await _cacheService.RemoveAsync($"{MenuItemCacheKeyPrefix}{id}");

                return Result<MenuItemDto>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<MenuItemDto>.Failure($"Failed to update menu item: {ex.Message}");
            }
        }

        public async Task<Result> DeleteMenuItemAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return Result.Failure("Menu item ID is required");
            }

            try
            {
                // Delete menu item via API
                var response = await _apiClient.DeleteMenuItemAsync(id);

                if (!response.IsSuccessStatusCode)
                {
                    return Result.Failure($"Failed to delete menu item: HTTP {response.StatusCode}");
                }

                // Invalidate caches
                _cacheService.InvalidatePattern(AllMenuItemsCacheKey);
                _cacheService.InvalidatePattern(MenuCategoryPrefix);
                _cacheService.InvalidatePattern(AvailableMenuCacheKey);
                await _cacheService.RemoveAsync($"{MenuItemCacheKeyPrefix}{id}");

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure($"Failed to delete menu item: {ex.Message}");
            }
        }

        public async Task<Result<List<MenuItemDto>>> GetMenuItemsByCategoryAsync(string category)
        {
            if (string.IsNullOrWhiteSpace(category))
            {
                return Result<List<MenuItemDto>>.Failure("Category is required");
            }

            try
            {
                var cacheKey = $"{MenuCategoryPrefix}{category}";

                // Try to get from cache first
                var cached = await _cacheService.GetAsync<List<MenuItemDto>>(cacheKey);
                if (cached != null)
                {
                    return Result<List<MenuItemDto>>.Success(cached);
                }

                // Get all items and filter by category
                var allItemsResult = await GetMenuItemsAsync();
                if (!allItemsResult.IsSuccess)
                {
                    return Result<List<MenuItemDto>>.Failure(allItemsResult.Error ?? "Failed to get menu items");
                }

                var filtered = (allItemsResult.Value ?? new List<MenuItemDto>())
                    .Where(item => item.Category.Equals(category, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                // Cache the result
                await _cacheService.SetAsync(cacheKey, filtered, CacheTtl);

                return Result<List<MenuItemDto>>.Success(filtered);
            }
            catch (Exception ex)
            {
                return Result<List<MenuItemDto>>.Failure($"Failed to get menu items by category: {ex.Message}");
            }
        }

        public async Task<Result<List<MenuItemDto>>> GetAvailableMenuItemsAsync()
        {
            try
            {
                // Try to get from cache first
                var cached = await _cacheService.GetAsync<List<MenuItemDto>>(AvailableMenuCacheKey);
                if (cached != null)
                {
                    return Result<List<MenuItemDto>>.Success(cached);
                }

                // Get all items and filter by availability
                var allItemsResult = await GetMenuItemsAsync();
                if (!allItemsResult.IsSuccess)
                {
                    return Result<List<MenuItemDto>>.Failure(allItemsResult.Error ?? "Failed to get menu items");
                }

                var available = (allItemsResult.Value ?? new List<MenuItemDto>())
                    .Where(item => item.IsAvailable)
                    .ToList();

                // Cache the result
                await _cacheService.SetAsync(AvailableMenuCacheKey, available, CacheTtl);

                return Result<List<MenuItemDto>>.Success(available);
            }
            catch (Exception ex)
            {
                return Result<List<MenuItemDto>>.Failure($"Failed to get available menu items: {ex.Message}");
            }
        }
    }
}
