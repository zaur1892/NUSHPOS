using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace NUSHPOS.Models;

public partial class AuthorityItem : ObservableObject
{
    [ObservableProperty] private int _authorityID;
    [ObservableProperty] private string _groupName = "";
    [ObservableProperty] private string? _authorityKey;
    [ObservableProperty] private string _authorityText = "";
    [ObservableProperty] private string? _authorityDescription;
    [ObservableProperty] private int _defaultLevel;
    [ObservableProperty] private bool _needAllways;
    [ObservableProperty] private bool _allowManager;
    [ObservableProperty] private bool _allowCashier;
    [ObservableProperty] private string? _editKey;
    [ObservableProperty] private string? _syncKey;
    [ObservableProperty] private int? _branchID;

    // Helper for Level Degree bar width (0..100%)
    public double LevelPercent => Math.Clamp(DefaultLevel * 10.0, 0, 100);

    partial void OnDefaultLevelChanged(int value)
    {
        OnPropertyChanged(nameof(LevelPercent));
    }
}
