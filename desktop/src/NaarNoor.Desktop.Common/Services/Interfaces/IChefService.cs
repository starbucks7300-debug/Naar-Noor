using NaarNoor.Desktop.Common.DTOs;
using NaarNoor.Desktop.Common.Utilities;

namespace NaarNoor.Desktop.Common.Services
{
    /// <summary>
    /// Service interface for staff and chef management with observable updates.
    /// </summary>
    public interface IChefService
    {
        /// <summary>
        /// Get all staff members.
        /// Results are cached with 15-minute TTL.
        /// </summary>
        Task<Result<List<StaffDto>>> GetStaffAsync();

        /// <summary>
        /// Get staff members filtered by role.
        /// Results are cached with 15-minute TTL.
        /// </summary>
        Task<Result<List<StaffDto>>> GetStaffByRoleAsync(string role);

        /// <summary>
        /// Get available (not on break/busy) staff members.
        /// Results are cached with 5-minute TTL (frequent updates).
        /// </summary>
        Task<Result<List<StaffDto>>> GetAvailableStaffAsync();

        /// <summary>
        /// Update staff member status (available, busy, break).
        /// Invalidates cache and publishes observable update.
        /// </summary>
        Task<Result<StaffDto>> UpdateStaffStatusAsync(string staffId, UpdateStaffStatusRequest request);

        /// <summary>
        /// Observable stream of staff status updates for real-time UI notifications.
        /// </summary>
        IObservable<StaffStatusNotification> StaffStatusUpdates { get; }
    }

    /// <summary>
    /// Notification event for staff status changes (for IObservable).
    /// </summary>
    public class StaffStatusNotification
    {
        public required string StaffId { get; set; }
        public required string Name { get; set; }
        public required string Status { get; set; } // available, busy, break
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
