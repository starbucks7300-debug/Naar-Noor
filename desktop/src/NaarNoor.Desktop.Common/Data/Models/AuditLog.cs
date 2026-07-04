namespace NaarNoor.Desktop.Common.Data.Models
{
    /// <summary>
    /// Represents an audit log entry for security and compliance tracking
    /// Records all security events and sensitive operations
    /// </summary>
    public class AuditLog
    {
        /// <summary>
        /// Primary key for the audit log entry
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Timestamp when the event occurred
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// User ID who performed the action
        /// </summary>
        public required string UserId { get; set; }

        /// <summary>
        /// Action performed (e.g., "login", "logout", "create", "update", "delete")
        /// </summary>
        public required string Action { get; set; }

        /// <summary>
        /// Type of resource being acted upon
        /// </summary>
        public required string ResourceType { get; set; }

        /// <summary>
        /// ID of the specific resource (null for operations not tied to a specific resource)
        /// </summary>
        public string? ResourceId { get; set; }

        /// <summary>
        /// Status of the action: "success" or "failure"
        /// </summary>
        public required string Status { get; set; }

        /// <summary>
        /// Additional details about the action (e.g., error messages, context)
        /// </summary>
        public string? Details { get; set; }
    }
}
