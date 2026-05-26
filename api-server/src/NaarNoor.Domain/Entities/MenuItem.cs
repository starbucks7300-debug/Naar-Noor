using NaarNoor.Domain.Common;
using NaarNoor.Domain.Enums;

namespace NaarNoor.Domain.Entities;

public class MenuItem : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public MenuCategory Category { get; set; }
    public bool IsVegetarian { get; set; }
    public bool IsVegan { get; set; }
    public bool IsGlutenFree { get; set; }
    public bool IsAvailable { get; set; } = true;
    public string? ImageUrl { get; set; }
    public int SortOrder { get; set; }
}
