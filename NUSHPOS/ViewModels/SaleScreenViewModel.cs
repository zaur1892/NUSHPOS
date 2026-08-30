using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NUSHPOS.Helpers;
using NUSHPOS.Models;
using NUSHPOS.Services;
using NUSHPOS.ViewModels.Base;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NUSHPOS.ViewModels;

public partial class SaleScreenViewModel : ViewModelBase
{
    private readonly NavigationService _navigationService;
    private readonly MenuService _menuService;
    private readonly OrderService _orderService;
    private readonly PaymentService _paymentService;
    private readonly DiscountService _discountService;
    private readonly CustomerService _customerService;
    private readonly AccessLogService _accessLogService;

    // Order info
    [ObservableProperty] private int _orderType = 1; // 1=DineIn, 3=TakeOut, 5=Delivery
    [ObservableProperty] private int _tableId;
    [ObservableProperty] private int _orderId;
    [ObservableProperty] private string _orderKey = "";
    [ObservableProperty] private int _guestCount;
    [ObservableProperty] private string _tableName = "";
    [ObservableProperty] private string _employeeName = SessionManager.EmployeeName;
    [ObservableProperty] private string _currentDate = DateTime.Now.ToString("dd.MM.yyyy HH:mm");
    [ObservableProperty] private string _orderTypeName = "MASA";

    // Menu
    [ObservableProperty] private ObservableCollection<MenuGroup> _menuGroups = new();
    [ObservableProperty] private MenuGroup? _selectedMenuGroup;
    [ObservableProperty] private ObservableCollection<MenuItem> _menuItems = new();

    [ObservableProperty] private ObservableCollection<OrderLineViewModel> _orderLines = new();
    [ObservableProperty] private OrderLineViewModel? _selectedOrderLine;
    [ObservableProperty] private int _quantity = 1;

    // Payment Methods
    [ObservableProperty] private ObservableCollection<PaymentMethod> _paymentMethods = new();


    // Totals
    [ObservableProperty] private decimal _subTotal;
    [ObservableProperty] private decimal _taxTotal;
    [ObservableProperty] private decimal _gratuity;
    [ObservableProperty] private decimal _discountTotal;
    [ObservableProperty] private decimal _grandTotal;
    [ObservableProperty] private decimal _amountDue;
    [ObservableProperty] private string _appliedDiscountName = "ÇEK ENDİRİMİ";

    // State
    private OrderHeader? _currentOrder;
    private bool _isEditMode;

    public SaleScreenViewModel(
        NavigationService navigationService,
        MenuService menuService,
        OrderService orderService,
        PaymentService paymentService,
        DiscountService discountService,
        CustomerService customerService,
        AccessLogService accessLogService)
    {
        _navigationService = navigationService;
        _menuService = menuService;
        _orderService = orderService;
        _paymentService = paymentService;
        _discountService = discountService;
        _customerService = customerService;
        _accessLogService = accessLogService;
        Title = "Satış Ekranı";
    }

