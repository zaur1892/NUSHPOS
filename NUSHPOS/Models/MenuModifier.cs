using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NUSHPOS.Models;

[Table("MenuModifiers")]
public class MenuModifier
{
    [Key]
    public int AutoID { get; set; }
    public int MenuModifierID { get; set; }
    public string? MenuModifierText { get; set; }
    public decimal? AdditionalCost { get; set; }
    public bool? MenuModifierActive { get; set; }
    public string? SecLangModifierText { get; set; }
    public string? PictureName { get; set; }
    public string? MenuModifierKey { get; set; }
    public int? BranchID { get; set; }
}
