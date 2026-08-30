using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NUSHPOS.Models;

[Table("Printers")]
public class Printer
{
    [Key]
    public int AutoID { get; set; }
    public int PrinterID { get; set; }
    public string? PrinterName { get; set; }
    public string? EditKey { get; set; }
    public string? SyncKey { get; set; }
}
