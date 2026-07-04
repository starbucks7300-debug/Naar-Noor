using NaarNoor.Desktop.Common.DTOs;
using NaarNoor.Desktop.Common.Utilities;

namespace NaarNoor.Desktop.Common.Services
{
    /// <summary>
    /// Service interface for reservation management with caching and observable updates.
    /// </summary>
    public interface IReservationService
    {
        /// <summary>
        /// Get all reservations with optional date filtering.
        /// Results are cached with 30-second TTL.
        /// </summary>
        Task<Result<List<ReservationDto>>> GetReservationsAsync(
            DateTime? fromDate = null,
            DateTime? toDate = null);

        /// <summary>
        /// Get a specific reservation by ID.
        /// Results are cached with 30-second TTL.
        /// </summary>
        Task<Result<ReservationDto>> GetReservationByIdAsync(string id);

        /// <summary>
        /// Create a new reservation and invalidate cache.
        /// </summary>
        Task<Result<ReservationDto>> CreateReservationAsync(CreateReservationRequest request);

        /// <summary>
        /// Update an existing reservation and invalidate cache.
        /// </summary>
        Task<Result<ReservationDto>> UpdateReservationAsync(string id, UpdateReservationRequest request);

        /// <summary>
        /// Delete a reservation and invalidate cache.
        /// </summary>
        Task<Result> DeleteReservationAsync(string id);

        /// <summary>
        /// Observable stream of reservation updates for real-time UI notifications.
        /// </summary>
        IObservable<ReservationNotification> ReservationUpdates { get; }
    }

    /// <summary>
    /// Notification event for reservation changes (for IObservable).
    /// </summary>
    public class ReservationNotification
    {
        public required string ReservationId { get; set; }
        public required string EventType { get; set; } // "created", "updated", "deleted"
        public ReservationDto? Data { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
