using Dapper;
using NUSHPOS.Models;

namespace NUSHPOS.Services;

public class MenuService
{
    private readonly DatabaseService _db;

    public MenuService(DatabaseService db)
    {
        _db = db;
    }

    public async Task<IEnumerable<MenuGroup>> GetMenuGroupsAsync()
    {
        using var connection = _db.CreateConnection();
        const string sql = "SELECT MenuGroupID, MenuGroupText, CAST(MenuGroupKey AS NVARCHAR(100)) AS MenuGroupKey, DisplayIndex, PictureName, ButtonColor, BranchID FROM MenuGroups WHERE ISNULL(MenuGroupActive, 1) = 1 ORDER BY DisplayIndex";
        return await connection.QueryAsync<MenuGroup>(sql);
    }

    public async Task<IEnumerable<MenuGroup>> GetMenuGroupsFullAsync()
    {
        using var connection = _db.CreateConnection();
        const string sql = @"
            SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

            SELECT 
                [MenuGroupID],
                CAST([MenuGroupKey] AS NVARCHAR(100)) AS [MenuGroupKey],
                [MenuGroupText],
                [DisplayIndex],
                [MenuGroupActive],
                [SecLangMenuGroupText],
                [PictureName],
                [ShowCaption],
                [HideInDineIn],
                [HideInBar],
                [HideInTakeaway],
                [HideInCounter],
                [HideInDelivery],
                [ButtonColor],
                [RevenueCenterTypeID],
                [DeleteReason],
                [CustomField1],
                [CustomField2],
                [CustomField3],
                [CustomField4],
                [CustomField5],
                CAST([EditKey] AS NVARCHAR(100)) AS [EditKey],
                CAST([SyncKey] AS NVARCHAR(100)) AS [SyncKey],
                [BranchID],
                [AddUserID],
                [AddDateTime],
                [EditUserID],
                [EditDateTime]
            FROM [MenuGroups]
            ORDER BY [DisplayIndex];
        ";
        return await connection.QueryAsync<MenuGroup>(sql);
    }

