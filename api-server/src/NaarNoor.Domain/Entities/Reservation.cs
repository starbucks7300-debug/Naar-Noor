using NaarNoor.Domain.Common;
using NaarNoor.Domain.Enums;

namespace NaarNoor.Domain.Entities;

public class Reservation : BaseEntity
{
    public string CustomerName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public DateOnly ReservationDate { get; set; }
    public TimeOnly ReservationTime { get; set; }
    public int PartySize { get; set; }
    public ReservationStatus Status { get; set; } = ReservationStatus.Pending;
    public string? SpecialRequests { get; set; }
}
