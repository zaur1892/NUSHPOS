-- 1. İşçi məlumatlarının gətirilməsi (AutoID = 106)
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
SELECT 
    EmployeeFiles.AutoID, EmployeeFiles.EmployeeKey, EmployeeFiles.EmployeeID, EmployeeFiles.FirstName, EmployeeFiles.LastName,  
    EmployeeFiles.SocialSecurityNumber, EmployeeFiles.SmarCardCode, EmployeeFiles.MifareCardCode, EmployeeFiles.MailingAddress, 
    EmployeeFiles.MailingZipCode, EmployeeFiles.DateHired, EmployeeFiles.DateReleased, EmployeeFiles.EmployeeActive, 
    EmployeeFiles.JobTitleID, EmployeeFiles.SecurityLevel, EmployeeFiles.AccessCode, EmployeeFiles.TipsReceived,  
    EmployeeFiles.PayBasis, EmployeeFiles.PayRate, EmployeeFiles.ScanCode, 
    ISNULL(EmployeeFiles.DriverLicenseNumber, '') AS DriverLicenseNumber, EmployeeFiles.DriverLicenseExpires,  
    EmployeeFiles.CarInsurancePolicyCarrier, EmployeeFiles.CarInsurancePolicyNumber, EmployeeFiles.CarInsurancePolicyExpires,  
    EmployeeFiles.CarInsurancePolicyNotes, ISNULL(EmployeeFiles.PrefUserInterfaceLocale, '-') AS PrefUserInterfaceLocale, 
    EmployeeFiles.EmployeeNotes, EmployeeFiles.OrderEntryUseSecLang, EmployeeFiles.EmployeeIsDriver, 
    EmployeeFiles.DefaultOEMenuGroupID, EmployeeFiles.UseStaffBank, EmployeeFiles.ScheduleNotEnforced,  
    EmployeeFiles.UseHostess, EmployeeFiles.IsAServer, EmployeeFiles.IsOffline, EmployeeFiles.NoCashierOut, 
    EmployeeFiles.EditTimestamp, EmployeeFiles.PhoneNumber, EmployeeFiles.RevenueCenterTypeID, EmployeeFiles.DeleteReason, 
    EmployeeFiles.CustomField1, EmployeeFiles.CustomField2, EmployeeFiles.CustomField3, EmployeeFiles.CustomField4, 
    EmployeeFiles.CustomField5, EmployeeFiles.EditKey, EmployeeFiles.SyncKey, EmployeeFiles.BranchID, EmployeeFiles.AddUserID, 
    EmployeeFiles.AddDateTime, EmployeeFiles.EditUserID, EmployeeFiles.EditDateTime, EmployeeTitles.TitleName AS JobTitleText, 
    ISNULL(EmployeeFiles.MonthlyDinnerFee, 0) AS MonthlyDinnerFee, CAST(0 AS BIT) AS IsChecked 
FROM EmployeeFiles  
LEFT OUTER JOIN EmployeeTitles ON EmployeeFiles.JobTitleID = EmployeeTitles.TitleID  
WHERE EmployeeFiles.AutoID = 106;

-- 2. Müştərinin telefon nömrəsi ilə mövcudluq yoxlanışı
SELECT ISNULL(
    (SELECT COUNT(DISTINCT CustomerID) AS CustomerID 
     FROM [CustomerPhones]  
     WHERE PhoneNumber = '0553946829'), 0
) AS CustomerID;

-- 3. Telefon nömrəsinə görə müştəri axtarışı
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
SELECT 
    CustomerFiles.WebUserName, CustomerFiles.WebPassword, CustomerFiles.AutoID, CustomerFiles.CustomerID, 
    CustomerFiles.CustomerKey, CustomerFiles.CustomerGlobalKey, CustomerFiles.CustomerIsActive, CustomerFiles.CustomerName, 
    CustomerFiles.CustomerFullName, CustomerFiles.CardNumber, CustomerFiles.CustomerNotes, CustomerFiles.OrderCount, 
    CustomerFiles.LastCallDate, CustomerFiles.CallingCount, CustomerFiles.AllowHouseAccount, CustomerFiles.IsFrequentDiner, 
    CustomerFiles.CreditLimit, CustomerFiles.CreditSatusID, CustomerFiles.DiscountPercent, CustomerFiles.SpecialBonusPercent, 
    CustomerFiles.TotalDebt, CustomerFiles.TotalPayment, CustomerFiles.TotalRemainig, 
    ISNULL(CustomerFiles.BonusStartupValue, 0) AS BonusStartupValue, 
    ISNULL(CustomerFiles.TotalBonusUsed, 0) AS TotalBonusUsed, 
    ISNULL(CustomerFiles.TotalBonusEarned, 0) AS TotalBonusEarned, 
    ISNULL(CustomerFiles.TotalBonusRemaing, 0) AS TotalBonusRemaing, 
    CustomerFiles.CityName, CustomerFiles.District, CustomerFiles.Neighborhood, CustomerFiles.Avenue, CustomerFiles.Street, 
    CustomerFiles.Buildings, CustomerFiles.Block, CustomerFiles.Apartment, CustomerFiles.ApartmentNo, CustomerFiles.FlatNo, 
    CustomerFiles.IsDefault, CustomerFiles.TaxOfficeName, CustomerFiles.TaxNumber, CustomerFiles.ZipCode, CustomerFiles.AddressNotes, 
    CustomerFiles.AreaCode, CustomerFiles.CustomerSpecialNotes, CustomerFiles.BirthDay, CustomerFiles.MaritialStatus, 
    CustomerFiles.Age, CustomerFiles.EmailAddress, CustomerFiles.Sexuality, CustomerFiles.FacebookAccount, 
    CustomerFiles.TwitterAccount, CustomerFiles.WebSite, CustomerFiles.PhotoPath, CustomerFiles.ProximityCardID, 
    CustomerFiles.EditKey, CustomerFiles.SyncKey, CustomerFiles.BranchID, CustomerFiles.LockData, CustomerFiles.LockStationID, 
    CustomerFiles.AddUserID, CustomerFiles.AddDateTime, CustomerFiles.EditUserID, CustomerFiles.EditDateTime, 
    ISNULL(CustomerFiles.OpenValue, 0) AS OpenValue, 
    ISNULL(CustomerFiles.AparmentFlatNo, '') AS AparmentFlatNo, 
    '' AS AddressText, 
    ISNULL(br.BranchName, '') AS BranchName, 
    (ISNULL(CustomerFiles.CreditLimit, 0) - ISNULL(CustomerFiles.TotalDebt, 0)) AS RemainCreditLimit, 
    ISNULL(CustomerFiles.IsEmployee, 0) AS IsEmployee, 
    ISNULL(CustomerFiles.DetailData, '') AS DetailData 
FROM CustomerFiles 
LEFT OUTER JOIN efr_Branchs AS br ON CustomerFiles.BranchID = br.BranchID 
INNER JOIN CustomerPhones AS p ON CustomerFiles.CustomerKey = p.CustomerKey 
WHERE p.PhoneNumber LIKE '%0553946829%';

-- 4. Filial statusunun yoxlanışı
SELECT ISNULL(
    (SELECT TOP 1 ISNULL(ParamValue, 'NO') 
     FROM StoreSettings 
     WHERE ParamKey = 'BranchStatus'), 'NO'
) AS ParamValue;

-- 5. Müştəri vergi məlumatlarının yenilənməsi
UPDATE CustomerFiles 
SET TaxOfficeName = CustomerCompanies.TaxOfficeName,
    TaxNumber = CustomerCompanies.TaxNumber 
FROM CustomerFiles 
INNER JOIN CustomerCompanies ON CustomerCompanies.CompanyKey = CustomerFiles.PhotoPath 
WHERE LEN(CustomerFiles.PhotoPath) > 20 
  AND CustomerFiles.AutoID = 9;

-- 6. Müştərinin əsas profili və son sifariş statistikası (AutoID = 9)
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; 
DECLARE @ckey UNIQUEIDENTIFIER;
SET @ckey = (SELECT c.CustomerKey FROM CustomerFiles AS c WHERE c.AutoID = 9); 

