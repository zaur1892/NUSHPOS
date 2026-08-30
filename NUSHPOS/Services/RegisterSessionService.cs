using Dapper;
using NUSHPOS.Models;

namespace NUSHPOS.Services;

public class RegisterSessionService
{
    private readonly DatabaseService _db;

    public RegisterSessionService(DatabaseService db)
    {
        _db = db;
    }

    public async Task<RegisterSession?> GetActiveSessionAsync(int employeeId)
    {
        using var connection = _db.CreateConnection();
        const string sql = "SELECT * FROM RegisterSessions WHERE EmployeeID = @EmployeeId AND SignOutDateTime IS NULL ORDER BY SignInDateTime DESC";
        return await connection.QueryFirstOrDefaultAsync<RegisterSession>(sql, new { EmployeeId = employeeId });
    }

    public async Task<int> OpenSessionAsync(RegisterSession session)
    {
        using var connection = _db.CreateConnection();
        const string sql = @"
            INSERT INTO RegisterSessions (
                BranchID, EmployeeID, RegisterSessionKey, StationID, AccountingDateTime,
                SignInDateTime, RegisterStartAmount, EmployeeKey
            )
            VALUES (
                @BranchID, @EmployeeID, @RegisterSessionKey, @StationID, @AccountingDateTime,
                @SignInDateTime, @RegisterStartAmount, @EmployeeKey
            );
            SELECT CAST(SCOPE_IDENTITY() AS INT);";
        return await connection.ExecuteScalarAsync<int>(sql, session);
    }

    public async Task CloseSessionAsync(int sessionId, decimal endAmount)
    {
        using var connection = _db.CreateConnection();
        const string sql = @"
            UPDATE RegisterSessions SET
                SignOutDateTime = @SignOutDateTime,
                RegisterEndAmount = @RegisterEndAmount
            WHERE RegisterSessionID = @SessionId";
        await connection.ExecuteAsync(sql, new { SessionId = sessionId, RegisterEndAmount = endAmount, SignOutDateTime = DateTime.Now });
    }
}
