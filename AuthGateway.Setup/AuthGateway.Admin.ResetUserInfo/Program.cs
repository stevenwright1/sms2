using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuthGateway.AuthEngine.Logic.DAL;
using AuthGateway.Shared;
using AuthGateway.Shared.Log;

namespace AuthGateway.Admin.ResetUserInfo
{
	class Program
	{
		static void Main(string[] args)
		{
			try
			{
				string adGroup = string.Empty;
				if (args.Length < 1)
				{
					Console.WriteLine("This tool wipes user current phone and mobile numbers from the database.");
					Console.WriteLine("Usage:");
					Console.WriteLine("Wright.Admin.ResetUserInfo.exe <configration.xml> [AdGroup]");
					return;
				}

				string configFile = args[0];
				if (args.Length > 1)
					adGroup = args[1];

				var sc = new SystemConfiguration();
				sc.LoadSettingsFromFile(configFile, false);

				List<string> userNames = new List<string>();

				if (args.Length < 2)
				{
					Console.WriteLine("WARNING: This will delete from the database all stored phones and mobile phones");
				}
				else
				{
					var get = new AdOrLocalUsersGetter();
					var filter = string.Empty;
					if (!adGroup.EndsWith(sc.ADBaseDN))
						adGroup += "," + sc.ADBaseDN;
					if (string.IsNullOrEmpty(sc.ADFilter))
						filter = string.Format("(&(memberOf=CN={0}))", adGroup);
					else
						filter = string.Format("(&({0})(memberOf=CN={1}))", sc.ADFilter, adGroup);
					Console.WriteLine("Using AD container: {0}", sc.ADContainer);
					Console.WriteLine("Using AD filter: {0}", filter);
					userNames = get.GetADUsers(sc.ADServerAddress, sc.ADUsername, sc.ADPassword, sc.ADBaseDN, sc.ADContainer, filter);

					Console.WriteLine(string.Format("WARNING: This will delete from the database all stored phones and mobile phones of users in '{0}', matched {1} users.", adGroup, userNames.Count));
				}
				Console.WriteLine("Do you want to continue? (y/n)");
				var key = Console.ReadKey();
				if (key.KeyChar != 'y' && key.KeyChar != 'Y')
				{
					Console.WriteLine("ABORT.");
					return;
				}

				var where = string.Empty;
				if (adGroup != string.Empty)
				{
					var sanitizedUsernames = new List<string>();
					foreach(var userName in userNames)
						sanitizedUsernames.Add(userName.Replace("'", "''"));
					where = string.Format(@" WHERE [AD_USERNAME] IN ('{0}')", string.Join("','", sanitizedUsernames));
				}

				using (var q = new DBQueries(sc.GetSQLConnectionString(true)))
				{
					var query = string.Format(@"
UPDATE SMS_CONTACT
SET [INPUT_PHONE_NUMBER]='',[MOBILE_NUMBER]='',phoneOverrided=0, mobileOverrided=0 {0}
", where);
					if (sc.AuthEngineFileLogLevel >= LogLevel.Debug)
						Console.WriteLine("Running query: " + query);
					var affectedRows = q.NonQuery(query);

					if (affectedRows == 0)
					{
						Console.WriteLine("WARNING: No records were updated.");
						return;
					}
					Console.WriteLine(string.Format("SUCCESS: {0} records were updated in SMS_CONTACT.", affectedRows.ToString()));
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("ERROR: " + ex.Message);
			}
		}
	}

	public class AdOrLocalUsersGetter
	{
		private string GetComputerDomainName()
		{
			string domain = String.Empty;
			try
			{
				domain = Domain.GetComputerDomain().Name;
			}
			catch
			{
				domain = Environment.UserDomainName;
			}
			return domain;
		}

		public List<string> GetADUsers(string ldapServerName, string username, string password, string baseDN, string container, string filter)
		{

			/*
			var baseDN = "DC=this,DC=test,DC=com";
			var container = "CN=Users";
			var filter = "(&(objectClass=person))";
			*/

			var retry = true;
			int users = 0;
			var userNames = new List<string>();
			while (retry)
			{
				retry = false;
				users = 0;
				PrincipalContext pc = null;
				try
				{

					string domainName = baseDN.Replace(",DC=", ".").Replace("DC=", "");

					var domainNetbiosName = string.Empty;

					var containerAndBaseDN = baseDN;
					if (!string.IsNullOrEmpty(container))
						containerAndBaseDN = string.Join(",", new string[] { container, baseDN });
					var ldapRoot = string.Format("LDAP://{0}/{1}", ldapServerName, containerAndBaseDN);
					using (DirectoryEntry oRoot = new DirectoryEntry(ldapRoot))
					{
						oRoot.Username = username;
						oRoot.Password = password;
						oRoot.AuthenticationType = AuthenticationTypes.Secure;

						using (DirectorySearcher oSearcher = new DirectorySearcher(oRoot))
						{

							oSearcher.PageSize = 1000;
							oSearcher.SizeLimit = 100000;
							oSearcher.Filter = filter;

							oSearcher.PropertiesToLoad.Add("sAMAccountName");

							using (SearchResultCollection oResults = oSearcher.FindAll())
							{
								foreach (SearchResult oResult in oResults)
								{
									using (var entry = oResult.GetDirectoryEntry())
									{
										users++;
										userNames.Add(entry.Properties["sAMAccountName"].Value.ToString());
									}
								}
							}
						}
					}
				}
				finally
				{
					if (pc != null)
						pc.Dispose();
				}
			}
			return userNames;
		}

		private bool isItemInAnyFQN(List<string> groups, string item, string baseDN)
		{
			foreach (var group in groups)
			{
				if (group.Contains(item) && group.EndsWith(baseDN))
					return true;
			}
			return false;
		}

		private bool isAnyItemInFQN(string group, List<string> items, string baseDN)
		{
			foreach (var item in items)
			{
				if (group.Contains(item) && group.EndsWith(baseDN))
					return true;
			}
			return false;
		}

		private T getValue<T>(object val)
		{
			if (val == null)
				return default(T);
			return (T)val;
		}

		private string getStringValue(object val)
		{
			if (val == null)
				return string.Empty;
			return (string)val;
		}
	}
}