WITH sonuc AS (
    SELECT 
        h.CustomerKey, s.LastOrderTotal, COUNT(h.AutoID) AS OrderCount, 
        SUM(h.AmountDue) AS OrderTotal, MAX(h.OrderDateTime) AS LastOrderDateTime 
    FROM (
        SELECT 
            h.CustomerKey, h.AmountDue AS LastOrderTotal, 
            ROW_NUMBER() OVER (PARTITION BY h.CustomerKey ORDER BY OrderDateTime DESC) AS R 
        FROM OrderHeaders AS h WITH (NOLOCK) 
        WHERE h.OrderType = 5 
          AND (h.OrderStatus <> 3) 
          AND (ISNULL(h.LineDeleted, 0) = 0) 
          AND h.CustomerKey = @ckey 
        GROUP BY h.CustomerKey, h.AmountDue, h.OrderDateTime
    ) s 
    INNER JOIN OrderHeaders AS h ON h.CustomerKey = s.CustomerKey 
    WHERE s.R = 1 
    GROUP BY h.CustomerKey, s.LastOrderTotal
) 
SELECT 
    CustomerFiles.WebUserName, CustomerFiles.WebPassword, CustomerFiles.AutoID, CustomerFiles.CustomerID, 
    CustomerFiles.CustomerKey, CustomerFiles.CustomerGlobalKey, CustomerFiles.CustomerIsActive, CustomerFiles.CustomerName, 
    CustomerFiles.CustomerFullName, CustomerFiles.CardNumber, CustomerFiles.CustomerNotes, CustomerFiles.OrderCount, 
    CustomerFiles.LastCallDate, CustomerFiles.CallingCount, CustomerFiles.AllowHouseAccount, CustomerFiles.IsFrequentDiner, 
    CustomerFiles.CreditLimit, CustomerFiles.CreditSatusID, CustomerFiles.DiscountPercent, CustomerFiles.SpecialBonusPercent, 
    CustomerFiles.TotalDebt, CustomerFiles.TotalPayment, CustomerFiles.TotalRemainig, 
    ISNULL(CustomerFiles.BonusStartupValue, 0) AS BonusStartupValue, 
    ISNULL(CustomerFiles.TotalBonusUsed, 0) AS TotalBonusUsed, 
    ISNULL(CustomerFiles.TotalBonusEarned, 0) AS TotalBonusEarned, 
    ISNULL(CustomerFiles.TotalBonusRemaing, 0) AS TotalBonusRemaing, 
    CustomerFiles.CityName, CustomerFiles.District, CustomerFiles.Neighborhood, CustomerFiles.Avenue, CustomerFiles.Street, 
    CustomerFiles.Buildings, CustomerFiles.Block, CustomerFiles.Apartment, CustomerFiles.ApartmentNo, CustomerFiles.FlatNo, 
    CustomerFiles.IsDefault, CustomerFiles.TaxOfficeName, CustomerFiles.TaxNumber, CustomerFiles.ZipCode, CustomerFiles.AddressNotes, 
    CustomerFiles.AreaCode, CustomerFiles.CustomerSpecialNotes, CustomerFiles.BirthDay, CustomerFiles.MaritialStatus, CustomerFiles.Age, 
    CustomerFiles.EmailAddress, CustomerFiles.Sexuality, CustomerFiles.FacebookAccount, CustomerFiles.TwitterAccount, 
    CustomerFiles.WebSite, CustomerFiles.PhotoPath, CustomerFiles.ProximityCardID, CustomerFiles.EditKey, CustomerFiles.SyncKey, 
    CustomerFiles.BranchID, CustomerFiles.LockData, CustomerFiles.LockStationID, CustomerFiles.AddUserID, CustomerFiles.AddDateTime, 
    CustomerFiles.EditUserID, CustomerFiles.EditDateTime, 
    ISNULL(CustomerFiles.OpenValue, 0) AS OpenValue, 
    ISNULL(CustomerFiles.AparmentFlatNo, '') AS AparmentFlatNo, 
    '' AS AddressText, 
    ea.FirstName AS AddEmployeeName, 
    ee.FirstName AS EditEmployeeName, 
    ISNULL(s.LastOrderTotal, 0) AS LastOrderTotal, 
    ISNULL(s.OrderCount, 0) AS TakeawayOrderCount, 
    ISNULL(s.OrderTotal, 0) AS TakeawayOrderTotal, 
    s.LastOrderDateTime, 
    ISNULL(br.BranchName, '') AS BranchName, 
    (ISNULL(CustomerFiles.CreditLimit, 0) - ISNULL(CustomerFiles.TotalRemainig, 0)) AS RemainCreditLimit, 
    ISNULL(CustomerFiles.IsEmployee, 0) AS IsEmployee, 
    ISNULL(CustomerFiles.DetailData, '') AS DetailData 
FROM CustomerFiles 
LEFT OUTER JOIN sonuc AS s ON s.CustomerKey = CustomerFiles.CustomerKey 
LEFT OUTER JOIN EmployeeFiles AS ea ON CustomerFiles.AddUserID = ea.AutoID 
LEFT OUTER JOIN EmployeeFiles AS ee ON CustomerFiles.EditUserID = ee.AutoID 
LEFT OUTER JOIN efr_Branchs AS br ON CustomerFiles.BranchID = br.BranchID 
WHERE CustomerFiles.AutoID = 9;

-- 7. Müştərinin əlaqəli şəxslərinin siyahısı
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
SELECT [CustomerContactID], [ContactKey], [CustomerKey], [CustomerID], [ContactName], [LastCalling], [CallCounter], [EditKey], [SyncKey] 
FROM [CustomerContacts]  
WHERE CustomerID = 9;

-- 8. Müştərinin əlaqəli nömrələrinin siyahısı
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
SELECT [PhoneNumberID], [PhoneKey], [CustomerID], [CustomerKey], [PhoneNumber], [Extension], [LastCalling], [CallCounter], [EditKey], [SyncKey]  
FROM [CustomerPhones] 
WHERE CustomerID = 9;

-- 9. Müştərinin aktiv sifariş statusları
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; 
SELECT TOP 2 OrderID, LevelID, r.StatusText, DATEDIFF(minute, r.OrderDateTime, GETDATE()) AS PassedMinutes 
FROM ( 
    SELECT TOP 2 OrderID, 1 AS LevelID, 'ÜRETİMDE' AS StatusText, OrderDateTime 
    FROM OrderHeaders 
    WHERE CustomerKey = 'D3FF3A39-C77F-4A0C-BE37-3F5173B53ADC' 
      AND OrderStatus = 1 AND DriverDepartureTime IS NULL AND DriverArrivalTime IS NULL AND TableReady = 0 
    UNION ALL 
    SELECT TOP 2 OrderID, 2 AS LevelID, 'HAZIR' AS StatusText, OrderDateTime 
    FROM OrderHeaders 
    WHERE CustomerKey = 'D3FF3A39-C77F-4A0C-BE37-3F5173B53ADC' 
      AND OrderStatus = 1 AND DriverDepartureTime IS NULL AND DriverArrivalTime IS NULL AND TableReady = 1 
    UNION ALL 
    SELECT TOP 2 OrderID, 3 AS LevelID, 'YOLDA' AS StatusText, OrderDateTime 
    FROM OrderHeaders 
    WHERE CustomerKey = 'D3FF3A39-C77F-4A0C-BE37-3F5173B53ADC' 
      AND OrderStatus = 1 AND DriverDepartureTime IS NOT NULL AND DriverArrivalTime IS NULL AND TableReady = 1 
    UNION ALL 
    SELECT TOP 2 OrderID, 4 AS LevelID, 'TAMAMLANMIŞ' AS StatusText, OrderDateTime 
    FROM OrderHeaders 
    WHERE CustomerKey = 'D3FF3A39-C77F-4A0C-BE37-3F5173B53ADC' 
      AND OrderStatus = 1 AND DriverDepartureTime IS NOT NULL AND DriverArrivalTime IS NOT NULL AND TableReady = 1 
) AS r 
ORDER BY r.OrderDateTime DESC;

-- 10. Dil tərcümə resursunun əlavəsi
INSERT INTO [LanguageResource] (
    [KeyField], [Turkish], [English], [Lang1], [Lang2], [Lang3], [Lang4], [Lang5], [LangType]
) VALUES (
    N'hazır', N'HAZIR', NULL, NULL, NULL, NULL, NULL, NULL, 0
);

