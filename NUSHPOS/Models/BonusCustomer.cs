using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NUSHPOS.Models;

[Table("BonusCustomerFiles")]
public class BonusCustomer
{
    [Key]
    public int AutoID { get; set; }
    public int CustomerID { get; set; }
    public string? CustomerKey { get; set; }
    public bool? CustomerIsActive { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerFullName { get; set; }
    public string? CardNumber { get; set; }
    public string? PhoneNumber { get; set; }
    public decimal? DiscountPercent { get; set; }
    public decimal? SpecialBonusPercent { get; set; }
    public decimal? TotalDebt { get; set; }
    public decimal? TotalPayment { get; set; }
    public decimal? TotalRemainig { get; set; }
    public decimal? BonusStartupValue { get; set; }
    public decimal? TotalBonusUsed { get; set; }
    public decimal? TotalBonusEarned { get; set; }
    public decimal? TotalBonusRemaing { get; set; }
    public int? BranchID { get; set; }
}
