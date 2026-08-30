using Dapper;
using NUSHPOS.Models;

namespace NUSHPOS.Services;

public class OrderService
{
    private readonly DatabaseService _db;

    public OrderService(DatabaseService db)
    {
        _db = db;
    }

    public async Task<OrderHeader?> GetOrderByIdAsync(int orderId)
    {
        using var connection = _db.CreateConnection();
        const string sql = "SELECT * FROM OrderHeaders WHERE OrderID = @OrderId";
        return await connection.QueryFirstOrDefaultAsync<OrderHeader>(sql, new { OrderId = orderId });
    }

    public async Task<OrderHeader?> GetOrderByKeyAsync(string orderKey)
    {
        using var connection = _db.CreateConnection();
        const string sql = "SELECT * FROM OrderHeaders WHERE OrderKey = @OrderKey";
        return await connection.QueryFirstOrDefaultAsync<OrderHeader>(sql, new { OrderKey = orderKey });
    }

    public async Task<IEnumerable<OrderHeader>> GetActiveOrdersAsync()
    {
        using var connection = _db.CreateConnection();
        const string sql = "SELECT * FROM OrderHeaders WHERE ISNULL(LineDeleted, 0) = 0 AND (OrderStatus = 1 OR OrderStatus = 0 OR OrderStatus IS NULL) ORDER BY OrderDateTime DESC";
        return await connection.QueryAsync<OrderHeader>(sql);
    }

    public async Task<IEnumerable<OrderHeader>> GetOrdersByTableAsync(int tableId)
    {
        using var connection = _db.CreateConnection();
        const string sql = "SELECT * FROM OrderHeaders WHERE DineInTableID = @TableId AND ISNULL(LineDeleted, 0) = 0 AND (OrderStatus = 1 OR OrderStatus = 0 OR OrderStatus IS NULL) ORDER BY OrderDateTime DESC";
        return await connection.QueryAsync<OrderHeader>(sql, new { TableId = tableId });
    }

    public async Task<IEnumerable<OrderHeader>> GetOrdersByEmployeeAsync(int employeeId)
    {
        using var connection = _db.CreateConnection();
        const string sql = "SELECT * FROM OrderHeaders WHERE EmployeeID = @EmployeeId AND ISNULL(LineDeleted, 0) = 0 ORDER BY OrderDateTime DESC";
        return await connection.QueryAsync<OrderHeader>(sql, new { EmployeeId = employeeId });
    }

    public async Task<int> CreateOrderAsync(OrderHeader order)
    {
        using var connection = _db.CreateConnection();
        const string sql = @"
            INSERT INTO OrderHeaders (
                ReceiptNo, MainOrderKey, MainOrderID, OrderTypeSourceID, OrderTypeSourceExternalNo, OrderKey, OrderDateTime,
                EmployeeID, StationID, RevenueCenterTypeID, OrderType, DineInTableID, CustomerID, DeliveryCharge, DeliveryComp,
                DeliveryZoneID, DriverEmployeeID, SalesTaxRate, DiscountID, DiscountLineAmount, DiscountOrderAmount, DiscountCashAmount,
                DiscountTotalAmount, DiscountAmountValue, DiscountBasisValue, OrderStatus, BonusAmountUsed, BonusAmountEarned,
                BonusID, BonusCustomerID, AmountDue, SubTotal, OrderCost, GratuityPercent, CashGratuity, SalesTaxAmount,
                GuestNumber, SpecificCustomerName, OrderPhone, OrderNotes, LineDeleted, DeleteReason, BranchID, AddUserID,
                AddDateTime, CustomerKey, DiscountKey, EmployeeKey, EmployeeName, DineInTableName, CustomerName, RoundAmount, EditKey, SyncKey
            )
            VALUES (
                @ReceiptNo, @MainOrderKey, @MainOrderID, @OrderTypeSourceID, @OrderTypeSourceExternalNo, @OrderKey, @OrderDateTime,
                @EmployeeID, @StationID, @RevenueCenterTypeID, @OrderType, @DineInTableID, @CustomerID, @DeliveryCharge, @DeliveryComp,
                @DeliveryZoneID, @DriverEmployeeID, @SalesTaxRate, @DiscountID, @DiscountLineAmount, @DiscountOrderAmount, @DiscountCashAmount,
                @DiscountTotalAmount, @DiscountAmountValue, @DiscountBasisValue, @OrderStatus, @BonusAmountUsed, @BonusAmountEarned,
                @BonusID, @BonusCustomerID, @AmountDue, @SubTotal, @OrderCost, @GratuityPercent, @CashGratuity, @SalesTaxAmount,
                @GuestNumber, @SpecificCustomerName, @OrderPhone, @OrderNotes, @LineDeleted, @DeleteReason, @BranchID, @AddUserID,
                @AddDateTime, @CustomerKey, @DiscountKey, @EmployeeKey, @EmployeeName, @DineInTableName, @CustomerName, @RoundAmount, @EditKey, @SyncKey
            );
            SELECT CAST(SCOPE_IDENTITY() AS INT);";
        return await connection.ExecuteScalarAsync<int>(sql, order);
    }

