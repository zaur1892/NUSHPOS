using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NUSHPOS.Models;

[Table("TaxGroups")]
public class TaxGroup
{
    [Key]
    public int TaxGroupID { get; set; }
    public string? GroupName { get; set; }
    public decimal? TaxRate { get; set; }
    public string? EditKey { get; set; }
    public string? SyncKey { get; set; }
    public int? BranchID { get; set; }
    public bool? IsActive { get; set; }
    public string? Ingenico { get; set; }
}