    public async Task<IEnumerable<MenuItem>> GetMenuItemsLayoutByGroupIdAsync(int menuGroupId)
    {
        using var connection = _db.CreateConnection();
        const string sql = @"
            SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

            SELECT 
                MenuItems.AutoID, 
                MenuItems.MenuItemID, 
                ISNULL(MenuItems.MainMenuItemID, 0) AS MainMenuItemID, 
                MenuItems.RevenueCenterTypeID, 
                MenuItems.MenuItemText, 
                MenuItems.MenuCategoryID, 
                MenuItems.MenuGroupID, 
                MenuItems.DisplayIndex AS DisplayIndex2, 
                MenuItems.DefaultUnitPrice, 
                MenuItems.MenuItemCost, 
                MenuItems.MenuItemDescription, 
                MenuItems.MenuItemNotification, 
                MenuItems.MenuItemActive, 
                MenuItems.MenuItemInStock, 
                MenuItems.MenuItemTaxable, 
                ISNULL(TaxGroups.TaxRate, 8) AS TaxPercent, 
                MenuItems.MenuModifierID, 
                MenuItems.MenuItemDiscountable, 
                MenuItems.MenuItemPopUpHeaderID, 
                MenuItems.MenuItemPopUpChoiceText, 
                MenuItems.HasModifierPopUps, 
                ISNULL(MenuItems.SecLangMenuItemText, '') AS SecLangMenuItemText, 
                MenuItems.SecLangPopUpChoiceText, 
                MenuItems.PictureName, 
                MenuItems.ShowCaption, 
                ISNULL(MenuItems.IsComboMenu, 0) AS IsComboMenu,
                ISNULL(MenuItems.IsTopMenu, 0) AS IsTopMenu, 
                MenuItems.ButtonColor, 
                ISNULL(MenuItems.Barcode, '') AS Barcode,
                ISNULL(MenuItems.Barcode2, '') AS Barcode2, 
                MenuItems.ItemDelCharge, 
                MenuItems.ItemDelComp, 
                MenuItems.DineInPrice, 
                MenuItems.BarTabPrice, 
                MenuItems.TakeOutPrice, 
                MenuItems.DriveThruPrice, 
                MenuItems.DeliveryPrice, 
                MenuItems.OrderByWeight, 
                MenuItems.PrintPizzaLabel, 
                MenuItems.KitchenSortNumber, 
                MenuItems.ModBuilderTemplateID, 
                MenuItems.MenuItemTypeID, 
                ISNULL(MenuItems.AccountingCode, '') AS AccountingCode, 
                ISNULL(MenuItems.PrintOnLabel, 0) AS PrintOnLabel, 
                MenuItems.UsedPrinterID1, 
                MenuItems.UsedPrinterID2, 
                MenuItems.UsedPrinterID3, 
                MenuItems.UsedPrinterID4, 
                MenuItems.UsedPrinterID5, 
                MenuItems.SecurityLevel, 
                MenuItems.DeleteReason, 
                MenuItems.CustomField1, 
                MenuItems.CustomField2, 
                MenuItems.CustomField3, 
                MenuItems.CustomField4, 
                MenuItems.CustomField5, 
                CAST(MenuItems.EditKey AS NVARCHAR(100)) AS EditKey, 
                CAST(MenuItems.SyncKey AS NVARCHAR(100)) AS SyncKey, 
                MenuItems.BranchID, 
                MenuItems.AddUserID, 
                MenuItems.AddDateTime, 
                MenuItems.EditUserID, 
                ISNULL(MenuItems.UseKds1, 0) AS UseKds1, 
                ISNULL(MenuItems.UseKds2, 0) AS UseKds2, 
                ISNULL(MenuItems.UseKds3, 0) AS UseKds3,
                ISNULL(MenuItems.UseKds4, 0) AS UseKds4, 
                ISNULL(MenuItems.UseKds5, 0) AS UseKds5,
                ISNULL(MenuItems.UseKds6, 0) AS UseKds6,
                ISNULL(MenuItems.UseKds7, 0) AS UseKds7,
                ISNULL(MenuItems.UseKds8, 0) AS UseKds8, 
                ISNULL(MenuItems.UseKds9, 0) AS UseKds9,
                ISNULL(MenuItems.UseKds10, 0) AS UseKds10, 
                MenuItems.CountDownDate, 
                ISNULL(MenuItems.CountDownValue, 0.0) AS CountDownValue,
                ISNULL(MenuItems.CountDownActualResult, 0.0) AS CountDownActualResult,  
                MenuItems.EditDateTime, 
                MenuCategories.MenuCategoryText, 
                MenuSubCategories.MenuSubCategoryText, 
                TaxGroups.GroupName AS TaxGroupText, 
                MenuModifierGroups.MenuModifierGroupText AS MenuModifierText, 
                MenuItems.DisplayIndex AS MenuDisplayIndex, 
                CAST(MenuItems.MenuItemKey AS NVARCHAR(100)) AS MenuItemKey, 
                CAST(MenuItems.MainMenuItemKey AS NVARCHAR(100)) AS MainMenuItemKey, 
                CAST(MenuItems.MenuItemGlobalKey AS NVARCHAR(100)) AS MenuItemGlobalKey, 
                CAST(MenuItems.MenuCategoryKey AS NVARCHAR(100)) AS MenuCategoryKey, 
                CAST(MenuItems.MenuGroupKey AS NVARCHAR(100)) AS MenuGroupKey, 
                MenuItems.TaxGroupID, 
                CAST(MenuItems.TaxGroupKey AS NVARCHAR(100)) AS TaxGroupKey, 
                CAST(MenuItems.MenuModifierKey AS NVARCHAR(100)) AS MenuModifierKey, 
                MenuItems.MenuModifierForcedID, 
                CAST(MenuItems.MenuModifierForcedKey AS NVARCHAR(100)) AS MenuModifierForcedKey, 
                MenuForcedModifierGroups.MenuModifierGroupText AS MenuForcedModifierText,
                efr_Branchs.BranchName,
                MenuItemLayout.DisplayIndex AS DisplayIndex,
                MenuItemLayout.MenuGroupID AS MenuScreenGroupID,
                CAST(MenuItemLayout.MenuGroupKey AS NVARCHAR(100)) AS MenuScreenGroupKey 
            FROM MenuItems  
            LEFT OUTER JOIN MenuModifierGroups AS MenuForcedModifierGroups 
                ON MenuItems.MenuModifierForcedKey = MenuForcedModifierGroups.MenuModifierGroupKey 
            LEFT OUTER JOIN MenuModifierGroups 
                ON MenuItems.MenuModifierKey = MenuModifierGroups.MenuModifierGroupKey 
            LEFT OUTER JOIN MenuSubCategories 
                ON MenuItems.MenuGroupKey = MenuSubCategories.MenuSubCategoryKey 
            LEFT OUTER JOIN MenuCategories 
                ON MenuItems.MenuCategoryKey = MenuCategories.MenuCategoryKey 
            LEFT OUTER JOIN TaxGroups 
                ON MenuItems.TaxGroupID = TaxGroups.TaxGroupID  
            LEFT OUTER JOIN efr_Branchs 
                ON MenuItems.BranchID = efr_Branchs.BranchID 
            INNER JOIN MenuItemLayout 
                ON (MenuItemLayout.MenuItemKey = MenuItems.MenuItemKey 
                    OR MenuItemLayout.MenuItemID = MenuItems.MenuItemID 
                    OR MenuItemLayout.MenuItemID = MenuItems.AutoID)
            WHERE (MenuItemLayout.MenuGroupID = @MenuGroupID 
                   OR MenuItemLayout.MenuGroupKey = (SELECT TOP 1 MenuGroupKey FROM MenuGroups WHERE MenuGroupID = @MenuGroupID))
            ORDER BY MenuItemLayout.DisplayIndex;
        ";
        return await connection.QueryAsync<MenuItem>(sql, new { MenuGroupID = menuGroupId });
    }

