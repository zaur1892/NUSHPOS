SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
SELECT 
    OrderHeaders.AutoID, 
    ISNULL(OrderHeaders.AutoID, 0) AS OrderID, 
    OrderHeaders.ReceiptNo,
    OrderHeaders.MainOrderKey, 
    OrderHeaders.MainOrderID, 
    OrderHeaders.OrderTypeSourceID, 
    ISNULL(OrderHeaders.OrderTypeSourceExternalNo, '') AS OrderTypeSourceExternalNo, 
    OrderHeaders.OrderKey, 
    OrderHeaders.OrderDateTime, 
    OrderHeaders.EmployeeID, 
    OrderHeaders.StationID, 
    OrderHeaders.RevenueCenterTypeID, 
    OrderHeaders.OrderType, 
    OrderHeaders.DineInTableID, 
    OrderHeaders.CustomerID, 
    OrderHeaders.DeliveryCharge, 
    OrderHeaders.DeliveryComp, 
    OrderHeaders.DeliveryZoneID, 
    OrderHeaders.DriverEmployeeID, 
    OrderHeaders.DriverDepartureTime, 
    OrderHeaders.DriverArrivalTime, 
    OrderHeaders.OnHoldUntilTime, 
    OrderHeaders.SalesTaxRate, 
    OrderHeaders.DiscountID, 
    OrderHeaders.DiscountLineAmount, 
    OrderHeaders.DiscountOrderAmount, 
    OrderHeaders.DiscountCashAmount, 
    OrderHeaders.DiscountTotalAmount, 
    OrderHeaders.OrderStatus, 
    OrderHeaders.BonusAmountUsed, 
    OrderHeaders.BonusAmountEarned, 
    OrderHeaders.BonusID, 
    OrderHeaders.BonusCustomerID, 
    (ISNULL(OrderHeaders.AmountDue, 0.0) + ISNULL(OrderHeaders.CashGratuity, 0.0)) AS GrandTotal,
    OrderHeaders.AmountDue, 
    OrderHeaders.PackagerAlreadyPrinted, 
    OrderHeaders.GuestCheckPrinted, 
    OrderHeaders.GuestCheckPrintCount, 
    OrderHeaders.AdditionPrinted, 
    ISNULL(OrderHeaders.AdditionPrintedLineCount, 0) AS AdditionPrintedLineCount, 
    OrderHeaders.SurchargeID, 
    OrderHeaders.SurchargeLineAmount, 
    OrderHeaders.SurchargeOrderAmount, 
    OrderHeaders.SurchargeCashAmount, 
    OrderHeaders.SurchargeTotalAmount, 
    OrderHeaders.ComplimentaryAmount, 
    OrderHeaders.SubTotal, 
    OrderHeaders.OrderCost, 
    OrderHeaders.GratuityPercent, 
    OrderHeaders.CashGratuity, 
    OrderHeaders.SalesTaxAmount,
    ISNULL(OrderHeaders.IsProduced, 0) AS IsProduced, 
    OrderHeaders.DriveThruComplete, 
    OrderHeaders.BarTabName, 
    OrderHeaders.TableReady, 
    OrderHeaders.GuestNumber, 
    OrderHeaders.SpecificCustomerName, 
    OrderHeaders.OrderPhone, 
    OrderHeaders.InvoicePrinted, 
    OrderHeaders.FiscalPrinted,
    OrderHeaders.OrderNotes, 
    OrderHeaders.OrderExternalNotes, 
    OrderHeaders.LineDeleted, 
    OrderHeaders.DeleteReason, 
    OrderHeaders.CustomField1, 
    OrderHeaders.CustomField2, 
    OrderHeaders.CustomField3, 
    OrderHeaders.CustomField4, 
    OrderHeaders.CustomField5,
    '' AS GratuityText, 
    OrderHeaders.CustomField6, 
    OrderHeaders.CustomField7, 
    OrderHeaders.CustomField8, 
    OrderHeaders.CustomField9, 
    OrderHeaders.CustomField10, 
    OrderHeaders.EditKey, 
    OrderHeaders.SyncKey, 
    OrderHeaders.BranchID, 
    OrderHeaders.LockData, 
    OrderHeaders.LockStationID, 
    OrderHeaders.AddUserID, 
    OrderHeaders.AddDateTime, 
    OrderHeaders.EditUserID, 
    OrderHeaders.EditDateTime, 
    ISNULL(OrderHeaders.EmployeeName, '') AS EmployeeName, 
    DineInTables.DineInTableText, 
    ISNULL(OrderHeaders.CustomerName, '') AS CustomerName, 
    de.FirstName AS DriverEmployeeName,
    ISNULL(OrderHeaders.AddUserName, '') AS AddEmployeeName, 
    ISNULL(OrderHeaders.EditUserName, '') AS EditEmployeeName,
    '' AS OrderInfo,
    (CASE dbo.OrderHeaders.OrderType 
        WHEN 1 THEN 'MASA' 
        WHEN 2 THEN 'BAR SATIŞI' 
        WHEN 3 THEN 'AL GÖTÜR' 
        WHEN 4 THEN 'TEZGAH SATIŞI' 
        WHEN 5 THEN 'PAKET SATIŞI' 
        WHEN 66 THEN 'İADE' 
        ELSE '-' 
     END) AS OrderTypeName,
    ISNULL(OrderHeaders.DiscountAmountValue, 0.0) AS DiscountAmountValue,
    ISNULL(OrderHeaders.DiscountBasisValue, 0) AS DiscountBasisValue, 
    OrderHeaders.EmployeeKey, 
    OrderHeaders.CustomerKey, 
    OrderHeaders.DiscountKey,
    '' AS AmountText, 
    ISNULL(Discounts.DiscountText, '') AS DiscountText, 
    (CASE OrderHeaders.OrderStatus 
        WHEN 1 THEN 'AÇIK' 
        WHEN 2 THEN 'KAPALI' 
        WHEN 3 THEN 'İPTAL' 
        ELSE '-' 
     END) AS OrderStatusName,
    ISNULL(br.BranchName, '') AS BranchName,
    (ISNULL(OrderHeaders.AmountDue, 0.0) + ISNULL(OrderHeaders.DiscountTotalAmount, 0.0)) AS TotalPrice, 
    ISNULL(OrderHeaders.ExternalOrderStatus, 0) AS ExternalOrderStatus, 
    ISNULL(Discounts.DiscountAmount, 0) AS DiscountPercent,
    ISNULL(OrderHeaders.DineInTableName, '') AS DineInTableName,
    ISNULL(DineInTableGroups.TableGroupText, '') AS TableGroupText,
    ISNULL(OrderHeaders.AddUserName, '') AS AddUserName,
    ISNULL(OrderHeaders.EditUserName, '') AS EditUserName,
    ISNULL(OrderHeaders.FiscalKey, '') AS FiscalKey, 
    ISNULL(OrderHeaders.InvoiceDetail, '') AS InvoiceDetail, 
    ISNULL(OrderHeaders.FiscalStatus, 0) AS FiscalStatus, 
    ISNULL(OrderHeaders.TsmStatus, 0) AS TsmStatus,
    ISNULL(OrderHeaders.RetailData, '') AS RetailData,
    ISNULL(OrderHeaders.DiscountUserName, '') AS DiscountUserName, 
    OrderHeaders.PaperNumber, 
    OrderHeaders.ReturnType, 
    OrderHeaders.ReturnOrderNo, 
    OrderHeaders.ReturnCustomerName, 
    OrderHeaders.ReturnCustomerAddress, 
    OrderHeaders.ReturnTaxNumber, 
    OrderHeaders.ReturnTaxOffice, 
    OrderHeaders.ReturnSerialNo, 
    OrderHeaders.ReturnReason, 
    OrderHeaders.ReturnReasonCode, 
    ISNULL(OrderHeaders.UsdAmount, 0) AS UsdAmount, 
    ISNULL(OrderHeaders.EurAmount, 0) AS EurAmount, 
    ISNULL(OrderHeaders.GbpAmount, 0) AS GbpAmount, 
    ISNULL(OrderHeaders.OrderCounter, 0) AS OrderCounter   
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
WHERE OrderHeaders.[OrderKey] = '6B9A7C36-0B57-43AD-B696-A2573CC9D8B5';
```[cite: 5]

---

**2. Sifariş Sətirlərinin / Məhsullarının (OrderTransactions) Gətirilməsi**
```sql
SELECT DISTINCT 
    OrderTransactions.AutoID, 
    OrderTransactions.TransactionDateTime, 
    OrderTransactions.TransactionID, 
    OrderTransactions.TransactionKey, 
    OrderTransactions.OrderKey, 
    OrderTransactions.OrderID, 
    OrderTransactions.OrderDateTime, 
    OrderTransactions.StationID, 
    OrderTransactions.EmployeeID, 
    OrderTransactions.RevenueCenterTypeID, 
    OrderTransactions.MenuItemID, 
    OrderTransactions.MenuItemText,
    '' AS DisplayText, 
    OrderTransactions.MenuItemGroupText, 
    OrderTransactions.MenumItemCategoryText, 
    OrderTransactions.MenuItemUnitPrice, 
    OrderTransactions.MenuItemCost, 
    OrderTransactions.Quantity, 
    OrderTransactions.ExtendedPrice, 
    OrderTransactions.DiscountID, 
    OrderTransactions.DiscountLineAmount, 
    OrderTransactions.DiscountCashAmount, 
    OrderTransactions.DiscountTotalAmount, 
    OrderTransactions.TransactionStatus, 
    OrderTransactions.NotificationStatus, 
    OrderTransactions.AdditionLinePrinted, 
    ISNULL(OrderTransactions.TaxPercent, 0) AS TaxPercent, 
    OrderTransactions.Mod1ID, OrderTransactions.Mod1Cost, 
    OrderTransactions.Mod2ID, OrderTransactions.Mod2Cost, 
    OrderTransactions.Mod3ID, OrderTransactions.Mod3Cost, 
    OrderTransactions.Mod4ID, OrderTransactions.Mod4Cost, 
    OrderTransactions.Mod5ID, OrderTransactions.Mod5Cost, 
    OrderTransactions.Mod6ID, OrderTransactions.Mod6Cost, 
    OrderTransactions.Mod7ID, OrderTransactions.Mod7Cost, 
    OrderTransactions.Mod8ID, OrderTransactions.Mod8Cost, 
    OrderTransactions.Mod9ID, OrderTransactions.Mod9Cost, 
    OrderTransactions.Mod10ID, OrderTransactions.Mod10Cost, 
    OrderTransactions.Mod11ID, OrderTransactions.Mod11Cost, 
    OrderTransactions.Mod12ID, OrderTransactions.Mod12Cost, 
    OrderTransactions.Mod13ID, OrderTransactions.Mod13Cost, 
    OrderTransactions.Mod14ID, OrderTransactions.Mod14Cost, 
    OrderTransactions.Mod15ID, OrderTransactions.Mod15Cost, 
    OrderTransactions.Mod16ID, OrderTransactions.Mod16Cost, 
    OrderTransactions.Mod17ID, OrderTransactions.Mod17Cost, 
    OrderTransactions.Mod18ID, OrderTransactions.Mod18Cost, 
    OrderTransactions.Mod19ID, OrderTransactions.Mod19Cost, 
    OrderTransactions.Mod20ID, OrderTransactions.Mod20Cost, 
    OrderTransactions.SeatNumber, 
    OrderTransactions.OnHoldUntilTime, 
    OrderTransactions.Notes, 
    ISNULL(OrderTransactions.SaleTaxAmount, 0) AS SaleTaxAmount, 
    ISNULL(OrderTransactions.TaxPercentReduction, 0) AS TaxPercentReduction, 
    OrderTransactions.UsedPrinterID1, OrderTransactions.UsedPrinterID2, OrderTransactions.UsedPrinterID3, OrderTransactions.UsedPrinterID4, OrderTransactions.UsedPrinterID5, 
    OrderTransactions.UsedPrinterComplated1, OrderTransactions.UsedPrinterComplated2, OrderTransactions.UsedPrinterComplated3, OrderTransactions.UsedPrinterComplated4, OrderTransactions.UsedPrinterComplated5, 
    OrderTransactions.LineDeleted, 
    OrderTransactions.DeleteReason, 
    ISNULL(OrderTransactions.CustomField1, '') AS CustomField1, 
    ISNULL(OrderTransactions.CustomField2, '') AS CustomField2, 
    OrderTransactions.CustomField3, 
    OrderTransactions.CustomField4, 
    OrderTransactions.CustomField5, 
    OrderTransactions.EditKey, 
    OrderTransactions.SyncKey, 
    OrderTransactions.BranchID, 
    ISNULL(OrderTransactions.SplitDetail, '') AS SplitDetail, 
    OrderTransactions.AddUserID, 
    OrderTransactions.AddDateTime, 
    OrderTransactions.EditUserID, 
    OrderTransactions.EditDateTime, 
    ISNULL(OrderTransactions.EmployeeName, '') AS EmployeeName, 
    ISNULL(OrderTransactions.AddUserName, '') AS AddEmployeeName, 
    ISNULL(OrderTransactions.EditUserName, '') AS EditUserName,
    ISNULL(OrderTransactions.EditUserName, '') AS EditEmployeeName,
    ISNULL(Discounts.DiscountText, '') AS DiscountText,
    ISNULL(OrderTransactions.DiscountAmountValue, 0) AS DiscountAmountValue,
    ISNULL(OrderTransactions.DiscountBasisValue, 0) AS DiscountBasisValue, 
    mm1.MenuModifierText AS Mod1IDtext, mm2.MenuModifierText AS Mod2IDtext, mm3.MenuModifierText AS Mod3IDtext, mm4.MenuModifierText AS Mod4IDtext, mm5.MenuModifierText AS Mod5IDtext, 
    mm6.MenuModifierText AS Mod6IDtext, mm7.MenuModifierText AS Mod7IDtext, mm8.MenuModifierText AS Mod8IDtext, mm9.MenuModifierText AS Mod9IDtext, mm10.MenuModifierText AS Mod10IDtext, 
    mm11.MenuModifierText AS Mod11IDtext, mm12.MenuModifierText AS Mod12IDtext, mm13.MenuModifierText AS Mod13IDtext, mm14.MenuModifierText AS Mod14IDtext, mm15.MenuModifierText AS Mod15IDtext, 
    mm16.MenuModifierText AS Mod16IDtext, mm17.MenuModifierText AS Mod17IDtext, mm18.MenuModifierText AS Mod18IDtext, mm19.MenuModifierText AS Mod19IDtext, mm20.MenuModifierText AS Mod20IDtext, 
    lee.FirstName AS LastEmployeeName,
    ISNULL(Discounts.DiscountBasis, 0) AS DiscountBasis,
    ISNULL(Discounts.DiscountAmount, 0) AS DiscountApplyAmount, 
    OrderTransactions.MenuItemKey, 
    OrderTransactions.DiscountKey,
    OrderTransactions.PromotionKey, 
    ISNULL(Promotions.PromotionName, '') AS PromotionName, 
    (ISNULL(OrderTransactions.Quantity, 0) * ISNULL(OrderTransactions.MenuItemUnitPrice, 0)) AS TotalPrice,
    ISNULL(mi.MenuItemDescription, mi.MenuItemText) AS MenuItemText2,
    ISNULL(mi.Barcode, '') AS Barcode,
    ISNULL(mi.Barcode2, '') AS Barcode2,
    ISNULL(OrderTransactions.AddUserName, '') AS AddUserName,
    ISNULL(OrderTransactions.OrderByWeight, 0) AS OrderByWeight,
    ISNULL(OrderTransactions.PromotionName, '') AS PromotionName,
    ISNULL(OrderTransactions.EditUserName, '') AS EditUserName, 
    ISNULL(OrderTransactions.UsedDiscountName, '') AS UsedDiscountName,
    ISNULL(OrderTransactions.DiscountUserName, 0) AS DiscountUserName,
    OrderTransactions.OwnerKey,
    OrderTransactions.PromotionCount,
    OrderTransactions.PromotionAmount, 
    OrderTransactions.AccountingCode, 
    OrderTransactions.MainMenuItemText, 
    OrderTransactions.MainMenuItemTransactionKey, 
    ISNULL(OrderTransactions.LabelPrinted, 0) AS LabelPrinted 
