-- 1. Cari stansiya üzrə açıq sessiyanın yoxlanılması
EXEC sp_executesql 
    N'SELECT COUNT(RegisterSessionID) AS countValue FROM RegisterSessions WHERE StationID=@StationID AND SignOutDateTime IS NULL',
    N'@StationID int',
    @StationID = 1;
GO

-- 2. Bütün səlahiyyətlər siyahısının oxunması
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
SELECT 
    [AuthorityID], 
    [GroupName], 
    [AuthorityKey], 
    [AuthorityText], 
    [AuthorityDescription], 
    [DefaultLevel], 
    [DefaultLevel] AS OldLevel, 
    ISNULL([NeedAllways], 0) AS NeedAllways, 
    ISNULL([NeedAllways], 0) AS OldNeedAllways, 
    ISNULL([AllowManager], 0) AS AllowManager, 
    ISNULL([AllowManager], 0) AS OldAllowManager,
    ISNULL(AllowCashier, 0) AS AllowCashier,
    ISNULL(AllowCashier, 0) AS oldAllowCashier,
    [EditKey], 
    [SyncKey], 
    [BranchID] 
FROM [AuthorityList];
GO

-- 3. İşçi məlumatlarının şifrə/kart kodu ilə yoxlanılması
EXEC sp_executesql 
    N'SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
    SELECT EmployeeFiles.AutoID, EmployeeFiles.EmployeeKey, EmployeeFiles.EmployeeID, EmployeeFiles.FirstName, EmployeeFiles.LastName,  
     EmployeeFiles.SocialSecurityNumber, EmployeeFiles.SmarCardCode, EmployeeFiles.MifareCardCode, EmployeeFiles.MailingAddress, EmployeeFiles.MailingZipCode, EmployeeFiles.DateHired, EmployeeFiles.DateReleased,  
     EmployeeFiles.EmployeeActive, EmployeeFiles.JobTitleID, EmployeeFiles.SecurityLevel, EmployeeFiles.AccessCode, EmployeeFiles.TipsReceived,  
     EmployeeFiles.PayBasis, EmployeeFiles.PayRate, EmployeeFiles.ScanCode, isnull(EmployeeFiles.DriverLicenseNumber,'''') as DriverLicenseNumber, EmployeeFiles.DriverLicenseExpires,  
     EmployeeFiles.CarInsurancePolicyCarrier, EmployeeFiles.CarInsurancePolicyNumber, EmployeeFiles.CarInsurancePolicyExpires,  
     EmployeeFiles.CarInsurancePolicyNotes, isnull(EmployeeFiles.PrefUserInterfaceLocale,''-'') as PrefUserInterfaceLocale, EmployeeFiles.EmployeeNotes, EmployeeFiles.OrderEntryUseSecLang,  
     EmployeeFiles.EmployeeIsDriver, EmployeeFiles.DefaultOEMenuGroupID, EmployeeFiles.UseStaffBank, EmployeeFiles.ScheduleNotEnforced,  
     EmployeeFiles.UseHostess, EmployeeFiles.IsAServer, EmployeeFiles.IsOffline, EmployeeFiles.NoCashierOut, EmployeeFiles.EditTimestamp,  
     EmployeeFiles.PhoneNumber, EmployeeFiles.RevenueCenterTypeID, EmployeeFiles.DeleteReason, EmployeeFiles.CustomField1, EmployeeFiles.CustomField2,  
     EmployeeFiles.CustomField3, EmployeeFiles.CustomField4, EmployeeFiles.CustomField5, EmployeeFiles.EditKey, EmployeeFiles.SyncKey,  
     EmployeeFiles.BranchID, EmployeeFiles.AddUserID, EmployeeFiles.AddDateTime, EmployeeFiles.EditUserID, EmployeeFiles.EditDateTime,  
     EmployeeTitles.TitleName as JobTitleText, isnull(EmployeeFiles.MonthlyDinnerFee,0) as MonthlyDinnerFee,CAST(0 AS BIT) AS IsChecked 
    FROM EmployeeFiles  
    LEFT OUTER JOIN EmployeeTitles ON EmployeeFiles.JobTitleID = EmployeeTitles.TitleID  
    WHERE (EmployeeFiles.AccessCode=@password OR EmployeeFiles.MifareCardCode=@password OR EmployeeFiles.SmarCardCode=@password) 
      AND ISNULL(EmployeeFiles.EmployeeActive,0)=1',
    N'@password nvarchar(6)',
    @password = N'106510';
GO

-- 4. Oxunmamış istifadəçi mesajlarının yoxlanılması
SELECT ISNULL((
    SELECT TOP 1 CAST(MessageKey AS NVARCHAR(50)) 
    FROM UserMessages 
    WHERE Reciepments LIKE +'%,106,%' 
      AND ISNULL(Readed, '') NOT LIKE +'%,106,%'
), '-');
GO

-- 5. Giriş qeydinin yazılması (Kassir sessiyasının açılması)
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
      @ActionName nvarchar(24), @WrongPassword nvarchar(6), @AdditionalInfo nvarchar(4000), 
      @IsSuccess bit, @OrderKey uniqueidentifier, @TransactionKey uniqueidentifier, 
      @AccessLogKey uniqueidentifier, @EditKey uniqueidentifier, @SyncKey uniqueidentifier',
    @BranchID = 422,
    @LogDate = '2026-09-05 19:07:11.373',
    @StationID = 1,
    @EmployeeID = 106,
    @ActionName = N'KASSİR SESSİYASINI AÇMAQ',
    @WrongPassword = N'106510',
    @AdditionalInfo = NULL,
    @IsSuccess = 1,
    @OrderKey = '00000000-0000-0000-0000-000000000000',
    @TransactionKey = '00000000-0000-0000-0000-000000000000',
    @AccessLogKey = NULL,
    @EditKey = 'D752DB3C-9498-4D58-AD04-76F43B59F581',
    @SyncKey = '131B047E-2366-4AD5-9696-DC173601C21B';
GO

-- 6. Server vaxtının alınması
SELECT GETDATE() AS serverDatetime;
GO

-- 7. Yeni kassir sessiyasının yaradılması (RegisterSessions Insert)
EXEC sp_executesql 
    N'INSERT INTO [RegisterSessions] (
        [BranchID], [EmployeeID], [RegisterSessionKey], [StationID], [AccountingDateTime], [SignInDateTime], 
        [RegisterStartAmount], [SignOutDateTime], [RegisterEndAmount], [DiscrepancyAmount], [DiscrepancyNotes], 
        [DiscrepancyNotes2], [ManagerEmployeeID], [TotalPaymentMethod1], [TotalPaymentMethod2], [TotalPaymentMethod3], 
        [TotalPaymentMethod4], [TotalPaymentMethod5], [TotalPaymentMethod6], [TotalPaymentMethod7], [TotalPaymentMethod8], 
        [TotalPaymentMethod9], [TotalPaymentMethod10], [TotalPaymentMethod11], [TotalPaymentMethod12], [TotalPaymentMethod13], 
        [TotalPaymentMethod14], [TotalPaymentMethod15], [TotalPaymentMethod16], [TotalPaymentMethod17], [TotalPaymentMethod18], 
        [TotalPaymentMethod19], [TotalPaymentMethod20], [ClosePaymentMethod1], [ClosePaymentMethod2], [ClosePaymentMethod3], 
        [ClosePaymentMethod4], [ClosePaymentMethod5], [ClosePaymentMethod6], [ClosePaymentMethod7], [ClosePaymentMethod8], 
        [ClosePaymentMethod9], [ClosePaymentMethod10], [ClosePaymentMethod11], [ClosePaymentMethod12], [ClosePaymentMethod13], 
        [ClosePaymentMethod14], [ClosePaymentMethod15], [ClosePaymentMethod16], [ClosePaymentMethod17], [ClosePaymentMethod18], 
        [ClosePaymentMethod19], [ClosePaymentMethod20], [ZReportID], [EditKey], [SyncKey], [EmployeeKey],
        [PaymentMethodName1], [PaymentMethodName2], [PaymentMethodName3], [PaymentMethodName4], [PaymentMethodName5], 
        [PaymentMethodName6], [PaymentMethodName7], [PaymentMethodName8], [PaymentMethodName9], [PaymentMethodName10], 
        [PaymentMethodName11], [PaymentMethodName12], [PaymentMethodName13], [PaymentMethodName14], [PaymentMethodName15], 
        [PaymentMethodName16], [PaymentMethodName17], [PaymentMethodName18], [PaymentMethodName19], [PaymentMethodName20]
    ) VALUES (
        @BranchID, @EmployeeID, @RegisterSessionKey, @StationID, @AccountingDateTime, GETDATE(), 
        @RegisterStartAmount, NULL, @RegisterEndAmount, @DiscrepancyAmount, @DiscrepancyNotes, 
        @DiscrepancyNotes2, @ManagerEmployeeID, @TotalPaymentMethod1, @TotalPaymentMethod2, @TotalPaymentMethod3, 
        @TotalPaymentMethod4, @TotalPaymentMethod5, @TotalPaymentMethod6, @TotalPaymentMethod7, @TotalPaymentMethod8, 
        @TotalPaymentMethod9, @TotalPaymentMethod10, @TotalPaymentMethod11, @TotalPaymentMethod12, @TotalPaymentMethod13, 
        @TotalPaymentMethod14, @TotalPaymentMethod15, @TotalPaymentMethod16, @TotalPaymentMethod17, @TotalPaymentMethod18, 
        @TotalPaymentMethod19, @TotalPaymentMethod20, @ClosePaymentMethod1, @ClosePaymentMethod2, @ClosePaymentMethod3, 
        @ClosePaymentMethod4, @ClosePaymentMethod5, @ClosePaymentMethod6, @ClosePaymentMethod7, @ClosePaymentMethod8, 
        @ClosePaymentMethod9, @ClosePaymentMethod10, @ClosePaymentMethod11, @ClosePaymentMethod12, @ClosePaymentMethod13, 
        @ClosePaymentMethod14, @ClosePaymentMethod15, @ClosePaymentMethod16, @ClosePaymentMethod17, @ClosePaymentMethod18, 
        @ClosePaymentMethod19, @ClosePaymentMethod20, @ZReportID, @EditKey, @SyncKey, @EmployeeKey,
        @PaymentMethodName1, @PaymentMethodName2, @PaymentMethodName3, @PaymentMethodName4, @PaymentMethodName5, 
        @PaymentMethodName6, @PaymentMethodName7, @PaymentMethodName8, @PaymentMethodName9, @PaymentMethodName10, 
        @PaymentMethodName11, @PaymentMethodName12, @PaymentMethodName13, @PaymentMethodName14, @PaymentMethodName15, 
        @PaymentMethodName16, @PaymentMethodName17, @PaymentMethodName18, @PaymentMethodName19, @PaymentMethodName20
    )',
    N'@BranchID int, @EmployeeID int, @EmployeeKey uniqueidentifier, @RegisterSessionKey uniqueidentifier, @StationID int, 
      @AccountingDateTime datetime, @SignInDateTime datetime, @RegisterStartAmount float, @SignOutDateTime datetime, 
      @RegisterEndAmount float, @DiscrepancyAmount float, @DiscrepancyNotes nvarchar(4000), @DiscrepancyNotes2 nvarchar(4000), 
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
      @PaymentMethodName1 nvarchar(4000), @PaymentMethodName2 nvarchar(4000), @PaymentMethodName3 nvarchar(4000), 
      @PaymentMethodName4 nvarchar(4000), @PaymentMethodName5 nvarchar(4000), @PaymentMethodName6 nvarchar(4000), 
      @PaymentMethodName7 nvarchar(4000), @PaymentMethodName8 nvarchar(4000), @PaymentMethodName9 nvarchar(4000), 
      @PaymentMethodName10 nvarchar(4000), @PaymentMethodName11 nvarchar(4000), @PaymentMethodName12 nvarchar(4000), 
      @PaymentMethodName13 nvarchar(4000), @PaymentMethodName14 nvarchar(4000), @PaymentMethodName15 nvarchar(4000), 
      @PaymentMethodName16 nvarchar(4000), @PaymentMethodName17 nvarchar(4000), @PaymentMethodName18 nvarchar(4000), 
      @PaymentMethodName19 nvarchar(4000), @PaymentMethodName20 nvarchar(4000)',
    @BranchID = 422,
    @EmployeeID = 106,
    @EmployeeKey = '4740D379-BF42-4288-BF67-4B46D795F77D',
    @RegisterSessionKey = '583075B1-95C2-4B6C-A8E3-B56E9EC0152F',
    @StationID = 1,
    @AccountingDateTime = '2026-09-05 00:00:00',
    @SignInDateTime = '2026-09-05 19:07:38.370',
    @RegisterStartAmount = 0,
    @SignOutDateTime = NULL,
    @RegisterEndAmount = 0,
    @DiscrepancyAmount = 0,
    @DiscrepancyNotes = NULL,
    @DiscrepancyNotes2 = NULL,
    @ManagerEmployeeID = 0,
    @TotalPaymentMethod1 = 0, @TotalPaymentMethod2 = 0, @TotalPaymentMethod3 = 0, @TotalPaymentMethod4 = 0, @TotalPaymentMethod5 = 0, 
    @TotalPaymentMethod6 = 0, @TotalPaymentMethod7 = 0, @TotalPaymentMethod8 = 0, @TotalPaymentMethod9 = 0, @TotalPaymentMethod10 = 0, 
    @TotalPaymentMethod11 = 0, @TotalPaymentMethod12 = 0, @TotalPaymentMethod13 = 0, @TotalPaymentMethod14 = 0, @TotalPaymentMethod15 = 0, 
    @TotalPaymentMethod16 = 0, @TotalPaymentMethod17 = 0, @TotalPaymentMethod18 = 0, @TotalPaymentMethod19 = 0, @TotalPaymentMethod20 = 0, 
    @ClosePaymentMethod1 = 0, @ClosePaymentMethod2 = 0, @ClosePaymentMethod3 = 0, @ClosePaymentMethod4 = 0, @ClosePaymentMethod5 = 0, 
    @ClosePaymentMethod6 = 0, @ClosePaymentMethod7 = 0, @ClosePaymentMethod8 = 0, @ClosePaymentMethod9 = 0, @ClosePaymentMethod10 = 0, 
    @ClosePaymentMethod11 = 0, @ClosePaymentMethod12 = 0, @ClosePaymentMethod13 = 0, @ClosePaymentMethod14 = 0, @ClosePaymentMethod15 = 0, 
    @ClosePaymentMethod16 = 0, @ClosePaymentMethod17 = 0, @ClosePaymentMethod18 = 0, @ClosePaymentMethod19 = 0, @ClosePaymentMethod20 = 0, 
    @ZReportID = 0,
    @EditKey = '2F2C2030-9943-45A7-81DC-2D552A320886',
    @SyncKey = '00000000-0000-0000-0000-000000000000',
    @PaymentMethodName1 = NULL, @PaymentMethodName2 = NULL, @PaymentMethodName3 = NULL, @PaymentMethodName4 = NULL, @PaymentMethodName5 = NULL, 
    @PaymentMethodName6 = NULL, @PaymentMethodName7 = NULL, @PaymentMethodName8 = NULL, @PaymentMethodName9 = NULL, @PaymentMethodName10 = NULL, 
    @PaymentMethodName11 = NULL, @PaymentMethodName12 = NULL, @PaymentMethodName13 = NULL, @PaymentMethodName14 = NULL, @PaymentMethodName15 = NULL, 
    @PaymentMethodName16 = NULL, @PaymentMethodName17 = NULL, @PaymentMethodName18 = NULL, @PaymentMethodName19 = NULL, @PaymentMethodName20 = NULL;
GO

-- 8. Cari aktiv sessiyanın ətraflı məlumatları
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
WHERE RegisterSessions.SignOutDateTime IS NULL 
  AND RegisterSessions.StationID = 1;
GO

-- 9. Şəbəkə üzrə bütün açıq sessiyaların sayı
SELECT COUNT(RegisterSessionID) AS countValue 
FROM RegisterSessions 
WHERE SignOutDateTime IS NULL;
GO

-- 10. Filial ID və Server Soket URL oxunması
SELECT ISNULL((SELECT TOP 1 ParamValue FROM StoreSettings WHERE ParamKey = 'BranchID'), '0') AS BranchID;
SELECT ISNULL((SELECT TOP 1 ParamValue FROM StoreSettings WHERE ParamKey = 'ServerSocketUrl'), '0') AS ServerSocketUrl;
GO

-- 11. Baza tamamlama və təmizləmə skriptləri
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

-- 12. Mağaza parametrlərinin oxunması (StoreSettings)
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
SELECT 
    [SettingsID], [OrderID], [TabName], [GroupName], [ParamName], [ParamKey], 
    [ParamValue], [DefaultValue], [ParamType], [EditKey], [SyncKey], 
    [BranchID], [AddUserID], [AddDateTime], [EditUserID], [EditDateTime]  
FROM [StoreSettings] 
WHERE ParamKey <> 'StoreStatus' 
ORDER BY TabName, GroupName, OrderID;
GO

-- 13. Menyu dəyişiklik tarixinin yoxlanılması (Son yenilənmə)
SELECT TOP 1 ISNULL(r.lastDate, '1/1/2013') AS LastDate 
FROM ( 
    SELECT ISNULL(MAX(EditDateTime), '1/1/2013') AS lastDate FROM MenuItems 
    UNION ALL 
    SELECT ISNULL(MAX(AddDateTime), '1/1/2013') AS lastDate FROM MenuItems 
) AS r 
ORDER BY r.lastDate DESC;
GO

-- 14. Ödəniş üsulları (PaymentMethods)
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

-- 15. Menyu elementlərinin (MenuItems + Layout) oxunması
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

-- 16. Müştəri kart və ad sinxronizasiyası (CustomerFiles Update)
UPDATE CustomerFiles 
SET CustomerName = EmployeeFiles.FirstName + ISNULL(NULLIF((' ' + ISNULL(EmployeeFiles.LastName, ' ')), ''), ''), 
    CustomerIsActive = EmployeeFiles.EmployeeActive, 
    CardNumber = EmployeeFiles.SmarCardCode 
FROM CustomerFiles 
INNER JOIN EmployeeFiles ON CustomerFiles.CustomerKey = EmployeeFiles.EmployeeKey;
GO

-- 17. Pərakəndə barkodların oxunması
SELECT 
    [AutoID], [MenuItemKey], 
    ISNULL([Barcode], '') AS [Barcode], 
    ISNULL([Quantity], 0) AS [Quantity], 
    ISNULL([SalePrice], 0) AS [SalePrice] 
FROM [GlobalRetailBarcodes];
GO

-- 18. Vergi dərəcələri (TaxGroups)
SELECT 
    [TaxGroupID], [GroupName], [TaxRate], [EditKey], [SyncKey] 
FROM [TaxGroups] 
ORDER BY TaxGroupID;
GO

-- 19. Ümumi menyu elementlərinin təkrar oxunması (Layout-suz)
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

-- 20. Stansiya sazlamaları (StationSettings)
SELECT COUNT(AutoID) AS countValue FROM StationSettings WHERE StationID = 1;

SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
SELECT 
    [AutoID], [StationID], [StationName], [AllowRegister], [DefineValue], [ActivateCashier], 
    [RememberCashier], [BackgroundPicture], [SideBarPicture], [DefaultLoginEntrance], 
    [DefultTablePlan], [SkinName], [ShowDineInButton], [ShowTakeOutButton], [ShowDriveThruButton], 
    [ShowDeliveryButton], [ShowRecallButton], [ShowDriverStatusButton], [ShowTimeCardButton], 
    [ShowOperationsButton], [ShowBackOfficeButton], [ShowQuickServiceDineIn], [ShowQuickServiceTakeOut], 
    [ShowQuickServiceDriveThru], [ShowQuickServiceDelivery], [StayInOrderScreenTakeOut], 
    [StayInOrderScreenDriveThru], [StayTablePlanInDineIn], [TimeOutScreenLock], [IsMobile], 
    [CustomerDisplayPortNo], [CustomerDisplayLine1], [CustomerDisplayLine2], [WeightScale1PortNo], 
    [WeightScale1BaudRate], [WeightScale2PortNo], [WeightScale2BaudRate], [CashRegisterModel], 
    [CashRegisterBaudRate], [CashRegisterDataDelay], [CashRegisterLineDelay], [CashRegisterStations], 
    [CashRegisterPortNo], [PaymentOverTime], [DirectOpenOrderInEditMode], [PrintVoidedLinesOnGuestCheck], 
    [StationKey], ISNULL([AskPasswordForReduce], 0) AS [AskPasswordForReduce], 
    ISNULL([MediaDisplayIsActive], 0) AS [MediaDisplayIsActive], [MediaDisplayOnWaiting], 
    [MediaDisplayOnSale], ISNULL([MediaDisplayClosedMessage], 'KASA KAPALIDIR') AS [MediaDisplayClosedMessage], 
    ISNULL([MediaDisplayMoneyOver], 'TEŞEKKÜR EDERİZ') AS [MediaDisplayMoneyOver], [EditKey], [SyncKey], 
    ISNULL(ShowCashTrayButton, '') AS ShowCashTrayButton, 
    ISNULL(DisableSaleOnOpenCashTray, '') AS DisableSaleOnOpenCashTray, 
    BekoPosDesign, BekoPosOutput, BekoPosDatabase, 
    ISNULL(EnableCallCenterClient, 0) AS EnableCallCenterClient, [CallCenterClientAddress], 
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
WHERE StationID = 1;
GO

-- 21. Printer sazlamaları (StationPrinterSettings)
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

-- 22. Masa kartları və Happy Hour sazlamaları
SELECT COUNT(*) AS countValue FROM OrderCards WITH(NOLOCK);

SELECT ISNULL((
    SELECT TOP 1 ISNULL(ParamValue, 'NO') 
    FROM StoreSettings 
    WHERE ParamKey = 'HappyHoursActive'
), 'NO') AS ParamValue;
GO
```[cite: 2]