    public async Task<IEnumerable<MenuItem>> GetMenuItemsByGroupAsync(int menuGroupId)
    {
        using var connection = _db.CreateConnection();
        const string sql = @"
            SELECT m.AutoID, m.MenuItemID, m.MenuItemText, m.MenuCategoryID, m.MenuGroupID, m.DisplayIndex, m.DefaultUnitPrice, m.MenuItemCost, m.MenuItemDescription, m.TaxPercent, m.PictureName, m.ButtonColor, m.Barcode, m.DineInPrice, m.TakeOutPrice, m.DeliveryPrice, m.UsedPrinterID1, m.UsedPrinterID2, m.UsedPrinterID3, m.UsedPrinterID4, m.UsedPrinterID5, CAST(m.MenuItemKey AS NVARCHAR(100)) AS MenuItemKey, CAST(m.MenuCategoryKey AS NVARCHAR(100)) AS MenuCategoryKey, CAST(m.MenuGroupKey AS NVARCHAR(100)) AS MenuGroupKey, m.BranchID, mg.MenuGroupText AS MenuItemGroupText, mc.MenuCategoryText AS MenumItemCategoryText
            FROM MenuItems m
            INNER JOIN MenuItemLayout l ON m.MenuItemID = l.MenuItemID
            LEFT JOIN MenuGroups mg ON l.MenuGroupID = mg.MenuGroupID
            LEFT JOIN MenuCategories mc ON m.MenuCategoryID = mc.MenuCategoryID
            WHERE l.MenuGroupID = @MenuGroupId AND ISNULL(m.MenuItemActive, 1) = 1
            ORDER BY l.DisplayIndex";
        return await connection.QueryAsync<MenuItem>(sql, new { MenuGroupId = menuGroupId });
    }

    public async Task<IEnumerable<MenuItem>> GetAllActiveMenuItemsAsync()
    {
        using var connection = _db.CreateConnection();
        const string sql = @"
            SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
            SELECT 
                AutoID, 
                MenuItemID, 
                MenuItemText, 
                MenuCategoryID, 
                MenuGroupID, 
                TaxGroupID,
                DisplayIndex, 
                DefaultUnitPrice, 
                MenuItemCost, 
                MenuItemDescription, 
                MenuItemNotification, 
                MenuItemActive, 
                MenuItemInStock, 
                MenuItemTaxable, 
                TaxPercent, 
                MenuModifierID, 
                MenuItemDiscountable, 
                SecLangMenuItemText, 
                PictureName, 
                ShowCaption, 
                IsComboMenu, 
                IsTopMenu, 
                ButtonColor, 
                Barcode, 
                Barcode2, 
                ItemDelCharge, 
                ItemDelComp, 
                DineInPrice, 
                BarTabPrice, 
                TakeOutPrice, 
                DriveThruPrice, 
                DeliveryPrice, 
                OrderByWeight, 
                PrintPizzaLabel, 
                KitchenSortNumber, 
                ModBuilderTemplateID, 
                MenuItemTypeID, 
                AccountingCode, 
                UsedPrinterID1, 
                UsedPrinterID2, 
                UsedPrinterID3, 
                UsedPrinterID4, 
                UsedPrinterID5, 
                UseKds1, UseKds2, UseKds3, UseKds4, UseKds5, UseKds6, UseKds7, UseKds8, UseKds9, UseKds10, 
                CAST(MenuItemKey AS NVARCHAR(100)) AS MenuItemKey, 
                CAST(MenuCategoryKey AS NVARCHAR(100)) AS MenuCategoryKey, 
                CAST(MenuGroupKey AS NVARCHAR(100)) AS MenuGroupKey, 
                CAST(MenuModifierKey AS NVARCHAR(100)) AS MenuModifierKey, 
                CAST(MenuModifierForcedKey AS NVARCHAR(100)) AS MenuModifierForcedKey,
                SecurityLevel,
                PrintOnLabel
            FROM MenuItems 
            WHERE ISNULL(MenuItemActive, 1) = 1
            ORDER BY MenuItemText;
        ";
        return await connection.QueryAsync<MenuItem>(sql);
    }

