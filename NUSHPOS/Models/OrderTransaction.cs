using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NUSHPOS.Models;

[Table("OrderTransactions")]
public class OrderTransaction
{
    [Key]
    public int AutoID { get; set; }
    public int? TransactionID { get; set; }
    public string? TransactionKey { get; set; }
    public string? EditKey { get; set; }
    public string? SyncKey { get; set; }
    public string? OrderKey { get; set; }
    public int? OrderID { get; set; }
    public DateTime? OrderDateTime { get; set; }
    public DateTime? TransactionDateTime { get; set; }
    public int? StationID { get; set; }
    public int? EmployeeID { get; set; }
    public int? RevenueCenterTypeID { get; set; }
    public int? MenuItemID { get; set; }
    public string? MenuItemKey { get; set; }
    public string? MenuItemText { get; set; }
    public string? MenuItemGroupText { get; set; }
    public string? MenumItemCategoryText { get; set; }
    public decimal? MenuItemUnitPrice { get; set; }
    public decimal? MenuItemCost { get; set; }
    public decimal? Quantity { get; set; }
    public decimal? ExtendedPrice { get; set; }
    public int? DiscountID { get; set; }
    public string? DiscountKey { get; set; }
    public decimal? DiscountLineAmount { get; set; }
    public decimal? DiscountCashAmount { get; set; }
    public decimal? DiscountTotalAmount { get; set; }
    public decimal? DiscountAmountValue { get; set; }
    public int? DiscountBasisValue { get; set; }
    public string? UsedDiscountName { get; set; }
    public string? DiscountUserName { get; set; }
    public int? TransactionStatus { get; set; }
    public int? NotificationStatus { get; set; }
    public bool? AdditionLinePrinted { get; set; }
    public decimal? TaxPercent { get; set; }
    public int? RoundID { get; set; }
    public int? Mod1ID { get; set; }
    public decimal? Mod1Cost { get; set; }
    public int? Mod2ID { get; set; }
    public decimal? Mod2Cost { get; set; }
    public int? Mod3ID { get; set; }
    public decimal? Mod3Cost { get; set; }
    public int? Mod4ID { get; set; }
    public decimal? Mod4Cost { get; set; }
    public int? Mod5ID { get; set; }
    public decimal? Mod5Cost { get; set; }
    public int? SeatNumber { get; set; }
    public string? Notes { get; set; }
    public decimal? SaleTaxAmount { get; set; }
    public int? LineDeleted { get; set; }
    public string? DeleteReason { get; set; }
    public int? BranchID { get; set; }
    public int? AddUserID { get; set; }
    public DateTime? AddDateTime { get; set; }
    public string? EmployeeName { get; set; }
    public decimal? RoundAmount { get; set; }
    public int? EditUserID { get; set; }
    public DateTime? EditDateTime { get; set; }
    public int? UsedPrinterID1 { get; set; }
    public int? UsedPrinterID2 { get; set; }
    public int? UsedPrinterID3 { get; set; }
    public int? UsedPrinterID4 { get; set; }
    public int? UsedPrinterID5 { get; set; }
}
