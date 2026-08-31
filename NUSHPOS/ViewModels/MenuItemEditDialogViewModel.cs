using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NUSHPOS.Models;
using NUSHPOS.Services;
using NUSHPOS.ViewModels.Base;

namespace NUSHPOS.ViewModels;

public partial class MenuItemEditDialogViewModel : ViewModelBase
{
    private readonly MenuService _menuService;

    [ObservableProperty]
    private ObservableCollection<MenuItem> _templateItems = new();

    [ObservableProperty]
    private MenuItem? _selectedTemplateItem;

    [ObservableProperty]
    private MenuItem _item = new();

    [ObservableProperty]
    private ObservableCollection<MenuCategory> _categories = new();

    [ObservableProperty]
    private MenuCategory? _selectedCategory;

    [ObservableProperty]
    private ObservableCollection<MenuGroup> _groups = new();

    [ObservableProperty]
    private MenuGroup? _selectedGroup;

    [ObservableProperty]
    private ObservableCollection<TaxGroup> _taxGroups = new();

    [ObservableProperty]
    private TaxGroup? _selectedTaxGroup;

    [ObservableProperty]
    private ObservableCollection<Printer> _printers = new();

    [ObservableProperty]
    private Printer? _selectedPrinter1;

    [ObservableProperty]
    private Printer? _selectedPrinter2;

    [ObservableProperty]
    private Printer? _selectedPrinter3;

    [ObservableProperty]
    private ObservableCollection<MenuModifierGroup> _modifierGroups = new();

    [ObservableProperty]
    private MenuModifierGroup? _selectedModifierGroup;

    [ObservableProperty]
    private MenuModifierGroup? _selectedForcedModifierGroup;

    [ObservableProperty]
    private ObservableCollection<MenuComboItem> _comboItems = new();

    [ObservableProperty]
    private MenuComboItem? _selectedComboItem;

    [ObservableProperty]
    private ObservableCollection<int> _securityLevels = new(Enumerable.Range(1, 10));

    [ObservableProperty]
    private int _selectedTabIndex = 0;

    public bool DialogResult { get; private set; }
    public event Action? RequestClose;

    public MenuItemEditDialogViewModel(MenuService menuService)
    {
        _menuService = menuService;
        Title = "MENYU MƏHSULUNUN REDAKTƏSİ";
    }

