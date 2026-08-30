namespace NUSHPOS.Models;

public class EmployeeTitle
{
    public int TitleID { get; set; }
    public string TitleName { get; set; } = "";
    public Guid? EditKey { get; set; }
    public Guid? SyncKey { get; set; }
}