using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NUSHPOS.Models;

[Table("MenuSubCategories")]
public class MenuSubCategory
{
    [Key]
    public int AutoID { get; set; }
    public int MenuSubCategoryID { get; set; }
    public string? MenuSubCategoryKey { get; set; }
    public string? MenuSubCategoryText { get; set; }
    public bool? MenuSubCategoryActive { get; set; }
    public decimal? DefaultTaxPercent { get; set; }
    public int? RevenueCenterTypeID { get; set; }
    public string? DeleteReason { get; set; }
    public string? CustomField1 { get; set; }
    public string? CustomField2 { get; set; }
    public string? CustomField3 { get; set; }
    public string? CustomField4 { get; set; }
    public string? CustomField5 { get; set; }
    public string? EditKey { get; set; }
    public string? SyncKey { get; set; }
    public int? BranchID { get; set; }
    public int? AddUserID { get; set; }
    public DateTime? AddDateTime { get; set; }
    public int? EditUserID { get; set; }
    public DateTime? EditDateTime { get; set; }
}