    public async Task InitializeAsync(MenuItem item)
    {
        // Clone item so changes can be cancelled
        Item = new MenuItem
        {
            AutoID = item.AutoID,
            MenuItemID = item.MenuItemID,
            MainMenuItemID = item.MainMenuItemID,
            MenuItemText = item.MenuItemText,
            MenuCategoryID = item.MenuCategoryID,
            MenuGroupID = item.MenuGroupID,
            DisplayIndex = item.DisplayIndex,
            DefaultUnitPrice = item.DefaultUnitPrice,
            MenuItemCost = item.MenuItemCost,
            MenuItemDescription = item.MenuItemDescription,
            MenuItemActive = item.MenuItemActive ?? true,
            MenuItemInStock = item.MenuItemInStock ?? true,
            TaxPercent = item.TaxPercent,
            TaxGroupID = item.TaxGroupID,
            MenuItemDiscountable = item.MenuItemDiscountable ?? true,
            PictureName = item.PictureName,
            ShowCaption = item.ShowCaption ?? true,
            IsComboMenu = item.IsComboMenu,
            IsTopMenu = item.IsTopMenu,
            ButtonColor = item.ButtonColor,
            Barcode = item.Barcode,
            Barcode2 = item.Barcode2,
            ItemDelCharge = item.ItemDelCharge,
            ItemDelComp = item.ItemDelComp,
            DineInPrice = item.DineInPrice,
            BarTabPrice = item.BarTabPrice,
            TakeOutPrice = item.TakeOutPrice,
            DeliveryPrice = item.DeliveryPrice,
            OrderByWeight = item.OrderByWeight,
            PrintPizzaLabel = item.PrintPizzaLabel,
            KitchenSortNumber = item.KitchenSortNumber,
            ModBuilderTemplateID = item.ModBuilderTemplateID,
            MenuItemTypeID = item.MenuItemTypeID,
            AccountingCode = item.AccountingCode,
            PrintOnLabel = item.PrintOnLabel,
            UsedPrinterID1 = item.UsedPrinterID1,
            UsedPrinterID2 = item.UsedPrinterID2,
            UsedPrinterID3 = item.UsedPrinterID3,
            SecurityLevel = item.SecurityLevel ?? 1,
            MenuItemKey = item.MenuItemKey,
            MenuCategoryKey = item.MenuCategoryKey,
            MenuGroupKey = item.MenuGroupKey,
            TaxGroupKey = item.TaxGroupKey,
            MenuModifierKey = item.MenuModifierKey,
            MenuModifierForcedKey = item.MenuModifierForcedKey,
            SecLangMenuItemText = item.SecLangMenuItemText,
            UseKds1 = item.UseKds1,
            UseKds2 = item.UseKds2,
            UseKds3 = item.UseKds3,
            UseKds4 = item.UseKds4,
            UseKds5 = item.UseKds5,
            UseKds6 = item.UseKds6,
            UseKds7 = item.UseKds7,
            UseKds8 = item.UseKds8,
        };

        // Load dropdown lists
        var cats = await _menuService.GetCategoriesAsync();
        Categories = new ObservableCollection<MenuCategory>(cats);
        SelectedCategory = Categories.FirstOrDefault(c => c.MenuCategoryID == Item.MenuCategoryID || c.MenuCategoryKey == Item.MenuCategoryKey);

        var groups = await _menuService.GetMenuGroupsFullAsync();
        Groups = new ObservableCollection<MenuGroup>(groups);
        SelectedGroup = Groups.FirstOrDefault(g => g.MenuGroupID == Item.MenuGroupID || g.MenuGroupKey == Item.MenuGroupKey);

        var taxes = await _menuService.GetTaxGroupsAsync();
        TaxGroups = new ObservableCollection<TaxGroup>(taxes);
        SelectedTaxGroup = TaxGroups.FirstOrDefault(t => t.TaxGroupID == Item.TaxGroupID);

        var printers = await _menuService.GetPrintersAsync();
        Printers = new ObservableCollection<Printer>(printers);
        SelectedPrinter1 = Printers.FirstOrDefault(p => p.PrinterID == Item.UsedPrinterID1);
        SelectedPrinter2 = Printers.FirstOrDefault(p => p.PrinterID == Item.UsedPrinterID2);
        SelectedPrinter3 = Printers.FirstOrDefault(p => p.PrinterID == Item.UsedPrinterID3);

        var modGroups = await _menuService.GetModifierGroupsAsync();
        ModifierGroups = new ObservableCollection<MenuModifierGroup>(modGroups);
        SelectedModifierGroup = ModifierGroups.FirstOrDefault(m => m.MenuModifierGroupKey == Item.MenuModifierKey);
        SelectedForcedModifierGroup = ModifierGroups.FirstOrDefault(m => m.MenuModifierGroupKey == Item.MenuModifierForcedKey);

        var combos = await _menuService.GetComboMenuNamesAsync();
        ComboItems = new ObservableCollection<MenuComboItem>(combos);
        SelectedComboItem = ComboItems.FirstOrDefault(c => c.ComboMenuItemKey == Item.ComboMenuItemKey);

        // Load reference template items
        var allItems = await _menuService.GetAllActiveMenuItemsAsync();
        var templateList = new ObservableCollection<MenuItem>();
        templateList.Add(new MenuItem { AutoID = 0, MenuItemText = "----" });
        foreach (var itm in allItems)
        {
            templateList.Add(itm);
        }
        TemplateItems = templateList;
        SelectedTemplateItem = TemplateItems.FirstOrDefault();
    }

