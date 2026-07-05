using MediatR;
using NaarNoor.Application.MenuItems.Queries.GetMenuItems;

namespace NaarNoor.Application.MenuItems.Queries.GetMenuItemById;

public record GetMenuItemByIdQuery(Guid Id) : IRequest<MenuItemDto?>;
