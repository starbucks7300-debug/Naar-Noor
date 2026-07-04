namespace NaarNoor.Desktop.Common.DTOs
{
    /// <summary>
    /// Data transfer object for reservation display and manipulation
    /// </summary>
    public class ReservationDto
    {
        public required string Id { get; set; }
        public required string CustomerName { get; set; }
        public required int PartySize { get; set; }
        public required DateTime BookingTime { get; set; }
        public string? TableNumber { get; set; }
        public required string Status { get; set; } // confirmed, pending, arrived, completed, cancelled
        public string? CustomerEmail { get; set; }
        public string? CustomerPhone { get; set; }
        public string? SpecialRequests { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    /// <summary>
    /// Request DTO for creating a new reservation
    /// </summary>
    public class CreateReservationRequest
    {
        public required string CustomerName { get; set; }
        public required int PartySize { get; set; }
        public required DateTime BookingTime { get; set; }
        public string? TableNumber { get; set; }
        public string? CustomerEmail { get; set; }
        public string? CustomerPhone { get; set; }
        public string? SpecialRequests { get; set; }
    }

    /// <summary>
    /// Request DTO for updating an existing reservation
    /// </summary>
    public class UpdateReservationRequest
    {
        public string? CustomerName { get; set; }
        public int? PartySize { get; set; }
        public DateTime? BookingTime { get; set; }
        public string? TableNumber { get; set; }
        public string? Status { get; set; }
        public string? CustomerEmail { get; set; }
        public string? CustomerPhone { get; set; }
        public string? SpecialRequests { get; set; }
    }
}
