using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using NaarNoor.Desktop.Common.DTOs;
using NaarNoor.Desktop.Common.Services.ApiClients;
using NaarNoor.Desktop.Common.Utilities;

namespace NaarNoor.Desktop.Common.Services
{
    /// <summary>
    /// Service for managing reservations with caching and observable updates.
    /// Implements CRUD operations with automatic cache invalidation and retry logic.
    /// </summary>
    public class ReservationService : IReservationService
    {
        private readonly IReservationApiClient _apiClient;
        private readonly ICacheService _cacheService;
        private readonly Subject<ReservationNotification> _updates;

        private const string CacheKeyPrefix = "reservations:";
        private const string AllReservationsCacheKey = "reservations:all";
        private static readonly TimeSpan CacheTtl = TimeSpan.FromSeconds(30);

        public IObservable<ReservationNotification> ReservationUpdates => _updates.AsObservable();

        public ReservationService(
            IReservationApiClient apiClient,
            ICacheService cacheService)
        {
            _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
            _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
            _updates = new Subject<ReservationNotification>();
        }

        public async Task<Result<List<ReservationDto>>> GetReservationsAsync(
            DateTime? fromDate = null,
            DateTime? toDate = null)
        {
            try
            {
                var cacheKey = $"{AllReservationsCacheKey}:{fromDate:O}:{toDate:O}";
                
                // Try to get from cache first
                var cached = await _cacheService.GetAsync<List<ReservationDto>>(cacheKey);
                if (cached != null)
                {
                    return Result<List<ReservationDto>>.Success(cached);
                }

                // Fetch from API - using pagination with all items (page 1, large pageSize)
                var response = await _apiClient.GetReservationsAsync(
                    page: 1,
                    pageSize: 1000,
                    fromDate: fromDate,
                    toDate: toDate);

                var reservations = response.Data ?? new List<ReservationDto>();

                // Cache the result
                await _cacheService.SetAsync(cacheKey, reservations, CacheTtl);

                return Result<List<ReservationDto>>.Success(reservations);
            }
            catch (Exception ex)
            {
                return Result<List<ReservationDto>>.Failure($"Failed to get reservations: {ex.Message}");
            }
        }

        public async Task<Result<ReservationDto>> GetReservationByIdAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return Result<ReservationDto>.Failure("Reservation ID is required");
            }

            try
            {
                var cacheKey = $"{CacheKeyPrefix}{id}";

                // Try to get from cache first
                var cached = await _cacheService.GetAsync<ReservationDto>(cacheKey);
                if (cached != null)
                {
                    return Result<ReservationDto>.Success(cached);
                }

                // Fetch from API
                var reservation = await _apiClient.GetReservationByIdAsync(id);

                // Cache the result
                await _cacheService.SetAsync(cacheKey, reservation, CacheTtl);

                return Result<ReservationDto>.Success(reservation);
            }
            catch (Exception ex)
            {
                return Result<ReservationDto>.Failure($"Failed to get reservation: {ex.Message}");
            }
        }

        public async Task<Result<ReservationDto>> CreateReservationAsync(CreateReservationRequest request)
        {
            if (request == null)
            {
                return Result<ReservationDto>.Failure("Reservation request is required");
            }

            try
            {
                // Create reservation via API
                var result = await _apiClient.CreateReservationAsync(request);

                // Invalidate cache
                _cacheService.InvalidatePattern(AllReservationsCacheKey);

                // Publish notification
                _updates.OnNext(new ReservationNotification
                {
                    ReservationId = result.Id,
                    EventType = "created",
                    Data = result,
                    Timestamp = DateTime.UtcNow
                });

                return Result<ReservationDto>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<ReservationDto>.Failure($"Failed to create reservation: {ex.Message}");
            }
        }

        public async Task<Result<ReservationDto>> UpdateReservationAsync(
            string id,
            UpdateReservationRequest request)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return Result<ReservationDto>.Failure("Reservation ID is required");
            }

            if (request == null)
            {
                return Result<ReservationDto>.Failure("Update request is required");
            }

            try
            {
                // Update reservation via API
                var result = await _apiClient.UpdateReservationAsync(id, request);

                // Invalidate specific and list caches
                _cacheService.InvalidatePattern(AllReservationsCacheKey);
                await _cacheService.RemoveAsync($"{CacheKeyPrefix}{id}");

                // Publish notification
                _updates.OnNext(new ReservationNotification
                {
                    ReservationId = id,
                    EventType = "updated",
                    Data = result,
                    Timestamp = DateTime.UtcNow
                });

                return Result<ReservationDto>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<ReservationDto>.Failure($"Failed to update reservation: {ex.Message}");
            }
        }

        public async Task<Result> DeleteReservationAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return Result.Failure("Reservation ID is required");
            }

            try
            {
                // Delete reservation via API
                var response = await _apiClient.DeleteReservationAsync(id);

                if (!response.IsSuccessStatusCode)
                {
                    return Result.Failure($"Failed to delete reservation: HTTP {response.StatusCode}");
                }

                // Invalidate specific and list caches
                _cacheService.InvalidatePattern(AllReservationsCacheKey);
                await _cacheService.RemoveAsync($"{CacheKeyPrefix}{id}");

                // Publish notification
                _updates.OnNext(new ReservationNotification
                {
                    ReservationId = id,
                    EventType = "deleted",
                    Data = null,
                    Timestamp = DateTime.UtcNow
                });

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure($"Failed to delete reservation: {ex.Message}");
            }
        }
    }
}