    partial void OnSelectedTemplateItemChanged(MenuItem? value)
    {
        if (value == null || value.AutoID == 0) return;

        // Copy all characteristics from the template
        Item.MenuCategoryID = value.MenuCategoryID;
        Item.MenuCategoryKey = value.MenuCategoryKey;
        Item.MenuGroupID = value.MenuGroupID;
        Item.MenuGroupKey = value.MenuGroupKey;
        Item.TaxGroupID = value.TaxGroupID;
        Item.TaxPercent = value.TaxPercent;
        Item.DefaultUnitPrice = value.DefaultUnitPrice;
        Item.MenuItemCost = value.MenuItemCost;
        Item.MenuItemDescription = value.MenuItemDescription;
        Item.MenuItemNotification = value.MenuItemNotification;
        Item.MenuItemActive = value.MenuItemActive ?? true;
        Item.MenuItemInStock = value.MenuItemInStock ?? true;
        Item.MenuItemTaxable = value.MenuItemTaxable ?? true;
        Item.MenuItemDiscountable = value.MenuItemDiscountable ?? true;
        Item.PictureName = value.PictureName;
        Item.ShowCaption = value.ShowCaption ?? true;
        Item.IsComboMenu = value.IsComboMenu;
        Item.IsTopMenu = value.IsTopMenu;
        Item.ButtonColor = value.ButtonColor;
        Item.Barcode = value.Barcode;
        Item.Barcode2 = value.Barcode2;
        Item.ItemDelCharge = value.ItemDelCharge;
        Item.ItemDelComp = value.ItemDelComp;
        Item.DineInPrice = value.DineInPrice;
        Item.BarTabPrice = value.BarTabPrice;
        Item.TakeOutPrice = value.TakeOutPrice;
        Item.DriveThruPrice = value.DriveThruPrice;
        Item.DeliveryPrice = value.DeliveryPrice;
        Item.OrderByWeight = value.OrderByWeight;
        Item.PrintPizzaLabel = value.PrintPizzaLabel;
        Item.KitchenSortNumber = value.KitchenSortNumber;
        Item.ModBuilderTemplateID = value.ModBuilderTemplateID;
        Item.MenuItemTypeID = value.MenuItemTypeID;
        Item.AccountingCode = value.AccountingCode;
        Item.PrintOnLabel = value.PrintOnLabel;
        Item.UsedPrinterID1 = value.UsedPrinterID1;
        Item.UsedPrinterID2 = value.UsedPrinterID2;
        Item.UsedPrinterID3 = value.UsedPrinterID3;
        Item.SecurityLevel = value.SecurityLevel ?? 1;
        Item.MenuModifierKey = value.MenuModifierKey;
        Item.MenuModifierForcedKey = value.MenuModifierForcedKey;
        Item.SecLangMenuItemText = value.SecLangMenuItemText;
        Item.UseKds1 = value.UseKds1;
        Item.UseKds2 = value.UseKds2;
        Item.UseKds3 = value.UseKds3;
        Item.UseKds4 = value.UseKds4;
        Item.UseKds5 = value.UseKds5;
        Item.UseKds6 = value.UseKds6;
        Item.UseKds7 = value.UseKds7;
        Item.UseKds8 = value.UseKds8;

        // Leave product name blank for new item input
        Item.MenuItemText = string.Empty;

        // Trigger property changed notifications on Item
        OnPropertyChanged(nameof(Item));

        // Update dropdown selections to reflect template values
        SelectedCategory = Categories.FirstOrDefault(c => c.MenuCategoryID == Item.MenuCategoryID || c.MenuCategoryKey == Item.MenuCategoryKey);
        SelectedGroup = Groups.FirstOrDefault(g => g.MenuGroupID == Item.MenuGroupID || g.MenuGroupKey == Item.MenuGroupKey);
        SelectedTaxGroup = TaxGroups.FirstOrDefault(t => t.TaxGroupID == Item.TaxGroupID);
        SelectedPrinter1 = Printers.FirstOrDefault(p => p.PrinterID == Item.UsedPrinterID1);
        SelectedPrinter2 = Printers.FirstOrDefault(p => p.PrinterID == Item.UsedPrinterID2);
        SelectedPrinter3 = Printers.FirstOrDefault(p => p.PrinterID == Item.UsedPrinterID3);
        SelectedModifierGroup = ModifierGroups.FirstOrDefault(m => m.MenuModifierGroupKey == Item.MenuModifierKey);
        SelectedForcedModifierGroup = ModifierGroups.FirstOrDefault(m => m.MenuModifierGroupKey == Item.MenuModifierForcedKey);
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (SelectedCategory != null)
        {
            Item.MenuCategoryID = SelectedCategory.MenuCategoryID;
            Item.MenuCategoryKey = SelectedCategory.MenuCategoryKey;
            Item.MenuCategoryText = SelectedCategory.MenuCategoryText;
        }

        if (SelectedGroup != null)
        {
            Item.MenuGroupID = SelectedGroup.MenuGroupID;
            Item.MenuGroupKey = SelectedGroup.MenuGroupKey;
            Item.MenuItemGroupText = SelectedGroup.MenuGroupText;
        }

        if (SelectedTaxGroup != null)
        {
            Item.TaxGroupID = SelectedTaxGroup.TaxGroupID;
            Item.TaxPercent = SelectedTaxGroup.TaxRate;
        }

        Item.UsedPrinterID1 = SelectedPrinter1?.PrinterID;
        Item.UsedPrinterID2 = SelectedPrinter2?.PrinterID;
        Item.UsedPrinterID3 = SelectedPrinter3?.PrinterID;

        Item.MenuModifierKey = SelectedModifierGroup?.MenuModifierGroupKey;
        Item.MenuModifierForcedKey = SelectedForcedModifierGroup?.MenuModifierGroupKey;
        Item.ComboMenuItemKey = SelectedComboItem?.ComboMenuItemKey;

        try
        {
            if (string.IsNullOrWhiteSpace(Item.MenuItemText))
            {
                MessageBox.Show("Zəhmət olmasa məhsulun tam adını qeyd edin.", "Xəbərdarlıq", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (Item.AutoID > 0)
            {
                await _menuService.UpdateMenuItemAsync(Item);
            }
            else
            {
                int grpId = Item.MenuGroupID ?? SelectedGroup?.MenuGroupID ?? 0;
                int dispIdx = Item.DisplayIndex ?? 1;
                await _menuService.InsertMenuItemAsync(Item, grpId, dispIdx);
            }

            DialogResult = true;
            RequestClose?.Invoke();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Məhsul yadda saxlanılarkən xəta baş verdi: {ex.Message}", "Xəta", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private void Cancel()
    {
        DialogResult = false;
        RequestClose?.Invoke();
    }

    [RelayCommand]
    private async Task HideItemAsync()
    {
        Item.MenuItemActive = false;
        await SaveAsync();
    }
}
