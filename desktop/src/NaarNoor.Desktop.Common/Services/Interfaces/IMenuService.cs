using NaarNoor.Desktop.Common.DTOs;
using NaarNoor.Desktop.Common.Utilities;

namespace NaarNoor.Desktop.Common.Services
{
    /// <summary>
    /// Service interface for menu item management with caching.
    /// </summary>
    public interface IMenuService
    {
        /// <summary>
        /// Get all menu items.
        /// Results are cached with 2-hour TTL for stability.
        /// </summary>
        Task<Result<List<MenuItemDto>>> GetMenuItemsAsync();

        /// <summary>
        /// Get a specific menu item by ID.
        /// Results are cached with 2-hour TTL.
        /// </summary>
        Task<Result<MenuItemDto>> GetMenuItemByIdAsync(string id);

        /// <summary>
        /// Create a new menu item and invalidate cache.
        /// </summary>
        Task<Result<MenuItemDto>> CreateMenuItemAsync(CreateMenuItemRequest request);

        /// <summary>
        /// Update an existing menu item and invalidate cache.
        /// </summary>
        Task<Result<MenuItemDto>> UpdateMenuItemAsync(string id, UpdateMenuItemRequest request);

        /// <summary>
        /// Delete a menu item and invalidate cache.
        /// </summary>
        Task<Result> DeleteMenuItemAsync(string id);

        /// <summary>
        /// Get menu items filtered by category.
        /// Results are cached with 2-hour TTL.
        /// </summary>
        Task<Result<List<MenuItemDto>>> GetMenuItemsByCategoryAsync(string category);

        /// <summary>
        /// Get available menu items.
        /// Results are cached with 2-hour TTL.
        /// </summary>
        Task<Result<List<MenuItemDto>>> GetAvailableMenuItemsAsync();
    }
}
