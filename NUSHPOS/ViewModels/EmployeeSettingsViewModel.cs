using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NUSHPOS.Models;
using NUSHPOS.Services;
using NUSHPOS.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace NUSHPOS.ViewModels;

public partial class EmployeeSettingsViewModel : ViewModelBase
{
    private readonly EmployeeService _employeeService;

    // ─── Siyahılar ───────────────────────────────────────────────────────────
    [ObservableProperty]
    private ObservableCollection<Employee> _employees = new();

    [ObservableProperty]
    private ObservableCollection<EmployeeTitle> _titles = new();

    [ObservableProperty]
    private ObservableCollection<string> _securityLevels = new() { "1","2","3","4","5","6","7","8","9","10" };

    [ObservableProperty]
    private ObservableCollection<string> _languages = new() { "-", "az", "en", "ru", "tr" };

    // ─── Seçilmiş əməkdaş ────────────────────────────────────────────────────
    [ObservableProperty]
    private Employee? _selectedEmployee;

    // ─── Tab seçimi ───────────────────────────────────────────────────────────
    [ObservableProperty]
    private bool _showActive = true;

    // ─── Loading ──────────────────────────────────────────────────────────────
    [ObservableProperty]
    private bool _isBusy = false;

    // ─── Mövcud tab məlumatları ────────────────────────────────────────────────
    // "GENEL" tab
    [ObservableProperty] private string _firstName = "";
    [ObservableProperty] private string _lastName = "";
    [ObservableProperty] private EmployeeTitle? _selectedTitle;
    [ObservableProperty] private string _selectedSecurityLevel = "3";
    [ObservableProperty] private string _selectedLanguage = "-";
    [ObservableProperty] private string _accessCode = "";
    [ObservableProperty] private string _mifareCode = "";
    [ObservableProperty] private string _scanCode = "";
    [ObservableProperty] private bool _isPackager = false;
    [ObservableProperty] private bool _isHidden = false;

    // "DİGƏR" tab
    [ObservableProperty] private string _phoneNumber = "";
    [ObservableProperty] private string _employeeNotes = "";
    [ObservableProperty] private bool _useStaffBank = false;
    [ObservableProperty] private bool _isDriver = false;
    [ObservableProperty] private bool _isServer = false;
    [ObservableProperty] private bool _useHostess = false;
    [ObservableProperty] private bool _noCashierOut = false;
    [ObservableProperty] private decimal _monthlyDinnerFee = 0;

    public EmployeeSettingsViewModel(EmployeeService employeeService)
    {
        _employeeService = employeeService;
        Title = "İşçi Sazlamaları";
        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        IsBusy = true;
        try
        {
            var titles = await _employeeService.GetTitlesAsync();
            Titles = new ObservableCollection<EmployeeTitle>(titles);

            await RefreshEmployeeListAsync();
        }
        catch { }
        finally { IsBusy = false; }
    }

    private async Task RefreshEmployeeListAsync()
    {
        var list = ShowActive
            ? await _employeeService.GetActiveEmployeesAsync()
            : await _employeeService.GetFormerEmployeesAsync();
        Employees = new ObservableCollection<Employee>(list);
        SelectedEmployee = null;
        ClearForm();
    }

    partial void OnSelectedEmployeeChanged(Employee? value)
    {
        if (value == null) { ClearForm(); return; }
        // GENEL tab
        FirstName = value.FirstName ?? "";
        LastName = value.LastName ?? "";
        SelectedTitle = Titles.Count > 0
            ? System.Linq.Enumerable.FirstOrDefault(Titles, t => t.TitleID == value.JobTitleID)
            : null;
        SelectedSecurityLevel = value.SecurityLevel?.ToString() ?? "3";
        SelectedLanguage = value.PrefUserInterfaceLocale ?? "-";
        AccessCode = value.AccessCode ?? "";
        MifareCode = value.MifareCardCode ?? "";
        ScanCode = value.ScanCode ?? "";
        IsPackager = value.UseStaffBank ?? false;
        IsHidden = value.IsOffline ?? false;
        // DİGƏR tab
        PhoneNumber = value.PhoneNumber ?? "";
        EmployeeNotes = value.EmployeeNotes ?? "";
        UseStaffBank = value.UseStaffBank ?? false;
        IsDriver = value.EmployeeIsDriver ?? false;
        IsServer = value.IsAServer ?? false;
        UseHostess = value.UseHostess ?? false;
        NoCashierOut = value.NoCashierOut ?? false;
        MonthlyDinnerFee = value.MonthlyDinnerFee ?? 0;
    }

