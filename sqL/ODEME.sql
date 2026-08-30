-- 1. Sifariş və Ödəniş Məbləğlərinin / Saylarının Yoxlanması
DECLARE @transactionCount INT;
DECLARE @transactionSum FLOAT(53);
DECLARE @paymentCount INT;
DECLARE @paymentSum FLOAT(53);

SELECT
    @transactionCount = COUNT(t.AutoID),
    @transactionSum = ISNULL(SUM(t.ExtendedPrice), 0)
FROM OrderTransactions t
WHERE t.OrderKey = '676C7755-311A-476B-ADEC-75723D4EEE78'
  AND t.LineDeleted = 0;
	
SELECT
    @paymentCount = COUNT(p.AutoID),
    @paymentSum = ISNULL(SUM(p.AmountPaid), 0)
FROM OrderPayments p
WHERE p.OrderKey = '676C7755-311A-476B-ADEC-75723D4EEE78'
  AND p.LineDeleted = 0;

SELECT REPLACE(REPLACE((CAST(@transactionCount AS VARCHAR) + '_' + CAST(@transactionSum AS VARCHAR) + '_' + CAST(@paymentCount AS VARCHAR) + '_' + CAST(@paymentSum AS VARCHAR)), '.', ''), ',', '');

-- 2. "ÖDEME ALMA" Əməliyyat Loqunun Yazılması (AccessLogs) [cite: 1981]
INSERT INTO [AccessLogs] (
    [BranchID], [LogDate], [StationID], [EmployeeID], [ActionName], 
    [WrongPassword], [AdditionalInfo], [IsSuccess], [OrderKey], 
    [TransactionKey], [AccessLogKey], [EditKey], [SyncKey]
) VALUES (
    422, GETDATE(), 1, 106, N'ÖDEME ALMA', 
    NULL, NULL, 1, '00000000-0000-0000-0000-000000000000', 
    '00000000-0000-0000-0000-000000000000', NEWID(), 'EEE0A178-DDE5-4363-8B45-A66BD33DBC2F', 'EC5DCAC4-78CC-4E78-AAF6-103B4CAB3F21'
);

-- 3. Sifarişin Eyni Anda Başqa Yerdən Dəyişdirilmədiyinin Yoxlanması (EditKey Yoxlaması) [cite: 1990]
SELECT COUNT(r.EditKey) AS adet 
FROM (
    SELECT ISNULL((SELECT TOP 1 EditKey FROM OrderHeaders AS h WHERE h.OrderKey = '676C7755-311A-476B-ADEC-75723D4EEE78'), NEWID()) AS EditKey
) AS r 
WHERE r.EditKey = 'ED80DEF3-898B-4512-A7ED-4513EEEA19A1';

-- 4. Sifarişin Açıq Statusda Olub-Olmamasının Yoxlanması [cite: 2000]
SELECT COUNT(OrderKey) AS countValue 
FROM OrderHeaders WITH (NOLOCK) 
WHERE OrderKey = '676C7755-311A-476B-ADEC-75723D4EEE78' 
  AND OrderStatus <> 1;

