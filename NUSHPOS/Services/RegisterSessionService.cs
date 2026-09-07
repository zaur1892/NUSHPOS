using Dapper;
using NUSHPOS.Models;
using System;
using System.Threading.Tasks;

namespace NUSHPOS.Services;

public class RegisterSessionService
{
    private readonly DatabaseService _db;
    private readonly AccessLogService _accessLogService;

    public RegisterSessionService(DatabaseService db, AccessLogService accessLogService)
    {
        _db = db;
        _accessLogService = accessLogService;
    }

    public async Task<int> CheckStationOpenSessionAsync(int stationId)
    {
        using var connection = _db.CreateConnection();
        const string sql = @"
            SELECT COUNT(RegisterSessionID) AS countValue 
            FROM RegisterSessions 
            WHERE StationID = @StationID AND SignOutDateTime IS NULL";
        return await connection.ExecuteScalarAsync<int>(sql, new { StationID = stationId });
    }

    public async Task<RegisterSession?> GetActiveSessionAsync(int employeeId)
    {
        using var connection = _db.CreateConnection();
        const string sql = @"
            SELECT * FROM RegisterSessions 
            WHERE EmployeeID = @EmployeeId AND SignOutDateTime IS NULL 
            ORDER BY SignInDateTime DESC";
        return await connection.QueryFirstOrDefaultAsync<RegisterSession>(sql, new { EmployeeId = employeeId });
    }

    public async Task<RegisterSession?> GetActiveStationSessionAsync(int stationId)
    {
        using var connection = _db.CreateConnection();
        const string sql = @"
            SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
            SELECT TOP 1 * 
            FROM RegisterSessions 
            WHERE StationID = @StationID AND SignOutDateTime IS NULL 
            ORDER BY SignInDateTime DESC";
        return await connection.QueryFirstOrDefaultAsync<RegisterSession>(sql, new { StationID = stationId });
    }

