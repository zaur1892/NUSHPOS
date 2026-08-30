using System;
using System.Threading.Tasks;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NUSHPOS.Helpers;
using NUSHPOS.Services;
using NUSHPOS.ViewModels.Base;

namespace NUSHPOS.ViewModels;

public partial class LoginViewModel : ViewModelBase
{
    private readonly NavigationService _navigationService;
    private readonly EmployeeService _employeeService;
    private DispatcherTimer _timer;

    [ObservableProperty]
    private string _accessCode = string.Empty;

    [ObservableProperty]
    private string _displayCode = string.Empty;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private string _currentDateTime = string.Empty;

    public LoginViewModel(NavigationService navigationService, EmployeeService employeeService)
    {
        _navigationService = navigationService;
        _employeeService = employeeService;
        Title = "Login";

        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };
        _timer.Tick += (s, e) => CurrentDateTime = DateTime.Now.ToString("dd MMM yyyy HH:mm:ss");
        _timer.Start();
        CurrentDateTime = DateTime.Now.ToString("dd MMM yyyy HH:mm:ss");
    }

    [RelayCommand]
    private void Numpad(string digit)
    {
        if (AccessCode.Length < 10)
        {
            AccessCode += digit;
            UpdateDisplayCode();
            ErrorMessage = string.Empty;
        }
    }

    [RelayCommand]
    private void Clear()
    {
        AccessCode = string.Empty;
        UpdateDisplayCode();
        ErrorMessage = string.Empty;
    }

    [RelayCommand]
    private void Backspace()
    {
        if (AccessCode.Length > 0)
        {
            AccessCode = AccessCode.Substring(0, AccessCode.Length - 1);
            UpdateDisplayCode();
            ErrorMessage = string.Empty;
        }
    }

    private void UpdateDisplayCode()
    {
        DisplayCode = new string('●', AccessCode.Length);
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        if (string.IsNullOrEmpty(AccessCode))
        {
            ErrorMessage = "Zəhmət olmasa şifrəni daxil edin.";
            return;
        }

        IsBusy = true;
        ErrorMessage = string.Empty;

        try
        {
            var employee = await _employeeService.LoginAsync(AccessCode);

            if (employee != null)
            {
                SessionManager.EmployeeID = employee.EmployeeID;
                SessionManager.EmployeeName = $"{employee.FirstName} {employee.LastName}".Trim();
                SessionManager.EmployeeKey = employee.EmployeeKey?.ToString() ?? Guid.Empty.ToString().ToUpper();
                SessionManager.SecurityLevel = employee.SecurityLevel ?? 0;

                _timer.Stop();
                _navigationService.NavigateTo<MainScreenViewModel>();
            }
            else
            {
                ErrorMessage = "Yanlış şifrə! Yenidən cəhd edin.";
                AccessCode = string.Empty;
                UpdateDisplayCode();
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Bağlantı xətası: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}
