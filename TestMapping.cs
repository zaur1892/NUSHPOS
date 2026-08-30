using System;
using System.Data.SqlClient;
using Dapper;
using System.Linq;

class Program
{
    public class ReportModel
    {
        public int AutoID { get; set; }
        public int ReportID { get; set; }
        public string ReportName { get; set; } = string.Empty;
        public int ReportCategoryID { get; set; }
        public int ReportTypeID { get; set; }
    }

    static void Main()
    {
        string connStr = "Server=TERMSRV;Database=infinia;Trusted_Connection=True;";
        using (SqlConnection conn = new SqlConnection(connStr))
        {
            conn.Open();
            string query = @"
SELECT 
    Reports.AutoID, Reports.ReportID, Reports.ReportKey, Reports.ReportName, Reports.ReportCategoryID, 
    Reports.ReportActive, Reports.IsPosReport, ReportCategories.CategoryName, Reports.ReportTypeID, 
    Reports.SecurityLevel, Reports.EditKey, Reports.SyncKey 
FROM Reports  
LEFT OUTER JOIN ReportCategories ON Reports.ReportCategoryID = ReportCategories.ReportCategoryID;";

            var reports = conn.Query<ReportModel>(query).ToList();
            var t1 = reports.Count(r => r.ReportTypeID == 1);
            var t2 = reports.Count(r => r.ReportTypeID == 2);
            var t0 = reports.Count(r => r.ReportTypeID == 0);
            
            Console.WriteLine($"Mapped reports: Total={reports.Count}, Type1={t1}, Type2={t2}, Type0={t0}");
        }
    }
}