-- 11. Müştəri kartının yenilənməsi
UPDATE [CustomerFiles] 
SET [CustomerID] = 9,
    [CustomerKey] = 'D3FF3A39-C77F-4A0C-BE37-3F5173B53ADC',
    [CustomerGlobalKey] = NULL,
    [CustomerIsActive] = 1,
    [CustomerName] = N'ZAUR MAHMUDOV',
    [CustomerFullName] = N'ZAHUR',
    [CardNumber] = NULL,
    [CustomerNotes] = NULL,
    [OrderCount] = 2,
    [LastCallDate] = NULL,
    [CallingCount] = 0,
    [AllowHouseAccount] = 0,
    [IsFrequentDiner] = 0,
    [CreditLimit] = 0,
    [CreditSatusID] = 0,
    [DiscountPercent] = 0,
    [SpecialBonusPercent] = 10,
    [TotalDebt] = 0,
    [TotalPayment] = 0,
    [TotalRemainig] = 0,
    [BonusStartupValue] = 0,
    [TotalBonusUsed] = 0,
    [TotalBonusEarned] = 0,
    [TotalBonusRemaing] = 0,
    [CityName] = NULL,
    [District] = NULL,
    [Neighborhood] = NULL,
    [Avenue] = NULL,
    [Street] = NULL,
    [Buildings] = NULL,
    [Block] = NULL,
    [Apartment] = NULL,
    [ApartmentNo] = NULL,
    [FlatNo] = NULL,
    [IsDefault] = 1,
    [TaxOfficeName] = NULL,
    [TaxNumber] = NULL,
    [ZipCode] = NULL,
    [AddressNotes] = NULL,
    [AreaCode] = NULL,
    [CustomerSpecialNotes] = NULL,
    [BirthDay] = NULL,
    [MaritialStatus] = NULL,
    [Age] = NULL,
    [EmailAddress] = NULL,
    [Sexuality] = NULL,
    [FacebookAccount] = NULL,
    [TwitterAccount] = NULL,
    [WebSite] = NULL,
    [PhotoPath] = NULL,
    [ProximityCardID] = NULL,
    [EditKey] = 'CA6672FB-F962-4A70-A59E-211951C8ABDB',
    [SyncKey] = NEWID(),
    [BranchID] = 422,
    [LockData] = 0,
    [LockStationID] = 0,
    [AddDateTime] = '2017-09-07 21:06:06.160',
    [EditUserID] = 106,
    [EditDateTime] = GETDATE(),
    [OpenValue] = 0,
    [DetailData] = NULL,
    [AparmentFlatNo] = NULL 
WHERE [AutoID] = 9;

-- 12. Müştərinin bonus balansının yenidən hesablanması
UPDATE CustomerFiles 
SET TotalBonusEarned = (
    SELECT ISNULL((SELECT SUM(ISNULL(BonusAmountEarned, 0)) 
                   FROM OrderHeaders 
                   WHERE OrderHeaders.BonusCustomerID = 9 AND OrderHeaders.LineDeleted = 0), 0)
), 
TotalBonusUsed = (
    SELECT ISNULL((SELECT SUM(ISNULL(AmountPaid, 0)) 
                   FROM OrderPayments 
                   WHERE OrderPayments.CustomerID = 9 AND OrderPayments.LineDeleted = 0 AND OrderPayments.PaymentMethodID = 102), 0)
) 
WHERE CustomerID = 9; 

UPDATE CustomerFiles 
SET TotalBonusRemaing = ISNULL(BonusStartupValue, 0) + ISNULL(TotalBonusEarned, 0) - ISNULL(TotalBonusUsed, 0) 
WHERE AutoID = 9;

-- 13. İşçi üçün nahar/bonus balansının hesablanması
DECLARE @startDate DATETIME;
SET @startDate = (DATEADD(month, DATEDIFF(month, 0, GETDATE()), 0)); 
DECLARE @startDay INT;
SET @startDay = (SELECT ISNULL((SELECT TOP 1 s.ParamValue FROM StoreSettings AS s WHERE s.ParamKey = 'EmployeeDinnerStartupDay'), '1')); 
SET @startDate = (DATEADD(DAY, @startDay - 1, @startDate)); 

UPDATE CustomerFiles 
SET TotalBonusEarned = 0, 
    TotalBonusUsed = (
        SELECT ISNULL((SELECT SUM(ISNULL(AmountPaid, 0)) 
                       FROM OrderPayments 
                       WHERE OrderPayments.PaymentDateTime > @startDate 
                         AND OrderPayments.CustomerID = 9 
                         AND OrderPayments.LineDeleted = 0 
                         AND OrderPayments.PaymentMethodID = 102), 0)
    ) 
WHERE CustomerID = 9 AND ISNULL(IsEmployee, 0) = 1; 

UPDATE CustomerFiles 
SET TotalBonusRemaing = ISNULL(BonusStartupValue, 0) + ISNULL(TotalBonusEarned, 0) - ISNULL(TotalBonusUsed, 0) 
WHERE AutoID = 9;

-- 14. Müştəri telefonunun yenilənməsi
UPDATE [CustomerPhones] 
SET [CustomerID] = 9,
    [PhoneNumber] = N'0553946829',
    [Extension] = NULL,
    [LastCalling] = NULL,
    [CallCounter] = 0,
    [CustomerKey] = 'D3FF3A39-C77F-4A0C-BE37-3F5173B53ADC',
    [PhoneKey] = 'CE30F2FC-9069-4B36-AA25-3C1CB477A039',
    [EditKey] = '3F1830CA-77E0-4FF4-A6DC-74513B25D9C9',
    [SyncKey] = NEWID()  
WHERE [PhoneNumberID] = 9;

-- 15. Sistem parametrlərinin oxunması
SELECT ISNULL((SELECT TOP 1 ParamValue FROM StoreSettings WHERE ParamKey = 'BranchID'), '0') AS BranchID;
SELECT ISNULL((SELECT TOP 1 ParamValue FROM StoreSettings WHERE ParamKey = 'ServerSocketUrl'), '0') AS ServerSocketUrl;

-- 16. POS daxili düzəliş və təmizləmə scriptləri
UPDATE MenuModifierGroups SET MenuModifierGroupKey = NEWID() WHERE MenuModifierGroupKey IS NULL; 
UPDATE MenuModifierLayout SET MenuModifierLayoutKey = NEWID() WHERE MenuModifierLayoutKey IS NULL; 
UPDATE PaymentMethods SET PaymentName = 'NAKİT' WHERE PaymentName = 'NAKIT'; 
UPDATE MenuItemLayout SET LayoutKey = NEWID() WHERE LayoutKey IS NULL; 
UPDATE DineInTables SET DineInTableID = AutoID WHERE ISNULL(DineInTableID, 0) = 0; 
UPDATE DineInTableGroups SET TableGroupID = AutoID WHERE ISNULL(TableGroupID, 0) = 0; 
UPDATE DineInTableGroups SET TableGroupKey = NEWID() WHERE TableGroupKey IS NULL; 
UPDATE DineInTables SET DineInTableKey = NEWID() WHERE DineInTableKey IS NULL; 
UPDATE DineInTables SET TableGroupKey = DineInTableGroups.TableGroupKey 
FROM DineInTables INNER JOIN DineInTableGroups ON DineInTables.TableGroupID = DineInTableGroups.TableGroupID WHERE DineInTables.TableGroupKey IS NULL;
UPDATE t SET TableGroupKey = g.TableGroupKey FROM DineInTables t
INNER JOIN DineInTableGroups g ON g.TableGroupID = t.TableGroupID
WHERE t.TableGroupKey <> g.TableGroupKey;

UPDATE MenuItemLayout SET MainMenuItemKey = MenuItems.MenuItemKey FROM MenuItemLayout INNER JOIN MenuItems ON MenuItemLayout.MainMenuItemID = MenuItems.MenuItemID WHERE MenuItemLayout.MainMenuItemKey IS NULL; 
UPDATE GlobalMenuItemLayout SET MainMenuItemKey = GlobalMenuItems.MenuItemKey FROM GlobalMenuItemLayout INNER JOIN GlobalMenuItems ON GlobalMenuItemLayout.MainMenuItemID = GlobalMenuItems.MenuItemID WHERE GlobalMenuItemLayout.MainMenuItemKey IS NULL; 
UPDATE MenuItemLayout SET MenuItemKey = MenuItems.MenuItemKey FROM MenuItemLayout INNER JOIN MenuItems ON MenuItemLayout.MenuItemID = MenuItems.MenuItemID WHERE MenuItemLayout.MenuItemKey IS NULL; 
UPDATE GlobalMenuItemLayout SET MenuItemKey = GlobalMenuItems.MenuItemKey FROM GlobalMenuItemLayout INNER JOIN GlobalMenuItems ON GlobalMenuItemLayout.MenuItemID = GlobalMenuItems.MenuItemID AND GlobalMenuItemLayout.BranchID = GlobalMenuItems.BranchID WHERE GlobalMenuItemLayout.MenuItemKey IS NULL; 
UPDATE MenuModifierLayout SET MenuModifierKey = MenuModifiers.MenuModifierKey FROM MenuModifierLayout INNER JOIN MenuModifiers ON MenuModifierLayout.MenuModifierID = MenuModifiers.MenuModifierID WHERE MenuModifierLayout.MenuModifierKey IS NULL; 
UPDATE MenuModifierLayout SET MenuModifierGroupKey = MenuModifierGroups.MenuModifierGroupKey FROM MenuModifierLayout INNER JOIN MenuModifierGroups ON MenuModifierLayout.MenuModifierGroupID = MenuModifierGroups.MenuModifierGroupID WHERE MenuModifierLayout.MenuModifierGroupKey IS NULL; 
UPDATE GlobalMenuModifierLayout SET MenuModifierKey = GlobalMenuModifiers.MenuModifierKey FROM GlobalMenuModifierLayout INNER JOIN GlobalMenuModifiers ON GlobalMenuModifierLayout.MenuModifierID = GlobalMenuModifiers.MenuModifierID WHERE GlobalMenuModifierLayout.MenuModifierKey IS NULL; 
UPDATE GlobalMenuModifierLayout SET MenuModifierGroupKey = GlobalMenuModifierGroups.MenuModifierGroupKey FROM GlobalMenuModifierLayout INNER JOIN GlobalMenuModifierGroups ON GlobalMenuModifierLayout.MenuModifierGroupID = GlobalMenuModifierGroups.MenuModifierGroupID WHERE GlobalMenuModifierLayout.MenuModifierGroupKey IS NULL;
UPDATE MenuItemLayout SET MenuGroupKey = MenuGroups.MenuGroupKey FROM MenuItemLayout INNER JOIN MenuGroups ON MenuItemLayout.MenuGroupID = MenuGroups.MenuGroupID WHERE MenuItemLayout.MenuGroupKey IS NULL;
UPDATE MenuItemLayout SET MainMenuItemKey = MenuItems.MenuItemKey FROM MenuItemLayout INNER JOIN MenuItems ON MenuItemLayout.MainMenuItemID = MenuItems.AutoID WHERE MenuItemLayout.MainMenuItemKey IS NULL AND ISNULL(MenuItemLayout.MainMenuItemID, 0) > 0;
UPDATE EmployeeFiles SET EmployeeID = AutoID WHERE ISNULL(EmployeeID, 0) <> AutoID;
UPDATE [MenuItemLayout] SET MainMenuItemID = 0 WHERE MainMenuItemID IS NULL;
DELETE FROM MenuItemLayout WHERE AutoID NOT IN (SELECT MAX(AutoID) FROM MenuItemLayout GROUP BY ISNULL(MenuGroupID, 0), ISNULL(MainMenuItemID, 0), DisplayIndex);

