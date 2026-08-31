using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using NUSHPOS.Models;

namespace NUSHPOS.Services;

public class StationSettingsService
{
    private readonly DatabaseService _db;

    public StationSettingsService(DatabaseService db)
    {
        _db = db;
    }

    public async Task<StationSettings?> GetStationSettingsAsync(int stationId = 1)
    {
        using var connection = _db.CreateConnection();
        const string sql = @"
            SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
            SELECT 
                [AutoID], 
                [StationID], 
                [StationName], 
                [AllowRegister], 
                [DefineValue], 
                [ActivateCashier], 
                [RememberCashier], 
                [BackgroundPicture], 
                [SideBarPicture], 
                [DefaultLoginEntrance], 
                [DefultTablePlan], 
                [SkinName], 
                [ShowDineInButton], 
                [ShowTakeOutButton], 
                [ShowDriveThruButton], 
                [ShowDeliveryButton], 
                [ShowRecallButton], 
                [ShowDriverStatusButton], 
                [ShowTimeCardButton], 
                [ShowOperationsButton], 
                [ShowBackOfficeButton], 
                [ShowQuickServiceDineIn], 
                [ShowQuickServiceTakeOut], 
                [ShowQuickServiceDriveThru], 
                [ShowQuickServiceDelivery], 
                [StayInOrderScreenTakeOut], 
                [StayInOrderScreenDriveThru], 
                [StayTablePlanInDineIn], 
                [TimeOutScreenLock], 
                [IsMobile], 
                [CustomerDisplayPortNo], 
                [CustomerDisplayLine1], 
                [CustomerDisplayLine2], 
                [WeightScale1PortNo], 
                [WeightScale1BaudRate], 
                [WeightScale2PortNo], 
                [WeightScale2BaudRate], 
                [CashRegisterModel], 
                [CashRegisterBaudRate], 
                [CashRegisterDataDelay], 
                [CashRegisterLineDelay], 
                [CashRegisterStations], 
                [CashRegisterPortNo], 
                [PaymentOverTime], 
                [DirectOpenOrderInEditMode], 
                [PrintVoidedLinesOnGuestCheck], 
                CAST([StationKey] AS NVARCHAR(100)) AS [StationKey], 
                ISNULL([AskPasswordForReduce], 0) AS [AskPasswordForReduce], 
                ISNULL([MediaDisplayIsActive], 0) AS [MediaDisplayIsActive], 
                [MediaDisplayOnWaiting], 
                [MediaDisplayOnSale], 
                ISNULL([MediaDisplayClosedMessage], N'KASA BAÄLIDIR') AS [MediaDisplayClosedMessage], 
                ISNULL([MediaDisplayMoneyOver], N'TÆÅÆKKÃœR EDÄ°RÄ°K') AS [MediaDisplayMoneyOver], 
                CAST([EditKey] AS NVARCHAR(100)) AS [EditKey], 
                CAST([SyncKey] AS NVARCHAR(100)) AS [SyncKey],
                ISNULL(ShowCashTrayButton, '') AS ShowCashTrayButton,
                ISNULL(DisableSaleOnOpenCashTray, '') AS DisableSaleOnOpenCashTray,
                BekoPosDesign,
                BekoPosOutput,
                BekoPosDatabase,
                ISNULL(EnableCallCenterClient, 0) AS EnableCallCenterClient, 
                [CallCenterClientAddress],
                ISNULL(ShowScaleOrderDineIn, 0) AS ShowScaleOrderDineIn,
                ISNULL(ShowScaleOrderTakeOut, 0) AS ShowScaleOrderTakeOut,
                ISNULL(ShowScaleOrderDriveThru, 0) AS ShowScaleOrderDriveThru,
                ISNULL(ShowScaleOrderDelivery, 0) AS ShowScaleOrderDelivery,
                ISNULL(UseRetailMode, 0) AS UseRetailMode,
                ISNULL(StaticPluNumber, 0) AS StaticPluNumber,
                ISNULL(UseStaticPlu, 0) AS UseStaticPlu,
                ISNULL(EnableCentralCallCenterClient, 0) AS EnableCentralCallCenterClient,
                ISNULL(CentralCallCenterClientAddress, '') AS CentralCallCenterClientAddress,
                ISNULL(UseSecondLangOnKitchenPrint, 0) AS UseSecondLangOnKitchenPrint,
                ISNULL(CallerIDPort, '') AS CallerIDPort,
                ISNULL(MediaDisplayFontSize, 40) AS MediaDisplayFontSize,
                ISNULL(SendTareOnWeightMinus, 0) AS SendTareOnWeightMinus,
                ISNULL(SendTareOnAfterAddButton, 0) AS SendTareOnAfterAddButton,
                ISNULL(ShowScaleOrderRetail, 0) AS ShowScaleOrderRetail,
                ISNULL(ShowComboOnMediaDisplay, 0) AS ShowComboOnMediaDisplay,
                ISNULL(ShowMenuModifierOnMediaDisplay, 0) AS ShowMenuModifierOnMediaDisplay,
                ISNULL(ShowPicturedModifierOnMediaDisplay, 0) AS ShowPicturedModifierOnMediaDisplay,
                ISNULL(ShowSidePictureOnMediaDisplay, 0) AS ShowSidePictureOnMediaDisplay,
                ISNULL(ScaleMode, '0') AS ScaleMode,
                ISNULL(ShowCashDiscountButtonOnMainScreen, '0') AS ShowCashDiscountButtonOnMainScreen, 
                ISNULL(HideOkButtonOnDineIn, 0) AS [HideOkButtonOnDineIn], 
                ISNULL(HideOkButtonOnDelivery, 0) AS [HideOkButtonOnDelivery], 
                ISNULL(HideOkButtonOnDriveThru, 0) AS [HideOkButtonOnDriveThru], 
                ISNULL(HideOkButtonOnTakeOut, 0) AS [HideOkButtonOnTakeOut] 
            FROM [StationSettings]  
            WHERE StationID = @StationID;
        ";
        return await connection.QueryFirstOrDefaultAsync<StationSettings>(sql, new { StationID = stationId });
    }