    public async Task<IEnumerable<MenuCategory>> GetCategoriesAsync()
    {
        using var connection = _db.CreateConnection();
        const string sql = @"
            SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
            SELECT 
                [MenuCategoryID],
                CAST([MenuCategoryKey] AS NVARCHAR(100)) AS [MenuCategoryKey],
                [MenuCategoryText],
                [MenuCategoryActive],
                [DefaultTaxPercent],
                [RevenueCenterTypeID],
                [DeleteReason],
                [CustomField1],
                [CustomField2],
                [CustomField3],
                [CustomField4],
                [CustomField5],
                CAST([EditKey] AS NVARCHAR(100)) AS [EditKey],
                CAST([SyncKey] AS NVARCHAR(100)) AS [SyncKey],
                [BranchID],
                [AddUserID],
                [AddDateTime],
                [EditUserID],
                [EditDateTime]
            FROM [MenuCategories] 
            WHERE ISNULL([MenuCategoryActive], 0) = 1
            ORDER BY [MenuCategoryID]";
        return await connection.QueryAsync<MenuCategory>(sql);
    }

    public async Task<IEnumerable<MenuSubCategory>> GetActiveSubCategoriesAsync()
    {
        using var connection = _db.CreateConnection();
        const string sql = @"
            SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
            SELECT 
                [MenuSubCategoryID],
                CAST([MenuSubCategoryKey] AS NVARCHAR(100)) AS [MenuSubCategoryKey],
                [MenuSubCategoryText],
                [MenuSubCategoryActive],
                [DefaultTaxPercent],
                [RevenueCenterTypeID],
                [DeleteReason],
                [CustomField1],
                [CustomField2],
                [CustomField3],
                [CustomField4],
                [CustomField5],
                CAST([EditKey] AS NVARCHAR(100)) AS [EditKey],
                CAST([SyncKey] AS NVARCHAR(100)) AS [SyncKey],
                [BranchID],
                [AddUserID],
                [AddDateTime],
                [EditUserID],
                [EditDateTime]
            FROM [MenuSubCategories] 
            WHERE ISNULL([MenuSubCategoryActive], 0) = 1
            ORDER BY [MenuSubCategoryID]";
        return await connection.QueryAsync<MenuSubCategory>(sql);
    }

    public async Task<IEnumerable<Printer>> GetPrintersAsync()
    {
        using var connection = _db.CreateConnection();
        const string sql = @"
            SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
            SELECT 
                [AutoID],
                [PrinterID],
                [PrinterName],
                CAST([EditKey] AS NVARCHAR(100)) AS [EditKey],
                CAST([SyncKey] AS NVARCHAR(100)) AS [SyncKey]
            FROM [Printers]
            ORDER BY [PrinterID]";
        return await connection.QueryAsync<Printer>(sql);
    }

    public async Task<IEnumerable<MenuComboItem>> GetComboMenuNamesAsync()
    {
        using var connection = _db.CreateConnection();
        const string sql = @"
            SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
            SELECT 
                ExtendedMenuName,
                CAST(ComboMenuItemKey AS NVARCHAR(100)) AS ComboMenuItemKey 
            FROM MenuComboItems 
            WHERE ISNULL(ExtendedMenuName, '') <> '' 
            GROUP BY ExtendedMenuName, ComboMenuItemKey";
        return await connection.QueryAsync<MenuComboItem>(sql);
    }

    public async Task<IEnumerable<TaxGroup>> GetTaxGroupsAsync()
    {
        using var connection = _db.CreateConnection();
        const string sql = @"
            SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
            SELECT 
                TaxGroupID,
                GroupName,
                TaxRate,
                CAST(EditKey AS NVARCHAR(100)) AS EditKey,
                CAST(SyncKey AS NVARCHAR(100)) AS SyncKey,
                BranchID,
                IsActive,
                ingenico
            FROM TaxGroups";
        return await connection.QueryAsync<TaxGroup>(sql);
    }