    public async Task UpdateOrderAsync(OrderHeader order)
    {
        using var connection = _db.CreateConnection();
        const string sql = @"
            UPDATE OrderHeaders SET
                ReceiptNo = @ReceiptNo,
                OrderType = @OrderType,
                DineInTableID = @DineInTableID,
                CustomerID = @CustomerID,
                DeliveryCharge = @DeliveryCharge,
                SalesTaxRate = @SalesTaxRate,
                DiscountID = @DiscountID,
                DiscountLineAmount = @DiscountLineAmount,
                DiscountOrderAmount = @DiscountOrderAmount,
                DiscountCashAmount = @DiscountCashAmount,
                DiscountTotalAmount = @DiscountTotalAmount,
                DiscountAmountValue = @DiscountAmountValue,
                DiscountBasisValue = @DiscountBasisValue,
                OrderStatus = @OrderStatus,
                BonusAmountUsed = @BonusAmountUsed,
                BonusAmountEarned = @BonusAmountEarned,
                AmountDue = @AmountDue,
                GuestCheckPrinted = @GuestCheckPrinted,
                GuestCheckPrintCount = @GuestCheckPrintCount,
                AdditionPrinted = @AdditionPrinted,
                SubTotal = @SubTotal,
                OrderCost = @OrderCost,
                GratuityPercent = @GratuityPercent,
                CashGratuity = @CashGratuity,
                SalesTaxAmount = @SalesTaxAmount,
                GuestNumber = @GuestNumber,
                SpecificCustomerName = @SpecificCustomerName,
                OrderPhone = @OrderPhone,
                InvoicePrinted = @InvoicePrinted,
                FiscalPrinted = @FiscalPrinted,
                OrderNotes = @OrderNotes,
                LineDeleted = @LineDeleted,
                DeleteReason = @DeleteReason,
                EditUserID = @EditUserID,
                EditDateTime = @EditDateTime,
                EmployeeName = @EmployeeName,
                DineInTableName = @DineInTableName,
                CustomerName = @CustomerName,
                DiscountKey = @DiscountKey,
                RoundAmount = @RoundAmount,
                EditKey = @EditKey,
                SyncKey = @SyncKey
            WHERE OrderID = @OrderID OR (AutoID = @AutoID AND @AutoID > 0)";
        await connection.ExecuteAsync(sql, order);
    }

    public async Task<IEnumerable<OrderTransaction>> GetOrderTransactionsAsync(int orderId)
    {
        using var connection = _db.CreateConnection();
        const string sql = "SELECT * FROM OrderTransactions WHERE OrderID = @OrderId AND ISNULL(LineDeleted, 0) = 0 ORDER BY AutoID";
        return await connection.QueryAsync<OrderTransaction>(sql, new { OrderId = orderId });
    }

    public async Task<IEnumerable<OrderTransaction>> GetOrderTransactionsByKeyAsync(string orderKey)
    {
        using var connection = _db.CreateConnection();
        const string sql = @"
            SELECT 
                AutoID, TransactionID, CAST(TransactionKey AS NVARCHAR(36)) AS TransactionKey,
                CAST(EditKey AS NVARCHAR(36)) AS EditKey, CAST(SyncKey AS NVARCHAR(36)) AS SyncKey,
                CAST(OrderKey AS NVARCHAR(36)) AS OrderKey, OrderID, OrderDateTime, TransactionDateTime, 
                StationID, EmployeeID, RevenueCenterTypeID, MenuItemID, 
                CAST(MenuItemKey AS NVARCHAR(36)) AS MenuItemKey, MenuItemText, MenuItemGroupText, 
                MenumItemCategoryText, MenuItemUnitPrice, MenuItemCost, Quantity, ExtendedPrice, 
                DiscountID, CAST(DiscountKey AS NVARCHAR(36)) AS DiscountKey, DiscountLineAmount, 
                DiscountCashAmount, DiscountTotalAmount, DiscountAmountValue, DiscountBasisValue, 
                UsedDiscountName, DiscountUserName, TransactionStatus, NotificationStatus, 
                AdditionLinePrinted, TaxPercent, RoundID, Mod1ID, Mod1Cost, Mod2ID, Mod2Cost, 
                Mod3ID, Mod3Cost, Mod4ID, Mod4Cost, Mod5ID, Mod5Cost, SeatNumber, Notes, 
                SaleTaxAmount, LineDeleted, DeleteReason, BranchID, AddUserID, AddDateTime, 
                EmployeeName, RoundAmount, EditUserID, EditDateTime
            FROM OrderTransactions 
            WHERE OrderKey = @OrderKey AND ISNULL(LineDeleted, 0) = 0 
            ORDER BY AutoID";
        return await connection.QueryAsync<OrderTransaction>(sql, new { OrderKey = orderKey });
    }

