using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NUSHPOS.Models;

[Table("OrderHeaders")]
public class OrderHeader
{
    [Key]
    public int AutoID { get; set; }
    public int OrderID { get; set; }
    public string? ReceiptNo { get; set; }
    public string? OrderKey { get; set; }
    public string? EditKey { get; set; }
    public string? SyncKey { get; set; }
    public string? MainOrderKey { get; set; }
    public int? MainOrderID { get; set; }
    public int? OrderTypeSourceID { get; set; }
    public string? OrderTypeSourceExternalNo { get; set; }
    public DateTime? OrderDateTime { get; set; }
    public int? EmployeeID { get; set; }
    public int? StationID { get; set; }
    public int? OrderType { get; set; }
    public int? DineInTableID { get; set; }
    public int? CustomerID { get; set; }
    public int? RevenueCenterTypeID { get; set; }
    public decimal? DeliveryCharge { get; set; }
    public decimal? DeliveryComp { get; set; }
    public int? DeliveryZoneID { get; set; }
    public int? DriverEmployeeID { get; set; }
    public int? DiscountID { get; set; }
    public decimal? DiscountLineAmount { get; set; }
    public decimal? DiscountOrderAmount { get; set; }
    public decimal? DiscountCashAmount { get; set; }
    public decimal? DiscountTotalAmount { get; set; }
    public decimal? DiscountAmountValue { get; set; }
    public int? DiscountBasisValue { get; set; }
    public int? OrderStatus { get; set; }
    public decimal? BonusAmountUsed { get; set; }
    public decimal? BonusAmountEarned { get; set; }
    public int? BonusID { get; set; }
    public int? BonusCustomerID { get; set; }
    public decimal? AmountDue { get; set; }
    public bool? GuestCheckPrinted { get; set; }
    public int? GuestCheckPrintCount { get; set; }
    public decimal? SubTotal { get; set; }
    public decimal? OrderCost { get; set; }
    public decimal? CashGratuity { get; set; }
    public decimal? GratuityPercent { get; set; }
    public decimal? SalesTaxAmount { get; set; }
    public decimal? SalesTaxRate { get; set; }
    public decimal? RoundAmount { get; set; }
    public int? GuestNumber { get; set; }
    public string? SpecificCustomerName { get; set; }
    public string? OrderPhone { get; set; }
    public string? BarTabName { get; set; }
    public int? TableReady { get; set; }
    public bool? InvoicePrinted { get; set; }
    public bool? AdditionPrinted { get; set; }
    public bool? FiscalPrinted { get; set; }
    public string? OrderNotes { get; set; }
    public int? LineDeleted { get; set; }
    public string? DeleteReason { get; set; }
    public int? BranchID { get; set; }
    public string? DineInTableName { get; set; }
    public string? CustomerName { get; set; }
    public string? EmployeeName { get; set; }
    public string? EmployeeKey { get; set; }
    public string? CustomerKey { get; set; }
    public string? DiscountKey { get; set; }
    public string? UsedDiscountName { get; set; }
    public DateTime? DayEnd { get; set; }
    public int? AddUserID { get; set; }
    public DateTime? AddDateTime { get; set; }
    public int? EditUserID { get; set; }
    public DateTime? EditDateTime { get; set; }

    [NotMapped]
    public List<OrderTransaction> Transactions { get; set; } = new();

    [NotMapped]
    public List<OrderPayment> Payments { get; set; } = new();
}