-- 5. Sifariş Başlığının Yenilənməsi (OrderHeaders) [cite: 2007, 2026]
UPDATE [OrderHeaders] 
SET [OrderID] = 19,
    [ReceiptNo] = N'3',
    [MainOrderKey] = NULL,
    [MainOrderID] = 0,
    [OrderTypeSourceID] = 1,
    [OrderTypeSourceExternalNo] = NULL,
    [OrderDateTime] = '2026-08-18 09:58:04.310',
    [EmployeeID] = 106,
    [StationID] = 1,
    [RevenueCenterTypeID] = 1,
    [OrderType] = 1,
    [DineInTableID] = 232,
    [CustomerID] = 0,
    [DeliveryCharge] = 0,
    [DeliveryComp] = 0,
    [DeliveryZoneID] = 0,
    [DriverEmployeeID] = 0,
    [DriverDepartureTime] = NULL,
    [DriverArrivalTime] = NULL,
    [OnHoldUntilTime] = NULL,
    [SalesTaxRate] = 0,
    [DiscountID] = 0,
    [DiscountLineAmount] = 0,
    [DiscountOrderAmount] = 0,
    [DiscountCashAmount] = 0,
    [DiscountTotalAmount] = 0,
    [OrderStatus] = 1,
    [BonusAmountUsed] = 0,
    [BonusAmountEarned] = 0,
    [BonusID] = 0,
    [BonusCustomerID] = 0,
    [AmountDue] = 98,
    [PackagerAlreadyPrinted] = 0,
    [GuestCheckPrinted] = 0,
    [GuestCheckPrintCount] = 0,
    [AdditionPrinted] = 0,
    [AdditionPrintedLineCount] = 0,
    [SurchargeID] = 0,
    [SurchargeLineAmount] = 10032718,
    [SurchargeOrderAmount] = 0,
    [SurchargeCashAmount] = 0,
    [SurchargeTotalAmount] = 0,
    [ComplimentaryAmount] = 0,
    [SubTotal] = 98,
    [OrderCost] = 0,
    [GratuityPercent] = 0,
    [CashGratuity] = 0,
    [SalesTaxAmount] = 0,
    [DriveThruComplete] = NULL,
    [BarTabName] = NULL,
    [TableReady] = 1,
    [GuestNumber] = 2,
    [SpecificCustomerName] = NULL,
    [OrderPhone] = NULL,
    [InvoicePrinted] = 0,
    [FiscalPrinted] = NULL,
    [OrderNotes] = NULL,
    [OrderExternalNotes] = NULL,
    [LineDeleted] = 0,
    [DeleteReason] = NULL,
    [CustomField1] = NULL,
    [CustomField2] = NULL,
    [CustomField3] = NULL,
    [CustomField4] = NULL,
    [CustomField5] = NULL,
    [EditKey] = NEWID(),
    [SyncKey] = NEWID(),
    [BranchID] = 422,
    [LockData] = NULL,
    [LockStationID] = NULL,
    [EditUserID] = 106,
    [EditDateTime] = GETDATE(),
    [CustomerKey] = NULL,
    [DiscountKey] = NULL,
    [EmployeeKey] = '4740D379-BF42-4288-BF67-4B46D795F77D',
    [EmployeeName] = N'ROBOTPOS',
    [DineInTableName] = N'T74',
    [CustomerName] = NULL,
    [EditUserName] = N'ROBOTPOS',
    [DiscountAmountValue] = 0,
    [DiscountBasisValue] = 0,
    [AddUserName] = N'ROBOTPOS',
    [AddUserID] = 106,
    [InvoiceDetail] = NULL,
    [RetailData] = NULL,
    [DiscountUserName] = NULL,
    [PosVersion] = N'1.0.0.32718'
WHERE [AutoID] = 19 AND [EditKey] = 'ED80DEF3-898B-4512-A7ED-4513EEEA19A1';

