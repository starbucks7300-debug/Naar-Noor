using Refit;
using NaarNoor.Desktop.Common.DTOs;

namespace NaarNoor.Desktop.Common.Services.ApiClients
{
    /// <summary>
    /// Refit API client interface for menu management endpoints
    /// </summary>
    [Headers("Accept: application/json", "Content-Type: application/json", "Authorization: Bearer")]
    public interface IMenuApiClient
    {
        /// <summary>
        /// Get all menu items
        /// </summary>
        [Get("/api/menu")]
        Task<List<MenuItemDto>> GetMenuItemsAsync();

        /// <summary>
        /// Get a specific menu item by ID
        /// </summary>
        [Get("/api/menu/{id}")]
        Task<MenuItemDto> GetMenuItemByIdAsync(string id);

        /// <summary>
        /// Create a new menu item
        /// </summary>
        [Post("/api/menu")]
        Task<MenuItemDto> CreateMenuItemAsync([Body] CreateMenuItemRequest request);

        /// <summary>
        /// Update an existing menu item
        /// </summary>
        [Put("/api/menu/{id}")]
        Task<MenuItemDto> UpdateMenuItemAsync(
            string id, 
            [Body] UpdateMenuItemRequest request);

        /// <summary>
        /// Delete a menu item
        /// </summary>
        [Delete("/api/menu/{id}")]
        Task<IApiResponse> DeleteMenuItemAsync(string id);
    }
}
