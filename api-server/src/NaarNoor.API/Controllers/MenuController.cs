using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NaarNoor.Application.MenuItems.Commands.CreateMenuItem;
using NaarNoor.Application.MenuItems.Commands.DeleteMenuItem;
using NaarNoor.Application.MenuItems.Commands.UpdateMenuItem;
using NaarNoor.Application.MenuItems.Queries.GetMenuItemById;
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
    [ProducesResponseType(typeof(List<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] string? category = null, CancellationToken cancellationToken = default)
    {
        var items = await _mediator.Send(new GetMenuItemsQuery(category), cancellationToken);
        return Ok(items.Select(MapToDesktopDto));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var item = await _mediator.Send(new GetMenuItemByIdQuery(id), cancellationToken);
        if (item is null) return NotFound();
        return Ok(MapToDesktopDto(item));
    }

    [Authorize]
    [HttpPost]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateMenuItemRequest body, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var command = new CreateMenuItemCommand(
            Name:        body.Name ?? body.NameEn ?? "Unnamed",
            Description: body.Description ?? body.DescriptionEn,
            Price:       body.Price,
            Category:    body.Category,
            IsVegetarian: body.IsVegetarian,
            IsVegan:     body.IsVegan,
            IsGlutenFree: body.IsGlutenFree,
            IsAvailable: body.IsAvailable,
            ImageUrl:    body.ImageUrl,
            SortOrder:   body.SortOrder
        );

        var id = await _mediator.Send(command, cancellationToken);
        var created = await _mediator.Send(new GetMenuItemByIdQuery(id), cancellationToken);
        return Created($"/api/menu/{id}", MapToDesktopDto(created!));
    }

    [Authorize]
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateMenuItemRequest body, CancellationToken cancellationToken)
    {
        var command = new UpdateMenuItemCommand(
            Id:          id,
            Name:        body.Name ?? body.NameEn,
            Description: body.Description ?? body.DescriptionEn,
            Price:       body.Price,
            Category:    body.Category,
            IsVegetarian: body.IsVegetarian,
            IsVegan:     body.IsVegan,
            IsGlutenFree: body.IsGlutenFree,
            IsAvailable: body.IsAvailable,
            ImageUrl:    body.ImageUrl
        );

        var updated = await _mediator.Send(command, cancellationToken);
        if (!updated) return NotFound();

        var item = await _mediator.Send(new GetMenuItemByIdQuery(id), cancellationToken);
        return Ok(MapToDesktopDto(item!));
    }

    [Authorize]
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _mediator.Send(new DeleteMenuItemCommand(id), cancellationToken);
        return deleted ? NoContent() : NotFound();
    }

    private static object MapToDesktopDto(MenuItemDto m) => new
    {
        id           = m.Id.ToString(),
        nameEn       = m.Name,
        nameAr       = m.Name,
        descriptionEn = m.Description,
        descriptionAr = m.Description,
        name         = m.Name,
        description  = m.Description,
        category     = m.Category,
        price        = m.Price,
        isVegetarian = m.IsVegetarian,
        isVegan      = m.IsVegan,
        isGlutenFree = m.IsGlutenFree,
        isAvailable  = m.IsAvailable,
        imageUrl     = m.ImageUrl,
        sortOrder    = m.SortOrder,
        createdAt    = DateTime.UtcNow,
        updatedAt    = DateTime.UtcNow,
    };
}

public class CreateMenuItemRequest
{
    public string? Name        { get; set; }
    public string? NameEn      { get; set; }
    public string? NameAr      { get; set; }
    public string? Description { get; set; }
    public string? DescriptionEn { get; set; }
    public string? DescriptionAr { get; set; }
    public string  Category    { get; set; } = "Mains";
    public decimal Price       { get; set; }
    public bool    IsVegetarian { get; set; }
    public bool    IsVegan      { get; set; }
    public bool    IsGlutenFree { get; set; }
    public bool    IsAvailable  { get; set; } = true;
    public string? ImageUrl     { get; set; }
    public int     SortOrder    { get; set; }
}

public class UpdateMenuItemRequest
{
    public string?  Name         { get; set; }
    public string?  NameEn       { get; set; }
    public string?  NameAr       { get; set; }
    public string?  Description  { get; set; }
    public string?  DescriptionEn { get; set; }
    public string?  DescriptionAr { get; set; }
    public string?  Category     { get; set; }
    public decimal? Price        { get; set; }
    public bool?    IsVegetarian { get; set; }
    public bool?    IsVegan      { get; set; }
    public bool?    IsGlutenFree { get; set; }
    public bool?    IsAvailable  { get; set; }
    public string?  ImageUrl     { get; set; }
}
