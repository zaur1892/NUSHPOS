using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NUSHPOS.Models;

[Table("StoreSettings")]
public class StoreSettings
{
    [Key]
    public int SettingsID { get; set; }
    public int? OrderID { get; set; }
    public string? TabName { get; set; }
    public string? GroupName { get; set; }
    public string? ParamName { get; set; }
    public string? ParamKey { get; set; }
    public string? ParamValue { get; set; }
    public string? DefaultValue { get; set; }
    public string? ParamType { get; set; }
    public int? BranchID { get; set; }
    public bool? AllowEdit { get; set; }
}
