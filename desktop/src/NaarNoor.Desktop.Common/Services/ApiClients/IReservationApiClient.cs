using Refit;
using NaarNoor.Desktop.Common.DTOs;

namespace NaarNoor.Desktop.Common.Services.ApiClients
{
    /// <summary>
    /// Refit API client interface for reservation management endpoints
    /// </summary>
    [Headers("Accept: application/json", "Content-Type: application/json", "Authorization: Bearer")]
    public interface IReservationApiClient
    {
        /// <summary>
        /// Get paginated list of reservations with optional filtering
        /// </summary>
        [Get("/api/reservations")]
        Task<PagedResponse<ReservationDto>> GetReservationsAsync(
            [Query] int? page = null,
            [Query] int? pageSize = null,
            [Query] DateTime? fromDate = null,
            [Query] DateTime? toDate = null);

        /// <summary>
        /// Get a specific reservation by ID
        /// </summary>
        [Get("/api/reservations/{id}")]
        Task<ReservationDto> GetReservationByIdAsync(string id);

        /// <summary>
        /// Create a new reservation
        /// </summary>
        [Post("/api/reservations")]
        Task<ReservationDto> CreateReservationAsync([Body] CreateReservationRequest request);

        /// <summary>
        /// Update an existing reservation
        /// </summary>
        [Put("/api/reservations/{id}")]
        Task<ReservationDto> UpdateReservationAsync(
            string id, 
            [Body] UpdateReservationRequest request);

        /// <summary>
        /// Delete a reservation
        /// </summary>
        [Delete("/api/reservations/{id}")]
        Task<IApiResponse> DeleteReservationAsync(string id);
    }
}
