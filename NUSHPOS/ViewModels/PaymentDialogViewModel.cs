using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NUSHPOS.Helpers;
using NUSHPOS.Models;
using NUSHPOS.Services;
using NUSHPOS.ViewModels.Base;
using System;
using System.Threading.Tasks;

namespace NUSHPOS.ViewModels;

public partial class PaymentDialogViewModel : ViewModelBase
{
    private readonly PaymentService _paymentService;
    private readonly OrderService _orderService;
    private readonly OrderHeader _order;
    private readonly PaymentMethod _paymentMethod;

    [ObservableProperty] private string _amountDueString = "0,00";
    [ObservableProperty] private string _amountTenderedString = "0,00";
    [ObservableProperty] private decimal _amountDue;
    
    // Commands for numpad
    public RelayCommand<string> NumpadCommand { get; }
    public RelayCommand ClearCommand { get; }
    public RelayCommand<string> QuickAmountCommand { get; }
    public RelayCommand ExactAmountCommand { get; }
    public RelayCommand CancelCommand { get; }
    public IAsyncRelayCommand ProcessPaymentCommand { get; }
    
    public Action? RequestClose { get; set; }
    public bool IsPaymentCompleted { get; private set; }

    public PaymentDialogViewModel(PaymentService paymentService, OrderService orderService, OrderHeader order, PaymentMethod paymentMethod, decimal amountDue)
    {
        _paymentService = paymentService;
        _orderService = orderService;
        _order = order;
        _paymentMethod = paymentMethod;

        AmountDue = amountDue;
        AmountDueString = AmountDue.ToString("N2");

        NumpadCommand = new RelayCommand<string>(OnNumpad);
        ClearCommand = new RelayCommand(OnClear);
        QuickAmountCommand = new RelayCommand<string>(OnQuickAmount);
        ExactAmountCommand = new RelayCommand(OnExactAmount);
        CancelCommand = new RelayCommand(() => RequestClose?.Invoke());
        ProcessPaymentCommand = new AsyncRelayCommand(OnProcessPaymentAsync);
    }

    private void OnNumpad(string? value)
    {
        if (value == null) return;
        
        if (AmountTenderedString == "0,00" || AmountTenderedString == "0")
        {
            if (value == ",") AmountTenderedString = "0,";
            else AmountTenderedString = value;
        }
        else
        {
            // Prevent multiple commas
            if (value == "," && AmountTenderedString.Contains(",")) return;
            
            // Limit to 2 decimal places
            if (AmountTenderedString.Contains(","))
            {
                var parts = AmountTenderedString.Split(',');
                if (parts.Length > 1 && parts[1].Length >= 2) return;
            }
            
            AmountTenderedString += value;
        }
    }

    private void OnClear()
    {
        if (AmountTenderedString.Length > 1)
        {
            AmountTenderedString = AmountTenderedString.Substring(0, AmountTenderedString.Length - 1);
            if (AmountTenderedString.EndsWith(","))
            {
                AmountTenderedString = AmountTenderedString.Substring(0, AmountTenderedString.Length - 1);
            }
        }
        else
        {
            AmountTenderedString = "0,00";
        }
    }

    private void OnQuickAmount(string? amountStr)
    {
        if (decimal.TryParse(amountStr, out decimal amount))
        {
            AmountTenderedString = amount.ToString("N2");
        }
    }

    private void OnExactAmount()
    {
        AmountTenderedString = AmountDue.ToString("N2");
        _ = OnProcessPaymentAsync();
    }

    private async Task OnProcessPaymentAsync()
    {
        if (!decimal.TryParse(AmountTenderedString, out decimal tenderedAmount))
            return;

        if (tenderedAmount <= 0)
        {
            // If they didn't type anything, assume they pay exact amount
            tenderedAmount = AmountDue;
            AmountTenderedString = tenderedAmount.ToString("N2");
        }

        decimal changeAmount = 0;
        decimal paidAmount = tenderedAmount;
        
        if (tenderedAmount > AmountDue)
        {
            changeAmount = tenderedAmount - AmountDue;
            paidAmount = AmountDue;
        }

        IsBusy = true;
        try
        {
            // Add payment record
            var payment = new OrderPayment
            {
                PaymentKey = Guid.NewGuid().ToString().ToUpper(),
                OrderID = _order.OrderID,
                OrderKey = _order.OrderKey,
                StationID = SessionManager.StationID,
                RegisterSessionID = SessionManager.RegisterSessionID,
                PaymentDateTime = DateTime.Now,
                EmployeeID = SessionManager.EmployeeID,
                PaymentMethodID = _paymentMethod.PaymentMethodID,
                AmountTendered = tenderedAmount,
                AmountPaid = paidAmount,
                AmountChange = changeAmount,
                LineDeleted = 0,
                BranchID = SessionManager.BranchID,
                AddUserID = SessionManager.EmployeeID,
                AddDateTime = DateTime.Now,
                EmployeeName = SessionManager.EmployeeName,
                PaymentMethodName = _paymentMethod.PaymentName,
                IsAccountSale = _paymentMethod.IsAccountSale ?? 0,
                IsAccountPayment = _paymentMethod.IsAccountPayment ?? 0,
                PaymentMethodKey = Guid.Empty.ToString().ToUpper(), // Could be populated if PaymentMethod has a key
                EmployeeKey = SessionManager.EmployeeKey ?? Guid.Empty.ToString().ToUpper()
            };
            
            await _paymentService.AddPaymentAsync(payment);
            
            // Update Order Header
            _order.OrderStatus = 4; // Paid / Closed
            _order.EditUserID = SessionManager.EmployeeID;
            _order.EditDateTime = DateTime.Now;
            _order.EditKey = Guid.NewGuid().ToString().ToUpper();
            _order.SyncKey = Guid.NewGuid().ToString().ToUpper();
            
            await _orderService.UpdateOrderAsync(_order);

            IsPaymentCompleted = true;

            // Open Change Dialog if change > 0
            if (changeAmount > 0)
            {
                var changeVm = new ChangeDialogViewModel(changeAmount);
                var changeView = new Views.Dialogs.ChangeDialog { DataContext = changeVm };
                changeVm.RequestClose = () => changeView.Close();
                changeView.ShowDialog();
            }

            RequestClose?.Invoke();
        }
        finally
        {
            IsBusy = false;
        }
    }
}
