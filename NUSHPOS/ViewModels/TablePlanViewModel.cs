using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NUSHPOS.Helpers;
using NUSHPOS.Models;
using NUSHPOS.Services;
using NUSHPOS.ViewModels.Base;
using System.Linq;
using System.Threading.Tasks;

namespace NUSHPOS.ViewModels;

public partial class TablePlanViewModel : ViewModelBase
{
    private readonly NavigationService _navigationService;
    private readonly TableService _tableService;
    private readonly OrderService _orderService;

    [ObservableProperty]
    private ObservableCollection<DineInTableGroup> _tableGroups = new();

    [ObservableProperty]
    private DineInTableGroup? _selectedGroup;

    [ObservableProperty]
    private ObservableCollection<TableViewModel> _tables = new();

    [ObservableProperty]
    private bool _showGuestDialog;

    [ObservableProperty]
    private int _guestCount = 2;

    [ObservableProperty]
    private string _guestCountText = "2";

    private DineInTable? _selectedTable;

    public TablePlanViewModel(NavigationService navigationService, TableService tableService, OrderService orderService)
    {
        _navigationService = navigationService;
        _tableService = tableService;
        _orderService = orderService;
        Title = "Masa Satışı";
        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        IsBusy = true;
        try
        {
            var groups = await _tableService.GetTableGroupsAsync();
            TableGroups = new ObservableCollection<DineInTableGroup>(groups);
            if (TableGroups.Count > 0)
            {
                SelectedGroup = TableGroups[0];
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    partial void OnSelectedGroupChanged(DineInTableGroup? value)
    {
        if (value != null)
            _ = LoadTablesAsync(value.AutoID);
    }

    private async Task LoadTablesAsync(int groupId)
    {
        var dynamicTables = await _tableService.GetTablesByGroupWithActiveOrdersAsync(groupId);
        var tableViewModels = new ObservableCollection<TableViewModel>();
        
        foreach (var dt in dynamicTables)
        {
            var dict = (System.Collections.Generic.IDictionary<string, object>)dt;
            
            var tableModel = new DineInTable 
            {
                AutoID = (int)dict["AutoID"],
                DineInTableID = (int)dict["DineInTableID"],
                DineInTableText = dict["DineInTableText"] as string,
                MaxGuests = dict["MaxGuests"] != null ? Convert.ToInt32(dict["MaxGuests"]) : null,
                DineInTableActive = Convert.ToBoolean(dict["DineInTableActive"] ?? 1)
            };

            var activeOrderKeyStr = dict["ActiveOrderKey"] as string;
            bool hasActiveOrder = !string.IsNullOrEmpty(activeOrderKeyStr);
            bool guestCheckPrinted = dict["GuestCheckPrinted"] != null && Convert.ToBoolean(dict["GuestCheckPrinted"]);
            int guestCount = dict.TryGetValue("ActiveOrderGuestCount", out var gc) && gc != null ? Convert.ToInt32(gc) : 0;
            
            string employeeName = hasActiveOrder && dict.TryGetValue("ActiveOrderEmyloyeeName", out var emp) && emp != null ? emp.ToString()! : "";
            
            string elapsedTime = "";
            if (hasActiveOrder && dict.TryGetValue("ActiveOrderDateTime", out var orderDt) && orderDt != null)
            {
                if (DateTime.TryParse(orderDt.ToString(), out DateTime orderTime))
                {
                    var diff = DateTime.Now - orderTime;
                    elapsedTime = $"{(int)diff.TotalMinutes} dk";
                }
            }

            tableViewModels.Add(new TableViewModel
            {
                Table = tableModel,
                TableName = dict["DineInTableText"] as string ?? $"Masa {dict["DineInTableID"]}",
                Status = hasActiveOrder ? (guestCheckPrinted ? 2 : 1) : 0,
                GuestCount = hasActiveOrder ? guestCount : 0,
                OrderId = dict.TryGetValue("ActiveOrderID", out var oid) && oid != null ? Convert.ToInt32(oid) : 0,
                OrderKey = activeOrderKeyStr ?? "",
                Amount = hasActiveOrder && dict["ActiveOrderAmountDue"] != null ? Convert.ToDecimal(dict["ActiveOrderAmountDue"]) : 0m,
                EmployeeName = employeeName,
                ElapsedTime = elapsedTime
            });
        }
        
        Tables = tableViewModels;
    }

    [RelayCommand]
    private void SelectTable(TableViewModel tableVm)
    {
        if (tableVm.Status == 0) // Empty table
        {
            _selectedTable = tableVm.Table;
            GuestCount = 2;
            GuestCountText = "2";
            ShowGuestDialog = true;
        }
        else // Occupied table - go to existing order
        {
            _navigationService.NavigateTo<SaleScreenViewModel>(new { OrderType = 1, TableId = tableVm.Table!.DineInTableID, OrderId = tableVm.OrderId, OrderKey = tableVm.OrderKey });
        }
    }

    [RelayCommand]
    private void ConfirmGuests()
    {
        if (int.TryParse(GuestCountText, out int count) && count > 0)
        {
            GuestCount = count;
        }
        ShowGuestDialog = false;
        if (_selectedTable != null)
        {
            _navigationService.NavigateTo<SaleScreenViewModel>(new { OrderType = 1, TableId = _selectedTable.DineInTableID, GuestCount = GuestCount });
        }
    }

    [RelayCommand]
    private void CancelGuestDialog()
    {
        ShowGuestDialog = false;
    }

    [RelayCommand]
    private void GuestNumpad(string digit)
    {
        if (digit == "C")
            GuestCountText = "";
        else if (digit == "⌫")
            GuestCountText = GuestCountText.Length > 0 ? GuestCountText[..^1] : "";
        else
            GuestCountText += digit;
    }

    [RelayCommand]
    private void GoBack()
    {
        _navigationService.NavigateTo<MainScreenViewModel>();
    }
}

// Helper ViewModel for table display
public partial class TableViewModel : ObservableObject
{
    [ObservableProperty] private DineInTable? _table;
    [ObservableProperty] private string _tableName = "";
    [ObservableProperty] private int _status; // 0=empty, 1=active, 2=check printed
    [ObservableProperty] private int _guestCount;
    [ObservableProperty] private int _orderId;
    [ObservableProperty] private string _orderKey = "";
    [ObservableProperty] private decimal _amount;
    [ObservableProperty] private string _employeeName = "";
    [ObservableProperty] private string _elapsedTime = "";
}
