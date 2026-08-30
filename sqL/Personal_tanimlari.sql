-- 1. Vəzifələrin oxunması
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
SELECT [TitleID] , [TitleName] , [EditKey] , [SyncKey]  FROM [EmployeeTitles];

-- 2. Parametrlərin oxunması
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
SELECT [ParamName] , [ParamValue],[EditKey] ,[SyncKey] FROM [Params] where ParamName like 'lang%';

-- 3. İşçilərin siyahısının oxunması
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
DECLARE @EmployeeActive bit = 1;

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
 LEFT OUTER JOIN EmployeeTitles ON EmployeeFiles.JobTitleID = EmployeeTitles.TitleID  
 WHERE isnull(EmployeeActive,0)=@EmployeeActive and isnull(EmployeeFiles.SecurityLevel,0)<=10;
```[cite: 1]