using Dapper;
using NUSHPOS.Models;

namespace NUSHPOS.Services;

public class DiscountService
{
    private readonly DatabaseService _db;

    public DiscountService(DatabaseService db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Discount>> GetActiveDiscountsAsync()
    {
        using var connection = _db.CreateConnection();
        const string sql = "SELECT DiscountID, CAST(DiscountKey AS NVARCHAR(50)) AS DiscountKey, DiscountText, DiscountDescription, DiscountActive, DiscountAmount, DiscountBasis, DiscountExpireDate, Barcode, ButtonColor, PictureName, SecurityLevel, BranchID, UseGroupFilter, UseMenuItemFilter FROM Discounts WHERE ISNULL(DiscountActive, 1) = 1 ORDER BY DiscountID";
        return await connection.QueryAsync<Discount>(sql);
    }

    public async Task<Discount?> GetDiscountByIdAsync(int discountId)
    {
        using var connection = _db.CreateConnection();
        const string sql = "SELECT AutoID, DiscountID, CAST(DiscountKey AS NVARCHAR(50)) AS DiscountKey, DiscountText, DiscountDescription, DiscountActive, DiscountAmount, DiscountBasis, DiscountExpireDate, Barcode, ButtonColor, PictureName, SecurityLevel, BranchID, UseGroupFilter, UseMenuItemFilter FROM Discounts WHERE DiscountID = @DiscountId";
        return await connection.QueryFirstOrDefaultAsync<Discount>(sql, new { DiscountId = discountId });
    }

    public async Task<(bool Success, string Message)> ApplyCheckDiscountAsync(string orderKey, string discountKey, int employeeId, int stationId, string oldEditKey)
    {
        using var connection = _db.CreateConnection();
        var parameters = new DynamicParameters();
        
        // C# Guid strings to SQL UNIQUEIDENTIFIER mapping handled automatically by Dapper if strings are valid GUIDs,
        // or we can convert them to Guid first for strictness.
        parameters.Add("@OrderKey", Guid.Parse(orderKey));
        parameters.Add("@DiscountKey", Guid.Parse(discountKey));
        parameters.Add("@EmployeeID", employeeId);
        parameters.Add("@StationID", stationId);
        parameters.Add("@OldEditKey", Guid.Parse(oldEditKey));
        
        parameters.Add("@Success", dbType: System.Data.DbType.Boolean, direction: System.Data.ParameterDirection.Output);
        parameters.Add("@Message", dbType: System.Data.DbType.String, size: 255, direction: System.Data.ParameterDirection.Output);

        await connection.ExecuteAsync("sp_ApplyCheckDiscount", parameters, commandType: System.Data.CommandType.StoredProcedure);

        bool success = parameters.Get<bool>("@Success");
        string message = parameters.Get<string>("@Message");

        return (success, message);
    }
}
