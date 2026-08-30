using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NUSHPOS.Models;

[Table("MenuGroups")]
public class MenuGroup
{
    [Key]
    public int AutoID { get; set; }
    public int MenuGroupID { get; set; }
    public string? MenuGroupKey { get; set; }
    public string? MenuGroupText { get; set; }
    public int? DisplayIndex { get; set; }
    public bool? MenuGroupActive { get; set; }
    public string? SecLangMenuGroupText { get; set; }
    public string? PictureName { get; set; }
    public bool? ShowCaption { get; set; }
    public bool? HideInDineIn { get; set; }
    public bool? HideInBar { get; set; }
    public bool? HideInTakeaway { get; set; }
    public bool? HideInCounter { get; set; }
    public bool? HideInDelivery { get; set; }
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
}
