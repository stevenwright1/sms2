using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using AuthGateway.Shared.Log;

namespace AuthGateway.AuthEngine.Logic.DAL
{
	public static class DBQueriesProvider
	{
		private static IDBQueriesProvider provider = null;

		public static void SetQueriesProviderIfNullProvider(IDBQueriesProvider provider)
		{
			if (DBQueriesProvider.provider == null)
				DBQueriesProvider.provider = provider;
		}
		public static void SetQueriesProvider(IDBQueriesProvider provider)
		{
			DBQueriesProvider.provider = provider;
		}

		public static DBQueries Get()
		{
			if (DBQueriesProvider.provider == null)
				return null;
			return DBQueriesProvider.provider.Get();
		}
	}

	public class DBQueriesProviderNew : IDBQueriesProvider
	{
		private string connectionString;

		public DBQueriesProviderNew(string connectionString)
		{
			this.connectionString = connectionString;
			Logger.Instance.WriteToLog(string.Format("DBQueriesProviderNew.Initialized with: '{0}'", connectionString), LogLevel.Debug);
		}

		public DBQueries Get()
		{
			return new DBQueries(this.connectionString);
		}
	}

	public class DBQueriesProviderSame : IDBQueriesProvider
	{
		private string connectionString;
		private DBQueries queries = null;
		private object lckObject = new object();

		public DBQueriesProviderSame(string connectionString)
		{
			this.connectionString = connectionString;			
		}

		public DBQueries Get()
		{
			if (this.queries == null)
			{
				lock (lckObject)
				{
					if (this.queries == null)
					{
						this.queries = new DBQueries(this.connectionString);
						this.queries.DiposeThis = false;
					}
				}
			}
			return this.queries;
		}
	}
}
