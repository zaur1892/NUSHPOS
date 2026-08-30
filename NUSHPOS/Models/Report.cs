using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NUSHPOS.Models;

[Table("Reports")]
public class Report
{
    [Key]
    public int AutoID { get; set; }
    public int ReportID { get; set; }
    public string? ReportKey { get; set; }
    public string? ReportName { get; set; }
    public int? ReportCategoryID { get; set; }
    public bool? ReportActive { get; set; }
    public int? SecurityLevel { get; set; }
    public int? ReportTypeID { get; set; }
    public bool? IsPosReport { get; set; }
}
