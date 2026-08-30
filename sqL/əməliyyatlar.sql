-- 1. Əməkdaş Məlumatlarının Oxunması
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;[cite: 1]
SELECT EmployeeFiles.AutoID, EmployeeFiles.EmployeeKey, EmployeeFiles.EmployeeID, EmployeeFiles.FirstName, EmployeeFiles.LastName,  
 EmployeeFiles.SocialSecurityNumber, EmployeeFiles.SmarCardCode, EmployeeFiles.MifareCardCode, EmployeeFiles.MailingAddress, EmployeeFiles.MailingZipCode, EmployeeFiles.DateHired, EmployeeFiles.DateReleased,  
 EmployeeFiles.EmployeeActive, EmployeeFiles.JobTitleID, EmployeeFiles.SecurityLevel, EmployeeFiles.AccessCode, EmployeeFiles.TipsReceived,  
 EmployeeFiles.PayBasis, EmployeeFiles.PayRate, EmployeeFiles.ScanCode, isnull(EmployeeFiles.DriverLicenseNumber,'') as DriverLicenseNumber, EmployeeFiles.DriverLicenseExpires,  
 EmployeeFiles.CarInsurancePolicyCarrier, EmployeeFiles.CarInsurancePolicyNumber, EmployeeFiles.CarInsurancePolicyExpires,  
 EmployeeFiles.CarInsurancePolicyNotes, isnull(EmployeeFiles.PrefUserInterfaceLocale,'-') as PrefUserInterfaceLocale, EmployeeFiles.EmployeeNotes, EmployeeFiles.OrderEntryUseSecLang,  
 EmployeeFiles.EmployeeIsDriver, EmployeeFiles.DefaultOEMenuGroupID, EmployeeFiles.UseStaffBank, EmployeeFiles.ScheduleNotEnforced,  
 EmployeeFiles.UseHostess, EmployeeFiles.IsAServer, EmployeeFiles.IsOffline, EmployeeFiles.NoCashierOut, EmployeeFiles.EditTimestamp,  
 EmployeeFiles.PhoneNumber, EmployeeFiles.RevenueCenterTypeID, EmployeeFiles.DeleteReason, EmployeeFiles.CustomField1, EmployeeFiles.CustomField2,  
 EmployeeFiles.CustomField3, EmployeeFiles.CustomField4, EmployeeFiles.CustomField5, EmployeeFiles.EditKey, EmployeeFiles.SyncKey,  
 EmployeeFiles.BranchID, EmployeeFiles.AddUserID, EmployeeFiles.AddDateTime, EmployeeFiles.EditUserID, EmployeeFiles.EditDateTime,  
 EmployeeTitles.TitleName as JobTitleText, isnull(EmployeeFiles.MonthlyDinnerFee,0) as MonthlyDinnerFee,CAST(0 AS BIT) AS IsChecked 
 FROM EmployeeFiles  
 LEFT OUTER JOIN EmployeeTitles ON EmployeeFiles.JobTitleID = EmployeeTitles.TitleID  Where EmployeeFiles.AutoID=106;[cite: 1]

-- 2. Giriş Qeydinin Yazılması
INSERT INTO [AccessLogs]  ([BranchID] ,[LogDate] ,[StationID] ,[EmployeeID] ,[ActionName] ,[WrongPassword] ,[AdditionalInfo] ,[IsSuccess] ,[OrderKey] ,[TransactionKey] ,[AccessLogKey] ,[EditKey] ,[SyncKey] )[cite: 1]
values (422 ,getdate() ,1 ,106 ,N'İŞLEMLER PENCERESİ' ,NULL ,NULL ,1 ,'00000000-0000-0000-0000-000000000000' ,'00000000-0000-0000-0000-000000000000' ,newid() ,'F5CF3B7B-B1B0-4AB6-A9E2-EB1140779A5A' ,'615FB686-FE20-4312-B136-2768A46AB30A' );[cite: 1]

-- 3. Cədvəlin Yaradılması və Təmizlənməsi
IF (not EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'LanguageResource'))[cite: 1]
 BEGIN
 CREATE TABLE [dbo].[LanguageResource](
 [AutoID] [int] IDENTITY(1,1) NOT NULL,
 [KeyField] [nvarchar](150) NULL,
 [Turkish] [nvarchar](500) NULL,
 [English] [nvarchar](500) NULL,
 [Lang1] [nvarchar](500) NULL,
 [Lang2] [nvarchar](500) NULL,
 [Lang3] [nvarchar](500) NULL,
 [Lang4] [nvarchar](500) NULL,
 [Lang5] [nvarchar](500) NULL,
 [LangType] [int] NULL
 ) ON [PRIMARY];[cite: 1]
 END;[cite: 1]
DELETE FROM LanguageResource WHERE KeyField IS NULL;[cite: 1]
DELETE FROM LanguageResource WHERE AutoID NOT IN (SELECT MIN(l.AutoID) FROM LanguageResource AS l GROUP BY l.KeyField);[cite: 1]

-- 4. Dil Resurslarının Oxunması
SELECT [AutoID] , [KeyField] , [Turkish] , [English] , [Lang1] , [Lang2] , [Lang3] , [Lang4] , [Lang5],CAST(0 AS BIT) AS IsEdited,isnull(LangType,0) as LangType FROM [LanguageResource];[cite: 1]