    private void ClearForm()
    {
        FirstName = ""; LastName = "";
        SelectedTitle = null; SelectedSecurityLevel = "3";
        SelectedLanguage = "-"; AccessCode = "";
        MifareCode = ""; ScanCode = "";
        IsPackager = false; IsHidden = false;
        PhoneNumber = ""; EmployeeNotes = "";
        UseStaffBank = false; IsDriver = false;
        IsServer = false; UseHostess = false;
        NoCashierOut = false; MonthlyDinnerFee = 0;
    }

    [RelayCommand]
    private void ShowActiveEmployees()
    {
        ShowActive = true;
        _ = RefreshEmployeeListAsync();
    }

    [RelayCommand]
    private void ShowFormerEmployees()
    {
        ShowActive = false;
        _ = RefreshEmployeeListAsync();
    }

    [RelayCommand]
    private void NewEmployee()
    {
        SelectedEmployee = null;
        ClearForm();
    }

    [RelayCommand]
    private async Task Save()
    {
        IsBusy = true;
        try
        {
            var emp = SelectedEmployee ?? new Employee();
            emp.FirstName = FirstName;
            emp.LastName = LastName;
            emp.JobTitleID = SelectedTitle?.TitleID ?? 0;
            emp.JobTitleText = SelectedTitle?.TitleName ?? "";
            emp.SecurityLevel = int.TryParse(SelectedSecurityLevel, out int sl) ? sl : 3;
            emp.PrefUserInterfaceLocale = SelectedLanguage;
            emp.AccessCode = AccessCode;
            emp.MifareCardCode = MifareCode;
            emp.ScanCode = ScanCode;
            emp.UseStaffBank = IsPackager;
            emp.IsOffline = IsHidden;
            emp.PhoneNumber = PhoneNumber;
            emp.EmployeeNotes = EmployeeNotes;
            emp.EmployeeIsDriver = IsDriver;
            emp.IsAServer = IsServer;
            emp.UseHostess = UseHostess;
            emp.NoCashierOut = NoCashierOut;
            emp.MonthlyDinnerFee = MonthlyDinnerFee;
            emp.EmployeeActive = true;

            await _employeeService.SaveEmployeeAsync(emp);
            await RefreshEmployeeListAsync();
        }
        catch { }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    private void Close()
    {
        var backOfficeVm = App.Services.GetService(typeof(BackOfficeViewModel)) as BackOfficeViewModel;
        if (backOfficeVm != null)
            backOfficeVm.CurrentSubView = null;
    }

    // Navigation arrow commands (move selection up/down in list)
    [RelayCommand]
    private void MoveUp()
    {
        if (SelectedEmployee == null || Employees.Count == 0) return;
        int idx = Employees.IndexOf(SelectedEmployee);
        if (idx > 0) SelectedEmployee = Employees[idx - 1];
    }

    [RelayCommand]
    private void MoveDown()
    {
        if (SelectedEmployee == null || Employees.Count == 0) return;
        int idx = Employees.IndexOf(SelectedEmployee);
        if (idx < Employees.Count - 1) SelectedEmployee = Employees[idx + 1];
    }

    [RelayCommand]
    private void MoveFirst()
    {
        if (Employees.Count > 0) SelectedEmployee = Employees[0];
    }

    [RelayCommand]
    private void MoveLast()
    {
        if (Employees.Count > 0) SelectedEmployee = Employees[^1];
    }
}