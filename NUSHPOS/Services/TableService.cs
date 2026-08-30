using Dapper;
using NUSHPOS.Models;

namespace NUSHPOS.Services;

public class TableService
{
    private readonly DatabaseService _db;

    public TableService(DatabaseService db)
    {
        _db = db;
    }

    public async Task<IEnumerable<DineInTableGroup>> GetTableGroupsAsync()
    {
        using var connection = _db.CreateConnection();
        const string sql = "SELECT AutoID, TableGroupID, TableGroupText, CAST(TableGroupKey AS NVARCHAR(100)) AS TableGroupKey, TableRowCount, TableColumnCount, BranchID FROM DineInTableGroups ORDER BY TableGroupID";
        return await connection.QueryAsync<DineInTableGroup>(sql);
    }

    public async Task<IEnumerable<dynamic>> GetTablesByGroupWithActiveOrdersAsync(int tableGroupId)
    {
        using var connection = _db.CreateConnection();
        const string sql = @"
            SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
            SELECT DineInTables.AutoID, DineInTables.DineInTableID, DineInTables.DineInTableKey, DineInTables.TableGroupKey, DineInTables.DineInTableText, DineInTables.SectionNumber, DineInTables.TableGroupID,  
            DineInTables.DisplayIndex, isnull(DineInTables.DineInTableActive,1) as DineInTableActive, DineInTables.MaxGuests, DineInTables.Smoking, DineInTables.Window, DineInTables.Booth,  
            DineInTables.Privacy, DineInTables.PictureName, DineInTables.AvarageSeatTime, DineInTables.RevenueCenterTypeID, isnull(DineInTables.SecurityLevel,0) as SecurityLevel ,  
            DineInTables.DeleteReason, DineInTables.CustomField1, DineInTables.CustomField2, DineInTables.CustomField3, DineInTables.CustomField4,  
            DineInTables.CustomField5, CAST(DineInTables.EditKey AS NVARCHAR(100)) as EditKey, CAST(DineInTables.SyncKey AS NVARCHAR(100)) as SyncKey, DineInTables.BranchID, DineInTables.AddUserID, DineInTables.AddDateTime,  
            DineInTables.EditUserID, DineInTables.EditDateTime, CAST(OrderHeaders.OrderKey AS NVARCHAR(100)) AS ActiveOrderKey, isnull(OrderHeaders.AutoID, 0) AS ActiveOrderID, (isnull(OrderHeaders.AmountDue,0.0)+isnull(OrderHeaders.CashGratuity,0.0)) as ActiveOrderAmountDue, OrderHeaders.OrderDateTime AS ActiveOrderDateTime, OrderHeaders.EditDateTime AS ActiveOrderEditTime, 
            isnull(EmployeeFiles.FirstName,'') AS ActiveOrderEmyloyeeName,isnull(OrderHeaders.GuestCheckPrinted,0) AS GuestCheckPrinted,isnull(OrderHeaders.TableReady,0) AS TableReady, 
            isnull(OrderHeaders.AdditionPrintedLineCount,0) AS AdditionPrintedLineCount, CAST(ISNULL(EmployeeFiles.EmployeeKey,'00000000-0000-0000-0000-000000000000') AS NVARCHAR(100)) AS ActiveOrderEmyloyeeKey 
            FROM DineInTables with (nolock)  
            LEFT OUTER JOIN OrderHeaders ON OrderHeaders.DineInTableID = DineInTables.DineInTableID  AND isnull(OrderHeaders.OrderStatus,1)=1 AND OrderHeaders.OrderType=1 
            LEFT OUTER JOIN EmployeeFiles ON EmployeeFiles.EmployeeID = OrderHeaders.EmployeeID  
            Where DineInTables.DineInTableActive=1 and DineInTables.TableGroupID=@TableGroupID";
            
        return await connection.QueryAsync(sql, new { TableGroupID = tableGroupId });
    }

