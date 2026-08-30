using Dapper;
using NUSHPOS.Models;

namespace NUSHPOS.Services;

public class CustomerService
{
    private readonly DatabaseService _db;

    public CustomerService(DatabaseService db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Customer>> SearchCustomersAsync(string searchTerm)
    {
        using var connection = _db.CreateConnection();
        const string sql = @"
            SELECT DISTINCT c.* 
            FROM CustomerFiles c
            LEFT JOIN CustomerPhones p ON c.CustomerKey = p.CustomerKey
            WHERE ISNULL(c.CustomerIsActive, 1) = 1
              AND (
                  c.CustomerName LIKE @SearchPattern 
                  OR c.CustomerFullName LIKE @SearchPattern 
                  OR c.CardNumber LIKE @SearchPattern 
                  OR p.PhoneNumber LIKE @SearchPattern
                  OR c.CustomerKey LIKE @SearchPattern
              )
            ORDER BY c.CustomerName";
        return await connection.QueryAsync<Customer>(sql, new { SearchPattern = $"%{searchTerm}%" });
    }

    public async Task<Customer?> GetCustomerByIdAsync(int customerId)
    {
        using var connection = _db.CreateConnection();
        const string sql = "SELECT * FROM CustomerFiles WHERE CustomerID = @CustomerId";
        return await connection.QueryFirstOrDefaultAsync<Customer>(sql, new { CustomerId = customerId });
    }

    public async Task<IEnumerable<BonusCustomer>> GetBonusCustomersAsync()
    {
        using var connection = _db.CreateConnection();
        const string sql = "SELECT * FROM BonusCustomerFiles WHERE ISNULL(CustomerIsActive, 1) = 1 ORDER BY CustomerName";
        return await connection.QueryAsync<BonusCustomer>(sql);
    }

    public async Task UpdateCustomerDebtAsync(int customerId, decimal amount)
    {
        using var connection = _db.CreateConnection();
        const string sql = "UPDATE CustomerFiles SET TotalDebt = @Amount WHERE CustomerID = @CustomerId";
        await connection.ExecuteAsync(sql, new { CustomerId = customerId, Amount = amount });
    }
}
