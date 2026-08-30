using Dapper;
using NUSHPOS.Models;

namespace NUSHPOS.Services;

public class SettingsService
{
    private readonly DatabaseService _db;

    public SettingsService(DatabaseService db)
    {
        _db = db;
    }

    public async Task<IEnumerable<StoreSettings>> GetStoreSettingsAsync()
    {
        using var connection = _db.CreateConnection();
        const string sql = "SELECT * FROM StoreSettings ORDER BY TabName, GroupName, OrderID";
        return await connection.QueryAsync<StoreSettings>(sql);
    }

    public async Task UpdateSettingAsync(int settingsId, string value)
    {
        using var connection = _db.CreateConnection();
        const string sql = "UPDATE StoreSettings SET ParamValue = @ParamValue, EditDateTime = @EditDateTime WHERE SettingsID = @SettingsId";
        await connection.ExecuteAsync(sql, new { SettingsId = settingsId, ParamValue = value, EditDateTime = DateTime.Now });
    }

    public async Task<string> IsHappyHoursActiveAsync()
    {
        using var connection = _db.CreateConnection();
        const string sql = "SELECT isnull((SELECT TOP 1 isnull(ParamValue,'NO') FROM StoreSettings WHERE ParamKey='HappyHoursActive'),'NO') AS ParamValue";
        return await connection.ExecuteScalarAsync<string>(sql) ?? "NO";
    }
}
