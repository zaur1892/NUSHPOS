using Dapper;
using System;
using System.Threading.Tasks;

namespace NUSHPOS.Services;

public class AccessLogService
{
    private readonly DatabaseService _db;

    public AccessLogService(DatabaseService db)
    {
        _db = db;
    }

    public async Task InsertAccessLogAsync(
        int branchId, 
        int stationId, 
        int employeeId, 
        string actionName, 
        string wrongPassword, 
        string additionalInfo, 
        bool isSuccess, 
        string orderKey, 
        string transactionKey)
    {
        using var connection = _db.CreateConnection();
        const string sql = @"
            INSERT INTO [AccessLogs] (
                [BranchID], [LogDate], [StationID], [EmployeeID], [ActionName], 
                [WrongPassword], [AdditionalInfo], [IsSuccess], [OrderKey], 
                [TransactionKey], [AccessLogKey], [EditKey], [SyncKey]
            ) 
            VALUES (
                @BranchID, GETDATE(), @StationID, @EmployeeID, @ActionName, 
                @WrongPassword, @AdditionalInfo, @IsSuccess, @OrderKey, 
                @TransactionKey, NEWID(), NEWID(), NEWID()
            )";

        var parameters = new
        {
            BranchID = branchId,
            StationID = stationId,
            EmployeeID = employeeId,
            ActionName = actionName,
            WrongPassword = wrongPassword,
            AdditionalInfo = additionalInfo,
            IsSuccess = isSuccess,
            OrderKey = string.IsNullOrEmpty(orderKey) ? null : (Guid?)Guid.Parse(orderKey),
            TransactionKey = string.IsNullOrEmpty(transactionKey) ? null : (Guid?)Guid.Parse(transactionKey)
        };

        await connection.ExecuteAsync(sql, parameters);
    }
}
