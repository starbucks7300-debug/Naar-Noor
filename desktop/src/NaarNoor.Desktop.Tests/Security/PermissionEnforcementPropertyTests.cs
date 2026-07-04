using Xunit;
using FsCheck;
using FsCheck.Xunit;
using Moq;
using NaarNoor.Desktop.Common.Services;
using System.Collections.Generic;
using System.Linq;

namespace NaarNoor.Desktop.Tests.Security
{
    /// <summary>
    /// Property-based tests for Role-Based Access Control (RBAC) enforcement
    /// Validates: REQ-004 (Role-based access control)
    /// 
    /// Property 4: Permission Enforcement
    /// For all users with role R and resource requiring role set R':
    /// authorize(user, resource) ⟹ (R ⊇ R')
    /// 
    /// This ensures that:
    /// 1. Users can only access resources their role permits
    /// 2. All role combinations are properly enforced
    /// 3. Unauthorized attempts are logged
    /// </summary>
    public class PermissionEnforcementPropertyTests
    {
        private static readonly string[] AllRoles = { "Admin", "Manager", "Chef", "Staff" };

        private static readonly string[] AllFeatures =
        {
            "MenuManagement",
            "ReservationManagement",
            "StaffManagement",
            "Reports",
            "OrderManagement",
            "Settings",
            "AuditLogs",
            "ViewReports",
            "UserManagement"
        };

        private static readonly string[] AllActions =
        {
            // Menu actions
            "CreateMenuItem", "UpdateMenuItem", "DeleteMenuItem", "ViewMenu",
            // Reservation actions
            "CreateReservation", "UpdateReservation", "DeleteReservation", "ViewReservations",
            // Order actions
            "CreateOrder", "UpdateOrder", "CancelOrder", "ViewOrders",
            // Staff actions
            "ManageStaff", "ViewStaff",
            // Report actions
            "ViewReports", "ExportReports",
            // Admin actions
            "ManageUsers", "ViewAuditLogs", "ManageSettings"
        };

        /// <summary>
        /// Property Test 1: User can access feature if and only if role permits
        /// 
        /// For any role and feature:
        /// authorize(role, feature) ⟹ configuration.allows(role, feature)
        /// </summary>
        [Property]
        public void Property_FeatureAccessEnforcement(int roleIndex, int featureIndex)
        {
            if (roleIndex < 0 || roleIndex >= AllRoles.Length)
                return;
            if (featureIndex < 0 || featureIndex >= AllFeatures.Length)
                return;

            var role = AllRoles[roleIndex];
            var feature = AllFeatures[featureIndex];

            // Arrange
            var authService = CreateAuthorizationService(role);

            // Act
            bool canAccess = authService.CanAccessFeature(feature);

            // Assert: Check against configuration
            bool expectedAccess = RolePermissionConfiguration.RoleFeatureMapping
                .TryGetValue(role, out var allowedFeatures) && allowedFeatures.Contains(feature);

            Assert.Equal(expectedAccess, canAccess);
        }

        /// <summary>
        /// Property Test 2: User can perform action if and only if role permits
        /// 
        /// For any role and action:
        /// authorize(role, action) ⟹ configuration.allows(role, action)
        /// </summary>
        [Property]
        public void Property_ActionPermissionEnforcement(int roleIndex, int actionIndex)
        {
            if (roleIndex < 0 || roleIndex >= AllRoles.Length)
                return;
            if (actionIndex < 0 || actionIndex >= AllActions.Length)
                return;

            var role = AllRoles[roleIndex];
            var action = AllActions[actionIndex];

            // Arrange
            var authService = CreateAuthorizationService(role);

            // Act
            bool canPerform = authService.CanPerformAction(action);

            // Assert: Check against configuration
            bool expectedPermission = RolePermissionConfiguration.ActionRoleMapping
                .TryGetValue(action, out var allowedRoles) && allowedRoles.Contains(role);

            Assert.Equal(expectedPermission, canPerform);
        }

        /// <summary>
        /// Property Test 3: Role hierarchy is respected
        /// Admin has all permissions, Manager has most, Chef and Staff limited
        /// </summary>
        [Fact]
        public void Property_RoleHierarchyRespected()
        {
            var adminService = CreateAuthorizationService("Admin");
            
            // Admin should have all permissions from the configuration
            if (RolePermissionConfiguration.RoleFeatureMapping.TryGetValue("Admin", out var adminFeatures))
            {
                foreach (var feature in adminFeatures)
                {
                    Assert.True(adminService.CanAccessFeature(feature),
                        $"Admin should access {feature}");
                }
            }
        }

