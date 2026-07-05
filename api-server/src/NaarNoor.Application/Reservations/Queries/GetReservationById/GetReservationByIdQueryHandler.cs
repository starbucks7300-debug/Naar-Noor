using MediatR;
using Microsoft.EntityFrameworkCore;
using NaarNoor.Application.Common.Interfaces;
using NaarNoor.Application.Reservations.Queries.GetReservations;

namespace NaarNoor.Application.Reservations.Queries.GetReservationById;

public class GetReservationByIdQueryHandler : IRequestHandler<GetReservationByIdQuery, ReservationDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetReservationByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ReservationDto?> Handle(GetReservationByIdQuery request, CancellationToken cancellationToken)
    {
        return await _unitOfWork.Reservations.Query()
            .Where(r => r.Id == request.Id)
            .Select(r => new ReservationDto(
                r.Id,
                r.CustomerName,
                r.Email,
                r.PhoneNumber,
                r.ReservationDate,
                r.ReservationTime.ToString("HH:mm"),
                r.PartySize,
                r.Status.ToString(),
                r.SpecialRequests,
                r.CreatedAt
            ))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
