using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NUSHPOS.Models;

[Table("MenuItemLayout")]
public class MenuItemLayout
{
    [Key]
    public int AutoID { get; set; }
    public int? MenuGroupID { get; set; }
    public int? MainMenuItemID { get; set; }
    public int? MenuItemID { get; set; }
    public int? DisplayIndex { get; set; }
    public int? BranchID { get; set; }
    public string? LayoutKey { get; set; }
    public string? MenuItemKey { get; set; }
    public string? MenuGroupKey { get; set; }
    public string? MainMenuItemKey { get; set; }
}
