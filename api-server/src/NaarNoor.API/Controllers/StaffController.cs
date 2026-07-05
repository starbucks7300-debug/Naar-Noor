using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NaarNoor.Application.Common.Interfaces;

namespace NaarNoor.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StaffController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public StaffController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var staff = await _unitOfWork.Chefs.Query()
            .Where(c => c.IsActive)
            .OrderBy(c => c.SortOrder)
            .Select(c => new
            {
                id             = c.Id.ToString(),
                name           = c.Name,
                role           = c.Title,
                status         = "available",
                scheduledHours = (string?)null,
                createdAt      = c.CreatedAt,
                updatedAt      = c.UpdatedAt,
            })
            .ToListAsync(cancellationToken);

        return Ok(staff);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var staff = await _unitOfWork.Chefs.Query()
            .Where(c => c.Id == id && c.IsActive)
            .Select(c => new
            {
                id             = c.Id.ToString(),
                name           = c.Name,
                role           = c.Title,
                status         = "available",
                scheduledHours = (string?)null,
                createdAt      = c.CreatedAt,
                updatedAt      = c.UpdatedAt,
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (staff is null) return NotFound();
        return Ok(staff);
    }

    [Authorize]
    [HttpPut("{id:guid}/status")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateStaffStatusBody body, CancellationToken cancellationToken)
    {
        var chef = await _unitOfWork.Chefs.Query()
            .FirstOrDefaultAsync(c => c.Id == id && c.IsActive, cancellationToken);

        if (chef is null) return NotFound();

        return Ok(new
        {
            id             = chef.Id.ToString(),
            name           = chef.Name,
            role           = chef.Title,
            status         = body.Status,
            scheduledHours = (string?)null,
            createdAt      = chef.CreatedAt,
            updatedAt      = DateTime.UtcNow,
        });
    }
}

public class UpdateStaffStatusBody
{
    public string Status { get; set; } = "available";
}
