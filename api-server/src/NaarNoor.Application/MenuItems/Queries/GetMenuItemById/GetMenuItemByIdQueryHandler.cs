using MediatR;
using Microsoft.EntityFrameworkCore;
using NaarNoor.Application.Common.Interfaces;
using NaarNoor.Application.MenuItems.Queries.GetMenuItems;

namespace NaarNoor.Application.MenuItems.Queries.GetMenuItemById;

public class GetMenuItemByIdQueryHandler : IRequestHandler<GetMenuItemByIdQuery, MenuItemDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetMenuItemByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<MenuItemDto?> Handle(GetMenuItemByIdQuery request, CancellationToken cancellationToken)
    {
        return await _unitOfWork.MenuItems.Query()
            .Where(m => m.Id == request.Id)
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
            .FirstOrDefaultAsync(cancellationToken);
    }
}