-- 6. Sifariş Sətirlərinin Yenilənməsi (OrderTransactions) [cite: 2034, 2108]
UPDATE [OrderTransactions] 
SET [TransactionDateTime] = '2026-08-18 09:58:04.347',
    [TransactionID] = 111,
    [TransactionKey] = 'FAD2B39D-8D17-49A9-A5F4-CCBB3488C76E',
    [OrderKey] = '676C7755-311A-476B-ADEC-75723D4EEE78',
    [OrderID] = 19,
    [OrderDateTime] = '2026-08-18 09:58:04.310',
    [StationID] = 1,
    [EmployeeID] = 106,
    [RevenueCenterTypeID] = 99,
    [MenuItemID] = 4,
    [MenuItemKey] = 'FDE08677-5B6D-4C9E-9CB0-F1CB2462BFBA',
    [MenuItemText] = N'Dönər Çörəkdə',
    [MenuItemGroupText] = N'DÖNER',
    [MenumItemCategoryText] = N'MUTFAK',
    [MenuItemUnitPrice] = 2,
    [MenuItemCost] = 0,
    [Quantity] = 2,
    [ExtendedPrice] = 4,
    [DiscountID] = 0,
    [DiscountKey] = NULL,
    [DiscountLineAmount] = 0,
    [DiscountCashAmount] = 0,
    [DiscountTotalAmount] = 0,
    [TransactionStatus] = 1,
    [NotificationStatus] = 1,
    [AdditionLinePrinted] = 0,
    [TaxPercent] = 0,
    [RoundID] = NULL,
    [Mod1ID] = 0, [Mod1Cost] = 0, [Mod2ID] = 0, [Mod2Cost] = 0, [Mod3ID] = 0, [Mod3Cost] = 0, [Mod4ID] = 0, [Mod4Cost] = 0, [Mod5ID] = 0, [Mod5Cost] = 0,
    [Mod6ID] = 0, [Mod6Cost] = 0, [Mod7ID] = 0, [Mod7Cost] = 0, [Mod8ID] = 0, [Mod8Cost] = 0, [Mod9ID] = 0, [Mod9Cost] = 0, [Mod10ID] = 0, [Mod10Cost] = 0,
    [Mod11ID] = 0, [Mod11Cost] = 0, [Mod12ID] = 0, [Mod12Cost] = 0, [Mod13ID] = 0, [Mod13Cost] = 0, [Mod14ID] = 0, [Mod14Cost] = 0, [Mod15ID] = 0, [Mod15Cost] = 0,
    [Mod16ID] = 0, [Mod16Cost] = 0, [Mod17ID] = 0, [Mod17Cost] = 0, [Mod18ID] = 0, [Mod18Cost] = 0, [Mod19ID] = 0, [Mod19Cost] = 0, [Mod20ID] = 0, [Mod20Cost] = 0,
    [SeatNumber] = 1,
    [OnHoldUntilTime] = '2026-08-18 09:57:55.067',
    [Notes] = NULL,
    [SaleTaxAmount] = 0,
    [TaxPercentReduction] = 0,
    [LineDeleted] = 0,
    [DeleteReason] = NULL,
    [EditKey] = NEWID(),
    [SyncKey] = NEWID(),
    [BranchID] = 422,
    [EmployeeName] = NULL,
    [SplitDetail] = NULL,
    [ExternalOrderStatus] = 0,
    [PromotionKey] = '00000000-0000-0000-0000-000000000000',
    [UsedPrinterComplated1] = 1,
    [DiscountBasisValue] = 0,
    [DiscountAmountValue] = 0,
    [UsedDiscountName] = NULL,
    [RetailData] = NULL,
    [OrderByWeight] = 0,
    [DiscountUserName] = N'0',
    [PromotionAmount] = NULL,
    [PromotionCount] = NULL,
    [PosVersion] = N'1.0.0.32718'
WHERE [AutoID] = 111 AND [EditKey] = '98422617-2997-4B10-AC21-BC675207E54A';

-- 7. ID və Qəbz Nömrələrinin Sinxronizasiyası
EXEC dbo.fncUpdateReceiptNo;

UPDATE OrderTransactions 
SET TransactionID = AutoID 
WHERE (ISNULL(TransactionID, 0) = 0 OR ISNULL(TransactionID, 0) <> AutoID) 
  AND OrderKey = '676C7755-311A-476B-ADEC-75723D4EEE78';

UPDATE OrderTransactions 
SET OrderID = OrderHeaders.OrderID, 
    OrderDateTime = OrderHeaders.OrderDateTime 
FROM OrderTransactions 
INNER JOIN OrderHeaders ON OrderTransactions.OrderKey = OrderHeaders.OrderKey 
WHERE (ISNULL(OrderTransactions.OrderID, 0) = 0 OR ISNULL(OrderTransactions.OrderID, 0) <> OrderHeaders.AutoID) 
  AND OrderTransactions.OrderKey = '676C7755-311A-476B-ADEC-75723D4EEE78';