-- Uğursuz ECR ödənişlərinin ləğvi
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

-- 17. Mağaza tənzimləmələri
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
SELECT [SettingsID], [OrderID], [TabName], [GroupName], [ParamName], [ParamKey], [ParamValue], [DefaultValue], [ParamType], [EditKey], [SyncKey], [BranchID], [AddUserID], [AddDateTime], [EditUserID], [EditDateTime]  
FROM [StoreSettings] 
WHERE ParamKey <> 'StoreStatus' 
ORDER BY TabName, GroupName, OrderID;

-- 18. Menyuda son dəyişiklik tarixi
SELECT TOP 1 ISNULL(r.lastDate, '1/1/2013') AS LastDate 
FROM ( 
    SELECT ISNULL(MAX(EditDateTime), '1/1/2013') AS lastDate FROM MenuItems 
    UNION ALL 
    SELECT ISNULL(MAX(AddDateTime), '1/1/2013') AS lastDate FROM MenuItems 
) AS r 
ORDER BY r.lastDate DESC;

-- 19. Bütün ödəniş növlərinin siyahısı
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
SELECT 
    [AutoID], [PaymentMethodID], [PaymentMethodKey], [PaymentName], [IsDefault], [DenyInvoice], [DenyFiscal], 
    [DenyMoneyChange], [ExchangeRate], [AccountingCode], [SecurityLevel], [IsLocked], [PaymentMethodActive], 
    ISNULL(EffectRegister, 1) AS [EffectRegister], [IsCoupon], [IsAccountPayment], [IsAccountSale], 
    [HideInRecievePayement], [PictureName], [DisplayIndex], [ButtonColor], [EditKey], [SyncKey], 
    ISNULL([ForcedInvoice], 0) AS ForcedInvoice, 
    ISNULL([IsCampusCard], 0) AS IsCampusCard, 
    [CampusCardServer], [BranchID], 
    ISNULL([AskCustomerName], 0) AS AskCustomerName, 
    ISNULL([GlobalBankCode], 0) AS GlobalBankCode, 
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

-- 20. Aktiv menyu məhsullarının Layout ilə birgə siyahısı
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
SELECT 
    MenuItems.AutoID, MenuItems.MenuItemID, ISNULL(MenuItemLayout.MainMenuItemID, 0) AS MainMenuItemID, 
    MenuItems.RevenueCenterTypeID, MenuItems.MenuItemText, MenuItems.MenuCategoryID, MenuItems.MenuGroupID, 
    MenuItems.DisplayIndex AS DisplayIndex2, MenuItems.DefaultUnitPrice, MenuItems.MenuItemCost, MenuItems.MenuItemDescription, 
    MenuItems.MenuItemNotification, MenuItems.MenuItemActive, MenuItems.MenuItemInStock, MenuItems.MenuItemTaxable, 
    ISNULL(TaxGroups.TaxRate, 8) AS TaxPercent, MenuItems.MenuModifierID, MenuItems.MenuItemDiscountable, 
    MenuItems.MenuItemPopUpHeaderID, MenuItems.MenuItemPopUpChoiceText, MenuItems.HasModifierPopUps, 
    ISNULL(MenuItems.SecLangMenuItemText, '') AS SecLangMenuItemText, MenuItems.SecLangPopUpChoiceText, 
    MenuItems.PictureName, MenuItems.ShowCaption, ISNULL(MenuItems.IsComboMenu, 0) AS IsComboMenu, 
    ISNULL(MenuItems.IsTopMenu, 0) AS IsTopMenu, MenuItems.ButtonColor, ISNULL(MenuItems.Barcode, '') AS Barcode, 
    ISNULL(MenuItems.Barcode2, '') AS Barcode2, MenuItems.ItemDelCharge, MenuItems.ItemDelComp, MenuItems.DineInPrice, 
    MenuItems.BarTabPrice, MenuItems.TakeOutPrice, MenuItems.DriveThruPrice, MenuItems.DeliveryPrice, MenuItems.OrderByWeight, 
    MenuItems.PrintPizzaLabel, MenuItems.KitchenSortNumber, MenuItems.ModBuilderTemplateID, MenuItems.MenuItemTypeID, 
    ISNULL(MenuItems.AccountingCode, '') AS AccountingCode, ISNULL(MenuItems.PrintOnLabel, 0) AS PrintOnLabel, 
    MenuItems.UsedPrinterID1, MenuItems.UsedPrinterID2, MenuItems.UsedPrinterID3, MenuItems.UsedPrinterID4, MenuItems.UsedPrinterID5, 
    MenuItems.SecurityLevel, MenuItems.DeleteReason, MenuItems.CustomField1, MenuItems.CustomField2, MenuItems.CustomField3, 
    MenuItems.CustomField4, MenuItems.CustomField5, MenuItems.EditKey, MenuItems.SyncKey, MenuItems.BranchID, MenuItems.AddUserID, 
    MenuItems.AddDateTime, MenuItems.EditUserID, ISNULL(MenuItems.UseKds1, 0) AS UseKds1, ISNULL(MenuItems.UseKds2, 0) AS UseKds2, 
    ISNULL(MenuItems.UseKds3, 0) AS UseKds3, ISNULL(MenuItems.UseKds4, 0) AS UseKds4, ISNULL(MenuItems.UseKds5, 0) AS UseKds5, 
    ISNULL(MenuItems.UseKds6, 0) AS UseKds6, ISNULL(MenuItems.UseKds7, 0) AS UseKds7, ISNULL(MenuItems.UseKds8, 0) AS UseKds8, 
    ISNULL(MenuItems.UseKds9, 0) AS UseKds9, ISNULL(MenuItems.UseKds10, 0) AS UseKds10, MenuItems.CountDownDate, 
    ISNULL(MenuItems.CountDownValue, 0.0) AS CountDownValue, ISNULL(MenuItems.CountDownActualResult, 0.0) AS CountDownActualResult,  
    MenuItems.EditDateTime, MenuCategories.MenuCategoryText, MenuSubCategories.MenuSubCategoryText, TaxGroups.GroupName AS TaxGroupText, 
    MenuModifierGroups.MenuModifierGroupText AS MenuModifierText, MenuItems.DisplayIndex AS MenuDisplayIndex, MenuItems.MenuItemKey, 
    MenuItemLayout.MainMenuItemKey, MenuItems.MenuItemGlobalKey, MenuItems.MenuCategoryKey, MenuItems.MenuGroupKey, MenuItems.TaxGroupID, 
    MenuItems.TaxGroupKey, MenuItems.MenuModifierKey, MenuItems.MenuModifierForcedID, MenuItems.MenuModifierForcedKey, 
    MenuForcedModifierGroups.MenuModifierGroupText AS MenuForcedModifierText, efr_Branchs.BranchName, 
    MenuItemLayout.DisplayIndex AS DisplayIndex, MenuItemLayout.MenuGroupID AS MenuScreenGroupID, MenuItemLayout.MenuGroupKey AS MenuScreenGroupKey 
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

