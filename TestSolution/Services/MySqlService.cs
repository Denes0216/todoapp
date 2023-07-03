using System.Data;
using MySqlConnector;

namespace TestSolution.Services;

public class MySqlService : IDbService
{
    private readonly IConfiguration _configuration;

    public MySqlService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IDbConnection CreateConnection()
    {
        var connectionString = _configuration.GetConnectionString("DefaultConnection");
        return new MySqlConnection(connectionString);
    }
}