using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NUSHPOS.Models;

[Table("ReportCategories")]
public class ReportCategory
{
    [Key]
    public int ReportCategoryID { get; set; }
    public string? CategoryName { get; set; }
    public int? MainCategoryID { get; set; }
}
