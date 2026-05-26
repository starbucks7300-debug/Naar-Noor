using MediatR;
using Microsoft.EntityFrameworkCore;
using NaarNoor.Application.Common.Interfaces;
using NaarNoor.Domain.Enums;

namespace NaarNoor.Application.MenuItems.Queries.GetMenuItems;

public class GetMenuItemsQueryHandler : IRequestHandler<GetMenuItemsQuery, List<MenuItemDto>>
{
    private readonly IApplicationDbContext _context;

    public GetMenuItemsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<MenuItemDto>> Handle(GetMenuItemsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.MenuItems.Where(m => m.IsAvailable);

        if (!string.IsNullOrWhiteSpace(request.Category) &&
            Enum.TryParse<MenuCategory>(request.Category, true, out var category))
        {
            query = query.Where(m => m.Category == category);
        }

        return await query
            .OrderBy(m => m.Category)
            .ThenBy(m => m.SortOrder)
            .Select(m => new MenuItemDto(
                m.Id,
                m.Name,
                m.Description,
                m.Price,
                m.Category.ToString(),
                m.IsVegetarian,
                m.IsVegan,
                m.IsGlutenFree,
                m.IsAvailable,
                m.ImageUrl,
                m.SortOrder
            ))
            .ToListAsync(cancellationToken);
    }
}
