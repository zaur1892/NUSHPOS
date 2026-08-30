using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NUSHPOS.ViewModels.Base;
using System;
using System.Windows.Threading;

namespace NUSHPOS.ViewModels;

public partial class ChangeDialogViewModel : ViewModelBase
{
    [ObservableProperty] private string _changeAmountString;
    [ObservableProperty] private int _countdown = 25;
    
    private DispatcherTimer _timer;
    public Action? RequestClose { get; set; }

    public ChangeDialogViewModel(decimal changeAmount)
    {
        ChangeAmountString = changeAmount.ToString("N2");
        
        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };
        _timer.Tick += Timer_Tick;
        _timer.Start();
    }

    private void Timer_Tick(object? sender, EventArgs e)
    {
        Countdown--;
        if (Countdown <= 0)
        {
            CloseDialog();
        }
    }

    [RelayCommand]
    private void CloseDialog()
    {
        _timer.Stop();
        RequestClose?.Invoke();
    }
}
