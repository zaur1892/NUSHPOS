using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NUSHPOS.Models;

[Table("DineInTableGroups")]
public class DineInTableGroup
{
    [Key]
    public int AutoID { get; set; }
    public int TableGroupID { get; set; }
    public string? TableGroupText { get; set; }
    public string? TableGroupKey { get; set; }
    public int? TableRowCount { get; set; }
    public int? TableColumnCount { get; set; }
    public int? BranchID { get; set; }
}