FROM OrderTransactions WITH (NOLOCK) 
LEFT OUTER JOIN EmployeeFiles AS lee ON ISNULL(OrderTransactions.EditUserID, OrderTransactions.AddUserID) = lee.AutoID 
LEFT OUTER JOIN EmployeeFiles AS lo ON OrderTransactions.EmployeeID = lo.AutoID 
LEFT OUTER JOIN MenuItems AS mi ON mi.MenuItemKey = OrderTransactions.MenuItemKey 
LEFT OUTER JOIN Discounts ON OrderTransactions.DiscountKey = Discounts.DiscountKey 
LEFT OUTER JOIN MenuModifiers AS mm1 ON OrderTransactions.Mod1ID = mm1.MenuModifierID 
LEFT OUTER JOIN MenuModifiers AS mm2 ON OrderTransactions.Mod2ID = mm2.MenuModifierID 
LEFT OUTER JOIN MenuModifiers AS mm3 ON OrderTransactions.Mod3ID = mm3.MenuModifierID 
LEFT OUTER JOIN MenuModifiers AS mm4 ON OrderTransactions.Mod4ID = mm4.MenuModifierID 
LEFT OUTER JOIN MenuModifiers AS mm5 ON OrderTransactions.Mod5ID = mm5.MenuModifierID 
LEFT OUTER JOIN MenuModifiers AS mm6 ON OrderTransactions.Mod6ID = mm6.MenuModifierID 
LEFT OUTER JOIN MenuModifiers AS mm7 ON OrderTransactions.Mod7ID = mm7.MenuModifierID 
LEFT OUTER JOIN MenuModifiers AS mm8 ON OrderTransactions.Mod8ID = mm8.MenuModifierID 
LEFT OUTER JOIN MenuModifiers AS mm9 ON OrderTransactions.Mod9ID = mm9.MenuModifierID 
LEFT OUTER JOIN MenuModifiers AS mm10 ON OrderTransactions.Mod10ID = mm10.MenuModifierID 
LEFT OUTER JOIN MenuModifiers AS mm11 ON OrderTransactions.Mod11ID = mm11.MenuModifierID 
LEFT OUTER JOIN MenuModifiers AS mm12 ON OrderTransactions.Mod12ID = mm12.MenuModifierID 
LEFT OUTER JOIN MenuModifiers AS mm13 ON OrderTransactions.Mod13ID = mm13.MenuModifierID 
LEFT OUTER JOIN MenuModifiers AS mm14 ON OrderTransactions.Mod14ID = mm14.MenuModifierID 
LEFT OUTER JOIN MenuModifiers AS mm15 ON OrderTransactions.Mod15ID = mm15.MenuModifierID 
LEFT OUTER JOIN MenuModifiers AS mm16 ON OrderTransactions.Mod16ID = mm16.MenuModifierID 
LEFT OUTER JOIN MenuModifiers AS mm17 ON OrderTransactions.Mod17ID = mm17.MenuModifierID 
LEFT OUTER JOIN MenuModifiers AS mm18 ON OrderTransactions.Mod18ID = mm18.MenuModifierID 
LEFT OUTER JOIN MenuModifiers AS mm19 ON OrderTransactions.Mod19ID = mm19.MenuModifierID 
LEFT OUTER JOIN MenuModifiers AS mm20 ON OrderTransactions.Mod20ID = mm20.MenuModifierID 
LEFT OUTER JOIN Promotions ON OrderTransactions.PromotionKey = Promotions.PromotionKey  
WHERE OrderTransactions.[OrderKey] = '6B9A7C36-0B57-43AD-B696-A2573CC9D8B5' 
ORDER BY OrderTransactions.AutoID;
```[cite: 5]

---

**3. Sifarişin Ödənişlərinin (OrderPayments) Gətirilməsi**
```sql
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
SELECT 
    OrderPayments.AutoID, 
    OrderPayments.OrderPaymentID, 
    OrderPayments.PaymentKey, 
    OrderPayments.OrderID, 
    OrderPayments.OrderKey, 
    OrderPayments.StationID,  
    OrderPayments.CustomerID, 
    OrderPayments.CustomerEmployeeID, 
    OrderPayments.CouponNumber,
    OrderPayments.RegisterSessionID, 
    OrderPayments.RevenueCenterTypeID, 
    OrderPayments.PaymentDateTime, 
    OrderPayments.EmployeeID, 
    OrderPayments.RegisterNo,  
    OrderPayments.PaymentMethodID, 
    OrderPayments.AmountTendered, 
    OrderPayments.AmountPaid, 
    OrderPayments.AmountChange, 
    OrderPayments.ExhangeRate,  
    OrderPayments.RoundingAmount, 
    OrderPayments.IsAccountPayment, 
    OrderPayments.IsAccountSale, 
    OrderPayments.CurrencyID, 
    OrderPayments.PaymentMethodCode, 
    OrderPayments.PaymentNotes, 
    ISNULL(OrderPayments.LineDeleted, 0) AS LineDeleted,  
    OrderPayments.DeleteReason, 
    OrderPayments.CustomField1, 
    OrderPayments.CustomField2, 
    OrderPayments.CustomField3, 
    OrderPayments.CustomField4,  
    OrderPayments.CustomField5, 
    OrderPayments.EditKey, 
    OrderPayments.SyncKey, 
    OrderPayments.BranchID, 
    OrderPayments.AddUserID,  
    OrderPayments.AddDateTime, 
    OrderPayments.EditUserID, 
    OrderPayments.EditDateTime, 
    PaymentMethods.PaymentName AS PaymentMethodName,
    e.FirstName AS EmployeeName,
    ea.FirstName AS AddEmployeeName, 
    ee.FirstName AS EditEmployeeName,
    c.CustomerName, 
    OrderPayments.CustomerKey, 
    OrderPayments.CustomerEmployeeKey, 
    OrderPayments.EmployeeKey,
    OrderPayments.PaymentMethodKey,
    ISNULL(OrderPayments.RetailData, '') AS RetailData, 
    OrderPayments.GlobalBankCode, 
    OrderPayments.GlobalBankName, 
    OrderPayments.AccountingCode, 
    OrderPayments.BankAccountingCode, 
    OrderPayments.IncomeAccountingCode, 
    ISNULL(OrderPayments.PaymentStatus, 0) AS PaymentStatus, 
    ISNULL(OrderPayments.IntegrationApprove, 0) AS IntegrationApprove, 
    OrderPayments.IntegrationReferenceNo, 
    ISNULL(OrderPayments.RefundDetail, '') AS RefundDetail, 
    ISNULL(OrderPayments.TsmUsed, 0) AS TsmUsed 
FROM OrderPayments WITH (NOLOCK)  
LEFT OUTER JOIN PaymentMethods ON OrderPayments.PaymentMethodID = PaymentMethods.PaymentMethodID  
LEFT OUTER JOIN EmployeeFiles AS ee ON OrderPayments.EditUserID = ee.EmployeeID  
LEFT OUTER JOIN EmployeeFiles AS ea ON OrderPayments.AddUserID = ea.EmployeeID  
LEFT OUTER JOIN EmployeeFiles AS e ON OrderPayments.EmployeeID = e.EmployeeID 
LEFT OUTER JOIN CustomerFiles AS c ON OrderPayments.CustomerID = c.CustomerID  
WHERE OrderPayments.OrderKey = '6B9A7C36-0B57-43AD-B696-A2573CC9D8B5';
```[cite: 5]

---

**4. Sifariş Qeydlərinin (OrderNotes) Gətirilməsi**
```sql
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
SELECT 
    [AutoID], 
    [BranchID], 
    [OrderKey], 
    [TypeName], 
    [OrderData], 
    [TransactionKey]  
FROM [OrderNotes]  
WHERE OrderKey = '6B9A7C36-0B57-43AD-B696-A2573CC9D8B5';
```[cite: 5]