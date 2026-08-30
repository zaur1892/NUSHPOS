-- 1. Hesabat məlumatlarının oxunması (ReportKey üzrə)
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
SELECT Reports.AutoID, Reports.ReportID, Reports.ReportKey, Reports.ReportName, Reports.ReportCategoryID, Reports.ReportActive, Reports.IsPosReport, ReportCategories.CategoryName 
 ,Reports.ReportTypeID,Reports.SecurityLevel,Reports.EditKey ,Reports.SyncKey FROM Reports  
 LEFT OUTER JOIN ReportCategories ON Reports.ReportCategoryID = ReportCategories.ReportCategoryID WHERE Reports.ReportKey='250F04F2-0334-49F6-A4A2-4262DBBE02E6';[cite: 4]

-- 2. Cari server vaxtının oxunması
SELECT getdate() AS serverDatetime;[cite: 4]

-- 3. Hesabat sorğusunun oxunması və cədvəl adlarının əvəzlənməsi
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
SELECT [AutoID], [ReportQueryID], [ReportQueryKey], [ReportID], [ReportKey], [QueryName], 

	REPLACE(
	REPLACE(
	REPLACE(
	REPLACE( 
	REPLACE( 
	REPLACE(
	REPLACE(
	REPLACE(
	REPLACE(
	REPLACE(
	CAST ( [QueryData] AS NVARCHAR ( MAX )), 
	'GlobalPaymentMethods','PaymentMethods'),
	'GlobalEmployeeFiles','EmployeeFiles'),
	'GlobalMenuItems','MenuItems'),
	'GlobalPromotions','Promotions'),
	'GlobalDiscounts','Discounts'),
	'PaymentMethods', 'PaymentMethods'), 
	'EmployeeFiles', 'EmployeeFiles'),
	'MenuItems', 'MenuItems'),
	'Promotions', 'Promotions'),
	'Discounts', 'Discounts') 
	AS QueryData

, [IsDefault], [EditKey], [SyncKey] FROM [ReportQueries] Where ReportKey='250F04F2-0334-49F6-A4A2-4262DBBE02E6';[cite: 4]

-- 4. Ürün Bazlı Satış Hesabatı (Məhsul hərəkətləri və satış detalları)
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
SELECT 
t.orderdatetime as [CEK TARIHI],
h.orderdatetime AS [CEK ICERIKDEKI TARIH],
t.transactiondatetime [URUN HAREKET TARIHI],
t.MenuItemText as [Ürün],
       SUM(t.Quantity) AS Adet,
   SUM(t.ExtendedPrice)AS Tutar,
t.OrderID as [Çek NO],
     
       t.MenuItemGroupText as Grup,
       t.MenumItemCategoryText as Kategori,
     t.AddUserName AS Personel,
       CASE h.OrderType
            WHEN '1' THEN 'MASA SERVIS'
            WHEN '2' THEN 'İSME ÇEK'
            WHEN '3' THEN 'AL GÖTÜR'
            WHEN '4' THEN 'TEZGAH'
            WHEN '5' THEN 'PAKET SERVIS'
            ELSE 'TANIMSIZ'
       END AS 'Satış Türü'
FROM   OrderTransactions AS t
       LEFT OUTER JOIN OrderHeaders AS h
            ON  t.OrderKey = h.OrderKey
WHERE  t.LineDeleted = 0
AND t.OrderDateTime >= '2026-08-22 06:00:00'
AND t.OrderDateTime <= '2026-08-23 05:59:59'
     
GROUP BY
       MenuItemText,
       h.OrderType,
       t.ExtendedPrice,
       t.Quantity,
       t.MenuItemGroupText,
       t.MenumItemCategoryText,
t.OrderID,
t.orderdatetime,
h.orderdatetime,
t.transactiondatetime,
       t.AddUserName
ORDER BY
       t.MenuItemText ASC;[cite: 4]

-- 5. Hesabat sorğularının təmiz şəkildə oxunması
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
SELECT [AutoID], [ReportQueryID], [ReportQueryKey], [ReportID], [ReportKey], [QueryName], [QueryData], [IsDefault], [EditKey], [SyncKey] 
FROM [ReportQueries] 
Where ReportKey='250F04F2-0334-49F6-A4A2-4262DBBE02E6';[cite: 4]