    public async Task<int> OpenCashierSessionAsync(
        int branchId, 
        int stationId, 
        int employeeId, 
        string employeeKey, 
        decimal startAmount, 
        string accessCode = "")
    {
        using var connection = _db.CreateConnection();

        // 1. Access Log qeydinin yazılması (Kassir sessiyasının açılması)
        try
        {
            await _accessLogService.InsertAccessLogAsync(
                branchId: branchId,
                stationId: stationId,
                employeeId: employeeId,
                actionName: "KASSİR SESSİYASINI AÇMAQ",
                wrongPassword: accessCode,
                additionalInfo: $"Açılış məbləği: {startAmount:N2} AZN",
                isSuccess: true,
                orderKey: Guid.Empty.ToString(),
                transactionKey: Guid.Empty.ToString()
            );
        }
        catch { }

        // 2. RegisterSessions cədvəlinə əlavə edilməsi (SQL strukturuna tam uyğun)
        var registerSessionKey = Guid.NewGuid();
        var editKey = Guid.NewGuid();
        var syncKey = Guid.Empty;
        var empGuid = Guid.TryParse(employeeKey, out var g) ? g : Guid.Empty;

        const string sql = @"
            INSERT INTO [RegisterSessions] (
                [BranchID], [EmployeeID], [EmployeeKey], [RegisterSessionKey], [StationID], 
                [AccountingDateTime], [SignInDateTime], [RegisterStartAmount], [SignOutDateTime], 
                [RegisterEndAmount], [DiscrepancyAmount], [DiscrepancyNotes], [DiscrepancyNotes2], 
                [ManagerEmployeeID], 
                [TotalPaymentMethod1], [TotalPaymentMethod2], [TotalPaymentMethod3], [TotalPaymentMethod4], [TotalPaymentMethod5], 
                [TotalPaymentMethod6], [TotalPaymentMethod7], [TotalPaymentMethod8], [TotalPaymentMethod9], [TotalPaymentMethod10], 
                [TotalPaymentMethod11], [TotalPaymentMethod12], [TotalPaymentMethod13], [TotalPaymentMethod14], [TotalPaymentMethod15], 
                [TotalPaymentMethod16], [TotalPaymentMethod17], [TotalPaymentMethod18], [TotalPaymentMethod19], [TotalPaymentMethod20], 
                [ClosePaymentMethod1], [ClosePaymentMethod2], [ClosePaymentMethod3], [ClosePaymentMethod4], [ClosePaymentMethod5], 
                [ClosePaymentMethod6], [ClosePaymentMethod7], [ClosePaymentMethod8], [ClosePaymentMethod9], [ClosePaymentMethod10], 
                [ClosePaymentMethod11], [ClosePaymentMethod12], [ClosePaymentMethod13], [ClosePaymentMethod14], [ClosePaymentMethod15], 
                [ClosePaymentMethod16], [ClosePaymentMethod17], [ClosePaymentMethod18], [ClosePaymentMethod19], [ClosePaymentMethod20], 
                [ZReportID], [EditKey], [SyncKey]
            ) VALUES (
                @BranchID, @EmployeeID, @EmployeeKey, @RegisterSessionKey, @StationID, 
                CAST(GETDATE() AS DATE), GETDATE(), @RegisterStartAmount, NULL, 
                0, 0, NULL, NULL, 
                0, 
                0, 0, 0, 0, 0, 
                0, 0, 0, 0, 0, 
                0, 0, 0, 0, 0, 
                0, 0, 0, 0, 0, 
                0, 0, 0, 0, 0, 
                0, 0, 0, 0, 0, 
                0, 0, 0, 0, 0, 
                0, 0, 0, 0, 0, 
                0, @EditKey, @SyncKey
            );
            SELECT CAST(SCOPE_IDENTITY() AS INT);";

        var parameters = new
        {
            BranchID = branchId,
            EmployeeID = employeeId,
            EmployeeKey = empGuid,
            RegisterSessionKey = registerSessionKey,
            StationID = stationId,
            RegisterStartAmount = (double)startAmount,
            EditKey = editKey,
            SyncKey = syncKey
        };

        var sessionId = await connection.ExecuteScalarAsync<int>(sql, parameters);
        return sessionId;
    }

    public async Task<int> GetOpenOrdersCountAsync()
    {
        using var connection = _db.CreateConnection();
        const string sql = @"
            SELECT COUNT(AutoID) AS countValue 
            FROM OrderHeaders 
            WHERE OrderStatus = 1 AND ISNULL(LineDeleted, 0) = 0";
        return await connection.ExecuteScalarAsync<int>(sql);
    }

    public async Task<List<dynamic>> GetActiveSessionsDetailedListAsync(int stationId = 0)
    {
        using var connection = _db.CreateConnection();
        string sql = @"
            SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
            SELECT 
                rs.RegisterSessionID, rs.RegisterSessionKey, rs.BranchID, rs.EmployeeID, rs.StationID,  
                rs.AccountingDateTime, rs.SignInDateTime, rs.RegisterStartAmount, rs.SignOutDateTime,  
                rs.EmployeeKey, rs.SyncKey, 
                RTRIM(LTRIM(emp.FirstName)) + ' ' + RTRIM(LTRIM(ISNULL(emp.LastName, ''))) AS EmployeeFullName, 
                st.StationName 
            FROM RegisterSessions rs
            INNER JOIN EmployeeFiles emp ON rs.EmployeeKey = emp.EmployeeKey  
            INNER JOIN StationSettings st ON rs.StationID = st.StationID  
            WHERE rs.SignOutDateTime IS NULL " + 
            (stationId > 0 ? "AND rs.StationID = @StationID " : "") + 
            "ORDER BY rs.SignInDateTime DESC";
        var result = await connection.QueryAsync<dynamic>(sql, new { StationID = stationId });
        return result.AsList();
    }

