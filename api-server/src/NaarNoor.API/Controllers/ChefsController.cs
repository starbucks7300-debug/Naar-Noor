using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NaarNoor.Application.Chefs.Queries.GetChefs;
using NaarNoor.Application.Common.Interfaces;

namespace NaarNoor.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChefsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IUnitOfWork _unitOfWork;

    public ChefsController(IMediator mediator, IUnitOfWork unitOfWork)
    {
        _mediator = mediator;
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var chefs = await _mediator.Send(new GetChefsQuery(), cancellationToken);
        return Ok(chefs.Select(MapToDesktopDto));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var chef = await _unitOfWork.Chefs.Query()
            .Where(c => c.Id == id && c.IsActive)
            .Select(c => new ChefDto(c.Id, c.Name, c.Title, c.Bio, c.ImageUrl, c.Specialty, c.SortOrder))
            .FirstOrDefaultAsync(cancellationToken);

        if (chef is null) return NotFound();
        return Ok(MapToDesktopDto(chef));
    }

    private static object MapToDesktopDto(ChefDto c) => new
    {
        id              = c.Id.ToString(),
        name            = c.Name,
        title           = c.Title,
        bio             = c.Bio,
        imageUrl        = c.ImageUrl,
        specialty       = c.Specialty,
        sortOrder       = c.SortOrder,
        status          = "available",
        assignedOrders  = 0,
        lastAssignment  = (DateTime?)null,
    };
}
