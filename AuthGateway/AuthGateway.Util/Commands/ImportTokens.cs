/*
 * Created by SharpDevelop.
 * User: GPB
 * Date: 10/07/2014
 * Time: 17:47
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using System.Reflection;
using AuthGateway.AuthEngine.Logic.DAL;
using AuthGateway.Shared;

namespace AuthGateway.Util.Commands
{
	class ImportTokens : UtilCommands
	{
		protected string configFile = string.Empty;
	
		public ImportTokens()
		{
			this.IsCommand("importtokens", "Import Feitian tokens to be used from AdminGUI referencing serial numbers");
	
			var assemblyDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			var parentDir = Directory.GetParent(assemblyDir).FullName;
			var configFileDir = Path.Combine(parentDir, "Settings");
			configFile = Path.Combine(configFileDir, "Configuration.xml");
	
			HasOption("c|config=", "The system configuration file",
					v =>
					{
						if (v != null)
							configFile = v;
					});
			HasAdditionalArguments(1, "<tokens seed file>");
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
	
			var tokensFile = remainingArguments[0];
	
			var importedTokens = 0;
			using (var streamReader = new StreamReader(tokensFile))
			{
				var line = streamReader.ReadLine();
				while (line != null)
				{
					var serialAndToken = line.Split(' ');
					var tokenType = "";
					var window = "";
					if (serialAndToken.Length < 2)
						continue;
	
					var serial = serialAndToken[0];
					var token = serialAndToken[1];
					if (serialAndToken.Length > 2)
						tokenType = serialAndToken[2].ToUpper().Trim();
					if (serialAndToken.Length > 3)
						window = serialAndToken[3].Trim();
					using (var q = new DBQueries(sc.GetSQLConnectionString(true)))
					{
						var result = q.NonQuery(@"
	IF (NOT EXISTS (SELECT * FROM [HardToken] WHERE [serial] = @pserial))
	BEGIN 
	  INSERT INTO [HardToken]([serial], [key], [tokentype], [window]) VALUES (@pserial, @pkey, @ptokentype, @pwindow)
	END
	ELSE
	BEGIN
	  UPDATE [HardToken] SET
	    [key]=@pkey,
	  	[tokentype]=@ptokentype,
	  	[window]=@pwindow
	  WHERE [serial] = @pserial
	END
	",
	                        new DBQueryParm("pserial", serial),
	                        new DBQueryParm("pkey", token),
	                        new DBQueryParm("ptokentype", tokenType),
	                        new DBQueryParm("pwindow", window));
						if (result == 0) {
							Console.WriteLine(string.Format("WARNING: Record for serial {0} already existed.", serial));
						} else {
							//Console.WriteLine(string.Format("SUCCESS: Inserted serial {0}.", serial));
							importedTokens++;
						}
					}
	
					line = streamReader.ReadLine();
				}
			}
			Console.WriteLine(string.Format("Imported tokens: {0}", importedTokens));
			return 0;
		}
	}
}