-- 21. İşçilərin müştəri kartındakı məlumatlarının sinxronizasiyası
UPDATE CustomerFiles 
SET CustomerName = EmployeeFiles.FirstName + ISNULL(NULLIF((' ' + ISNULL(EmployeeFiles.LastName, ' ')), ''), ''), 
    CustomerIsActive = EmployeeFiles.EmployeeActive,
    CardNumber = EmployeeFiles.SmarCardCode 
FROM CustomerFiles 
INNER JOIN EmployeeFiles ON CustomerFiles.CustomerKey = EmployeeFiles.EmployeeKey;

-- 22. Menyu qruplarının oxunması
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
SELECT [MenuGroupID], [MenuGroupKey], [MenuGroupText], [DisplayIndex], [MenuGroupActive], [SecLangMenuGroupText], [PictureName], 
       [ShowCaption], [HideInDineIn], [HideInBar], [HideInTakeaway], [HideInCounter], [HideInDelivery], [ButtonColor], 
       [RevenueCenterTypeID], [DeleteReason], [CustomField1], [CustomField2], [CustomField3], [CustomField4], [CustomField5], 
       [EditKey], [SyncKey], [BranchID], [AddUserID], [AddDateTime], [EditUserID], [EditDateTime]  
FROM [MenuGroups];

-- 23. Masa qrupları və tənzimləmələri
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
SELECT [AutoID], [TableGroupID], [TableGroupKey], [TableGroupText], [RevenueCenterTypeID], [DeleteReason], [CustomField1], 
       [CustomField2], [CustomField3], [CustomField4], [CustomField5], [EditKey], [SyncKey], [BranchID], [AddUserID], 
       [AddDateTime], [EditUserID], [EditDateTime], 
       ISNULL(TableRowCount, 8) AS TableRowCount, 
       ISNULL(TableColumnCount, 9) AS TableColumnCount  
FROM [DineInTableGroups];

-- 24. Masalar və aktiv sifariş statusları
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
SELECT 
    DineInTables.AutoID, DineInTables.DineInTableID, DineInTables.DineInTableKey, DineInTables.TableGroupKey, 
    DineInTables.DineInTableText, DineInTables.SectionNumber, DineInTables.TableGroupID, DineInTables.DisplayIndex, 
    ISNULL(DineInTables.DineInTableActive, 1) AS DineInTableActive, DineInTables.MaxGuests, DineInTables.Smoking, 
    DineInTables.Window, DineInTables.Booth, DineInTables.Privacy, DineInTables.PictureName, DineInTables.AvarageSeatTime, 
    DineInTables.RevenueCenterTypeID, ISNULL(DineInTables.SecurityLevel, 0) AS SecurityLevel, DineInTables.DeleteReason, 
    DineInTables.CustomField1, DineInTables.CustomField2, DineInTables.CustomField3, DineInTables.CustomField4, 
    DineInTables.CustomField5, DineInTables.EditKey, DineInTables.SyncKey, DineInTables.BranchID, DineInTables.AddUserID, 
    DineInTables.AddDateTime, DineInTables.EditUserID, DineInTables.EditDateTime, 
    OrderHeaders.OrderKey AS ActiveOrderKey, 
    (ISNULL(OrderHeaders.AmountDue, 0.0) + ISNULL(OrderHeaders.CashGratuity, 0.0)) AS ActiveOrderAmountDue, 
    OrderHeaders.OrderDateTime AS ActiveOrderDateTime, OrderHeaders.EditDateTime AS ActiveOrderEditTime, 
    ISNULL(EmployeeFiles.FirstName, '') AS ActiveOrderEmyloyeeName, 
    ISNULL(OrderHeaders.GuestCheckPrinted, 0) AS GuestCheckPrinted, 
    ISNULL(OrderHeaders.TableReady, 0) AS TableReady, 
    ISNULL(OrderHeaders.AdditionPrintedLineCount, 0) AS AdditionPrintedLineCount, 
    ISNULL(EmployeeFiles.EmployeeKey, '00000000-0000-0000-0000-000000000000') AS ActiveOrderEmyloyeeKey 
FROM DineInTables WITH (NOLOCK)  
LEFT OUTER JOIN OrderHeaders ON OrderHeaders.DineInTableID = DineInTables.DineInTableID 
    AND ISNULL(OrderHeaders.OrderStatus, 1) = 1 
    AND OrderHeaders.OrderType = 1 
LEFT OUTER JOIN EmployeeFiles ON EmployeeFiles.EmployeeID = OrderHeaders.EmployeeID;

-- 25. Vergi qrupları
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
SELECT [TaxGroupID], [GroupName], [TaxRate], [EditKey], [SyncKey] 
FROM [TaxGroups] 
ORDER BY TaxGroupID;

-- 26. Modifikator qrupları
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
SELECT 
    MenuModifierGroupID, MenuModifierGroupText, DisplayIndex, MenuModifierGroupActive, SecLangMenuModifierGroupText, 
    PictureName, ButtonColor, RevenueCenterTypeID, DeleteReason, CustomField1, CustomField2, CustomField3, CustomField4, 
    CustomField5, EditKey, SyncKey, BranchID, AddUserID, AddDateTime, EditUserID, EditDateTime, MenuModifierGroupKey 
FROM MenuModifierGroups;

-- 27. Pərakəndə barkodlar
SELECT [AutoID], [MenuItemKey], ISNULL([Barcode], '') AS [Barcode], ISNULL([Quantity], 0) AS [Quantity], ISNULL([SalePrice], 0) AS [SalePrice] 
FROM [GlobalRetailBarcodes];

-- 28. Kassa (Station) ayarları yoxlanışı və oxunması (StationID = 1)
SELECT COUNT(AutoID) AS countValue FROM StationSettings WHERE StationID = 1;

SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
SELECT 
    [AutoID], [StationID], [StationName], [AllowRegister], [DefineValue], [ActivateCashier], [RememberCashier], 
    [BackgroundPicture], [SideBarPicture], [DefaultLoginEntrance], [DefultTablePlan], [SkinName], [ShowDineInButton], 
    [ShowTakeOutButton], [ShowDriveThruButton], [ShowDeliveryButton], [ShowRecallButton], [ShowDriverStatusButton], 
    [ShowTimeCardButton], [ShowOperationsButton], [ShowBackOfficeButton], [ShowQuickServiceDineIn], [ShowQuickServiceTakeOut], 
    [ShowQuickServiceDriveThru], [ShowQuickServiceDelivery], [StayInOrderScreenTakeOut], [StayInOrderScreenDriveThru], 
    [StayTablePlanInDineIn], [TimeOutScreenLock], [IsMobile], [CustomerDisplayPortNo], [CustomerDisplayLine1], 
    [CustomerDisplayLine2], [WeightScale1PortNo], [WeightScale1BaudRate], [WeightScale2PortNo], [WeightScale2BaudRate], 
    [CashRegisterModel], [CashRegisterBaudRate], [CashRegisterDataDelay], [CashRegisterLineDelay], [CashRegisterStations], 
    [CashRegisterPortNo], [PaymentOverTime], [DirectOpenOrderInEditMode], [PrintVoidedLinesOnGuestCheck], [StationKey], 
    ISNULL([AskPasswordForReduce], 0) AS [AskPasswordForReduce], 
    ISNULL([MediaDisplayIsActive], 0) AS [MediaDisplayIsActive], 
    [MediaDisplayOnWaiting], [MediaDisplayOnSale], 
    ISNULL([MediaDisplayClosedMessage], 'KASA KAPALIDIR') AS [MediaDisplayClosedMessage], 
    ISNULL([MediaDisplayMoneyOver], 'TEŞEKKÜR EDERİZ') AS [MediaDisplayMoneyOver], 
    [EditKey], [SyncKey], 
    ISNULL(ShowCashTrayButton, '') AS ShowCashTrayButton, 
    ISNULL(DisableSaleOnOpenCashTray, '') AS DisableSaleOnOpenCashTray, 
    BekoPosDesign, BekoPosOutput, BekoPosDatabase, 
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
WHERE StationID = 1;

-- 29. Kassa printer tənzimləmələri (StationID = 1)
SELECT COUNT(AutoID) AS countValue FROM StationPrinterSettings WHERE StationID = 1;

SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
SELECT 
    AutoID, StationID, CheckPrinterName, CheckAltPrinterName, CheckPrinterID, CheckDesignPath, DeliveryPrinterName, 
    DeliveryAltPrinterName, DeliveryPrinterID, DeliveryDesignPath, InvoicePrinterName, InvoiceAltPrinterName, 
    InvoicePrinterID, InvoiceDesignPath, InvoiceRowCount, ReportPrinterName, ReportAltPrinterName, ReportPrinterID, 
    ReportDesignPath, AdditionPrinterName, AdditionPrinterID, AdditionDesignPath, AdditionRowCount, ReportA4PrinterName, 
    ReportA4AltPrinterName, ReportA4PrinterID, ReportA4DesignPath, Kitchen1PrinterName, Kitchen1AltPrinterName, 
    Kitchen1PrinterID, Kitchen1DesignPath, Kitchen2PrinterName, Kitchen2AltPrinterName, Kitchen2PrinterID, Kitchen2DesignPath, 
    Kitchen3PrinterName, Kitchen3AltPrinterName, Kitchen3PrinterID, Kitchen3DesignPath, Kitchen4PrinterName, 
    Kitchen4AltPrinterName, Kitchen4PrinterID, Kitchen4DesignPath, Kitchen5PrinterName, Kitchen5AltPrinterName, 
    Kitchen5PrinterID, Kitchen5DesignPath, Kitchen6PrinterName, Kitchen6AltPrinterName, Kitchen6PrinterID, Kitchen6DesignPath, 
    Kitchen7PrinterName, Kitchen7AltPrinterName, Kitchen7PrinterID, Kitchen7DesignPath, Kitchen8PrinterName, 
    Kitchen8AltPrinterName, Kitchen8PrinterID, Kitchen8DesignPath, Kitchen9PrinterName, Kitchen9AltPrinterName, 
    Kitchen9PrinterID, Kitchen9DesignPath, Kitchen10PrinterName, Kitchen10AltPrinterName, Kitchen10PrinterID, Kitchen10DesignPath, 
    Kitchen11PrinterName, Kitchen11AltPrinterName, Kitchen11PrinterID, Kitchen11DesignPath, Kitchen12PrinterName, 
    Kitchen12AltPrinterName, Kitchen12PrinterID, Kitchen12DesignPath, Kitchen13PrinterName, Kitchen13AltPrinterName, 
    Kitchen13PrinterID, Kitchen13DesignPath, Kitchen14PrinterName, Kitchen14AltPrinterName, Kitchen14PrinterID, 
    Kitchen14DesignPath, Kitchen15PrinterName, Kitchen15AltPrinterName, Kitchen15PrinterID, Kitchen15DesignPath, 
    Kitchen16PrinterName, Kitchen16AltPrinterName, Kitchen16PrinterID, Kitchen16DesignPath, Kitchen17PrinterName, 
    Kitchen17AltPrinterName, Kitchen17PrinterID, Kitchen17DesignPath, Kitchen18PrinterName, Kitchen18AltPrinterName, 
    Kitchen18PrinterID, Kitchen18DesignPath, Kitchen19PrinterName, Kitchen19AltPrinterName, Kitchen19PrinterID, 
    Kitchen19DesignPath, Kitchen20PrinterName, Kitchen20AltPrinterName, Kitchen20PrinterID, Kitchen20DesignPath, 
    InvoiceTopFeed, AdditionTopFeed, 
    ISNULL(PrintDineInOrdersKitchen, 1) AS PrintDineInOrdersKitchen, 
    ISNULL(PrintBarTableOrdersKitchen, 1) AS PrintBarTableOrdersKitchen, 
    ISNULL(PrintTakeOutOrdersKitchen, 1) AS PrintTakeOutOrdersKitchen, 
    ISNULL(PrintDriveThruOrdersKitchen, 1) AS PrintDriveThruOrdersKitchen, 
    ISNULL(PrintDeliveryOrdersKitchen, 1) AS PrintDeliveryOrdersKitchen, 
    [EditKey], [SyncKey], [LabelPrinterID], [LabelPrinterName], [LabelDesignPath], [ReturnPrinterID], [ReturnPrinterName], [ReturnDesignPath] 
FROM StationPrinterSettings  
WHERE StationID = 1;

-- 30. Açıq kassa növbəsi yoxlanışı
SELECT COUNT(RegisterSessionID) AS countValue 
FROM RegisterSessions 
WHERE StationID = 1 AND SignOutDateTime IS NULL;

-- 31. Sifariş kartları və Happy Hour parametrləri
SELECT COUNT(*) AS countValue FROM OrderCards WITH (NOLOCK);

SELECT ISNULL(
    (SELECT TOP 1 ISNULL(ParamValue, 'NO') 
     FROM StoreSettings 
     WHERE ParamKey = 'HappyHoursActive'), 'NO'
) AS ParamValue;

-- 32. Menyu səhifələmə hesablaması
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
SELECT ((MAX(DisplayIndex) / 32) + 1) AS MaxPageID, MenuGroupKey, MainMenuItemKey 
FROM MenuItemLayout 
GROUP BY MenuGroupKey, MainMenuItemKey;

-- 33. Aksiyalar və kampaniyalar (Promotions)
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
SELECT 
    Promotions.PromotionID, Promotions.PromotionKey, Promotions.PromotionName, Promotions.PromotionNotes, 
    Promotions.IsActive, Promotions.UseAuto, Promotions.MenuGroupKey, Promotions.MenuGroupCondition, 
    Promotions.MenuGroupMinSale, Promotions.MenuItemKey, Promotions.MenuItemsCondition, Promotions.MenuItemsMinSale, 
    Promotions.StartDate, Promotions.EndDate, Promotions.StartHour, Promotions.EndHour, Promotions.SaleTypes, 
    Promotions.SaleTypeCondition, Promotions.Stations, Promotions.StationCondition, Promotions.Days, 
    Promotions.DayCondition, Promotions.Employees, Promotions.EmployeeCondition, Promotions.CustomerGroup, 
    Promotions.CustomerGroupCondition, Promotions.PaymentTypes, Promotions.PaymentTypeCondition, Promotions.GiftMenuGroupKey, 
    Promotions.GiftMenuGroupContidion, Promotions.GiftMenuGroupQuantity, Promotions.GiftMenuItemKey, 
    Promotions.GiftMenuItemsCondition, Promotions.GiftMenuItemQuantity, Promotions.GiftMenuDiscountAmount, 
    Promotions.GiftMenuDiscountPercent, Promotions.GiftOrderDiscountAmount, Promotions.GiftOrderDiscountPercent, 
    Promotions.GiftStaticPrice, Promotions.GiftBonusPercent, Promotions.EditKey, Promotions.SyncKey, Promotions.BranchID, 
    Promotions.AddUserID, Promotions.AddDateTime, Promotions.EditUserID, Promotions.EditDateTime, 
    ISNULL(MenuGroups.MenuGroupText, '') AS MenuGroupName, 
    ISNULL(GiftMenuGroups.MenuGroupText, '') AS GiftMenuGroupName, 
    ISNULL(MenuItems.MenuItemText, '') AS MenuItemName, 
    ISNULL(GiftMenuItems.MenuItemText, '') AS GiftMenuItemName, 
    ISNULL(ea.FirstName, '') AS AddEmployeeName, 
    ISNULL(ee.FirstName, '') AS EditEmployeeName, 
    ISNULL(Promotions.MenuItemKeyList, '') AS MenuItemKeyList, 
    ISNULL(Promotions.GiftMenuItemKeyList, '') AS GiftMenuItemKeyList, 
    ISNULL(Promotions.MinimumOrderAmount, 0) AS MinimumOrderAmount, 
    ISNULL(Promotions.PromotionPriority, 0) AS PromotionPriority, 
    ISNULL(Promotions.AllowJointlyUse, 0) AS AllowJointlyUse, 
    Promotions.IgnoredPromotionKeys AS IgnoredPromotionKeys 
FROM Promotions WITH (NOLOCK) 
LEFT OUTER JOIN MenuGroups ON Promotions.MenuGroupKey = MenuGroups.MenuGroupKey 
LEFT OUTER JOIN MenuGroups AS GiftMenuGroups ON Promotions.GiftMenuGroupKey = GiftMenuGroups.MenuGroupKey 
LEFT OUTER JOIN MenuItems ON Promotions.MenuItemKey = MenuItems.MenuItemKey 
LEFT OUTER JOIN MenuItems AS GiftMenuItems ON Promotions.GiftMenuItemKey = GiftMenuItems.MenuItemKey 
LEFT OUTER JOIN EmployeeFiles AS ee ON Promotions.EditUserID = ee.AutoID 
LEFT OUTER JOIN EmployeeFiles AS ea ON Promotions.AddUserID = ea.AutoID;

