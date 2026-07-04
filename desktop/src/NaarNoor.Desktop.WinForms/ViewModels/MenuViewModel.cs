using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NaarNoor.Desktop.Common.DTOs;
using NaarNoor.Desktop.Common.Services;

namespace NaarNoor.Desktop.WinForms.ViewModels
{
    /// <summary>
    /// ViewModel for menu item management.
    /// Handles CRUD operations with bilingual support and category filtering.
    /// </summary>
    public partial class MenuViewModel : ViewModelBase
    {
        private readonly IMenuService _menuService;

        /// <summary>
        /// Observable collection of menu items for data binding
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<MenuItemDto> menuItems = new();

        /// <summary>
        /// Currently selected menu item
        /// </summary>
        [ObservableProperty]
        private MenuItemDto? selectedMenuItem;

        /// <summary>
        /// Category filter (all, appetizers, mains, desserts, beverages)
        /// </summary>
        [ObservableProperty]
        private string categoryFilter = "all";

        /// <summary>
        /// Search filter by name
        /// </summary>
        [ObservableProperty]
        private string searchName = string.Empty;

        /// <summary>
        /// Show/hide form for creating new menu item
        /// </summary>
        [ObservableProperty]
        private bool showNewItemForm;

        /// <summary>
        /// Form inputs for new/edit menu item
        /// </summary>
        [ObservableProperty]
        private string formNameEn = string.Empty;

        [ObservableProperty]
        private string formNameAr = string.Empty;

        [ObservableProperty]
        private string? formDescriptionEn;

        [ObservableProperty]
        private string? formDescriptionAr;

        [ObservableProperty]
        private string formCategory = "Main";

        [ObservableProperty]
        private decimal formPrice = 0m;

        [ObservableProperty]
        private bool formIsAvailable = true;

        /// <summary>
        /// Is editing existing item vs creating new
        /// </summary>
        private bool _isEditMode;

        public MenuViewModel(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _menuService = GetService<IMenuService>();
        }

        /// <summary>
        /// Initialize the menu view - load menu items on startup.
        /// </summary>
        [RelayCommand]
        public async Task Initialize()
        {
            await LoadMenuItemsCommand.ExecuteAsync(null);
        }

        /// <summary>
        /// Load all menu items and apply current filters.
        /// </summary>
        [RelayCommand]
        public async Task LoadMenuItems()
        {
            var result = await ExecuteAsync(
                async () => await _menuService.GetMenuItemsAsync(),
                onSuccess: (items) =>
                {
                    // Filter by category and search
                    var filtered = items
                        .Where(i => CategoryFilter == "all" || i.Category == CategoryFilter)
                        .Where(i => string.IsNullOrEmpty(SearchName) || 
                                   i.NameEn.Contains(SearchName, StringComparison.OrdinalIgnoreCase) ||
                                   i.NameAr.Contains(SearchName, StringComparison.OrdinalIgnoreCase))
                        .OrderBy(i => i.NameEn)
                        .ToList();

                    // Update collection
                    MenuItems.Clear();
                    foreach (var item in filtered)
                    {
                        MenuItems.Add(item);
                    }

                    SelectedMenuItem = null;
                }
            );
        }

        /// <summary>
        /// Create a new menu item from form inputs.
        /// </summary>
        [RelayCommand]
        public async Task CreateMenuItem()
        {
            if (!ValidateMenuItemForm())
            {
                return;
            }

            var request = new CreateMenuItemRequest
            {
                NameEn = FormNameEn,
                NameAr = FormNameAr,
                DescriptionEn = FormDescriptionEn,
                DescriptionAr = FormDescriptionAr,
                Category = FormCategory,
                Price = FormPrice,
                IsAvailable = FormIsAvailable
            };

            var result = await ExecuteAsync(
                async () => await _menuService.CreateMenuItemAsync(request),
                onSuccess: (_) =>
                {
                    ClearMenuItemForm();
                    ShowNewItemForm = false;
                }
            );

            if (result.IsSuccess)
            {
                await LoadMenuItemsCommand.ExecuteAsync(null);
            }
        }

