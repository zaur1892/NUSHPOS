using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NUSHPOS.Models;

[Table("MenuCategories")]
public class MenuCategory
{
    [Key]
    public int AutoID { get; set; }
    public int MenuCategoryID { get; set; }
    public string? MenuCategoryKey { get; set; }
    public string? MenuCategoryText { get; set; }
    public bool? MenuCategoryActive { get; set; }
    public decimal? DefaultTaxPercent { get; set; }
    public int? BranchID { get; set; }
}
