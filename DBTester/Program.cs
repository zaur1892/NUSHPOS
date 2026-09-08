using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Dapper;

namespace DBTester
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = "Server=TERMSRV;Database=infinia;Trusted_Connection=True;TrustServerCertificate=True;";
            using (var connection = new SqlConnection(connectionString))
            {
                string htmlContent = System.IO.File.ReadAllText(@"c:\Users\Zaur\source\repos\NUSHPOS\DBTester\Report35_Design.html");
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(htmlContent);
                int updated = connection.Execute("UPDATE ReportDesigns SET DesignData = @Data WHERE ReportID = 35", new { Data = bytes });
                Console.WriteLine($"Updated Report 35 in database: {updated} row(s) affected.");
            }
        }
    }
}
