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

    private static readonly Dictionary<ReservationStatus, ReservationStatus[]> ValidTransitions = new()
    {
        [ReservationStatus.Pending] = new[] { ReservationStatus.Confirmed, ReservationStatus.Cancelled },
        [ReservationStatus.Confirmed] = new[] { ReservationStatus.Completed, ReservationStatus.Cancelled },
        [ReservationStatus.Cancelled] = Array.Empty<ReservationStatus>(),
        [ReservationStatus.Completed] = Array.Empty<ReservationStatus>(),
    };

    /// <summary>
    /// Transitions the reservation to a new status, enforcing the valid state machine rules.
    /// Pending -> Confirmed | Cancelled
    /// Confirmed -> Completed | Cancelled
    /// Cancelled and Completed are terminal states.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the requested transition is not allowed.</exception>
    public void TransitionTo(ReservationStatus newStatus)
    {
        if (!ValidTransitions.TryGetValue(Status, out var allowedNextStates) ||
            !allowedNextStates.Contains(newStatus))
        {
            throw new InvalidOperationException(
                $"Cannot transition reservation from '{Status}' to '{newStatus}'.");
        }

        Status = newStatus;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Verifies that the reservation satisfies all business rule invariants:
    /// a positive party size, a valid (defined) status, and non-empty contact details.
    /// </summary>
    public bool IsInValidState()
    {
        return PartySize > 0
            && Enum.IsDefined(typeof(ReservationStatus), Status)
            && !string.IsNullOrWhiteSpace(CustomerName)
            && !string.IsNullOrWhiteSpace(Email)
            && !string.IsNullOrWhiteSpace(PhoneNumber);
    }
}
