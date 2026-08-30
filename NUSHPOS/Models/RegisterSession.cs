using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NUSHPOS.Models;

[Table("RegisterSessions")]
public class RegisterSession
{
    [Key]
    public int RegisterSessionID { get; set; }
    public int? BranchID { get; set; }
    public int? EmployeeID { get; set; }
    public string? RegisterSessionKey { get; set; }
    public int? StationID { get; set; }
    public DateTime? SignInDateTime { get; set; }
    public decimal? RegisterStartAmount { get; set; }
    public DateTime? SignOutDateTime { get; set; }
    public decimal? RegisterEndAmount { get; set; }
    public decimal? DiscrepancyAmount { get; set; }
    public string? DiscrepancyNotes { get; set; }
    public string? EmployeeKey { get; set; }
}
