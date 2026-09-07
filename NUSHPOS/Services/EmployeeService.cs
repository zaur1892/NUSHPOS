using Dapper;
using NUSHPOS.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NUSHPOS.Services;

public class EmployeeService
{
    private readonly DatabaseService _databaseService;

    public EmployeeService(DatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    public async Task<Employee?> LoginAsync(string accessCode)
    {
        using var connection = _databaseService.CreateConnection();
        const string sql = @"
            SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
            SELECT TOP 1
                EmployeeFiles.AutoID, EmployeeFiles.EmployeeKey, EmployeeFiles.EmployeeID,
                EmployeeFiles.FirstName, EmployeeFiles.LastName,
                EmployeeFiles.EmployeeActive, EmployeeFiles.JobTitleID, EmployeeFiles.SecurityLevel,
                EmployeeFiles.AccessCode, EmployeeFiles.SmarCardCode, EmployeeFiles.MifareCardCode,
                EmployeeFiles.PhoneNumber, EmployeeFiles.UseStaffBank, EmployeeFiles.NoCashierOut,
                EmployeeFiles.EmployeeIsDriver, EmployeeFiles.IsAServer, EmployeeFiles.UseHostess,
                EmployeeTitles.TitleName as JobTitleText
            FROM EmployeeFiles
            LEFT OUTER JOIN EmployeeTitles ON EmployeeFiles.JobTitleID = EmployeeTitles.TitleID
            WHERE (EmployeeFiles.AccessCode = @AccessCode OR EmployeeFiles.MifareCardCode = @AccessCode OR EmployeeFiles.SmarCardCode = @AccessCode) 
              AND ISNULL(EmployeeFiles.EmployeeActive, 0) = 1;";
        return await connection.QueryFirstOrDefaultAsync<Employee>(sql, new { AccessCode = accessCode });
    }

    public async Task<List<EmployeeTitle>> GetTitlesAsync()
    {
        using var connection = _databaseService.CreateConnection();
        const string sql = @"
            SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
            SELECT [TitleID], [TitleName], [EditKey], [SyncKey]
            FROM [EmployeeTitles];";
        var result = await connection.QueryAsync<EmployeeTitle>(sql);
        return result.ToList();
    }

    public async Task<List<string>> GetLanguagesAsync()
    {
        using var connection = _databaseService.CreateConnection();
        const string sql = @"
            SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
            SELECT [ParamValue] FROM [Params]
            WHERE ParamName LIKE 'lang%';";
        var result = await connection.QueryAsync<string>(sql);
        return result.ToList();
    }

    public async Task<List<Employee>> GetActiveEmployeesAsync()
    {
        using var connection = _databaseService.CreateConnection();
        const string sql = @"
            SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
            SELECT
                EmployeeFiles.AutoID, EmployeeFiles.EmployeeKey, EmployeeFiles.EmployeeID,
                EmployeeFiles.FirstName, EmployeeFiles.LastName,
                EmployeeFiles.SocialSecurityNumber, EmployeeFiles.SmarCardCode,
                EmployeeFiles.MifareCardCode, EmployeeFiles.MailingAddress, EmployeeFiles.MailingZipCode,
                EmployeeFiles.DateHired, EmployeeFiles.DateReleased,
                EmployeeFiles.EmployeeActive, EmployeeFiles.JobTitleID, EmployeeFiles.SecurityLevel,
                EmployeeFiles.AccessCode, EmployeeFiles.TipsReceived,
                EmployeeFiles.PayBasis, EmployeeFiles.PayRate, EmployeeFiles.ScanCode,
                isnull(EmployeeFiles.DriverLicenseNumber,'') as DriverLicenseNumber,
                EmployeeFiles.DriverLicenseExpires,
                EmployeeFiles.CarInsurancePolicyCarrier, EmployeeFiles.CarInsurancePolicyNumber,
                EmployeeFiles.CarInsurancePolicyExpires, EmployeeFiles.CarInsurancePolicyNotes,
                isnull(EmployeeFiles.PrefUserInterfaceLocale,'-') as PrefUserInterfaceLocale,
                EmployeeFiles.EmployeeNotes, EmployeeFiles.OrderEntryUseSecLang,
                EmployeeFiles.EmployeeIsDriver, EmployeeFiles.DefaultOEMenuGroupID,
                EmployeeFiles.UseStaffBank, EmployeeFiles.ScheduleNotEnforced,
                EmployeeFiles.UseHostess, EmployeeFiles.IsAServer, EmployeeFiles.IsOffline,
                EmployeeFiles.NoCashierOut, EmployeeFiles.EditTimestamp,
                EmployeeFiles.PhoneNumber, EmployeeFiles.RevenueCenterTypeID,
                EmployeeFiles.DeleteReason,
                EmployeeFiles.CustomField1, EmployeeFiles.CustomField2,
                EmployeeFiles.CustomField3, EmployeeFiles.CustomField4, EmployeeFiles.CustomField5,
                EmployeeFiles.EditKey, EmployeeFiles.SyncKey,
                EmployeeFiles.BranchID, EmployeeFiles.AddUserID, EmployeeFiles.AddDateTime,
                EmployeeFiles.EditUserID, EmployeeFiles.EditDateTime,
                EmployeeTitles.TitleName as JobTitleText,
                isnull(EmployeeFiles.MonthlyDinnerFee,0) as MonthlyDinnerFee,
                CAST(0 AS BIT) AS IsChecked
            FROM EmployeeFiles
            LEFT OUTER JOIN EmployeeTitles ON EmployeeFiles.JobTitleID = EmployeeTitles.TitleID
            WHERE isnull(EmployeeActive,0)=1
              AND isnull(EmployeeFiles.SecurityLevel,0)<=10;";
        var result = await connection.QueryAsync<Employee>(sql);
        return result.ToList();
    }

    public async Task<List<Employee>> GetFormerEmployeesAsync()
    {
        using var connection = _databaseService.CreateConnection();
        const string sql = @"
            SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
            SELECT
                EmployeeFiles.AutoID, EmployeeFiles.EmployeeKey, EmployeeFiles.EmployeeID,
                EmployeeFiles.FirstName, EmployeeFiles.LastName,
                EmployeeFiles.SocialSecurityNumber, EmployeeFiles.SmarCardCode,
                EmployeeFiles.MifareCardCode, EmployeeFiles.MailingAddress, EmployeeFiles.MailingZipCode,
                EmployeeFiles.DateHired, EmployeeFiles.DateReleased,
                EmployeeFiles.EmployeeActive, EmployeeFiles.JobTitleID, EmployeeFiles.SecurityLevel,
                EmployeeFiles.AccessCode, EmployeeFiles.TipsReceived,
                EmployeeFiles.PayBasis, EmployeeFiles.PayRate, EmployeeFiles.ScanCode,
                isnull(EmployeeFiles.DriverLicenseNumber,'') as DriverLicenseNumber,
                EmployeeFiles.DriverLicenseExpires,
                EmployeeFiles.CarInsurancePolicyCarrier, EmployeeFiles.CarInsurancePolicyNumber,
                EmployeeFiles.CarInsurancePolicyExpires, EmployeeFiles.CarInsurancePolicyNotes,
                isnull(EmployeeFiles.PrefUserInterfaceLocale,'-') as PrefUserInterfaceLocale,
                EmployeeFiles.EmployeeNotes, EmployeeFiles.OrderEntryUseSecLang,
                EmployeeFiles.EmployeeIsDriver, EmployeeFiles.DefaultOEMenuGroupID,
                EmployeeFiles.UseStaffBank, EmployeeFiles.ScheduleNotEnforced,
                EmployeeFiles.UseHostess, EmployeeFiles.IsAServer, EmployeeFiles.IsOffline,
                EmployeeFiles.NoCashierOut, EmployeeFiles.EditTimestamp,
                EmployeeFiles.PhoneNumber, EmployeeFiles.RevenueCenterTypeID,
                EmployeeFiles.DeleteReason,
                EmployeeFiles.CustomField1, EmployeeFiles.CustomField2,
                EmployeeFiles.CustomField3, EmployeeFiles.CustomField4, EmployeeFiles.CustomField5,
                EmployeeFiles.EditKey, EmployeeFiles.SyncKey,
                EmployeeFiles.BranchID, EmployeeFiles.AddUserID, EmployeeFiles.AddDateTime,
                EmployeeFiles.EditUserID, EmployeeFiles.EditDateTime,
                EmployeeTitles.TitleName as JobTitleText,
                isnull(EmployeeFiles.MonthlyDinnerFee,0) as MonthlyDinnerFee,
                CAST(0 AS BIT) AS IsChecked
            FROM EmployeeFiles
            LEFT OUTER JOIN EmployeeTitles ON EmployeeFiles.JobTitleID = EmployeeTitles.TitleID
            WHERE isnull(EmployeeActive,0)=0
              AND isnull(EmployeeFiles.SecurityLevel,0)<=10;";
        var result = await connection.QueryAsync<Employee>(sql);
        return result.ToList();
    }

    public async Task SaveEmployeeAsync(Employee emp)
    {
        using var connection = _databaseService.CreateConnection();
        if (emp.AutoID == 0)
        {
            // INSERT
            const string sql = @"
                INSERT INTO EmployeeFiles
                    (FirstName, LastName, JobTitleID, SecurityLevel, AccessCode,
                     SmarCardCode, MifareCardCode, PhoneNumber, EmployeeActive,
                     EmployeeNotes, UseStaffBank, NoCashierOut, EmployeeIsDriver,
                     IsAServer, UseHostess, MonthlyDinnerFee, PrefUserInterfaceLocale,
                     AddDateTime, EditDateTime)
                VALUES
                    (@FirstName, @LastName, @JobTitleID, @SecurityLevel, @AccessCode,
                     @SmarCardCode, @MifareCardCode, @PhoneNumber, @EmployeeActive,
                     @EmployeeNotes, @UseStaffBank, @NoCashierOut, @EmployeeIsDriver,
                     @IsAServer, @UseHostess, @MonthlyDinnerFee, @PrefUserInterfaceLocale,
                     GETDATE(), GETDATE());";
            await connection.ExecuteAsync(sql, emp);
        }
        else
        {
            // UPDATE
            const string sql = @"
                UPDATE EmployeeFiles SET
                    FirstName = @FirstName,
                    LastName = @LastName,
                    JobTitleID = @JobTitleID,
                    SecurityLevel = @SecurityLevel,
                    AccessCode = @AccessCode,
                    SmarCardCode = @SmarCardCode,
                    MifareCardCode = @MifareCardCode,
                    PhoneNumber = @PhoneNumber,
                    EmployeeActive = @EmployeeActive,
                    EmployeeNotes = @EmployeeNotes,
                    UseStaffBank = @UseStaffBank,
                    NoCashierOut = @NoCashierOut,
                    EmployeeIsDriver = @EmployeeIsDriver,
                    IsAServer = @IsAServer,
                    UseHostess = @UseHostess,
                    MonthlyDinnerFee = @MonthlyDinnerFee,
                    PrefUserInterfaceLocale = @PrefUserInterfaceLocale,
                    EditDateTime = GETDATE()
                WHERE AutoID = @AutoID;";
            await connection.ExecuteAsync(sql, emp);
        }
    }

    public async Task DeleteEmployeeAsync(int autoId)
    {
        using var connection = _databaseService.CreateConnection();
        const string sql = "UPDATE EmployeeFiles SET EmployeeActive=0, DateReleased=GETDATE() WHERE AutoID=@AutoID;";
        await connection.ExecuteAsync(sql, new { AutoID = autoId });
    }
}