using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using NUSHPOS.Models;
using NUSHPOS.Services;
using NUSHPOS.ViewModels.Base;

namespace NUSHPOS.ViewModels;

public partial class TerminalSettingsViewModel : ViewModelBase
{
    private readonly StationSettingsService _settingsService;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private StationSettings _station = new();

    [ObservableProperty]
    private StationPrinterSettings _printerSettings = new();

    [ObservableProperty]
    private ObservableCollection<DineInTableGroup> _tableGroups = new();

    [ObservableProperty]
    private ObservableCollection<Printer> _printers = new();

    [ObservableProperty]
    private ObservableCollection<PrinterDesign> _printerDesigns = new();

    [ObservableProperty]
    private ObservableCollection<string> _loginEntrances = new()
    {
        "MASA SATIÅI",
        "SÃœRÆTLÄ° SATIÅ",
        "Ã‡ATDIRILMA",
        "GÃ–TÃœR-APAR",
        "QÆBZ Ã‡AÄIRMA"
    };

    [ObservableProperty]
    private ObservableCollection<string> _skins = new()
    {
        "Office 2010 Blue",
        "Office 2010 Silver",
        "Office 2010 Black",
        "Metro Dark",
        "Metro Light"
    };

    [ObservableProperty]
    private ObservableCollection<string> _languages = new()
    {
        "AzÉ™rbaycan",
        "TÃ¼rkÃ§e",
        "English",
        "Ğ ÑƒÑÑĞºĞ¸Ğ¹"
    };

    [ObservableProperty]
    private ObservableCollection<string> _comPorts = new()
    {
        "-",
        "COM1",
        "COM2",
        "COM3",
        "COM4",
        "COM5",
        "COM6",
        "COM7",
        "COM8",
        "COM9"
    };

    [ObservableProperty]
    private ObservableCollection<int> _baudRates = new()
    {
        1200,
        2400,
        4800,
        9600,
        19200,
        38400,
        57600,
        115200
    };

    [ObservableProperty]
    private int _selectedTabIndex = 0;

    public event Action? RequestClose;

    public TerminalSettingsViewModel(StationSettingsService settingsService)
    {
        _settingsService = settingsService;
        Title = "TERMÄ°NAL PARAMETRLÆRÄ°";

        _ = LoadSettingsAsync();
    }

    public async Task LoadSettingsAsync()
    {
        IsLoading = true;
        try
        {
            var st = await _settingsService.GetStationSettingsAsync(1);
            if (st != null)
            {
                Station = st;
            }

            var prn = await _settingsService.GetStationPrinterSettingsAsync(1);
            if (prn != null)
            {
                PrinterSettings = prn;
            }

            var groups = await _settingsService.GetTableGroupsAsync();
            TableGroups = new ObservableCollection<DineInTableGroup>(groups);

            var printersList = await _settingsService.GetPrintersAsync();
            Printers = new ObservableCollection<Printer>(printersList);

            var designs = await _settingsService.GetPrinterDesignsAsync();
            PrinterDesigns = new ObservableCollection<PrinterDesign>(designs);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"ParametrlÉ™r yÃ¼klÉ™nÉ™rkÉ™n xÉ™ta baÅŸ verdi: {ex.Message}", "XÉ™ta", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private void BrowseBackgroundPicture()
    {
        var dialog = new OpenFileDialog
        {
            Filter = "ÅÉ™kil FayllarÄ±|*.png;*.jpg;*.jpeg;*.bmp|BÃ¼tÃ¼n Fayllar|*.*"
        };
        if (dialog.ShowDialog() == true)
        {
            Station.BackgroundPicture = dialog.FileName;
            OnPropertyChanged(nameof(Station));
        }
    }

    [RelayCommand]
    private void ClearBackgroundPicture()
    {
        Station.BackgroundPicture = string.Empty;
        OnPropertyChanged(nameof(Station));
    }

    [RelayCommand]
    private void BrowseSideBarPicture()
    {
        var dialog = new OpenFileDialog
        {
            Filter = "ÅÉ™kil FayllarÄ±|*.png;*.jpg;*.jpeg;*.bmp|BÃ¼tÃ¼n Fayllar|*.*"
        };
        if (dialog.ShowDialog() == true)
        {
            Station.SideBarPicture = dialog.FileName;
            OnPropertyChanged(nameof(Station));
        }
    }

    [RelayCommand]
    private void ClearSideBarPicture()
    {
        Station.SideBarPicture = string.Empty;
        OnPropertyChanged(nameof(Station));
    }

    [RelayCommand]
    private void TestCallCenterAddress()
    {
        if (string.IsNullOrWhiteSpace(Station.CallCenterClientAddress))
        {
            MessageBox.Show("ZÉ™hmÉ™t olmasa Ã‡aÄŸrÄ± MÉ™rkÉ™zi Ã¼nvanÄ±nÄ± daxil edin.", "MÉ™lumat", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }
        MessageBox.Show($"Ã‡aÄŸrÄ± MÉ™rkÉ™zi serverinÉ™ qoÅŸulma uÄŸurludur: {Station.CallCenterClientAddress}", "UÄŸurlu QoÅŸulma", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    [RelayCommand]
    private void TestCentralCallCenterAddress()
    {
        if (string.IsNullOrWhiteSpace(Station.CentralCallCenterClientAddress))
        {
            MessageBox.Show("ZÉ™hmÉ™t olmasa MÉ™rkÉ™zi Ã‡aÄŸrÄ± MÉ™rkÉ™zi Ã¼nvanÄ±nÄ± daxil edin.", "MÉ™lumat", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }
        MessageBox.Show($"MÉ™rkÉ™zi Ã‡aÄŸrÄ± MÉ™rkÉ™zi serverinÉ™ qoÅŸulma uÄŸurludur: {Station.CentralCallCenterClientAddress}", "UÄŸurlu QoÅŸulma", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    [RelayCommand]
    private async Task SaveAndCloseAsync()
    {
        try
        {
            await _settingsService.UpdateStationSettingsAsync(Station);
            await _settingsService.UpdateStationPrinterSettingsAsync(PrinterSettings);

            MessageBox.Show("Terminal parametrlÉ™ri uÄŸurla yadda saxlanÄ±ldÄ±!", "MÉ™lumat", MessageBoxButton.OK, MessageBoxImage.Information);
            RequestClose?.Invoke();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Yadda saxlanÄ±larkÉ™n xÉ™ta baÅŸ verdi: {ex.Message}", "XÉ™ta", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private void Cancel()
    {
        RequestClose?.Invoke();
    }
}