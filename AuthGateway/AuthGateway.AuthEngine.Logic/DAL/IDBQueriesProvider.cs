using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AuthGateway.AuthEngine.Logic.DAL
{
	public interface IDBQueriesProvider
	{
		DBQueries Get();
	}
}
