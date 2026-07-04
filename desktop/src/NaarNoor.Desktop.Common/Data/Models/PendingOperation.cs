namespace NaarNoor.Desktop.Common.Data.Models
{
    /// <summary>
    /// Represents a pending operation in the offline operations queue
    /// Used for FIFO queuing of operations when working offline
    /// </summary>
    public class PendingOperation
    {
        /// <summary>
        /// Primary key for the pending operation
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// User ID who initiated this operation
        /// </summary>
        public required string UserId { get; set; }

        /// <summary>
        /// Type of operation: CREATE, UPDATE, DELETE
        /// </summary>
        public required string OperationType { get; set; }

        /// <summary>
        /// Resource type being operated on (e.g., "reservation", "menu_item", "staff")
        /// </summary>
        public required string ResourceType { get; set; }

        /// <summary>
        /// JSON payload containing the operation data
        /// </summary>
        public required string Payload { get; set; }

        /// <summary>
        /// When this operation was created
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// When this operation was successfully synced to the server (null if not synced)
        /// </summary>
        public DateTime? SyncedAt { get; set; }

        /// <summary>
        /// Error message if sync failed (null if no error)
        /// </summary>
        public string? ErrorMessage { get; set; }
    }
}
