using MediatR;
using NaarNoor.Application.Common.Interfaces;
using NaarNoor.Domain.Entities;

namespace NaarNoor.Application.Reservations.Commands.CreateReservation;

public class CreateReservationCommandHandler : IRequestHandler<CreateReservationCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateReservationCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateReservationCommand request, CancellationToken cancellationToken)
    {
        var reservation = new Reservation
        {
            CustomerName = request.CustomerName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            ReservationDate = request.ReservationDate,
            ReservationTime = TimeOnly.Parse(request.ReservationTime),
            PartySize = request.PartySize,
            SpecialRequests = request.SpecialRequests
        };

        _context.Reservations.Add(reservation);
        await _context.SaveChangesAsync(cancellationToken);

        return reservation.Id;
    }
}
