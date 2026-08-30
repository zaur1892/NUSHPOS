using Dapper;
using NUSHPOS.Models;

namespace NUSHPOS.Services;

public class PaymentService
{
    private readonly DatabaseService _db;

    public PaymentService(DatabaseService db)
    {
        _db = db;
    }

    public async Task<IEnumerable<PaymentMethod>> GetPaymentMethodsAsync()
    {
        using var connection = _db.CreateConnection();
        const string sql = "SELECT * FROM PaymentMethods WHERE ISNULL(PaymentMethodActive, 1) = 1 AND ISNULL(DisplayIndex, 0) < 100 ORDER BY DisplayIndex";
        return await connection.QueryAsync<PaymentMethod>(sql);
    }

    public async Task<IEnumerable<OrderPayment>> GetOrderPaymentsAsync(int orderId)
    {
        using var connection = _db.CreateConnection();
        const string sql = "SELECT * FROM OrderPayments WHERE OrderID = @OrderId AND ISNULL(LineDeleted, 0) = 0 ORDER BY AutoID";
        return await connection.QueryAsync<OrderPayment>(sql, new { OrderId = orderId });
    }

    public async Task AddPaymentAsync(OrderPayment payment)
    {
        using var connection = _db.CreateConnection();
        const string sql = @"
            INSERT INTO OrderPayments (
                PaymentKey, OrderID, OrderKey, StationID, CustomerID, CustomerEmployeeID, CouponNumber,
                RegisterSessionID, RevenueCenterTypeID, PaymentDateTime, EmployeeID, RegisterNo,
                PaymentMethodID, AmountTendered, AmountPaid, AmountChange, ExhangeRate, RoundingAmount,
                CurrencyID, IsAccountPayment, IsAccountSale, PaymentMethodCode, PaymentNotes, LineDeleted,
                DeleteReason, BranchID, AddUserID, AddDateTime, CustomerKey, EmployeeKey, PaymentMethodKey,
                EmployeeName, PaymentMethodName
            )
            VALUES (
                @PaymentKey, @OrderID, @OrderKey, @StationID, @CustomerID, @CustomerEmployeeID, @CouponNumber,
                @RegisterSessionID, @RevenueCenterTypeID, @PaymentDateTime, @EmployeeID, @RegisterNo,
                @PaymentMethodID, @AmountTendered, @AmountPaid, @AmountChange, @ExhangeRate, @RoundingAmount,
                @CurrencyID, @IsAccountPayment, @IsAccountSale, @PaymentMethodCode, @PaymentNotes, @LineDeleted,
                @DeleteReason, @BranchID, @AddUserID, @AddDateTime, @CustomerKey, @EmployeeKey, @PaymentMethodKey,
                @EmployeeName, @PaymentMethodName
            )";
        await connection.ExecuteAsync(sql, payment);
    }

    public async Task VoidPaymentAsync(int paymentId)
    {
        using var connection = _db.CreateConnection();
        const string sql = "UPDATE OrderPayments SET LineDeleted = 1 WHERE AutoID = @PaymentId OR OrderPaymentID = @PaymentId";
        await connection.ExecuteAsync(sql, new { PaymentId = paymentId });
    }
}
