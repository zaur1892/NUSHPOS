using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NUSHPOS.Models;
using NUSHPOS.Services;
using NUSHPOS.ViewModels.Base;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace NUSHPOS.ViewModels;

public class MenuItemSlot : ObservableObject
{
    private int _slotIndex;
    public int SlotIndex
    {
        get => _slotIndex;
        set => SetProperty(ref _slotIndex, value);
    }

    private int _globalIndex;
    public int GlobalIndex
    {
        get => _globalIndex;
        set => SetProperty(ref _globalIndex, value);
    }

    private MenuItem? _item;
    public MenuItem? Item
    {
        get => _item;
        set
        {
            if (SetProperty(ref _item, value))
            {
                OnPropertyChanged(nameof(HasItem));
                OnPropertyChanged(nameof(DisplayText));
                OnPropertyChanged(nameof(ButtonColor));
            }
        }
    }

    public bool HasItem => Item != null;
    public string DisplayText => Item?.MenuItemText ?? "";
    public string ButtonColor => Item?.ButtonColor ?? "#dce8f5";
}

public partial class MenuDesignerViewModel : ViewModelBase
{
    private readonly MenuService _menuService;
    private readonly DatabaseService _databaseService;

    [ObservableProperty]
    private ObservableCollection<MenuGroup> _menuGroups = new();

    [ObservableProperty]
    private MenuGroup? _selectedMenuGroup;

    [ObservableProperty]
    private ObservableCollection<MenuItemSlot> _gridSlots = new();

    [ObservableProperty]
    private int _selectedPageIndex = 1;

    [ObservableProperty]
    private List<int> _pages = Enumerable.Range(1, 10).ToList();

    [ObservableProperty]
    private bool _isLoading;

    private List<MenuItem> _currentGroupItems = new();

    public event Action? RequestClose;

    public MenuDesignerViewModel(MenuService menuService, DatabaseService databaseService)
    {
        _menuService = menuService;
        _databaseService = databaseService;
        Title = "MENYU DİZAYNERİ";

        // Initialize 28 slots (4 columns x 7 rows)
        for (int i = 1; i <= 28; i++)
        {
            GridSlots.Add(new MenuItemSlot { SlotIndex = i, GlobalIndex = i });
        }

        _ = LoadMenuGroupsAsync();
    }

    [RelayCommand]
    public async Task LoadMenuGroupsAsync()
    {
        IsLoading = true;
        try
        {
            var groups = await _menuService.GetMenuGroupsFullAsync();
            MenuGroups = new ObservableCollection<MenuGroup>(groups);

            if (MenuGroups.Count > 0 && SelectedMenuGroup == null)
            {
                SelectedMenuGroup = MenuGroups.First();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Menyu qrupları yüklənərkən xəta baş verdi: {ex.Message}", "Xəta", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            IsLoading = false;
        }
    }

    async partial void OnSelectedMenuGroupChanged(MenuGroup? value)
    {
        if (value != null)
        {
            await LoadGroupItemsAsync(value.MenuGroupID);
        }
        else
        {
            _currentGroupItems.Clear();
            RefreshGridSlots();
        }
    }

    private async Task LoadGroupItemsAsync(int menuGroupId)
    {
        IsLoading = true;
        try
        {
            var items = await _menuService.GetMenuItemsLayoutByGroupIdAsync(menuGroupId);
            _currentGroupItems = items.ToList();
            RefreshGridSlots();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Menyu məhsulları yüklənərkən xəta baş verdi: {ex.Message}", "Xəta", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private void SelectPage(int page)
    {
        SelectedPageIndex = page;
        RefreshGridSlots();
    }

    private void RefreshGridSlots()
    {
        int pageOffset = (SelectedPageIndex - 1) * 28;

        for (int i = 0; i < 28; i++)
        {
            int target0 = pageOffset + i;        // 0..27 on page 1
            int target1 = pageOffset + i + 1;    // 1..28 on page 1

            // Match by DisplayIndex in layout (supports both 0-indexed and 1-indexed database records)
            var matchingItem = _currentGroupItems.FirstOrDefault(x => 
                x.DisplayIndex == target0 || 
                x.DisplayIndex == target1);

            GridSlots[i].SlotIndex = i + 1;
            GridSlots[i].GlobalIndex = target0;
            GridSlots[i].Item = matchingItem;
        }
    }

    [RelayCommand]
    private void MoveFirstGroup()
    {
        if (MenuGroups.Count > 0)
        {
            SelectedMenuGroup = MenuGroups.First();
        }
    }

    [RelayCommand]
    private void MovePreviousGroup()
    {
        if (SelectedMenuGroup == null || MenuGroups.Count == 0) return;
        int idx = MenuGroups.IndexOf(SelectedMenuGroup);
        if (idx > 0)
        {
            SelectedMenuGroup = MenuGroups[idx - 1];
        }
    }

    [RelayCommand]
    private void MoveNextGroup()
    {
        if (SelectedMenuGroup == null || MenuGroups.Count == 0) return;
        int idx = MenuGroups.IndexOf(SelectedMenuGroup);
        if (idx >= 0 && idx < MenuGroups.Count - 1)
        {
            SelectedMenuGroup = MenuGroups[idx + 1];
        }
    }

    [RelayCommand]
    private void MoveLastGroup()
    {
        if (MenuGroups.Count > 0)
        {
            SelectedMenuGroup = MenuGroups.Last();
        }
    }

    [RelayCommand]
    private void ManageGroups()
    {
        MessageBox.Show("Menyu qruplarının tənzimlənməsi bölməsi tezliklə aktiv olacaq.", "Məlumat", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    [RelayCommand]
    private async Task SlotClick(MenuItemSlot slot)
    {
        if (SelectedMenuGroup == null)
        {
            MessageBox.Show("Zəhmət olmasa əvvəlcə sol siyahıdan bir menyu qrupu seçin.", "Məlumat", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        MenuItem targetItem;
        if (slot.Item != null && !string.IsNullOrEmpty(slot.Item.MenuItemKey))
        {
            targetItem = await _menuService.GetMenuItemByKeyAsync(slot.Item.MenuItemKey) ?? slot.Item;
        }
        else
        {
            // Empty slot - create new menu item
            int calculatedDisplayIndex = slot.GlobalIndex;
            targetItem = new MenuItem
            {
                MenuGroupID = SelectedMenuGroup.MenuGroupID,
                MenuGroupKey = SelectedMenuGroup.MenuGroupKey,
                MenuItemGroupText = SelectedMenuGroup.MenuGroupText,
                DisplayIndex = calculatedDisplayIndex,
                MenuItemActive = true,
                MenuItemInStock = true,
                MenuItemDiscountable = true,
                ShowCaption = true,
                DefaultUnitPrice = 0,
                SecurityLevel = 1,
                MenuItemKey = Guid.NewGuid().ToString()
            };
        }

        var vm = new MenuItemEditDialogViewModel(_menuService);
        await vm.InitializeAsync(targetItem);

        var dialog = new Views.Dialogs.MenuItemEditDialog
        {
            DataContext = vm,
            Owner = Application.Current.MainWindow
        };

        vm.RequestClose += () => dialog.Close();
        dialog.ShowDialog();

        if (vm.DialogResult && SelectedMenuGroup != null)
        {
            await LoadGroupItemsAsync(SelectedMenuGroup.MenuGroupID);
        }
    }

    [RelayCommand]
    private void SaveAndClose()
    {
        RequestClose?.Invoke();
    }

    [RelayCommand]
    private void Close()
    {
        RequestClose?.Invoke();
    }
}
