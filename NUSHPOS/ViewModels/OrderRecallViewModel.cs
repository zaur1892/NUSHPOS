using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dapper;
using NUSHPOS.Helpers;
using NUSHPOS.Models;
using NUSHPOS.Services;
using NUSHPOS.ViewModels.Base;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace NUSHPOS.ViewModels
{
    public partial class OrderRecallViewModel : ViewModelBase
    {
        private readonly DatabaseService _databaseService;
        private readonly NavigationService _navigationService;
        private DispatcherTimer? _timer;

        [ObservableProperty]
        private string _currentDateTime = DateTime.Now.ToString("dd.MM.yyyy HH:mm");

        [ObservableProperty]
        private string _employeeName = SessionManager.EmployeeName;

        [ObservableProperty]
        private ObservableCollection<OrderRecallItem> _orders = new();

        [ObservableProperty]
        private OrderRecallItem? _selectedOrder;

        [ObservableProperty]
        private DateTime _selectedDate = DateTime.Today;

        private int _currentFilterMode = 0; // 0=Open, 1=All, 2=Unprinted, 3=My
        private int _currentOrderTypeFilter = 0; // 0=All, 1=MASA, 2=ADA ÇEK, 3=AL GÖTÜR, 4=TƏZGAH, 5=PAKET
        private List<OrderRecallItem> _loadedOrders = new();

        partial void OnSelectedDateChanged(DateTime value)
        {
            _ = ReloadCurrentModeAsync();
        }

        [RelayCommand]
        private void SetOrderTypeFilter(string filterStr)
        {
            if (int.TryParse(filterStr, out int filter))
            {
                _currentOrderTypeFilter = filter;
                ApplyFilter();
            }
        }

        private void ApplyFilter()
        {
            if (_currentOrderTypeFilter == 0)
            {
                Orders = new ObservableCollection<OrderRecallItem>(_loadedOrders);
            }
            else
            {
                // Note: If "ADA ÇEK" requires specific logic, we map it to 6 for now or just standard types
                Orders = new ObservableCollection<OrderRecallItem>(_loadedOrders.Where(o => o.OrderType == _currentOrderTypeFilter));
            }
            TotalOrderCount = Orders.Count;
        }

        private async Task ReloadCurrentModeAsync()
        {
            if (_currentFilterMode == 1)
                await LoadAllChecksAsync();
            else if (_currentFilterMode == 2)
                await LoadUnprintedChecksAsync();
            else if (_currentFilterMode == 0)
                await LoadOpenChecksAsync();
            else if (_currentFilterMode == 3)
                await LoadMyChecksAsync();
        }

        [ObservableProperty]
        private string _searchReceiptNo = "";

        // Total orders count to show at the bottom
        [ObservableProperty]
        private int _totalOrderCount = 0;

        public OrderRecallViewModel(DatabaseService databaseService, NavigationService navigationService)
        {
            _databaseService = databaseService;
            _navigationService = navigationService;
            
            StartClock();
            
            // Load "AÇIK ÇEKLER" (Open Checks) by default
            _ = LoadOpenChecksAsync();
        }

        private void StartClock()
        {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(30);
            _timer.Tick += (s, e) => CurrentDateTime = DateTime.Now.ToString("dd.MM.yyyy HH:mm");
            _timer.Start();
        }

        [RelayCommand]
        private async Task LoadOpenChecksAsync()
        {
            _currentFilterMode = 0;
            var sql = @"
                SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
                SELECT 
                    OrderHeaders.AutoID, 
                    OrderHeaders.OrderID, 
                    OrderHeaders.ReceiptNo,
                    CAST(OrderHeaders.OrderKey AS NVARCHAR(50)) AS OrderKey, 
                    OrderHeaders.OrderDateTime, 
                    CASE WHEN ISNULL(OrderHeaders.AmountDue, 0.0) = 0 THEN (ISNULL(OrderHeaders.AmountDue, 0.0) + ISNULL((SELECT SUM(AmountPaid) FROM OrderPayments WHERE OrderPayments.OrderKey = OrderHeaders.OrderKey AND ISNULL(OrderPayments.LineDeleted, 0) = 0), 0.0)) ELSE ISNULL(OrderHeaders.AmountDue, 0.0) END AS GrandTotal,
                    OrderHeaders.OrderStatus, 
                    OrderHeaders.OrderType,
                    ISNULL(OrderHeaders.EmployeeName, '') AS EmployeeName,
                    ISNULL(OrderHeaders.DineInTableName, '') AS DineInTableName,
                    ISNULL(OrderHeaders.CustomerName, '') AS CustomerName,
                    ISNULL(OrderHeaders.OrderPhone, '') AS OrderPhone,
                    ISNULL(OrderHeaders.BarTabName, '') AS BarTabName,
                    (CASE OrderHeaders.OrderType 
                        WHEN 1 THEN 'MASA' 
                        WHEN 2 THEN 'BAR SATIŞI' 
                        WHEN 3 THEN 'AL GÖTÜR' 
                        WHEN 4 THEN 'TEZGAH SATIŞI' 
                        WHEN 5 THEN 'PAKET SATIŞI' 
                        WHEN 66 THEN 'İADE' 
                        ELSE '-' 
                     END) AS OrderTypeName,
                    (CASE OrderHeaders.OrderStatus 
                        WHEN 1 THEN 'AÇIQ' 
                        WHEN 2 THEN 'BAĞLI' 
                        WHEN 3 THEN 'İPTAL' 
                        ELSE '-' 
                     END) AS OrderStatusName
                FROM OrderHeaders WITH (NOLOCK) 
                WHERE OrderHeaders.OrderStatus = 1 
                  AND (ISNULL(OrderHeaders.DeleteReason, '') NOT LIKE '%ayrıldı%')  
                ORDER BY OrderHeaders.OrderDateTime DESC;";

            await LoadDataAsync(sql);
        }

        [RelayCommand]
        private async Task LoadAllChecksAsync()
        {
            _currentFilterMode = 1;
            var start = SelectedDate.Date.ToString("yyyy-MM-dd 06:00:00");
            var end = SelectedDate.Date.AddDays(1).ToString("yyyy-MM-dd 05:59:59");

            var sql = $@"
                SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
                SELECT 
                    OrderHeaders.AutoID, 
                    OrderHeaders.OrderID, 
                    OrderHeaders.ReceiptNo,
                    CAST(OrderHeaders.OrderKey AS NVARCHAR(50)) AS OrderKey, 
                    OrderHeaders.OrderDateTime, 
                    CASE WHEN ISNULL(OrderHeaders.AmountDue, 0.0) = 0 THEN (ISNULL(OrderHeaders.AmountDue, 0.0) + ISNULL((SELECT SUM(AmountPaid) FROM OrderPayments WHERE OrderPayments.OrderKey = OrderHeaders.OrderKey AND ISNULL(OrderPayments.LineDeleted, 0) = 0), 0.0)) ELSE ISNULL(OrderHeaders.AmountDue, 0.0) END AS GrandTotal,
                    OrderHeaders.OrderStatus, 
                    OrderHeaders.OrderType,
                    ISNULL(OrderHeaders.EmployeeName, '') AS EmployeeName,
                    ISNULL(OrderHeaders.DineInTableName, '') AS DineInTableName,
                    ISNULL(OrderHeaders.CustomerName, '') AS CustomerName,
                    ISNULL(OrderHeaders.OrderPhone, '') AS OrderPhone,
                    ISNULL(OrderHeaders.BarTabName, '') AS BarTabName,
                    (CASE OrderHeaders.OrderType 
                        WHEN 1 THEN 'MASA' 
                        WHEN 2 THEN 'BAR SATIŞI' 
                        WHEN 3 THEN 'AL GÖTÜR' 
                        WHEN 4 THEN 'TEZGAH SATIŞI' 
                        WHEN 5 THEN 'PAKET SATIŞI' 
                        WHEN 66 THEN 'İADE' 
                        ELSE '-' 
                     END) AS OrderTypeName,
                    (CASE OrderHeaders.OrderStatus 
                        WHEN 1 THEN 'AÇIQ' 
                        WHEN 2 THEN 'BAĞLI' 
                        WHEN 3 THEN 'İPTAL' 
                        ELSE '-' 
                     END) AS OrderStatusName
                FROM OrderHeaders WITH (NOLOCK) 
                WHERE OrderHeaders.OrderDateTime >= '{start}' 
                  AND OrderHeaders.OrderDateTime < '{end}'  
                  AND (ISNULL(OrderHeaders.DeleteReason, '') NOT LIKE '%ayrıldı%')  
                ORDER BY OrderHeaders.OrderDateTime DESC;";

            await LoadDataAsync(sql);
        }

        [RelayCommand]
        private async Task LoadUnprintedChecksAsync()
        {
            _currentFilterMode = 2;
            var start = SelectedDate.Date.ToString("yyyy-MM-dd 06:00:00");
            var end = SelectedDate.Date.AddDays(1).ToString("yyyy-MM-dd 05:59:59");

            var sql = $@"
                SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
                SELECT 
                    OrderHeaders.AutoID, 
                    OrderHeaders.OrderID, 
                    OrderHeaders.ReceiptNo,
                    CAST(OrderHeaders.OrderKey AS NVARCHAR(50)) AS OrderKey, 
                    OrderHeaders.OrderDateTime, 
                    CASE WHEN ISNULL(OrderHeaders.AmountDue, 0.0) = 0 THEN (ISNULL(OrderHeaders.AmountDue, 0.0) + ISNULL((SELECT SUM(AmountPaid) FROM OrderPayments WHERE OrderPayments.OrderKey = OrderHeaders.OrderKey AND ISNULL(OrderPayments.LineDeleted, 0) = 0), 0.0)) ELSE ISNULL(OrderHeaders.AmountDue, 0.0) END AS GrandTotal,
                    OrderHeaders.OrderStatus, 
                    OrderHeaders.OrderType,
                    ISNULL(OrderHeaders.EmployeeName, '') AS EmployeeName,
                    ISNULL(OrderHeaders.DineInTableName, '') AS DineInTableName,
                    ISNULL(OrderHeaders.CustomerName, '') AS CustomerName,
                    ISNULL(OrderHeaders.OrderPhone, '') AS OrderPhone,
                    ISNULL(OrderHeaders.BarTabName, '') AS BarTabName,
                    (CASE OrderHeaders.OrderType 
                        WHEN 1 THEN 'MASA' 
                        WHEN 2 THEN 'BAR SATIŞI' 
                        WHEN 3 THEN 'AL GÖTÜR' 
                        WHEN 4 THEN 'TEZGAH SATIŞI' 
                        WHEN 5 THEN 'PAKET SATIŞI' 
                        WHEN 66 THEN 'İADE' 
                        ELSE '-' 
                     END) AS OrderTypeName,
                    (CASE OrderHeaders.OrderStatus 
                        WHEN 1 THEN 'AÇIQ' 
                        WHEN 2 THEN 'BAĞLI' 
                        WHEN 3 THEN 'İPTAL' 
                        ELSE '-' 
                     END) AS OrderStatusName
                FROM OrderHeaders WITH (NOLOCK) 
                WHERE OrderHeaders.OrderDateTime >= '{start}' 
                  AND OrderHeaders.OrderDateTime < '{end}'  
                  AND (ISNULL(OrderHeaders.DeleteReason, '') NOT LIKE '%ayrıldı%')  
                  AND (ISNULL(OrderHeaders.FiscalStatus, 0) = 0)  
                ORDER BY OrderHeaders.OrderDateTime DESC;";

            await LoadDataAsync(sql);
        }

        [RelayCommand]
        private async Task LoadMyChecksAsync()
        {
            _currentFilterMode = 3;
            var sql = $@"
                SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
                SELECT 
                    OrderHeaders.AutoID, 
                    OrderHeaders.OrderID, 
                    OrderHeaders.ReceiptNo,
                    CAST(OrderHeaders.OrderKey AS NVARCHAR(50)) AS OrderKey, 
                    OrderHeaders.OrderDateTime, 
                    CASE WHEN ISNULL(OrderHeaders.AmountDue, 0.0) = 0 THEN (ISNULL(OrderHeaders.AmountDue, 0.0) + ISNULL((SELECT SUM(AmountPaid) FROM OrderPayments WHERE OrderPayments.OrderKey = OrderHeaders.OrderKey AND ISNULL(OrderPayments.LineDeleted, 0) = 0), 0.0)) ELSE ISNULL(OrderHeaders.AmountDue, 0.0) END AS GrandTotal,
                    OrderHeaders.OrderStatus, 
                    OrderHeaders.OrderType,
                    ISNULL(OrderHeaders.EmployeeName, '') AS EmployeeName,
                    ISNULL(OrderHeaders.DineInTableName, '') AS DineInTableName,
                    ISNULL(OrderHeaders.CustomerName, '') AS CustomerName,
                    ISNULL(OrderHeaders.OrderPhone, '') AS OrderPhone,
                    ISNULL(OrderHeaders.BarTabName, '') AS BarTabName,
                    (CASE OrderHeaders.OrderType 
                        WHEN 1 THEN 'MASA' 
                        WHEN 2 THEN 'BAR SATIŞI' 
                        WHEN 3 THEN 'AL GÖTÜR' 
                        WHEN 4 THEN 'TEZGAH SATIŞI' 
                        WHEN 5 THEN 'PAKET SATIŞI' 
                        WHEN 66 THEN 'İADE' 
                        ELSE '-' 
                     END) AS OrderTypeName,
                    (CASE OrderHeaders.OrderStatus 
                        WHEN 1 THEN 'AÇIQ' 
                        WHEN 2 THEN 'BAĞLI' 
                        WHEN 3 THEN 'İPTAL' 
                        ELSE '-' 
                     END) AS OrderStatusName
                FROM OrderHeaders WITH (NOLOCK) 
                WHERE OrderHeaders.OrderStatus = 1 
                  AND OrderHeaders.EmployeeID = {SessionManager.EmployeeID} 
                  AND (ISNULL(OrderHeaders.DeleteReason, '') NOT LIKE '%ayrıldı%')  
                ORDER BY OrderHeaders.OrderDateTime DESC;";

            await LoadDataAsync(sql);
        }

        private async Task LoadDataAsync(string sql)
        {
            using var connection = _databaseService.CreateConnection();
            var results = (await connection.QueryAsync<OrderRecallItem>(sql)).ToList();

            // Also load payments for these orders to show payment method if closed
            var orderKeys = results.Select(r => r.OrderKey).Distinct().ToList();
            if (orderKeys.Any())
            {
                var keysSql = string.Join("','", orderKeys);
                var paymentSql = $@"
                    SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
                    SELECT CAST(OrderKey AS NVARCHAR(50)) AS OrderKey, ISNULL(PaymentMethodName, '') AS PaymentMethodName 
                    FROM OrderPayments WITH (NOLOCK) 
                    WHERE OrderKey IN ('{keysSql}') AND ISNULL(LineDeleted, 0) = 0;";
                
                var payments = await connection.QueryAsync(paymentSql);
                var paymentDict = payments.GroupBy(p => (string)p.OrderKey).ToDictionary(g => g.Key, g => string.Join(", ", g.Select(p => (string)p.PaymentMethodName)));

                foreach (var item in results)
                {
                    if (paymentDict.TryGetValue(item.OrderKey, out var pName))
                    {
                        item.PaymentMethodName = pName;
                    }
                }
            }

            _loadedOrders = results;
            ApplyFilter();
        }

        [RelayCommand]
        private void GoBack()
        {
            _navigationService.NavigateTo<MainScreenViewModel>();
        }

        [RelayCommand]
        private void OpenCheck()
        {
            if (SelectedOrder != null)
            {
                _navigationService.NavigateTo<SaleScreenViewModel>(new 
                { 
                    OrderId = SelectedOrder.OrderID,
                    OrderKey = SelectedOrder.OrderKey,
                    OrderType = SelectedOrder.OrderType,
                    CustomerName = SelectedOrder.CustomerName ?? "",
                    OrderPhone = SelectedOrder.OrderPhone ?? "",
                    BarTabName = SelectedOrder.BarTabName ?? "",
                    TableName = SelectedOrder.DineInTableName ?? ""
                });
            }
        }
        
        [RelayCommand]
        private void Search()
        {
            if (!string.IsNullOrEmpty(SearchReceiptNo))
            {
                var order = Orders.FirstOrDefault(o => o.ReceiptNo != null && o.ReceiptNo.Contains(SearchReceiptNo));
                if (order != null)
                {
                    SelectedOrder = order;
                    OpenCheck();
                }
            }
        }

        [RelayCommand]
        private void FilterAll()
        {
            // The "TÜMÜ" button filters the current list or does it just show everything in the list?
            // "Bütün çekləri göstərir" -> just clear filter if there was any. We can just keep the list as is.
        }

        [RelayCommand]
        private void FilterMasa()
        {
            // "MASA" -> Masa satış və sairə
            var filtered = Orders.Where(o => o.OrderType == 1).ToList();
            Orders = new ObservableCollection<OrderRecallItem>(filtered);
            TotalOrderCount = Orders.Count;
        }

        [RelayCommand]
        private void AppendNumpad(string number)
        {
            SearchReceiptNo += number;
        }

        [RelayCommand]
        private void ClearSearch()
        {
            SearchReceiptNo = "";
        }
        
        [RelayCommand]
        private void DeleteLastChar()
        {
            if (SearchReceiptNo.Length > 0)
                SearchReceiptNo = SearchReceiptNo.Substring(0, SearchReceiptNo.Length - 1);
        }

        [RelayCommand]
        private void PreviousDay()
        {
            SelectedDate = SelectedDate.AddDays(-1);
        }

        [RelayCommand]
        private void NextDay()
        {
            SelectedDate = SelectedDate.AddDays(1);
        }
    }
}