    public async Task<bool> UpdateStationSettingsAsync(StationSettings s)
    {
        using var connection = _db.CreateConnection();
        const string sql = @"
            UPDATE [StationSettings] SET
                [StationName] = @StationName,
                [AllowRegister] = @AllowRegister,
                [DefineValue] = @DefineValue,
                [ActivateCashier] = @ActivateCashier,
                [RememberCashier] = @RememberCashier,
                [BackgroundPicture] = @BackgroundPicture,
                [SideBarPicture] = @SideBarPicture,
                [DefaultLoginEntrance] = @DefaultLoginEntrance,
                [DefultTablePlan] = @DefultTablePlan,
                [SkinName] = @SkinName,
                [ShowDineInButton] = @ShowDineInButton,
                [ShowTakeOutButton] = @ShowTakeOutButton,
                [ShowDriveThruButton] = @ShowDriveThruButton,
                [ShowDeliveryButton] = @ShowDeliveryButton,
                [ShowRecallButton] = @ShowRecallButton,
                [ShowDriverStatusButton] = @ShowDriverStatusButton,
                [ShowTimeCardButton] = @ShowTimeCardButton,
                [ShowOperationsButton] = @ShowOperationsButton,
                [ShowBackOfficeButton] = @ShowBackOfficeButton,
                [ShowQuickServiceDineIn] = @ShowQuickServiceDineIn,
                [ShowQuickServiceTakeOut] = @ShowQuickServiceTakeOut,
                [ShowQuickServiceDriveThru] = @ShowQuickServiceDriveThru,
                [ShowQuickServiceDelivery] = @ShowQuickServiceDelivery,
                [StayInOrderScreenTakeOut] = @StayInOrderScreenTakeOut,
                [StayInOrderScreenDriveThru] = @StayInOrderScreenDriveThru,
                [StayTablePlanInDineIn] = @StayTablePlanInDineIn,
                [TimeOutScreenLock] = @TimeOutScreenLock,
                [IsMobile] = @IsMobile,
                [CustomerDisplayPortNo] = @CustomerDisplayPortNo,
                [CustomerDisplayLine1] = @CustomerDisplayLine1,
                [CustomerDisplayLine2] = @CustomerDisplayLine2,
                [WeightScale1PortNo] = @WeightScale1PortNo,
                [WeightScale1BaudRate] = @WeightScale1BaudRate,
                [WeightScale2PortNo] = @WeightScale2PortNo,
                [WeightScale2BaudRate] = @WeightScale2BaudRate,
                [CashRegisterModel] = @CashRegisterModel,
                [CashRegisterBaudRate] = @CashRegisterBaudRate,
                [CashRegisterDataDelay] = @CashRegisterDataDelay,
                [CashRegisterLineDelay] = @CashRegisterLineDelay,
                [CashRegisterStations] = @CashRegisterStations,
                [CashRegisterPortNo] = @CashRegisterPortNo,
                [PaymentOverTime] = @PaymentOverTime,
                [DirectOpenOrderInEditMode] = @DirectOpenOrderInEditMode,
                [PrintVoidedLinesOnGuestCheck] = @PrintVoidedLinesOnGuestCheck,
                [AskPasswordForReduce] = @AskPasswordForReduce,
                [MediaDisplayIsActive] = @MediaDisplayIsActive,
                [MediaDisplayOnWaiting] = @MediaDisplayOnWaiting,
                [MediaDisplayOnSale] = @MediaDisplayOnSale,
                [MediaDisplayClosedMessage] = @MediaDisplayClosedMessage,
                [MediaDisplayMoneyOver] = @MediaDisplayMoneyOver,
                [ShowCashTrayButton] = @ShowCashTrayButton,
                [DisableSaleOnOpenCashTray] = @DisableSaleOnOpenCashTray,
                [EnableCallCenterClient] = @EnableCallCenterClient,
                [CallCenterClientAddress] = @CallCenterClientAddress,
                [ShowScaleOrderDineIn] = @ShowScaleOrderDineIn,
                [ShowScaleOrderTakeOut] = @ShowScaleOrderTakeOut,
                [ShowScaleOrderDriveThru] = @ShowScaleOrderDriveThru,
                [ShowScaleOrderDelivery] = @ShowScaleOrderDelivery,
                [UseRetailMode] = @UseRetailMode,
                [StaticPluNumber] = @StaticPluNumber,
                [UseStaticPlu] = @UseStaticPlu,
                [EnableCentralCallCenterClient] = @EnableCentralCallCenterClient,
                [CentralCallCenterClientAddress] = @CentralCallCenterClientAddress,
                [UseSecondLangOnKitchenPrint] = @UseSecondLangOnKitchenPrint,
                [CallerIDPort] = @CallerIDPort,
                [MediaDisplayFontSize] = @MediaDisplayFontSize,
                [SendTareOnWeightMinus] = @SendTareOnWeightMinus,
                [SendTareOnAfterAddButton] = @SendTareOnAfterAddButton,
                [ShowScaleOrderRetail] = @ShowScaleOrderRetail,
                [ShowComboOnMediaDisplay] = @ShowComboOnMediaDisplay,
                [ShowMenuModifierOnMediaDisplay] = @ShowMenuModifierOnMediaDisplay,
                [ShowPicturedModifierOnMediaDisplay] = @ShowPicturedModifierOnMediaDisplay,
                [ShowSidePictureOnMediaDisplay] = @ShowSidePictureOnMediaDisplay,
                [ScaleMode] = @ScaleMode,
                [ShowCashDiscountButtonOnMainScreen] = @ShowCashDiscountButtonOnMainScreen,
                [HideOkButtonOnDineIn] = @HideOkButtonOnDineIn,
                [HideOkButtonOnDelivery] = @HideOkButtonOnDelivery,
                [HideOkButtonOnDriveThru] = @HideOkButtonOnDriveThru,
                [HideOkButtonOnTakeOut] = @HideOkButtonOnTakeOut
            WHERE [StationID] = @StationID;
        ";
        var rows = await connection.ExecuteAsync(sql, s);
        return rows > 0;
    }