-- 34. Menyu Məhsulları Sorğusu (Parametrik şablon - log boyu seçilən müxtəlif məhsullar üçün)
/*
İcra olunan @MenuItemKey dəyərləri:
- 'FDE08677-5B6D-4C9E-9CB0-F1CB2462BFBA'
- '4D3B6A48-D341-4B1D-A039-63F445804FC1'
- '31A13AAA-8A51-4806-817E-DEE236B50134'
- '117208E1-EF60-4F84-8CEB-B1CDD4C6358E'
- 'F03006FE-5D58-48DF-B97E-82F825080377'
- '7F9F17F6-E8E4-43CF-9C3A-B6BCAF60B0C1'
- '5AC6BE72-6858-426C-ADFE-041344F25987'
*/
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
SELECT MenuItems.AutoID, MenuItems.MenuItemID, isnull(MenuItems.MainMenuItemID,0) as MainMenuItemID, MenuItems.RevenueCenterTypeID, MenuItems.MenuItemText, MenuItems.MenuCategoryID, 
 MenuItems.MenuGroupID, MenuItems.DisplayIndex as DisplayIndex, MenuItems.DefaultUnitPrice, MenuItems.MenuItemCost, MenuItems.MenuItemDescription, 
 MenuItems.MenuItemNotification, MenuItems.MenuItemActive, MenuItems.MenuItemInStock, MenuItems.MenuItemTaxable, isnull(TaxGroups.TaxRate,8) AS TaxPercent, 
 MenuItems.MenuModifierID, MenuItems.MenuItemDiscountable, MenuItems.MenuItemPopUpHeaderID, MenuItems.MenuItemPopUpChoiceText, 
 MenuItems.HasModifierPopUps, isnull(MenuItems.SecLangMenuItemText,'') as SecLangMenuItemText, MenuItems.SecLangPopUpChoiceText, MenuItems.PictureName, MenuItems.ShowCaption, 
 isnull(MenuItems.IsComboMenu,0) as IsComboMenu,isnull(MenuItems.IsTopMenu,0) as IsTopMenu, MenuItems.ButtonColor, isnull(MenuItems.Barcode,'') as Barcode,isnull(MenuItems.Barcode2,'') as Barcode2, MenuItems.ItemDelCharge, MenuItems.ItemDelComp, 
 MenuItems.DineInPrice, MenuItems.BarTabPrice, MenuItems.TakeOutPrice, MenuItems.DriveThruPrice, MenuItems.DeliveryPrice, MenuItems.OrderByWeight, 
 MenuItems.PrintPizzaLabel, MenuItems.KitchenSortNumber, MenuItems.ModBuilderTemplateID, MenuItems.MenuItemTypeID, isnull(MenuItems.AccountingCode,'') as AccountingCode, 
 isnull(MenuItems.PrintOnLabel,0) as PrintOnLabel, MenuItems.UsedPrinterID1, MenuItems.UsedPrinterID2, MenuItems.UsedPrinterID3, MenuItems.UsedPrinterID4, MenuItems.UsedPrinterID5, 
 MenuItems.SecurityLevel, MenuItems.DeleteReason, MenuItems.CustomField1, MenuItems.CustomField2, MenuItems.CustomField3, MenuItems.CustomField4, 
 MenuItems.CustomField5, MenuItems.EditKey, MenuItems.SyncKey, MenuItems.BranchID, MenuItems.AddUserID, MenuItems.AddDateTime, MenuItems.EditUserID, 
 isnull(MenuItems.UseKds1,0) as UseKds1, isnull(MenuItems.UseKds2,0) as UseKds2, isnull(MenuItems.UseKds3,0) as UseKds3,isnull(MenuItems.UseKds4,0) as UseKds4, 
 isnull(MenuItems.UseKds5,0) as UseKds5,isnull(MenuItems.UseKds6,0) as UseKds6,isnull(MenuItems.UseKds7,0) as UseKds7,isnull(MenuItems.UseKds8,0) as UseKds8, 
 isnull(MenuItems.UseKds9,0) as UseKds9,isnull(MenuItems.UseKds10,0) as UseKds10, 
 MenuItems.CountDownDate, isnull(MenuItems.CountDownValue,0.0) as CountDownValue,isnull(MenuItems.CountDownActualResult,0.0) as CountDownActualResult,  MenuItems.EditDateTime, MenuCategories.MenuCategoryText, MenuSubCategories.MenuSubCategoryText, TaxGroups.GroupName AS TaxGroupText, 
 MenuModifierGroups.MenuModifierGroupText AS MenuModifierText, MenuItems.DisplayIndex AS MenuDisplayIndex, MenuItems.MenuItemKey, 
 MenuItems.MainMenuItemKey, MenuItems.MenuItemGlobalKey, MenuItems.MenuCategoryKey, MenuItems.MenuGroupKey, MenuItems.TaxGroupID, 
 MenuItems.TaxGroupKey, MenuItems.MenuModifierKey, MenuItems.MenuModifierForcedID, MenuItems.MenuModifierForcedKey, 
 MenuForcedModifierGroups.MenuModifierGroupText AS MenuForcedModifierText,efr_Branchs.BranchName 
 FROM MenuItems 
 LEFT OUTER JOIN MenuModifierGroups AS MenuForcedModifierGroups ON MenuItems.MenuModifierForcedKey = MenuForcedModifierGroups.MenuModifierGroupKey 
 LEFT OUTER JOIN MenuModifierGroups ON MenuItems.MenuModifierKey = MenuModifierGroups.MenuModifierGroupKey 
 LEFT OUTER JOIN MenuSubCategories ON MenuItems.MenuGroupKey = MenuSubCategories.MenuSubCategoryKey 
 LEFT OUTER JOIN MenuCategories ON MenuItems.MenuCategoryKey = MenuCategories.MenuCategoryKey 
 LEFT OUTER JOIN TaxGroups ON MenuItems.TaxGroupID = TaxGroups.TaxGroupID  LEFT OUTER JOIN efr_Branchs ON MenuItems.BranchID = efr_Branchs.BranchID  
 WHERE MenuItems.MenuItemKey = @MenuItemKey;

-- 35. Yalnız aktiv ödəniş metodlarının sorğusu
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
SELECT [AutoID] , [PaymentMethodID] , [PaymentMethodKey] , [PaymentName] , [IsDefault] , [DenyInvoice] , [DenyFiscal] , [DenyMoneyChange] , [ExchangeRate] , [AccountingCode] , [SecurityLevel] , [IsLocked] , [PaymentMethodActive] ,isnull(EffectRegister,1) as [EffectRegister] , [IsCoupon] , [IsAccountPayment] , [IsAccountSale] , [HideInRecievePayement] , [PictureName] , [DisplayIndex] , [ButtonColor], [EditKey] ,[SyncKey] ,ISNULL([ForcedInvoice],0) AS ForcedInvoice ,ISNULL([IsCampusCard],0) AS IsCampusCard ,[CampusCardServer] ,[BranchID] ,isnull([AskCustomerName],0) as AskCustomerName,isnull([GlobalBankCode],0) as GlobalBankCode  
 ,isnull(PaymentTypeID,2) as PaymentTypeID, 
 CASE PaymentTypeID WHEN 0 THEN ''  WHEN 1 THEN 'Nakit' WHEN 2 THEN 'Kredi Kartı' WHEN 3 THEN 'Yemek Çeki'  
 WHEN 4 THEN 'Cari Hesap' WHEN 5 THEN 'Para Puan' ELSE ''END AS PaymentTypeName, 
 ISNULL(CustomField1,'') AS CustomField1 ,ISNULL(CustomField2,'') AS CustomField2 ,ISNULL(CustomField3,'') AS CustomField3 ,ISNULL(CustomField4,'') AS CustomField4 ,ISNULL(CustomField5,'') AS CustomField5 
 FROM [PaymentMethods]  
 WHERE isnull(PaymentMethodActive,0)=1 
 ORDER BY PaymentMethodID;

-- 36. Əsas Sifarişin Yazılması (OrderHeaders)
DELETE FROM OrderHeaders WHERE OrderKey = 'AB41DEA4-8D63-4B5C-9A35-435205EF0C6E';

