using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NUSHPOS.ViewModels.Base;
using System;
using System.Globalization;
using System.Windows.Threading;

namespace NUSHPOS.ViewModels;

public partial class CashierInDialogViewModel : ViewModelBase
{
    private readonly DispatcherTimer _countdownTimer;

    [ObservableProperty]
    private string _rawAmount = "0";

    [ObservableProperty]
    private decimal _amount = 0m;

    [ObservableProperty]
    private string _displayAmount = "0.00 ₼";

    [ObservableProperty]
    private bool _isConfirming = false;

    [ObservableProperty]
    private int _countdown = 30;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    public event Action<bool>? RequestClose;

    public CashierInDialogViewModel()
    {
        Title = "NUSHPOS KASSİR GİRİŞİ";

        _countdownTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };
        _countdownTimer.Tick += CountdownTimer_Tick;

        UpdateDisplay();
    }

    private void CountdownTimer_Tick(object? sender, EventArgs e)
    {
        if (Countdown > 0)
        {
            Countdown--;
        }
        else
        {
            _countdownTimer.Stop();
            // Automatically return to input if timeout
            IsConfirming = false;
        }
    }

    [RelayCommand]
    private void Numpad(string key)
    {
        if (IsConfirming) return;

        if (RawAmount == "0" && key != ".")
        {
            RawAmount = key;
        }
        else if (key == ".")
        {
            if (!RawAmount.Contains("."))
            {
                RawAmount += ".";
            }
        }
        else
        {
            // Limit to 2 decimal places if decimal point exists
            int dotIndex = RawAmount.IndexOf('.');
            if (dotIndex >= 0 && RawAmount.Length - dotIndex > 2)
            {
                return;
            }

            if (RawAmount.Length < 9)
            {
                RawAmount += key;
            }
        }

        UpdateDisplay();
    }

    [RelayCommand]
    private void Backspace()
    {
        if (IsConfirming) return;

        if (RawAmount.Length > 1)
        {
            RawAmount = RawAmount.Substring(0, RawAmount.Length - 1);
        }
        else
        {
            RawAmount = "0";
        }

        UpdateDisplay();
    }

    [RelayCommand]
    private void Clear()
    {
        if (IsConfirming) return;

        RawAmount = "0";
        UpdateDisplay();
    }

    [RelayCommand]
    private void AddPreset(string amountStr)
    {
        if (IsConfirming) return;

        if (decimal.TryParse(amountStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var preset))
        {
            Amount += preset;
            RawAmount = Amount.ToString("F2", CultureInfo.InvariantCulture);
            UpdateDisplay();
        }
    }

    private void UpdateDisplay()
    {
        if (decimal.TryParse(RawAmount, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsed))
        {
            Amount = parsed;
        }
        else
        {
            Amount = 0m;
        }

        DisplayAmount = $"{Amount:N2} ₼";
        ErrorMessage = string.Empty;
    }

    [RelayCommand]
    private void SubmitAmount()
    {
        ErrorMessage = string.Empty;
        Countdown = 30;
        IsConfirming = true;
        _countdownTimer.Start();
    }

    [RelayCommand]
    private void ConfirmYes()
    {
        _countdownTimer.Stop();
        RequestClose?.Invoke(true);
    }

    [RelayCommand]
    private void ConfirmNo()
    {
        _countdownTimer.Stop();
        IsConfirming = false;
    }

    [RelayCommand]
    private void Cancel()
    {
        _countdownTimer.Stop();
        RequestClose?.Invoke(false);
    }

    public void Cleanup()
    {
        _countdownTimer.Stop();
    }
}
