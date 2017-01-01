using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AuthGateway.AuthEngine.Logic.DAL;
using AuthGateway.Shared;

namespace AuthGateway.MakeAdmin
{
	class Program
	{
		static void Main(string[] args)
		{
			try
			{
				if (args.Length < 2)
				{
					Console.WriteLine("AuthGateway.MakeAdmin.exe <configration.xml> <ad_username>");
					return;
				}

				string configFile = args[0];
				string user = args[1];

				var sc = new SystemConfiguration();
				sc.LoadSettingsFromFile(configFile, false);

				long userId;
				string sid;
				using (var q = new DBQueries(sc.GetSQLConnectionString(true)))
				{
					var userData = q.Query(string.Format(@"
SELECT ID, SID 
FROM SMS_CONTACT
WHERE AD_USERNAME = '{0}'", user));

					if (userData.Rows.Count == 0)
					{
						Console.WriteLine("WARNING: No records were updated.");
						return;
					}

					userId = Convert.ToInt64(userData.Rows[0]["ID"]);
					sid = Convert.ToString(userData.Rows[0]["SID"]);
				}

				using (var q = new DBQueries(sc.GetSQLConnectionString(true)))
				{
					var result = q.NonQuery(string.Format(@"
UPDATE SMS_CONTACT 
SET USER_TYPE = 'Administrator' 
WHERE ID = '{0}'", userId));
					if (result == 0)
					{
						Console.WriteLine("WARNING: No records were updated.");
						return;
					}
					Console.WriteLine(string.Format("SUCCESS: {0} records were updated in SMS_CONTACT.", result.ToString()));
				}

				using (var q = new DBQueries(sc.GetSQLConnectionString(true)))
				{
					var result = q.NonQuery(string.Format(@"
IF (NOT EXISTS (SELECT * FROM [UserAdmin] WHERE userId = {0}))
BEGIN 
  INSERT INTO [UserAdmin](sid, userId)
	VALUES ('{1}', '{0}')
END
ELSE
BEGIN
	UPDATE [UserAdmin]
	SET sid='{1}'
	WHERE userId = {0}
END
", userId, sid));
					if (result == 0)
					{
						Console.WriteLine("WARNING: No records were updated/added in UserAdmin.");
						return;
					}
					Console.WriteLine(string.Format("SUCCESS: {0} records were updated.", result.ToString()));
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("ERROR: " + ex.Message);
			}
		}
	}
}
