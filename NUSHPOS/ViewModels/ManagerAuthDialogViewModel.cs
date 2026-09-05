using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NUSHPOS.Models;
using NUSHPOS.Services;
using NUSHPOS.ViewModels.Base;

namespace NUSHPOS.ViewModels;

public partial class ManagerAuthDialogViewModel : ViewModelBase
{
    private readonly EmployeeService _employeeService;

    public Action? RequestClose { get; set; }

    [ObservableProperty]
    private string _actionName = "Əməliyyat";

    [ObservableProperty]
    private int _requiredLevel = 1;

    [ObservableProperty]
    private bool _isForced;

    [ObservableProperty]
    private string _pinCode = string.Empty;

    [ObservableProperty]
    private string _displayPin = string.Empty;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    public bool IsAuthorized { get; private set; }
    public Employee? AuthorizedEmployee { get; private set; }

    public ManagerAuthDialogViewModel(
        EmployeeService employeeService, 
        string actionName, 
        int requiredLevel, 
        bool isForced = false)
    {
        _employeeService = employeeService;
        _actionName = actionName;
        _requiredLevel = requiredLevel;
        _isForced = isForced;
        Title = "Səlahiyyət Təsdiqi";
    }

    [RelayCommand]
    private void Numpad(string digit)
    {
        if (PinCode.Length < 10)
        {
            PinCode += digit;
            UpdateDisplayPin();
            ErrorMessage = string.Empty;
        }
    }

    [RelayCommand]
    private void Clear()
    {
        PinCode = string.Empty;
        UpdateDisplayPin();
        ErrorMessage = string.Empty;
    }

    [RelayCommand]
    private void Backspace()
    {
        if (PinCode.Length > 0)
        {
            PinCode = PinCode.Substring(0, PinCode.Length - 1);
            UpdateDisplayPin();
            ErrorMessage = string.Empty;
        }
    }

    private void UpdateDisplayPin()
    {
        DisplayPin = PinCode.Length == 0 ? string.Empty : string.Join("  ", new string('●', PinCode.Length).ToCharArray());
    }

    [RelayCommand]
    private async Task ConfirmAsync()
    {
        if (string.IsNullOrWhiteSpace(PinCode))
        {
            ErrorMessage = "Zəhmət olmasa şifrəni daxil edin.";
            return;
        }

        IsBusy = true;
        ErrorMessage = string.Empty;

        try
        {
            var employee = await _employeeService.LoginAsync(PinCode);

            if (employee == null)
            {
                ErrorMessage = "Daxil edilən şifrə yalnışdır!";
                PinCode = string.Empty;
                UpdateDisplayPin();
                return;
            }

            int empLevel = employee.SecurityLevel ?? 0;

            if (empLevel < RequiredLevel)
            {
                ErrorMessage = $"Personalın səlahiyyəti çatmır! (Səviyyə: {empLevel}, Tələb: {RequiredLevel})";
                PinCode = string.Empty;
                UpdateDisplayPin();
                return;
            }

            // Successfully authorized
            IsAuthorized = true;
            AuthorizedEmployee = employee;
            RequestClose?.Invoke();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Xəta baş verdi: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void Cancel()
    {
        IsAuthorized = false;
        RequestClose?.Invoke();
    }
}
