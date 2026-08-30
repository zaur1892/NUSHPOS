using Microsoft.Data.SqlClient;
using System.Data;

namespace NUSHPOS.Services;

public class DatabaseService
{
    private const string ConnectionString = "Server=TERMSRV;Database=infinia;Trusted_Connection=True;TrustServerCertificate=True;";

    public IDbConnection CreateConnection()
    {
        return new SqlConnection(ConnectionString);
    }
}
