using System;
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
                var colors = connection.Query("SELECT PaymentName, ButtonColor FROM PaymentMethods WHERE PaymentMethodActive = 1");
                foreach (var color in colors)
                {
                    Console.WriteLine($"{color.PaymentName}: {color.ButtonColor} (Type: {(color.ButtonColor != null ? color.ButtonColor.GetType().Name : "null")})");
                }
            }
        }
    }
}
