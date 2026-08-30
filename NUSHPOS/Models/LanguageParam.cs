namespace NUSHPOS.Models;

public class LanguageParam
{
    public string ParamName { get; set; } = "";
    public string ParamValue { get; set; } = "";
    public Guid? EditKey { get; set; }
    public Guid? SyncKey { get; set; }
}