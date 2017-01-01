/// <summary>
/// Defines extentions made to the <see cref="Domain"/> class.
/// </summary>
using System.DirectoryServices.ActiveDirectory;
using System.DirectoryServices;
public static class DomainExtensions
{
	public static string GetDomainNetBiosName(this Domain domain, string username, string password)
	{
		//var ForestRootDirectoryEntry = Forest.GetCurrentForest().RootDomain.GetDirectoryEntry();
		using (var ForestRootDirectoryEntry = domain.Forest.RootDomain.GetDirectoryEntry())
		{
			string forestConfigurationBindPath = string.Format("LDAP://CN=Partitions,CN=Configuration,{0}", ForestRootDirectoryEntry.Properties["distinguishedName"].Value);
			using (var ForestRootConfigurationDirectoryEntry = new DirectoryEntry(forestConfigurationBindPath, username, password))
			{
				string netBiosName = string.Empty;

				using (DirectorySearcher directorySearcher = new DirectorySearcher(ForestRootConfigurationDirectoryEntry))
				{
					directorySearcher.Filter = string.Format("(&(nETBIOSName=*)(dnsRoot={0}))", domain.Name);
					directorySearcher.PropertiesToLoad.AddRange(new string[] { "dnsRoot", "nETBIOSName" });
					var result = directorySearcher.FindOne();
					if ((result != null) && (result.Properties.Contains("nETBIOSName"))) netBiosName = result.Properties["nETBIOSName"][0].ToString();
					result = null;
				}
				return netBiosName;
			}
		}
	}

	public static string GetNetbiosName(this Domain domain, string Username, string Password)
	{

		// Bind to RootDSE and grab the configuration naming context
		//DirectoryEntry rootDSE = new DirectoryEntry(@"LDAP://RootDSE");
		//DirectoryEntry partitions = new DirectoryEntry(@"LDAP://cn=Partitions," + rootDSE.Properties["configurationNamingContext"].Value);
		using(DirectoryEntry domainEntry = domain.GetDirectoryEntry())
		{
			//Iterate through the cross references collection in the Partitions container
			//DirectorySearcher directorySearcher = new DirectorySearcher(partitions)
			using(DirectorySearcher directorySearcher = new DirectorySearcher(domainEntry)
			      {
			      	ReferralChasing = ReferralChasingOption.All,
			      	Filter = "(&(objectCategory=crossRef)(ncName=" +
			      		domainEntry.Path
			      		.Replace("LDAP://", string.Empty)
			      		.Replace(domain.Name + "/", string.Empty) + "))",
			      	SearchScope = SearchScope.Subtree
			      }) {
				string netbiosName = null;
				directorySearcher.PropertiesToLoad.Add("nETBIOSName");
				//Display result (should only be one)
				SearchResult results = directorySearcher.FindOne();
				if (results != null && results.Properties["nETBIOSName"] != null && results.Properties["nETBIOSName"].Count > 0)
				{
					netbiosName = results.Properties["nETBIOSName"][0].ToString();
				}
				return netbiosName;
			}
		}
	}

	public static string GetNetBiosName(
		string ldapUrl,
		string userName,
		string password)
	{
		string netbiosName = string.Empty;
		using (DirectoryEntry dirEntry = new DirectoryEntry(ldapUrl,
		                                                    userName, password))
		{

			using (DirectorySearcher searcher = new DirectorySearcher(dirEntry))
			{
				searcher.Filter = "netbiosname=*";
				searcher.PropertiesToLoad.Add("cn");
				searcher.ReferralChasing = ReferralChasingOption.All;
				SearchResult results = searcher.FindOne();
				if (results != null && results.Properties["CN"] != null && results.Properties["CN"].Count > 0)
				{
					netbiosName = results.Properties["CN"][0].ToString();
				}
				return netbiosName;
			}
		}
	}
}
