-- 1. Açıq sifarişlərin sayı[cite: 3]
SELECT COUNT(AutoID) AS countValue 
FROM OrderHeaders 
WHERE OrderStatus = 1 AND ISNULL(LineDeleted, 0) = 0;
GO

-- 2. İstifadəçi şifrəsinin / kart kodunun yoxlanması[cite: 3]
EXEC sp_executesql 
    N'SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
    SELECT EmployeeFiles.AutoID, EmployeeFiles.EmployeeKey, EmployeeFiles.EmployeeID, EmployeeFiles.FirstName, EmployeeFiles.LastName,  
        EmployeeFiles.SocialSecurityNumber, EmployeeFiles.SmarCardCode, EmployeeFiles.MifareCardCode, EmployeeFiles.MailingAddress, 
        EmployeeFiles.MailingZipCode, EmployeeFiles.DateHired, EmployeeFiles.DateReleased, EmployeeFiles.EmployeeActive, 
        EmployeeFiles.JobTitleID, EmployeeFiles.SecurityLevel, EmployeeFiles.AccessCode, EmployeeFiles.TipsReceived,  
        EmployeeFiles.PayBasis, EmployeeFiles.PayRate, EmployeeFiles.ScanCode, ISNULL(EmployeeFiles.DriverLicenseNumber,'''') AS DriverLicenseNumber, 
        EmployeeFiles.DriverLicenseExpires, EmployeeFiles.CarInsurancePolicyCarrier, EmployeeFiles.CarInsurancePolicyNumber, 
        EmployeeFiles.CarInsurancePolicyExpires, EmployeeFiles.CarInsurancePolicyNotes, ISNULL(EmployeeFiles.PrefUserInterfaceLocale,''-'') AS PrefUserInterfaceLocale, 
        EmployeeFiles.EmployeeNotes, EmployeeFiles.OrderEntryUseSecLang, EmployeeFiles.EmployeeIsDriver, EmployeeFiles.DefaultOEMenuGroupID, 
        EmployeeFiles.UseStaffBank, EmployeeFiles.ScheduleNotEnforced, EmployeeFiles.UseHostess, EmployeeFiles.IsAServer, EmployeeFiles.IsOffline, 
        EmployeeFiles.NoCashierOut, EmployeeFiles.EditTimestamp, EmployeeFiles.PhoneNumber, EmployeeFiles.RevenueCenterTypeID, 
        EmployeeFiles.DeleteReason, EmployeeFiles.CustomField1, EmployeeFiles.CustomField2, EmployeeFiles.CustomField3, EmployeeFiles.CustomField4, 
        EmployeeFiles.CustomField5, EmployeeFiles.EditKey, EmployeeFiles.SyncKey, EmployeeFiles.BranchID, EmployeeFiles.AddUserID, 
        EmployeeFiles.AddDateTime, EmployeeFiles.EditUserID, EmployeeFiles.EditDateTime, EmployeeTitles.TitleName AS JobTitleText, 
        ISNULL(EmployeeFiles.MonthlyDinnerFee,0) AS MonthlyDinnerFee, CAST(0 AS BIT) AS IsChecked 
    FROM EmployeeFiles  
    LEFT OUTER JOIN EmployeeTitles ON EmployeeFiles.JobTitleID = EmployeeTitles.TitleID  
    WHERE (EmployeeFiles.AccessCode = @password OR EmployeeFiles.MifareCardCode = @password OR EmployeeFiles.SmarCardCode = @password) 
      AND ISNULL(EmployeeFiles.EmployeeActive, 0) = 1',
    N'@password nvarchar(6)',
    @password = N'106510';
GO

-- 3. Oxunmamış bildiriş/mesaj yoxlanışı[cite: 3]
SELECT ISNULL((
    SELECT TOP 1 CAST(MessageKey AS NVARCHAR(50)) 
    FROM UserMessages 
    WHERE Reciepments LIKE +'%,106,%' AND ISNULL(Readed, '') NOT LIKE +'%,106,%'
), '-');
GO

-- 4. Log: Kassir sessiyasını bağlamaq[cite: 3]
EXEC sp_executesql 
    N'INSERT INTO [AccessLogs] (
        [BranchID], [LogDate], [StationID], [EmployeeID], [ActionName], 
        [WrongPassword], [AdditionalInfo], [IsSuccess], [OrderKey], 
        [TransactionKey], [AccessLogKey], [EditKey], [SyncKey]
    ) VALUES (
        @BranchID, GETDATE(), @StationID, @EmployeeID, @ActionName, 
        @WrongPassword, @AdditionalInfo, @IsSuccess, @OrderKey, 
        @TransactionKey, NEWID(), @EditKey, @SyncKey
    )',
    N'@BranchID int, @LogDate datetime, @StationID int, @EmployeeID int, 
      @ActionName nvarchar(27), @WrongPassword nvarchar(6), @AdditionalInfo nvarchar(4000), 
      @IsSuccess bit, @OrderKey uniqueidentifier, @TransactionKey uniqueidentifier, 
      @AccessLogKey uniqueidentifier, @EditKey uniqueidentifier, @SyncKey uniqueidentifier',
    @BranchID = 422,
    @LogDate = '2026-09-05 19:51:20.333',
    @StationID = 1,
    @EmployeeID = 106,
    @ActionName = N'KASSİR SESSİYASINI BAĞLAMAQ',
    @WrongPassword = N'106510',
    @AdditionalInfo = NULL,
    @IsSuccess = 1,
    @OrderKey = '00000000-0000-0000-0000-000000000000',
    @TransactionKey = '00000000-0000-0000-0000-000000000000',
    @AccessLogKey = NULL,
    @EditKey = 'F4B8E948-AAE9-4F99-80A1-8DCDBD40B305',
    @SyncKey = '369E18A8-8BFA-4556-B427-90294EBC0440';
GO

-- 5. Cari stansiyanın aktiv sessiya məlumatı[cite: 3]
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
SELECT 
    RegisterSessions.RegisterSessionID, RegisterSessions.RegisterSessionKey, RegisterSessions.BranchID, RegisterSessions.EmployeeID, RegisterSessions.StationID,  
    RegisterSessions.AccountingDateTime, RegisterSessions.SignInDateTime, RegisterSessions.RegisterStartAmount, RegisterSessions.SignOutDateTime,  
    RegisterSessions.RegisterEndAmount, RegisterSessions.DiscrepancyAmount, RegisterSessions.DiscrepancyNotes, RegisterSessions.DiscrepancyNotes2, RegisterSessions.ManagerEmployeeID,  
    RegisterSessions.TotalPaymentMethod1, RegisterSessions.TotalPaymentMethod2, RegisterSessions.TotalPaymentMethod3,  
    RegisterSessions.TotalPaymentMethod4, RegisterSessions.TotalPaymentMethod5, RegisterSessions.TotalPaymentMethod6,  
    RegisterSessions.TotalPaymentMethod7, RegisterSessions.TotalPaymentMethod8, RegisterSessions.TotalPaymentMethod9,  
    RegisterSessions.TotalPaymentMethod10, RegisterSessions.TotalPaymentMethod11, RegisterSessions.TotalPaymentMethod12,  
    RegisterSessions.TotalPaymentMethod13, RegisterSessions.TotalPaymentMethod14, RegisterSessions.TotalPaymentMethod15,  
    RegisterSessions.TotalPaymentMethod16, RegisterSessions.TotalPaymentMethod17, RegisterSessions.TotalPaymentMethod18,  
    RegisterSessions.TotalPaymentMethod19, RegisterSessions.TotalPaymentMethod20, RegisterSessions.ZReportID, RegisterSessions.EditKey,  
    RegisterSessions.PaymentMethodName1, RegisterSessions.PaymentMethodName2, RegisterSessions.PaymentMethodName3, RegisterSessions.PaymentMethodName4, RegisterSessions.PaymentMethodName5, RegisterSessions.PaymentMethodName6, RegisterSessions.PaymentMethodName7, RegisterSessions.PaymentMethodName8, RegisterSessions.PaymentMethodName9, RegisterSessions.PaymentMethodName10, RegisterSessions.PaymentMethodName11, RegisterSessions.PaymentMethodName12, RegisterSessions.PaymentMethodName13, RegisterSessions.PaymentMethodName14, RegisterSessions.PaymentMethodName15, RegisterSessions.PaymentMethodName16, RegisterSessions.PaymentMethodName17, RegisterSessions.PaymentMethodName18, RegisterSessions.PaymentMethodName19, RegisterSessions.PaymentMethodName20, 
    RegisterSessions.EmployeeKey, RegisterSessions.SyncKey, 
    RTRIM(LTRIM(EmployeeFiles.FirstName)) + ' ' + RTRIM(LTRIM(ISNULL(EmployeeFiles.LastName, ''))) AS EmployeeFullName, 
    StationSettings.StationName,  
    EmployeeManager.FirstName + ' ' + ISNULL(EmployeeManager.LastName, '') AS ManagerFullName 
FROM RegisterSessions  
INNER JOIN EmployeeFiles ON RegisterSessions.EmployeeKey = EmployeeFiles.EmployeeKey  
INNER JOIN StationSettings ON RegisterSessions.StationID = StationSettings.StationID  
LEFT OUTER JOIN EmployeeFiles AS EmployeeManager ON RegisterSessions.ManagerEmployeeID = EmployeeManager.EmployeeID  
WHERE RegisterSessions.SignOutDateTime IS NULL AND RegisterSessions.StationID = 1;
GO

-- 6. Sessiya ID = 5 üzrə məlumatlar[cite: 3]
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
SELECT 
    RegisterSessions.RegisterSessionID, RegisterSessions.RegisterSessionKey, RegisterSessions.BranchID, RegisterSessions.EmployeeID, RegisterSessions.StationID,  
    RegisterSessions.AccountingDateTime, RegisterSessions.SignInDateTime, RegisterSessions.RegisterStartAmount, RegisterSessions.SignOutDateTime,  
    RegisterSessions.RegisterEndAmount, RegisterSessions.DiscrepancyAmount, RegisterSessions.DiscrepancyNotes, RegisterSessions.DiscrepancyNotes2, RegisterSessions.ManagerEmployeeID,  
    RegisterSessions.TotalPaymentMethod1, RegisterSessions.TotalPaymentMethod2, RegisterSessions.TotalPaymentMethod3,  
    RegisterSessions.TotalPaymentMethod4, RegisterSessions.TotalPaymentMethod5, RegisterSessions.TotalPaymentMethod6,  
    RegisterSessions.TotalPaymentMethod7, RegisterSessions.TotalPaymentMethod8, RegisterSessions.TotalPaymentMethod9,  
    RegisterSessions.TotalPaymentMethod10, RegisterSessions.TotalPaymentMethod11, RegisterSessions.TotalPaymentMethod12,  
    RegisterSessions.TotalPaymentMethod13, RegisterSessions.TotalPaymentMethod14, RegisterSessions.TotalPaymentMethod15,  
    RegisterSessions.TotalPaymentMethod16, RegisterSessions.TotalPaymentMethod17, RegisterSessions.TotalPaymentMethod18,  
    RegisterSessions.TotalPaymentMethod19, RegisterSessions.TotalPaymentMethod20, RegisterSessions.ZReportID, RegisterSessions.EditKey,  
    RegisterSessions.PaymentMethodName1, RegisterSessions.PaymentMethodName2, RegisterSessions.PaymentMethodName3, RegisterSessions.PaymentMethodName4, RegisterSessions.PaymentMethodName5, RegisterSessions.PaymentMethodName6, RegisterSessions.PaymentMethodName7, RegisterSessions.PaymentMethodName8, RegisterSessions.PaymentMethodName9, RegisterSessions.PaymentMethodName10, RegisterSessions.PaymentMethodName11, RegisterSessions.PaymentMethodName12, RegisterSessions.PaymentMethodName13, RegisterSessions.PaymentMethodName14, RegisterSessions.PaymentMethodName15, RegisterSessions.PaymentMethodName16, RegisterSessions.PaymentMethodName17, RegisterSessions.PaymentMethodName18, RegisterSessions.PaymentMethodName19, RegisterSessions.PaymentMethodName20, 
    RegisterSessions.EmployeeKey, RegisterSessions.SyncKey, 
    RTRIM(LTRIM(EmployeeFiles.FirstName)) + ' ' + RTRIM(LTRIM(ISNULL(EmployeeFiles.LastName, ''))) AS EmployeeFullName, 
    StationSettings.StationName,  
    EmployeeManager.FirstName + ' ' + ISNULL(EmployeeManager.LastName, '') AS ManagerFullName 
FROM RegisterSessions  
INNER JOIN EmployeeFiles ON RegisterSessions.EmployeeKey = EmployeeFiles.EmployeeKey  
INNER JOIN StationSettings ON RegisterSessions.StationID = StationSettings.StationID  
LEFT OUTER JOIN EmployeeFiles AS EmployeeManager ON RegisterSessions.ManagerEmployeeID = EmployeeManager.EmployeeID  
WHERE RegisterSessions.RegisterSessionID = 5;
GO

-- 7. Ödəniş növləri üzrə xərclər çıxılmaqla yekun məbləğlərin hesablanması[cite: 3]
EXEC sp_executesql 
    N'SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
    SELECT r2.AutoID, r2.PaymentMethodID, r2.PaymentName AS PaymentMethodName, r2.AmountPaid, 0 AS AmountDifference, r2.AmountPaid AS AmountGive, r2.PaymentTypeID 
    FROM ( 
        SELECT r.AutoID, r.PaymentMethodID, r.PaymentName, r.SumAmountPaid - ISNULL(SUM(e.ExpenseAmount), 0) AS AmountPaid, r.PaymentTypeID  
        FROM ( 
            SELECT pm.AutoID, pm.PaymentMethodID, pm.PaymentName, ISNULL(SUM(op.AmountPaid), 0) AS SumAmountPaid, pm.PaymentTypeID 
            FROM OrderPayments AS op  
            RIGHT OUTER JOIN PaymentMethods AS pm ON op.PaymentMethodID = pm.PaymentMethodID 
                AND op.RegisterSessionID = @RegisterSessionID AND ISNULL(op.LineDeleted, 0) = 0 
            WHERE ISNULL(pm.PaymentMethodActive, 0) = 1 AND pm.PaymentMethodID > 0 
            GROUP BY pm.AutoID, pm.PaymentMethodID, pm.PaymentName, pm.PaymentTypeID  
        ) AS r 
        LEFT OUTER JOIN Expenses AS e ON r.PaymentMethodID = e.PaymentMethodID 
            AND e.RegisterSessionID = @RegisterSessionID AND ISNULL(e.LineDeleted, 0) = 0  
        GROUP BY r.AutoID, r.PaymentMethodID, r.PaymentName, r.SumAmountPaid, r.PaymentTypeID 
    ) AS r2 
    ORDER BY r2.PaymentMethodID',
    N'@RegisterSessionID int',
    @RegisterSessionID = 5;
GO

-- 8. Cari sessiya üzrə xərclər (Expenses) siyahısı[cite: 3]
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
SELECT 
    Expenses.ExpenseID, Expenses.ExpenseKey, Expenses.ExpenseDatetime, Expenses.RegisterSessionID, 
    Expenses.RegisterID, Expenses.PaymentMethodID, Expenses.ExpenseName, Expenses.ExpenseAmount, 
    Expenses.ExpenseDescription, Expenses.IsEmplyeeAdvance, Expenses.AdvanceEmplyeeID, 
    Expenses.AdvanceEmplyeeKey, Expenses.StationID, Expenses.LineDeleted, Expenses.DeleteReason, 
    Expenses.CustomField1, Expenses.CustomField2, Expenses.CustomField3, Expenses.CustomField4, 
    Expenses.CustomField5, Expenses.EditKey, Expenses.SyncKey, Expenses.BranchID, 
    Expenses.AddUserID, Expenses.AddDateTime, Expenses.EditUserID, Expenses.EditDateTime, 
    ISNULL(ea.FirstName, '') + ' ' + ISNULL(ea.LastName, '') AS AddEmployeeName, 
    ee.FirstName AS EditEmployeeName, avd.FirstName AS AdvanceEmployeeName, 
    ss.StationName, pm.PaymentName AS PaymentMethodName 
FROM Expenses 
LEFT OUTER JOIN EmployeeFiles AS ea ON Expenses.AddUserID = ea.AutoID 
LEFT OUTER JOIN EmployeeFiles AS ee ON Expenses.EditUserID = ee.AutoID 
LEFT OUTER JOIN EmployeeFiles AS avd ON Expenses.AdvanceEmplyeeKey = avd.EmployeeKey 
LEFT OUTER JOIN StationSettings AS ss ON Expenses.StationID = ss.StationID
LEFT OUTER JOIN PaymentMethods AS pm ON Expenses.PaymentMethodID = pm.PaymentMethodID
INNER JOIN RegisterSessions ON Expenses.RegisterSessionID = RegisterSessions.RegisterSessionID   
    AND RegisterSessions.SignOutDateTime IS NULL 
    AND Expenses.ExpenseDatetime > RegisterSessions.SignInDateTime  
WHERE ISNULL(Expenses.LineDeleted, 0) = 0 
ORDER BY Expenses.ExpenseDatetime;
GO

-- 9. Log: Kassa dövriyyəsini / cəmini görmək[cite: 3]
EXEC sp_executesql 
    N'INSERT INTO [AccessLogs] (
        [BranchID], [LogDate], [StationID], [EmployeeID], [ActionName], 
        [WrongPassword], [AdditionalInfo], [IsSuccess], [OrderKey], 
        [TransactionKey], [AccessLogKey], [EditKey], [SyncKey]
    ) VALUES (
        @BranchID, GETDATE(), @StationID, @EmployeeID, @ActionName, 
        @WrongPassword, @AdditionalInfo, @IsSuccess, @OrderKey, 
        @TransactionKey, NEWID(), @EditKey, @SyncKey
    )',
    N'@BranchID int, @LogDate datetime, @StationID int, @EmployeeID int, 
      @ActionName nvarchar(34), @WrongPassword nvarchar(4000), @AdditionalInfo nvarchar(4000), 
      @IsSuccess bit, @OrderKey uniqueidentifier, @TransactionKey uniqueidentifier, 
      @AccessLogKey uniqueidentifier, @EditKey uniqueidentifier, @SyncKey uniqueidentifier',
    @BranchID = 422,
    @LogDate = '2026-09-05 19:51:27.400',
    @StationID = 1,
    @EmployeeID = 106,
    @ActionName = N'KASSA DÖVRİYYƏSİNİ / CƏMİNİ GÖRMƏK',
    @WrongPassword = NULL,
    @AdditionalInfo = NULL,
    @IsSuccess = 1,
    @OrderKey = '00000000-0000-0000-0000-000000000000',
    @TransactionKey = '00000000-0000-0000-0000-000000000000',
    @AccessLogKey = NULL,
    @EditKey = 'EEDEE9F9-F88D-4850-9E05-FF7E0CC9B631',
    @SyncKey = '90828AC1-4D86-45FB-B9E1-27B57D6619DF';
GO

-- 10. Son 4 saat ərzində bağlanmış sifarişlər (OrderHeaders)[cite: 3]
SELECT 
    h.OrderKey, h.OrderKey AS LineKey, h.AmountDue, h.OrderStatus, 
    ISNULL(h.LineDeleted, 0) AS LineDeleted, ISNULL(h.DeleteReason, '') AS DeleteReason, 
    ISNULL(h.EditDateTime, h.AddDateTime) AS EditDateTime 
FROM OrderHeaders AS h 
WHERE h.OrderDateTime > DATEADD(hour, -4, GETDATE()) 
  AND h.OrderDateTime < DATEADD(minute, -20, GETDATE())  
  AND h.OrderStatus > 1;
GO

-- 11. Son 4 saatın tranzaksiyaları (OrderTransactions)[cite: 3]
SELECT 
    h.OrderKey, h.TransactionKey AS LineKey, h.ExtendedPrice AS AmountDue, 
    h.TransactionStatus AS OrderStatus, ISNULL(h.LineDeleted, 0) AS LineDeleted, 
    ISNULL(h.DeleteReason, '') AS DeleteReason, ISNULL(h.EditDateTime, h.AddDateTime) AS EditDateTime 
FROM OrderTransactions AS h 
WHERE h.LineDeleted = 0 
  AND h.TransactionDateTime > DATEADD(hour, -4, GETDATE());
GO

-- 12. Son 4 saatın ödənişləri (OrderPayments)[cite: 3]
SELECT 
    h.OrderKey, h.PaymentKey AS LineKey, h.AmountPaid AS AmountDue, 2 AS OrderStatus, 
    ISNULL(h.LineDeleted, 0) AS LineDeleted, ISNULL(h.DeleteReason, '') AS DeleteReason, 
    ISNULL(h.EditDateTime, h.AddDateTime) AS EditDateTime 
FROM OrderPayments AS h 
WHERE h.LineDeleted = 0 
  AND h.PaymentDateTime > DATEADD(hour, -4, GETDATE());
GO

-- 13. Sessiyada bağlanmamış sifarişin olub-olmaması yoxlanışı[cite: 3]
EXEC sp_executesql 
    N'SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
    SELECT OrderHeaders.AutoID AS CountValue 
    FROM OrderPayments 
    INNER JOIN OrderHeaders ON OrderPayments.OrderKey = OrderHeaders.OrderKey 
    WHERE (OrderPayments.RegisterSessionID = @RegisterSessionID) 
      AND ISNULL(OrderHeaders.OrderStatus, 1) < 2 
      AND ISNULL(OrderPayments.LineDeleted, 0) = 0',
    N'@RegisterSessionID int',
    @RegisterSessionID = 5;
GO

-- 14. Server vaxtı[cite: 3]
SELECT GETDATE() AS serverDatetime;
GO

-- 15. Açıq sifarişlərin ümumi borc məbləği cəmi[cite: 3]
SELECT ROUND(ISNULL(SUM(Amountdue), 0.0), 2) AS countValue 
FROM OrderHeaders 
WHERE OrderStatus = 1 AND ISNULL(LineDeleted, 0) = 0;
GO

-- 16. KASSA SESSIYASININ BAĞLANMASI VƏ YEKUN MƏBLƏĞLƏRİN YAZILMASI[cite: 3]
EXEC sp_executesql 
    N'UPDATE [RegisterSessions] SET 
        [BranchID]=@BranchID, [EmployeeID]=@EmployeeID, [RegisterSessionKey]=@RegisterSessionKey, 
        [StationID]=@StationID, [AccountingDateTime]=@AccountingDateTime, [RegisterStartAmount]=@RegisterStartAmount, 
        [SignOutDateTime]=@SignOutDateTime, [RegisterEndAmount]=@RegisterEndAmount, [DiscrepancyAmount]=@DiscrepancyAmount, 
        [DiscrepancyNotes]=@DiscrepancyNotes, [DiscrepancyNotes2]=@DiscrepancyNotes2, [ManagerEmployeeID]=@ManagerEmployeeID, 
        [TotalPaymentMethod1]=@TotalPaymentMethod1, [TotalPaymentMethod2]=@TotalPaymentMethod2, [TotalPaymentMethod3]=@TotalPaymentMethod3, 
        [TotalPaymentMethod4]=@TotalPaymentMethod4, [TotalPaymentMethod5]=@TotalPaymentMethod5, [TotalPaymentMethod6]=@TotalPaymentMethod6, 
        [TotalPaymentMethod7]=@TotalPaymentMethod7, [TotalPaymentMethod8]=@TotalPaymentMethod8, [TotalPaymentMethod9]=@TotalPaymentMethod9, 
        [TotalPaymentMethod10]=@TotalPaymentMethod10, [TotalPaymentMethod11]=@TotalPaymentMethod11, [TotalPaymentMethod12]=@TotalPaymentMethod12, 
        [TotalPaymentMethod13]=@TotalPaymentMethod13, [TotalPaymentMethod14]=@TotalPaymentMethod14, [TotalPaymentMethod15]=@TotalPaymentMethod15, 
        [TotalPaymentMethod16]=@TotalPaymentMethod16, [TotalPaymentMethod17]=@TotalPaymentMethod17, [TotalPaymentMethod18]=@TotalPaymentMethod18, 
        [TotalPaymentMethod19]=@TotalPaymentMethod19, [TotalPaymentMethod20]=@TotalPaymentMethod20, [ClosePaymentMethod1]=@ClosePaymentMethod1, 
        [ClosePaymentMethod2]=@ClosePaymentMethod2, [ClosePaymentMethod3]=@ClosePaymentMethod3, [ClosePaymentMethod4]=@ClosePaymentMethod4, 
        [ClosePaymentMethod5]=@ClosePaymentMethod5, [ClosePaymentMethod6]=@ClosePaymentMethod6, [ClosePaymentMethod7]=@ClosePaymentMethod7, 
        [ClosePaymentMethod8]=@ClosePaymentMethod8, [ClosePaymentMethod9]=@ClosePaymentMethod9, [ClosePaymentMethod10]=@ClosePaymentMethod10, 
        [ClosePaymentMethod11]=@ClosePaymentMethod11, [ClosePaymentMethod12]=@ClosePaymentMethod12, [ClosePaymentMethod13]=@ClosePaymentMethod13, 
        [ClosePaymentMethod14]=@ClosePaymentMethod14, [ClosePaymentMethod15]=@ClosePaymentMethod15, [ClosePaymentMethod16]=@ClosePaymentMethod16, 
        [ClosePaymentMethod17]=@ClosePaymentMethod17, [ClosePaymentMethod18]=@ClosePaymentMethod18, [ClosePaymentMethod19]=@ClosePaymentMethod19, 
        [ClosePaymentMethod20]=@ClosePaymentMethod20, [ZReportID]=@ZReportID, [EditKey]=NEWID(), [SyncKey]=NEWID() 
    WHERE [RegisterSessionID]=@RegisterSessionID;

    UPDATE [RegisterSessions] SET 
        EditKey = NEWID(), SyncKey = NEWID(), 
        [PaymentMethodName1]=@PaymentMethodName1, [PaymentMethodName2]=@PaymentMethodName2, [PaymentMethodName3]=@PaymentMethodName3, 
        [PaymentMethodName4]=@PaymentMethodName4, [PaymentMethodName5]=@PaymentMethodName5, [PaymentMethodName6]=@PaymentMethodName6, 
        [PaymentMethodName7]=@PaymentMethodName7, [PaymentMethodName8]=@PaymentMethodName8, [PaymentMethodName9]=@PaymentMethodName9, 
        [PaymentMethodName10]=@PaymentMethodName10, [PaymentMethodName11]=@PaymentMethodName11, [PaymentMethodName12]=@PaymentMethodName12, 
        [PaymentMethodName13]=@PaymentMethodName13, [PaymentMethodName14]=@PaymentMethodName14, [PaymentMethodName15]=@PaymentMethodName15, 
        [PaymentMethodName16]=@PaymentMethodName16, [PaymentMethodName17]=@PaymentMethodName17, [PaymentMethodName18]=@PaymentMethodName18, 
        [PaymentMethodName19]=@PaymentMethodName19, [PaymentMethodName20]=@PaymentMethodName20 
    WHERE [RegisterSessionID]=@RegisterSessionID',
    N'@RegisterSessionID int, @BranchID int, @EmployeeID int, @RegisterSessionKey uniqueidentifier, @StationID int, 
      @AccountingDateTime datetime, @SignInDateTime datetime, @RegisterStartAmount float, @SignOutDateTime datetime, 
      @RegisterEndAmount float, @DiscrepancyAmount float, @DiscrepancyNotes nvarchar(7), @DiscrepancyNotes2 nvarchar(66), 
      @ManagerEmployeeID int, @TotalPaymentMethod1 float, @TotalPaymentMethod2 float, @TotalPaymentMethod3 float, 
      @TotalPaymentMethod4 float, @TotalPaymentMethod5 float, @TotalPaymentMethod6 float, @TotalPaymentMethod7 float, 
      @TotalPaymentMethod8 float, @TotalPaymentMethod9 float, @TotalPaymentMethod10 float, @TotalPaymentMethod11 float, 
      @TotalPaymentMethod12 float, @TotalPaymentMethod13 float, @TotalPaymentMethod14 float, @TotalPaymentMethod15 float, 
      @TotalPaymentMethod16 float, @TotalPaymentMethod17 float, @TotalPaymentMethod18 float, @TotalPaymentMethod19 float, 
      @TotalPaymentMethod20 float, @ClosePaymentMethod1 float, @ClosePaymentMethod2 float, @ClosePaymentMethod3 float, 
      @ClosePaymentMethod4 float, @ClosePaymentMethod5 float, @ClosePaymentMethod6 float, @ClosePaymentMethod7 float, 
      @ClosePaymentMethod8 float, @ClosePaymentMethod9 float, @ClosePaymentMethod10 float, @ClosePaymentMethod11 float, 
      @ClosePaymentMethod12 float, @ClosePaymentMethod13 float, @ClosePaymentMethod14 float, @ClosePaymentMethod15 float, 
      @ClosePaymentMethod16 float, @ClosePaymentMethod17 float, @ClosePaymentMethod18 float, @ClosePaymentMethod19 float, 
      @ClosePaymentMethod20 float, @ZReportID int, @EditKey uniqueidentifier, @SyncKey uniqueidentifier, 
      @PaymentMethodName1 nvarchar(4), @PaymentMethodName2 nvarchar(4000), @PaymentMethodName3 nvarchar(4000), 
      @PaymentMethodName4 nvarchar(10), @PaymentMethodName5 nvarchar(10), @PaymentMethodName6 nvarchar(4), 
      @PaymentMethodName7 nvarchar(9), @PaymentMethodName8 nvarchar(4), @PaymentMethodName9 nvarchar(3), 
      @PaymentMethodName10 nvarchar(4000), @PaymentMethodName11 nvarchar(4000), @PaymentMethodName12 nvarchar(6), 
      @PaymentMethodName13 nvarchar(9), @PaymentMethodName14 nvarchar(4000), @PaymentMethodName15 nvarchar(11), 
      @PaymentMethodName16 nvarchar(3), @PaymentMethodName17 nvarchar(4000), @PaymentMethodName18 nvarchar(4000), 
      @PaymentMethodName19 nvarchar(4000), @PaymentMethodName20 nvarchar(4000)',
    @RegisterSessionID = 5,
    @BranchID = 422,
    @EmployeeID = 106,
    @RegisterSessionKey = '9D53A887-8A5E-4B71-9D4B-22D7723E315E',
    @StationID = 1,
    @AccountingDateTime = '2026-09-05 00:00:00',
    @SignInDateTime = '2026-09-05 19:46:23.837',
    @RegisterStartAmount = 0,
    @SignOutDateTime = '2026-09-05 19:51:43.943',
    @RegisterEndAmount = 90,
    @DiscrepancyAmount = 25,
    @DiscrepancyNotes = N'NUSHPOS',
    @DiscrepancyNotes2 = N'MÜDİRİN XƏBƏRİ VAR!, (AÇIK ÇEK TUTARI : 0,00), (1 x 90,00 = 90,00)',
    @ManagerEmployeeID = 106,
    @TotalPaymentMethod1 = 88, @TotalPaymentMethod2 = 0, @TotalPaymentMethod3 = 0, @TotalPaymentMethod4 = 27, 
    @TotalPaymentMethod5 = 0, @TotalPaymentMethod6 = 0, @TotalPaymentMethod7 = 0, @TotalPaymentMethod8 = 0, 
    @TotalPaymentMethod9 = 0, @TotalPaymentMethod10 = 0, @TotalPaymentMethod11 = 0, @TotalPaymentMethod12 = 0, 
    @TotalPaymentMethod13 = 0, @TotalPaymentMethod14 = 0, @TotalPaymentMethod15 = 0, @TotalPaymentMethod16 = 0, 
    @TotalPaymentMethod17 = 0, @TotalPaymentMethod18 = 0, @TotalPaymentMethod19 = 0, @TotalPaymentMethod20 = 0, 
    @ClosePaymentMethod1 = 90, @ClosePaymentMethod2 = 0, @ClosePaymentMethod3 = 0, @ClosePaymentMethod4 = 0, 
    @ClosePaymentMethod5 = 0, @ClosePaymentMethod6 = 0, @ClosePaymentMethod7 = 0, @ClosePaymentMethod8 = 0, 
    @ClosePaymentMethod9 = 0, @ClosePaymentMethod10 = 0, @ClosePaymentMethod11 = 0, @ClosePaymentMethod12 = 0, 
    @ClosePaymentMethod13 = 0, @ClosePaymentMethod14 = 0, @ClosePaymentMethod15 = 0, @ClosePaymentMethod16 = 0, 
    @ClosePaymentMethod17 = 0, @ClosePaymentMethod18 = 0, @ClosePaymentMethod19 = 0, @ClosePaymentMethod20 = 0, 
    @ZReportID = 0,
    @EditKey = 'C88B54F6-BE57-461C-9640-6B1055B8F94F',
    @SyncKey = '00000000-0000-0000-0000-000000000000',
    @PaymentMethodName1 = N'NAĞD',
    @PaymentMethodName2 = NULL,
    @PaymentMethodName3 = NULL,
    @PaymentMethodName4 = N'PASHA BANK',
    @PaymentMethodName5 = N'YAPI KREDI',
    @PaymentMethodName6 = N'WOLT',
    @PaymentMethodName7 = N'BOLT FOOD',
    @PaymentMethodName8 = N'VOOP',
    @PaymentMethodName9 = N'189',
    @PaymentMethodName10 = NULL,
    @PaymentMethodName11 = NULL,
    @PaymentMethodName12 = N'TEZIBU',
    @PaymentMethodName13 = N'HUNGRY.AZ',
    @PaymentMethodName14 = NULL,
    @PaymentMethodName15 = N'ZIRAAT BANK',
    @PaymentMethodName16 = N'ABB',
    @PaymentMethodName17 = NULL,
    @PaymentMethodName18 = NULL,
    @PaymentMethodName19 = NULL,
    @PaymentMethodName20 = NULL;
GO

-- 17. Sessiya müddətində vurulan sifariş başlıqlarının sinxron açarlarının yenilənməsi[cite: 3]
EXEC sp_executesql 
    N'UPDATE OrderHeaders 
      SET EditKey = NEWID(), SyncKey = NEWID() 
      FROM OrderHeaders 
      INNER JOIN RegisterSessions ON OrderHeaders.OrderDateTime > RegisterSessions.SignInDateTime 
        AND RegisterSessions.RegisterSessionID = @RegisterSessionID 
      WHERE ISNULL(OrderHeaders.LineDeleted, 0) = 0;',
    N'@RegisterSessionID int',
    @RegisterSessionID = 5;
GO

-- 18. Z-Hesabatı / Sessiya Hesabatı şablon sorğularının oxunması[cite: 3]
EXEC sp_executesql 
    N'SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
    SELECT [AutoID], [ReportQueryID], [ReportQueryKey], [ReportID], [ReportKey], [QueryName], 
        REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(
            CAST([QueryData] AS NVARCHAR(MAX)), 
            ''GlobalPaymentMethods'', ''PaymentMethods''),
            ''GlobalEmployeeFiles'', ''EmployeeFiles''),
            ''GlobalMenuItems'', ''MenuItems''),
            ''GlobalPromotions'', ''Promotions''),
            ''GlobalDiscounts'', ''Discounts''),
            ''PaymentMethods'', ''PaymentMethods''), 
            ''EmployeeFiles'', ''EmployeeFiles''),
            ''MenuItems'', ''MenuItems''),
            ''Promotions'', ''Promotions''),
            ''Discounts'', ''Discounts'') AS QueryData, 
        [IsDefault], [EditKey], [SyncKey] 
    FROM [ReportQueries] 
    WHERE ReportKey = @ReportKey',
    N'@ReportKey uniqueidentifier',
    @ReportKey = 'DFCAB4A5-7ADB-4B1D-9611-C6FE13114AFF';
GO

-- 19. Hesabat tərifləri[cite: 3]
EXEC sp_executesql 
    N'SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
    SELECT Reports.AutoID, Reports.ReportID, Reports.ReportKey, Reports.ReportName, Reports.ReportCategoryID, 
        Reports.ReportActive, Reports.IsPosReport, ReportCategories.CategoryName, Reports.ReportTypeID, 
        Reports.SecurityLevel, Reports.EditKey, Reports.SyncKey 
    FROM Reports  
    LEFT OUTER JOIN ReportCategories ON Reports.ReportCategoryID = ReportCategories.ReportCategoryID 
    WHERE Reports.ReportKey = @ReportKey',
    N'@ReportKey uniqueidentifier',
    @ReportKey = 'DFCAB4A5-7ADB-4B1D-9611-C6FE13114AFF';
GO

-- 20. Hesabat çap dizaynı[cite: 3]
EXEC sp_executesql 
    N'SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
    SELECT [ReportDesignKey], [DesignName], [IsDefault]  
    FROM [ReportDesigns]  
    WHERE ReportKey = @ReportKey 
    ORDER BY IsDefault DESC',
    N'@ReportKey uniqueidentifier',
    @ReportKey = 'DFCAB4A5-7ADB-4B1D-9611-C6FE13114AFF';
GO

-- 21. Baza konfiqurasiyası və təmizləmə skriptləri[cite: 3]
UPDATE MenuModifierGroups SET MenuModifierGroupKey = NEWID() WHERE MenuModifierGroupKey IS NULL; 
UPDATE MenuModifierLayout SET MenuModifierLayoutKey = NEWID() WHERE MenuModifierLayoutKey IS NULL; 
UPDATE PaymentMethods SET PaymentName = 'NAKİT' WHERE PaymentName = 'NAKIT'; 
UPDATE MenuItemLayout SET LayoutKey = NEWID() WHERE LayoutKey IS NULL; 
UPDATE DineInTables SET DineInTableID = AutoID WHERE ISNULL(DineInTableID, 0) = 0; 
UPDATE DineInTableGroups SET TableGroupID = AutoID WHERE ISNULL(TableGroupID, 0) = 0; 
UPDATE DineInTableGroups SET TableGroupKey = NEWID() WHERE TableGroupKey IS NULL; 
UPDATE DineInTables SET DineInTableKey = NEWID() WHERE DineInTableKey IS NULL; 

UPDATE DineInTables 
SET TableGroupKey = DineInTableGroups.TableGroupKey 
FROM DineInTables 
INNER JOIN DineInTableGroups ON DineInTables.TableGroupID = DineInTableGroups.TableGroupID 
WHERE DineInTables.TableGroupKey IS NULL;

UPDATE t 
SET TableGroupKey = g.TableGroupKey 
FROM DineInTables t
INNER JOIN DineInTableGroups g ON g.TableGroupID = t.TableGroupID
WHERE t.TableGroupKey <> g.TableGroupKey;

UPDATE MenuItemLayout 
SET MainMenuItemKey = MenuItems.MenuItemKey 
FROM MenuItemLayout 
INNER JOIN MenuItems ON MenuItemLayout.MainMenuItemID = MenuItems.MenuItemID 
WHERE MenuItemLayout.MainMenuItemKey IS NULL; 

UPDATE GlobalMenuItemLayout 
SET MainMenuItemKey = GlobalMenuItems.MenuItemKey 
FROM GlobalMenuItemLayout 
INNER JOIN GlobalMenuItems ON GlobalMenuItemLayout.MainMenuItemID = GlobalMenuItems.MenuItemID 
WHERE GlobalMenuItemLayout.MainMenuItemKey IS NULL; 

UPDATE MenuItemLayout 
SET MenuItemKey = MenuItems.MenuItemKey 
FROM MenuItemLayout 
INNER JOIN MenuItems ON MenuItemLayout.MenuItemID = MenuItems.MenuItemID 
WHERE MenuItemLayout.MenuItemKey IS NULL; 

UPDATE GlobalMenuItemLayout 
SET MenuItemKey = GlobalMenuItems.MenuItemKey 
FROM GlobalMenuItemLayout 
INNER JOIN GlobalMenuItems ON GlobalMenuItemLayout.MenuItemID = GlobalMenuItems.MenuItemID AND GlobalMenuItemLayout.BranchID = GlobalMenuItems.BranchID 
WHERE GlobalMenuItemLayout.MenuItemKey IS NULL; 

UPDATE MenuModifierLayout 
SET MenuModifierKey = MenuModifiers.MenuModifierKey 
FROM MenuModifierLayout 
INNER JOIN MenuModifiers ON MenuModifierLayout.MenuModifierID = MenuModifiers.MenuModifierID 
WHERE MenuModifierLayout.MenuModifierKey IS NULL; 

UPDATE MenuModifierLayout 
SET MenuModifierGroupKey = MenuModifierGroups.MenuModifierGroupKey 
FROM MenuModifierLayout 
INNER JOIN MenuModifierGroups ON MenuModifierLayout.MenuModifierGroupID = MenuModifierGroups.MenuModifierGroupID 
WHERE MenuModifierLayout.MenuModifierGroupKey IS NULL; 

UPDATE GlobalMenuModifierLayout 
SET MenuModifierKey = GlobalMenuModifiers.MenuModifierKey 
FROM GlobalMenuModifierLayout 
INNER JOIN GlobalMenuModifiers ON GlobalMenuModifierLayout.MenuModifierID = GlobalMenuModifiers.MenuModifierID 
WHERE GlobalMenuModifierLayout.MenuModifierKey IS NULL; 

UPDATE GlobalMenuModifierLayout 
SET MenuModifierGroupKey = GlobalMenuModifierGroups.MenuModifierGroupKey 
FROM GlobalMenuModifierLayout 
INNER JOIN GlobalMenuModifierGroups ON GlobalMenuModifierLayout.MenuModifierGroupID = GlobalMenuModifierGroups.MenuModifierGroupID 
WHERE GlobalMenuModifierLayout.MenuModifierGroupKey IS NULL;

UPDATE MenuItemLayout 
SET MenuGroupKey = MenuGroups.MenuGroupKey 
FROM MenuItemLayout 
INNER JOIN MenuGroups ON MenuItemLayout.MenuGroupID = MenuGroups.MenuGroupID 
WHERE MenuItemLayout.MenuGroupKey IS NULL; 

UPDATE MenuItemLayout 
SET MainMenuItemKey = MenuItems.MenuItemKey 
FROM MenuItemLayout 
INNER JOIN MenuItems ON MenuItemLayout.MainMenuItemID = MenuItems.AutoID 
WHERE MenuItemLayout.MainMenuItemKey IS NULL AND ISNULL(MenuItemLayout.MainMenuItemID, 0) > 0; 

UPDATE EmployeeFiles 
SET EmployeeID = AutoID 
WHERE ISNULL(EmployeeID, 0) <> AutoID;

UPDATE [MenuItemLayout] 
SET MainMenuItemID = 0 
WHERE MainMenuItemID IS NULL;

DELETE FROM MenuItemLayout 
WHERE AutoID NOT IN (
    SELECT MAX(AutoID) 
    FROM MenuItemLayout 
    GROUP BY ISNULL(MenuGroupID, 0), ISNULL(MainMenuItemID, 0), DisplayIndex
);

UPDATE p
SET LineDeleted = 1,
    DeleteReason = 'ECR ERİŞİM SORUNU'
FROM OrderPayments p
INNER JOIN OrderHeaders h ON h.OrderKey = p.OrderKey 
WHERE h.OrderStatus = 1 
  AND h.LineDeleted = 0
  AND h.OrderDateTime > DATEADD(DAY, -7, GETDATE())
  AND p.LineDeleted = 0
  AND p.PaymentStatus IN (1, 3);
GO

-- 22. Müştəri fayllarının yenilənməsi[cite: 3]
UPDATE CustomerFiles 
SET CustomerName = EmployeeFiles.FirstName + ISNULL(NULLIF((' ' + ISNULL(EmployeeFiles.LastName, ' ')), ''), ''), 
    CustomerIsActive = EmployeeFiles.EmployeeActive, 
    CardNumber = EmployeeFiles.SmarCardCode 
FROM CustomerFiles 
INNER JOIN EmployeeFiles ON CustomerFiles.CustomerKey = EmployeeFiles.EmployeeKey;
GO

-- 23. Mağaza parametrlərinin oxunması[cite: 3]
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
SELECT 
    [SettingsID], [OrderID], [TabName], [GroupName], [ParamName], [ParamKey], 
    [ParamValue], [DefaultValue], [ParamType], [EditKey], [SyncKey], 
    [BranchID], [AddUserID], [AddDateTime], [EditUserID], [EditDateTime]  
FROM [StoreSettings] 
WHERE ParamKey <> 'StoreStatus' 
ORDER BY TabName, GroupName, OrderID;
GO

-- 24. Pərakəndə satış barkodları[cite: 3]
SELECT 
    [AutoID], [MenuItemKey], 
    ISNULL([Barcode], '') AS [Barcode], 
    ISNULL([Quantity], 0) AS [Quantity], 
    ISNULL([SalePrice], 0) AS [SalePrice] 
FROM [GlobalRetailBarcodes];
GO

-- 25. Vergi dərəcələri[cite: 3]
SELECT 
    [TaxGroupID], [GroupName], [TaxRate], [EditKey], [SyncKey] 
FROM [TaxGroups] 
ORDER BY TaxGroupID;
GO

-- 26. Menyu elementləri (Layout-suz)[cite: 3]
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
SELECT 
    MenuItems.AutoID, MenuItems.MenuItemID, ISNULL(MenuItems.MainMenuItemID, 0) AS MainMenuItemID, 
    MenuItems.RevenueCenterTypeID, MenuItems.MenuItemText, MenuItems.MenuCategoryID, MenuItems.MenuGroupID, 
    MenuItems.DisplayIndex AS DisplayIndex, MenuItems.DefaultUnitPrice, MenuItems.MenuItemCost, 
    MenuItems.MenuItemDescription, MenuItems.MenuItemNotification, MenuItems.MenuItemActive, 
    MenuItems.MenuItemInStock, MenuItems.MenuItemTaxable, ISNULL(TaxGroups.TaxRate, 8) AS TaxPercent, 
    MenuItems.MenuModifierID, MenuItems.MenuItemDiscountable, MenuItems.MenuItemPopUpHeaderID, 
    MenuItems.MenuItemPopUpChoiceText, MenuItems.HasModifierPopUps, 
    ISNULL(MenuItems.SecLangMenuItemText, '') AS SecLangMenuItemText, MenuItems.SecLangPopUpChoiceText, 
    MenuItems.PictureName, MenuItems.ShowCaption, ISNULL(MenuItems.IsComboMenu, 0) AS IsComboMenu, 
    ISNULL(MenuItems.IsTopMenu, 0) AS IsTopMenu, MenuItems.ButtonColor, ISNULL(MenuItems.Barcode, '') AS Barcode, 
    ISNULL(MenuItems.Barcode2, '') AS Barcode2, MenuItems.ItemDelCharge, MenuItems.ItemDelComp, 
    MenuItems.DineInPrice, MenuItems.BarTabPrice, MenuItems.TakeOutPrice, MenuItems.DriveThruPrice, 
    MenuItems.DeliveryPrice, MenuItems.OrderByWeight, MenuItems.PrintPizzaLabel, MenuItems.KitchenSortNumber, 
    MenuItems.ModBuilderTemplateID, MenuItems.MenuItemTypeID, ISNULL(MenuItems.AccountingCode, '') AS AccountingCode, 
    ISNULL(MenuItems.PrintOnLabel, 0) AS PrintOnLabel, MenuItems.UsedPrinterID1, MenuItems.UsedPrinterID2, 
    MenuItems.UsedPrinterID3, MenuItems.UsedPrinterID4, MenuItems.UsedPrinterID5, MenuItems.SecurityLevel, 
    MenuItems.DeleteReason, MenuItems.CustomField1, MenuItems.CustomField2, MenuItems.CustomField3, 
    MenuItems.CustomField4, MenuItems.CustomField5, MenuItems.EditKey, MenuItems.SyncKey, MenuItems.BranchID, 
    MenuItems.AddUserID, MenuItems.AddDateTime, MenuItems.EditUserID, 
    ISNULL(MenuItems.UseKds1, 0) AS UseKds1, ISNULL(MenuItems.UseKds2, 0) AS UseKds2, 
    ISNULL(MenuItems.UseKds3, 0) AS UseKds3, ISNULL(MenuItems.UseKds4, 0) AS UseKds4, 
    ISNULL(MenuItems.UseKds5, 0) AS UseKds5, ISNULL(MenuItems.UseKds6, 0) AS UseKds6, 
    ISNULL(MenuItems.UseKds7, 0) AS UseKds7, ISNULL(MenuItems.UseKds8, 0) AS UseKds8, 
    ISNULL(MenuItems.UseKds9, 0) AS UseKds9, ISNULL(MenuItems.UseKds10, 0) AS UseKds10, 
    MenuItems.CountDownDate, ISNULL(MenuItems.CountDownValue, 0.0) AS CountDownValue, 
    ISNULL(MenuItems.CountDownActualResult, 0.0) AS CountDownActualResult, MenuItems.EditDateTime, 
    MenuCategories.MenuCategoryText, MenuSubCategories.MenuSubCategoryText, TaxGroups.GroupName AS TaxGroupText, 
    MenuModifierGroups.MenuModifierGroupText AS MenuModifierText, MenuItems.DisplayIndex AS MenuDisplayIndex, 
    MenuItems.MenuItemKey, MenuItems.MainMenuItemKey, MenuItems.MenuItemGlobalKey, MenuItems.MenuCategoryKey, 
    MenuItems.MenuGroupKey, MenuItems.TaxGroupID, MenuItems.TaxGroupKey, MenuItems.MenuModifierKey, 
    MenuItems.MenuModifierForcedID, MenuItems.MenuModifierForcedKey, 
    MenuForcedModifierGroups.MenuModifierGroupText AS MenuForcedModifierText, efr_Branchs.BranchName 
FROM MenuItems 
LEFT OUTER JOIN MenuModifierGroups AS MenuForcedModifierGroups ON MenuItems.MenuModifierForcedKey = MenuForcedModifierGroups.MenuModifierGroupKey 
LEFT OUTER JOIN MenuModifierGroups ON MenuItems.MenuModifierKey = MenuModifierGroups.MenuModifierGroupKey 
LEFT OUTER JOIN MenuSubCategories ON MenuItems.MenuGroupKey = MenuSubCategories.MenuSubCategoryKey 
LEFT OUTER JOIN MenuCategories ON MenuItems.MenuCategoryKey = MenuCategories.MenuCategoryKey 
LEFT OUTER JOIN TaxGroups ON MenuItems.TaxGroupID = TaxGroups.TaxGroupID  
LEFT OUTER JOIN efr_Branchs ON MenuItems.BranchID = efr_Branchs.BranchID;
GO

-- 27. Menyu elementləri (Layout ilə)[cite: 3]
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
SELECT 
    MenuItems.AutoID, MenuItems.MenuItemID, ISNULL(MenuItemLayout.MainMenuItemID, 0) AS MainMenuItemID, 
    MenuItems.RevenueCenterTypeID, MenuItems.MenuItemText, MenuItems.MenuCategoryID, MenuItems.MenuGroupID, 
    MenuItems.DisplayIndex AS DisplayIndex2, MenuItems.DefaultUnitPrice, MenuItems.MenuItemCost, 
    MenuItems.MenuItemDescription, MenuItems.MenuItemNotification, MenuItems.MenuItemActive, 
    MenuItems.MenuItemInStock, MenuItems.MenuItemTaxable, ISNULL(TaxGroups.TaxRate, 8) AS TaxPercent, 
    MenuItems.MenuModifierID, MenuItems.MenuItemDiscountable, MenuItems.MenuItemPopUpHeaderID, 
    MenuItems.MenuItemPopUpChoiceText, MenuItems.HasModifierPopUps, 
    ISNULL(MenuItems.SecLangMenuItemText, '') AS SecLangMenuItemText, MenuItems.SecLangPopUpChoiceText, 
    MenuItems.PictureName, MenuItems.ShowCaption, ISNULL(MenuItems.IsComboMenu, 0) AS IsComboMenu, 
    ISNULL(MenuItems.IsTopMenu, 0) AS IsTopMenu, MenuItems.ButtonColor, ISNULL(MenuItems.Barcode, '') AS Barcode, 
    ISNULL(MenuItems.Barcode2, '') AS Barcode2, MenuItems.ItemDelCharge, MenuItems.ItemDelComp, 
    MenuItems.DineInPrice, MenuItems.BarTabPrice, MenuItems.TakeOutPrice, MenuItems.DriveThruPrice, 
    MenuItems.DeliveryPrice, MenuItems.OrderByWeight, MenuItems.PrintPizzaLabel, MenuItems.KitchenSortNumber, 
    MenuItems.ModBuilderTemplateID, MenuItems.MenuItemTypeID, ISNULL(MenuItems.AccountingCode, '') AS AccountingCode, 
    ISNULL(MenuItems.PrintOnLabel, 0) AS PrintOnLabel, MenuItems.UsedPrinterID1, MenuItems.UsedPrinterID2, 
    MenuItems.UsedPrinterID3, MenuItems.UsedPrinterID4, MenuItems.UsedPrinterID5, MenuItems.SecurityLevel, 
    MenuItems.DeleteReason, MenuItems.CustomField1, MenuItems.CustomField2, MenuItems.CustomField3, 
    MenuItems.CustomField4, MenuItems.CustomField5, MenuItems.EditKey, MenuItems.SyncKey, MenuItems.BranchID, 
    MenuItems.AddUserID, MenuItems.AddDateTime, MenuItems.EditUserID, 
    ISNULL(MenuItems.UseKds1, 0) AS UseKds1, ISNULL(MenuItems.UseKds2, 0) AS UseKds2, 
    ISNULL(MenuItems.UseKds3, 0) AS UseKds3, ISNULL(MenuItems.UseKds4, 0) AS UseKds4, 
    ISNULL(MenuItems.UseKds5, 0) AS UseKds5, ISNULL(MenuItems.UseKds6, 0) AS UseKds6, 
    ISNULL(MenuItems.UseKds7, 0) AS UseKds7, ISNULL(MenuItems.UseKds8, 0) AS UseKds8, 
    ISNULL(MenuItems.UseKds9, 0) AS UseKds9, ISNULL(MenuItems.UseKds10, 0) AS UseKds10, 
    MenuItems.CountDownDate, ISNULL(MenuItems.CountDownValue, 0.0) AS CountDownValue, 
    ISNULL(MenuItems.CountDownActualResult, 0.0) AS CountDownActualResult, MenuItems.EditDateTime, 
    MenuCategories.MenuCategoryText, MenuSubCategories.MenuSubCategoryText, TaxGroups.GroupName AS TaxGroupText, 
    MenuModifierGroups.MenuModifierGroupText AS MenuModifierText, MenuItems.DisplayIndex AS MenuDisplayIndex, 
    MenuItems.MenuItemKey, MenuItemLayout.MainMenuItemKey, MenuItems.MenuItemGlobalKey, 
    MenuItems.MenuCategoryKey, MenuItems.MenuGroupKey, MenuItems.TaxGroupID, MenuItems.TaxGroupKey, 
    MenuItems.MenuModifierKey, MenuItems.MenuModifierForcedID, MenuItems.MenuModifierForcedKey, 
    MenuForcedModifierGroups.MenuModifierGroupText AS MenuForcedModifierText, efr_Branchs.BranchName, 
    MenuItemLayout.DisplayIndex AS DisplayIndex, MenuItemLayout.MenuGroupID AS MenuScreenGroupID, 
    MenuItemLayout.MenuGroupKey AS MenuScreenGroupKey 
FROM MenuItems  
LEFT OUTER JOIN MenuModifierGroups AS MenuForcedModifierGroups ON MenuItems.MenuModifierForcedKey = MenuForcedModifierGroups.MenuModifierGroupKey 
LEFT OUTER JOIN MenuModifierGroups ON MenuItems.MenuModifierKey = MenuModifierGroups.MenuModifierGroupKey 
LEFT OUTER JOIN MenuSubCategories ON MenuItems.MenuGroupKey = MenuSubCategories.MenuSubCategoryKey 
LEFT OUTER JOIN MenuCategories ON MenuItems.MenuCategoryKey = MenuCategories.MenuCategoryKey 
LEFT OUTER JOIN TaxGroups ON MenuItems.TaxGroupID = TaxGroups.TaxGroupID  
LEFT OUTER JOIN efr_Branchs ON MenuItems.BranchID = efr_Branchs.BranchID 
INNER JOIN MenuItemLayout ON MenuItemLayout.MenuItemKey = MenuItems.MenuItemKey  
WHERE ISNULL(MenuItems.MenuItemActive, 0) = 1 
ORDER BY MenuItems.MenuItemText;
GO

-- 28. Stansiya printer sazlamaları[cite: 3]
SELECT COUNT(AutoID) AS countValue FROM StationPrinterSettings WHERE StationID = 1;

SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
SELECT 
    AutoID, StationID, CheckPrinterName, CheckAltPrinterName, CheckPrinterID, CheckDesignPath, 
    DeliveryPrinterName, DeliveryAltPrinterName, DeliveryPrinterID, DeliveryDesignPath, 
    InvoicePrinterName, InvoiceAltPrinterName, InvoicePrinterID, InvoiceDesignPath, InvoiceRowCount, 
    ReportPrinterName, ReportAltPrinterName, ReportPrinterID, ReportDesignPath, AdditionPrinterName, 
    AdditionPrinterID, AdditionDesignPath, AdditionRowCount, ReportA4PrinterName, ReportA4AltPrinterName, 
    ReportA4PrinterID, ReportA4DesignPath, Kitchen1PrinterName, Kitchen1AltPrinterName, Kitchen1PrinterID, 
    Kitchen1DesignPath, Kitchen2PrinterName, Kitchen2AltPrinterName, Kitchen2PrinterID, Kitchen2DesignPath, 
    Kitchen3PrinterName, Kitchen3AltPrinterName, Kitchen3PrinterID, Kitchen3DesignPath, Kitchen4PrinterName, 
    Kitchen4AltPrinterName, Kitchen4PrinterID, Kitchen4DesignPath, Kitchen5PrinterName, Kitchen5AltPrinterName, 
    Kitchen5PrinterID, Kitchen5DesignPath, Kitchen6PrinterName, Kitchen6AltPrinterName, Kitchen6PrinterID, 
    Kitchen6DesignPath, Kitchen7PrinterName, Kitchen7AltPrinterName, Kitchen7PrinterID, Kitchen7DesignPath, 
    Kitchen8PrinterName, Kitchen8AltPrinterName, Kitchen8PrinterID, Kitchen8DesignPath, Kitchen9PrinterName, 
    Kitchen9AltPrinterName, Kitchen9PrinterID, Kitchen9DesignPath, Kitchen10PrinterName, Kitchen10AltPrinterName, 
    Kitchen10PrinterID, Kitchen10DesignPath, Kitchen11PrinterName, Kitchen11AltPrinterName, Kitchen11PrinterID, 
    Kitchen11DesignPath, Kitchen12PrinterName, Kitchen12AltPrinterName, Kitchen12PrinterID, Kitchen12DesignPath, 
    Kitchen13PrinterName, Kitchen13AltPrinterName, Kitchen13PrinterID, Kitchen13DesignPath, Kitchen14PrinterName, 
    Kitchen14AltPrinterName, Kitchen14PrinterID, Kitchen14DesignPath, Kitchen15PrinterName, Kitchen15AltPrinterName, 
    Kitchen15PrinterID, Kitchen15DesignPath, Kitchen16PrinterName, Kitchen16AltPrinterName, Kitchen16PrinterID, 
    Kitchen16DesignPath, Kitchen17PrinterName, Kitchen17AltPrinterName, Kitchen17PrinterID, Kitchen17DesignPath, 
    Kitchen18PrinterName, Kitchen18AltPrinterName, Kitchen18PrinterID, Kitchen18DesignPath, Kitchen19PrinterName, 
    Kitchen19AltPrinterName, Kitchen19PrinterID, Kitchen19DesignPath, Kitchen20PrinterName, Kitchen20AltPrinterName, 
    Kitchen20PrinterID, Kitchen20DesignPath, InvoiceTopFeed, AdditionTopFeed, 
    ISNULL(PrintDineInOrdersKitchen, 1) AS PrintDineInOrdersKitchen, 
    ISNULL(PrintBarTableOrdersKitchen, 1) AS PrintBarTableOrdersKitchen, 
    ISNULL(PrintTakeOutOrdersKitchen, 1) AS PrintTakeOutOrdersKitchen, 
    ISNULL(PrintDriveThruOrdersKitchen, 1) AS PrintDriveThruOrdersKitchen, 
    ISNULL(PrintDeliveryOrdersKitchen, 1) AS PrintDeliveryOrdersKitchen, 
    [EditKey], [SyncKey], [LabelPrinterID], [LabelPrinterName], [LabelDesignPath], 
    [ReturnPrinterID], [ReturnPrinterName], [ReturnDesignPath] 
FROM StationPrinterSettings  
WHERE StationID = 1;
GO

-- 29. Menyu son yenilənmə tarixi yoxlanışı[cite: 3]
SELECT TOP 1 ISNULL(r.lastDate, '1/1/2013') AS LastDate 
FROM ( 
    SELECT ISNULL(MAX(EditDateTime), '1/1/2013') AS lastDate FROM MenuItems 
    UNION ALL 
    SELECT ISNULL(MAX(AddDateTime), '1/1/2013') AS lastDate FROM MenuItems 
) AS r 
ORDER BY r.lastDate DESC;
GO

-- 30. Ödəniş üsulları siyahısı[cite: 3]
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
SELECT 
    [AutoID], [PaymentMethodID], [PaymentMethodKey], [PaymentName], [IsDefault], 
    [DenyInvoice], [DenyFiscal], [DenyMoneyChange], [ExchangeRate], [AccountingCode], 
    [SecurityLevel], [IsLocked], [PaymentMethodActive], ISNULL(EffectRegister, 1) AS [EffectRegister], 
    [IsCoupon], [IsAccountPayment], [IsAccountSale], [HideInRecievePayement], [PictureName], 
    [DisplayIndex], [ButtonColor], [EditKey], [SyncKey], ISNULL([ForcedInvoice], 0) AS ForcedInvoice, 
    ISNULL([IsCampusCard], 0) AS IsCampusCard, [CampusCardServer], [BranchID], 
    ISNULL([AskCustomerName], 0) AS AskCustomerName, ISNULL([GlobalBankCode], 0) AS GlobalBankCode, 
    ISNULL(PaymentTypeID, 2) AS PaymentTypeID, 
    CASE PaymentTypeID 
        WHEN 0 THEN ''  
        WHEN 1 THEN 'Nakit' 
        WHEN 2 THEN 'Kredi Kartı' 
        WHEN 3 THEN 'Yemek Çeki'  
        WHEN 4 THEN 'Cari Hesap' 
        WHEN 5 THEN 'Para Puan' 
        ELSE '' 
    END AS PaymentTypeName, 
    ISNULL(CustomField1, '') AS CustomField1, 
    ISNULL(CustomField2, '') AS CustomField2, 
    ISNULL(CustomField3, '') AS CustomField3, 
    ISNULL(CustomField4, '') AS CustomField4, 
    ISNULL(CustomField5, '') AS CustomField5 
FROM [PaymentMethods]  
ORDER BY PaymentMethodID;
GO