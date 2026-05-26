using MediatR;

namespace NaarNoor.Application.Reservations.Queries.GetReservations;

public record ReservationDto(
    Guid Id,
    string CustomerName,
    string Email,
    string PhoneNumber,
    DateOnly ReservationDate,
    string ReservationTime,
    int PartySize,
    string Status,
    string? SpecialRequests,
    DateTime CreatedAt
);

public record GetReservationsQuery(int Page = 1, int PageSize = 20) : IRequest<List<ReservationDto>>;
