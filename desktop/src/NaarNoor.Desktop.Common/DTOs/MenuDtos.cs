namespace NaarNoor.Desktop.Common.DTOs
{
    /// <summary>
    /// Data transfer object for menu item display and manipulation
    /// </summary>
    public class MenuItemDto
    {
        public required string Id { get; set; }
        public required string NameEn { get; set; }
        public required string NameAr { get; set; }
        public string? DescriptionEn { get; set; }
        public string? DescriptionAr { get; set; }
        public required string Category { get; set; }
        public required decimal Price { get; set; }
        public bool IsAvailable { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    /// <summary>
    /// Request DTO for creating a new menu item
    /// </summary>
    public class CreateMenuItemRequest
    {
        public required string NameEn { get; set; }
        public required string NameAr { get; set; }
        public string? DescriptionEn { get; set; }
        public string? DescriptionAr { get; set; }
        public required string Category { get; set; }
        public required decimal Price { get; set; }
        public bool IsAvailable { get; set; } = true;
    }

    /// <summary>
    /// Request DTO for updating an existing menu item
    /// </summary>
    public class UpdateMenuItemRequest
    {
        public string? NameEn { get; set; }
        public string? NameAr { get; set; }
        public string? DescriptionEn { get; set; }
        public string? DescriptionAr { get; set; }
        public string? Category { get; set; }
        public decimal? Price { get; set; }
        public bool? IsAvailable { get; set; }
    }
}