UPDATE OrderPayments 
SET OrderPaymentID = AutoID 
WHERE (ISNULL(OrderPaymentID, 0) = 0 OR ISNULL(OrderPaymentID, 0) <> AutoID) 
  AND OrderKey = '676C7755-311A-476B-ADEC-75723D4EEE78';

UPDATE OrderPayments 
SET OrderID = OrderHeaders.OrderID 
FROM OrderPayments 
INNER JOIN OrderHeaders ON OrderPayments.OrderKey = OrderHeaders.OrderKey 
WHERE (ISNULL(OrderPayments.OrderID, 0) = 0 OR ISNULL(OrderPayments.OrderID, 0) <> OrderPayments.OrderID) 
  AND OrderPayments.OrderKey = '676C7755-311A-476B-ADEC-75723D4EEE78';

-- 8. Son Yaddaşa Verilmə Vaxtı (OrderSaveTime)
DELETE FROM Params WHERE ParamName = 'OrderSaveTime';
INSERT INTO Params (ParamName, ParamValue) VALUES ('OrderSaveTime', GETDATE());

-- 9. Əlaqəli Məhsul Məlumatlarının Yenilənməsi
UPDATE t1 
SET MainMenuItemText = t2.MenuItemText,
    MainMenuItemTransactionKey = t2.TransactionKey, 
    SendOk = 0
FROM OrderTransactions t1
LEFT JOIN dbo.OrderTransactions t2 ON t2.OrderKey = t1.OrderKey 
    AND t2.CustomField1 = SUBSTRING(t1.CustomField1, 0, CHARINDEX('-', t1.CustomField1) + 1) 
    AND t2.TransactionKey <> t1.TransactionKey 
WHERE t1.CustomField1 IS NOT NULL 
  AND t1.MainMenuItemText IS NULL 
  AND t2.TransactionKey IS NOT NULL;

-- 10. Valyuta Məbləğlərinin Hesablanması (USD, EUR, GBP)
UPDATE h 
SET UsdAmount = CAST(ROUND(h.AmountDue / ISNULL(m1.ExchangeRate, 1), 2) AS DECIMAL(18, 2)),
    EurAmount = CAST(ROUND(h.AmountDue / ISNULL(m2.ExchangeRate, 1), 2) AS DECIMAL(18, 2)),
    GbpAmount = CAST(ROUND(h.AmountDue / ISNULL(m3.ExchangeRate, 1), 2) AS DECIMAL(18, 2))
FROM OrderHeaders h
LEFT JOIN PaymentMethods m1 ON m1.PaymentName IN ('DOLAR', 'USD')
LEFT JOIN PaymentMethods m2 ON m2.PaymentName IN ('EURO', 'EUR')
LEFT JOIN PaymentMethods m3 ON m3.PaymentName IN ('STERLIN', 'GBP')
WHERE h.OrderKey = '676C7755-311A-476B-ADEC-75723D4EEE78';