    public async Task AddOrderTransactionAsync(OrderTransaction transaction)
    {
        using var connection = _db.CreateConnection();
        const string sql = @"
            INSERT INTO OrderTransactions (
                TransactionDateTime, TransactionID, TransactionKey, OrderKey, OrderID, OrderDateTime,
                StationID, EmployeeID, RevenueCenterTypeID, MenuItemID, MenuItemKey, MenuItemText,
                MenuItemGroupText, MenumItemCategoryText, MenuItemUnitPrice, MenuItemCost, Quantity,
                ExtendedPrice, DiscountID, DiscountKey, DiscountLineAmount, DiscountCashAmount,
                DiscountTotalAmount, TransactionStatus, NotificationStatus, AdditionLinePrinted,
                TaxPercent, RoundID, Mod1ID, Mod1Cost, Mod2ID, Mod2Cost, Mod3ID, Mod3Cost, Mod4ID, Mod4Cost,
                Mod5ID, Mod5Cost, SeatNumber, Notes, SaleTaxAmount, LineDeleted, DeleteReason, BranchID,
                AddUserID, AddDateTime, EmployeeName, RoundAmount, EditKey, SyncKey
            )
            VALUES (
                @TransactionDateTime, @TransactionID, @TransactionKey, @OrderKey, @OrderID, @OrderDateTime,
                @StationID, @EmployeeID, @RevenueCenterTypeID, @MenuItemID, @MenuItemKey, @MenuItemText,
                @MenuItemGroupText, @MenumItemCategoryText, @MenuItemUnitPrice, @MenuItemCost, @Quantity,
                @ExtendedPrice, @DiscountID, @DiscountKey, @DiscountLineAmount, @DiscountCashAmount,
                @DiscountTotalAmount, @TransactionStatus, @NotificationStatus, @AdditionLinePrinted,
                @TaxPercent, @RoundID, @Mod1ID, @Mod1Cost, @Mod2ID, @Mod2Cost, @Mod3ID, @Mod3Cost, @Mod4ID, @Mod4Cost,
                @Mod5ID, @Mod5Cost, @SeatNumber, @Notes, @SaleTaxAmount, @LineDeleted, @DeleteReason, @BranchID,
                @AddUserID, @AddDateTime, @EmployeeName, @RoundAmount, @EditKey, @SyncKey
            )";
        await connection.ExecuteAsync(sql, transaction);
    }

    public async Task UpdateOrderTransactionAsync(OrderTransaction transaction)
    {
        using var connection = _db.CreateConnection();
        const string sql = @"
            UPDATE OrderTransactions SET
                Quantity = @Quantity,
                ExtendedPrice = @ExtendedPrice,
                DiscountID = @DiscountID,
                DiscountKey = @DiscountKey,
                DiscountLineAmount = @DiscountLineAmount,
                DiscountTotalAmount = @DiscountTotalAmount,
                DiscountAmountValue = @DiscountAmountValue,
                DiscountBasisValue = @DiscountBasisValue,
                UsedDiscountName = @UsedDiscountName,
                DiscountUserName = @DiscountUserName,
                TransactionStatus = @TransactionStatus,
                TaxPercent = @TaxPercent,
                SaleTaxAmount = @SaleTaxAmount,
                Notes = @Notes,
                SeatNumber = @SeatNumber,
                LineDeleted = @LineDeleted,
                DeleteReason = @DeleteReason,
                EditUserID = @EditUserID,
                EditDateTime = @EditDateTime,
                RoundAmount = @RoundAmount
            WHERE AutoID = @AutoID";
        await connection.ExecuteAsync(sql, transaction);
    }

    public async Task DeleteOrderTransactionAsync(int autoId)
    {
        using var connection = _db.CreateConnection();
        const string sql = "UPDATE OrderTransactions SET LineDeleted = 1 WHERE AutoID = @AutoId";
        await connection.ExecuteAsync(sql, new { AutoId = autoId });
    }

    public async Task FinalizeOrderProcessingAsync(string orderKey)
    {
        using var connection = _db.CreateConnection();
        const string sql = @"
            EXEC dbo.fncUpdateReceiptNo; 
            UPDATE OrderTransactions SET TransactionID = AutoID WHERE (isnull(TransactionID,0)=0 or isnull(TransactionID,0)<>AutoID) and OrderKey=@OrderKey;
            UPDATE OrderTransactions SET OrderID = OrderHeaders.OrderID, OrderDateTime = OrderHeaders.OrderDateTime 
            FROM OrderTransactions INNER JOIN OrderHeaders ON OrderTransactions.OrderKey = OrderHeaders.OrderKey 
            WHERE (isnull(OrderTransactions.OrderID,0)=0 or isnull(OrderTransactions.OrderID,0)<>OrderHeaders.AutoID) and OrderTransactions.OrderKey=@OrderKey;
            
            UPDATE OrderPayments SET OrderPaymentID = AutoID WHERE (isnull(OrderPaymentID,0)=0 or isnull(OrderPaymentID,0)<>AutoID) and OrderKey=@OrderKey;
            UPDATE OrderPayments SET OrderID = OrderHeaders.OrderID 
            FROM OrderPayments INNER JOIN OrderHeaders ON OrderPayments.OrderKey = OrderHeaders.OrderKey 
            WHERE (isnull(OrderPayments.OrderID,0)=0 or isnull(OrderPayments.OrderID,0)<>OrderPayments.OrderID) and OrderPayments.OrderKey=@OrderKey;";
            
        await connection.ExecuteAsync(sql, new { OrderKey = orderKey });
    }