    public async Task<MenuItem?> GetMenuItemByKeyAsync(string menuItemKey)
    {
        using var connection = _db.CreateConnection();
        const string sql = @"
            SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
            SELECT 
                MenuItems.AutoID, 
                MenuItems.MenuItemID, 
                ISNULL(MenuItems.MainMenuItemID, 0) AS MainMenuItemID, 
                MenuItems.RevenueCenterTypeID, 
                MenuItems.MenuItemText, 
                MenuItems.MenuCategoryID, 
                MenuItems.MenuGroupID, 
                MenuItems.DisplayIndex AS DisplayIndex, 
                MenuItems.DefaultUnitPrice, 
                MenuItems.MenuItemCost, 
                MenuItems.MenuItemDescription, 
                MenuItems.MenuItemNotification, 
                MenuItems.MenuItemActive, 
                MenuItems.MenuItemInStock, 
                MenuItems.MenuItemTaxable, 
                ISNULL(TaxGroups.TaxRate, 8) AS TaxPercent, 
                MenuItems.MenuModifierID, 
                MenuItems.MenuItemDiscountable, 
                MenuItems.MenuItemPopUpHeaderID, 
                MenuItems.MenuItemPopUpChoiceText, 
                MenuItems.HasModifierPopUps, 
                ISNULL(MenuItems.SecLangMenuItemText, '') AS SecLangMenuItemText, 
                MenuItems.SecLangPopUpChoiceText, 
                MenuItems.PictureName, 
                MenuItems.ShowCaption, 
                ISNULL(MenuItems.IsComboMenu, 0) AS IsComboMenu,
                ISNULL(MenuItems.IsTopMenu, 0) AS IsTopMenu, 
                MenuItems.ButtonColor, 
                ISNULL(MenuItems.Barcode, '') AS Barcode,
                ISNULL(MenuItems.Barcode2, '') AS Barcode2, 
                MenuItems.ItemDelCharge, 
                MenuItems.ItemDelComp, 
                MenuItems.DineInPrice, 
                MenuItems.BarTabPrice, 
                MenuItems.TakeOutPrice, 
                MenuItems.DriveThruPrice, 
                MenuItems.DeliveryPrice, 
                MenuItems.OrderByWeight, 
                MenuItems.PrintPizzaLabel, 
                MenuItems.KitchenSortNumber, 
                MenuItems.ModBuilderTemplateID, 
                MenuItems.MenuItemTypeID, 
                ISNULL(MenuItems.AccountingCode, '') AS AccountingCode, 
                ISNULL(MenuItems.PrintOnLabel, 0) AS PrintOnLabel, 
                MenuItems.UsedPrinterID1, 
                MenuItems.UsedPrinterID2, 
                MenuItems.UsedPrinterID3, 
                MenuItems.UsedPrinterID4, 
                MenuItems.UsedPrinterID5, 
                MenuItems.SecurityLevel, 
                MenuItems.DeleteReason, 
                MenuItems.CustomField1, 
                MenuItems.CustomField2, 
                MenuItems.CustomField3, 
                MenuItems.CustomField4, 
                MenuItems.CustomField5, 
                CAST(MenuItems.EditKey AS NVARCHAR(100)) AS EditKey, 
                CAST(MenuItems.SyncKey AS NVARCHAR(100)) AS SyncKey, 
                MenuItems.BranchID, 
                MenuItems.AddUserID, 
                MenuItems.AddDateTime, 
                MenuItems.EditUserID, 
                ISNULL(MenuItems.UseKds1, 0) AS UseKds1, 
                ISNULL(MenuItems.UseKds2, 0) AS UseKds2, 
                ISNULL(MenuItems.UseKds3, 0) AS UseKds3, 
                ISNULL(MenuItems.UseKds4, 0) AS UseKds4, 
                ISNULL(MenuItems.UseKds5, 0) AS UseKds5, 
                ISNULL(MenuItems.UseKds6, 0) AS UseKds6, 
                ISNULL(MenuItems.UseKds7, 0) AS UseKds7, 
                ISNULL(MenuItems.UseKds8, 0) AS UseKds8, 
                ISNULL(MenuItems.UseKds9, 0) AS UseKds9, 
                ISNULL(MenuItems.UseKds10, 0) AS UseKds10, 
                MenuItems.CountDownDate, 
                ISNULL(MenuItems.CountDownValue, 0.0) AS CountDownValue, 
                ISNULL(MenuItems.CountDownActualResult, 0.0) AS CountDownActualResult, 
                MenuItems.EditDateTime, 
                MenuCategories.MenuCategoryText, 
                MenuSubCategories.MenuSubCategoryText, 
                TaxGroups.GroupName AS TaxGroupText, 
                MenuModifierGroups.MenuModifierGroupText AS MenuModifierText, 
                MenuItems.DisplayIndex AS MenuDisplayIndex, 
                CAST(MenuItems.MenuItemKey AS NVARCHAR(100)) AS MenuItemKey, 
                CAST(MenuItems.MainMenuItemKey AS NVARCHAR(100)) AS MainMenuItemKey, 
                CAST(MenuItems.MenuItemGlobalKey AS NVARCHAR(100)) AS MenuItemGlobalKey, 
                CAST(MenuItems.MenuCategoryKey AS NVARCHAR(100)) AS MenuCategoryKey, 
                CAST(MenuItems.MenuGroupKey AS NVARCHAR(100)) AS MenuGroupKey, 
                MenuItems.TaxGroupID, 
                CAST(MenuItems.TaxGroupKey AS NVARCHAR(100)) AS TaxGroupKey, 
                CAST(MenuItems.MenuModifierKey AS NVARCHAR(100)) AS MenuModifierKey, 
                MenuItems.MenuModifierForcedID, 
                CAST(MenuItems.MenuModifierForcedKey AS NVARCHAR(100)) AS MenuModifierForcedKey, 
                MenuForcedModifierGroups.MenuModifierGroupText AS MenuForcedModifierText, 
                efr_Branchs.BranchName 
            FROM MenuItems 
            LEFT OUTER JOIN MenuModifierGroups AS MenuForcedModifierGroups 
                ON MenuItems.MenuModifierForcedKey = MenuForcedModifierGroups.MenuModifierGroupKey 
            LEFT OUTER JOIN MenuModifierGroups 
                ON MenuItems.MenuModifierKey = MenuModifierGroups.MenuModifierGroupKey 
            LEFT OUTER JOIN MenuSubCategories 
                ON MenuItems.MenuGroupKey = MenuSubCategories.MenuSubCategoryKey 
            LEFT OUTER JOIN MenuCategories 
                ON MenuItems.MenuCategoryKey = MenuCategories.MenuCategoryKey 
            LEFT OUTER JOIN TaxGroups 
                ON MenuItems.TaxGroupID = TaxGroups.TaxGroupID 
            LEFT OUTER JOIN efr_Branchs 
                ON MenuItems.BranchID = efr_Branchs.BranchID 
            WHERE MenuItems.MenuItemKey = @MenuItemKey";
        return await connection.QueryFirstOrDefaultAsync<MenuItem>(sql, new { MenuItemKey = menuItemKey });
    }

