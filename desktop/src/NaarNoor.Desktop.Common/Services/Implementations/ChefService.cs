using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using NaarNoor.Desktop.Common.DTOs;
using NaarNoor.Desktop.Common.Services.ApiClients;
using NaarNoor.Desktop.Common.Utilities;

namespace NaarNoor.Desktop.Common.Services
{
    /// <summary>
    /// Service for managing staff and chef operations with observable updates.
    /// Implements staff management with automatic cache invalidation.
    /// </summary>
    public class ChefService : IChefService
    {
        private readonly IChefApiClient _apiClient;
        private readonly ICacheService _cacheService;
        private readonly Subject<StaffStatusNotification> _statusUpdates;

        private const string AllStaffCacheKey = "staff:all";
        private const string StaffByRolePrefix = "staff:role:";
        private const string AvailableStaffCacheKey = "staff:available";
        private static readonly TimeSpan StaffCacheTtl = TimeSpan.FromMinutes(15);
        private static readonly TimeSpan AvailabilityCacheTtl = TimeSpan.FromMinutes(5);

        public IObservable<StaffStatusNotification> StaffStatusUpdates => _statusUpdates.AsObservable();

        public ChefService(
            IChefApiClient apiClient,
            ICacheService cacheService)
        {
            _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
            _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
            _statusUpdates = new Subject<StaffStatusNotification>();
        }

        public async Task<Result<List<StaffDto>>> GetStaffAsync()
        {
            try
            {
                // Try to get from cache first
                var cached = await _cacheService.GetAsync<List<StaffDto>>(AllStaffCacheKey);
                if (cached != null)
                {
                    return Result<List<StaffDto>>.Success(cached);
                }

                // Fetch from API
                var staff = await _apiClient.GetStaffAsync();

                // Cache the result
                await _cacheService.SetAsync(AllStaffCacheKey, staff, StaffCacheTtl);

                return Result<List<StaffDto>>.Success(staff);
            }
            catch (Exception ex)
            {
                return Result<List<StaffDto>>.Failure($"Failed to get staff: {ex.Message}");
            }
        }

        public async Task<Result<List<StaffDto>>> GetStaffByRoleAsync(string role)
        {
            if (string.IsNullOrWhiteSpace(role))
            {
                return Result<List<StaffDto>>.Failure("Role is required");
            }

            try
            {
                var cacheKey = $"{StaffByRolePrefix}{role}";

                // Try to get from cache first
                var cached = await _cacheService.GetAsync<List<StaffDto>>(cacheKey);
                if (cached != null)
                {
                    return Result<List<StaffDto>>.Success(cached);
                }

                // Get all staff and filter by role
                var staffResult = await GetStaffAsync();
                if (!staffResult.IsSuccess)
                {
                    return Result<List<StaffDto>>.Failure(staffResult.Error ?? "Failed to get staff");
                }

                var filtered = (staffResult.Value ?? new List<StaffDto>())
                    .Where(s => s.Role.Equals(role, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                // Cache the result
                await _cacheService.SetAsync(cacheKey, filtered, StaffCacheTtl);

                return Result<List<StaffDto>>.Success(filtered);
            }
            catch (Exception ex)
            {
                return Result<List<StaffDto>>.Failure($"Failed to get staff by role: {ex.Message}");
            }
        }

        public async Task<Result<List<StaffDto>>> GetAvailableStaffAsync()
        {
            try
            {
                // Try to get from cache first (shorter TTL for availability)
                var cached = await _cacheService.GetAsync<List<StaffDto>>(AvailableStaffCacheKey);
                if (cached != null)
                {
                    return Result<List<StaffDto>>.Success(cached);
                }

                // Get all staff and filter by availability
                var staffResult = await GetStaffAsync();
                if (!staffResult.IsSuccess)
                {
                    return Result<List<StaffDto>>.Failure(staffResult.Error ?? "Failed to get staff");
                }

                var available = (staffResult.Value ?? new List<StaffDto>())
                    .Where(s => s.Status.Equals("available", StringComparison.OrdinalIgnoreCase))
                    .ToList();

                // Cache the result with shorter TTL
                await _cacheService.SetAsync(AvailableStaffCacheKey, available, AvailabilityCacheTtl);

                return Result<List<StaffDto>>.Success(available);
            }
            catch (Exception ex)
            {
                return Result<List<StaffDto>>.Failure($"Failed to get available staff: {ex.Message}");
            }
        }

        public async Task<Result<StaffDto>> UpdateStaffStatusAsync(
            string staffId,
            UpdateStaffStatusRequest request)
        {
            if (string.IsNullOrWhiteSpace(staffId))
            {
                return Result<StaffDto>.Failure("Staff ID is required");
            }

            if (request == null)
            {
                return Result<StaffDto>.Failure("Update request is required");
            }

            try
            {
                // Update status via API
                var result = await _apiClient.UpdateStaffStatusAsync(staffId, request);

                // Invalidate caches
                _cacheService.InvalidatePattern(AllStaffCacheKey);
                _cacheService.InvalidatePattern(StaffByRolePrefix);
                _cacheService.InvalidatePattern(AvailableStaffCacheKey);

                // Publish notification
                _statusUpdates.OnNext(new StaffStatusNotification
                {
                    StaffId = staffId,
                    Name = result.Name,
                    Status = result.Status,
                    Timestamp = DateTime.UtcNow
                });

                return Result<StaffDto>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<StaffDto>.Failure($"Failed to update staff status: {ex.Message}");
            }
        }
    }
}
