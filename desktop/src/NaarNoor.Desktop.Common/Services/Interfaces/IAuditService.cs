namespace NaarNoor.Desktop.Common.Services
{
    /// <summary>
    /// Service for logging security events and sensitive operations to the audit trail
    /// Implements requirements REQ-005 for audit logging and compliance tracking
    /// </summary>
    public interface IAuditService
    {
        /// <summary>
        /// Log a security event to the audit trail
        /// </summary>
        /// <param name="userId">ID of the user performing the action</param>
        /// <param name="action">Action performed (e.g., "login", "unauthorized_access", "create", "delete")</param>
        /// <param name="resourceType">Type of resource being acted upon</param>
        /// <param name="status">Status of the action: "success" or "failure"</param>
        /// <param name="resourceId">ID of the specific resource (optional)</param>
        /// <param name="details">Additional details about the action (optional)</param>
        /// <returns>Task representing the asynchronous operation</returns>
        Task LogSecurityEventAsync(
            string userId,
            string action,
            string resourceType,
            string status,
            string? resourceId = null,
            string? details = null
        );

        /// <summary>
        /// Log an unauthorized access attempt to the audit trail
        /// </summary>
        /// <param name="userId">ID of the user attempting access</param>
        /// <param name="feature">Feature or resource that was denied</param>
        /// <param name="details">Optional context about the attempt</param>
        /// <returns>Task representing the asynchronous operation</returns>
        Task LogUnauthorizedAccessAsync(string userId, string feature, string? details = null);

        /// <summary>
        /// Log a login event
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="success">Whether the login was successful</param>
        /// <param name="details">Optional details (e.g., failure reason)</param>
        /// <returns>Task representing the asynchronous operation</returns>
        Task LogLoginAsync(string userId, bool success, string? details = null);

        /// <summary>
        /// Log a logout event
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>Task representing the asynchronous operation</returns>
        Task LogLogoutAsync(string userId);

        /// <summary>
        /// Get audit logs for a specific user
        /// </summary>
        /// <param name="userId">User ID to filter by</param>
        /// <param name="days">Number of days to look back (default 90)</param>
        /// <returns>Collection of audit log entries</returns>
        Task<IReadOnlyList<AuditLogEntry>> GetUserAuditLogsAsync(string userId, int days = 90);

        /// <summary>
        /// Get all unauthorized access attempts
        /// </summary>
        /// <param name="days">Number of days to look back (default 30)</param>
        /// <returns>Collection of unauthorized access attempts</returns>
        Task<IReadOnlyList<AuditLogEntry>> GetUnauthorizedAccessAttemptsAsync(int days = 30);
    }

    /// <summary>
    /// DTO for audit log entries
    /// </summary>
    public class AuditLogEntry
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string ResourceType { get; set; } = string.Empty;
        public string? ResourceId { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Details { get; set; }
    }
}
