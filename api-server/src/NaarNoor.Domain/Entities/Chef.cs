using NaarNoor.Domain.Common;

namespace NaarNoor.Domain.Entities;

public class Chef : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string Specialty { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public int SortOrder { get; set; }
}
