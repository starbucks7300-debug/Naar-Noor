using MediatR;
using Microsoft.AspNetCore.Mvc;
using NaarNoor.Application.Chefs.Queries.GetChefs;

namespace NaarNoor.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChefsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ChefsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<ChefDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetChefsQuery(), cancellationToken);
        return Ok(result);
    }
}
