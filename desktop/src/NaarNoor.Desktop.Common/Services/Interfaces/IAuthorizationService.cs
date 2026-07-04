namespace NaarNoor.Desktop.Common.Services
{
    /// <summary>
    /// Role-based access control (RBAC) service for enforcing authorization policies
    /// </summary>
    public interface IAuthorizationService
    {
        /// <summary>
        /// Check if user can access a specific feature
        /// </summary>
        /// <param name="feature">Feature name (e.g., "MenuManagement", "ReservationManagement")</param>
        /// <returns>True if user has access, false otherwise</returns>
        bool CanAccessFeature(string feature);

        /// <summary>
        /// Check if user can perform a specific action
        /// </summary>
        /// <param name="action">Action name (e.g., "CreateReservation", "DeleteMenuItem")</param>
        /// <returns>True if user has permission, false otherwise</returns>
        bool CanPerformAction(string action);

        /// <summary>
        /// Get all permissions for current user
        /// </summary>
        IReadOnlyCollection<string> GetUserPermissions();

        /// <summary>
        /// Get user's role
        /// </summary>
        string? GetUserRole();

        /// <summary>
        /// Verify authorization and throw exception if denied
        /// </summary>
        /// <param name="feature">Feature to check access for</param>
        /// <param name="context">Optional context for logging</param>
        /// <exception cref="UnauthorizedAccessException">Thrown if access denied</exception>
        void VerifyAccess(string feature, string? context = null);
    }
}