        /// <summary>
        /// Update the selected menu item.
        /// </summary>
        [RelayCommand]
        public async Task UpdateMenuItem()
        {
            if (SelectedMenuItem == null)
            {
                SetError("No menu item selected");
                return;
            }

            if (!ValidateMenuItemForm())
            {
                return;
            }

            var request = new UpdateMenuItemRequest
            {
                NameEn = FormNameEn,
                NameAr = FormNameAr,
                DescriptionEn = FormDescriptionEn,
                DescriptionAr = FormDescriptionAr,
                Category = FormCategory,
                Price = FormPrice,
                IsAvailable = FormIsAvailable
            };

            var result = await ExecuteAsync(
                async () => await _menuService.UpdateMenuItemAsync(SelectedMenuItem.Id, request)
            );

            if (result.IsSuccess)
            {
                ClearMenuItemForm();
                ShowNewItemForm = false;
                await LoadMenuItemsCommand.ExecuteAsync(null);
            }
        }

        /// <summary>
        /// Delete the selected menu item with confirmation.
        /// </summary>
        [RelayCommand]
        public async Task DeleteMenuItem()
        {
            if (SelectedMenuItem == null)
            {
                SetError("No menu item selected");
                return;
            }

            // In real app, show confirmation dialog here
            var result = await ExecuteAsync(
                async () => await _menuService.DeleteMenuItemAsync(SelectedMenuItem.Id)
            );

            if (result.IsSuccess)
            {
                SelectedMenuItem = null;
                await LoadMenuItemsCommand.ExecuteAsync(null);
            }
        }

        /// <summary>
        /// Toggle visibility of new item form.
        /// </summary>
        [RelayCommand]
        public void ToggleNewItemForm()
        {
            ShowNewItemForm = !ShowNewItemForm;
            _isEditMode = false;
            if (!ShowNewItemForm)
            {
                ClearMenuItemForm();
            }
        }

        /// <summary>
        /// Edit the selected menu item (populate form with its data).
        /// </summary>
        [RelayCommand]
        public void EditSelectedMenuItem()
        {
            if (SelectedMenuItem == null)
            {
                SetError("No menu item selected");
                return;
            }

            _isEditMode = true;
            FormNameEn = SelectedMenuItem.NameEn;
            FormNameAr = SelectedMenuItem.NameAr;
            FormDescriptionEn = SelectedMenuItem.DescriptionEn;
            FormDescriptionAr = SelectedMenuItem.DescriptionAr;
            FormCategory = SelectedMenuItem.Category;
            FormPrice = SelectedMenuItem.Price;
            FormIsAvailable = SelectedMenuItem.IsAvailable;
            ShowNewItemForm = true;
        }

        /// <summary>
        /// Validate menu item form inputs.
        /// </summary>
        private bool ValidateMenuItemForm()
        {
            if (string.IsNullOrWhiteSpace(FormNameEn))
            {
                SetError("English name is required");
                return false;
            }

            if (string.IsNullOrWhiteSpace(FormNameAr))
            {
                SetError("Arabic name is required");
                return false;
            }

            if (FormPrice < 0)
            {
                SetError("Price cannot be negative");
                return false;
            }

            if (FormPrice > 999.99m)
            {
                SetError("Price cannot exceed 999.99");
                return false;
            }

            if (string.IsNullOrWhiteSpace(FormCategory))
            {
                SetError("Category is required");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Clear menu item form inputs.
        /// </summary>
        private void ClearMenuItemForm()
        {
            FormNameEn = string.Empty;
            FormNameAr = string.Empty;
            FormDescriptionEn = null;
            FormDescriptionAr = null;
            FormCategory = "Main";
            FormPrice = 0m;
            FormIsAvailable = true;
            ClearError();
            _isEditMode = false;
        }

        /// <summary>
        /// Handle category filter change - reload items.
        /// </summary>
        partial void OnCategoryFilterChanged(string value)
        {
            LoadMenuItemsCommand.ExecuteAsync(null);
        }

        /// <summary>
        /// Handle search filter change - reload items.
        /// </summary>
        partial void OnSearchNameChanged(string value)
        {
            LoadMenuItemsCommand.ExecuteAsync(null);
        }
    }
}