INSERT INTO [OrderHeaders] (
    [OrderID] ,[ReceiptNo] ,[MainOrderKey] ,[MainOrderID] ,[OrderTypeSourceID] ,[OrderTypeSourceExternalNo] ,
    [OrderKey] ,[OrderDateTime] ,[EmployeeID] ,[StationID] ,[RevenueCenterTypeID] ,[OrderType] ,
    [DineInTableID] ,[CustomerID] ,[DeliveryCharge] ,[DeliveryComp] ,[DeliveryZoneID] ,[DriverEmployeeID] ,
    [DriverDepartureTime] ,[DriverArrivalTime] ,[OnHoldUntilTime] ,[SalesTaxRate] ,[DiscountID] ,
    [DiscountLineAmount] ,[DiscountOrderAmount] ,[DiscountCashAmount] ,[DiscountTotalAmount] ,[OrderStatus] ,
    [BonusAmountUsed] ,[BonusAmountEarned] ,[BonusID] ,[BonusCustomerID] ,[AmountDue] ,[PackagerAlreadyPrinted] ,
    [GuestCheckPrinted] ,[GuestCheckPrintCount] ,[AdditionPrinted] ,[AdditionPrintedLineCount] ,[SurchargeID] ,
    [SurchargeLineAmount] ,[SurchargeOrderAmount] ,[SurchargeCashAmount] ,[SurchargeTotalAmount] ,[ComplimentaryAmount] ,
    [SubTotal] ,[OrderCost] ,[GratuityPercent] ,[CashGratuity] ,[SalesTaxAmount] ,[DriveThruComplete] ,
    [BarTabName] ,[TableReady] ,[GuestNumber] ,[SpecificCustomerName] ,[OrderPhone] ,[InvoicePrinted] ,
    [FiscalPrinted] ,[OrderNotes] ,[OrderExternalNotes] ,[LineDeleted] ,[DeleteReason] ,[CustomField1] ,
    [CustomField2] ,[CustomField3] ,[CustomField4] ,[CustomField5] ,[EditKey] ,[SyncKey] ,
    [BranchID] ,[LockData] ,[LockStationID] ,[AddUserID] ,[AddDateTime] ,[EditUserID] ,
    [EditDateTime] ,[CustomerKey] ,[DiscountKey] ,[EmployeeKey] ,[EmployeeName] ,[DineInTableName] ,
    [CustomerName] ,[AddUserName] ,[EditUserName] ,[DiscountAmountValue] ,[DiscountBasisValue] ,[FiscalKey] ,
    [InvoiceDetail] ,[RetailData] ,[DiscountUserName], [PaperNumber], [PosVersion], 
    [ReturnType], [ReturnOrderNo], [ReturnCustomerName], [ReturnCustomerAddress], [ReturnTaxNumber], 
    [ReturnTaxOffice], [ReturnSerialNo], [ReturnReason], [ReturnReasonCode], [OrderCounter]
) VALUES (
    0, dbo.getReceiptNo(), NULL, 0, 1, NULL, 
    'AB41DEA4-8D63-4B5C-9A35-435205EF0C6E', GETDATE(), 106, 1, 1, 5, 
    0, 9, 0, 0, 0, 0, 
    NULL, NULL, NULL, 0, 0, 
    0, 0, 0, 0, 1, 
    0, 0, 0, 0, 38, 0, 
    0, 0, 0, 0, 0, 
    10032718, 0, 0, 0, 0, 
    38, 0, 0, 0, 0, NULL, 
    N'NAĞD', 1, 1, N'ZAUR MAHMUDOV', N'0553946829', 0, 
    NULL, NULL, NULL, 0, NULL, NULL, 
    NULL, NULL, NULL, NULL, 'FC956F29-8B1F-4759-AC31-2CEB8B823DBF', '8DFA5F2A-A7F4-4DCD-BE77-6DDEC5F942C1', 
    422, NULL, NULL, 106, GETDATE(), NULL, 
    NULL, 'D3FF3A39-C77F-4A0C-BE37-3F5173B53ADC', NULL, '4740D379-BF42-4288-BF67-4B46D795F77D', N'NUSHPOS', NULL, 
    N'ZAUR MAHMUDOV', N'NUSHPOS', NULL, 0, NULL, NULL, 
    NULL, NULL, NULL, NULL, N'1.0.0.32718', 
    NULL, NULL, NULL, NULL, NULL, 
    NULL, NULL, NULL, NULL, 0
);

-- 37. Sifariş Sətirlərinin Əlavə Edilməsi (OrderTransactions)
DELETE FROM OrderTransactions WHERE TransactionKey = @TransactionKey;

INSERT INTO [OrderTransactions] (
    [TransactionDateTime], [TransactionID], [TransactionKey], [OrderKey], [OrderID], [OrderDateTime], 
    [StationID], [EmployeeID], [RevenueCenterTypeID], [MenuItemID], [MenuItemKey], [MenuItemText], 
    [MenuItemGroupText], [MenumItemCategoryText], [MenuItemUnitPrice], [MenuItemCost], [Quantity], 
    [ExtendedPrice], [DiscountID], [DiscountKey], [DiscountLineAmount], [DiscountCashAmount], 
    [DiscountTotalAmount], [TransactionStatus], [NotificationStatus], [AdditionLinePrinted], [TaxPercent], 
    [RoundID], [Mod1ID], [Mod1Cost], [Mod2ID], [Mod2Cost], [Mod3ID], [Mod3Cost], [Mod4ID], [Mod4Cost], 
    [Mod5ID], [Mod5Cost], [Mod6ID], [Mod6Cost], [Mod7ID], [Mod7Cost], [Mod8ID], [Mod8Cost], [Mod9ID], 
    [Mod9Cost], [Mod10ID], [Mod10Cost], [Mod11ID], [Mod11Cost], [Mod12ID], [Mod12Cost], [Mod13ID], 
    [Mod13Cost], [Mod14ID], [Mod14Cost], [Mod15ID], [Mod15Cost], [Mod16ID], [Mod16Cost], [Mod17ID], 
    [Mod17Cost], [Mod18ID], [Mod18Cost], [Mod19ID], [Mod19Cost], [Mod20ID], [Mod20Cost], [SeatNumber], 
    [OnHoldUntilTime], [Notes], [SalesTaxAmount], [TaxPercentReduction], [UsedPrinterID1], [UsedPrinterID2], 
    [UsedPrinterID3], [UsedPrinterID4], [UsedPrinterID5], [UsedPrinterComplated1], [UsedPrinterComplated2], 
    [UsedPrinterComplated3], [UsedPrinterComplated4], [UsedPrinterComplated5], [LineDeleted], [DeleteReason], 
    [CustomField1], [CustomField2], [CustomField3], [CustomField4], [CustomField5], [EditKey], [SyncKey], 
    [BranchID], [AddUserID], [AddDateTime], [AddUserName], [EmployeeName], [SplitDetail], [ExternalOrderStatus], 
    [PromotionKey], [DiscountAmountValue], [DiscountBasisValue], [EditUserID], [EditUserName], [EditDateTime], 
    [UsedDiscountName], [PromotionName], [RetailData], [OrderByWeight], [DiscountUserName], [OwnerKey], 
    [PromotionCount], [PromotionAmount], [AccountingCode], [PosVersion]
) VALUES (
    GETDATE(), @TransactionID, @TransactionKey, @OrderKey, @OrderID, @OrderDateTime, 
    @StationID, @EmployeeID, @RevenueCenterTypeID, @MenuItemID, @MenuItemKey, N'DÖNƏR ÇÖRƏKDƏ', 
    N'DÖNƏR', N'MƏTBAX', @MenuItemUnitPrice, @MenuItemCost, @Quantity, 
    @ExtendedPrice, @DiscountID, @DiscountKey, @DiscountLineAmount, @DiscountCashAmount, 
    @DiscountTotalAmount, @TransactionStatus, @NotificationStatus, @AdditionLinePrinted, @TaxPercent, 
    @RoundID, @Mod1ID, @Mod1Cost, @Mod2ID, @Mod2Cost, @Mod3ID, @Mod3Cost, @Mod4ID, @Mod4Cost, 
    @Mod5ID, @Mod5Cost, @Mod6ID, @Mod6Cost, @Mod7ID, @Mod7Cost, @Mod8ID, @Mod8Cost, @Mod9ID, 
    @Mod9Cost, @Mod10ID, @Mod10Cost, @Mod11ID, @Mod11Cost, @Mod12ID, @Mod12Cost, @Mod13ID, 
    @Mod13Cost, @Mod14ID, @Mod14Cost, @Mod15ID, @Mod15Cost, @Mod16ID, @Mod16Cost, @Mod17ID, 
    @Mod17Cost, @Mod18ID, @Mod18Cost, @Mod19ID, @Mod19Cost, @Mod20ID, @Mod20Cost, @SeatNumber, 
    @OnHoldUntilTime, @Notes, @SalesTaxAmount, @TaxPercentReduction, @UsedPrinterID1, @UsedPrinterID2, 
    @UsedPrinterID3, @UsedPrinterID4, @UsedPrinterID5, @UsedPrinterComplated1, @UsedPrinterComplated2, 
    @UsedPrinterComplated3, @UsedPrinterComplated4, @UsedPrinterComplated5, @LineDeleted, @DeleteReason, 
    @CustomField1, @CustomField2, @CustomField3, @CustomField4, @CustomField5, @EditKey, NEWID(), 
    @BranchID, @AddUserID, GETDATE(), @AddUserName, @EmployeeName, @SplitDetail, ISNULL(@TransactionStatus, 0) - 1, 
    @PromotionKey, @DiscountAmountValue, @DiscountBasisValue, NULL, NULL, NULL, 
    @UsedDiscountName, @PromotionName, @RetailData, @OrderByWeight, @DiscountUserName, @OwnerKey, 
    @PromotionCount, @PromotionAmount, @AccountingCode, @PosVersion
);