    public async Task<int> UpdateMenuItemAsync(MenuItem item)
    {
        using var connection = _db.CreateConnection();
        const string sql = @"
            UPDATE MenuItems SET
                MenuItemText = @MenuItemText,
                MenuCategoryID = @MenuCategoryID,
                MenuGroupID = @MenuGroupID,
                DefaultUnitPrice = @DefaultUnitPrice,
                DineInPrice = @DineInPrice,
                BarTabPrice = @BarTabPrice,
                TakeOutPrice = @TakeOutPrice,
                DeliveryPrice = @DeliveryPrice,
                ItemDelCharge = @ItemDelCharge,
                ItemDelComp = @ItemDelComp,
                Barcode = @Barcode,
                Barcode2 = @Barcode2,
                SecLangMenuItemText = @SecLangMenuItemText,
                MenuItemActive = @MenuItemActive,
                MenuItemDiscountable = @MenuItemDiscountable,
                IsComboMenu = @IsComboMenu,
                IsTopMenu = @IsTopMenu,
                OrderByWeight = @OrderByWeight,
                PrintOnLabel = @PrintOnLabel,
                AccountingCode = @AccountingCode,
                SecurityLevel = @SecurityLevel,
                UsedPrinterID1 = @UsedPrinterID1,
                UsedPrinterID2 = @UsedPrinterID2,
                UsedPrinterID3 = @UsedPrinterID3,
                UseKds1 = @UseKds1,
                UseKds2 = @UseKds2,
                UseKds3 = @UseKds3,
                UseKds4 = @UseKds4,
                UseKds5 = @UseKds5,
                UseKds6 = @UseKds6,
                UseKds7 = @UseKds7,
                UseKds8 = @UseKds8,
                PictureName = @PictureName,
                ButtonColor = @ButtonColor,
                TaxGroupID = @TaxGroupID,
                MenuModifierKey = @MenuModifierKey,
                MenuModifierForcedKey = @MenuModifierForcedKey,
                EditDateTime = GETDATE()
            WHERE AutoID = @AutoID OR (MenuItemKey = @MenuItemKey AND @MenuItemKey IS NOT NULL);";
        return await connection.ExecuteAsync(sql, item);
    }

