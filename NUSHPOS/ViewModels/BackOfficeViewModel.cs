using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NUSHPOS.Helpers;
using NUSHPOS.Services;
using NUSHPOS.ViewModels.Base;
using System;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.DependencyInjection;

namespace NUSHPOS.ViewModels;

public partial class BackOfficeViewModel : ViewModelBase
{
    private readonly NavigationService _navigationService;
    private readonly DatabaseService _databaseService;

    [ObservableProperty]
    private string _dbPath = "";

    [ObservableProperty]
    private string _dbSize = "0 MB";

    [ObservableProperty]
    private string _orderCount = "0";

    [ObservableProperty]
    private string _customerCount = "0";

    [ObservableProperty]
    private string _employeeCount = "0";

    [ObservableProperty]
    private string _backupDate = "-";

    [ObservableProperty]
    private string _licenseCustomer = "STEAK BAKU";

    [ObservableProperty]
    private string _serverName = "TERMSRV";

    public BackOfficeViewModel(NavigationService navigationService, DatabaseService databaseService)
    {
        _navigationService = navigationService;
        _databaseService = databaseService;
        Title = "ARXA OFİS";

        _ = LoadDashboardDataAsync();
    }

    private async Task LoadDashboardDataAsync()
    {
        try
        {
            using var connection = _databaseService.CreateConnection();
            
            string query = @"
                SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

                SELECT TOP 1 
                    'DbPath' AS Textname,
                    [filename] AS TextValue 
                FROM sysfiles 
                WHERE fileid = 1

                UNION ALL 

                SELECT TOP 1 
                    'DbSize' AS Textname,
                    CAST(SUM((SIZE * 8) / 1024) AS NVARCHAR(50)) + ' MB' AS TextValue 
                FROM sys.master_files 
                WHERE physical_name LIKE '%' + DB_NAME() + '.%'

                UNION ALL 

                SELECT 
                    'OrderCount' AS Textname,
                    CAST(COUNT(AutoID) AS NVARCHAR(50)) AS TextValue
                FROM OrderHeaders WITH(NOLOCK)

                UNION ALL 

                SELECT 
                    'CustomerCount' AS Textname,
                    CAST(COUNT(AutoID) AS NVARCHAR(50)) AS TextValue
                FROM CustomerFiles WITH(NOLOCK)

                UNION ALL 

                SELECT 
                    'EmployeeCount' AS Textname,
                    CAST(COUNT(AutoID) AS NVARCHAR(50)) AS TextValue
                FROM EmployeeFiles WITH(NOLOCK)

                UNION ALL 

                SELECT 
                    'BackupDate' AS Textname,
                    ISNULL((SELECT TOP 1 ParamValue FROM Params WITH(NOLOCK) WHERE ParamName = 'BackupDate'), '-') AS TextValue;
            ";

            var results = await connection.QueryAsync<(string Textname, string TextValue)>(query);

            foreach (var result in results)
            {
                switch (result.Textname)
                {
                    case "DbPath":
                        DbPath = result.TextValue;
                        break;
                    case "DbSize":
                        DbSize = result.TextValue;
                        break;
                    case "OrderCount":
                        OrderCount = result.TextValue;
                        break;
                    case "CustomerCount":
                        CustomerCount = result.TextValue;
                        break;
                    case "EmployeeCount":
                        EmployeeCount = result.TextValue;
                        break;
                    case "BackupDate":
                        BackupDate = result.TextValue;
                        break;
                }
            }
        }
        catch
        {
            // Ignore errors, leave defaults
        }
    }

    [ObservableProperty]
    private object? _currentSubView;

    [ObservableProperty]
    private bool _isMenuSettingsExpanded = true;

    [ObservableProperty]
    private string _activeSidebarSection = "MenuAyarlari";

    [RelayCommand]
    private void NavigateBack()
    {
        if (CurrentSubView != null)
        {
            CurrentSubView = null;
        }
        else
        {
            _navigationService.NavigateTo<MainScreenViewModel>();
        }
    }

    [RelayCommand]
    private void OpenCompanySettings()
    {
        ActiveSidebarSection = "GenelAyarlar";
        CurrentSubView = App.Services.GetService(typeof(CompanySettingsViewModel));
    }

    [RelayCommand]
    private void OpenEmployeeSettings()
    {
        ActiveSidebarSection = "PersonelAyarlari";
        CurrentSubView = App.Services.GetService(typeof(EmployeeSettingsViewModel));
    }

    [RelayCommand]
    private void ToggleMenuSettings()
    {
        ActiveSidebarSection = "MenuAyarlari";
        IsMenuSettingsExpanded = !IsMenuSettingsExpanded;
    }

    [RelayCommand]
    private void OpenMenuDesigner()
    {
        ActiveSidebarSection = "MenuAyarlari";
        var vm = App.Services.GetService(typeof(MenuDesignerViewModel)) as MenuDesignerViewModel;
        if (vm != null)
        {
            vm.RequestClose += () => CurrentSubView = null;
            CurrentSubView = vm;
        }
    }

    [RelayCommand]
    private void OpenMenuScreenGroups()
    {
        ActiveSidebarSection = "MenuAyarlari";
        OpenMenuDesigner();
    }

    [RelayCommand]
    private void OpenModifierGroups()
    {
        ActiveSidebarSection = "MenuAyarlari";
        System.Windows.MessageBox.Show("Məhsul Mesaj Qrupları bölməsi tezliklə aktiv olacaq.", "Məlumat", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
    }

    [RelayCommand]
    private void OpenModifiers()
    {
        ActiveSidebarSection = "MenuAyarlari";
        System.Windows.MessageBox.Show("Məhsul Mesajları bölməsi tezliklə aktiv olacaq.", "Məlumat", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
    }

    [RelayCommand]
    private void OpenComboMenus()
    {
        ActiveSidebarSection = "MenuAyarlari";
        System.Windows.MessageBox.Show("Kombo Menyular bölməsi tezliklə aktiv olacaq.", "Məlumat", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
    }

    [RelayCommand]
    private void OpenTableSettings()
    {
        ActiveSidebarSection = "MasaAyarlari";
        System.Windows.MessageBox.Show("Masa Sazlamaları bölməsi tezliklə aktiv olacaq.", "Məlumat", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
    }

    [RelayCommand]
    private void OpenDiscountSettings()
    {
        ActiveSidebarSection = "IndirimAyarlari";
        System.Windows.MessageBox.Show("Endirim, Kampaniya və Ödəniş sazlamaları tezliklə aktiv olacaq.", "Məlumat", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
    }

    [RelayCommand]
    private void OpenOperations()
    {
        ActiveSidebarSection = "Islemler";
        System.Windows.MessageBox.Show("Əməliyyatlar bölməsi tezliklə aktiv olacaq.", "Məlumat", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
    }
}

