using Microsoft.EntityFrameworkCore;

namespace NaarNoor.Desktop.Common.Data
{
    /// <summary>
    /// Initializes the local SQLite database on application startup
    /// Creates tables and applies migrations
    /// </summary>
    public class DatabaseInitializer
    {
        private readonly DatabaseContext _context;

        public DatabaseInitializer(DatabaseContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Initialize database: create schema and apply migrations
        /// </summary>
        public async Task InitializeAsync()
        {
            try
            {
                // Create database if it doesn't exist and apply migrations
                await _context.Database.MigrateAsync();

                System.Diagnostics.Debug.WriteLine("[Database] Initialization complete");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Database] Error during initialization: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Reset database to initial state (development only)
        /// </summary>
        public async Task ResetAsync()
        {
            try
            {
                await _context.Database.EnsureDeletedAsync();
                await _context.Database.MigrateAsync();

                System.Diagnostics.Debug.WriteLine("[Database] Reset complete");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Database] Error during reset: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Verify database is healthy (can connect and query)
        /// </summary>
        public async Task<bool> HealthCheckAsync()
        {
            try
            {
                await _context.Database.ExecuteSqlRawAsync("SELECT 1");
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