    public async Task<dynamic?> GetFullOrderDetailsByKeyAsync(string orderKey)
    {
        using var connection = _db.CreateConnection();
        const string sql = @"
            SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
            SELECT OrderHeaders.AutoID, isnull(OrderHeaders.AutoID,0) as OrderID, OrderHeaders.ReceiptNo, CAST(OrderHeaders.MainOrderKey AS NVARCHAR(100)) as MainOrderKey, OrderHeaders.MainOrderID, OrderHeaders.OrderTypeSourceID, 
            isnull(OrderHeaders.OrderTypeSourceExternalNo,'') as OrderTypeSourceExternalNo, CAST(OrderHeaders.OrderKey AS NVARCHAR(100)) as OrderKey, OrderHeaders.OrderDateTime, OrderHeaders.EmployeeID, OrderHeaders.StationID, 
            OrderHeaders.RevenueCenterTypeID, OrderHeaders.OrderType, OrderHeaders.DineInTableID, OrderHeaders.CustomerID, OrderHeaders.DeliveryCharge, 
            OrderHeaders.DeliveryComp, OrderHeaders.DeliveryZoneID, OrderHeaders.DriverEmployeeID, OrderHeaders.DriverDepartureTime, OrderHeaders.DriverArrivalTime, 
            OrderHeaders.OnHoldUntilTime, OrderHeaders.SalesTaxRate, OrderHeaders.DiscountID, OrderHeaders.DiscountLineAmount, OrderHeaders.DiscountOrderAmount, 
            OrderHeaders.DiscountCashAmount, OrderHeaders.DiscountTotalAmount, OrderHeaders.OrderStatus, OrderHeaders.BonusAmountUsed, 
            OrderHeaders.BonusAmountEarned, OrderHeaders.BonusID, OrderHeaders.BonusCustomerID, (isnull(OrderHeaders.AmountDue,0.0)+isnull(OrderHeaders.CashGratuity,0.0)) AS GrandTotal ,OrderHeaders.AmountDue, OrderHeaders.PackagerAlreadyPrinted, OrderHeaders.GuestCheckPrinted, 
            OrderHeaders.GuestCheckPrintCount, OrderHeaders.AdditionPrinted, isnull(OrderHeaders.AdditionPrintedLineCount,0) as AdditionPrintedLineCount, OrderHeaders.SurchargeID,OrderHeaders.SurchargeLineAmount, 
            OrderHeaders.SurchargeOrderAmount, OrderHeaders.SurchargeCashAmount, OrderHeaders.SurchargeTotalAmount, OrderHeaders.ComplimentaryAmount, 
            OrderHeaders.SubTotal, OrderHeaders.OrderCost, OrderHeaders.GratuityPercent, OrderHeaders.CashGratuity, OrderHeaders.SalesTaxAmount,isnull(OrderHeaders.IsProduced,0) as IsProduced, 
            OrderHeaders.DriveThruComplete, OrderHeaders.BarTabName, OrderHeaders.TableReady, OrderHeaders.GuestNumber, OrderHeaders.SpecificCustomerName, OrderHeaders.OrderPhone, 
            OrderHeaders.InvoicePrinted, OrderHeaders.FiscalPrinted ,OrderHeaders.OrderNotes, OrderHeaders.OrderExternalNotes, OrderHeaders.LineDeleted, OrderHeaders.DeleteReason, 
            OrderHeaders.CustomField1, OrderHeaders.CustomField2, OrderHeaders.CustomField3, OrderHeaders.CustomField4, OrderHeaders.CustomField5,'' as GratuityText, 
            OrderHeaders.CustomField6, OrderHeaders.CustomField7, OrderHeaders.CustomField8, OrderHeaders.CustomField9, OrderHeaders.CustomField10, 
            CAST(OrderHeaders.EditKey AS NVARCHAR(100)) as EditKey, CAST(OrderHeaders.SyncKey AS NVARCHAR(100)) as SyncKey, OrderHeaders.BranchID, OrderHeaders.LockData, OrderHeaders.LockStationID, OrderHeaders.AddUserID, 
            OrderHeaders.AddDateTime, OrderHeaders.EditUserID, OrderHeaders.EditDateTime, isnull(OrderHeaders.EmployeeName,'') AS EmployeeName, DineInTables.DineInTableText, 
            isnull(OrderHeaders.CustomerName,'') as CustomerName, de.FirstName AS DriverEmployeeName,isnull(OrderHeaders.AddUserName,'') AS AddEmployeeName, isnull(OrderHeaders.EditUserName,'') AS EditEmployeeName,'' as OrderInfo 
            ,(CASE dbo.OrderHeaders.OrderType WHEN 1 THEN 'MASA' WHEN 2 THEN 'BAR SATIŞI' WHEN 3 THEN 'AL GÖTÜR' WHEN 4 THEN 'TEZGAH SATIŞI' WHEN 5 THEN 'PAKET SATIŞI' WHEN 66 THEN 'İADE' ELSE '-' END) AS OrderTypeName 
            ,isnull(OrderHeaders.DiscountAmountValue,0.0) as DiscountAmountValue ,isnull(OrderHeaders.DiscountBasisValue,0) as DiscountBasisValue, 
            CAST(OrderHeaders.EmployeeKey AS NVARCHAR(100)) as EmployeeKey, CAST(OrderHeaders.CustomerKey AS NVARCHAR(100)) as CustomerKey, CAST(OrderHeaders.DiscountKey AS NVARCHAR(100)) as DiscountKey,'' as AmountText, isnull(Discounts.DiscountText,'') AS DiscountText, 
            (CASE OrderHeaders.OrderStatus WHEN 1 THEN 'AÇIK' WHEN 2 THEN 'KAPALI' WHEN 3 THEN 'İPTAL' ELSE '-' END) AS OrderStatusName 
            ,isnull(br.BranchName,'') AS BranchName,(ISNULL(OrderHeaders.AmountDue,0.0)+ISNULL(OrderHeaders.DiscountTotalAmount,0.0)) AS TotalPrice, isnull(OrderHeaders.ExternalOrderStatus,0) as ExternalOrderStatus, 
            isnull(Discounts.DiscountAmount,0) AS DiscountPercent,isnull(OrderHeaders.DineInTableName,'') AS DineInTableName,isnull(DineInTableGroups.TableGroupText,'') AS TableGroupText,isnull(OrderHeaders.AddUserName,'') AS AddUserName,isnull(OrderHeaders.EditUserName,'') AS EditUserName, CAST(isnull(OrderHeaders.FiscalKey,'00000000-0000-0000-0000-000000000000') AS NVARCHAR(100)) AS FiscalKey, 
            isnull(OrderHeaders.InvoiceDetail,'') AS InvoiceDetail, isnull(OrderHeaders.FiscalStatus,0) AS FiscalStatus, isnull(OrderHeaders.TsmStatus,0) AS TsmStatus ,isnull(OrderHeaders.RetailData,'') AS RetailData ,isnull(OrderHeaders.DiscountUserName,'') AS DiscountUserName, OrderHeaders.PaperNumber, OrderHeaders.ReturnType, OrderHeaders.ReturnOrderNo, OrderHeaders.ReturnCustomerName, OrderHeaders.ReturnCustomerAddress, OrderHeaders.ReturnTaxNumber, OrderHeaders.ReturnTaxOffice, OrderHeaders.ReturnSerialNo, OrderHeaders.ReturnReason, OrderHeaders.ReturnReasonCode, ISNULL(OrderHeaders.UsdAmount,0) AS UsdAmount, ISNULL(OrderHeaders.EurAmount,0) AS EurAmount, ISNULL(OrderHeaders.GbpAmount,0) AS GbpAmount, ISNULL(OrderHeaders.OrderCounter,0) AS OrderCounter   
            FROM OrderHeaders WITH (NOLOCK) 
            LEFT OUTER JOIN DineInTables ON OrderHeaders.DineInTableID = DineInTables.DineInTableID 
            LEFT OUTER JOIN DineInTableGroups ON DineInTables.TableGroupID = DineInTableGroups.AutoID 
            LEFT OUTER JOIN EmployeeFiles AS de ON OrderHeaders.DriverEmployeeID = de.AutoID 
            LEFT OUTER JOIN EmployeeFiles AS ee ON OrderHeaders.EditUserID = ee.AutoID 
            LEFT OUTER JOIN EmployeeFiles AS ea ON OrderHeaders.AddUserID = ea.AutoID 
            LEFT OUTER JOIN CustomerFiles ON OrderHeaders.CustomerKey = CustomerFiles.CustomerKey 
            LEFT OUTER JOIN EmployeeFiles AS e ON OrderHeaders.EmployeeID = e.AutoID 
            LEFT OUTER JOIN efr_Branchs AS br ON OrderHeaders.BranchID = br.BranchID 
            LEFT OUTER JOIN Discounts ON OrderHeaders.DiscountKey = Discounts.DiscountKey  
            Where OrderHeaders.OrderKey=@OrderKey";
            
        return await connection.QueryFirstOrDefaultAsync(sql, new { OrderKey = orderKey });
    }

