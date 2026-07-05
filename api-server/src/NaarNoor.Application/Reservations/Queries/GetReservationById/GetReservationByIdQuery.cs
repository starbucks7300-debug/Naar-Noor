using MediatR;
using NaarNoor.Application.Reservations.Queries.GetReservations;

namespace NaarNoor.Application.Reservations.Queries.GetReservationById;

public record GetReservationByIdQuery(Guid Id) : IRequest<ReservationDto?>;
