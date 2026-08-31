using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NUSHPOS.Models;

[Table("PrinterDesigns")]
public class PrinterDesign
{
    [Key]
    public int AutoID { get; set; }
    public string? DesignKey { get; set; }
    public int? DocumentTypeID { get; set; }
    public string? DesignName { get; set; }
    public string? DesignData { get; set; }
    public bool? IsDefault { get; set; }
    public string? EditKey { get; set; }
    public string? SyncKey { get; set; }
}
