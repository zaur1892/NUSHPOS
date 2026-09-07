using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NUSHPOS.Helpers;
using NUSHPOS.Models;
using NUSHPOS.Services;
using NUSHPOS.ViewModels.Base;
using NUSHPOS.Views.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace NUSHPOS.ViewModels;

public enum CashierOutStep
{
    OpenOrdersWarning,
    SessionSelection,
    MainForm,
    ConfirmPrompt,
    DiscrepancyReason,
    SuccessInfo
}

public partial class CashierOutDialogViewModel : ViewModelBase
{
    private readonly RegisterSessionService _registerSessionService;
    private readonly FastReportService _fastReportService;
    private readonly DispatcherTimer _confirmTimer;
    private readonly DispatcherTimer _successTimer;

    [ObservableProperty] private CashierOutStep _currentStep = CashierOutStep.MainForm;
    [ObservableProperty] private int _openOrdersCount = 0;
    [ObservableProperty] private int _confirmCountdown = 30;
    [ObservableProperty] private int _successCountdown = 10;
    [ObservableProperty] private bool _showSystemAmounts = true;
    [ObservableProperty] private string _stationName = $"Terminal {SessionManager.StationID}";
    [ObservableProperty] private string _employeeName = SessionManager.EmployeeName;
    [ObservableProperty] private string _signInTime = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
    [ObservableProperty] private decimal _startAmount = 0m;
    [ObservableProperty] private int _selectedSessionId = SessionManager.RegisterSessionID ?? 0;

    // Payment categories
    [ObservableProperty] private ObservableCollection<SessionPaymentSummary> _cashPayments = new();
    [ObservableProperty] private ObservableCollection<SessionPaymentSummary> _cardPayments = new();
    [ObservableProperty] private ObservableCollection<SessionPaymentSummary> _otherPayments = new();
    [ObservableProperty] private ObservableCollection<Expense> _expenses = new();

    // Totals
    [ObservableProperty] private decimal _totalCashCounted;
    [ObservableProperty] private decimal _totalCashExpected;
    [ObservableProperty] private decimal _totalCardCounted;
    [ObservableProperty] private decimal _totalCardExpected;
    [ObservableProperty] private decimal _totalOtherCounted;
    [ObservableProperty] private decimal _totalOtherExpected;
    [ObservableProperty] private decimal _totalExpenses;
    [ObservableProperty] private decimal _grandTotalCounted;
    [ObservableProperty] private decimal _grandTotalExpected;
    [ObservableProperty] private decimal _discrepancyAmount;
    [ObservableProperty] private string _discrepancyStatusText = "Fərq yoxdur (0.00 ₼)";
    [ObservableProperty] private string _discrepancyReasonText = "FƏRQDƏN MÜDİRİN XƏBƏRİ VAR!";

    // Currently selected payment summary for editing in Touch Numpad
    [ObservableProperty] private SessionPaymentSummary? _selectedPaymentItem;
    [ObservableProperty] private string _numpadDisplay = "0.00";
    [ObservableProperty] private bool _isNumpadOpen = false;

    public Action<bool>? RequestClose { get; set; }

    public CashierOutDialogViewModel(
        RegisterSessionService registerSessionService,
        FastReportService fastReportService)
    {
        _registerSessionService = registerSessionService;
        _fastReportService = fastReportService;
        Title = "Kassir Çıxış Forması";

        _confirmTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        _confirmTimer.Tick += ConfirmTimer_Tick;

        _successTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        _successTimer.Tick += SuccessTimer_Tick;
    }

