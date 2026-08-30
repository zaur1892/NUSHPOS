using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NUSHPOS.Models;

[Table("OrderPayments")]
public class OrderPayment
{
    [Key]
    public int AutoID { get; set; }
    public int? OrderPaymentID { get; set; }
    public string? PaymentKey { get; set; }
    public int? OrderID { get; set; }
    public string? OrderKey { get; set; }
    public int? StationID { get; set; }
    public int? CustomerID { get; set; }
    public int? RegisterSessionID { get; set; }
    public DateTime? PaymentDateTime { get; set; }
    public int? EmployeeID { get; set; }
    public int? PaymentMethodID { get; set; }
    public decimal? AmountTendered { get; set; }
    public decimal? AmountPaid { get; set; }
    public decimal? AmountChange { get; set; }
    public decimal? ExhangeRate { get; set; }
    public int? IsAccountPayment { get; set; }
    public int? IsAccountSale { get; set; }
    public string? PaymentNotes { get; set; }
    public int? LineDeleted { get; set; }
    public int? BranchID { get; set; }
    public string? CustomerKey { get; set; }
    public string? EmployeeKey { get; set; }
    public string? PaymentMethodKey { get; set; }
    public string? PaymentMethodName { get; set; }
    public string? EmployeeName { get; set; }
    public DateTime? DayEnd { get; set; }

    // Missing properties required by SQL
    public int? CustomerEmployeeID { get; set; }
    public string? CouponNumber { get; set; }
    public int? RevenueCenterTypeID { get; set; }
    public int? RegisterNo { get; set; }
    public decimal? RoundingAmount { get; set; }
    public int? CurrencyID { get; set; }
    public string? PaymentMethodCode { get; set; }
    public string? DeleteReason { get; set; }
    public int? AddUserID { get; set; }
    public DateTime? AddDateTime { get; set; }
}
