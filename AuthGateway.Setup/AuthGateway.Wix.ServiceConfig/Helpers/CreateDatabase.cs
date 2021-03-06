using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace AuthGateway.Setup.SQLDB
{
	public class DatabaseHandlerException : Exception
	{
		public DatabaseHandlerException()
			: base()
		{
		}
		public DatabaseHandlerException(string message)
			: base(message)
		{
		}
		public DatabaseHandlerException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
	public class DatabaseStatus
	{
		public DatabaseStatus()
		{
			this.Exists = false;
			this.Empty = true;
			this.Version = string.Empty;
		}

		public bool Exists { get; set; }
		public bool Empty { get; set; }
		public string Version { get; set; }
	}

	public class DatabaseHandler
	{
		private Dictionary<string, string> upgradePaths;

		public DatabaseHandler()
		{
			var currentPaths = new Dictionary<string, string>(); ;
			string[] resourceNames = this.GetType().Assembly.GetManifestResourceNames();

			foreach (string resourceName in resourceNames)
			{
				if (Path.GetExtension(resourceName) != ".sql")
					continue;

				var parts = Path.GetFileNameWithoutExtension(resourceName).Split(new char[] { '.' });
				var version = string.Empty;

				double numTest = 0;
				foreach (var part in parts)
				{
					if (Double.TryParse(part, out numTest))
						version += part + ".";
				}
				version = version.TrimEnd(new char[] { '.' });
				currentPaths.Add(version, resourceName);
			}

			upgradePaths = new Dictionary<string, string>();
			var list = currentPaths.Keys.ToList();
			list.Sort();

			foreach (var key in list)
				upgradePaths.Add(key, currentPaths[key]);
		}
		public DatabaseStatus DoStatus(string connStringNoDb, string connStringDb, string dbname)
		{
			DatabaseStatus stat = new DatabaseStatus();
			stat.Exists = CheckDatabaseExists(connStringNoDb, dbname);
			stat.Empty = true;
			if (stat.Exists)
				stat.Empty = !CheckDatabaseHasSchema(connStringDb);
			if (!stat.Empty)
				stat.Version = GetDatabaseVersion(connStringDb);
			return stat;
		}

		public string getLastPath()
		{
			string last = string.Empty;
			foreach (KeyValuePair<string, string> upgradePath in upgradePaths)
			{
				last = upgradePath.Key;
			}
			return last;
		}

		private bool CheckDatabaseHasSchema(string connStringDb)
		{
			try
			{
				using (SqlConnection conn = new SqlConnection(connStringDb))
				{
					conn.Open();
					using (SqlCommand com = new SqlCommand(@"SELECT VALUE FROM SETTINGS WHERE OBJECT = 'AUTHGATEWAY' AND SETTING = 'VERSION'", conn))
					{
						string version = Convert.ToString(com.ExecuteScalar());
					}
				}
				return true;
			}
			catch
			{
				return false;
			}
		}

		private string GetDatabaseVersion(string connStringDb)
		{
			using (SqlConnection conn = new SqlConnection(connStringDb))
			{
				conn.Open();
				using (SqlCommand com = new SqlCommand(@"SELECT VALUE FROM SETTINGS WHERE OBJECT = 'AUTHGATEWAY' AND SETTING = 'VERSION'", conn))
				{
					return Convert.ToString(com.ExecuteScalar());
				}
			}
		}

		private static bool CheckDatabaseExists(string connString, string dbname)
		{
			string sqlCreateDBQuery;
			bool result = false;
			try
			{
				using (SqlConnection conn = new SqlConnection(connString))
				{
					conn.Open();
					sqlCreateDBQuery = string.Format("SELECT database_id FROM sys.databases WHERE Name = '{0}'", dbname);
					using (SqlCommand sqlCmd = new SqlCommand(sqlCreateDBQuery, conn))
					{
						int databaseID = (int)sqlCmd.ExecuteScalar();
						result = (databaseID > 0);
					}
				}
			}
			catch
			{
				result = false;
			}
			return result;
		}

		public void DoCreate(string connString, string dbname)
		{
			try
			{
				using (SqlConnection conn = new SqlConnection(connString))
				{
					conn.Open();
					foreach (String command in CreateSqlStatements)
					{
						using (SqlCommand com = new SqlCommand(
							string.Format(Regex.Replace(command, @"\{([^0-9]+)\}", "{{$1}}"), dbname), conn))
						{
							com.ExecuteNonQuery();
						}
					}
					conn.Close();
				}
			}
			catch (Exception ex)
			{
				throw new DatabaseHandlerException("Create database error: " + ex.Message);
			}
		}

		public void DoDrop(string connString, string dbname)
		{
			List<string> sqlStatements = new List<string>()
						{
								string.Format(@"ALTER DATABASE [{0}] SET SINGLE_USER  WITH ROLLBACK IMMEDIATE",dbname),
								string.Format(@"DROP DATABASE [{0}]",dbname),
						};
			try
			{
				using (SqlConnection conn = new SqlConnection(connString))
				{
					SqlConnection.ClearPool(conn);
					conn.Open();
					SqlConnection.ClearPool(conn);
					foreach (String command in sqlStatements)
					{
						using (SqlCommand com = new SqlCommand(
							string.Format(Regex.Replace(command, @"\{([^0-9]+)\}", "{{$1}}"), dbname), conn))
						{
							com.ExecuteNonQuery();
						}
					}
					conn.Close();
					SqlConnection.ClearAllPools();
					SqlConnection.ClearPool(conn);
				}
			}
			catch (Exception ex)
			{
				throw new DatabaseHandlerException("Create database error: " + ex.Message);
			}
		}

		public void DoSchema(string connString, string dbname, string upgradeFrom)
		{
			Assembly _assembly = Assembly.GetExecutingAssembly();
			List<string> commands = new List<string>();
			bool start = false;
			if (string.IsNullOrEmpty(upgradeFrom))
				start = true;
			foreach (KeyValuePair<string, string> upgradePath in upgradePaths)
			{
				if (start)
				{

					using (TextReader upgcommands = new StreamReader(_assembly.GetManifestResourceStream(upgradePath.Value)))
					{
						String content = upgcommands.ReadToEnd();
						commands.AddRange(Regex.Split(content, "^GO", RegexOptions.Multiline));
						//commands.Add(content);
					}
				}
				if (!start && upgradeFrom == upgradePath.Key)
					start = true;
			}

			if (commands.Count == 0)
				return;

			try
			{
				using (SqlConnection conn = new SqlConnection(connString))
				{
					conn.Open();
					foreach (var command in commands)
					{
						try
						{
							if (string.IsNullOrEmpty(command)) continue;
							var commandWithDb = string.Format(Regex.Replace(command, @"\{([^0-9\}]+)\}", "{{$1}}"), dbname);
							using (SqlCommand com = new SqlCommand(commandWithDb, conn))
							{
								com.ExecuteNonQuery();
							}
						}
						catch (Exception ex)
						{
							throw new DatabaseHandlerException("Create schema inner error: " + ex.Message);
						}
					}
					conn.Close();
				}
			}
			catch (Exception ex)
			{
				throw new DatabaseHandlerException("Create schema error: " + ex.Message, ex);
			}
		}

		List<string> CreateSqlStatements = new List<String>() { 
						@"
						/****** Object:  Database [{0}]    Script Date: 05/23/2012 02:10:21 ******/
						IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'{0}')
						BEGIN
						CREATE DATABASE [{0}]
						END
						",@"
						ALTER DATABASE [{0}] SET ANSI_NULL_DEFAULT OFF
						",@"
						ALTER DATABASE [{0}] SET ANSI_NULLS OFF
						",@"
						ALTER DATABASE [{0}] SET ANSI_PADDING OFF
						",@"
						ALTER DATABASE [{0}] SET ANSI_WARNINGS OFF
						",@"
						ALTER DATABASE [{0}] SET ARITHABORT OFF
						",@"
						ALTER DATABASE [{0}] SET AUTO_CLOSE OFF
						",@"
						ALTER DATABASE [{0}] SET AUTO_CREATE_STATISTICS ON
						",@"
						ALTER DATABASE [{0}] SET AUTO_SHRINK OFF
						",@"
						ALTER DATABASE [{0}] SET AUTO_UPDATE_STATISTICS ON
						",@"
						ALTER DATABASE [{0}] SET CURSOR_CLOSE_ON_COMMIT OFF
						",@"
						ALTER DATABASE [{0}] SET CURSOR_DEFAULT  GLOBAL
						",@"
						ALTER DATABASE [{0}] SET CONCAT_NULL_YIELDS_NULL OFF
						",@"
						ALTER DATABASE [{0}] SET NUMERIC_ROUNDABORT OFF
						",@"
						ALTER DATABASE [{0}] SET QUOTED_IDENTIFIER OFF
						",@"
						ALTER DATABASE [{0}] SET RECURSIVE_TRIGGERS OFF
						",@"
						ALTER DATABASE [{0}] SET  DISABLE_BROKER
						",@"
						ALTER DATABASE [{0}] SET AUTO_UPDATE_STATISTICS_ASYNC OFF
						",@"
						ALTER DATABASE [{0}] SET DATE_CORRELATION_OPTIMIZATION OFF
						",@"
						ALTER DATABASE [{0}] SET TRUSTWORTHY OFF
						",@"
						ALTER DATABASE [{0}] SET ALLOW_SNAPSHOT_ISOLATION OFF
						",@"
						ALTER DATABASE [{0}] SET PARAMETERIZATION SIMPLE
						",@"
						ALTER DATABASE [{0}] SET READ_COMMITTED_SNAPSHOT OFF
						",@"
						ALTER DATABASE [{0}] SET  READ_WRITE
						",@"
						ALTER DATABASE [{0}] SET RECOVERY FULL
						",@"
						ALTER DATABASE [{0}] SET  MULTI_USER
						",@"
						ALTER DATABASE [{0}] SET PAGE_VERIFY NONE
						",@"
						ALTER DATABASE [{0}] SET DB_CHAINING OFF
						"};
	}
}
