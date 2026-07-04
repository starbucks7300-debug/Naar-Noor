using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NaarNoor.Desktop.Common.DTOs;
using NaarNoor.Desktop.Common.Services;

namespace NaarNoor.Desktop.WinForms.ViewModels
{
    /// <summary>
    /// ViewModel for reservation management.
    /// Handles CRUD operations, filtering by date, and pagination.
    /// </summary>
    public partial class ReservationViewModel : ViewModelBase
    {
        private readonly IReservationService _reservationService;
        private const int PageSize = 50;

        /// <summary>
        /// Currently selected reservation from list
        /// </summary>
        [ObservableProperty]
        private ReservationDto? selectedReservation;

        /// <summary>
        /// Observable collection of reservations for data binding
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<ReservationDto> reservations = new();

        /// <summary>
        /// Customer name search filter
        /// </summary>
        [ObservableProperty]
        private string searchCustomerName = string.Empty;

        /// <summary>
        /// Reservation status filter (all, confirmed, pending, etc.)
        /// </summary>
        [ObservableProperty]
        private string statusFilter = "all";

        /// <summary>
        /// Current page for pagination
        /// </summary>
        [ObservableProperty]
        private int currentPage = 1;

        /// <summary>
        /// Total number of reservations matching filter
        /// </summary>
        [ObservableProperty]
        private int totalCount;

        /// <summary>
        /// Show/hide form for creating new reservation
        /// </summary>
        [ObservableProperty]
        private bool showNewReservationForm;

        /// <summary>
        /// Form inputs for new reservation
        /// </summary>
        [ObservableProperty]
        private string formCustomerName = string.Empty;

        [ObservableProperty]
        private DateTime formBookingTime = DateTime.Now.AddDays(1);

        [ObservableProperty]
        private int formPartySize = 2;

        [ObservableProperty]
        private string? formCustomerEmail;

        [ObservableProperty]
        private string? formCustomerPhone;

        [ObservableProperty]
        private string? formSpecialRequests;

        public ReservationViewModel(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _reservationService = GetService<IReservationService>();
        }

        /// <summary>
        /// Initialize the reservation view - load reservations on startup.
        /// </summary>
        [RelayCommand]
        public async Task Initialize()
        {
            await LoadReservationsCommand.ExecuteAsync(null);
        }

        /// <summary>
        /// Load reservations with current filters.
        /// </summary>
        [RelayCommand]
        public async Task LoadReservations()
        {
            var result = await ExecuteAsync(
                async () => await _reservationService.GetReservationsAsync(),
                onSuccess: (allReservations) =>
                {
                    // Filter by customer name and status
                    var filtered = allReservations
                        .Where(r => string.IsNullOrEmpty(SearchCustomerName) || 
                                   r.CustomerName.Contains(SearchCustomerName, StringComparison.OrdinalIgnoreCase))
                        .Where(r => StatusFilter == "all" || r.Status == StatusFilter)
                        .ToList();

                    TotalCount = filtered.Count;

                    // Apply pagination
                    var paged = filtered
                        .Skip((CurrentPage - 1) * PageSize)
                        .Take(PageSize)
                        .ToList();

                    // Update collection
                    Reservations.Clear();
                    foreach (var reservation in paged)
                    {
                        Reservations.Add(reservation);
                    }
                }
            );
        }

        /// <summary>
        /// Create a new reservation from form inputs.
        /// </summary>
        [RelayCommand]
        public async Task CreateReservation()
        {
            if (!ValidateReservationForm())
            {
                return;
            }

            var request = new CreateReservationRequest
            {
                CustomerName = FormCustomerName,
                BookingTime = FormBookingTime,
                PartySize = FormPartySize,
                CustomerEmail = FormCustomerEmail,
                CustomerPhone = FormCustomerPhone,
                SpecialRequests = FormSpecialRequests
            };

            var result = await ExecuteAsync(
                async () => await _reservationService.CreateReservationAsync(request),
                onSuccess: (_) =>
                {
                    // Clear form and reload
                    ClearReservationForm();
                    ShowNewReservationForm = false;
                }
            );

            if (result.IsSuccess)
            {
                await LoadReservationsCommand.ExecuteAsync(null);
            }
        }

        /// <summary>
        /// Update the selected reservation.
        /// </summary>
        [RelayCommand]
        public async Task UpdateReservation()
        {
            if (SelectedReservation == null)
            {
                SetError("No reservation selected");
                return;
            }

            var request = new UpdateReservationRequest
            {
                CustomerName = SelectedReservation.CustomerName,
                BookingTime = SelectedReservation.BookingTime,
                PartySize = SelectedReservation.PartySize,
                Status = SelectedReservation.Status,
                TableNumber = SelectedReservation.TableNumber,
                CustomerEmail = SelectedReservation.CustomerEmail,
                CustomerPhone = SelectedReservation.CustomerPhone,
                SpecialRequests = SelectedReservation.SpecialRequests
            };

            var result = await ExecuteAsync(
                async () => await _reservationService.UpdateReservationAsync(SelectedReservation.Id, request)
            );

            if (result.IsSuccess)
            {
                await LoadReservationsCommand.ExecuteAsync(null);
            }
        }

        /// <summary>
        /// Delete the selected reservation with confirmation.
        /// </summary>
        [RelayCommand]
        public async Task DeleteReservation()
        {
            if (SelectedReservation == null)
            {
                SetError("No reservation selected");
                return;
            }

            // In real app, show confirmation dialog here
            // For now, proceed directly
            var result = await ExecuteAsync(
                async () => await _reservationService.DeleteReservationAsync(SelectedReservation.Id)
            );

            if (result.IsSuccess)
            {
                SelectedReservation = null;
                await LoadReservationsCommand.ExecuteAsync(null);
            }
        }

        /// <summary>
        /// Toggle visibility of new reservation form.
        /// </summary>
        [RelayCommand]
        public void ToggleNewReservationForm()
        {
            ShowNewReservationForm = !ShowNewReservationForm;
            if (!ShowNewReservationForm)
            {
                ClearReservationForm();
            }
        }

        /// <summary>
        /// Validate reservation form inputs.
        /// </summary>
        private bool ValidateReservationForm()
        {
            if (string.IsNullOrWhiteSpace(FormCustomerName))
            {
                SetError("Customer name is required");
                return false;
            }

            if (FormBookingTime <= DateTime.Now)
            {
                SetError("Booking time must be in the future");
                return false;
            }

            if (FormPartySize <= 0)
            {
                SetError("Party size must be at least 1");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Clear reservation form inputs.
        /// </summary>
        private void ClearReservationForm()
        {
            FormCustomerName = string.Empty;
            FormBookingTime = DateTime.Now.AddDays(1);
            FormPartySize = 2;
            FormCustomerEmail = null;
            FormCustomerPhone = null;
            FormSpecialRequests = null;
            ClearError();
        }

        /// <summary>
        /// Handle search filter change - reload reservations.
        /// </summary>
        partial void OnSearchCustomerNameChanged(string value)
        {
            CurrentPage = 1;
            LoadReservationsCommand.ExecuteAsync(null);
        }

        /// <summary>
        /// Handle status filter change - reload reservations.
        /// </summary>
        partial void OnStatusFilterChanged(string value)
        {
            CurrentPage = 1;
            LoadReservationsCommand.ExecuteAsync(null);
        }
    }
}
