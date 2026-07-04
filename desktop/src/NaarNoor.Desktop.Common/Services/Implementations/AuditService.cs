using NaarNoor.Desktop.Common.Data;
using NaarNoor.Desktop.Common.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace NaarNoor.Desktop.Common.Services
{
    /// <summary>
    /// Service for logging security events and sensitive operations to the audit trail
    /// Implements requirements REQ-005 for audit logging and compliance tracking
    /// </summary>
    public class AuditService : IAuditService
    {
        private readonly DatabaseContext _dbContext;

        public AuditService(DatabaseContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        /// <summary>
        /// Log a security event to the audit trail
        /// </summary>
        public async Task LogSecurityEventAsync(
            string userId,
            string action,
            string resourceType,
            string status,
            string? resourceId = null,
            string? details = null)
        {
            try
            {
                var auditLog = new AuditLog
                {
                    Timestamp = DateTime.UtcNow,
                    UserId = userId ?? "unknown",
                    Action = action,
                    ResourceType = resourceType,
                    ResourceId = resourceId,
                    Status = status,
                    Details = details
                };

                _dbContext.AuditLogs.Add(auditLog);
                await _dbContext.SaveChangesAsync();

                Debug.WriteLine($"[AUDIT] Logged event: Action={action}, UserId={userId}, Status={status}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR] Failed to log security event: {ex.Message}");
                // Don't throw - logging failures should not crash the application
            }
        }

        /// <summary>
        /// Log an unauthorized access attempt to the audit trail
        /// </summary>
        public async Task LogUnauthorizedAccessAsync(string userId, string feature, string? details = null)
        {
            try
            {
                var fullDetails = $"Unauthorized access attempt to feature: {feature}";
                if (!string.IsNullOrEmpty(details))
                {
                    fullDetails += $". Context: {details}";
                }

                await LogSecurityEventAsync(
                    userId,
                    "unauthorized_access",
                    "Feature",
                    "failure",
                    feature,
                    fullDetails
                );
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR] Failed to log unauthorized access: {ex.Message}");
            }
        }

        /// <summary>
        /// Log a login event
        /// </summary>
        public async Task LogLoginAsync(string userId, bool success, string? details = null)
        {
            try
            {
                var status = success ? "success" : "failure";
                await LogSecurityEventAsync(
                    userId,
                    "login",
                    "Authentication",
                    status,
                    userId,
                    details
                );
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR] Failed to log login event: {ex.Message}");
            }
        }

        /// <summary>
        /// Log a logout event
        /// </summary>
        public async Task LogLogoutAsync(string userId)
        {
            try
            {
                await LogSecurityEventAsync(
                    userId,
                    "logout",
                    "Authentication",
                    "success",
                    userId
                );
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR] Failed to log logout event: {ex.Message}");
            }
        }

        /// <summary>
        /// Get audit logs for a specific user
        /// </summary>
        public async Task<IReadOnlyList<AuditLogEntry>> GetUserAuditLogsAsync(string userId, int days = 90)
        {
            try
            {
                var cutoffDate = DateTime.UtcNow.AddDays(-days);

                var logs = await _dbContext.AuditLogs
                    .Where(a => a.UserId == userId && a.Timestamp >= cutoffDate)
                    .OrderByDescending(a => a.Timestamp)
                    .Select(a => new AuditLogEntry
                    {
                        Id = a.Id,
                        Timestamp = a.Timestamp,
                        UserId = a.UserId,
                        Action = a.Action,
                        ResourceType = a.ResourceType,
                        ResourceId = a.ResourceId,
                        Status = a.Status,
                        Details = a.Details
                    })
                    .ToListAsync();

                return logs;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR] Failed to retrieve user audit logs: {ex.Message}");
                return Array.Empty<AuditLogEntry>();
            }
        }

        /// <summary>
        /// Get all unauthorized access attempts
        /// </summary>
        public async Task<IReadOnlyList<AuditLogEntry>> GetUnauthorizedAccessAttemptsAsync(int days = 30)
        {
            try
            {
                var cutoffDate = DateTime.UtcNow.AddDays(-days);

                var logs = await _dbContext.AuditLogs
                    .Where(a => a.Action == "unauthorized_access" && a.Timestamp >= cutoffDate)
                    .OrderByDescending(a => a.Timestamp)
                    .Select(a => new AuditLogEntry
                    {
                        Id = a.Id,
                        Timestamp = a.Timestamp,
                        UserId = a.UserId,
                        Action = a.Action,
                        ResourceType = a.ResourceType,
                        ResourceId = a.ResourceId,
                        Status = a.Status,
                        Details = a.Details
                    })
                    .ToListAsync();

                return logs;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR] Failed to retrieve unauthorized access attempts: {ex.Message}");
                return Array.Empty<AuditLogEntry>();
            }
        }
    }
}
