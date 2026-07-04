using Microsoft.EntityFrameworkCore;
using NaarNoor.Desktop.Common.Data.Models;

namespace NaarNoor.Desktop.Common.Data
{
    /// <summary>
    /// Entity Framework Core DbContext for local SQLite database
    /// Manages cache entries, pending operations, and audit logs
    /// </summary>
    public class DatabaseContext : DbContext
    {
        /// <summary>
        /// DbSet for cached entries (L2/L3 caching layer)
        /// </summary>
        public DbSet<CacheEntry> CacheEntries { get; set; }

        /// <summary>
        /// DbSet for pending operations (offline operation queue)
        /// </summary>
        public DbSet<PendingOperation> PendingOperations { get; set; }

        /// <summary>
        /// DbSet for audit logs (security event tracking)
        /// </summary>
        public DbSet<AuditLog> AuditLogs { get; set; }

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure CacheEntry entity
            modelBuilder.Entity<CacheEntry>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                // Column configuration
                entity.Property(e => e.Key)
                    .IsRequired()
                    .HasMaxLength(500);
                
                entity.Property(e => e.Value)
                    .IsRequired();
                
                entity.Property(e => e.CreatedAt)
                    .IsRequired();
                
                entity.Property(e => e.UpdatedAt);
                entity.Property(e => e.ExpiresAt);
                
                // Indexes for frequently queried columns
                entity.HasIndex(e => e.Key)
                    .IsUnique()
                    .HasDatabaseName("IX_CacheEntries_Key");
                
                entity.HasIndex(e => e.ExpiresAt)
                    .HasDatabaseName("IX_CacheEntries_ExpiresAt");
                
                entity.HasIndex(e => e.CreatedAt)
                    .HasDatabaseName("IX_CacheEntries_CreatedAt");
            });

            // Configure PendingOperation entity
            modelBuilder.Entity<PendingOperation>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                // Column configuration
                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(256);
                
                entity.Property(e => e.OperationType)
                    .IsRequired()
                    .HasMaxLength(50);
                
                entity.Property(e => e.ResourceType)
                    .IsRequired()
                    .HasMaxLength(100);
                
                entity.Property(e => e.Payload)
                    .IsRequired();
                
                entity.Property(e => e.CreatedAt)
                    .IsRequired();
                
                entity.Property(e => e.SyncedAt);
                
                entity.Property(e => e.ErrorMessage)
                    .HasMaxLength(1000);
                
                // Indexes for frequently queried columns and filtering
                entity.HasIndex(e => e.UserId)
                    .HasDatabaseName("IX_PendingOperations_UserId");
                
                entity.HasIndex(e => e.CreatedAt)
                    .HasDatabaseName("IX_PendingOperations_CreatedAt");
                
                entity.HasIndex(e => e.SyncedAt)
                    .HasDatabaseName("IX_PendingOperations_SyncedAt");
                
                entity.HasIndex(e => new { e.UserId, e.SyncedAt })
                    .HasDatabaseName("IX_PendingOperations_UserId_SyncedAt");
            });

            // Configure AuditLog entity
            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                // Column configuration
                entity.Property(e => e.Timestamp)
                    .IsRequired();
                
                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(256);
                
                entity.Property(e => e.Action)
                    .IsRequired()
                    .HasMaxLength(100);
                
                entity.Property(e => e.ResourceType)
                    .IsRequired()
                    .HasMaxLength(100);
                
                entity.Property(e => e.ResourceId)
                    .HasMaxLength(500);
                
                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(50);
                
                entity.Property(e => e.Details)
                    .HasMaxLength(2000);
                
                // Indexes for audit trail queries
                entity.HasIndex(e => e.Timestamp)
                    .HasDatabaseName("IX_AuditLogs_Timestamp");
                
                entity.HasIndex(e => e.UserId)
                    .HasDatabaseName("IX_AuditLogs_UserId");
                
                entity.HasIndex(e => new { e.UserId, e.Timestamp })
                    .HasDatabaseName("IX_AuditLogs_UserId_Timestamp");
                
                entity.HasIndex(e => e.Action)
                    .HasDatabaseName("IX_AuditLogs_Action");
            });
        }
    }
}
