using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NUSHPOS.Helpers;
using NUSHPOS.Services;
using NUSHPOS.ViewModels.Base;
using System.Threading.Tasks;
using Dapper;

namespace NUSHPOS.ViewModels;

public partial class OperationsViewModel : ViewModelBase
{
    private readonly NavigationService _navigationService;
    private readonly DatabaseService _databaseService;

    public OperationsViewModel(NavigationService navigationService, DatabaseService databaseService)
    {
        _navigationService = navigationService;
        _databaseService = databaseService;
        Title = "İşlemler";
        _ = InitializeOperationsAsync();
    }

    private async Task InitializeOperationsAsync()
    {
        using var connection = _databaseService.CreateConnection();
        
        // 1. Əməkdaş Məlumatlarının Oxunması
        string employeeQuery = @"
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
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
 LEFT OUTER JOIN EmployeeTitles ON EmployeeFiles.JobTitleID = EmployeeTitles.TitleID  Where EmployeeFiles.AutoID=@AutoID;
";
        var employee = await connection.QueryFirstOrDefaultAsync(employeeQuery, new { AutoID = SessionManager.EmployeeID });

        // 2. Giriş Qeydinin Yazılması
        string accessLogQuery = @"
INSERT INTO [AccessLogs]  ([BranchID] ,[LogDate] ,[StationID] ,[EmployeeID] ,[ActionName] ,[WrongPassword] ,[AdditionalInfo] ,[IsSuccess] ,[OrderKey] ,[TransactionKey] ,[AccessLogKey] ,[EditKey] ,[SyncKey] )
values (@BranchID ,getdate() ,@StationID ,@EmployeeID ,N'İŞLEMLER PENCERESİ' ,NULL ,NULL ,1 ,'00000000-0000-0000-0000-000000000000' ,'00000000-0000-0000-0000-000000000000' ,newid() ,newid() ,newid() );
";
        await connection.ExecuteAsync(accessLogQuery, new 
        { 
            BranchID = SessionManager.BranchID,
            StationID = SessionManager.StationID,
            EmployeeID = SessionManager.EmployeeID
        });

        // 3. Cədvəlin Yaradılması və Təmizlənməsi
        string createLangQuery = @"
IF (not EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'LanguageResource'))
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
 ) ON [PRIMARY];
 END;
DELETE FROM LanguageResource WHERE KeyField IS NULL;
DELETE FROM LanguageResource WHERE AutoID NOT IN (SELECT MIN(l.AutoID) FROM LanguageResource AS l GROUP BY l.KeyField);
";
        await connection.ExecuteAsync(createLangQuery);

        // 4. Dil Resurslarının Oxunması
        string readLangQuery = @"
SELECT [AutoID] , [KeyField] , [Turkish] , [English] , [Lang1] , [Lang2] , [Lang3] , [Lang4] , [Lang5],CAST(0 AS BIT) AS IsEdited,isnull(LangType,0) as LangType FROM [LanguageResource];
";
        var langResources = await connection.QueryAsync(readLangQuery);
    }

    [RelayCommand]
    private void GoBack()
    {
        _navigationService.NavigateTo<MainScreenViewModel>();
    }

    [RelayCommand]
    private void OpenReports()
    {
        _navigationService.NavigateTo<ReportsViewModel>();
    }
}
