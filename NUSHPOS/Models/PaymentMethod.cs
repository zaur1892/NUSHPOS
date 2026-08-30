using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NUSHPOS.Models;

[Table("PaymentMethods")]
public class PaymentMethod
{
    [Key]
    public int AutoID { get; set; }
    public int PaymentMethodID { get; set; }
    public string? PaymentName { get; set; }
    public int? IsDefault { get; set; }
    public decimal? ExchangeRate { get; set; }
    public int? SecurityLevel { get; set; }
    public int? PaymentMethodActive { get; set; }
    public int? EffectRegister { get; set; }
    public int? IsAccountPayment { get; set; }
    public int? IsAccountSale { get; set; }
    public int? HideInRecievePayement { get; set; }
    public string? PictureName { get; set; }
    public int? DisplayIndex { get; set; }
    public string? ButtonColor { get; set; }
    public Guid? PaymentMethodKey { get; set; }
    public int? BranchID { get; set; }
    public int? PaymentTypeID { get; set; }
}
