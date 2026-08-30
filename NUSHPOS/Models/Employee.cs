namespace NUSHPOS.Models;

public class Employee
{
    public int AutoID { get; set; }
    public int EmployeeID { get; set; }
    public Guid? EmployeeKey { get; set; }
    public string? FirstName { get; set; } = "";
    public string? LastName { get; set; } = "";
    public string? SocialSecurityNumber { get; set; } = "";
    public string? SmarCardCode { get; set; } = "";
    public string? MifareCardCode { get; set; } = "";
    public string? MailingAddress { get; set; } = "";
    public string? MailingZipCode { get; set; } = "";
    public DateTime? DateHired { get; set; }
    public DateTime? DateReleased { get; set; }
    public bool? EmployeeActive { get; set; } = true;
    public int? JobTitleID { get; set; }
    public int? SecurityLevel { get; set; }
    public string? AccessCode { get; set; } = "";
    public bool? TipsReceived { get; set; }
    public int? PayBasis { get; set; }
    public decimal? PayRate { get; set; }
    public string? ScanCode { get; set; } = "";
    public string? DriverLicenseNumber { get; set; } = "";
    public DateTime? DriverLicenseExpires { get; set; }
    public string? CarInsurancePolicyCarrier { get; set; } = "";
    public string? CarInsurancePolicyNumber { get; set; } = "";
    public DateTime? CarInsurancePolicyExpires { get; set; }
    public string? CarInsurancePolicyNotes { get; set; } = "";
    public string? PrefUserInterfaceLocale { get; set; } = "-";
    public string? EmployeeNotes { get; set; } = "";
    public bool? OrderEntryUseSecLang { get; set; }
    public bool? EmployeeIsDriver { get; set; }
    public int? DefaultOEMenuGroupID { get; set; }
    public bool? UseStaffBank { get; set; }
    public bool? ScheduleNotEnforced { get; set; }
    public bool? UseHostess { get; set; }
    public bool? IsAServer { get; set; }
    public bool? IsOffline { get; set; }
    public bool? NoCashierOut { get; set; }
    public DateTime? EditTimestamp { get; set; }
    public string? PhoneNumber { get; set; } = "";
    public int? RevenueCenterTypeID { get; set; }
    public string? DeleteReason { get; set; } = "";
    public string? CustomField1 { get; set; } = "";
    public string? CustomField2 { get; set; } = "";
    public string? CustomField3 { get; set; } = "";
    public string? CustomField4 { get; set; } = "";
    public string? CustomField5 { get; set; } = "";
    public Guid? EditKey { get; set; }
    public Guid? SyncKey { get; set; }
    public int? BranchID { get; set; }
    public int? AddUserID { get; set; }
    public DateTime? AddDateTime { get; set; }
    public int? EditUserID { get; set; }
    public DateTime? EditDateTime { get; set; }
    public decimal? MonthlyDinnerFee { get; set; }
    public bool IsChecked { get; set; }

    // Joined field
    public string? JobTitleText { get; set; } = "";

    // Computed
    public string FullName => $"{FirstName} {LastName}".Trim();
}