    public async Task<List<SessionPaymentSummary>> GetSessionPaymentTotalsAsync(int registerSessionId)
    {
        using var connection = _db.CreateConnection();
        const string sql = @"
            SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
            SELECT 
                r2.AutoID, 
                r2.PaymentMethodID, 
                r2.PaymentName AS PaymentMethodName, 
                r2.AmountPaid, 
                r2.PaymentTypeID,
                CASE r2.PaymentTypeID 
                    WHEN 0 THEN 'Digər'  
                    WHEN 1 THEN 'Nağd' 
                    WHEN 2 THEN 'Bank Kartı' 
                    WHEN 3 THEN 'Yemək Çeki'  
                    WHEN 4 THEN 'Cari Hesab' 
                    WHEN 5 THEN 'Bonus / Bal' 
                    ELSE 'Digər' 
                END AS PaymentTypeName
            FROM ( 
                SELECT 
                    r.AutoID, 
                    r.PaymentMethodID, 
                    r.PaymentName, 
                    r.SumAmountPaid - ISNULL(SUM(e.ExpenseAmount), 0) AS AmountPaid, 
                    r.PaymentTypeID  
                FROM ( 
                    SELECT 
                        pm.AutoID, 
                        pm.PaymentMethodID, 
                        pm.PaymentName, 
                        ISNULL(SUM(op.AmountPaid), 0) AS SumAmountPaid, 
                        ISNULL(pm.PaymentTypeID, 2) AS PaymentTypeID 
                    FROM PaymentMethods AS pm
                    LEFT OUTER JOIN OrderPayments AS op ON op.PaymentMethodID = pm.PaymentMethodID 
                        AND op.RegisterSessionID = @RegisterSessionID 
                        AND ISNULL(op.LineDeleted, 0) = 0 
                    WHERE ISNULL(pm.PaymentMethodActive, 0) = 1 AND pm.PaymentMethodID > 0 
                    GROUP BY pm.AutoID, pm.PaymentMethodID, pm.PaymentName, pm.PaymentTypeID  
                ) AS r 
                LEFT OUTER JOIN Expenses AS e ON r.PaymentMethodID = e.PaymentMethodID 
                    AND e.RegisterSessionID = @RegisterSessionID 
                    AND ISNULL(e.LineDeleted, 0) = 0  
                GROUP BY r.AutoID, r.PaymentMethodID, r.PaymentName, r.SumAmountPaid, r.PaymentTypeID 
            ) AS r2 
            ORDER BY r2.PaymentMethodID";

        var list = await connection.QueryAsync<SessionPaymentSummary>(sql, new { RegisterSessionID = registerSessionId });
        return list.AsList();
    }

    public async Task<List<Expense>> GetSessionExpensesAsync(int registerSessionId)
    {
        using var connection = _db.CreateConnection();
        const string sql = @"
            SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
            SELECT 
                Expenses.ExpenseID, Expenses.ExpenseKey, Expenses.ExpenseDatetime, Expenses.RegisterSessionID, 
                Expenses.PaymentMethodID, Expenses.ExpenseName, Expenses.ExpenseAmount, 
                Expenses.ExpenseDescription, Expenses.StationID, Expenses.LineDeleted,
                ISNULL(ea.FirstName, '') + ' ' + ISNULL(ea.LastName, '') AS AddEmployeeName, 
                ss.StationName, pm.PaymentName AS PaymentMethodName 
            FROM Expenses 
            LEFT OUTER JOIN EmployeeFiles AS ea ON Expenses.AddUserID = ea.AutoID 
            LEFT OUTER JOIN StationSettings AS ss ON Expenses.StationID = ss.StationID
            LEFT OUTER JOIN PaymentMethods AS pm ON Expenses.PaymentMethodID = pm.PaymentMethodID
            WHERE Expenses.RegisterSessionID = @RegisterSessionID 
              AND ISNULL(Expenses.LineDeleted, 0) = 0 
            ORDER BY Expenses.ExpenseDatetime DESC";

        var list = await connection.QueryAsync<Expense>(sql, new { RegisterSessionID = registerSessionId });
        return list.AsList();
    }