    public async Task<IEnumerable<dynamic>> GetFullOrderTransactionsByKeyAsync(string orderKey)
    {
        using var connection = _db.CreateConnection();
        const string sql = @"
            SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
            SELECT DISTINCT OrderTransactions.AutoID, OrderTransactions.TransactionDateTime, OrderTransactions.TransactionID, CAST(OrderTransactions.TransactionKey AS NVARCHAR(100)) as TransactionKey, 
            CAST(OrderTransactions.OrderKey AS NVARCHAR(100)) as OrderKey, OrderTransactions.OrderID, OrderTransactions.OrderDateTime, OrderTransactions.StationID, OrderTransactions.EmployeeID, 
            OrderTransactions.RevenueCenterTypeID, OrderTransactions.MenuItemID, OrderTransactions.MenuItemText,'' as DisplayText, OrderTransactions.MenuItemGroupText, 
            OrderTransactions.MenumItemCategoryText, OrderTransactions.MenuItemUnitPrice, OrderTransactions.MenuItemCost, OrderTransactions.Quantity, 
            OrderTransactions.ExtendedPrice, OrderTransactions.DiscountID, OrderTransactions.DiscountLineAmount, OrderTransactions.DiscountCashAmount, 
            OrderTransactions.DiscountTotalAmount, OrderTransactions.DiscountAmountValue, OrderTransactions.DiscountBasisValue, OrderTransactions.UsedDiscountName, 
            OrderTransactions.DiscountUserName, OrderTransactions.TransactionStatus, OrderTransactions.NotificationStatus, OrderTransactions.AdditionLinePrinted, 
            isnull(OrderTransactions.TaxPercent,0) as TaxPercent, OrderTransactions.Mod1ID, OrderTransactions.Mod1Cost, OrderTransactions.Mod2ID, OrderTransactions.Mod2Cost, 
            OrderTransactions.Mod3ID, OrderTransactions.Mod3Cost, OrderTransactions.Mod4ID, OrderTransactions.Mod4Cost, OrderTransactions.Mod5ID, 
            OrderTransactions.Mod5Cost, OrderTransactions.Mod6ID, OrderTransactions.Mod6Cost, OrderTransactions.Mod7ID, OrderTransactions.Mod7Cost, 
            OrderTransactions.Mod8ID, OrderTransactions.Mod8Cost, OrderTransactions.Mod9ID, OrderTransactions.Mod9Cost, OrderTransactions.Mod10ID, 
            OrderTransactions.Mod10Cost, OrderTransactions.Mod11ID, OrderTransactions.Mod11Cost, OrderTransactions.Mod12ID, OrderTransactions.Mod12Cost, 
            OrderTransactions.Mod13ID, OrderTransactions.Mod13Cost, OrderTransactions.Mod14ID, OrderTransactions.Mod14Cost, OrderTransactions.Mod15ID, 
            OrderTransactions.Mod15Cost, OrderTransactions.Mod16ID, OrderTransactions.Mod16Cost, OrderTransactions.Mod17ID, OrderTransactions.Mod17Cost, 
            OrderTransactions.Mod18ID, OrderTransactions.Mod18Cost, OrderTransactions.Mod19ID, OrderTransactions.Mod19Cost, OrderTransactions.Mod20ID, 
            OrderTransactions.Mod20Cost, OrderTransactions.SeatNumber, OrderTransactions.OnHoldUntilTime, OrderTransactions.Notes, isnull(OrderTransactions.SaleTaxAmount,0) as SaleTaxAmount, isnull(OrderTransactions.TaxPercentReduction,0) as TaxPercentReduction, 
            OrderTransactions.UsedPrinterID1, OrderTransactions.UsedPrinterID2, OrderTransactions.UsedPrinterID3, OrderTransactions.UsedPrinterID4, 
            OrderTransactions.UsedPrinterID5, OrderTransactions.UsedPrinterComplated1, OrderTransactions.UsedPrinterComplated2, 
            OrderTransactions.UsedPrinterComplated3, OrderTransactions.UsedPrinterComplated4, OrderTransactions.UsedPrinterComplated5, OrderTransactions.LineDeleted, 
            OrderTransactions.DeleteReason, isnull(OrderTransactions.CustomField1,'') as CustomField1, isnull(OrderTransactions.CustomField2,'') as CustomField2, OrderTransactions.CustomField3, 
            OrderTransactions.CustomField4, OrderTransactions.CustomField5, CAST(OrderTransactions.EditKey AS NVARCHAR(100)) as EditKey, CAST(OrderTransactions.SyncKey AS NVARCHAR(100)) as SyncKey, OrderTransactions.BranchID, 
            isnull(OrderTransactions.SplitDetail,'') as SplitDetail, OrderTransactions.AddUserID, OrderTransactions.AddDateTime, OrderTransactions.EditUserID, OrderTransactions.EditDateTime, isnull(OrderTransactions.EmployeeName,'') AS EmployeeName, 
            isnull(OrderTransactions.AddUserName,'') AS AddEmployeeName, isnull(OrderTransactions.EditUserName,'') AS EditUserName,isnull(OrderTransactions.EditUserName,'') AS EditEmployeeName,isnull(Discounts.DiscountText,'') as DiscountText,isnull(OrderTransactions.DiscountAmountValue,0) as DiscountAmountValue ,isnull(OrderTransactions.DiscountBasisValue,0) as DiscountBasisValue , 
            mm1.MenuModifierText AS Mod1IDtext,mm2.MenuModifierText AS Mod2IDtext,mm3.MenuModifierText AS Mod3IDtext,mm4.MenuModifierText AS Mod4IDtext,mm5.MenuModifierText AS Mod5IDtext, 
            mm6.MenuModifierText AS Mod6IDtext,mm7.MenuModifierText AS Mod7IDtext,mm8.MenuModifierText AS Mod8IDtext,mm9.MenuModifierText AS Mod9IDtext,mm10.MenuModifierText AS Mod10IDtext, 
            mm11.MenuModifierText AS Mod11IDtext,mm12.MenuModifierText AS Mod12IDtext,mm13.MenuModifierText AS Mod13IDtext,mm14.MenuModifierText AS Mod14IDtext,mm15.MenuModifierText AS Mod15IDtext, 
            mm16.MenuModifierText AS Mod16IDtext,mm17.MenuModifierText AS Mod17IDtext,mm18.MenuModifierText AS Mod18IDtext,mm19.MenuModifierText AS Mod19IDtext,mm20.MenuModifierText AS Mod20IDtext, 
            lee.FirstName AS LastEmployeeName,isnull(Discounts.DiscountBasis,0) as DiscountBasis,isnull(Discounts.DiscountAmount,0) AS DiscountApplyAmount, 
            CAST(OrderTransactions.MenuItemKey AS NVARCHAR(100)) as MenuItemKey, CAST(OrderTransactions.DiscountKey AS NVARCHAR(100)) as DiscountKey, CAST(OrderTransactions.PromotionKey AS NVARCHAR(100)) as PromotionKey, isnull(Promotions.PromotionName,'') as PromotionName, (isnull(OrderTransactions.Quantity,0)*isnull(OrderTransactions.MenuItemUnitPrice,0)) as TotalPrice,ISNULL(mi.MenuItemDescription,mi.MenuItemText) AS MenuItemText2,isnull(mi.Barcode,'') AS Barcode,isnull(mi.Barcode2,'') AS Barcode2 
            ,isnull(OrderTransactions.AddUserName,'') AS AddUserName,isnull(OrderTransactions.OrderByWeight,0) AS OrderByWeight,isnull(OrderTransactions.PromotionName,'') AS PromotionName,isnull(OrderTransactions.EditUserName,'') AS EditUserName, isnull(OrderTransactions.UsedDiscountName,'') AS UsedDiscountName ,isnull(OrderTransactions.DiscountUserName,0) as DiscountUserName, CAST(OrderTransactions.OwnerKey AS NVARCHAR(100)) as OwnerKey ,OrderTransactions.PromotionCount ,OrderTransactions.PromotionAmount, OrderTransactions.AccountingCode, OrderTransactions.MainMenuItemText, CAST(OrderTransactions.MainMenuItemTransactionKey AS NVARCHAR(100)) as MainMenuItemTransactionKey, ISNULL(OrderTransactions.LabelPrinted, 0) AS LabelPrinted 
            FROM OrderTransactions WITH (NOLOCK) 
            LEFT OUTER JOIN EmployeeFiles AS lee ON isnull(OrderTransactions.EditUserID,OrderTransactions.AddUserID) = lee.AutoID 
            LEFT OUTER JOIN EmployeeFiles as lo ON OrderTransactions.EmployeeID = lo.AutoID 
            LEFT OUTER JOIN GlobalMenuItems AS mi ON mi.MenuItemKey=OrderTransactions.MenuItemKey 
            LEFT OUTER JOIN Discounts ON OrderTransactions.DiscountKey = Discounts.DiscountKey 
            LEFT OUTER JOIN GlobalMenuModifiers AS mm1 ON OrderTransactions.Mod1ID = mm1.MenuModifierID 
            LEFT OUTER JOIN GlobalMenuModifiers AS mm2 ON OrderTransactions.Mod2ID = mm2.MenuModifierID 
            LEFT OUTER JOIN GlobalMenuModifiers AS mm3 ON OrderTransactions.Mod3ID = mm3.MenuModifierID 
            LEFT OUTER JOIN GlobalMenuModifiers AS mm4 ON OrderTransactions.Mod4ID = mm4.MenuModifierID 
            LEFT OUTER JOIN GlobalMenuModifiers AS mm5 ON OrderTransactions.Mod5ID = mm5.MenuModifierID 
            LEFT OUTER JOIN GlobalMenuModifiers AS mm6 ON OrderTransactions.Mod6ID = mm6.MenuModifierID 
            LEFT OUTER JOIN GlobalMenuModifiers AS mm7 ON OrderTransactions.Mod7ID = mm7.MenuModifierID 
            LEFT OUTER JOIN GlobalMenuModifiers AS mm8 ON OrderTransactions.Mod8ID = mm8.MenuModifierID 
            LEFT OUTER JOIN GlobalMenuModifiers AS mm9 ON OrderTransactions.Mod9ID = mm9.MenuModifierID 
            LEFT OUTER JOIN GlobalMenuModifiers AS mm10 ON OrderTransactions.Mod10ID = mm10.MenuModifierID 
            LEFT OUTER JOIN GlobalMenuModifiers AS mm11 ON OrderTransactions.Mod11ID = mm11.MenuModifierID 
            LEFT OUTER JOIN GlobalMenuModifiers AS mm12 ON OrderTransactions.Mod12ID = mm12.MenuModifierID 
            LEFT OUTER JOIN GlobalMenuModifiers AS mm13 ON OrderTransactions.Mod13ID = mm13.MenuModifierID 
            LEFT OUTER JOIN GlobalMenuModifiers AS mm14 ON OrderTransactions.Mod14ID = mm14.MenuModifierID 
            LEFT OUTER JOIN GlobalMenuModifiers AS mm15 ON OrderTransactions.Mod15ID = mm15.MenuModifierID 
            LEFT OUTER JOIN GlobalMenuModifiers AS mm16 ON OrderTransactions.Mod16ID = mm16.MenuModifierID 
            LEFT OUTER JOIN GlobalMenuModifiers AS mm17 ON OrderTransactions.Mod17ID = mm17.MenuModifierID 
            LEFT OUTER JOIN GlobalMenuModifiers AS mm18 ON OrderTransactions.Mod18ID = mm18.MenuModifierID 
            LEFT OUTER JOIN GlobalMenuModifiers AS mm19 ON OrderTransactions.Mod19ID = mm19.MenuModifierID 
            LEFT OUTER JOIN GlobalMenuModifiers AS mm20 ON OrderTransactions.Mod20ID = mm20.MenuModifierID 
            LEFT OUTER JOIN Promotions ON OrderTransactions.PromotionKey = Promotions.PromotionKey  
            Where OrderTransactions.OrderKey=@OrderKey order by OrderTransactions.AutoID";
            
        return await connection.QueryAsync(sql, new { OrderKey = orderKey });
    }

