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
        "MASA SATIŞI",
        "SÜRƏTLİ SATIŞ",
        "ÇATDIRILMA",
        "GÖTÜR-APAR",
        "QƏBZ ÇAĞIRMA"
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
        "Azərbaycan",
        "Türkçe",
        "English",
        "Русский"
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
        Title = "TERMİNAL PARAMETRLƏRİ";

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
            MessageBox.Show($"Parametrlər yüklənərkən xəta baş verdi: {ex.Message}", "Xəta", MessageBoxButton.OK, MessageBoxImage.Error);
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
            Filter = "Şəkil Faylları|*.png;*.jpg;*.jpeg;*.bmp|Bütün Fayllar|*.*"
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
            Filter = "Şəkil Faylları|*.png;*.jpg;*.jpeg;*.bmp|Bütün Fayllar|*.*"
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
            MessageBox.Show("Zəhmət olmasa Çağrı Mərkəzi ünvanını daxil edin.", "Məlumat", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }
        MessageBox.Show($"Çağrı Mərkəzi serverinə qoşulma uğurludur: {Station.CallCenterClientAddress}", "Uğurlu Qoşulma", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    [RelayCommand]
    private void TestCentralCallCenterAddress()
    {
        if (string.IsNullOrWhiteSpace(Station.CentralCallCenterClientAddress))
        {
            MessageBox.Show("Zəhmət olmasa Mərkəzi Çağrı Mərkəzi ünvanını daxil edin.", "Məlumat", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }
        MessageBox.Show($"Mərkəzi Çağrı Mərkəzi serverinə qoşulma uğurludur: {Station.CentralCallCenterClientAddress}", "Uğurlu Qoşulma", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    [RelayCommand]
    private async Task SaveAndCloseAsync()
    {
        try
        {
            await _settingsService.UpdateStationSettingsAsync(Station);
            await _settingsService.UpdateStationPrinterSettingsAsync(PrinterSettings);

            MessageBox.Show("Terminal parametrləri uğurla yadda saxlanıldı!", "Məlumat", MessageBoxButton.OK, MessageBoxImage.Information);
            RequestClose?.Invoke();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Yadda saxlanılarkən xəta baş verdi: {ex.Message}", "Xəta", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private void Cancel()
    {
        RequestClose?.Invoke();
    }
}