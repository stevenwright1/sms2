/*
 * Created by SharpDevelop.
 * User: GPB
 * Date: 11/09/2014
 * Time: 20:01
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using System.Reflection;
using AuthGateway.Shared;
using AuthGateway.AuthEngine.Logic.DAL;

namespace AuthGateway.Util.Commands
{
	/// <summary>
	/// Description of ForcePoll.
	/// </summary>
	class ForceUpdateUsers : UtilCommands
	{
		protected string configFile = string.Empty;
		
		public ForceUpdateUsers()
		{
			this.IsCommand("forceupdateusers", "Clears uSNChanged and config checksum column to force a refresh of all users");
			
			var assemblyDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			var parentDir = Directory.GetParent(assemblyDir).FullName;
			var configFileDir = Path.Combine(parentDir, "Settings");
			configFile = Path.Combine(configFileDir, "Configuration.xml");
			
			HasOption("c|config=", "The system configuration file",
			          v =>
			          {
			          	if (v != null) {
			          		configFile = v;
			          	}
			          });
		}
		
		public override int Run(string[] remainingArguments)
		{
			if (!File.Exists(configFile))
			{
				Console.WriteLine(string.Format("Configuration file '{0}' not found.", configFile));
				return 1;
			}
			
			var sc = new SystemConfiguration();
			sc.LoadSettingsFromFile(configFile, false);

			int rows = 0;
			using (var queries = new DBQueries(sc.GetSQLConnectionString()))
			{
				rows = queries.NonQuery(string.Format(@"UPDATE [{0}] SET [uSNChanged] = ''", DBQueries.SMS_CONTACT));
			}
			if (rows == 0) {
				Console.WriteLine(string.Format("WARNING: No affected rows in {0}.", DBQueries.SMS_CONTACT));
			}
			
			using (var queries = new DBQueries(sc.GetSQLConnectionString()))
			{
				rows = queries.NonQuery(string.Format(@"
UPDATE [{0}] SET [VALUE] = '' WHERE [SETTING] = 'CONFIG_CHK' AND [OBJECT] = 'AUTHGATEWAY'
"
				                                      , DBQueries.SETTINGS));
			}
			if (rows == 0) {
				Console.WriteLine(string.Format("WARNING: No affected rows in {0}.", DBQueries.SETTINGS));
			}
			
			Console.WriteLine("Finished, next poll should update all users info.");
			return 0;
		}
	}
}
