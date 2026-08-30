-- 1. İşçi Məlumatlarının Oxunması
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
SELECT 
    EmployeeFiles.AutoID, 
    EmployeeFiles.EmployeeKey, 
    EmployeeFiles.EmployeeID, 
    EmployeeFiles.FirstName, 
    EmployeeFiles.LastName,  
    EmployeeFiles.SocialSecurityNumber, 
    EmployeeFiles.SmartCardCode, 
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
    EmployeeFiles.OrderentryUseSecLang,  
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
LEFT OUTER JOIN EmployeeTitles 
    ON EmployeeFiles.JobTitleID = EmployeeTitles.TitleID  
WHERE EmployeeFiles.AutoID = 106;

-- 2. Masa Qrupu Məlumatlarının Oxunması
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
SELECT 
    [AutoID], 
    [TableGroupID], 
    [TableGroupKey], 
    [TableGroupText], 
    [RevenueCenterTypeID], 
    [DeleteReason], 
    [CustomField1], 
    [CustomField2], 
    [CustomField3], 
    [CustomField4], 
    [CustomField5], 
    [EditKey], 
    [SyncKey], 
    [BranchID], 
    [AddUserID], 
    [AddDateTime], 
    [EditUserID], 
    [EditDateTime], 
    ISNULL(TableRowCount, 8) AS TableRowCount, 
    ISNULL(TableColumnCount, 9) AS TableColumnCount  
FROM [DineInTableGroups] 
WHERE AutoID = 1;

-- 3. Masalar və Aktiv Sifarişlərin Oxunması
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
SELECT 
    DineInTables.AutoID, 
    DineInTables.DineInTableID, 
    DineInTables.DineInTableKey, 
    DineInTables.TableGroupKey, 
    DineInTables.DineInTableText, 
    DineInTables.SectionNumber, 
    DineInTables.TableGroupID,  
    DineInTables.DisplayIndex, 
    ISNULL(DineInTables.DineInTableActive, 1) AS DineInTableActive, 
    DineInTables.MaxGuests, 
    DineInTables.Smoking, 
    DineInTables.Window, 
    DineInTables.Booth,  
    DineInTables.Privacy, 
    DineInTables.PictureName, 
    DineInTables.AvarageSeatTime, 
    DineInTables.RevenueCenterTypeID, 
    ISNULL(DineInTables.SecurityLevel, 0) AS SecurityLevel,  
    DineInTables.DeleteReason, 
    DineInTables.CustomField1, 
    DineInTables.CustomField2, 
    DineInTables.CustomField3, 
    DineInTables.CustomField4,  
    DineInTables.CustomField5, 
    DineInTables.EditKey, 
    DineInTables.SyncKey, 
    DineInTables.BranchID, 
    DineInTables.AddUserID, 
    DineInTables.AddDateTime,  
    DineInTables.EditUserID, 
    DineInTables.EditDateTime,
    OrderHeaders.OrderKey AS ActiveOrderKey,
    (ISNULL(OrderHeaders.AmountDue, 0.0) + ISNULL(OrderHeaders.CashGratuity, 0.0)) AS ActiveOrderAmountDue, 
    OrderHeaders.OrderDateTime AS ActiveOrderDateTime, 
    OrderHeaders.EditDateTime AS ActiveOrderEditTime, 
    ISNULL(EmployeeFiles.FirstName, '') AS ActiveOrderEmyloyeeName,
    ISNULL(OrderHeaders.GuestCheckPrinted, 0) AS GuestCheckPrinted,
    ISNULL(OrderHeaders.TableReady, 0) AS TableReady, 
    ISNULL(OrderHeaders.AdditionPrintedLineCount, 0) AS AdditionPrintedLineCount,
    ISNULL(EmployeeFiles.EmployeeKey, '00000000-0000-0000-0000-000000000000') AS ActiveOrderEmyloyeeKey 
FROM DineInTables WITH (NOLOCK)  
LEFT OUTER JOIN OrderHeaders 
    ON OrderHeaders.DineInTableID = DineInTables.DineInTableID  
    AND ISNULL(OrderHeaders.OrderStatus, 1) = 1 
    AND OrderHeaders.OrderType = 1 
LEFT OUTER JOIN EmployeeFiles 
    ON EmployeeFiles.EmployeeID = OrderHeaders.EmployeeID  
WHERE DineInTables.DineInTableActive = 1 
  AND DineInTables.TableGroupID = 1;

-- 4. Server Vaxtının Yoxlanması
SELECT GETDATE() AS serverDatetime;

-- 5. Masalar üzrə Son Əməliyyat Vaxtının Hesablanması
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
SELECT 
    r.DineInTableID,
    MAX(r.lastDate) AS lastDate 
FROM ( 
    SELECT 
        h.AutoID,
        h.OrderKey,
        h.DineInTableID,
        (ISNULL(ISNULL(d.EditDateTime, d.AddDateTime), h.AddDateTime)) AS lastDate   
    FROM OrderHeaders AS h WITH (NOLOCK) 
    INNER JOIN OrderTransactions AS d 
        ON d.OrderKey = h.OrderKey  
    WHERE h.OrderDateTime > DATEADD(hour, -48, GETDATE()) 
      AND ISNULL(h.LineDeleted, 0) = 0 
      AND h.OrderStatus < 2 
      AND h.DineInTableID > 0 
) AS r 
GROUP BY 
    r.AutoID, 
    r.OrderKey, 
    r.DineInTableID;