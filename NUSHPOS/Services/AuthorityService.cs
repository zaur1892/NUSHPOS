using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Dapper;
using NUSHPOS.Helpers;
using NUSHPOS.Models;
using NUSHPOS.ViewModels;

namespace NUSHPOS.Services;

public class AuthorityService
{
    private readonly DatabaseService _db;
    private readonly AccessLogService _accessLogService;
    private readonly EmployeeService _employeeService;
    private ConcurrentDictionary<string, AuthorityItem>? _cache;

    public AuthorityService(
        DatabaseService db, 
        AccessLogService accessLogService,
        EmployeeService employeeService)
    {
        _db = db;
        _accessLogService = accessLogService;
        _employeeService = employeeService;
    }

    public async Task<List<AuthorityItem>> GetAuthoritiesAsync()
    {
        using var connection = _db.CreateConnection();
        const string sql = @"
            SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
            SELECT 
                [AuthorityID], 
                ISNULL([GroupName], '') AS [GroupName], 
                CAST([AuthorityKey] AS NVARCHAR(100)) AS [AuthorityKey], 
                ISNULL([AuthorityText], '') AS [AuthorityText], 
                [AuthorityDescription], 
                ISNULL([DefaultLevel], 0) AS [DefaultLevel], 
                ISNULL([NeedAllways], 0) AS [NeedAllways], 
                ISNULL([AllowManager], 0) AS [AllowManager], 
                ISNULL([AllowCashier], 0) AS [AllowCashier],
                CAST([EditKey] AS NVARCHAR(100)) AS [EditKey], 
                CAST([SyncKey] AS NVARCHAR(100)) AS [SyncKey], 
                [BranchID] 
            FROM [AuthorityList]  
            WHERE ISNULL([DefaultLevel], 0) <= 10
            ORDER BY [AuthorityID];
        ";

        var list = (await connection.QueryAsync<AuthorityItem>(sql)).ToList();
        
        var dict = new ConcurrentDictionary<string, AuthorityItem>(StringComparer.OrdinalIgnoreCase);
        foreach (var item in list)
        {
            if (!string.IsNullOrWhiteSpace(item.AuthorityKey))
            {
                dict[item.AuthorityKey] = item;
            }
        }
        _cache = dict;

        return list;
    }

    public async Task<AuthorityItem?> GetAuthorityByKeyAsync(string authorityKey)
    {
        if (string.IsNullOrWhiteSpace(authorityKey)) return null;

        if (_cache != null && _cache.TryGetValue(authorityKey, out var cached))
        {
            return cached;
        }

        var all = await GetAuthoritiesAsync();
        return all.FirstOrDefault(a => string.Equals(a.AuthorityKey, authorityKey, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<List<string>> GetGroupNamesAsync()
    {
        using var connection = _db.CreateConnection();
        const string sql = @"
            SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
            SELECT 
                GroupName  
            FROM [AuthorityList] 
            WHERE GroupName IS NOT NULL AND GroupName <> ''
            GROUP BY GroupName
            ORDER BY GroupName;
        ";

        var groups = await connection.QueryAsync<string>(sql);
        return groups.ToList();
    }

    public async Task<bool> SaveAuthoritiesAsync(IEnumerable<AuthorityItem> items)
    {
        using var connection = _db.CreateConnection();
        const string updateSql = @"
            UPDATE [AuthorityList] SET
                [DefaultLevel] = @DefaultLevel,
                [NeedAllways] = @NeedAllways,
                [AllowManager] = @AllowManager,
                [AllowCashier] = @AllowCashier,
                [EditKey] = NEWID(),
                [SyncKey] = NEWID()
            WHERE [AuthorityID] = @AuthorityID;
        ";

        int count = 0;
        foreach (var item in items)
        {
            count += await connection.ExecuteAsync(updateSql, new
            {
                item.DefaultLevel,
                item.NeedAllways,
                item.AllowManager,
                item.AllowCashier,
                item.AuthorityID
            });
        }

        _cache = null; // Invalidate cache
        return count > 0;
    }

    /// <summary>
    /// Validates access for the given action according to security level and NeedAllways (Zorunlu / Məcburi).
    /// If required, prompts for Manager Authorization PIN dialog.
    /// </summary>
    public async Task<bool> ValidateActionAccessAsync(string authorityKey, string? defaultActionTitle = null)
    {
        var authority = await GetAuthorityByKeyAsync(authorityKey);
        if (authority == null)
        {
            // If action is not in AuthorityList, allow access by default
            return true;
        }

        string actionTitle = !string.IsNullOrWhiteSpace(authority.AuthorityText) 
            ? authority.AuthorityText 
            : (defaultActionTitle ?? authorityKey);

        int currentLevel = SessionManager.SecurityLevel;
        int requiredLevel = authority.DefaultLevel;
        bool needAlways = authority.NeedAllways;

        // If level requirement is 0 or user level is high enough AND not mandatory forced password
        if (!needAlways && currentLevel >= requiredLevel)
        {
            return true;
        }

        // If access requires authorization or is forced (NeedAllways = true)
        bool isAuthorized = false;
        Employee? authorizedEmp = null;

        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            var vm = new ManagerAuthDialogViewModel(_employeeService, actionTitle, requiredLevel, needAlways);
            var dlg = new Views.Dialogs.ManagerAuthDialog { DataContext = vm };
            vm.RequestClose = () => dlg.Close();
            dlg.ShowDialog();

            if (vm.IsAuthorized)
            {
                isAuthorized = true;
                authorizedEmp = vm.AuthorizedEmployee;
            }
        });

        // Log to AccessLogs
        try
        {
            await _accessLogService.InsertAccessLogAsync(
                branchId: SessionManager.BranchID,
                stationId: SessionManager.StationID,
                employeeId: authorizedEmp != null ? authorizedEmp.EmployeeID : SessionManager.EmployeeID,
                actionName: actionTitle,
                wrongPassword: "",
                additionalInfo: isAuthorized ? "Səlahiyyət təsdiqləndi" : "Səlahiyyət rədd edildi",
                isSuccess: isAuthorized,
                orderKey: Guid.Empty.ToString(),
                transactionKey: Guid.Empty.ToString()
            );
        }
        catch { }

        return isAuthorized;
    }

    public async Task LogSecurityAccessAsync()
    {
        try
        {
            await _accessLogService.InsertAccessLogAsync(
                branchId: SessionManager.BranchID,
                stationId: SessionManager.StationID,
                employeeId: SessionManager.EmployeeID,
                actionName: "TƏHLÜKƏSİZLİK SAZLAMALARINA GİRİŞ",
                wrongPassword: "",
                additionalInfo: "Təhlükəsizlik sazlamaları açıldı",
                isSuccess: true,
                orderKey: Guid.Empty.ToString(),
                transactionKey: Guid.Empty.ToString()
            );
        }
        catch { }
    }
}
