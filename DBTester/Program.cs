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
                var designs = connection.Query("SELECT AutoID, DesignName, DocumentTypeID, IsDefault, LEN(CAST(DesignData AS NVARCHAR(MAX))) as DataLen FROM PrinterDesigns");
                foreach (var d in designs)
                {
                    Console.WriteLine($"ID: {d.AutoID}, Name: {d.DesignName}, Type: {d.DocumentTypeID}, IsDefault: {d.IsDefault}, Len: {d.DataLen}");
                }
            }
        }
    }
}
