using MediatR;

namespace NaarNoor.Application.MenuItems.Queries.GetMenuItems;

public record MenuItemDto(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    string Category,
    bool IsVegetarian,
    bool IsVegan,
    bool IsGlutenFree,
    bool IsAvailable,
    string? ImageUrl,
    int SortOrder
);

public record GetMenuItemsQuery(string? Category = null) : IRequest<List<MenuItemDto>>;
