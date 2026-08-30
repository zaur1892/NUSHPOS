using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NUSHPOS.Models;

[Table("DineInTables")]
public class DineInTable
{
    [Key]
    public int AutoID { get; set; }
    public int DineInTableID { get; set; }
    public string? DineInTableText { get; set; }
    public int TableGroupID { get; set; }
    public int? DisplayIndex { get; set; }
    public bool? DineInTableActive { get; set; }
    public int? MaxGuests { get; set; }
    public string? DineInTableKey { get; set; }
    public string? TableGroupKey { get; set; }
    public int? BranchID { get; set; }
}
