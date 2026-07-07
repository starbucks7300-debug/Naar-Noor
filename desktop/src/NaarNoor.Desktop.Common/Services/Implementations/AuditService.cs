using System.Diagnostics;
using System.Text.Json;

namespace NaarNoor.Desktop.Common.Services
{
    /// <summary>
    /// Logs security events to a local append-only JSONL file.
    /// No local database — all business data lives in the centralized API server.
    /// </summary>
    public class AuditService : IAuditService
    {
        private readonly string _logFilePath;
        private readonly SemaphoreSlim _writeLock = new(1, 1);

        public AuditService()
        {
            var logDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "NaarNoor", "logs");

            Directory.CreateDirectory(logDirectory);
            _logFilePath = Path.Combine(logDirectory, "audit.jsonl");
        }

        public async Task LogSecurityEventAsync(
            string userId,
            string action,
            string resourceType,
            string status,
            string? resourceId = null,
            string? details = null)
        {
            var entry = new AuditLogEntry
            {
                Id           = Environment.TickCount,
                Timestamp    = DateTime.UtcNow,
                UserId       = userId ?? "unknown",
                Action       = action,
                ResourceType = resourceType,
                ResourceId   = resourceId,
                Status       = status,
                Details      = details,
            };

            await AppendEntryAsync(entry);
            Debug.WriteLine($"[AUDIT] {action} | user={userId} | resource={resourceType} | status={status}");
        }

        public Task LogUnauthorizedAccessAsync(string userId, string feature, string? details = null)
            => LogSecurityEventAsync(userId, "unauthorized_access", "Feature", "failure", feature,
                $"Unauthorized access to: {feature}" + (details is null ? "" : $". {details}"));

        public Task LogLoginAsync(string userId, bool success, string? details = null)
            => LogSecurityEventAsync(userId, "login", "Authentication",
                success ? "success" : "failure", userId, details);

        public Task LogLogoutAsync(string userId)
            => LogSecurityEventAsync(userId, "logout", "Authentication", "success", userId);

        public async Task<IReadOnlyList<AuditLogEntry>> GetUserAuditLogsAsync(string userId, int days = 90)
        {
            var cutoff = DateTime.UtcNow.AddDays(-days);
            return await ReadEntriesAsync(e => e.UserId == userId && e.Timestamp >= cutoff);
        }

        public async Task<IReadOnlyList<AuditLogEntry>> GetUnauthorizedAccessAttemptsAsync(int days = 30)
        {
            var cutoff = DateTime.UtcNow.AddDays(-days);
            return await ReadEntriesAsync(e => e.Action == "unauthorized_access" && e.Timestamp >= cutoff);
        }

        private async Task AppendEntryAsync(AuditLogEntry entry)
        {
            await _writeLock.WaitAsync();
            try
            {
                var line = JsonSerializer.Serialize(entry) + Environment.NewLine;
                await File.AppendAllTextAsync(_logFilePath, line);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[AUDIT] Write failed: {ex.Message}");
            }
            finally
            {
                _writeLock.Release();
            }
        }

        private async Task<IReadOnlyList<AuditLogEntry>> ReadEntriesAsync(Func<AuditLogEntry, bool> predicate)
        {
            var results = new List<AuditLogEntry>();
            try
            {
                if (!File.Exists(_logFilePath)) return results;

                var lines = await File.ReadAllLinesAsync(_logFilePath);
                foreach (var line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    try
                    {
                        var entry = JsonSerializer.Deserialize<AuditLogEntry>(line);
                        if (entry is not null && predicate(entry))
                            results.Add(entry);
                    }
                    catch { /* skip malformed lines */ }
                }

                results.Sort((a, b) => b.Timestamp.CompareTo(a.Timestamp));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[AUDIT] Read failed: {ex.Message}");
            }
            return results;
        }
    }
}
