using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices.ActiveDirectory;
using System.Runtime.InteropServices;

namespace AuthGateway.AuthEngine.Helpers
{
	public static class AdHelper
	{
		public static string GetDNfromBaseDN(string baseDN)
		{
			return baseDN.Replace(",DC=", ".").Replace("DC=", "");
		}

		public static bool validateUserSimple(string domain, string username, string password)
		{
			bool isValid = false;
			using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, domain))
			{
				isValid = pc.ValidateCredentials(username, password);
			}
			return isValid;
		}

		public static bool validateUserFull(string adserver, string adusername, string adpassword, string username, string password)
		{
			bool isValid = false;
			try
			{
				using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, adserver, adusername, adpassword))
					isValid = pc.ValidateCredentials(username, password, ContextOptions.SimpleBind);
			} 
			catch
			{
				
			}
			return isValid;
		}

		public static bool ValidateUserAd(string path, string account, string password)
		{
			using(var adsEntry = new DirectoryEntry(path, account, password, AuthenticationTypes.Secure)) {
				using (var adsSearcher = new DirectorySearcher(adsEntry))
				{
					adsSearcher.Filter = "(sAMAccountName=" + account + ")";
					try
					{
						var adsSearchResult = adsSearcher.FindOne();
						return true;
					}
					catch
					{
						return false;
					}
				}
			}
		}
		
		public static List<string> ValidateUserAdWithGroups(string path, string account, string password)
		{
			try {
				using(var adsEntry = new DirectoryEntry(path, account, password, AuthenticationTypes.Secure)) {
					using (var adsSearcher = new DirectorySearcher(adsEntry))
					{
						adsSearcher.Filter = "(sAMAccountName=" + account + ")";
						adsSearcher.PropertiesToLoad.Add("memberOf");
						var adsSearchResult = adsSearcher.FindOne();
						var groupsCount = adsSearchResult.Properties["memberOf"].Count;
						var ret = new List<string>();
						for( var i = 0; i < groupsCount; i++ ) {
							var adGroup = (String)adsSearchResult.Properties["memberOf"][i];
							ret.Add(adGroup);
						}
						return ret;
					}
				}
			} catch(DirectoryServicesCOMException ex) {
				if (
					ex.ErrorCode == -2147023570
					&& ex.ExtendedError == -2146893044
				)
					throw new BadLogonException();
				throw;
			} catch(COMException ex) {
				if (ex.ErrorCode == -2147016646)
					throw new ServerException();
				throw;
			}
		}

		public static bool IsServerOnline(string adserver, string adusername, string adpassword)
		{
			try
			{
				using (var pc = new PrincipalContext(ContextType.Domain, adserver, adusername, adpassword))
				{
				}
			}
			catch (PrincipalServerDownException)
			{
				return false;
			}
			catch (Exception)
			{
			}
			return true;
		}

		public static IList<string> GetOtherDomainControllers(string adserver, string adusername, string adpassword)
		{
			var servers = new List<string>();
			var directoryContext = new DirectoryContext(DirectoryContextType.DirectoryServer, adserver, adusername, adpassword);
			using (var domain = Domain.GetDomain(directoryContext))
			{
				foreach (DomainController d in domain.FindAllDiscoverableDomainControllers())
				{
					servers.Add(d.IPAddress);
				}
			}
			return servers;
		}
	}
	
	public class AdException : Exception { }
	public class ServerException : AdException { }
	public class BadLogonException : AdException { }
}