-- 11. Sifariş Başlığının Oxunması
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
SELECT 
    OrderHeaders.AutoID, isnull(OrderHeaders.AutoID,0) as OrderID, OrderHeaders.ReceiptNo, OrderHeaders.MainOrderKey, OrderHeaders.MainOrderID, OrderHeaders.OrderTypeSourceID, 
    isnull(OrderHeaders.OrderTypeSourceExternalNo,'') as OrderTypeSourceExternalNo, OrderHeaders.OrderKey, OrderHeaders.OrderDateTime, OrderHeaders.EmployeeID, OrderHeaders.StationID, 
    OrderHeaders.RevenueCenterTypeID, OrderHeaders.OrderType, OrderHeaders.DineInTableID, OrderHeaders.CustomerID, OrderHeaders.DeliveryCharge, 
    OrderHeaders.DeliveryComp, OrderHeaders.DeliveryZoneID, OrderHeaders.DriverEmployeeID, OrderHeaders.DriverDepartureTime, OrderHeaders.DriverArrivalTime, 
    OrderHeaders.OnHoldUntilTime, OrderHeaders.SalesTaxRate, OrderHeaders.DiscountID, OrderHeaders.DiscountLineAmount, OrderHeaders.DiscountOrderAmount, 
    OrderHeaders.DiscountCashAmount, OrderHeaders.DiscountTotalAmount, OrderHeaders.OrderStatus, OrderHeaders.BonusAmountUsed, 
    OrderHeaders.BonusAmountEarned, OrderHeaders.BonusID, OrderHeaders.BonusCustomerID, (isnull(OrderHeaders.AmountDue,0.0)+isnull(OrderHeaders.CashGratuity,0.0)) AS GrandTotal, OrderHeaders.AmountDue, OrderHeaders.PackagerAlreadyPrinted, OrderHeaders.GuestCheckPrinted, 
    OrderHeaders.GuestCheckPrintCount, OrderHeaders.AdditionPrinted, isnull(OrderHeaders.AdditionPrintedLineCount,0) as AdditionPrintedLineCount, OrderHeaders.SurchargeID, OrderHeaders.SurchargeLineAmount, 
    OrderHeaders.SurchargeOrderAmount, OrderHeaders.SurchargeCashAmount, OrderHeaders.SurchargeTotalAmount, OrderHeaders.ComplimentaryAmount, 
    OrderHeaders.SubTotal, OrderHeaders.OrderCost, OrderHeaders.GratuityPercent, OrderHeaders.CashGratuity, OrderHeaders.SalesTaxAmount, isnull(OrderHeaders.IsProduced,0) as IsProduced, 
    OrderHeaders.DriveThruComplete, OrderHeaders.BarTabName, OrderHeaders.TableReady, OrderHeaders.GuestNumber, OrderHeaders.SpecificCustomerName, OrderHeaders.OrderPhone, 
    OrderHeaders.InvoicePrinted, OrderHeaders.FiscalPrinted, OrderHeaders.OrderNotes, OrderHeaders.OrderExternalNotes, OrderHeaders.LineDeleted, OrderHeaders.DeleteReason, 
    OrderHeaders.CustomField1, OrderHeaders.CustomField2, OrderHeaders.CustomField3, OrderHeaders.CustomField4, OrderHeaders.CustomField5, '' as GratuityText, 
    OrderHeaders.CustomField6, OrderHeaders.CustomField7, OrderHeaders.CustomField8, OrderHeaders.CustomField9, OrderHeaders.CustomField10, 
    OrderHeaders.EditKey, OrderHeaders.SyncKey, OrderHeaders.BranchID, OrderHeaders.LockData, OrderHeaders.LockStationID, OrderHeaders.AddUserID, 
    OrderHeaders.AddDateTime, OrderHeaders.EditUserID, OrderHeaders.EditDateTime, isnull(OrderHeaders.EmployeeName,'') AS EmployeeName, DineInTables.DineInTableText, 
    isnull(OrderHeaders.CustomerName,'') as CustomerName, de.FirstName AS DriverEmployeeName, isnull(OrderHeaders.AddUserName,'') AS AddEmployeeName, isnull(OrderHeaders.EditUserName,'') AS EditEmployeeName, '' as OrderInfo, 
    (CASE dbo.OrderHeaders.OrderType WHEN 1 THEN 'MASA' WHEN 2 THEN 'BAR SATIŞI' WHEN 3 THEN 'AL GÖTÜR' WHEN 4 THEN 'TEZGAH SATIŞI' WHEN 5 THEN 'PAKET SATIŞI' WHEN 66 THEN 'İADE' ELSE '-' END) AS OrderTypeName, 
    isnull(OrderHeaders.DiscountAmountValue,0.0) as DiscountAmountValue, isnull(OrderHeaders.DiscountBasisValue,0) as DiscountBasisValue, 
    OrderHeaders.EmployeeKey, OrderHeaders.CustomerKey, OrderHeaders.DiscountKey, '' as AmountText, isnull(Discounts.DiscountText,'') AS DiscountText, 
    (CASE OrderHeaders.OrderStatus WHEN 1 THEN 'AÇIK' WHEN 2 THEN 'KAPALI' WHEN 3 THEN 'İPTAL' ELSE '-' END) AS OrderStatusName, 
    isnull(br.BranchName,'') AS BranchName, (ISNULL(OrderHeaders.AmountDue,0.0)+ISNULL(OrderHeaders.DiscountTotalAmount,0.0)) AS TotalPrice, isnull(OrderHeaders.ExternalOrderStatus,0) as ExternalOrderStatus, 
    isnull(Discounts.DiscountAmount,0) AS DiscountPercent, isnull(OrderHeaders.DineInTableName,'') AS DineInTableName, isnull(DineInTableGroups.TableGroupText,'') AS TableGroupText, isnull(OrderHeaders.AddUserName,'') AS AddUserName, isnull(OrderHeaders.EditUserName,'') AS EditUserName, isnull(OrderHeaders.FiscalKey,'') AS FiscalKey, 
    isnull(OrderHeaders.InvoiceDetail,'') AS InvoiceDetail, isnull(OrderHeaders.FiscalStatus,0) AS FiscalStatus, isnull(OrderHeaders.TsmStatus,0) AS TsmStatus, isnull(OrderHeaders.RetailData,'') AS RetailData, isnull(OrderHeaders.DiscountUserName,'') AS DiscountUserName, OrderHeaders.PaperNumber, OrderHeaders.ReturnType, OrderHeaders.ReturnOrderNo, OrderHeaders.ReturnCustomerName, OrderHeaders.ReturnCustomerAddress, OrderHeaders.ReturnTaxNumber, OrderHeaders.ReturnTaxOffice, OrderHeaders.ReturnSerialNo, OrderHeaders.ReturnReason, OrderHeaders.ReturnReasonCode, ISNULL(OrderHeaders.UsdAmount,0) AS UsdAmount, ISNULL(OrderHeaders.EurAmount,0) AS EurAmount, ISNULL(OrderHeaders.GbpAmount,0) AS GbpAmount, ISNULL(OrderHeaders.OrderCounter,0) AS OrderCounter 
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
WHERE OrderHeaders.[OrderKey] = '676C7755-311A-476B-ADEC-75723D4EEE78';

