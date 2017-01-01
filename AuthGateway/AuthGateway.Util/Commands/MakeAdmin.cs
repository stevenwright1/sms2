/*
 * Created by SharpDevelop.
 * User: GPB
 * Date: 10/07/2014
 * Time: 17:47
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using AuthGateway.AuthEngine.Logic.DAL;
using AuthGateway.Shared;

namespace AuthGateway.Util.Commands
{
	class MakeAdmin : UtilCommands
	{
		protected string configFile = string.Empty;
	
		public MakeAdmin()
		{
			this.IsCommand("makeadmin", "Promote a user to administrator inside Wright CSS SMS2");
	
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
			HasAdditionalArguments(1, "<username>");
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
	
			if (remainingArguments.Length == 0)
				throw new Exception("Missing argument.");
	
			var username = remainingArguments[0];
	
			long userId;
			string sid;
			using (var q = new DBQueries(sc.GetSQLConnectionString(true)))
			{
				var userData = q.Query(@"
	SELECT ID, SID 
	FROM SMS_CONTACT
	WHERE AD_USERNAME = @USERNAME", new DBQueryParm("USERNAME", username));
	
				if (userData.Rows.Count == 0)
				{
					Console.WriteLine("WARNING: No records were updated.");
					return 0;
				}
	
				userId = Convert.ToInt64(userData.Rows[0]["ID"]);
				sid = Convert.ToString(userData.Rows[0]["SID"]);
			}
	
			using (var q = new DBQueries(sc.GetSQLConnectionString(true)))
			{
				var result = q.NonQuery(@"
	UPDATE SMS_CONTACT 
	SET [USER_TYPE]='Administrator', [utOverridden]=1
	WHERE ID = @USERID", new DBQueryParm("USERID", userId));
				if (result == 0)
				{
					Console.WriteLine("WARNING: No records were updated.");
					return 0;
				}
				Console.WriteLine(string.Format("SUCCESS: {0} records were updated in SMS_CONTACT.", result.ToString()));
			}
	
			using (var q = new DBQueries(sc.GetSQLConnectionString(true)))
			{
				var parms = new List<DBQueryParm>();
				parms.Add(new DBQueryParm("USERID", userId));
				parms.Add(new DBQueryParm("SID", sid));
				var result = q.NonQuery(@"
	IF (NOT EXISTS (SELECT * FROM [UserAdmin] WHERE userId = @USERID))
	BEGIN 
	  INSERT INTO [UserAdmin](sid, userId)
	VALUES (@SID, @USERID)
	END
	ELSE
	BEGIN
	UPDATE [UserAdmin]
	SET sid=@SID
	WHERE userId = @USERID
	END
	", parms);
				if (result == 0)
				{
					Console.WriteLine("WARNING: No records were updated/added in UserAdmin.");
					return 0;
				}
				Console.WriteLine(string.Format("SUCCESS: {0} records were updated.", result.ToString()));
			}
	
			return 0;
		}
	}
}
