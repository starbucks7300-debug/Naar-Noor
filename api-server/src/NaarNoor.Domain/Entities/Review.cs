using NaarNoor.Domain.Common;

namespace NaarNoor.Domain.Entities;

public class Review : BaseEntity
{
    public string CustomerName { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public bool IsApproved { get; set; }
    public string? Source { get; set; }
}
