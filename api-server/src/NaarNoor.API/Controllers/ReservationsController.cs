using MediatR;
using Microsoft.AspNetCore.Mvc;
using NaarNoor.Application.Reservations.Commands.CreateReservation;
using NaarNoor.Application.Reservations.Queries.GetReservations;

namespace NaarNoor.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReservationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReservationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateReservationCommand command, CancellationToken cancellationToken)
    {
        var id = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetAll), new { id }, new { id });
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<ReservationDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetReservationsQuery(page, pageSize), cancellationToken);
        return Ok(result);
    }
}