    public async Task<StationPrinterSettings?> GetStationPrinterSettingsAsync(int stationId = 1)
    {
        using var connection = _db.CreateConnection();
        const string sql = @"
            SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
            SELECT 
                AutoID, 
                StationID, 
                CheckPrinterName, 
                CheckAltPrinterName, 
                CheckPrinterID, 
                CheckDesignPath, 
                DeliveryPrinterName, 
                DeliveryAltPrinterName, 
                DeliveryPrinterID,  
                DeliveryDesignPath, 
                InvoicePrinterName, 
                InvoiceAltPrinterName, 
                InvoicePrinterID, 
                InvoiceDesignPath, 
                InvoiceRowCount, 
                ReportPrinterName, 
                ReportAltPrinterName,  
                ReportPrinterID, 
                ReportDesignPath, 
                AdditionPrinterName, 
                AdditionPrinterID, 
                AdditionDesignPath, 
                AdditionRowCount, 
                ReportA4PrinterName, 
                ReportA4AltPrinterName,  
                ReportA4PrinterID, 
                ReportA4DesignPath, 
                Kitchen1PrinterName, 
                Kitchen1AltPrinterName, 
                Kitchen1PrinterID, 
                Kitchen1DesignPath, 
                Kitchen2PrinterName,  
                Kitchen2AltPrinterName, 
                Kitchen2PrinterID, 
                Kitchen2DesignPath, 
                Kitchen3PrinterName, 
                Kitchen3AltPrinterName, 
                Kitchen3PrinterID, 
                Kitchen3DesignPath,  
                Kitchen4PrinterName, 
                Kitchen4AltPrinterName, 
                Kitchen4PrinterID, 
                Kitchen4DesignPath, 
                Kitchen5PrinterName, 
                Kitchen5AltPrinterName, 
                Kitchen5PrinterID,  
                Kitchen5DesignPath, 
                Kitchen6PrinterName, 
                Kitchen6AltPrinterName, 
                Kitchen6PrinterID, 
                Kitchen6DesignPath, 
                Kitchen7PrinterName, 
                Kitchen7AltPrinterName,  
                Kitchen7PrinterID, 
                Kitchen7DesignPath, 
                Kitchen8PrinterName, 
                Kitchen8AltPrinterName, 
                Kitchen8PrinterID, 
                Kitchen8DesignPath, 
                Kitchen9PrinterName,  
                Kitchen9AltPrinterName, 
                Kitchen9PrinterID, 
                Kitchen9DesignPath, 
                Kitchen10PrinterName, 
                Kitchen10AltPrinterName, 
                Kitchen10PrinterID, 
                Kitchen10DesignPath, 
                InvoiceTopFeed, 
                AdditionTopFeed, 
                ISNULL(PrintDineInOrdersKitchen, 1) AS PrintDineInOrdersKitchen,
                ISNULL(PrintBarTableOrdersKitchen, 1) AS PrintBarTableOrdersKitchen,
                ISNULL(PrintTakeOutOrdersKitchen, 1) AS PrintTakeOutOrdersKitchen,
                ISNULL(PrintDriveThruOrdersKitchen, 1) AS PrintDriveThruOrdersKitchen,
                ISNULL(PrintDeliveryOrdersKitchen, 1) AS PrintDeliveryOrdersKitchen,
                CAST([EditKey] AS NVARCHAR(100)) AS [EditKey], 
                CAST([SyncKey] AS NVARCHAR(100)) AS [SyncKey], 
                [LabelPrinterID], 
                [LabelPrinterName], 
                [LabelDesignPath], 
                [ReturnPrinterID], 
                [ReturnPrinterName], 
                [ReturnDesignPath] 
            FROM StationPrinterSettings  
            WHERE StationID = @StationID;
        ";
        return await connection.QueryFirstOrDefaultAsync<StationPrinterSettings>(sql, new { StationID = stationId });
    }