    public async Task<int> InsertMenuItemAsync(MenuItem item, int menuGroupId, int displayIndex)
    {
        using var connection = _db.CreateConnection();
        const string sql = @"
            DECLARE @NewAutoID INT;
            DECLARE @NewMenuItemID INT;
            SELECT @NewMenuItemID = ISNULL(MAX(MenuItemID), 0) + 1 FROM MenuItems;

            DECLARE @NewMenuItemKey UNIQUEIDENTIFIER = NEWID();

            INSERT INTO MenuItems (
                MenuItemID,
                MainMenuItemID,
                RevenueCenterTypeID,
                MenuItemText,
                MenuCategoryID,
                MenuGroupID,
                DisplayIndex,
                DefaultUnitPrice,
                MenuItemCost,
                MenuItemDescription,
                MenuItemNotification,
                MenuItemActive,
                MenuItemInStock,
                MenuItemTaxable,
                MenuModifierID,
                MenuItemDiscountable,
                SecLangMenuItemText,
                PictureName,
                ShowCaption,
                IsComboMenu,
                IsTopMenu,
                ButtonColor,
                Barcode,
                Barcode2,
                ItemDelCharge,
                ItemDelComp,
                DineInPrice,
                BarTabPrice,
                TakeOutPrice,
                DriveThruPrice,
                DeliveryPrice,
                OrderByWeight,
                PrintPizzaLabel,
                KitchenSortNumber,
                ModBuilderTemplateID,
                MenuItemTypeID,
                AccountingCode,
                PrintOnLabel,
                UsedPrinterID1,
                UsedPrinterID2,
                UsedPrinterID3,
                SecurityLevel,
                EditKey,
                SyncKey,
                BranchID,
                AddUserID,
                AddDateTime,
                UseKds1, UseKds2, UseKds3, UseKds4, UseKds5, UseKds6, UseKds7, UseKds8,
                MenuItemKey,
                MenuCategoryKey,
                MenuGroupKey,
                TaxGroupID,
                MenuModifierKey,
                MenuModifierForcedKey
            )
            VALUES (
                @NewMenuItemID,
                @MainMenuItemID,
                @RevenueCenterTypeID,
                @MenuItemText,
                @MenuCategoryID,
                @MenuGroupID,
                @DisplayIndex,
                @DefaultUnitPrice,
                @MenuItemCost,
                @MenuItemDescription,
                @MenuItemNotification,
                ISNULL(@MenuItemActive, 1),
                ISNULL(@MenuItemInStock, 1),
                ISNULL(@MenuItemTaxable, 1),
                @MenuModifierID,
                ISNULL(@MenuItemDiscountable, 1),
                @SecLangMenuItemText,
                @PictureName,
                ISNULL(@ShowCaption, 1),
                ISNULL(@IsComboMenu, 0),
                ISNULL(@IsTopMenu, 0),
                @ButtonColor,
                @Barcode,
                @Barcode2,
                @ItemDelCharge,
                @ItemDelComp,
                @DineInPrice,
                @BarTabPrice,
                @TakeOutPrice,
                @DriveThruPrice,
                @DeliveryPrice,
                ISNULL(@OrderByWeight, 0),
                ISNULL(@PrintPizzaLabel, 0),
                @KitchenSortNumber,
                @ModBuilderTemplateID,
                @MenuItemTypeID,
                @AccountingCode,
                ISNULL(@PrintOnLabel, 0),
                @UsedPrinterID1,
                @UsedPrinterID2,
                @UsedPrinterID3,
                ISNULL(@SecurityLevel, 1),
                NEWID(),
                NEWID(),
                @BranchID,
                1,
                GETDATE(),
                ISNULL(@UseKds1, 0), ISNULL(@UseKds2, 0), ISNULL(@UseKds3, 0), ISNULL(@UseKds4, 0), ISNULL(@UseKds5, 0), ISNULL(@UseKds6, 0), ISNULL(@UseKds7, 0), ISNULL(@UseKds8, 0),
                @NewMenuItemKey,
                @MenuCategoryKey,
                @MenuGroupKey,
                @TaxGroupID,
                @MenuModifierKey,
                @MenuModifierForcedKey
            );

            SET @NewAutoID = SCOPE_IDENTITY();

            IF NOT EXISTS (SELECT 1 FROM MenuItemLayout WHERE MenuGroupID = @MenuGroupID AND DisplayIndex = @DisplayIndex)
            BEGIN
                INSERT INTO MenuItemLayout (MenuGroupID, MenuGroupKey, MenuItemID, MenuItemKey, DisplayIndex)
                VALUES (@MenuGroupID, @MenuGroupKey, @NewMenuItemID, @NewMenuItemKey, @DisplayIndex);
            END
            ELSE
            BEGIN
                UPDATE MenuItemLayout 
                SET MenuItemID = @NewMenuItemID,
                    MenuItemKey = @NewMenuItemKey
                WHERE MenuGroupID = @MenuGroupID AND DisplayIndex = @DisplayIndex;
            END

            SELECT @NewAutoID;
        ";
        return await connection.ExecuteScalarAsync<int>(sql, new
        {
            item.MainMenuItemID,
            item.RevenueCenterTypeID,
            item.MenuItemText,
            item.MenuCategoryID,
            MenuGroupID = menuGroupId,
            DisplayIndex = displayIndex,
            item.DefaultUnitPrice,
            item.MenuItemCost,
            item.MenuItemDescription,
            item.MenuItemNotification,
            item.MenuItemActive,
            item.MenuItemInStock,
            item.MenuItemTaxable,
            item.MenuModifierID,
            item.MenuItemDiscountable,
            item.SecLangMenuItemText,
            item.PictureName,
            item.ShowCaption,
            item.IsComboMenu,
            item.IsTopMenu,
            item.ButtonColor,
            item.Barcode,
            item.Barcode2,
            item.ItemDelCharge,
            item.ItemDelComp,
            item.DineInPrice,
            item.BarTabPrice,
            item.TakeOutPrice,
            item.DriveThruPrice,
            item.DeliveryPrice,
            item.OrderByWeight,
            item.PrintPizzaLabel,
            item.KitchenSortNumber,
            item.ModBuilderTemplateID,
            item.MenuItemTypeID,
            item.AccountingCode,
            item.PrintOnLabel,
            item.UsedPrinterID1,
            item.UsedPrinterID2,
            item.UsedPrinterID3,
            item.SecurityLevel,
            item.BranchID,
            item.UseKds1,
            item.UseKds2,
            item.UseKds3,
            item.UseKds4,
            item.UseKds5,
            item.UseKds6,
            item.UseKds7,
            item.UseKds8,
            item.MenuCategoryKey,
            item.MenuGroupKey,
            item.TaxGroupID,
            item.MenuModifierKey,
            item.MenuModifierForcedKey
        });
    }

