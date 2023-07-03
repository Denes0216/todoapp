using System.Data;

namespace TestSolution.Services;

public interface IDbService
{
    public IDbConnection CreateConnection();
}