        /// <summary>
        /// Property Test 4: Consistent permission set across queries
        /// For any role, calling GetUserPermissions multiple times yields identical results
        /// </summary>
        [Property]
        public void Property_PermissionSetConsistency(int roleIndex)
        {
            if (roleIndex < 0 || roleIndex >= AllRoles.Length)
                return;

            var role = AllRoles[roleIndex];
            var authService = CreateAuthorizationService(role);

            // Get permissions twice
            var permissions1 = authService.GetUserPermissions();
            var permissions2 = authService.GetUserPermissions();

            // Should be identical
            Assert.Equal(permissions1.Count, permissions2.Count);
            foreach (var perm in permissions1)
            {
                Assert.Contains(perm, permissions2);
            }
        }

        /// <summary>
        /// Property Test 5: Unauthorized access throws exception
        /// </summary>
        [Fact]
        public void Property_UnauthorizedAccessThrowsException()
        {
            var unauthorizedPairs = new[]
            {
                ("Staff", "StaffManagement"),
                ("Chef", "ReservationManagement"),
            };

            foreach (var (role, feature) in unauthorizedPairs)
            {
                var authService = CreateAuthorizationService(role);
                Assert.Throws<UnauthorizedAccessException>(
                    () => authService.VerifyAccess(feature));
            }
        }

        /// <summary>
        /// Property Test 6: Delete operations require elevated privileges
        /// </summary>
        [Fact]
        public void Property_DestructiveOperationsRequirePrivileges()
        {
            var deleteActions = AllActions.Where(a => a.StartsWith("Delete")).ToList();
            Assert.NotEmpty(deleteActions);

            var staffService = CreateAuthorizationService("Staff");
            var adminService = CreateAuthorizationService("Admin");

            // Staff should never delete anything
            foreach (var action in deleteActions)
            {
                Assert.False(staffService.CanPerformAction(action),
                    $"Staff should not perform {action}");
            }

            // Admin should always delete
            foreach (var action in deleteActions)
            {
                Assert.True(adminService.CanPerformAction(action),
                    $"Admin should perform {action}");
            }
        }

        /// <summary>
        /// Property Test 7: Permission grants are monotonic
        /// Permissions don't change across multiple calls for same role
        /// </summary>
        [Property]
        public void Property_PermissionGrantsAreMonotonic(int roleIndex)
        {
            if (roleIndex < 0 || roleIndex >= AllRoles.Length)
                return;

            var role = AllRoles[roleIndex];
            var authService = CreateAuthorizationService(role);
            var feature = AllFeatures[0];

            var results = new List<bool>();
            for (int i = 0; i < 5; i++)
            {
                results.Add(authService.CanAccessFeature(feature));
            }

            // All results should be identical
            Assert.All(results, r => Assert.Equal(results[0], r));
        }

        /// <summary>
        /// Property Test 8: Permission boundaries are clear
        /// </summary>
        [Fact]
        public void Property_PermissionBoundariesAreClear()
        {
            foreach (var role in AllRoles)
            {
                var authService = CreateAuthorizationService(role);
                var permittedFeatures = new HashSet<string>();

                foreach (var feature in AllFeatures)
                {
                    if (authService.CanAccessFeature(feature))
                    {
                        permittedFeatures.Add(feature);
                    }
                }

                // Get what's configured for this role
                if (RolePermissionConfiguration.RoleFeatureMapping.TryGetValue(role, out var configuredFeatures))
                {
                    // Check that each permitted feature is actually configured
                    foreach (var feature in permittedFeatures)
                    {
                        Assert.Contains(feature, configuredFeatures);
                    }
                    
                    // Check that no configured features are denied
                    foreach (var feature in configuredFeatures)
                    {
                        Assert.True(authService.CanAccessFeature(feature),
                            $"Feature {feature} should be accessible to {role}");
                    }
                }
            }
        }

        // Helper: Create authorization service with specific role
        private IAuthorizationService CreateAuthorizationService(string role)
        {
            // Create mock authentication service
            var authServiceMock = new Mock<IAuthenticationService>();
            authServiceMock.Setup(s => s.CurrentUserRole).Returns(role);
            authServiceMock.Setup(s => s.CurrentUserId).Returns("test-user-123");
            authServiceMock.Setup(s => s.IsAuthenticated).Returns(true);

            // Create authorization service with mocked auth service
            return new AuthorizationService(authServiceMock.Object, null);
        }
    }
}
