using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NUSHPOS.Models;
using NUSHPOS.Services;
using NUSHPOS.ViewModels.Base;

namespace NUSHPOS.ViewModels;

public partial class SecuritySettingsViewModel : ViewModelBase
{
    private readonly AuthorityService _authorityService;

    public event Action? RequestClose;

    [ObservableProperty]
    private ObservableCollection<AuthorityItem> _allAuthorities = new();

    [ObservableProperty]
    private ObservableCollection<AuthorityItem> _filteredAuthorities = new();

    [ObservableProperty]
    private ObservableCollection<string> _groups = new();

    [ObservableProperty]
    private string _selectedGroup = "HAMISI";

    [ObservableProperty]
    private string _searchText = string.Empty;

    public int[] LevelOptions { get; } = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

    public SecuritySettingsViewModel(AuthorityService authorityService)
    {
        _authorityService = authorityService;
        Title = "Təhlükəsizlik Sazlamaları";

        _ = InitializeAsync();
    }

    public async Task InitializeAsync()
    {
        IsBusy = true;
        try
        {
            await _authorityService.LogSecurityAccessAsync();

            var groups = await _authorityService.GetGroupNamesAsync();
            var items = await _authorityService.GetAuthoritiesAsync();

            Groups.Clear();
            Groups.Add("HAMISI");
            foreach (var g in groups)
            {
                if (!Groups.Contains(g, StringComparer.OrdinalIgnoreCase))
                {
                    Groups.Add(g);
                }
            }

            AllAuthorities = new ObservableCollection<AuthorityItem>(items);
            ApplyFilter();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Təhlükəsizlik sazlamaları yüklənərkən xəta baş verdi: {ex.Message}", "Xəta", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            IsBusy = false;
        }
    }

    partial void OnSelectedGroupChanged(string value)
    {
        ApplyFilter();
    }

    partial void OnSearchTextChanged(string value)
    {
        ApplyFilter();
    }

    [RelayCommand]
    private void SelectGroup(string group)
    {
        SelectedGroup = group;
    }

    [RelayCommand]
    private void ClearSearch()
    {
        SearchText = string.Empty;
    }

    private void ApplyFilter()
    {
        IEnumerable<AuthorityItem> query = AllAuthorities;

        if (!string.IsNullOrWhiteSpace(SelectedGroup) && 
            !SelectedGroup.Equals("HAMISI", StringComparison.OrdinalIgnoreCase) && 
            !SelectedGroup.Equals("TÜMÜ", StringComparison.OrdinalIgnoreCase))
        {
            query = query.Where(a => string.Equals(a.GroupName, SelectedGroup, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            var search = SearchText.Trim().ToLower();
            query = query.Where(a => 
                (!string.IsNullOrEmpty(a.AuthorityText) && a.AuthorityText.ToLower().Contains(search)) ||
                (!string.IsNullOrEmpty(a.AuthorityKey) && a.AuthorityKey.ToLower().Contains(search)) ||
                (!string.IsNullOrEmpty(a.GroupName) && a.GroupName.ToLower().Contains(search)));
        }

        FilteredAuthorities = new ObservableCollection<AuthorityItem>(query);
    }

    [RelayCommand]
    private async Task SaveAndClose()
    {
        IsBusy = true;
        try
        {
            bool success = await _authorityService.SaveAuthoritiesAsync(AllAuthorities);
            if (success)
            {
                MessageBox.Show("Təhlükəsizlik və icazə sazlamaları uğurla yadda saxlanıldı.", "Uğurlu", MessageBoxButton.OK, MessageBoxImage.Information);
                RequestClose?.Invoke();
            }
            else
            {
                MessageBox.Show("Dəyişikliklər qeydə alınmadı.", "Məlumat", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Yadda saxlanarkən xəta: {ex.Message}", "Xəta", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void Cancel()
    {
        RequestClose?.Invoke();
    }
}
