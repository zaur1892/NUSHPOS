using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NUSHPOS.Models;

[Table("Discounts")]
public class Discount
{
    [Key]
    public int AutoID { get; set; }
    public int DiscountID { get; set; }
    public string? DiscountKey { get; set; }
    public string? DiscountText { get; set; }
    public string? DiscountDescription { get; set; }
    public bool? DiscountActive { get; set; }
    public decimal? DiscountAmount { get; set; }
    public int? DiscountBasis { get; set; }
    public DateTime? DiscountExpireDate { get; set; }
    public string? Barcode { get; set; }
    public string? ButtonColor { get; set; }
    public string? PictureName { get; set; }
    public int? SecurityLevel { get; set; }
    public int? BranchID { get; set; }
    public bool? UseGroupFilter { get; set; }
    public bool? UseMenuItemFilter { get; set; }
}
