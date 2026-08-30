INSERT INTO [AccessLogs] (
    [BranchID], [LogDate], [StationID], [EmployeeID], 
    [ActionName], [WrongPassword], [AdditionalInfo], [IsSuccess], 
    [OrderKey], [TransactionKey], [AccessLogKey], [EditKey], [SyncKey]
) 
VALUES (
    422, 
    GETDATE(), 
    1, 
    106, 
    N'TÜM ÇEKLERİ GÖRME', 
    NULL, 
    NULL, 
    1, 
    '00000000-0000-0000-0000-000000000000', 
    '00000000-0000-0000-0000-000000000000', 
    NEWID(), 
    'FCA93C6A-BB96-4C83-8EE4-4747FEBD5089', 
    '630D826D-375F-430C-AFAD-63ED3E28049D'
);
```[cite: 2]

---

**2. Verilən Tarix Aralığı Üzrə Bütün Çeklərin/Sifarişlərin Gətirilməsi**
```sql
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
    ISNULL(OrderHeaders.EditDateTime, OrderHeaders.AddDateTime) AS EditDateTime, 
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
WHERE OrderHeaders.OrderDateTime >= '2026-08-23 06:00:00' 
  AND OrderHeaders.OrderDateTime < '2026-08-24 05:59:59'  
  AND (ISNULL(OrderHeaders.DeleteReason, '') NOT LIKE '%ayrıldı%')  
ORDER BY OrderHeaders.OrderDateTime DESC;
```[cite: 2]

---

**3. Sifarişlərin Ödəniş Detallarının Gətirilməsi**
```sql
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
SELECT 
    p.OrderKey, 
    ISNULL(p.PaymentMethodName, '') AS PaymentMethodName, 
    p.CouponNumber, 
    p.GlobalBankName 
FROM OrderPayments AS p WITH (NOLOCK) 
WHERE p.PaymentDateTime >= '2026-08-23 06:00:00' 
  AND p.PaymentDateTime < '2026-08-24 05:59:59' 
  AND p.LineDeleted = 0;
```[cite: 2]