-- 12. Sifariş Sətirlərinin Oxunması
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
SELECT DISTINCT 
    OrderTransactions.AutoID, OrderTransactions.TransactionDateTime, OrderTransactions.TransactionID, OrderTransactions.TransactionKey, 
    OrderTransactions.OrderKey, OrderTransactions.OrderID, OrderTransactions.OrderDateTime, OrderTransactions.StationID, OrderTransactions.EmployeeID, 
    OrderTransactions.RevenueCenterTypeID, OrderTransactions.MenuItemID, OrderTransactions.MenuItemText, '' as DisplayText, OrderTransactions.MenuItemGroupText, 
    OrderTransactions.MenumItemCategoryText, OrderTransactions.MenuItemUnitPrice, OrderTransactions.MenuItemCost, OrderTransactions.Quantity, 
    OrderTransactions.ExtendedPrice, OrderTransactions.DiscountID, OrderTransactions.DiscountLineAmount, OrderTransactions.DiscountCashAmount, 
    OrderTransactions.DiscountTotalAmount, OrderTransactions.TransactionStatus, OrderTransactions.NotificationStatus, OrderTransactions.AdditionLinePrinted, 
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
    OrderTransactions.CustomField4, OrderTransactions.CustomField5, OrderTransactions.EditKey, OrderTransactions.SyncKey, OrderTransactions.BranchID, 
    isnull(OrderTransactions.SplitDetail,'') as SplitDetail, OrderTransactions.AddUserID, OrderTransactions.AddDateTime, OrderTransactions.EditUserID, OrderTransactions.EditDateTime, isnull(OrderTransactions.EmployeeName,'') AS EmployeeName, 
    isnull(OrderTransactions.AddUserName,'') AS AddEmployeeName, isnull(OrderTransactions.EditUserName,'') AS EditUserName, isnull(OrderTransactions.EditUserName,'') AS EditEmployeeName, isnull(Discounts.DiscountText,'') as DiscountText, isnull(OrderTransactions.DiscountAmountValue,0) as DiscountAmountValue, isnull(OrderTransactions.DiscountBasisValue,0) as DiscountBasisValue, 
    mm1.MenuModifierText AS Mod1IDtext, mm2.MenuModifierText AS Mod2IDtext, mm3.MenuModifierText AS Mod3IDtext, mm4.MenuModifierText AS Mod4IDtext, mm5.MenuModifierText AS Mod5IDtext, 
    mm6.MenuModifierText AS Mod6IDtext, mm7.MenuModifierText AS Mod7IDtext, mm8.MenuModifierText AS Mod8IDtext, mm9.MenuModifierText AS Mod9IDtext, mm10.MenuModifierText AS Mod10IDtext, 
    mm11.MenuModifierText AS Mod11IDtext, mm12.MenuModifierText AS Mod12IDtext, mm13.MenuModifierText AS Mod13IDtext, mm14.MenuModifierText AS Mod14IDtext, mm15.MenuModifierText AS Mod15IDtext, 
    mm16.MenuModifierText AS Mod16IDtext, mm17.MenuModifierText AS Mod17IDtext, mm18.MenuModifierText AS Mod18IDtext, mm19.MenuModifierText AS Mod19IDtext, mm20.MenuModifierText AS Mod20IDtext, 
    lee.FirstName AS LastEmployeeName, isnull(Discounts.DiscountBasis,0) as DiscountBasis, isnull(Discounts.DiscountAmount,0) AS DiscountApplyAmount, 
    OrderTransactions.MenuItemKey, OrderTransactions.DiscountKey, OrderTransactions.PromotionKey, isnull(Promotions.PromotionName,'') as PromotionName, (isnull(OrderTransactions.Quantity,0)*isnull(OrderTransactions.MenuItemUnitPrice,0)) as TotalPrice, ISNULL(mi.MenuItemDescription,mi.MenuItemText) AS MenuItemText2, isnull(mi.Barcode,'') AS Barcode, isnull(mi.Barcode2,'') AS Barcode2, 
    isnull(OrderTransactions.AddUserName,'') AS AddUserName, isnull(OrderTransactions.OrderByWeight,0) AS OrderByWeight, isnull(OrderTransactions.PromotionName,'') AS PromotionName, isnull(OrderTransactions.EditUserName,'') AS EditUserName, isnull(OrderTransactions.UsedDiscountName,'') AS UsedDiscountName, isnull(OrderTransactions.DiscountUserName,0) as DiscountUserName, OrderTransactions.OwnerKey, OrderTransactions.PromotionCount, OrderTransactions.PromotionAmount, OrderTransactions.AccountingCode, OrderTransactions.MainMenuItemText, OrderTransactions.MainMenuItemTransactionKey, ISNULL(OrderTransactions.LabelPrinted, 0) AS LabelPrinted 
