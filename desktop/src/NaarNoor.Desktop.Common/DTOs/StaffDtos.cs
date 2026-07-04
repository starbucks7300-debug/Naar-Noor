namespace NaarNoor.Desktop.Common.DTOs
{
    /// <summary>
    /// Data transfer object for staff/chef information
    /// </summary>
    public class StaffDto
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public required string Role { get; set; } // Chef, Waiter, Manager, Admin
        public required string Status { get; set; } // available, busy, break
        public string? ScheduledHours { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    /// <summary>
    /// Data transfer object specific to chef operations
    /// </summary>
    public class ChefDto
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public required string Status { get; set; } // available, busy, break
        public int? AssignedOrders { get; set; }
        public DateTime? LastAssignment { get; set; }
    }

    /// <summary>
    /// Request DTO for updating staff status
    /// </summary>
    public class UpdateStaffStatusRequest
    {
        public required string Status { get; set; } // available, busy, break
    }
}