    public async Task<bool> UpdateStationPrinterSettingsAsync(StationPrinterSettings p)
    {
        using var connection = _db.CreateConnection();
        const string sql = @"
            UPDATE StationPrinterSettings SET
                CheckPrinterName = @CheckPrinterName,
                CheckPrinterID = @CheckPrinterID,
                CheckDesignPath = @CheckDesignPath,
                DeliveryPrinterName = @DeliveryPrinterName,
                DeliveryPrinterID = @DeliveryPrinterID,
                DeliveryDesignPath = @DeliveryDesignPath,
                InvoicePrinterName = @InvoicePrinterName,
                InvoicePrinterID = @InvoicePrinterID,
                InvoiceDesignPath = @InvoiceDesignPath,
                ReportPrinterName = @ReportPrinterName,
                ReportPrinterID = @ReportPrinterID,
                ReportDesignPath = @ReportDesignPath,
                AdditionPrinterName = @AdditionPrinterName,
                AdditionPrinterID = @AdditionPrinterID,
                AdditionDesignPath = @AdditionDesignPath,
                ReportA4PrinterName = @ReportA4PrinterName,
                ReportA4PrinterID = @ReportA4PrinterID,
                ReportA4DesignPath = @ReportA4DesignPath,
                Kitchen1PrinterName = @Kitchen1PrinterName,
                Kitchen1PrinterID = @Kitchen1PrinterID,
                Kitchen1DesignPath = @Kitchen1DesignPath,
                Kitchen2PrinterName = @Kitchen2PrinterName,
                Kitchen2PrinterID = @Kitchen2PrinterID,
                Kitchen2DesignPath = @Kitchen2DesignPath,
                Kitchen3PrinterName = @Kitchen3PrinterName,
                Kitchen3PrinterID = @Kitchen3PrinterID,
                Kitchen3DesignPath = @Kitchen3DesignPath,
                Kitchen4PrinterName = @Kitchen4PrinterName,
                Kitchen4PrinterID = @Kitchen4PrinterID,
                Kitchen4DesignPath = @Kitchen4DesignPath,
                Kitchen5PrinterName = @Kitchen5PrinterName,
                Kitchen5PrinterID = @Kitchen5PrinterID,
                Kitchen5DesignPath = @Kitchen5DesignPath,
                Kitchen6PrinterName = @Kitchen6PrinterName,
                Kitchen6PrinterID = @Kitchen6PrinterID,
                Kitchen6DesignPath = @Kitchen6DesignPath,
                Kitchen7PrinterName = @Kitchen7PrinterName,
                Kitchen7PrinterID = @Kitchen7PrinterID,
                Kitchen7DesignPath = @Kitchen7DesignPath,
                Kitchen8PrinterName = @Kitchen8PrinterName,
                Kitchen8PrinterID = @Kitchen8PrinterID,
                Kitchen8DesignPath = @Kitchen8DesignPath,
                Kitchen9PrinterName = @Kitchen9PrinterName,
                Kitchen9PrinterID = @Kitchen9PrinterID,
                Kitchen9DesignPath = @Kitchen9DesignPath,
                Kitchen10PrinterName = @Kitchen10PrinterName,
                Kitchen10PrinterID = @Kitchen10PrinterID,
                Kitchen10DesignPath = @Kitchen10DesignPath,
                InvoiceRowCount = @InvoiceRowCount,
                InvoiceTopFeed = @InvoiceTopFeed,
                AdditionRowCount = @AdditionRowCount,
                AdditionTopFeed = @AdditionTopFeed,
                PrintDineInOrdersKitchen = @PrintDineInOrdersKitchen,
                PrintBarTableOrdersKitchen = @PrintBarTableOrdersKitchen,
                PrintTakeOutOrdersKitchen = @PrintTakeOutOrdersKitchen,
                PrintDriveThruOrdersKitchen = @PrintDriveThruOrdersKitchen,
                PrintDeliveryOrdersKitchen = @PrintDeliveryOrdersKitchen,
                LabelPrinterID = @LabelPrinterID,
                LabelPrinterName = @LabelPrinterName,
                LabelDesignPath = @LabelDesignPath,
                ReturnPrinterID = @ReturnPrinterID,
                ReturnPrinterName = @ReturnPrinterName,
                ReturnDesignPath = @ReturnDesignPath
            WHERE StationID = @StationID;
        ";
        var rows = await connection.ExecuteAsync(sql, p);
        return rows > 0;
    }

