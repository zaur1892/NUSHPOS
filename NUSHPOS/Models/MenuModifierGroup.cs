using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NUSHPOS.Models;

[Table("MenuModifierGroups")]
public class MenuModifierGroup
{
    [Key]
    public int MenuModifierGroupID { get; set; }
    public string? MenuModifierGroupText { get; set; }
    public int? DisplayIndex { get; set; }
    public bool? MenuModifierGroupActive { get; set; }
    public string? SecLangMenuModifierGroupText { get; set; }
    public string? PictureName { get; set; }
    public string? ButtonColor { get; set; }
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
    public string? MenuModifierGroupKey { get; set; }
}