    public void Initialize(object? parameter)
    {
        if (parameter is null) return;
        
        var type = parameter.GetType();
        
        var orderTypeProp = type.GetProperty("OrderType");
        if (orderTypeProp != null) OrderType = (int)orderTypeProp.GetValue(parameter)!;
        
        var tableIdProp = type.GetProperty("TableId");
        if (tableIdProp != null) TableId = (int)tableIdProp.GetValue(parameter)!;

        var guestProp = type.GetProperty("GuestCount");
        if (guestProp != null) GuestCount = (int)guestProp.GetValue(parameter)!;

        var orderIdProp = type.GetProperty("OrderId");
        if (orderIdProp != null)
        {
            OrderId = (int)orderIdProp.GetValue(parameter)!;
            _isEditMode = OrderId > 0;
        }
        var orderKeyProp = type.GetProperty("OrderKey");
        if (orderKeyProp != null)
        {
            var ok = orderKeyProp.GetValue(parameter) as string;
            if (!string.IsNullOrEmpty(ok))
            {
                OrderKey = ok;
                _isEditMode = true;
            }
        }

        OrderTypeName = OrderType switch { 1 => "MASA", 3 => "AL GÖTÜR", 5 => "PAKET", _ => "SATIŞ" };
        
        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        IsBusy = true;
        try
        {
            // Load menu groups
            var groups = await _menuService.GetMenuGroupsAsync();
            MenuGroups = new ObservableCollection<MenuGroup>(groups);
            if (MenuGroups.Count > 0)
                SelectedMenuGroup = MenuGroups[0];

            // Load payment methods
            try
            {
                var payments = await _paymentService.GetPaymentMethodsAsync();
                PaymentMethods = new ObservableCollection<PaymentMethod>(payments);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading payment methods: {ex.Message}");
            }


            // Load existing order if editing
            if (_isEditMode)
            {
                dynamic? currentOrderDynamic = null;
                
                if (!string.IsNullOrEmpty(OrderKey))
                    currentOrderDynamic = await _orderService.GetFullOrderDetailsByKeyAsync(OrderKey);

                if (currentOrderDynamic != null)
                {
                    var dict = (System.Collections.Generic.IDictionary<string, object>)currentOrderDynamic;
                    OrderKey = dict["OrderKey"] as string ?? "";
                    OrderId = dict.TryGetValue("OrderID", out var oid) && oid != null ? Convert.ToInt32(oid) : 0;
                    TableName = dict["DineInTableName"] as string ?? "";
                    
                    if (dict.TryGetValue("GuestNumber", out var gn) && gn != null)
                        GuestCount = Convert.ToInt32(gn);

                    if (dict.TryGetValue("DiscountText", out var dt) && dt != null && !string.IsNullOrEmpty(dt.ToString()))
                        AppliedDiscountName = $"ÇEK ENDİRİMİ ({dt})";
                    else
                        AppliedDiscountName = "ÇEK ENDİRİMİ";
                    
                    // We need to keep a valid OrderHeader for Save operation later
                    _currentOrder = new OrderHeader 
                    {
                        AutoID = dict.TryGetValue("AutoID", out var aid) && aid != null ? Convert.ToInt32(aid) : 0,
                        OrderID = OrderId,
                        OrderKey = OrderKey,
                        DineInTableID = dict.TryGetValue("DineInTableID", out var did) && did != null ? Convert.ToInt32(did) : null,
                        GuestNumber = GuestCount,
                        EditKey = dict["EditKey"] as string,
                        SyncKey = dict["SyncKey"] as string,
                        OrderStatus = dict.TryGetValue("OrderStatus", out var os) && os != null ? Convert.ToInt32(os) : 1,
                        OrderType = dict.TryGetValue("OrderType", out var ot) && ot != null ? Convert.ToInt32(ot) : 1,
                        EmployeeID = dict.TryGetValue("EmployeeID", out var eid) && eid != null ? Convert.ToInt32(eid) : SessionManager.EmployeeID,
                        StationID = dict.TryGetValue("StationID", out var sid) && sid != null ? Convert.ToInt32(sid) : SessionManager.StationID,
                        BranchID = dict.TryGetValue("BranchID", out var bid) && bid != null ? Convert.ToInt32(bid) : SessionManager.BranchID,
                        LineDeleted = dict.TryGetValue("LineDeleted", out var ld) && ld != null ? Convert.ToInt32(ld) : 0,
                        DiscountID = dict.TryGetValue("DiscountID", out var did2) && did2 != null ? Convert.ToInt32(did2) : 0,
                        DiscountKey = dict["DiscountKey"] as string,
                        DiscountAmountValue = dict.TryGetValue("DiscountAmountValue", out var dav) && dav != null ? Convert.ToDecimal(dav) : 0,
                        DiscountBasisValue = dict.TryGetValue("DiscountBasisValue", out var dbv) && dbv != null ? Convert.ToInt32(dbv) : 0,
                        DiscountOrderAmount = dict.TryGetValue("DiscountOrderAmount", out var doa) && doa != null ? Convert.ToDecimal(doa) : 0,
                        DiscountTotalAmount = dict.TryGetValue("DiscountTotalAmount", out var dta) && dta != null ? Convert.ToDecimal(dta) : 0,
                        UsedDiscountName = dict.TryGetValue("DiscountText", out var dtext) && dtext != null ? dtext.ToString() : null
                    };
                    
                    var transactions = await _orderService.GetFullOrderTransactionsByKeyAsync(OrderKey);

                    foreach (var t in transactions)
                    {
                        var tDict = (System.Collections.Generic.IDictionary<string, object>)t;
                        OrderLines.Add(new OrderLineViewModel
                        {
                            AutoID = (int)tDict["AutoID"],
                            MenuItemText = tDict["MenuItemText"] as string ?? "",
                            MenuItemKey = tDict["MenuItemKey"] as string ?? "",
                            Quantity = tDict["Quantity"] != null ? Convert.ToDecimal(tDict["Quantity"]) : 1m,
                            UnitPrice = tDict["MenuItemUnitPrice"] != null ? Convert.ToDecimal(tDict["MenuItemUnitPrice"]) : 0m,
                            TotalPrice = tDict["ExtendedPrice"] != null ? Convert.ToDecimal(tDict["ExtendedPrice"]) : 0m,
                            DiscountAmount = tDict["DiscountLineAmount"] != null ? Convert.ToDecimal(tDict["DiscountLineAmount"]) : 0m,
                            UsedDiscountName = tDict.TryGetValue("UsedDiscountName", out var udn) && udn != null ? udn.ToString() ?? "" : "",
                            TaxPercent = tDict["TaxPercent"] != null ? Convert.ToDecimal(tDict["TaxPercent"]) : 0m,
                            MenuItemGroupText = tDict.TryGetValue("MenuItemGroupText", out var gt) && gt != null ? gt.ToString() ?? "" : "",
                            MenumItemCategoryText = tDict.TryGetValue("MenumItemCategoryText", out var ct) && ct != null ? ct.ToString() ?? "" : "",
                            IsExisting = true
                        });
                    }

                    // Fetch total paid so far
                    var payments = await _paymentService.GetOrderPaymentsAsync(OrderId);
                    _totalPaid = payments.Sum(p => p.AmountPaid ?? 0);

                    RecalculateTotals();
                }
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    partial void OnSelectedMenuGroupChanged(MenuGroup? value)
    {
        if (value != null)
            _ = LoadMenuItemsAsync(value.MenuGroupID);
    }

    private async Task LoadMenuItemsAsync(int groupId)
    {
        var items = await _menuService.GetMenuItemsByGroupAsync(groupId);
        MenuItems = new ObservableCollection<MenuItem>(items);
    }

      private bool IsOrderClosedOrVoided()
      {
          if (_currentOrder?.OrderStatus == 4 || _currentOrder?.OrderStatus == 2)
          {
              System.Windows.MessageBox.Show("Bu çek artıq bağlanıb və ya ləğv edilib. Dəyişiklik etmək olmaz.", "Məlumat", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
              return true;
          }
          return false;
      }

    [RelayCommand]
    private void AddMenuItem(MenuItem item)
    {
        if (IsOrderClosedOrVoided()) return;
        // Check if item already in order
        var existing = OrderLines.FirstOrDefault(l => l.MenuItemKey == item.MenuItemKey && !l.IsExisting);
        if (existing != null)
        {
            existing.Quantity += Quantity;
            existing.TotalPrice = existing.Quantity * existing.UnitPrice;
        }
        else
        {
            decimal price = item.DefaultUnitPrice ?? 0;
            if (OrderType == 1 && item.DineInPrice > 0) price = item.DineInPrice.Value;
            else if (OrderType == 3 && item.TakeOutPrice > 0) price = item.TakeOutPrice.Value;
            else if (OrderType == 5 && item.DeliveryPrice > 0) price = item.DeliveryPrice.Value;

            OrderLines.Add(new OrderLineViewModel
            {
                MenuItemText = item.MenuItemText ?? "",
                MenuItemKey = item.MenuItemKey ?? "",
                MenuItemID = item.MenuItemID,
                Quantity = Quantity,
                UnitPrice = price,
                TotalPrice = price * Quantity,
                TaxPercent = item.TaxPercent ?? 0,
                MenuItemGroupText = item.MenuItemGroupText ?? "",
                MenumItemCategoryText = item.MenumItemCategoryText ?? ""
            });
        }
        
        Quantity = 1;
        RecalculateTotals();
    }

    [RelayCommand]
    private void IncreaseQuantity()
    {
        if (IsOrderClosedOrVoided()) return;
        if (SelectedOrderLine != null)
        {
            SelectedOrderLine.Quantity++;
            SelectedOrderLine.TotalPrice = SelectedOrderLine.Quantity * SelectedOrderLine.UnitPrice;
            RecalculateTotals();
        }
        else
        {
            Quantity++;
        }
    }

    [RelayCommand]
    private void DecreaseQuantity()
    {
        if (IsOrderClosedOrVoided()) return;
        if (SelectedOrderLine != null)
        {
            if (SelectedOrderLine.Quantity > 1)
            {
                SelectedOrderLine.Quantity--;
                SelectedOrderLine.TotalPrice = SelectedOrderLine.Quantity * SelectedOrderLine.UnitPrice;
                RecalculateTotals();
            }
        }
        else if (Quantity > 1)
        {
            Quantity--;
        }
    }

    [RelayCommand]
    private void SetQuantity(string qty)
    {
        if (IsOrderClosedOrVoided()) return;
        if (int.TryParse(qty, out int q) && q > 0)
        {
            if (SelectedOrderLine != null)
            {
                SelectedOrderLine.Quantity = q;
                SelectedOrderLine.TotalPrice = SelectedOrderLine.Quantity * SelectedOrderLine.UnitPrice;
                RecalculateTotals();
            }
            else
            {
                Quantity = q;
            }
        }
    }

    [RelayCommand]
    private void RemoveOrderLine()
    {
        if (IsOrderClosedOrVoided()) return;
        if (SelectedOrderLine != null)
        {
            OrderLines.Remove(SelectedOrderLine);
            SelectedOrderLine = null;
            RecalculateTotals();
        }
    }

        private decimal _totalPaid = 0;

        private void RecalculateTotals()
        {
            SubTotal = OrderLines.Sum(l => l.TotalPrice);
            TaxTotal = OrderLines.Sum(l => l.TotalPrice * l.TaxPercent / 100);
            
            decimal lineDiscounts = OrderLines.Sum(l => l.DiscountAmount);
            decimal checkDiscount = 0;

            if (_currentOrder != null && _currentOrder.DiscountID > 0)
            {
                if (_currentOrder.DiscountBasisValue == 0 && _currentOrder.DiscountAmountValue > 0)
                {
                    checkDiscount = (SubTotal * _currentOrder.DiscountAmountValue.Value) / 100m;
                }
                else if (_currentOrder.DiscountBasisValue == 1)
                {
                    checkDiscount = _currentOrder.DiscountAmountValue ?? 0;
                }
                
                _currentOrder.DiscountOrderAmount = checkDiscount;
            }

            if (checkDiscount > 0 && lineDiscounts > 0)
            {
                AppliedDiscountName = "ÇEK VE ÜRÜN İNDİRİMİ";
            }
            else if (checkDiscount > 0)
            {
                AppliedDiscountName = "ÇEK İNDİRİMİ";
            }
            else if (lineDiscounts > 0)
            {
                AppliedDiscountName = "ÜRÜN İNDİRİMİ";
            }
            else
            {
                AppliedDiscountName = "İNDİRİM";
            }

            DiscountTotal = lineDiscounts + checkDiscount;
            GrandTotal = SubTotal + TaxTotal - DiscountTotal + Gratuity;
            AmountDue = Math.Max(0, GrandTotal - _totalPaid);
        }

    private async Task SaveOrderInternalAsync()
    {
        IsBusy = true;
        try
        {
            if (_currentOrder == null)
            {
                // Create new order
                _currentOrder = new OrderHeader
                {
                    OrderKey = Guid.NewGuid().ToString().ToUpper(),
                    EditKey = Guid.NewGuid().ToString().ToUpper(),
                    SyncKey = Guid.NewGuid().ToString().ToUpper(),
                    OrderDateTime = DateTime.Now,
                    EmployeeID = SessionManager.EmployeeID,
                    StationID = SessionManager.StationID,
                    BranchID = SessionManager.BranchID,
                    OrderType = OrderType,
                    DineInTableID = TableId > 0 ? TableId : null,
                    GuestNumber = GuestCount,
                    SubTotal = SubTotal,
                    AmountDue = GrandTotal,
                    SalesTaxAmount = TaxTotal,
                    CashGratuity = Gratuity,
                    DiscountTotalAmount = DiscountTotal,
                    OrderStatus = 1,
                    LineDeleted = 0,
                    EmployeeName = SessionManager.EmployeeName,
                    EmployeeKey = SessionManager.EmployeeKey ?? Guid.Empty.ToString().ToUpper(),
                    AddUserID = SessionManager.EmployeeID,
                    AddDateTime = DateTime.Now,
                    EditUserID = SessionManager.EmployeeID,
                    EditDateTime = DateTime.Now
                };
                OrderId = await _orderService.CreateOrderAsync(_currentOrder);
                _currentOrder.OrderID = OrderId;
            }
            else
            {
                // Update existing
                _currentOrder.SubTotal = SubTotal;
                _currentOrder.AmountDue = GrandTotal;
                _currentOrder.SalesTaxAmount = TaxTotal;
                _currentOrder.CashGratuity = Gratuity;
                _currentOrder.DiscountTotalAmount = DiscountTotal;
                _currentOrder.EditUserID = SessionManager.EmployeeID;
                _currentOrder.EditDateTime = DateTime.Now;
                _currentOrder.EditKey = Guid.NewGuid().ToString().ToUpper();
                _currentOrder.SyncKey = Guid.NewGuid().ToString().ToUpper();
                await _orderService.UpdateOrderAsync(_currentOrder);
            }

            // Save order lines
            foreach (var line in OrderLines.Where(l => !l.IsExisting))
            {
                var transaction = new OrderTransaction
                {
                    TransactionKey = Guid.NewGuid().ToString().ToUpper(),
                    OrderKey = _currentOrder.OrderKey,
                    OrderID = OrderId,
                    OrderDateTime = DateTime.Now,
                    TransactionDateTime = DateTime.Now,
                    StationID = SessionManager.StationID,
                    EmployeeID = SessionManager.EmployeeID,
                    MenuItemID = line.MenuItemID,
                    MenuItemKey = string.IsNullOrEmpty(line.MenuItemKey) ? Guid.Empty.ToString().ToUpper() : line.MenuItemKey,
                    MenuItemText = line.MenuItemText,
                    MenuItemUnitPrice = line.UnitPrice,
                    Quantity = line.Quantity,
                    ExtendedPrice = line.TotalPrice,
                    TaxPercent = line.TaxPercent,
                    MenuItemGroupText = line.MenuItemGroupText,
                    MenumItemCategoryText = line.MenumItemCategoryText,
                    TransactionStatus = 1,
                    NotificationStatus = 1,
                    LineDeleted = 0,
                    BranchID = SessionManager.BranchID,
                    AddUserID = SessionManager.EmployeeID,
                    AddDateTime = DateTime.Now,
                    EmployeeName = SessionManager.EmployeeName,
                    EditKey = Guid.NewGuid().ToString().ToUpper(),
                    SyncKey = Guid.NewGuid().ToString().ToUpper()
                };
                await _orderService.AddOrderTransactionAsync(transaction);
                line.IsExisting = true;
            }

            // For TakeOut/Delivery, go to payment
            if (OrderType == 3 || OrderType == 5)
            {
                // TODO: Open payment dialog
            }

            // Sync Database relationships
            if (!string.IsNullOrEmpty(_currentOrder?.OrderKey))
            {
                await _orderService.FinalizeOrderProcessingAsync(_currentOrder.OrderKey);
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task SaveOrder()
    {
        if (OrderLines.Count == 0) return;

        await SaveOrderInternalAsync();

        // Navigate back
        _navigationService.NavigateTo<TablePlanViewModel>();
    }

    [RelayCommand]
    private async Task PrintGuestCheck()
    {
        if (_currentOrder != null)
        {
            _currentOrder.GuestCheckPrinted = true;
            _currentOrder.GuestCheckPrintCount = (_currentOrder.GuestCheckPrintCount ?? 0) + 1;
            await _orderService.UpdateOrderAsync(_currentOrder);
            // TODO: Actually print the check
        }
    }

    [RelayCommand]
    private async Task OpenCheckDiscount()
    {
        if (IsOrderClosedOrVoided()) return;
        if (OrderLines.Count == 0)
        {
            System.Windows.MessageBox.Show("Zəhmət olmasa öncə məhsul əlavə edin.", "Xəta", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            return;
        }

        if (_currentOrder == null || OrderLines.Any(l => !l.IsExisting))
        {
            await SaveOrderInternalAsync();
        }

        if (_currentOrder == null || string.IsNullOrEmpty(_currentOrder.OrderKey))
        {
            return;
        }

        var vm = new CheckDiscountViewModel(_discountService);
        await vm.InitializeAsync();
        
        var view = new NUSHPOS.Views.CheckDiscountView { DataContext = vm };
        vm.RequestClose = () => view.Close();
        
        view.ShowDialog();

        if (vm.SelectedDiscount != null)
        {
            IsBusy = true;
            try
            {
                // If the selected discount is already applied, remove it
                if (_currentOrder.DiscountKey == vm.SelectedDiscount.DiscountKey)
                {
                    _currentOrder.DiscountID = 0;
                    _currentOrder.DiscountKey = null;
                    _currentOrder.DiscountAmountValue = 0;
                    _currentOrder.DiscountBasisValue = 0;
                    _currentOrder.DiscountOrderAmount = 0;
                    _currentOrder.DiscountTotalAmount = 0;
                    
                    AppliedDiscountName = "ÇEK ENDİRİMİ";
                    RecalculateTotals();
                    
                    // Save this removal immediately without navigating away
                    await SaveOrderInternalAsync();
                    return;
                }

                var result = await _discountService.ApplyCheckDiscountAsync(
                    _currentOrder.OrderKey, 
                    vm.SelectedDiscount.DiscountKey, 
                    SessionManager.EmployeeID, 
                    SessionManager.StationID, 
                    _currentOrder.EditKey ?? Guid.Empty.ToString());

                if (result.Success)
                {
                    // Refresh from DB
                    var oldOrderKey = _currentOrder.OrderKey;
                    _isEditMode = true;
                    OrderKey = oldOrderKey;
                    OrderLines.Clear();
                    await LoadDataAsync();
                }
                else
                {
                    System.Windows.MessageBox.Show(result.Message, "Xəta", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                }
            }
            finally
            {
                IsBusy = false;
            }
        }
    }

    [RelayCommand]
    private void GoBack()
    {
        _navigationService.NavigateTo<TablePlanViewModel>();
    }

    [RelayCommand]
    private async Task OpenItemDiscount()
    {
        if (IsOrderClosedOrVoided()) return;

        if (SelectedOrderLine == null || !SelectedOrderLine.IsExisting)
        {
            System.Windows.MessageBox.Show("Zəhmət olmasa öncə məhsulu sifarişə əlavə edib yadda saxlayın.", "Məlumat", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            return;
        }

        var vm = new CheckDiscountViewModel(_discountService);
        await vm.InitializeAsync();
        var view = new NUSHPOS.Views.CheckDiscountView { DataContext = vm };
        vm.RequestClose = () => view.Close();
        bool? result = view.ShowDialog();
        
        if (vm.SelectedDiscount != null)
        {
            try
            {
                IsBusy = true;
                
                var transactions = await _orderService.GetOrderTransactionsByKeyAsync(_currentOrder.OrderKey);
                var realTx = transactions.FirstOrDefault(t => t.AutoID == SelectedOrderLine.AutoID);
                if (realTx == null) return;
                
                var disc = vm.SelectedDiscount;
                
                if (realTx.DiscountKey == disc.DiscountKey)
                {
                    realTx.DiscountID = 0;
                    realTx.DiscountKey = null;
                    realTx.DiscountAmountValue = 0;
                    realTx.DiscountBasisValue = 0;
                    realTx.UsedDiscountName = null;
                    realTx.DiscountUserName = null;
                    realTx.DiscountLineAmount = 0;
                    realTx.DiscountTotalAmount = 0;
                }
                else
                {
                    realTx.DiscountID = disc.DiscountID;
                    realTx.DiscountKey = disc.DiscountKey;
                    realTx.DiscountAmountValue = disc.DiscountAmount;
                    realTx.DiscountBasisValue = disc.DiscountBasis;
                    realTx.UsedDiscountName = disc.DiscountText;
                    realTx.DiscountUserName = SessionManager.EmployeeName;

                    decimal discountLineAmt = 0;
                    if (disc.DiscountBasis == 0 && disc.DiscountAmount > 0)
                        discountLineAmt = (realTx.ExtendedPrice ?? 0) * (disc.DiscountAmount.Value / 100m);
                    else if (disc.DiscountBasis == 1)
                        discountLineAmt = disc.DiscountAmount ?? 0;
                        
                    realTx.DiscountLineAmount = discountLineAmt;
                    realTx.DiscountTotalAmount = discountLineAmt;
                }

                realTx.EditUserID = SessionManager.EmployeeID;
                realTx.EditDateTime = DateTime.Now;

                await _orderService.UpdateOrderTransactionAsync(realTx);
                
                SelectedOrderLine.DiscountAmount = realTx.DiscountLineAmount ?? 0;
                SelectedOrderLine.UsedDiscountName = realTx.UsedDiscountName ?? "";
                
                RecalculateTotals();
                await SaveOrderInternalAsync();

                await _accessLogService.InsertAccessLogAsync(
                    branchId: SessionManager.BranchID,
                    stationId: SessionManager.StationID,
                    employeeId: SessionManager.EmployeeID,
                    actionName: "ÜRÜN İNDİRİMİ",
                    wrongPassword: "",
                    additionalInfo: $"{SelectedOrderLine.MenuItemText} üçün indirim",
                    isSuccess: true,
                    orderKey: _currentOrder.OrderKey,
                    transactionKey: realTx.TransactionKey
                );
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Məhsul endirimi zamanı xəta: {ex.Message}", "Xəta", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }

    [RelayCommand]
    private async Task OpenPayment(PaymentMethod paymentMethod)
    {
        if (IsOrderClosedOrVoided()) return;
        if (OrderLines.Count == 0)
        {
            System.Windows.MessageBox.Show("Zəhmət olmasa öncə məhsul əlavə edin.", "Məlumat", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            return;
        }

        if (_currentOrder == null || OrderLines.Any(l => !l.IsExisting))
        {
            await SaveOrderInternalAsync();
        }

        if (_currentOrder == null || string.IsNullOrEmpty(_currentOrder.OrderKey))
            return;

        // Open the Payment Dialog
        var vm = new PaymentDialogViewModel(_paymentService, _orderService, _currentOrder, paymentMethod, GrandTotal);
        var view = new NUSHPOS.Views.Dialogs.PaymentDialog { DataContext = vm };
        vm.RequestClose = () => view.Close();
        bool? result = view.ShowDialog();

        if (vm.IsPaymentCompleted)
        {
            // Payment completed successfully, order is closed. Go back to Table Plan.
            _navigationService.NavigateTo<TablePlanViewModel>();
        }
    }

}

public partial class OrderLineViewModel : ObservableObject
{
    [ObservableProperty] private int _autoID;
    [ObservableProperty] private int _menuItemID;
    [ObservableProperty] private string _menuItemText = "";
    [ObservableProperty] private string _menuItemKey = "";
    [ObservableProperty] private decimal _quantity = 1;
    [ObservableProperty] private decimal _unitPrice;
    [ObservableProperty] private decimal _totalPrice;
    [ObservableProperty] private decimal _discountAmount;
    [ObservableProperty] private string _usedDiscountName = "";
    [ObservableProperty] private decimal _taxPercent;
    [ObservableProperty] private string _menuItemGroupText = "";
    [ObservableProperty] private string _menumItemCategoryText = "";
    [ObservableProperty] private bool _isExisting;
}