FROM OrderTransactions WITH (NOLOCK) 
LEFT OUTER JOIN EmployeeFiles AS lee ON isnull(OrderTransactions.EditUserID,OrderTransactions.AddUserID) = lee.AutoID 
LEFT OUTER JOIN EmployeeFiles as lo ON OrderTransactions.EmployeeID = lo.AutoID 
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
WHERE OrderTransactions.[OrderKey] = '676C7755-311A-476B-ADEC-75723D4EEE78' 
ORDER BY OrderTransactions.AutoID;

-- 13. Ödənişlərin Oxunması
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
SELECT 
    OrderPayments.AutoID, OrderPayments.OrderPaymentID, OrderPayments.PaymentKey, OrderPayments.OrderID, OrderPayments.OrderKey, OrderPayments.StationID, 
    OrderPayments.CustomerID, OrderPayments.CustomerEmployeeID, OrderPayments.CouponNumber, OrderPayments.RegisterSessionID, OrderPayments.RevenueCenterTypeID, OrderPayments.PaymentDateTime, OrderPayments.EmployeeID, OrderPayments.RegisterNo, 
    OrderPayments.PaymentMethodID, OrderPayments.AmountTendered, OrderPayments.AmountPaid, OrderPayments.AmountChange, OrderPayments.ExhangeRate, 
    OrderPayments.RoundingAmount, OrderPayments.IsAccountPayment, OrderPayments.IsAccountSale, OrderPayments.CurrencyID, OrderPayments.PaymentMethodCode, OrderPayments.PaymentNotes, isnull(OrderPayments.LineDeleted,0) as LineDeleted, 
    OrderPayments.DeleteReason, OrderPayments.CustomField1, OrderPayments.CustomField2, OrderPayments.CustomField3, OrderPayments.CustomField4, 
    OrderPayments.CustomField5, OrderPayments.EditKey, OrderPayments.SyncKey, OrderPayments.BranchID, OrderPayments.AddUserID, 
    OrderPayments.AddDateTime, OrderPayments.EditUserID, OrderPayments.EditDateTime, PaymentMethods.PaymentName AS PaymentMethodName, 
    e.FirstName AS EmployeeName, ea.FirstName AS AddEmployeeName, ee.FirstName AS EditEmployeeName, c.CustomerName, 
    OrderPayments.CustomerKey, OrderPayments.CustomerEmployeeKey, OrderPayments.EmployeeKey, OrderPayments.PaymentMethodKey, isnull(OrderPayments.RetailData,'') AS RetailData, OrderPayments.GlobalBankCode, OrderPayments.GlobalBankName, OrderPayments.AccountingCode, OrderPayments.BankAccountingCode, OrderPayments.IncomeAccountingCode, ISNULL(OrderPayments.PaymentStatus, 0) AS PaymentStatus, ISNULL(OrderPayments.IntegrationApprove, 0) AS IntegrationApprove, OrderPayments.IntegrationReferenceNo, ISNULL(OrderPayments.RefundDetail, '') AS RefundDetail, ISNULL(OrderPayments.TsmUsed, 0) AS TsmUsed 
