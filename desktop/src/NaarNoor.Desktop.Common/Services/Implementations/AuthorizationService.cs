using System.Diagnostics;

namespace NaarNoor.Desktop.Common.Services
{
    /// <summary>
    /// Role-based access control (RBAC) service implementation
    /// Enforces authorization policies based on user roles and permissions
    /// </summary>
    public class AuthorizationService : IAuthorizationService
    {
        private readonly IAuthenticationService _authService;

        public AuthorizationService(IAuthenticationService authService)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        /// <summary>
        /// Check if user can access a specific feature
        /// </summary>
        public bool CanAccessFeature(string feature)
        {
            if (string.IsNullOrWhiteSpace(feature))
            {
                return false;
            }

            var userRole = _authService.CurrentUserRole;
            if (string.IsNullOrEmpty(userRole))
            {
                return false;
            }

            if (!RolePermissionConfiguration.RoleFeatureMapping.TryGetValue(userRole, out var allowedFeatures))
            {
                return false;
            }

            return allowedFeatures.Contains(feature);
        }

        /// <summary>
        /// Check if user can perform a specific action
        /// </summary>
        public bool CanPerformAction(string action)
        {
            if (string.IsNullOrWhiteSpace(action))
            {
                return false;
            }

            var userRole = _authService.CurrentUserRole;
            if (string.IsNullOrEmpty(userRole))
            {
                return false;
            }

            if (!RolePermissionConfiguration.ActionRoleMapping.TryGetValue(action, out var allowedRoles))
            {
                // Unknown action - deny by default
                return false;
            }

            return allowedRoles.Contains(userRole);
        }

        /// <summary>
        /// Get all permissions for current user
        /// </summary>
        public IReadOnlyCollection<string> GetUserPermissions()
        {
            var userRole = _authService.CurrentUserRole;
            if (string.IsNullOrEmpty(userRole))
            {
                return Array.Empty<string>();
            }

            var permissions = new HashSet<string>();

            // Add all actions this role can perform
            foreach (var (action, roles) in RolePermissionConfiguration.ActionRoleMapping)
            {
                if (roles.Contains(userRole))
                {
                    permissions.Add(action);
                }
            }

            return permissions;
        }

        /// <summary>
        /// Get user's role
        /// </summary>
        public string? GetUserRole()
        {
            return _authService.CurrentUserRole;
        }

        /// <summary>
        /// Verify authorization and throw exception if denied
        /// </summary>
        public void VerifyAccess(string feature, string? context = null)
        {
            if (!CanAccessFeature(feature))
            {
                var userId = _authService.CurrentUserId ?? "Unknown";
                var role = _authService.CurrentUserRole ?? "Unknown";
                var message = $"User '{userId}' with role '{role}' denied access to feature '{feature}'";
                
                if (!string.IsNullOrEmpty(context))
                {
                    message += $" (Context: {context})";
                }

                LogUnauthorizedAttempt(userId, feature, context);
                throw new UnauthorizedAccessException(message);
            }
        }

        /// <summary>
        /// Log unauthorized access attempts for audit trail
        /// </summary>
        private void LogUnauthorizedAttempt(string userId, string feature, string? context)
        {
            try
            {
                var timestamp = DateTime.UtcNow.ToString("O");
                var logMessage = $"[AUDIT] Unauthorized access attempt - Time: {timestamp}, UserId: {userId}, Feature: {feature}, Context: {context ?? "N/A"}";
                Debug.WriteLine(logMessage);
                
                // TODO: In production, write to audit log service
                // _auditService.LogSecurityEvent("UnauthorizedAccess", userId, $"Feature: {feature}, Context: {context}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to log unauthorized attempt: {ex.Message}");
            }
        }
    }
}
