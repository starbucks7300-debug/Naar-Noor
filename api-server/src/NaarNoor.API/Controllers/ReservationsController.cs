using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NaarNoor.Application.Common.Interfaces;
using NaarNoor.Application.Reservations.Commands.CreateReservation;
using NaarNoor.Application.Reservations.Commands.DeleteReservation;
using NaarNoor.Application.Reservations.Commands.UpdateReservation;
using NaarNoor.Application.Reservations.Queries.GetReservationById;
using NaarNoor.Application.Reservations.Queries.GetReservations;

namespace NaarNoor.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReservationsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IUnitOfWork _unitOfWork;

    public ReservationsController(IMediator mediator, IUnitOfWork unitOfWork)
    {
        _mediator = mediator;
        _unitOfWork = unitOfWork;
    }

    [Authorize]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateReservationBody body, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        DateOnly date;
        TimeOnly time = TimeOnly.FromDateTime(DateTime.Now);

        if (body.BookingTime.HasValue)
        {
            date = DateOnly.FromDateTime(body.BookingTime.Value);
            time = TimeOnly.FromDateTime(body.BookingTime.Value);
        }
        else
        {
            date = body.ReservationDate ?? DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
            if (!string.IsNullOrWhiteSpace(body.ReservationTime) &&
                TimeOnly.TryParse(body.ReservationTime, out var parsedTime))
                time = parsedTime;
        }

        var command = new CreateReservationCommand(
            CustomerName:    body.CustomerName,
            Email:           body.CustomerEmail ?? body.Email ?? "guest@naarnoor.com",
            PhoneNumber:     body.CustomerPhone ?? body.PhoneNumber ?? "",
            ReservationDate: date,
            ReservationTime: body.ReservationTime ?? time.ToString("HH:mm"),
            PartySize:       body.PartySize,
            SpecialRequests: body.SpecialRequests
        );

        var id = await _mediator.Send(command, cancellationToken);
        return Created(string.Empty, new { id });
    }

    [Authorize]
    [HttpGet]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        CancellationToken cancellationToken = default)
    {
        var query = _unitOfWork.Reservations.Query();

        if (fromDate.HasValue)
            query = query.Where(r => r.ReservationDate >= DateOnly.FromDateTime(fromDate.Value));
        if (toDate.HasValue)
            query = query.Where(r => r.ReservationDate <= DateOnly.FromDateTime(toDate.Value));

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(r => r.ReservationDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(r => new
            {
                id              = r.Id.ToString(),
                customerName    = r.CustomerName,
                customerEmail   = r.Email,
                customerPhone   = r.PhoneNumber,
                email           = r.Email,
                phoneNumber     = r.PhoneNumber,
                reservationDate = r.ReservationDate.ToString("yyyy-MM-dd"),
                reservationTime = r.ReservationTime.ToString("HH:mm"),
                bookingTime     = r.ReservationDate.ToDateTime(r.ReservationTime),
                partySize       = r.PartySize,
                tableNumber     = (string?)null,
                status          = r.Status.ToString(),
                specialRequests = r.SpecialRequests,
                createdAt       = r.CreatedAt,
                updatedAt       = r.UpdatedAt,
            })
            .ToListAsync(cancellationToken);

        return Ok(new
        {
            data     = items,
            page     = page,
            pageSize = pageSize,
            total    = total,
        });
    }

    [Authorize]
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var r = await _unitOfWork.Reservations.Query()
            .Where(r => r.Id == id)
            .Select(r => new
            {
                id              = r.Id.ToString(),
                customerName    = r.CustomerName,
                customerEmail   = r.Email,
                customerPhone   = r.PhoneNumber,
                email           = r.Email,
                phoneNumber     = r.PhoneNumber,
                reservationDate = r.ReservationDate.ToString("yyyy-MM-dd"),
                reservationTime = r.ReservationTime.ToString("HH:mm"),
                bookingTime     = r.ReservationDate.ToDateTime(r.ReservationTime),
                partySize       = r.PartySize,
                tableNumber     = (string?)null,
                status          = r.Status.ToString(),
                specialRequests = r.SpecialRequests,
                createdAt       = r.CreatedAt,
                updatedAt       = r.UpdatedAt,
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (r is null) return NotFound();
        return Ok(r);
    }

    [Authorize]
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateReservationBody body, CancellationToken cancellationToken)
    {
        var command = new UpdateReservationCommand(
            Id:             id,
            CustomerName:   body.CustomerName,
            Email:          body.CustomerEmail ?? body.Email,
            PhoneNumber:    body.CustomerPhone ?? body.PhoneNumber,
            Status:         body.Status,
            PartySize:      body.PartySize,
            SpecialRequests: body.SpecialRequests
        );

        var updated = await _mediator.Send(command, cancellationToken);
        if (!updated) return NotFound();

        return await GetById(id, cancellationToken);
    }

    [Authorize]
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _mediator.Send(new DeleteReservationCommand(id), cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}

public class CreateReservationBody
{
    public string  CustomerName    { get; set; } = "";
    public int     PartySize       { get; set; }
    public DateTime? BookingTime   { get; set; }
    public DateOnly? ReservationDate { get; set; }
    public string? ReservationTime { get; set; }
    public string? TableNumber     { get; set; }
    public string? CustomerEmail   { get; set; }
    public string? CustomerPhone   { get; set; }
    public string? Email           { get; set; }
    public string? PhoneNumber     { get; set; }
    public string? SpecialRequests { get; set; }
}

public class UpdateReservationBody
{
    public string?   CustomerName    { get; set; }
    public int?      PartySize       { get; set; }
    public DateTime? BookingTime     { get; set; }
    public string?   TableNumber     { get; set; }
    public string?   Status          { get; set; }
    public string?   CustomerEmail   { get; set; }
    public string?   CustomerPhone   { get; set; }
    public string?   Email           { get; set; }
    public string?   PhoneNumber     { get; set; }
    public string?   SpecialRequests { get; set; }
}
