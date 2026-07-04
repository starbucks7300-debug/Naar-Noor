using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NaarNoor.Desktop.Common.DTOs;
using NaarNoor.Desktop.Common.Services;

namespace NaarNoor.Desktop.WinForms.ViewModels
{
    /// <summary>
    /// ViewModel for staff management.
    /// Handles staff status updates and role-based filtering.
    /// </summary>
    public partial class StaffViewModel : ViewModelBase
    {
        private readonly IChefService _chefService;
        private CancellationTokenSource? _autoRefreshCancellation;
        private Task? _autoRefreshTask;

        /// <summary>
        /// Observable collection of staff members
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<StaffDto> staff = new();

        /// <summary>
        /// Currently selected staff member
        /// </summary>
        [ObservableProperty]
        private StaffDto? selectedStaff;

        /// <summary>
        /// Search filter by staff name
        /// </summary>
        [ObservableProperty]
        private string searchName = string.Empty;

        /// <summary>
        /// Role filter (all, Chef, Waiter, Manager)
        /// </summary>
        [ObservableProperty]
        private string roleFilter = "all";

        /// <summary>
        /// New status for the selected staff member
        /// </summary>
        [ObservableProperty]
        private string statusUpdate = "available";

        /// <summary>
        /// Available status options for dropdown
        /// </summary>
        public ObservableCollection<string> StatusOptions { get; } = new()
        {
            "available",
            "busy",
            "break"
        };

        /// <summary>
        /// Available role options for filtering
        /// </summary>
        public ObservableCollection<string> RoleOptions { get; } = new()
        {
            "all",
            "Chef",
            "Waiter",
            "Manager",
            "Admin"
        };

        public StaffViewModel(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _chefService = GetService<IChefService>();
        }

        /// <summary>
        /// Initialize staff view - load staff and start auto-refresh.
        /// </summary>
        [RelayCommand]
        public async Task Initialize()
        {
            await LoadStaffCommand.ExecuteAsync(null);
            StartAutoRefresh();
        }

        /// <summary>
        /// Load all staff members and apply current filters.
        /// </summary>
        [RelayCommand]
        public async Task LoadStaff()
        {
            var result = await ExecuteAsync(
                async () => await _chefService.GetStaffAsync(),
                onSuccess: (allStaff) =>
                {
                    // Filter by role and search name
                    var filtered = allStaff
                        .Where(s => RoleFilter == "all" || s.Role == RoleFilter)
                        .Where(s => string.IsNullOrEmpty(SearchName) || 
                                   s.Name.Contains(SearchName, StringComparison.OrdinalIgnoreCase))
                        .OrderBy(s => s.Name)
                        .ToList();

                    // Update collection
                    Staff.Clear();
                    foreach (var member in filtered)
                    {
                        Staff.Add(member);
                    }

                    SelectedStaff = null;
                    StatusUpdate = "available";
                }
            );
        }

        /// <summary>
        /// Update the selected staff member's status.
        /// </summary>
        [RelayCommand]
        public async Task UpdateStaffStatus()
        {
            if (SelectedStaff == null)
            {
                SetError("No staff member selected");
                return;
            }

            var request = new UpdateStaffStatusRequest
            {
                Status = StatusUpdate
            };

            var result = await ExecuteAsync(
                async () => await _chefService.UpdateStaffStatusAsync(SelectedStaff.Id, request),
                onSuccess: (_) =>
                {
                    // Update the selected staff in the list
                    if (SelectedStaff != null)
                    {
                        SelectedStaff.Status = StatusUpdate;
                    }
                }
            );

            if (result.IsSuccess)
            {
                await LoadStaffCommand.ExecuteAsync(null);
            }
        }

        /// <summary>
        /// Get color for status badge (Green=available, Yellow=busy, Red=break).
        /// </summary>
        public string GetStatusColor(string status)
        {
            return status switch
            {
                "available" => "Green",
                "busy" => "Yellow",
                "break" => "Red",
                _ => "Gray"
            };
        }

        /// <summary>
        /// Start auto-refresh of staff list every 2 minutes.
        /// </summary>
        private void StartAutoRefresh()
        {
            StopAutoRefresh();

            _autoRefreshCancellation = new CancellationTokenSource();
            _autoRefreshTask = Task.Run(async () =>
            {
                while (!_autoRefreshCancellation.Token.IsCancellationRequested)
                {
                    try
                    {
                        await Task.Delay(TimeSpan.FromMinutes(2), _autoRefreshCancellation.Token);

                        if (!_autoRefreshCancellation.Token.IsCancellationRequested)
                        {
                            await LoadStaffCommand.ExecuteAsync(null);
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Staff auto-refresh error: {ex.Message}");
                    }
                }
            }, _autoRefreshCancellation.Token);
        }

        /// <summary>
        /// Stop auto-refresh of staff list.
        /// </summary>
        public void StopAutoRefresh()
        {
            _autoRefreshCancellation?.Cancel();
            _autoRefreshTask?.Wait(TimeSpan.FromSeconds(1));
            _autoRefreshCancellation?.Dispose();
            _autoRefreshCancellation = null;
            _autoRefreshTask = null;
        }

        /// <summary>
        /// Handle role filter change - reload staff.
        /// </summary>
        partial void OnRoleFilterChanged(string value)
        {
            LoadStaffCommand.ExecuteAsync(null);
        }

        /// <summary>
        /// Handle search filter change - reload staff.
        /// </summary>
        partial void OnSearchNameChanged(string value)
        {
            LoadStaffCommand.ExecuteAsync(null);
        }
    }
}