-- 6. Bütün hesabat kateqoriyalarının oxunması
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
SELECT [ReportCategoryID] , [CategoryName] , [MainCategoryID] ,[EditKey] ,[SyncKey] 
FROM [ReportCategories];[cite: 4]

-- 7. Filial statusunun yoxlanması
SELECT isnull((SELECT TOP 1 isnull(ParamValue,'NO') FROM StoreSettings WHERE ParamKey='BranchStatus'),'NO') AS ParamValue;[cite: 4]

-- 8. Hesabat məlumatının yenilənməsi (UPDATE Reports)
UPDATE [Reports] 
SET [ReportID]=94,
    [ReportKey]='250F04F2-0334-49F6-A4A2-4262DBBE02E6',
    [ReportName]=N'ÜRÜN BAZLI SATIŞ RAPORU - LİSTE-EXCEL',
    [ReportCategoryID]=1,
    [ReportActive]=1,
    [SecurityLevel]=1,
    [ReportTypeID]=1,
    [IsPosReport]=0,
    [EditKey]='3ACA3C8E-524E-46F5-A925-26072194E5D6',
    [SyncKey]=NEWID()  
WHERE [AutoID]=94;[cite: 4]

-- 9. Hesabat struktur/sorğu məlumatının yenilənməsi (UPDATE ReportQueries)
UPDATE [ReportQueries] 
SET [ReportQueryID]=0,
    [ReportQueryKey]='9ED7CDB1-0FCD-42A2-AD8B-EC3AB156EFE8',
    [ReportID]=94,
    [ReportKey]='250F04F2-0334-49F6-A4A2-4262DBBE02E6',
    [QueryName]=N'Detail',
    [QueryData]=N'SELECT 
t.orderdatetime as [CEK TARIHI],
h.orderdatetime AS [CEK ICERIKDEKI TARIH],
t.transactiondatetime [URUN HAREKET TARIHI],
t.MenuItemText as [Ürün],
       SUM(t.Quantity) AS Adet,
   SUM(t.ExtendedPrice)AS Tutar,
t.OrderID as [Çek NO],
     
       t.MenuItemGroupText as Grup,
       t.MenumItemCategoryText as Kategori,
     t.AddUserName AS Personel,
       CASE h.OrderType
            WHEN ''1'' THEN ''MASA SERVIS''
            WHEN ''2'' THEN ''İSME ÇEK''
            WHEN ''3'' THEN ''AL GÖTÜR''
            WHEN ''4'' THEN ''TEZGAH''
            WHEN ''5'' THEN ''PAKET SERVIS''
            ELSE ''TANIMSIZ''
       END AS ''Satış Türü''
FROM   OrderTransactions AS t
       LEFT OUTER JOIN OrderHeaders AS h
            ON  t.OrderKey = h.OrderKey
WHERE  t.LineDeleted = 0
AND t.OrderDateTime >= @date1
AND t.OrderDateTime <= @date2
     
GROUP BY
       MenuItemText,
       h.OrderType,
       t.ExtendedPrice,
       t.Quantity,
       t.MenuItemGroupText,
       t.MenumItemCategoryText,
t.OrderID,
t.orderdatetime,
h.orderdatetime,
t.transactiondatetime,
       t.AddUserName
ORDER BY
       t.MenuItemText ASC
',
    [IsDefault]=1,
    [EditKey]=NULL,
    [SyncKey]='3158BB43-A3FC-468C-9FA2-D95423C90816'  
WHERE [AutoID]=160;[cite: 4]

-- 10. Bütün hesabatların və onların kateqoriyalarının oxunması
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
SELECT Reports.AutoID, Reports.ReportID, Reports.ReportKey, Reports.ReportName, Reports.ReportCategoryID, Reports.ReportActive, Reports.IsPosReport, ReportCategories.CategoryName 
 ,Reports.ReportTypeID,Reports.SecurityLevel,Reports.EditKey ,Reports.SyncKey FROM Reports  
 LEFT OUTER JOIN ReportCategories ON Reports.ReportCategoryID = ReportCategories.ReportCategoryID;[cite: 4]

-- 11. Əsas hesabat kateqoriyalarının oxunması (MainCategoryID = 0)
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
SELECT [ReportCategoryID] , [CategoryName] , [MainCategoryID] ,[EditKey] ,[SyncKey] 
FROM [ReportCategories] 
Where [MainCategoryID]=0;[cite: 4]