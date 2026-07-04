namespace NaarNoor.Desktop.Common.Services
{
    /// <summary>
    /// Static configuration for role-based access control (RBAC) mappings
    /// Defines which features and actions are available to each role
    /// </summary>
    public static class RolePermissionConfiguration
    {
        /// <summary>
        /// Map roles to accessible features
        /// </summary>
        public static readonly IReadOnlyDictionary<string, IReadOnlySet<string>> RoleFeatureMapping = new Dictionary<string, IReadOnlySet<string>>
        {
            // Manager: full access to core features
            ["Manager"] = new HashSet<string>
            {
                "MenuManagement",
                "ReservationManagement",
                "StaffManagement",
                "Reports",
                "OrderManagement",
                "Settings",
                "AuditLogs"
            },

            // Chef: limited access to menu and orders
            ["Chef"] = new HashSet<string>
            {
                "MenuManagement",
                "OrderManagement",
                "Reports"
            },

            // Staff: basic operations for reservations and orders
            ["Staff"] = new HashSet<string>
            {
                "ReservationManagement",
                "OrderManagement",
                "ViewReports"
            },

            // Admin: unrestricted access
            ["Admin"] = new HashSet<string>
            {
                "MenuManagement",
                "ReservationManagement",
                "StaffManagement",
                "Reports",
                "OrderManagement",
                "Settings",
                "AuditLogs",
                "UserManagement"
            }
        };

        /// <summary>
        /// Map actions to the roles that can perform them
        /// </summary>
        public static readonly IReadOnlyDictionary<string, IReadOnlySet<string>> ActionRoleMapping = new Dictionary<string, IReadOnlySet<string>>
        {
            // Menu actions
            ["CreateMenuItem"] = new HashSet<string> { "Manager", "Chef", "Admin" },
            ["UpdateMenuItem"] = new HashSet<string> { "Manager", "Chef", "Admin" },
            ["DeleteMenuItem"] = new HashSet<string> { "Manager", "Admin" },
            ["ViewMenu"] = new HashSet<string> { "Manager", "Chef", "Staff", "Admin" },

            // Reservation actions
            ["CreateReservation"] = new HashSet<string> { "Manager", "Staff", "Admin" },
            ["UpdateReservation"] = new HashSet<string> { "Manager", "Staff", "Admin" },
            ["DeleteReservation"] = new HashSet<string> { "Manager", "Admin" },
            ["ViewReservations"] = new HashSet<string> { "Manager", "Staff", "Admin" },

            // Order actions
            ["CreateOrder"] = new HashSet<string> { "Manager", "Staff", "Admin" },
            ["UpdateOrder"] = new HashSet<string> { "Manager", "Chef", "Staff", "Admin" },
            ["CancelOrder"] = new HashSet<string> { "Manager", "Admin" },
            ["ViewOrders"] = new HashSet<string> { "Manager", "Chef", "Staff", "Admin" },

            // Staff management
            ["ManageStaff"] = new HashSet<string> { "Manager", "Admin" },
            ["ViewStaff"] = new HashSet<string> { "Manager", "Admin" },

            // Reports
            ["ViewReports"] = new HashSet<string> { "Manager", "Chef", "Staff", "Admin" },
            ["ExportReports"] = new HashSet<string> { "Manager", "Admin" },

            // Admin actions
            ["ManageUsers"] = new HashSet<string> { "Admin" },
            ["ViewAuditLogs"] = new HashSet<string> { "Admin" },
            ["ManageSettings"] = new HashSet<string> { "Admin" }
        };
    }
}