FROM OrderPayments WITH (NOLOCK) 
LEFT OUTER JOIN PaymentMethods ON OrderPayments.PaymentMethodID = PaymentMethods.PaymentMethodID 
LEFT OUTER JOIN EmployeeFiles AS ee ON OrderPayments.EditUserID = ee.EmployeeID 
LEFT OUTER JOIN EmployeeFiles AS ea ON OrderPayments.AddUserID = ea.EmployeeID 
LEFT OUTER JOIN EmployeeFiles AS e ON OrderPayments.EmployeeID = e.EmployeeID 
LEFT OUTER JOIN CustomerFiles AS c ON OrderPayments.CustomerID = c.CustomerID 
WHERE OrderPayments.OrderKey = '676C7755-311A-476B-ADEC-75723D4EEE78';

-- 14. Sifariş Qeydlərinin Yoxlanması
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
SELECT [AutoID], [BranchID], [OrderKey], [TypeName], [OrderData], [TransactionKey] 
FROM [OrderNotes] 
WHERE OrderKey = '676c7755-311a-476b-adec-75723d4eee78';

-- 15. Masalar Üzrə Son Hərəkət Tarixləri
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
SELECT r.DineInTableID, MAX(r.lastDate) AS lastDate 
FROM ( 
    SELECT h.AutoID, h.OrderKey, h.DineInTableID, (ISNULL(ISNULL(d.EditDateTime, d.AddDateTime), h.AddDateTime)) AS lastDate 
    FROM OrderHeaders AS h WITH(NOLOCK) 
    INNER JOIN OrderTransactions AS d ON d.OrderKey = h.OrderKey 
    WHERE h.OrderDateTime > DATEADD(hour, -48, GETDATE()) 
      AND isnull(h.LineDeleted, 0) = 0 
      AND h.OrderStatus < 2 
      AND h.DineInTableID > 0 
) AS r 
GROUP BY r.AutoID, r.OrderKey, r.DineInTableID;