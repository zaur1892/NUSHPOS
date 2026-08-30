using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NUSHPOS.Helpers;
using NUSHPOS.Services;
using NUSHPOS.ViewModels.Base;
using System;
using System.Threading.Tasks;
using Dapper;

namespace NUSHPOS.ViewModels;

public partial class MainScreenViewModel : ViewModelBase
{
    private readonly NavigationService _navigationService;
    private readonly RegisterSessionService _registerSessionService;
    private readonly DatabaseService _databaseService;

    [ObservableProperty]
    private string _employeeName = SessionManager.EmployeeName;

    [ObservableProperty]
    private string _currentDateTime = DateTime.Now.ToString("dd.MM.yyyy HH:mm");

    [ObservableProperty]
    private string _stationInfo = $"Terminal: {SessionManager.StationID}";

    [ObservableProperty]
    private bool _hasActiveSession;

    [ObservableProperty]
    private int _dailyOrderCount;

    [ObservableProperty]
    private int _openTableCount;

    private System.Windows.Threading.DispatcherTimer? _timer;

    public MainScreenViewModel(NavigationService navigationService, RegisterSessionService registerSessionService, DatabaseService databaseService)
    {
        _navigationService = navigationService;
        _registerSessionService = registerSessionService;
        _databaseService = databaseService;
        Title = "Ana Ekran";
        StartClock();
        _ = CheckSessionAsync();
        _ = LoadDashboardDataAsync();
    }

    private void StartClock()
    {
        _timer = new System.Windows.Threading.DispatcherTimer();
        _timer.Interval = TimeSpan.FromSeconds(30);
        _timer.Tick += (s, e) => CurrentDateTime = DateTime.Now.ToString("dd.MM.yyyy HH:mm");
        _timer.Start();
    }

    private async Task LoadDashboardDataAsync()
    {
        try
        {
            using var connection = _databaseService.CreateConnection();
            
            // Get today's order count
            string orderQuery = "SELECT COUNT(*) FROM OrderHeaders WHERE CONVERT(date, OrderDateTime) = CONVERT(date, GETDATE()) AND ISNULL(LineDeleted, 0) = 0";
            DailyOrderCount = await connection.ExecuteScalarAsync<int>(orderQuery);

            // Get open tables count
            string tableQuery = "SELECT COUNT(DISTINCT DineInTableID) FROM OrderHeaders WHERE (OrderStatus = 1 OR OrderStatus = 0 OR OrderStatus IS NULL) AND ISNULL(LineDeleted, 0) = 0 AND DineInTableID IS NOT NULL AND DineInTableID > 0";
            OpenTableCount = await connection.ExecuteScalarAsync<int>(tableQuery);
        }
        catch
        {
            // Ignore errors for dashboard
            DailyOrderCount = 0;
            OpenTableCount = 0;
        }
    }

    private async Task CheckSessionAsync()
    {
        var session = await _registerSessionService.GetActiveSessionAsync(SessionManager.EmployeeID);
        HasActiveSession = session != null;
        if (session != null)
            SessionManager.RegisterSessionID = session.RegisterSessionID;
    }

    [RelayCommand]
    private void OpenDineIn()
    {
        _navigationService.NavigateTo<TablePlanViewModel>();
    }

    [RelayCommand]
    private void OpenTakeAway()
    {
        // Navigate to SaleScreen with TakeAway mode
        _navigationService.NavigateTo<SaleScreenViewModel>(new { OrderType = 3, TableId = 0 });
    }

    [RelayCommand]
    private void OpenDelivery()
    {
        // TODO: Navigate to PackageSaleView
    }

    [RelayCommand]
    private void OpenRecall()
    {
        _navigationService.NavigateTo<OrderRecallViewModel>();
    }

    [RelayCommand]
    private async Task OpenCashRegisterIn()
    {
        // TODO: Show dialog for entering start amount
        var session = new Models.RegisterSession
        {
            EmployeeID = SessionManager.EmployeeID,
            StationID = SessionManager.StationID,
            BranchID = SessionManager.BranchID,
            SignInDateTime = DateTime.Now,
            RegisterStartAmount = 0
        };
        var sessionId = await _registerSessionService.OpenSessionAsync(session);
        SessionManager.RegisterSessionID = sessionId;
        HasActiveSession = true;
    }

    [RelayCommand]
    private async Task CloseCashRegister()
    {
        if (SessionManager.RegisterSessionID.HasValue)
        {
            await _registerSessionService.CloseSessionAsync(SessionManager.RegisterSessionID.Value, 0);
            SessionManager.RegisterSessionID = null;
            HasActiveSession = false;
        }
    }

    [RelayCommand]
    private void OpenOperations()
    {
        _navigationService.NavigateTo<OperationsViewModel>();
    }

    [RelayCommand]
    private void OpenBackOffice()
    {
        _navigationService.NavigateTo<BackOfficeViewModel>();
    }

    [RelayCommand]
    private void OpenReports()
    {
        _navigationService.NavigateTo<ReportsViewModel>();
    }

    [RelayCommand]
    private void Logout()
    {
        _timer?.Stop();
        SessionManager.Clear();
        _navigationService.NavigateTo<LoginViewModel>();
    }
}
