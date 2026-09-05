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
    private readonly FastReportService _fastReportService;

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
    private ObservableCollection<KitchenPrinterSlot> _kitchenSlotsPage1 = new();

    [ObservableProperty]
    private ObservableCollection<KitchenPrinterSlot> _kitchenSlotsPage2 = new();

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

    public TerminalSettingsViewModel(StationSettingsService settingsService, FastReportService fastReportService)
    {
        _settingsService = settingsService;
        _fastReportService = fastReportService;
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

            var installed = GetInstalledWindowsPrinters();
            var printersList = await _settingsService.SyncInstalledPrintersAsync(installed);
            Printers = new ObservableCollection<Printer>(printersList);

            var designs = await _settingsService.GetPrinterDesignsAsync();
            PrinterDesigns = new ObservableCollection<PrinterDesign>(designs);

            PopulateKitchenSlots();
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

    private static List<string> GetInstalledWindowsPrinters()
    {
        var installed = new List<string>();
        try
        {
            foreach (string pName in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
            {
                if (!string.IsNullOrWhiteSpace(pName) && !installed.Contains(pName, StringComparer.OrdinalIgnoreCase))
                {
                    installed.Add(pName);
                }
            }
        }
        catch { }

        if (installed.Count == 0)
        {
            try
            {
                using var printServer = new System.Printing.LocalPrintServer();
                var printQueues = printServer.GetPrintQueues();
                foreach (var pq in printQueues)
                {
                    if (!string.IsNullOrWhiteSpace(pq.Name) && !installed.Contains(pq.Name, StringComparer.OrdinalIgnoreCase))
                    {
                        installed.Add(pq.Name);
                    }
                }
            }
            catch { }
        }

        return installed;
    }

    private void PopulateKitchenSlots()
    {
        string[] defaultNames = new[]
        {
            "ABİYER", "ET-TESİR", "CAG-KEBAP", "SALATA", "ARASICAK", 
            "MEZE", "BAR", "TATLI", "SARKUTERİ", "USTKATMUTFAK",
            "MƏTBƏX 11", "MƏTBƏX 12", "MƏTBƏX 13", "MƏTBƏX 14", "MƏTBƏX 15",
            "MƏTBƏX 16", "MƏTBƏX 17", "MƏTBƏX 18", "MƏTBƏX 19", "MƏTBƏX 20"
        };

        KitchenSlotsPage1.Clear();
        KitchenSlotsPage1.Add(new KitchenPrinterSlot { SlotNo = 1, SlotName = defaultNames[0], PrinterID = PrinterSettings.Kitchen1PrinterID, DesignPath = PrinterSettings.Kitchen1DesignPath });
        KitchenSlotsPage1.Add(new KitchenPrinterSlot { SlotNo = 2, SlotName = defaultNames[1], PrinterID = PrinterSettings.Kitchen2PrinterID, DesignPath = PrinterSettings.Kitchen2DesignPath });
        KitchenSlotsPage1.Add(new KitchenPrinterSlot { SlotNo = 3, SlotName = defaultNames[2], PrinterID = PrinterSettings.Kitchen3PrinterID, DesignPath = PrinterSettings.Kitchen3DesignPath });
        KitchenSlotsPage1.Add(new KitchenPrinterSlot { SlotNo = 4, SlotName = defaultNames[3], PrinterID = PrinterSettings.Kitchen4PrinterID, DesignPath = PrinterSettings.Kitchen4DesignPath });
        KitchenSlotsPage1.Add(new KitchenPrinterSlot { SlotNo = 5, SlotName = defaultNames[4], PrinterID = PrinterSettings.Kitchen5PrinterID, DesignPath = PrinterSettings.Kitchen5DesignPath });
        KitchenSlotsPage1.Add(new KitchenPrinterSlot { SlotNo = 6, SlotName = defaultNames[5], PrinterID = PrinterSettings.Kitchen6PrinterID, DesignPath = PrinterSettings.Kitchen6DesignPath });
        KitchenSlotsPage1.Add(new KitchenPrinterSlot { SlotNo = 7, SlotName = defaultNames[6], PrinterID = PrinterSettings.Kitchen7PrinterID, DesignPath = PrinterSettings.Kitchen7DesignPath });
        KitchenSlotsPage1.Add(new KitchenPrinterSlot { SlotNo = 8, SlotName = defaultNames[7], PrinterID = PrinterSettings.Kitchen8PrinterID, DesignPath = PrinterSettings.Kitchen8DesignPath });
        KitchenSlotsPage1.Add(new KitchenPrinterSlot { SlotNo = 9, SlotName = defaultNames[8], PrinterID = PrinterSettings.Kitchen9PrinterID, DesignPath = PrinterSettings.Kitchen9DesignPath });
        KitchenSlotsPage1.Add(new KitchenPrinterSlot { SlotNo = 10, SlotName = defaultNames[9], PrinterID = PrinterSettings.Kitchen10PrinterID, DesignPath = PrinterSettings.Kitchen10DesignPath });

        KitchenSlotsPage2.Clear();
        KitchenSlotsPage2.Add(new KitchenPrinterSlot { SlotNo = 11, SlotName = defaultNames[10], PrinterID = PrinterSettings.Kitchen11PrinterID, DesignPath = PrinterSettings.Kitchen11DesignPath });
        KitchenSlotsPage2.Add(new KitchenPrinterSlot { SlotNo = 12, SlotName = defaultNames[11], PrinterID = PrinterSettings.Kitchen12PrinterID, DesignPath = PrinterSettings.Kitchen12DesignPath });
        KitchenSlotsPage2.Add(new KitchenPrinterSlot { SlotNo = 13, SlotName = defaultNames[12], PrinterID = PrinterSettings.Kitchen13PrinterID, DesignPath = PrinterSettings.Kitchen13DesignPath });
        KitchenSlotsPage2.Add(new KitchenPrinterSlot { SlotNo = 14, SlotName = defaultNames[13], PrinterID = PrinterSettings.Kitchen14PrinterID, DesignPath = PrinterSettings.Kitchen14DesignPath });
        KitchenSlotsPage2.Add(new KitchenPrinterSlot { SlotNo = 15, SlotName = defaultNames[14], PrinterID = PrinterSettings.Kitchen15PrinterID, DesignPath = PrinterSettings.Kitchen15DesignPath });
        KitchenSlotsPage2.Add(new KitchenPrinterSlot { SlotNo = 16, SlotName = defaultNames[15], PrinterID = PrinterSettings.Kitchen16PrinterID, DesignPath = PrinterSettings.Kitchen16DesignPath });
        KitchenSlotsPage2.Add(new KitchenPrinterSlot { SlotNo = 17, SlotName = defaultNames[16], PrinterID = PrinterSettings.Kitchen17PrinterID, DesignPath = PrinterSettings.Kitchen17DesignPath });
        KitchenSlotsPage2.Add(new KitchenPrinterSlot { SlotNo = 18, SlotName = defaultNames[17], PrinterID = PrinterSettings.Kitchen18PrinterID, DesignPath = PrinterSettings.Kitchen18DesignPath });
        KitchenSlotsPage2.Add(new KitchenPrinterSlot { SlotNo = 19, SlotName = defaultNames[18], PrinterID = PrinterSettings.Kitchen19PrinterID, DesignPath = PrinterSettings.Kitchen19DesignPath });
        KitchenSlotsPage2.Add(new KitchenPrinterSlot { SlotNo = 20, SlotName = defaultNames[19], PrinterID = PrinterSettings.Kitchen20PrinterID, DesignPath = PrinterSettings.Kitchen20DesignPath });
    }

    [RelayCommand]
    private async Task DesignReceiptAsync(object? parameter)
    {
        string designName = parameter?.ToString() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(designName))
        {
            designName = "Qəbz Dizaynı";
        }

        var vm = new DesignEditorViewModel(_settingsService, _fastReportService);
        await vm.InitializeAsync(designName);
        var win = new Views.DesignEditorWindow(vm);
        win.Owner = Application.Current?.MainWindow;
        win.ShowDialog();

        // Reload designs so any new design appears in the comboboxes
        var designs = await _settingsService.GetPrinterDesignsAsync();
        PrinterDesigns = new ObservableCollection<PrinterDesign>(designs);
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
            // Sync kitchen printer slots back to PrinterSettings
            if (KitchenSlotsPage1.Count >= 10)
            {
                PrinterSettings.Kitchen1PrinterID = KitchenSlotsPage1[0].PrinterID;
                PrinterSettings.Kitchen1DesignPath = KitchenSlotsPage1[0].DesignPath;
                PrinterSettings.Kitchen2PrinterID = KitchenSlotsPage1[1].PrinterID;
                PrinterSettings.Kitchen2DesignPath = KitchenSlotsPage1[1].DesignPath;
                PrinterSettings.Kitchen3PrinterID = KitchenSlotsPage1[2].PrinterID;
                PrinterSettings.Kitchen3DesignPath = KitchenSlotsPage1[2].DesignPath;
                PrinterSettings.Kitchen4PrinterID = KitchenSlotsPage1[3].PrinterID;
                PrinterSettings.Kitchen4DesignPath = KitchenSlotsPage1[3].DesignPath;
                PrinterSettings.Kitchen5PrinterID = KitchenSlotsPage1[4].PrinterID;
                PrinterSettings.Kitchen5DesignPath = KitchenSlotsPage1[4].DesignPath;
                PrinterSettings.Kitchen6PrinterID = KitchenSlotsPage1[5].PrinterID;
                PrinterSettings.Kitchen6DesignPath = KitchenSlotsPage1[5].DesignPath;
                PrinterSettings.Kitchen7PrinterID = KitchenSlotsPage1[6].PrinterID;
                PrinterSettings.Kitchen7DesignPath = KitchenSlotsPage1[6].DesignPath;
                PrinterSettings.Kitchen8PrinterID = KitchenSlotsPage1[7].PrinterID;
                PrinterSettings.Kitchen8DesignPath = KitchenSlotsPage1[7].DesignPath;
                PrinterSettings.Kitchen9PrinterID = KitchenSlotsPage1[8].PrinterID;
                PrinterSettings.Kitchen9DesignPath = KitchenSlotsPage1[8].DesignPath;
                PrinterSettings.Kitchen10PrinterID = KitchenSlotsPage1[9].PrinterID;
                PrinterSettings.Kitchen10DesignPath = KitchenSlotsPage1[9].DesignPath;
            }

            if (KitchenSlotsPage2.Count >= 10)
            {
                PrinterSettings.Kitchen11PrinterID = KitchenSlotsPage2[0].PrinterID;
                PrinterSettings.Kitchen11DesignPath = KitchenSlotsPage2[0].DesignPath;
                PrinterSettings.Kitchen12PrinterID = KitchenSlotsPage2[1].PrinterID;
                PrinterSettings.Kitchen12DesignPath = KitchenSlotsPage2[1].DesignPath;
                PrinterSettings.Kitchen13PrinterID = KitchenSlotsPage2[2].PrinterID;
                PrinterSettings.Kitchen13DesignPath = KitchenSlotsPage2[2].DesignPath;
                PrinterSettings.Kitchen14PrinterID = KitchenSlotsPage2[3].PrinterID;
                PrinterSettings.Kitchen14DesignPath = KitchenSlotsPage2[3].DesignPath;
                PrinterSettings.Kitchen15PrinterID = KitchenSlotsPage2[4].PrinterID;
                PrinterSettings.Kitchen15DesignPath = KitchenSlotsPage2[4].DesignPath;
                PrinterSettings.Kitchen16PrinterID = KitchenSlotsPage2[5].PrinterID;
                PrinterSettings.Kitchen16DesignPath = KitchenSlotsPage2[5].DesignPath;
                PrinterSettings.Kitchen17PrinterID = KitchenSlotsPage2[6].PrinterID;
                PrinterSettings.Kitchen17DesignPath = KitchenSlotsPage2[6].DesignPath;
                PrinterSettings.Kitchen18PrinterID = KitchenSlotsPage2[7].PrinterID;
                PrinterSettings.Kitchen18DesignPath = KitchenSlotsPage2[7].DesignPath;
                PrinterSettings.Kitchen19PrinterID = KitchenSlotsPage2[8].PrinterID;
                PrinterSettings.Kitchen19DesignPath = KitchenSlotsPage2[8].DesignPath;
                PrinterSettings.Kitchen20PrinterID = KitchenSlotsPage2[9].PrinterID;
                PrinterSettings.Kitchen20DesignPath = KitchenSlotsPage2[9].DesignPath;
            }

            var printerDict = Printers.Where(p => !string.IsNullOrEmpty(p.PrinterName)).ToDictionary(p => p.PrinterID, p => p.PrinterName!);
            if (PrinterSettings.CheckPrinterID.HasValue && printerDict.TryGetValue(PrinterSettings.CheckPrinterID.Value, out var chkName))
                PrinterSettings.CheckPrinterName = chkName;
            if (PrinterSettings.DeliveryPrinterID.HasValue && printerDict.TryGetValue(PrinterSettings.DeliveryPrinterID.Value, out var delName))
                PrinterSettings.DeliveryPrinterName = delName;
            if (PrinterSettings.InvoicePrinterID.HasValue && printerDict.TryGetValue(PrinterSettings.InvoicePrinterID.Value, out var invName))
                PrinterSettings.InvoicePrinterName = invName;
            if (PrinterSettings.ReportPrinterID.HasValue && printerDict.TryGetValue(PrinterSettings.ReportPrinterID.Value, out var repName))
                PrinterSettings.ReportPrinterName = repName;
            if (PrinterSettings.AdditionPrinterID.HasValue && printerDict.TryGetValue(PrinterSettings.AdditionPrinterID.Value, out var addName))
                PrinterSettings.AdditionPrinterName = addName;
            if (PrinterSettings.ReportA4PrinterID.HasValue && printerDict.TryGetValue(PrinterSettings.ReportA4PrinterID.Value, out var r4Name))
                PrinterSettings.ReportA4PrinterName = r4Name;
            if (PrinterSettings.LabelPrinterID.HasValue && printerDict.TryGetValue(PrinterSettings.LabelPrinterID.Value, out var lblName))
                PrinterSettings.LabelPrinterName = lblName;
            if (PrinterSettings.ReturnPrinterID.HasValue && printerDict.TryGetValue(PrinterSettings.ReturnPrinterID.Value, out var retName))
                PrinterSettings.ReturnPrinterName = retName;

            // Map Kitchen Printer Names
            if (PrinterSettings.Kitchen1PrinterID.HasValue && printerDict.TryGetValue(PrinterSettings.Kitchen1PrinterID.Value, out var k1)) PrinterSettings.Kitchen1PrinterName = k1;
            if (PrinterSettings.Kitchen2PrinterID.HasValue && printerDict.TryGetValue(PrinterSettings.Kitchen2PrinterID.Value, out var k2)) PrinterSettings.Kitchen2PrinterName = k2;
            if (PrinterSettings.Kitchen3PrinterID.HasValue && printerDict.TryGetValue(PrinterSettings.Kitchen3PrinterID.Value, out var k3)) PrinterSettings.Kitchen3PrinterName = k3;
            if (PrinterSettings.Kitchen4PrinterID.HasValue && printerDict.TryGetValue(PrinterSettings.Kitchen4PrinterID.Value, out var k4)) PrinterSettings.Kitchen4PrinterName = k4;
            if (PrinterSettings.Kitchen5PrinterID.HasValue && printerDict.TryGetValue(PrinterSettings.Kitchen5PrinterID.Value, out var k5)) PrinterSettings.Kitchen5PrinterName = k5;
            if (PrinterSettings.Kitchen6PrinterID.HasValue && printerDict.TryGetValue(PrinterSettings.Kitchen6PrinterID.Value, out var k6)) PrinterSettings.Kitchen6PrinterName = k6;
            if (PrinterSettings.Kitchen7PrinterID.HasValue && printerDict.TryGetValue(PrinterSettings.Kitchen7PrinterID.Value, out var k7)) PrinterSettings.Kitchen7PrinterName = k7;
            if (PrinterSettings.Kitchen8PrinterID.HasValue && printerDict.TryGetValue(PrinterSettings.Kitchen8PrinterID.Value, out var k8)) PrinterSettings.Kitchen8PrinterName = k8;
            if (PrinterSettings.Kitchen9PrinterID.HasValue && printerDict.TryGetValue(PrinterSettings.Kitchen9PrinterID.Value, out var k9)) PrinterSettings.Kitchen9PrinterName = k9;
            if (PrinterSettings.Kitchen10PrinterID.HasValue && printerDict.TryGetValue(PrinterSettings.Kitchen10PrinterID.Value, out var k10)) PrinterSettings.Kitchen10PrinterName = k10;
            if (PrinterSettings.Kitchen11PrinterID.HasValue && printerDict.TryGetValue(PrinterSettings.Kitchen11PrinterID.Value, out var k11)) PrinterSettings.Kitchen11PrinterName = k11;
            if (PrinterSettings.Kitchen12PrinterID.HasValue && printerDict.TryGetValue(PrinterSettings.Kitchen12PrinterID.Value, out var k12)) PrinterSettings.Kitchen12PrinterName = k12;
            if (PrinterSettings.Kitchen13PrinterID.HasValue && printerDict.TryGetValue(PrinterSettings.Kitchen13PrinterID.Value, out var k13)) PrinterSettings.Kitchen13PrinterName = k13;
            if (PrinterSettings.Kitchen14PrinterID.HasValue && printerDict.TryGetValue(PrinterSettings.Kitchen14PrinterID.Value, out var k14)) PrinterSettings.Kitchen14PrinterName = k14;
            if (PrinterSettings.Kitchen15PrinterID.HasValue && printerDict.TryGetValue(PrinterSettings.Kitchen15PrinterID.Value, out var k15)) PrinterSettings.Kitchen15PrinterName = k15;
            if (PrinterSettings.Kitchen16PrinterID.HasValue && printerDict.TryGetValue(PrinterSettings.Kitchen16PrinterID.Value, out var k16)) PrinterSettings.Kitchen16PrinterName = k16;
            if (PrinterSettings.Kitchen17PrinterID.HasValue && printerDict.TryGetValue(PrinterSettings.Kitchen17PrinterID.Value, out var k17)) PrinterSettings.Kitchen17PrinterName = k17;
            if (PrinterSettings.Kitchen18PrinterID.HasValue && printerDict.TryGetValue(PrinterSettings.Kitchen18PrinterID.Value, out var k18)) PrinterSettings.Kitchen18PrinterName = k18;
            if (PrinterSettings.Kitchen19PrinterID.HasValue && printerDict.TryGetValue(PrinterSettings.Kitchen19PrinterID.Value, out var k19)) PrinterSettings.Kitchen19PrinterName = k19;
            if (PrinterSettings.Kitchen20PrinterID.HasValue && printerDict.TryGetValue(PrinterSettings.Kitchen20PrinterID.Value, out var k20)) PrinterSettings.Kitchen20PrinterName = k20;

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

public class KitchenPrinterSlot : ObservableObject
{
    public int SlotNo { get; set; }
    public string SlotName { get; set; } = string.Empty;
    public int? PrinterID { get; set; }
    public string? DesignPath { get; set; }
}