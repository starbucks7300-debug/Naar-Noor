using MediatR;
using Microsoft.AspNetCore.Mvc;
using NaarNoor.Application.MenuItems.Queries.GetMenuItems;

namespace NaarNoor.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MenuController : ControllerBase
{
    private readonly IMediator _mediator;

    public MenuController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<MenuItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] string? category = null, CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetMenuItemsQuery(category), cancellationToken);
        return Ok(result);
    }
}
