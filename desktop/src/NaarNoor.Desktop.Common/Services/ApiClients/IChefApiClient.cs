using Refit;
using NaarNoor.Desktop.Common.DTOs;

namespace NaarNoor.Desktop.Common.Services.ApiClients
{
    /// <summary>
    /// Refit API client interface for chef/staff management endpoints
    /// </summary>
    [Headers("Accept: application/json", "Content-Type: application/json", "Authorization: Bearer")]
    public interface IChefApiClient
    {
        /// <summary>
        /// Get all staff members
        /// </summary>
        [Get("/api/staff")]
        Task<List<StaffDto>> GetStaffAsync();

        /// <summary>
        /// Get a specific staff member by ID
        /// </summary>
        [Get("/api/staff/{id}")]
        Task<StaffDto> GetStaffByIdAsync(string id);

        /// <summary>
        /// Update staff member status (available/busy/break)
        /// </summary>
        [Put("/api/staff/{id}/status")]
        Task<StaffDto> UpdateStaffStatusAsync(
            string id,
            [Body] UpdateStaffStatusRequest request);

        /// <summary>
        /// Get all chefs with their current workload
        /// </summary>
        [Get("/api/chefs")]
        Task<List<ChefDto>> GetChefsAsync();

        /// <summary>
        /// Get a specific chef by ID
        /// </summary>
        [Get("/api/chefs/{id}")]
        Task<ChefDto> GetChefByIdAsync(string id);
    }
}
