using System.Data.SqlClient;

namespace AuthGateway.AuthEngine.Logic.DAL
{
	public interface IDBProvider
	{
		SqlConnection GetConnection();
	}
}
