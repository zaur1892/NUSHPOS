using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dapper;
using NUSHPOS.Services;
using NUSHPOS.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace NUSHPOS.ViewModels;

public class StoreSettingModel : ObservableObject
{
    public string TabName { get; set; } = "";
    public string GroupName { get; set; } = "";
    public string ParamName { get; set; } = "";
    public string ParamKey { get; set; } = "";
    
    private string _paramValue = "";
    public string ParamValue
    {
        get => _paramValue;
        set => SetProperty(ref _paramValue, value);
    }
    
    public string DefaultValue { get; set; } = "";
    public string ParamType { get; set; } = ""; // E.g., boolean, string, list
    
    // UI Helpers
    public bool IsBoolean => ParamType?.ToLower() == "boolean" || ParamValue?.ToLower() == "true" || ParamValue?.ToLower() == "false" || ParamValue == "0" || ParamValue == "1" && (ParamType == null || ParamType == ""); 
    
    public bool BooleanValue
    {
        get => ParamValue == "True" || ParamValue == "1";
        set => ParamValue = value ? "True" : "False";
    }
}

public class SettingGroup
{
    public string GroupName { get; set; } = "";
    public ObservableCollection<StoreSettingModel> Settings { get; set; } = new();
}

public partial class CompanySettingsViewModel : ViewModelBase
{
    private readonly DatabaseService _databaseService;

    [ObservableProperty]
    private ObservableCollection<string> _tabs = new();

    [ObservableProperty]
    private string _selectedTab = "";

    [ObservableProperty]
    private ObservableCollection<SettingGroup> _currentTabGroups = new();

    private List<StoreSettingModel> _allSettings = new();

    public CompanySettingsViewModel(DatabaseService databaseService)
    {
        _databaseService = databaseService;
        _ = LoadSettingsAsync();
    }

    private async Task LoadSettingsAsync()
    {
        try
        {
            using var connection = _databaseService.CreateConnection();
            
            string query = @"
                SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
                SELECT 
                    [TabName], [GroupName], [ParamName], 
                    [ParamKey], [ParamValue], [DefaultValue], [ParamType]
                FROM [StoreSettings]  
                WHERE TabName <> 'GÜN SONU İŞLEMLERİ' 
                  AND TabName <> 'PERSONEL' 
                  AND TabName <> 'PARA PUAN'
                ORDER BY TabName, OrderID;
            ";

            var settings = await connection.QueryAsync<StoreSettingModel>(query);
            _allSettings = settings.ToList();

            var tabNames = _allSettings.Select(s => s.TabName).Distinct().ToList();
            Tabs = new ObservableCollection<string>(tabNames);

            if (Tabs.Any())
            {
                SelectedTab = Tabs.First();
                UpdateCurrentTabGroups();
            }
        }
        catch
        {
            // Ignore errors for now
        }
    }

    [RelayCommand]
    private void SelectTab(string tabName)
    {
        SelectedTab = tabName;
        UpdateCurrentTabGroups();
    }

    private void UpdateCurrentTabGroups()
    {
        var settingsForTab = _allSettings.Where(s => s.TabName == SelectedTab).ToList();
        
        var groups = settingsForTab.GroupBy(s => s.GroupName).Select(g => new SettingGroup
        {
            GroupName = string.IsNullOrEmpty(g.Key) ? "DİGƏR" : g.Key,
            Settings = new ObservableCollection<StoreSettingModel>(g)
        }).ToList();

        CurrentTabGroups = new ObservableCollection<SettingGroup>(groups);
    }

    [RelayCommand]
    private async Task SaveAndClose()
    {
        // TODO: Save to database using _databaseService
        
        // Close the subview
        var backOfficeVm = App.Services.GetService(typeof(BackOfficeViewModel)) as BackOfficeViewModel;
        if (backOfficeVm != null)
        {
            backOfficeVm.CurrentSubView = null;
        }
    }

    [RelayCommand]
    private void Cancel()
    {
        var backOfficeVm = App.Services.GetService(typeof(BackOfficeViewModel)) as BackOfficeViewModel;
        if (backOfficeVm != null)
        {
            backOfficeVm.CurrentSubView = null;
        }
    }
}
