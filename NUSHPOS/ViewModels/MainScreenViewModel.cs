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
    private readonly StationSettingsService _stationSettingsService;
    private readonly AuthorityService _authorityService;

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

    // Visibility toggles from Station Settings
    [ObservableProperty]
    private bool _showDineInButton = true;

    [ObservableProperty]
    private bool _showTakeOutButton = true;

    [ObservableProperty]
    private bool _showDriveThruButton = false;

    [ObservableProperty]
    private bool _showDeliveryButton = true;

    [ObservableProperty]
    private bool _showRecallButton = true;

    [ObservableProperty]
    private bool _showDriverStatusButton = true;

    [ObservableProperty]
    private bool _showOperationsButton = true;

    [ObservableProperty]
    private bool _showBackOfficeButton = true;

    private System.Windows.Threading.DispatcherTimer? _timer;

    public MainScreenViewModel(
        NavigationService navigationService, 
        RegisterSessionService registerSessionService, 
        DatabaseService databaseService,
        StationSettingsService stationSettingsService,
        AuthorityService authorityService)
    {
        _navigationService = navigationService;
        _registerSessionService = registerSessionService;
        _databaseService = databaseService;
        _stationSettingsService = stationSettingsService;
        _authorityService = authorityService;
        Title = "Ana Ekran";
        StartClock();
        _ = CheckSessionAsync();
        _ = LoadDashboardDataAsync();
        _ = LoadStationSettingsAsync();
    }

    private void StartClock()
    {
        _timer = new System.Windows.Threading.DispatcherTimer();
        _timer.Interval = TimeSpan.FromSeconds(30);
        _timer.Tick += (s, e) => CurrentDateTime = DateTime.Now.ToString("dd.MM.yyyy HH:mm");
        _timer.Start();
    }

    public async Task LoadStationSettingsAsync()
    {
        try
        {
            int stationId = SessionManager.StationID > 0 ? SessionManager.StationID : 1;
            var settings = await _stationSettingsService.GetStationSettingsAsync(stationId);
            if (settings != null)
            {
                ShowDineInButton = settings.ShowDineInButton ?? true;
                ShowTakeOutButton = settings.ShowTakeOutButton ?? true;
                ShowDriveThruButton = settings.ShowDriveThruButton ?? false;
                ShowDeliveryButton = settings.ShowDeliveryButton ?? true;
                ShowRecallButton = settings.ShowRecallButton ?? true;
                ShowDriverStatusButton = settings.ShowDriverStatusButton ?? true;
                ShowOperationsButton = settings.ShowOperationsButton ?? true;
                ShowBackOfficeButton = settings.ShowBackOfficeButton ?? true;
            }
        }
        catch { }
    }

    public async Task LoadDashboardDataAsync()
    {
        try
        {
            using var connection = _databaseService.CreateConnection();
            string query = @"
                SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
                SELECT 
                    (SELECT COUNT(AutoID) FROM OrderHeaders WITH(NOLOCK) WHERE CAST(AddDateTime AS DATE) = CAST(GETDATE() AS DATE)) as DailyOrders,
                    (SELECT COUNT(AutoID) FROM TableFiles WITH(NOLOCK) WHERE TableStatus = 1) as OpenTables;
            ";
            var result = await connection.QueryFirstOrDefaultAsync<dynamic>(query);
            if (result != null)
            {
                DailyOrderCount = (int)(result.DailyOrders ?? 0);
                OpenTableCount = (int)(result.OpenTables ?? 0);
            }
        }
        catch { }
    }

    public async Task CheckSessionAsync()
    {
        var session = await _registerSessionService.GetActiveSessionAsync(SessionManager.EmployeeID);
        HasActiveSession = session != null;
        if (session != null)
            SessionManager.RegisterSessionID = session.RegisterSessionID;
    }

    [RelayCommand]
    private async Task OpenDineIn()
    {
        if (!await _authorityService.ValidateActionAccessAsync("saleOrderDineIn", "MASA REJİMİ")) return;
        _navigationService.NavigateTo<TablePlanViewModel>();
    }

    [RelayCommand]
    private async Task OpenTakeAway()
    {
        if (!await _authorityService.ValidateActionAccessAsync("saleOrderTakeOut", "GÖTÜRÜLMƏ SATIŞ")) return;
        _navigationService.NavigateTo<SaleScreenViewModel>(new { OrderType = 3, TableId = 0 });
    }

    [RelayCommand]
    private async Task OpenDriveThru()
    {
        if (!await _authorityService.ValidateActionAccessAsync("saleDriveThru", "TEZGAH / SÜR-KEÇ SATIŞ")) return;
        _navigationService.NavigateTo<SaleScreenViewModel>(new { OrderType = 4, TableId = 0 });
    }

    [RelayCommand]
    private async Task OpenDelivery()
    {
        if (!await _authorityService.ValidateActionAccessAsync("saleOrderDelivery", "ÇATDIRILMA SATIŞI")) return;
        _navigationService.NavigateTo<SaleScreenViewModel>(new { OrderType = 2, TableId = 0 });
    }

    [RelayCommand]
    private async Task OpenRecall()
    {
        if (!await _authorityService.ValidateActionAccessAsync("orderRecall", "ÇEKLƏRİ GÖR")) return;
        _navigationService.NavigateTo<OrderRecallViewModel>();
    }

    [RelayCommand]
    private async Task OpenDriverStatus()
    {
        if (!await _authorityService.ValidateActionAccessAsync("deliveryStatus", "KURYER STATUSU")) return;
        _navigationService.NavigateTo<OperationsViewModel>();
    }

    [RelayCommand]
    private async Task OpenCashRegisterIn()
    {
        if (!await _authorityService.ValidateActionAccessAsync("employeeRegisterIn", "KASSİR SESSİYASINI AÇMAQ")) return;

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
        if (!await _authorityService.ValidateActionAccessAsync("employeeRegisterOut", "KASSİR SESSİYASINI BAĞLAMAQ")) return;

        if (SessionManager.RegisterSessionID.HasValue)
        {
            await _registerSessionService.CloseSessionAsync(SessionManager.RegisterSessionID.Value, 0);
            SessionManager.RegisterSessionID = null;
            HasActiveSession = false;
        }
    }

    [RelayCommand]
    private async Task OpenOperations()
    {
        if (!await _authorityService.ValidateActionAccessAsync("operations", "ƏMƏLİYYATLAR PƏNCƏRƏSİ")) return;
        _navigationService.NavigateTo<OperationsViewModel>();
    }

    [RelayCommand]
    private async Task OpenBackOffice()
    {
        if (!await _authorityService.ValidateActionAccessAsync("backOffice", "ARXA OFİS")) return;
        _navigationService.NavigateTo<BackOfficeViewModel>();
    }

    [RelayCommand]
    private async Task OpenReports()
    {
        if (!await _authorityService.ValidateActionAccessAsync("reports", "HESABATLAR")) return;
        _navigationService.NavigateTo<ReportsViewModel>();
    }

    [RelayCommand]
    private async Task Logout()
    {
        if (!await _authorityService.ValidateActionAccessAsync("exitApplication", "PROQRAMDAN ÇIXIŞ")) return;
        _timer?.Stop();
        SessionManager.Clear();
        _navigationService.NavigateTo<LoginViewModel>();
    }
}
