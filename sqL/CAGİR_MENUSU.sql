SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
SELECT 
    EmployeeFiles.AutoID, 
    EmployeeFiles.EmployeeKey, 
    EmployeeFiles.EmployeeID, 
    EmployeeFiles.FirstName, 
    EmployeeFiles.LastName,  
    EmployeeFiles.SocialSecurityNumber, 
    EmployeeFiles.SmarCardCode, 
    EmployeeFiles.MifareCardCode, 
    EmployeeFiles.MailingAddress, 
    EmployeeFiles.MailingZipCode, 
    EmployeeFiles.DateHired, 
    EmployeeFiles.DateReleased,  
    EmployeeFiles.EmployeeActive, 
    EmployeeFiles.JobTitleID, 
    EmployeeFiles.SecurityLevel, 
    EmployeeFiles.AccessCode, 
    EmployeeFiles.TipsReceived,  
    EmployeeFiles.PayBasis, 
    EmployeeFiles.PayRate, 
    EmployeeFiles.ScanCode, 
    ISNULL(EmployeeFiles.DriverLicenseNumber, '') AS DriverLicenseNumber, 
    EmployeeFiles.DriverLicenseExpires,  
    EmployeeFiles.CarInsurancePolicyCarrier, 
    EmployeeFiles.CarInsurancePolicyNumber, 
    EmployeeFiles.CarInsurancePolicyExpires,  
    EmployeeFiles.CarInsurancePolicyNotes, 
    ISNULL(EmployeeFiles.PrefUserInterfaceLocale, '-') AS PrefUserInterfaceLocale, 
    EmployeeFiles.EmployeeNotes, 
    EmployeeFiles.OrderEntryUseSecLang,  
    EmployeeFiles.EmployeeIsDriver, 
    EmployeeFiles.DefaultOEMenuGroupID, 
    EmployeeFiles.UseStaffBank, 
    EmployeeFiles.ScheduleNotEnforced,  
    EmployeeFiles.UseHostess, 
    EmployeeFiles.IsAServer, 
    EmployeeFiles.IsOffline, 
    EmployeeFiles.NoCashierOut, 
    EmployeeFiles.EditTimestamp,  
    EmployeeFiles.PhoneNumber, 
    EmployeeFiles.RevenueCenterTypeID, 
    EmployeeFiles.DeleteReason, 
    EmployeeFiles.CustomField1, 
    EmployeeFiles.CustomField2,  
    EmployeeFiles.CustomField3, 
    EmployeeFiles.CustomField4, 
    EmployeeFiles.CustomField5, 
    EmployeeFiles.EditKey, 
    EmployeeFiles.SyncKey,  
    EmployeeFiles.BranchID, 
    EmployeeFiles.AddUserID, 
    EmployeeFiles.AddDateTime, 
    EmployeeFiles.EditUserID, 
    EmployeeFiles.EditDateTime,  
    EmployeeTitles.TitleName AS JobTitleText, 
    ISNULL(EmployeeFiles.MonthlyDinnerFee, 0) AS MonthlyDinnerFee,
    CAST(0 AS BIT) AS IsChecked 
FROM EmployeeFiles  
LEFT OUTER JOIN EmployeeTitles ON EmployeeFiles.JobTitleID = EmployeeTitles.TitleID  
WHERE (EmployeeFiles.AccessCode = '106510' 
    OR EmployeeFiles.MifareCardCode = '106510' 
    OR EmployeeFiles.SmarCardCode = '106510') 
  AND ISNULL(EmployeeFiles.EmployeeActive, 0) = 1;
```[cite: 1]

---

**2. Oxunmamış Mesajların Yoxlanması**
```sql
SELECT ISNULL((
    SELECT TOP 1 CAST(MessageKey AS NVARCHAR(50)) 
    FROM UserMessages 
    WHERE Reciepments LIKE +'%,106,%' 
      AND ISNULL(Readed, '') NOT LIKE +'%,106,%'
), '-') AS MessageKey;
```[cite: 1]

---

**3. "ÇAĞIR" Əməliyyatının Loq Yazılışı**
```sql
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
    N'ÇAĞIR', 
    N'106510', 
    NULL, 
    1, 
    '00000000-0000-0000-0000-000000000000', 
    '00000000-0000-0000-0000-000000000000', 
    NEWID(), 
    '7431FDD8-DFB4-478D-995A-AF39BFF979B9', 
    '91B74A34-95B6-4E39-BFE0-D6897BB1DA99'
);
```[cite: 1]

---

**4. Açıq Sifarişlərin Gətirilməsi**
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
WHERE OrderHeaders.OrderStatus = 1 
  AND (ISNULL(OrderHeaders.DeleteReason, '') NOT LIKE '%ayrıldı%')  
ORDER BY OrderHeaders.OrderDateTime DESC;
```[cite: 1]

---

**5. Filial Statusunun (BranchStatus) Oxunması**
```sql
SELECT ISNULL((
    SELECT TOP 1 ISNULL(ParamValue, 'NO') 
    FROM StoreSettings 
    WHERE ParamKey = 'BranchStatus'
), 'NO') AS ParamValue;
```[cite: 1]