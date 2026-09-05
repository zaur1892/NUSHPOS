using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using NUSHPOS.Models;
using NUSHPOS.Services;
using NUSHPOS.ViewModels.Base;

namespace NUSHPOS.ViewModels;

public partial class DesignEditorViewModel : ViewModelBase
{
    private readonly StationSettingsService _settingsService;
    private readonly FastReportService _fastReportService;
    private PrinterDesign _currentDesign = new();

    [ObservableProperty]
    private string _designName = string.Empty;

    [ObservableProperty]
    private string _designData = string.Empty;

    [ObservableProperty]
    private int? _documentTypeID = 1;

    [ObservableProperty]
    private bool _isDefault = false;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    public event Action? RequestClose;

    public DesignEditorViewModel(StationSettingsService settingsService, FastReportService fastReportService)
    {
        _settingsService = settingsService;
        _fastReportService = fastReportService;
        Title = "ÇAP DİZAYN REDAKTORU";
    }

    public async Task InitializeAsync(string designName)
    {
        DesignName = designName;
        StatusMessage = "Şablon yüklənir...";

        try
        {
            var design = await _settingsService.GetPrinterDesignByNameAsync(designName);
            if (design != null)
            {
                _currentDesign = design;
                DesignName = design.DesignName ?? designName;
                DocumentTypeID = design.DocumentTypeID ?? 1;
                IsDefault = design.IsDefault ?? false;
                DesignData = string.IsNullOrWhiteSpace(design.DesignData) 
                    ? _fastReportService.GetDefaultReceiptTemplate(designName) 
                    : design.DesignData;
                StatusMessage = "Şablon bazadan yükləndi.";
            }
            else
            {
                _currentDesign = new PrinterDesign
                {
                    DesignName = designName,
                    DocumentTypeID = 1,
                    IsDefault = false
                };
                DesignData = _fastReportService.GetDefaultReceiptTemplate(designName);
                StatusMessage = "Yeni standart şablon yaradıldı.";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Xəta: {ex.Message}";
        }
    }

    [RelayCommand]
    private void OpenExternalDesigner()
    {
        try
        {
            string tempFrx = _fastReportService.ExportToTempFrx(DesignName, DesignData);
            bool launched = _fastReportService.TryLaunchExternalDesigner(tempFrx);
            if (launched)
            {
                StatusMessage = $"Dizayner açıldı: {Path.GetFileName(tempFrx)}. Redaktədən sonra 'Şablonu İdxal Et' ilə yeniləyə bilərsiniz.";
                MessageBox.Show($"Şablon müvəqqəti fayla ixrac edildi:\n{tempFrx}\n\nFastReport redaktorunda dəyişiklik edib yadda saxladıqdan sonra 'Şablonu İdxal Et' düyməsi ilə faylı yenidən yükləyə bilərsiniz.", "FastReport Dizayner", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Sistemdə FastReport Designer tapılmadı. Faylı əl ilə açmaq üçün '.frx' ixrac edə və ya aşağıdakı XML kodunu birbaşa redaktə edə bilərsiniz.", "Məlumat", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Dizayner açıla bilmədi: {ex.Message}", "Xəta", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private void ImportFrx()
    {
        var dialog = new OpenFileDialog
        {
            Filter = "FastReport Şablonları (*.frx)|*.frx|XML Faylları (*.xml)|*.xml|Bütün Fayllar (*.*)|*.*"
        };

        if (dialog.ShowDialog() == true)
        {
            try
            {
                DesignData = File.ReadAllText(dialog.FileName);
                StatusMessage = $"'{Path.GetFileName(dialog.FileName)}' faylından uğurla idxal edildi.";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fayl oxunarkən xəta: {ex.Message}", "Xəta", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    [RelayCommand]
    private void ExportFrx()
    {
        var dialog = new SaveFileDialog
        {
            FileName = $"{DesignName}.frx",
            Filter = "FastReport Şablonları (*.frx)|*.frx|XML Faylları (*.xml)|*.xml"
        };

        if (dialog.ShowDialog() == true)
        {
            try
            {
                File.WriteAllText(dialog.FileName, DesignData);
                StatusMessage = $"'{Path.GetFileName(dialog.FileName)}' ünvanına saxlanıldı.";
                MessageBox.Show("Şablon uğurla ixrac edildi!", "Məlumat", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fayl saxlanılarkən xəta: {ex.Message}", "Xəta", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    [RelayCommand]
    private void Preview()
    {
        if (string.IsNullOrWhiteSpace(DesignData))
        {
            MessageBox.Show("Ön baxış üçün şablon məzmunu boş ola bilməz.", "Xəbərdarlıq", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            string pdfPath = _fastReportService.GeneratePreviewPdf(DesignData, DesignName);
            Process.Start(new ProcessStartInfo
            {
                FileName = pdfPath,
                UseShellExecute = true
            });
            StatusMessage = "Ön baxış PDF faylı açıldı.";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ön baxış yaradılarkən xəta: {ex.Message}", "Xəta", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private void ResetDefaultTemplate()
    {
        var result = MessageBox.Show("Cari şablon standart qəbz dizaynı ilə əvəz edilsin?", "Təsdiq", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (result == MessageBoxResult.Yes)
        {
            DesignData = _fastReportService.GetDefaultReceiptTemplate(DesignName);
            StatusMessage = "Standart şablon bərpa edildi.";
        }
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(DesignName))
        {
            MessageBox.Show("Zəhmət olmasa dizayn adını daxil edin.", "Xəbərdarlıq", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            _currentDesign.DesignName = DesignName;
            _currentDesign.DesignData = DesignData;
            _currentDesign.DocumentTypeID = DocumentTypeID;
            _currentDesign.IsDefault = IsDefault;

            bool saved = await _settingsService.SavePrinterDesignAsync(_currentDesign);
            if (saved)
            {
                MessageBox.Show("Dizayn şablonu uğurla yadda saxlanıldı!", "Məlumat", MessageBoxButton.OK, MessageBoxImage.Information);
                RequestClose?.Invoke();
            }
            else
            {
                MessageBox.Show("Yadda saxlamaq mümkün olmadı.", "Xəta", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Xəta: {ex.Message}", "Xəta", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private void Cancel()
    {
        RequestClose?.Invoke();
    }
}
