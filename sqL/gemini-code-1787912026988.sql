-- ==============================================================================
-- 1. MAĞAZA TƏNZİMLƏMƏLƏRİNDƏKİ TAB ADLARI (Filtr ilə)
-- ==============================================================================
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;[cite: 3]

SELECT [TabName] 
FROM [StoreSettings] 
WHERE TabName <> 'GÜN SONU İŞLEMLERİ' 
  AND TabName <> 'PERSONEL' 
  AND TabName <> 'PARA PUAN' 
GROUP BY [TabName];[cite: 3]


-- ==============================================================================
-- 2. BÜTÜN MAĞAZA TƏNZİMLƏMƏLƏRİNİN DETALLI SİYAHISI
-- ==============================================================================
SELECT 
    [SettingsID], 
    [OrderID], 
    [TabName], 
    [GroupName], 
    [ParamName], 
    [ParamKey], 
    [ParamValue], 
    [DefaultValue], 
    [ParamType], 
    [EditKey], 
    [SyncKey], 
    [BranchID], 
    [AddUserID], 
    [AddDateTime], 
    [EditUserID], 
    [EditDateTime]  
FROM [StoreSettings]  
ORDER BY TabName, GroupName, OrderID;[cite: 3]