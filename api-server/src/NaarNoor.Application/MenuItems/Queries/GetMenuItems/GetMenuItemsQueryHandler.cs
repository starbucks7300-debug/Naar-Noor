using NaarNoor.Domain.Entities;
using NaarNoor.Domain.Enums;

namespace NaarNoor.Application.MenuItems.Queries.GetMenuItems;

/// <summary>
/// Centralized EF projection from MenuItem entity → MenuItemDto.
/// Used by all menu item query handlers to avoid duplicating Select(...) expressions.
/// </summary>
internal static class MenuItemProjection
{
    internal static IQueryable<MenuItemDto> ProjectToDto(this IQueryable<MenuItem> source)
        => source.Select(m => new MenuItemDto(
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
            m.SortOrder));

    internal static IQueryable<MenuItem> FilterByCategory(
        this IQueryable<MenuItem> source, string? category)
    {
        if (string.IsNullOrWhiteSpace(category)) return source;
        return Enum.TryParse<MenuCategory>(category, ignoreCase: true, out var cat)
            ? source.Where(m => m.Category == cat)
            : source;
    }
}
