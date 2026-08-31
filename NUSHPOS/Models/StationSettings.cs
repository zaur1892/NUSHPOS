using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NUSHPOS.Models;

[Table("StationSettings")]
public class StationSettings
{
    [Key]
    public int AutoID { get; set; }
    public int StationID { get; set; } = 1;
    public string? StationName { get; set; }
    public bool? AllowRegister { get; set; }
    public string? DefineValue { get; set; }
    public bool? ActivateCashier { get; set; }
    public bool? RememberCashier { get; set; }
    public string? BackgroundPicture { get; set; }
    public string? SideBarPicture { get; set; }
    public string? DefaultLoginEntrance { get; set; }
    public string? DefultTablePlan { get; set; }
    public string? SkinName { get; set; }
    public bool? ShowDineInButton { get; set; }
    public bool? ShowTakeOutButton { get; set; }
    public bool? ShowDriveThruButton { get; set; }
    public bool? ShowDeliveryButton { get; set; }
    public bool? ShowRecallButton { get; set; }
    public bool? ShowDriverStatusButton { get; set; }
    public bool? ShowTimeCardButton { get; set; }
    public bool? ShowOperationsButton { get; set; }
    public bool? ShowBackOfficeButton { get; set; }
    public bool? ShowQuickServiceDineIn { get; set; }
    public bool? ShowQuickServiceTakeOut { get; set; }
    public bool? ShowQuickServiceDriveThru { get; set; }
    public bool? ShowQuickServiceDelivery { get; set; }
    public bool? StayInOrderScreenTakeOut { get; set; }
    public bool? StayInOrderScreenDriveThru { get; set; }
    public bool? StayTablePlanInDineIn { get; set; }
    public int? TimeOutScreenLock { get; set; }
    public bool? IsMobile { get; set; }
    public string? CustomerDisplayPortNo { get; set; }
    public string? CustomerDisplayLine1 { get; set; }
    public string? CustomerDisplayLine2 { get; set; }
    public string? WeightScale1PortNo { get; set; }
    public int? WeightScale1BaudRate { get; set; }
    public string? WeightScale2PortNo { get; set; }
    public int? WeightScale2BaudRate { get; set; }
    public string? CashRegisterModel { get; set; }
    public int? CashRegisterBaudRate { get; set; }
    public int? CashRegisterDataDelay { get; set; }
    public int? CashRegisterLineDelay { get; set; }
    public string? CashRegisterStations { get; set; }
    public string? CashRegisterPortNo { get; set; }
    public bool? PaymentOverTime { get; set; }
    public bool? DirectOpenOrderInEditMode { get; set; }
    public bool? PrintVoidedLinesOnGuestCheck { get; set; }
    public string? StationKey { get; set; }
    public bool AskPasswordForReduce { get; set; }
    public bool MediaDisplayIsActive { get; set; }
    public string? MediaDisplayOnWaiting { get; set; }
    public string? MediaDisplayOnSale { get; set; }
    public string MediaDisplayClosedMessage { get; set; } = "KASA BAĞLIDIR";
    public string MediaDisplayMoneyOver { get; set; } = "TƏŞƏKKÜR EDİRİK";
    public string? EditKey { get; set; }
    public string? SyncKey { get; set; }
    public string? ShowCashTrayButton { get; set; }
    public string? DisableSaleOnOpenCashTray { get; set; }
    public string? BekoPosDesign { get; set; }
    public string? BekoPosOutput { get; set; }
    public string? BekoPosDatabase { get; set; }
    public bool EnableCallCenterClient { get; set; }
    public string? CallCenterClientAddress { get; set; }
    public bool ShowScaleOrderDineIn { get; set; }
    public bool ShowScaleOrderTakeOut { get; set; }
    public bool ShowScaleOrderDriveThru { get; set; }
    public bool ShowScaleOrderDelivery { get; set; }
    public bool UseRetailMode { get; set; }
    public int StaticPluNumber { get; set; }
    public bool UseStaticPlu { get; set; }
    public bool EnableCentralCallCenterClient { get; set; }
    public string CentralCallCenterClientAddress { get; set; } = "";
    public bool UseSecondLangOnKitchenPrint { get; set; }
    public string CallerIDPort { get; set; } = "";
 public int MediaDisplayFontSize { get; set; } = 40;
 public bool SendTareOnWeightMinus { get; set; }
 public bool SendTareOnAfterAddButton { get; set; }
 public bool ShowScaleOrderRetail { get; set; }
 public bool ShowComboOnMediaDisplay { get; set; }
 public bool ShowMenuModifierOnMediaDisplay { get; set; }
 public bool ShowPicturedModifierOnMediaDisplay { get; set; }
 public bool ShowSidePictureOnMediaDisplay { get; set; }
    public string ScaleMode { get; set; } = "0";
    public string ShowCashDiscountButtonOnMainScreen { get; set; } = "0";
 public bool HideOkButtonOnDineIn { get; set; }
 public bool HideOkButtonOnDelivery { get; set; }
 public bool HideOkButtonOnDriveThru { get; set; }
 public bool HideOkButtonOnTakeOut { get; set; }
 public string? Language { get; set; }
}