    public async Task<IEnumerable<MenuModifierGroup>> GetModifierGroupsAsync()
    {
        using var connection = _db.CreateConnection();
        const string sql = @"
            SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
            SELECT 
                MenuModifierGroupID, 
                MenuModifierGroupText, 
                DisplayIndex, 
                MenuModifierGroupActive, 
                SecLangMenuModifierGroupText, 
                PictureName, 
                ButtonColor, 
                RevenueCenterTypeID, 
                DeleteReason, 
                CustomField1, 
                CustomField2, 
                CustomField3, 
                CustomField4, 
                CustomField5, 
                CAST(EditKey AS NVARCHAR(100)) AS EditKey, 
                CAST(SyncKey AS NVARCHAR(100)) AS SyncKey, 
                BranchID, 
                AddUserID, 
                AddDateTime, 
                EditUserID, 
                EditDateTime, 
                CAST(MenuModifierGroupKey AS NVARCHAR(100)) AS MenuModifierGroupKey
            FROM MenuModifierGroups
            WHERE (ISNULL(MenuModifierGroupActive, 1) = 1)
            ORDER BY DisplayIndex";
        return await connection.QueryAsync<MenuModifierGroup>(sql);
    }

    public async Task<IEnumerable<MenuModifier>> GetModifiersByGroupAsync(string modifierGroupKey)
    {
        using var connection = _db.CreateConnection();
        const string sql = @"
            SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
            SELECT m.AutoID, m.MenuModifierID, m.MenuModifierText, m.AdditionalCost, CAST(m.MenuModifierKey AS NVARCHAR(100)) AS MenuModifierKey, m.PictureName, m.BranchID 
            FROM MenuModifiers m
            INNER JOIN MenuModifierLayout l ON (m.MenuModifierKey = l.MenuModifierKey OR m.MenuModifierID = l.MenuModifierID)
            WHERE CAST(l.MenuModifierGroupKey AS NVARCHAR(100)) = @ModifierGroupKey AND ISNULL(m.MenuModifierActive, 1) = 1
            ORDER BY l.DisplayIndex";
        return await connection.QueryAsync<MenuModifier>(sql, new { ModifierGroupKey = modifierGroupKey });
    }
}

