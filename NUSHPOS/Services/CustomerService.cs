using Dapper;
using NUSHPOS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
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

    public async Task<List<Customer>> SearchCustomersByPhoneAsync(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone)) return new List<Customer>();

        string cleanPhone = phone.Trim().Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");
        string phoneWithoutLeadingZero = cleanPhone.TrimStart('0');

        using var connection = _db.CreateConnection();
        const string sql = @"
            SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
            SELECT DISTINCT 
                c.AutoID, c.CustomerKey, c.CustomerID, c.BranchID, c.CardNumber, 
                c.CustomerName, c.CustomerFullName, c.CityName, c.District, 
                c.Neighborhood, c.Street, c.Buildings, c.Block, c.Apartment, 
                c.ApartmentNo, c.FlatNo, c.CustomerNotes, 
                c.AddressNotes, c.AllowHouseAccount, c.CreditLimit, 
                c.DiscountPercent, c.SpecialBonusPercent, c.TotalDebt, 
                c.TotalPayment, c.TotalRemainig, c.BonusStartupValue, 
                c.TotalBonusUsed, c.TotalBonusEarned, c.TotalBonusRemaing, 
                c.EmailAddress, c.IsEmployee, c.CustomerIsActive,
                p.PhoneNumber AS DisplayPhoneNumber
            FROM CustomerFiles c
            INNER JOIN CustomerPhones p ON c.CustomerKey = p.CustomerKey
            WHERE (ISNULL(c.CustomerIsActive, 1) = 1)
              AND (p.PhoneNumber LIKE @P1 OR p.PhoneNumber LIKE @P2 OR p.PhoneNumber LIKE @P3)
            ORDER BY c.AutoID DESC";

        var list = (await connection.QueryAsync<Customer>(sql, new
        {
            P1 = $"%{cleanPhone}%",
            P2 = $"%{phoneWithoutLeadingZero}%",
            P3 = $"%0{phoneWithoutLeadingZero}%"
        })).ToList();

        // Also enrich with stats
        foreach (var cust in list)
        {
            await EnrichCustomerStatsAsync(cust, connection);
        }

        return list;
    }

    public async Task EnrichCustomerStatsAsync(Customer cust, System.Data.IDbConnection? conn = null)
    {
        bool dispose = false;
        if (conn == null)
        {
            conn = _db.CreateConnection();
            dispose = true;
        }

        try
        {
            const string statSql = @"
                SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
                SELECT 
                    COUNT(AutoID) AS TotalOrders,
                    ISNULL(SUM(AmountDue), 0) AS TotalSpent
                FROM OrderHeaders
                WHERE (CustomerKey = @CustomerKey OR (CustomerID = @CustomerID AND @CustomerID > 0))
                  AND ISNULL(LineDeleted, 0) = 0";

            var stat = await conn.QueryFirstOrDefaultAsync<dynamic>(statSql, new
            {
                CustomerKey = cust.CustomerKey ?? "",
                CustomerID = cust.CustomerID
            });

            if (stat != null)
            {
                cust.TotalOrders = Convert.ToInt32(stat.TotalOrders ?? 0);
                cust.TotalSpent = Convert.ToDecimal(stat.TotalSpent ?? 0);
            }
        }
        finally
        {
            if (dispose) conn.Dispose();
        }
    }

    public async Task<Customer?> GetCustomerByIdAsync(int customerId)
    {
        using var connection = _db.CreateConnection();
        const string sql = "SELECT * FROM CustomerFiles WHERE CustomerID = @CustomerId";
        var cust = await connection.QueryFirstOrDefaultAsync<Customer>(sql, new { CustomerId = customerId });
        if (cust != null)
        {
            await EnrichCustomerStatsAsync(cust, connection);
        }
        return cust;
    }

    public async Task<Customer?> GetCustomerByKeyAsync(string customerKey)
    {
        using var connection = _db.CreateConnection();
        const string sql = "SELECT * FROM CustomerFiles WHERE CustomerKey = @CustomerKey";
        var cust = await connection.QueryFirstOrDefaultAsync<Customer>(sql, new { CustomerKey = customerKey });
        if (cust != null)
        {
            await EnrichCustomerStatsAsync(cust, connection);
        }
        return cust;
    }

    public async Task<Customer> SaveOrUpdateCustomerAsync(Customer customer, string phoneNumber)
    {
        using var connection = _db.CreateConnection();

        if (string.IsNullOrWhiteSpace(customer.CustomerKey) || customer.CustomerID <= 0)
        {
            // Create new customer
            customer.CustomerKey = Guid.NewGuid().ToString().ToUpper();
            
            int nextId = 1;
            try
            {
                nextId = await connection.ExecuteScalarAsync<int>("SELECT ISNULL(MAX(CustomerID), 0) + 1 FROM CustomerFiles");
            }
            catch { }
            customer.CustomerID = nextId;

            const string insertCustSql = @"
                INSERT INTO CustomerFiles (
                    CustomerKey, CustomerID, CustomerName, CustomerFullName, CardNumber,
                    CityName, District, Neighborhood, Street, Buildings, Block,
                    Apartment, ApartmentNo, FlatNo, CustomerNotes, AddressNotes,
                    AllowHouseAccount, CreditLimit, DiscountPercent, SpecialBonusPercent,
                    TotalDebt, TotalPayment, TotalRemainig, BonusStartupValue,
                    TotalBonusUsed, TotalBonusEarned, TotalBonusRemaing,
                    EmailAddress, IsEmployee, CustomerIsActive, BranchID
                ) VALUES (
                    @CustomerKey, @CustomerID, @CustomerName, @CustomerFullName, @CardNumber,
                    @CityName, @District, @Neighborhood, @Street, @Buildings, @Block,
                    @Apartment, @ApartmentNo, @FlatNo, @CustomerNotes, @AddressNotes,
                    @AllowHouseAccount, @CreditLimit, @DiscountPercent, @SpecialBonusPercent,
                    @TotalDebt, @TotalPayment, @TotalRemainig, @BonusStartupValue,
                    @TotalBonusUsed, @TotalBonusEarned, @TotalBonusRemaing,
                    @EmailAddress, @IsEmployee, @CustomerIsActive, @BranchID
                );
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            customer.AutoID = await connection.ExecuteScalarAsync<int>(insertCustSql, customer);

            // Insert phone
            if (!string.IsNullOrWhiteSpace(phoneNumber))
            {
                const string insertPhoneSql = @"
                    INSERT INTO CustomerPhones (
                        CustomerKey, PhoneKey, PhoneNumber, PhoneType, BranchID, DateCreated, CustomerIsActive
                    ) VALUES (
                        @CustomerKey, @PhoneKey, @PhoneNumber, 1, @BranchID, GETDATE(), 1
                    )";
                await connection.ExecuteAsync(insertPhoneSql, new
                {
                    CustomerKey = customer.CustomerKey,
                    PhoneKey = Guid.NewGuid().ToString().ToUpper(),
                    PhoneNumber = phoneNumber.Trim(),
                    BranchID = customer.BranchID ?? 1
                });
            }
        }
        else
        {
            // Update existing customer
            const string updateCustSql = @"
                UPDATE CustomerFiles SET
                    CustomerName = @CustomerName,
                    CustomerFullName = @CustomerFullName,
                    CardNumber = @CardNumber,
                    CityName = @CityName,
                    District = @District,
                    Neighborhood = @Neighborhood,
                    Street = @Street,
                    Buildings = @Buildings,
                    Block = @Block,
                    Apartment = @Apartment,
                    ApartmentNo = @ApartmentNo,
                    FlatNo = @FlatNo,
                    CustomerNotes = @CustomerNotes,
                    AddressNotes = @AddressNotes,
                    AllowHouseAccount = @AllowHouseAccount,
                    CreditLimit = @CreditLimit,
                    DiscountPercent = @DiscountPercent,
                    SpecialBonusPercent = @SpecialBonusPercent,
                    EmailAddress = @EmailAddress,
                    CustomerIsActive = @CustomerIsActive
                WHERE CustomerKey = @CustomerKey OR CustomerID = @CustomerID";

            await connection.ExecuteAsync(updateCustSql, customer);

            // Ensure phone is recorded
            if (!string.IsNullOrWhiteSpace(phoneNumber))
            {
                const string checkPhoneSql = "SELECT COUNT(1) FROM CustomerPhones WHERE CustomerKey = @CustomerKey AND PhoneNumber = @PhoneNumber";
                int count = await connection.ExecuteScalarAsync<int>(checkPhoneSql, new { CustomerKey = customer.CustomerKey, PhoneNumber = phoneNumber.Trim() });
                if (count == 0)
                {
                    const string insertPhoneSql = @"
                        INSERT INTO CustomerPhones (
                            CustomerKey, PhoneKey, PhoneNumber, PhoneType, BranchID, DateCreated, CustomerIsActive
                        ) VALUES (
                            @CustomerKey, @PhoneKey, @PhoneNumber, 1, @BranchID, GETDATE(), 1
                        )";
                    await connection.ExecuteAsync(insertPhoneSql, new
                    {
                        CustomerKey = customer.CustomerKey,
                        PhoneKey = Guid.NewGuid().ToString().ToUpper(),
                        PhoneNumber = phoneNumber.Trim(),
                        BranchID = customer.BranchID ?? 1
                    });
                }
            }
        }

        customer.DisplayPhoneNumber = phoneNumber;
        await EnrichCustomerStatsAsync(customer, connection);
        return customer;
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