    public async Task InitializeAsync()
    {
        IsBusy = true;
        try
        {
            // 1. Açıq sifarişlərin yoxlanması
            OpenOrdersCount = await _registerSessionService.GetOpenOrdersCountAsync();
            if (OpenOrdersCount > 0)
            {
                CurrentStep = CashierOutStep.OpenOrdersWarning;
                return;
            }

            // 2. Aktiv sessiya məlumatı
            var activeSession = await _registerSessionService.GetActiveStationSessionAsync(SessionManager.StationID);
            if (activeSession != null)
            {
                SelectedSessionId = activeSession.RegisterSessionID;
                StartAmount = activeSession.RegisterStartAmount ?? 0m;
                SignInTime = activeSession.SignInDateTime?.ToString("dd.MM.yyyy HH:mm:ss") ?? DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
            }

            if (SelectedSessionId <= 0 && SessionManager.RegisterSessionID.HasValue)
            {
                SelectedSessionId = SessionManager.RegisterSessionID.Value;
            }

            // 3. Ödənişlər və xərclər
            await LoadSessionDataAsync(SelectedSessionId);
            CurrentStep = CashierOutStep.MainForm;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error initializing CashierOut: {ex}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    public async Task LoadSessionDataAsync(int sessionId)
    {
        var allPayments = await _registerSessionService.GetSessionPaymentTotalsAsync(sessionId);
        var expensesList = await _registerSessionService.GetSessionExpensesAsync(sessionId);

        Expenses = new ObservableCollection<Expense>(expensesList);
        TotalExpenses = expensesList.Sum(e => e.ExpenseAmount ?? 0m);

        // Group by PaymentTypeID: 1 = Cash, 2 = Cards, others = 3,4,5
        var cashList = allPayments.Where(p => p.PaymentTypeID == 1).ToList();
        var cardList = allPayments.Where(p => p.PaymentTypeID == 2).ToList();
        var otherList = allPayments.Where(p => p.PaymentTypeID != 1 && p.PaymentTypeID != 2).ToList();

        // Default counted amounts to 0
        foreach (var item in allPayments)
        {
            item.CountedAmount = 0m;
            item.PropertyChanged += (s, e) => RecalculateTotals();
        }

        CashPayments = new ObservableCollection<SessionPaymentSummary>(cashList);
        CardPayments = new ObservableCollection<SessionPaymentSummary>(cardList);
        OtherPayments = new ObservableCollection<SessionPaymentSummary>(otherList);

        RecalculateTotals();
    }

    public void RecalculateTotals()
    {
        TotalCashCounted = CashPayments.Sum(c => c.CountedAmount);
        TotalCashExpected = CashPayments.Sum(c => c.AmountPaid) + StartAmount; // Includes register start cash

        TotalCardCounted = CardPayments.Sum(c => c.CountedAmount);
        TotalCardExpected = CardPayments.Sum(c => c.AmountPaid);

        TotalOtherCounted = OtherPayments.Sum(c => c.CountedAmount);
        TotalOtherExpected = OtherPayments.Sum(c => c.AmountPaid);

        GrandTotalCounted = TotalCashCounted + TotalCardCounted + TotalOtherCounted;
        GrandTotalExpected = TotalCashExpected + TotalCardExpected + TotalOtherExpected;

        DiscrepancyAmount = GrandTotalCounted - GrandTotalExpected;

        if (DiscrepancyAmount == 0)
        {
            DiscrepancyStatusText = "Fərq yoxdur (0.00 ₼)";
        }
        else if (DiscrepancyAmount > 0)
        {
            DiscrepancyStatusText = $"+{DiscrepancyAmount:N2} ₼ Artıq";
        }
        else
        {
            DiscrepancyStatusText = $"{DiscrepancyAmount:N2} ₼ Əskik";
        }
    }

    [RelayCommand]
    private void ToggleShowSystemAmounts()
    {
        ShowSystemAmounts = !ShowSystemAmounts;
    }

    [RelayCommand]
    private void SelectPaymentForEdit(SessionPaymentSummary item)
    {
        SelectedPaymentItem = item;
        NumpadDisplay = item.CountedAmount > 0 ? item.CountedAmount.ToString("F2", CultureInfo.InvariantCulture) : "0";
        IsNumpadOpen = true;
    }

    [RelayCommand]
    private void CloseNumpad()
    {
        if (SelectedPaymentItem != null && decimal.TryParse(NumpadDisplay, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsed))
        {
            SelectedPaymentItem.CountedAmount = parsed;
            RecalculateTotals();
        }
        IsNumpadOpen = false;
        SelectedPaymentItem = null;
    }

    [RelayCommand]
    private void NumpadPress(string key)
    {
        if (NumpadDisplay == "0" && key != ".")
        {
            NumpadDisplay = key;
        }
        else if (key == ".")
        {
            if (!NumpadDisplay.Contains(".")) NumpadDisplay += ".";
        }
        else
        {
            int dotIdx = NumpadDisplay.IndexOf('.');
            if (dotIdx >= 0 && NumpadDisplay.Length - dotIdx > 2) return;
            if (NumpadDisplay.Length < 9) NumpadDisplay += key;
        }
    }

    [RelayCommand]
    private void NumpadBackspace()
    {
        if (NumpadDisplay.Length > 1)
            NumpadDisplay = NumpadDisplay.Substring(0, NumpadDisplay.Length - 1);
        else
            NumpadDisplay = "0";
    }

    [RelayCommand]
    private void NumpadClear()
    {
        NumpadDisplay = "0";
    }

    [RelayCommand]
    private void SubmitForm()
    {
        // Pul sayımını bitirdinizmi? təsdiq modalı
        ConfirmCountdown = 30;
        CurrentStep = CashierOutStep.ConfirmPrompt;
        _confirmTimer.Start();
    }

    private void ConfirmTimer_Tick(object? sender, EventArgs e)
    {
        if (ConfirmCountdown > 0)
        {
            ConfirmCountdown--;
        }
        else
        {
            _confirmTimer.Stop();
            CurrentStep = CashierOutStep.MainForm;
        }
    }

    [RelayCommand]
    private void ConfirmYes()
    {
        _confirmTimer.Stop();

        // Check if there is discrepancy
        if (DiscrepancyAmount != 0)
        {
            CurrentStep = CashierOutStep.DiscrepancyReason;
        }
        else
        {
            _ = FinalizeSessionCloseAsync();
        }
    }

    [RelayCommand]
    private void ConfirmNo()
    {
        _confirmTimer.Stop();
        CurrentStep = CashierOutStep.MainForm;
    }

    [RelayCommand]
    private void AppendKeyToReason(string key)
    {
        if (key == "SPACE")
            DiscrepancyReasonText += " ";
        else if (key == "BACKSPACE")
        {
            if (!string.IsNullOrEmpty(DiscrepancyReasonText))
                DiscrepancyReasonText = DiscrepancyReasonText.Substring(0, DiscrepancyReasonText.Length - 1);
        }
        else if (key == "CLEAR")
            DiscrepancyReasonText = string.Empty;
        else
            DiscrepancyReasonText += key;
    }

    [RelayCommand]
    private void SelectPresetReason(string preset)
    {
        DiscrepancyReasonText = preset;
    }

    [RelayCommand]
    private async Task SubmitDiscrepancyReason()
    {
        await FinalizeSessionCloseAsync();
    }

    private async Task FinalizeSessionCloseAsync()
    {
        IsBusy = true;
        try
        {
            var allPayments = CashPayments.Concat(CardPayments).Concat(OtherPayments).ToList();

            string notes1 = SessionManager.EmployeeName;
            string notes2 = string.IsNullOrWhiteSpace(DiscrepancyReasonText) 
                ? $"Fərq: {DiscrepancyAmount:N2} AZN" 
                : $"{DiscrepancyReasonText}, (AÇIQ ÇEK: 0.00), (Daxil edilən cəm: {GrandTotalCounted:N2} AZN)";

            await _registerSessionService.CloseCashierSessionComprehensiveAsync(
                registerSessionId: SelectedSessionId,
                branchId: SessionManager.BranchID > 0 ? SessionManager.BranchID : 1,
                stationId: SessionManager.StationID > 0 ? SessionManager.StationID : 1,
                employeeId: SessionManager.EmployeeID,
                endAmount: GrandTotalCounted,
                discrepancyAmount: DiscrepancyAmount,
                discrepancyNotes: notes1,
                discrepancyNotes2: notes2,
                paymentSummaries: allPayments,
                accessCode: ""
            );

            SessionManager.RegisterSessionID = null;

            CurrentStep = CashierOutStep.SuccessInfo;
            SuccessCountdown = 10;
            _successTimer.Start();
        }
        catch (Exception ex)
        {
            PosMessageDialog.ShowError($"Kassa bağlanarkən xəta baş verdi:\n{ex.Message}", "XƏTA BİLDİRİŞİ", 15);
            CurrentStep = CashierOutStep.MainForm;
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void SuccessTimer_Tick(object? sender, EventArgs e)
    {
        if (SuccessCountdown > 0)
        {
            SuccessCountdown--;
        }
        else
        {
            _successTimer.Stop();
            FinishAndClose();
        }
    }

    [RelayCommand]
    private void FinishAndClose()
    {
        _confirmTimer.Stop();
        _successTimer.Stop();
        RequestClose?.Invoke(true);
    }

    [RelayCommand]
    private void Cancel()
    {
        _confirmTimer.Stop();
        _successTimer.Stop();
        RequestClose?.Invoke(false);
    }

    public void Cleanup()
    {
        _confirmTimer.Stop();
        _successTimer.Stop();
    }
}
