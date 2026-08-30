namespace NUSHPOS.Helpers;

public static class SessionManager
{
    public static int EmployeeID { get; set; }
    public static string EmployeeName { get; set; } = string.Empty;
    public static string EmployeeKey { get; set; } = string.Empty;
    public static int SecurityLevel { get; set; }
    public static int StationID { get; set; } = 1;
    public static int BranchID { get; set; } = 1;
    public static int? RegisterSessionID { get; set; }
    public static bool IsLoggedIn => EmployeeID > 0;
    
    public static void Clear()
    {
        EmployeeID = 0;
        EmployeeName = string.Empty;
        EmployeeKey = string.Empty;
        SecurityLevel = 0;
        RegisterSessionID = null;
    }
}
