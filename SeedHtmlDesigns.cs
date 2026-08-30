using System;
using System.Data.SqlClient;
using System.Text;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        string connStr = "Server=TERMSRV;Database=infinia;Trusted_Connection=True;";
        using (SqlConnection conn = new SqlConnection(connStr))
        {
            conn.Open();
            
            // Get all ReportTypeID = 1
            SqlCommand getReports = new SqlCommand("SELECT ReportID, ReportKey, ReportName FROM Reports WHERE ReportTypeID = 1", conn);
            using (SqlDataReader reader = getReports.ExecuteReader())
            {
                var reports = new List<Tuple<int, Guid, string>>();
                while (reader.Read())
                {
                    reports.Add(new Tuple<int, Guid, string>(reader.GetInt32(0), reader.GetGuid(1), reader.GetString(2)));
                }
                reader.Close();

                foreach (var r in reports)
                {
                    int reportId = r.Item1;
                    Guid reportKey = r.Item2;
                    string reportName = r.Item3;

                    // Get queries for this report
                    SqlCommand getQueries = new SqlCommand("SELECT QueryName FROM ReportQueries WHERE ReportID = @ID", conn);
                    getQueries.Parameters.AddWithValue("@ID", reportId);
                    List<string> queries = new List<string>();
                    using (SqlDataReader qReader = getQueries.ExecuteReader())
                    {
                        while (qReader.Read())
                        {
                            string qName = qReader.GetString(0);
                            if (qName != "XAML_TEMPLATE")
                                queries.Add(qName);
                        }
                    }

                    // Build HTML
                    StringBuilder html = new StringBuilder();
                    html.AppendLine("<!DOCTYPE html>");
                    html.AppendLine("<html>");
                    html.AppendLine("<head>");
                    html.AppendLine("    <meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\" />");
                    html.AppendLine("    <style>");
                    html.AppendLine("        body { font-family: 'Segoe UI', Tahoma, sans-serif; padding: 20px; background-color: #f9f9f9; }");
                    html.AppendLine("        .report-header { text-align: center; color: #d32f2f; margin-bottom: 30px; font-weight:bold; font-size:24px; }");
                    html.AppendLine("        .section { background: white; padding: 15px; border-radius: 8px; box-shadow: 0 2px 4px rgba(0,0,0,0.1); margin-bottom: 20px; }");
                    html.AppendLine("        .section-title { font-weight: bold; font-size: 18px; border-bottom: 2px solid #ffc107; padding-bottom: 5px; margin-bottom: 15px; text-transform: uppercase; }");
                    html.AppendLine("    </style>");
                    html.AppendLine("</head>");
                    html.AppendLine("<body>");
                    html.AppendLine($"    <div class=\"report-header\">{reportName}</div>");
                    
                    foreach (var q in queries)
                    {
                        html.AppendLine("    <div class=\"section\">");
                        html.AppendLine($"        <div class=\"section-title\">{q}</div>");
                        html.AppendLine($"        {{{{{q}}}}}");
                        html.AppendLine("    </div>");
                    }
                    
                    html.AppendLine("</body>");
                    html.AppendLine("</html>");

                    // Save to ReportDesigns
                    SqlCommand del = new SqlCommand("DELETE FROM ReportDesigns WHERE ReportID = @ID", conn);
                    del.Parameters.AddWithValue("@ID", reportId);
                    del.ExecuteNonQuery();

                    SqlCommand ins = new SqlCommand("INSERT INTO ReportDesigns (ReportDesignKey, ReportID, ReportKey, DocumentTypeID, DesignName, DesignData, IsDefault) VALUES (NEWID(), @ID, @Key, 0, @Name, @Data, 0)", conn);
                    ins.Parameters.AddWithValue("@ID", reportId);
                    ins.Parameters.AddWithValue("@Key", reportKey);
                    ins.Parameters.AddWithValue("@Name", reportName);
                    ins.Parameters.AddWithValue("@Data", Encoding.UTF8.GetBytes(html.ToString()));
                    ins.ExecuteNonQuery();
                }
            }
            Console.WriteLine("Done.");
        }
    }
}