    public async Task CloseCashierSessionComprehensiveAsync(
        int registerSessionId,
        int branchId,
        int stationId,
        int employeeId,
        decimal endAmount,
        decimal discrepancyAmount,
        string discrepancyNotes,
        string discrepancyNotes2,
        List<SessionPaymentSummary> paymentSummaries,
        string accessCode = "")
    {
        using var connection = _db.CreateConnection();

        // 1. Access Log qeydinin yazılması
        try
        {
            await _accessLogService.InsertAccessLogAsync(
                branchId: branchId,
                stationId: stationId,
                employeeId: employeeId,
                actionName: "KASSİR SESSİYASINI BAĞLAMAQ",
                wrongPassword: accessCode,
                additionalInfo: $"Bağlanış məbləği: {endAmount:N2} AZN, Fərq: {discrepancyAmount:N2} AZN ({discrepancyNotes})",
                isSuccess: true,
                orderKey: Guid.Empty.ToString(),
                transactionKey: Guid.Empty.ToString()
            );
        }
        catch { }

        // 2. 20 Ödəniş növü üzrə məlumatların hazırlanması
        var totalMethodValues = new double[21];
        var closeMethodValues = new double[21];
        var methodNames = new string?[21];

        for (int i = 1; i <= 20; i++)
        {
            if (i <= paymentSummaries.Count)
            {
                var summary = paymentSummaries[i - 1];
                totalMethodValues[i] = (double)summary.AmountPaid;
                closeMethodValues[i] = (double)summary.CountedAmount;
                methodNames[i] = summary.PaymentMethodName;
            }
            else
            {
                totalMethodValues[i] = 0;
                closeMethodValues[i] = 0;
                methodNames[i] = null;
            }
        }

        // 3. RegisterSessions cədvəlinin yenilənməsi
        const string updateSessionSql = @"
            UPDATE [RegisterSessions] SET 
                [SignOutDateTime] = GETDATE(), 
                [RegisterEndAmount] = @RegisterEndAmount, 
                [DiscrepancyAmount] = @DiscrepancyAmount, 
                [DiscrepancyNotes] = @DiscrepancyNotes, 
                [DiscrepancyNotes2] = @DiscrepancyNotes2, 
                [ManagerEmployeeID] = @ManagerEmployeeID, 
                [TotalPaymentMethod1] = @TotalPaymentMethod1, [TotalPaymentMethod2] = @TotalPaymentMethod2, [TotalPaymentMethod3] = @TotalPaymentMethod3, 
                [TotalPaymentMethod4] = @TotalPaymentMethod4, [TotalPaymentMethod5] = @TotalPaymentMethod5, [TotalPaymentMethod6] = @TotalPaymentMethod6, 
                [TotalPaymentMethod7] = @TotalPaymentMethod7, [TotalPaymentMethod8] = @TotalPaymentMethod8, [TotalPaymentMethod9] = @TotalPaymentMethod9, 
                [TotalPaymentMethod10] = @TotalPaymentMethod10, [TotalPaymentMethod11] = @TotalPaymentMethod11, [TotalPaymentMethod12] = @TotalPaymentMethod12, 
                [TotalPaymentMethod13] = @TotalPaymentMethod13, [TotalPaymentMethod14] = @TotalPaymentMethod14, [TotalPaymentMethod15] = @TotalPaymentMethod15, 
                [TotalPaymentMethod16] = @TotalPaymentMethod16, [TotalPaymentMethod17] = @TotalPaymentMethod17, [TotalPaymentMethod18] = @TotalPaymentMethod18, 
                [TotalPaymentMethod19] = @TotalPaymentMethod19, [TotalPaymentMethod20] = @TotalPaymentMethod20, 
                [ClosePaymentMethod1] = @ClosePaymentMethod1, [ClosePaymentMethod2] = @ClosePaymentMethod2, [ClosePaymentMethod3] = @ClosePaymentMethod3, 
                [ClosePaymentMethod4] = @ClosePaymentMethod4, [ClosePaymentMethod5] = @ClosePaymentMethod5, [ClosePaymentMethod6] = @ClosePaymentMethod6, 
                [ClosePaymentMethod7] = @ClosePaymentMethod7, [ClosePaymentMethod8] = @ClosePaymentMethod8, [ClosePaymentMethod9] = @ClosePaymentMethod9, 
                [ClosePaymentMethod10] = @ClosePaymentMethod10, [ClosePaymentMethod11] = @ClosePaymentMethod11, [ClosePaymentMethod12] = @ClosePaymentMethod12, 
                [ClosePaymentMethod13] = @ClosePaymentMethod13, [ClosePaymentMethod14] = @ClosePaymentMethod14, [ClosePaymentMethod15] = @ClosePaymentMethod15, 
                [ClosePaymentMethod16] = @ClosePaymentMethod16, [ClosePaymentMethod17] = @ClosePaymentMethod17, [ClosePaymentMethod18] = @ClosePaymentMethod18, 
                [ClosePaymentMethod19] = @ClosePaymentMethod19, [ClosePaymentMethod20] = @ClosePaymentMethod20, 
                [PaymentMethodName1] = @PaymentMethodName1, [PaymentMethodName2] = @PaymentMethodName2, [PaymentMethodName3] = @PaymentMethodName3, 
                [PaymentMethodName4] = @PaymentMethodName4, [PaymentMethodName5] = @PaymentMethodName5, [PaymentMethodName6] = @PaymentMethodName6, 
                [PaymentMethodName7] = @PaymentMethodName7, [PaymentMethodName8] = @PaymentMethodName8, [PaymentMethodName9] = @PaymentMethodName9, 
                [PaymentMethodName10] = @PaymentMethodName10, [PaymentMethodName11] = @PaymentMethodName11, [PaymentMethodName12] = @PaymentMethodName12, 
                [PaymentMethodName13] = @PaymentMethodName13, [PaymentMethodName14] = @PaymentMethodName14, [PaymentMethodName15] = @PaymentMethodName15, 
                [PaymentMethodName16] = @PaymentMethodName16, [PaymentMethodName17] = @PaymentMethodName17, [PaymentMethodName18] = @PaymentMethodName18, 
                [PaymentMethodName19] = @PaymentMethodName19, [PaymentMethodName20] = @PaymentMethodName20, 
                [EditKey] = NEWID(), [SyncKey] = NEWID() 
            WHERE [RegisterSessionID] = @RegisterSessionID";

        var sessionParams = new DynamicParameters();
        sessionParams.Add("@RegisterSessionID", registerSessionId);
        sessionParams.Add("@RegisterEndAmount", (double)endAmount);
        sessionParams.Add("@DiscrepancyAmount", (double)discrepancyAmount);
        sessionParams.Add("@DiscrepancyNotes", discrepancyNotes ?? "");
        sessionParams.Add("@DiscrepancyNotes2", discrepancyNotes2 ?? "");
        sessionParams.Add("@ManagerEmployeeID", employeeId);

        for (int i = 1; i <= 20; i++)
        {
            sessionParams.Add($"@TotalPaymentMethod{i}", totalMethodValues[i]);
            sessionParams.Add($"@ClosePaymentMethod{i}", closeMethodValues[i]);
            sessionParams.Add($"@PaymentMethodName{i}", methodNames[i]);
        }

        await connection.ExecuteAsync(updateSessionSql, sessionParams);

        // 4. Sessiya müddətində vurulan sifariş başlıqlarının açarlarının yenilənməsi
        try
        {
            const string updateOrdersSql = @"
                UPDATE OrderHeaders 
                SET EditKey = NEWID(), SyncKey = NEWID() 
                FROM OrderHeaders 
                INNER JOIN RegisterSessions ON OrderHeaders.OrderDateTime > RegisterSessions.SignInDateTime 
                    AND RegisterSessions.RegisterSessionID = @RegisterSessionID 
                WHERE ISNULL(OrderHeaders.LineDeleted, 0) = 0";
            await connection.ExecuteAsync(updateOrdersSql, new { RegisterSessionID = registerSessionId });
        }
        catch { }
    }
}