    public async Task<IEnumerable<dynamic>> GetTableLastActivityDatesAsync()
    {
        using var connection = _db.CreateConnection();
        const string sql = @"
            SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; 
            SELECT r.DineInTableID,MAX(r.lastDate) AS lastDate FROM ( 
            SELECT h.AutoID,h.OrderKey,h.DineInTableID,(ISNULL(ISNULL(d.EditDateTime,d.AddDateTime),h.AddDateTime)) AS lastDate   
            FROM OrderHeaders AS h WITH(NOLOCK) 
            INNER JOIN OrderTransactions AS d ON d.OrderKey = h.OrderKey  
            WHERE h.OrderDateTime>DATEADD(hour,-48,GETDATE()) AND isnull(h.LineDeleted,0)=0 AND h.OrderStatus<2 AND h.DineInTableID>0 
            ) AS r GROUP BY r.AutoID, r.OrderKey, r.DineInTableID";
            
        return await connection.QueryAsync(sql);
    }

    public async Task<dynamic?> GetTableWithActiveOrderAsync(int autoId)
    {
        using var connection = _db.CreateConnection();
        const string sql = @"
            SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
            SELECT DineInTables.AutoID, DineInTables.DineInTableID, DineInTables.DineInTableKey, DineInTables.TableGroupKey, DineInTables.DineInTableText, DineInTables.SectionNumber, DineInTables.TableGroupID,  
            DineInTables.DisplayIndex, isnull(DineInTables.DineInTableActive,1) as DineInTableActive, DineInTables.MaxGuests, DineInTables.Smoking, DineInTables.Window, DineInTables.Booth,  
            DineInTables.Privacy, DineInTables.PictureName, DineInTables.AvarageSeatTime, DineInTables.RevenueCenterTypeID, isnull(DineInTables.SecurityLevel,0) as SecurityLevel ,  
            DineInTables.DeleteReason, DineInTables.CustomField1, DineInTables.CustomField2, DineInTables.CustomField3, DineInTables.CustomField4,  
            DineInTables.CustomField5, CAST(DineInTables.EditKey AS NVARCHAR(100)) as EditKey, CAST(DineInTables.SyncKey AS NVARCHAR(100)) as SyncKey, DineInTables.BranchID, DineInTables.AddUserID, DineInTables.AddDateTime,  
            DineInTables.EditUserID, DineInTables.EditDateTime, CAST(OrderHeaders.OrderKey AS NVARCHAR(100)) AS ActiveOrderKey, isnull(OrderHeaders.AutoID, 0) AS ActiveOrderID, (isnull(OrderHeaders.AmountDue,0.0)+isnull(OrderHeaders.CashGratuity,0.0)) as ActiveOrderAmountDue, OrderHeaders.OrderDateTime AS ActiveOrderDateTime, OrderHeaders.EditDateTime AS ActiveOrderEditTime, 
            isnull(EmployeeFiles.FirstName,'') AS ActiveOrderEmyloyeeName,isnull(OrderHeaders.GuestCheckPrinted,0) AS GuestCheckPrinted,isnull(OrderHeaders.TableReady,0) AS TableReady, 
            isnull(OrderHeaders.AdditionPrintedLineCount,0) AS AdditionPrintedLineCount, CAST(ISNULL(EmployeeFiles.EmployeeKey,'00000000-0000-0000-0000-000000000000') AS NVARCHAR(100)) AS ActiveOrderEmyloyeeKey 
            FROM DineInTables with (nolock)  
            LEFT OUTER JOIN OrderHeaders ON OrderHeaders.DineInTableID = DineInTables.DineInTableID  AND isnull(OrderHeaders.OrderStatus,1)=1 AND OrderHeaders.OrderType=1 
            LEFT OUTER JOIN EmployeeFiles ON EmployeeFiles.EmployeeID = OrderHeaders.EmployeeID  
            Where DineInTables.AutoID=@AutoID";
            
        return await connection.QueryFirstOrDefaultAsync(sql, new { AutoID = autoId });
    }
}