    public async Task<IEnumerable<dynamic>> GetFullOrderPaymentsByKeyAsync(string orderKey)
    {
        using var connection = _db.CreateConnection();
        const string sql = @"
            SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
            SELECT OrderPayments.AutoID, OrderPayments.OrderPaymentID, CAST(OrderPayments.PaymentKey AS NVARCHAR(100)) as PaymentKey, OrderPayments.OrderID, CAST(OrderPayments.OrderKey AS NVARCHAR(100)) as OrderKey, OrderPayments.StationID,  
            OrderPayments.CustomerID, OrderPayments.CustomerEmployeeID, OrderPayments.CouponNumber,OrderPayments.RegisterSessionID, OrderPayments.RevenueCenterTypeID, OrderPayments.PaymentDateTime, OrderPayments.EmployeeID, OrderPayments.RegisterNo,  
            OrderPayments.PaymentMethodID, OrderPayments.AmountTendered, OrderPayments.AmountPaid, OrderPayments.AmountChange, OrderPayments.ExhangeRate,  
            OrderPayments.RoundingAmount, OrderPayments.IsAccountPayment, OrderPayments.IsAccountSale, OrderPayments.CurrencyID, OrderPayments.PaymentMethodCode, OrderPayments.PaymentNotes, isnull(OrderPayments.LineDeleted,0) as LineDeleted,  
            OrderPayments.DeleteReason, OrderPayments.CustomField1, OrderPayments.CustomField2, OrderPayments.CustomField3, OrderPayments.CustomField4,  
            OrderPayments.CustomField5, CAST(OrderPayments.EditKey AS NVARCHAR(100)) as EditKey, CAST(OrderPayments.SyncKey AS NVARCHAR(100)) as SyncKey, OrderPayments.BranchID, OrderPayments.AddUserID,  
            OrderPayments.AddDateTime, OrderPayments.EditUserID, OrderPayments.EditDateTime, PaymentMethods.PaymentName AS PaymentMethodName 
            ,e.FirstName AS EmployeeName,ea.FirstName AS AddEmployeeName, ee.FirstName AS EditEmployeeName,c.CustomerName, 
            CAST(OrderPayments.CustomerKey AS NVARCHAR(100)) as CustomerKey, CAST(OrderPayments.CustomerEmployeeKey AS NVARCHAR(100)) as CustomerEmployeeKey, CAST(OrderPayments.EmployeeKey AS NVARCHAR(100)) as EmployeeKey, CAST(OrderPayments.PaymentMethodKey AS NVARCHAR(100)) as PaymentMethodKey ,isnull(OrderPayments.RetailData,'') AS RetailData, OrderPayments.GlobalBankCode, OrderPayments.GlobalBankName, OrderPayments.AccountingCode, OrderPayments.BankAccountingCode, OrderPayments.IncomeAccountingCode, ISNULL(OrderPayments.PaymentStatus, 0) AS PaymentStatus, ISNULL(OrderPayments.IntegrationApprove, 0) AS IntegrationApprove, OrderPayments.IntegrationReferenceNo, ISNULL(OrderPayments.RefundDetail, '') AS RefundDetail, ISNULL(OrderPayments.TsmUsed, 0) AS TsmUsed 
            FROM OrderPayments WITH (NOLOCK)  
            LEFT OUTER JOIN PaymentMethods ON OrderPayments.PaymentMethodID = PaymentMethods.PaymentMethodID  
            LEFT OUTER JOIN EmployeeFiles AS ee ON OrderPayments.EditUserID = ee.EmployeeID  
            LEFT OUTER JOIN EmployeeFiles AS ea ON OrderPayments.AddUserID = ea.EmployeeID  
            LEFT OUTER JOIN EmployeeFiles AS e ON OrderPayments.EmployeeID = e.EmployeeID 
            LEFT OUTER JOIN CustomerFiles AS c ON OrderPayments.CustomerID = c.CustomerID  
            Where OrderPayments.OrderKey=@OrderKey";
            
        return await connection.QueryAsync(sql, new { OrderKey = orderKey });
    }

    public async Task<IEnumerable<dynamic>> GetOrderNotesByKeyAsync(string orderKey)
    {
        using var connection = _db.CreateConnection();
        const string sql = @"
            SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
            SELECT [AutoID] , [BranchID] , CAST([OrderKey] AS NVARCHAR(100)) as OrderKey , [TypeName] , [OrderData], CAST([TransactionKey] AS NVARCHAR(100)) as TransactionKey  
            FROM [OrderNotes]  
            where OrderKey=@OrderKey";
            
        return await connection.QueryAsync(sql, new { OrderKey = orderKey });
    }
}
