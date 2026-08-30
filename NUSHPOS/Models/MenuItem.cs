using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NUSHPOS.Models;

[Table("MenuItems")]
public class MenuItem
{
    [Key]
    public int AutoID { get; set; }
    public int MenuItemID { get; set; }
    public int? MainMenuItemID { get; set; }
    public string? MenuItemText { get; set; }
    public int? MenuCategoryID { get; set; }
    public int? MenuGroupID { get; set; }
    public int? DisplayIndex { get; set; }
    public int? RevenueCenterTypeID { get; set; }
    public decimal? DefaultUnitPrice { get; set; }
    public decimal? MenuItemCost { get; set; }
    public string? MenuItemDescription { get; set; }
    public string? MenuItemNotification { get; set; }
    public bool? MenuItemActive { get; set; }
    public bool? MenuItemInStock { get; set; }
    public bool? MenuItemTaxable { get; set; }
    public decimal? TaxPercent { get; set; }
    public bool? MenuItemDiscountable { get; set; }
    public string? PictureName { get; set; }
    public bool? ShowCaption { get; set; }
    public bool? IsComboMenu { get; set; }
    public bool? IsTopMenu { get; set; }
    public string? ButtonColor { get; set; }
    public string? Barcode { get; set; }
    public decimal? DineInPrice { get; set; }
    public decimal? TakeOutPrice { get; set; }
    public decimal? DeliveryPrice { get; set; }
    public string? MenuItemKey { get; set; }
    public string? MainMenuItemKey { get; set; }
    public string? MenuCategoryKey { get; set; }
    public string? MenuGroupKey { get; set; }
    public string? TaxGroupKey { get; set; }
    public string? MenuModifierKey { get; set; }
    public string? MenuModifierForcedKey { get; set; }
    public int? BranchID { get; set; }
    public string? SecLangMenuItemText { get; set; }
    public int? UsedPrinterID1 { get; set; }
    public int? UsedPrinterID2 { get; set; }
    public int? UsedPrinterID3 { get; set; }
    public int? UsedPrinterID4 { get; set; }
    public int? UsedPrinterID5 { get; set; }
    public string? MenuItemGroupText { get; set; }
    public string? MenumItemCategoryText { get; set; }
    public string? MenuCategoryText { get; set; }
    public string? MenuSubCategoryText { get; set; }
    public string? TaxGroupText { get; set; }
    public string? MenuModifierText { get; set; }
    public string? MenuForcedModifierText { get; set; }
    public string? BranchName { get; set; }
    public int? MenuScreenGroupID { get; set; }
    public string? MenuScreenGroupKey { get; set; }
    public int? DisplayIndex2 { get; set; }
    public string? Barcode2 { get; set; }
    public decimal? ItemDelCharge { get; set; }
    public decimal? ItemDelComp { get; set; }
    public decimal? BarTabPrice { get; set; }
    public decimal? DriveThruPrice { get; set; }
    public bool? OrderByWeight { get; set; }
    public bool? PrintPizzaLabel { get; set; }
    public int? KitchenSortNumber { get; set; }
    public int? ModBuilderTemplateID { get; set; }
    public int? MenuItemTypeID { get; set; }
    public string? AccountingCode { get; set; }
    public bool? PrintOnLabel { get; set; }
    public int? SecurityLevel { get; set; }
    public string? DeleteReason { get; set; }
    public string? CustomField1 { get; set; }
    public string? CustomField2 { get; set; }
    public string? CustomField3 { get; set; }
    public string? CustomField4 { get; set; }
    public string? CustomField5 { get; set; }
    public string? EditKey { get; set; }
    public string? SyncKey { get; set; }
    public int? AddUserID { get; set; }
    public DateTime? AddDateTime { get; set; }
    public int? EditUserID { get; set; }
    public DateTime? EditDateTime { get; set; }
    public bool? UseKds1 { get; set; }
    public bool? UseKds2 { get; set; }
    public bool? UseKds3 { get; set; }
    public bool? UseKds4 { get; set; }
    public bool? UseKds5 { get; set; }
    public bool? UseKds6 { get; set; }
    public bool? UseKds7 { get; set; }
    public bool? UseKds8 { get; set; }
    public bool? UseKds9 { get; set; }
    public bool? UseKds10 { get; set; }
    public DateTime? CountDownDate { get; set; }
    public decimal? CountDownValue { get; set; }
    public decimal? CountDownActualResult { get; set; }
    public int? TaxGroupID { get; set; }
    public int? MenuModifierID { get; set; }
    public int? MenuModifierForcedID { get; set; }
    public string? ComboMenuItemKey { get; set; }
}
