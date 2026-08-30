using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NUSHPOS.Models;

[Table("Promotions")]
public class Promotion
{
    [Key]
    public int PromotionID { get; set; }
    public string? PromotionKey { get; set; }
    public string? PromotionName { get; set; }
    public string? PromotionNotes { get; set; }
    public bool? IsActive { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int? BranchID { get; set; }
}