    public async Task<IEnumerable<DineInTableGroup>> GetTableGroupsAsync()
    {
        using var connection = _db.CreateConnection();
        const string sql = @"
            SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
            SELECT 
                [AutoID], 
                [TableGroupID], 
                CAST([TableGroupKey] AS NVARCHAR(100)) AS [TableGroupKey], 
                [TableGroupText], 
                [RevenueCenterTypeID], 
                [DeleteReason], 
                [BranchID], 
                ISNULL(TableRowCount, 8) AS TableRowCount, 
                ISNULL(TableColumnCount, 9) AS TableColumnCount  
            FROM [DineInTableGroups]
            ORDER BY [TableGroupID];
        ";
        return await connection.QueryAsync<DineInTableGroup>(sql);
    }

    public async Task<IEnumerable<LanguageParam>> GetLanguageParamsAsync()
    {
        using var connection = _db.CreateConnection();
        const string sql = @"
            SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
            SELECT 
                [ParamName], 
                [ParamValue]
            FROM [Params] 
            WHERE ParamName LIKE 'lang%';
        ";
        return await connection.QueryAsync<LanguageParam>(sql);
    }

    public async Task<IEnumerable<Printer>> GetPrintersAsync()
    {
        using var connection = _db.CreateConnection();
        const string sql = "SELECT [AutoID], [PrinterID], [PrinterName] FROM [Printers] ORDER BY [PrinterID];";
        return await connection.QueryAsync<Printer>(sql);
    }

    public async Task<IEnumerable<PrinterDesign>> GetPrinterDesignsAsync()
    {
        using var connection = _db.CreateConnection();
        const string sql = @"
            SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
            SELECT 
                [AutoID], 
                CAST([DesignKey] AS NVARCHAR(100)) AS [DesignKey], 
                [DocumentTypeID], 
                [DesignName], 
                [DesignData], 
                [IsDefault]
            FROM [PrinterDesigns]
            ORDER BY [DocumentTypeID], [DesignName];
        ";
        return await connection.QueryAsync<PrinterDesign>(sql);
    }
}