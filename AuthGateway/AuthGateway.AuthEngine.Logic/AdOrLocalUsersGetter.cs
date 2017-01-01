using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Runtime.InteropServices;
using System.Security.Principal;
using AuthGateway.AuthEngine.Helpers;
using AuthGateway.Shared;
using AuthGateway.Shared.Log;
using AuthGateway.Shared.Tracking;
using AuthGateway.Shared.XmlMessages;
using AuthGateway.Shared.XmlMessages.Request.Command.AuthEngine;

namespace AuthGateway.AuthEngine.Logic
{
    public class ADPollException : Exception
    {

    }
	public class AdOrLocalUsersGetter : IUsersGetter
	{        

        public void GetDomainAndReplacements(UserGettersConfig config, out string domain, AdReplacements replacements)
        {
            string ldapServerName;
            var sc = config.Configuration;
            var baseDN = sc.ADBaseDN;
            var Username = sc.ADUsername;
            var Password = sc.ADPassword;
            domain = "";

            var retry = true;
            while (retry)
            {
                retry = false;
                try
                {
                    if (!config.OnlineControllers.TryPeek(out ldapServerName))
                        throw new Exception("No online controllers available");

                    Logger.Instance.WriteToLog("GetDomainAndReplacements start", LogLevel.Debug);

                    var domainName = AdHelper.GetDNfromBaseDN(baseDN);
                    domain = domainName;
                    Logger.Instance.WriteToLog("Domain name 0:" + domainName, LogLevel.Info);

                    //replacements.Add(domainName, domainName);

                    //Logger.Instance.WriteToLog(string.Format("ServerLogic.GetADUsers: Adding domain replacement: {0} = {1}", domainName.Replace(".", ""), domainName), LogLevel.Debug);
                    replacements.Add(domainName.Replace(".", ""), domainName);

                    var domainNameSplitted = domainName.Split(new char[] { '.' });
                    var domainNameFirstDC = domainNameSplitted[0];
                    //Logger.Instance.WriteToLog(string.Format("ServerLogic.GetADUsers: Adding domain replacement: {0} = {1}", domainNameFirstDC, domainName), LogLevel.Debug);
                    replacements.Add(domainNameFirstDC, domainName);

                    try
                    {
                        var domainNetbiosName = string.Empty;
                        var ldapFDQN = string.Format("LDAP://{0}/CN=Partitions,CN=Configuration,{1}", ldapServerName, baseDN);
                        Logger.Instance.WriteToLog("NetBiosGetName3 Try: " + ldapFDQN, LogLevel.Debug);
                        domainNetbiosName = DomainExtensions.GetNetBiosName(
                            ldapFDQN, Username, Password
                        );
                        if (!string.IsNullOrEmpty(domainNetbiosName))
                        {
                            replacements.Add(domainNetbiosName, domainName);
                            //Logger.Instance.WriteToLog(string.Format("Adding domain replacement: {0} = {1}", domainNetbiosName, domainName), LogLevel.Debug);
                        }
                    }
                    catch (Exception ex)
                    {
                        var comEx = ex as COMException;
                        if (comEx != null && comEx.ErrorCode == -2147016646)
                        {
                            string downServer;
                            if (config.OnlineControllers.TryDequeue(out downServer))
                            {
                                Logger.Instance.WriteToLog(string.Format("ServerLogic.GetADUsers: Added offline server '{0}'", downServer), LogLevel.Error);
                                config.OfflineControllers.Enqueue(downServer);
                                retry = true;
                                continue;
                            }
                        }
                        Logger.Instance.WriteToLog("NetBiosGetName3 ERROR: " + ex.Message, LogLevel.Debug);
                        Logger.Instance.WriteToLog("NetBiosGetName3 STACK: " + ex.StackTrace, LogLevel.Debug);                        
                    }

                    Logger.Instance.WriteToLog("GetDomainAndReplacements end", LogLevel.Debug);
                }
                catch (Exception ex)
                {
                    var comEx = ex as COMException;
                    if (comEx != null && comEx.ErrorCode == -2147016646)
                    {
                        string downServer;
                        if (config.OnlineControllers.TryDequeue(out downServer))
                        {
                            Logger.Instance.WriteToLog(string.Format("ServerLogic.GetADUsers: Added offline server '{0}'", downServer), LogLevel.Error);
                            config.OfflineControllers.Enqueue(downServer);
                            retry = true;
                            continue;
                        }
                    }
                    Logger.Instance.WriteToLog("GetDomainAndReplacements ERROR: " + ex.Message, LogLevel.Error);
                    Logger.Instance.WriteToLog("GetDomainAndReplacements STACK: " + ex.StackTrace, LogLevel.Debug);

                    throw new ADPollException();
                }
            }
        }
		public void GetUsers(UserGettersConfig config, Func<AddFullDetails, RetBase> action, IUsersGetterResults results = null)
		{
			if (results == null)
				results = new UsersGetterResults();
			var sc = config.Configuration;
			string ldapServerName;
            var Username = sc.ADUsername;
            var Password = sc.ADPassword;

			var baseDN = sc.ADBaseDN;
			var container = sc.ADContainer;
			var filter = sc.ADFilter;
			if (string.IsNullOrEmpty(filter))
				filter = "(&(objectClass=person))";

			/*
			var baseDN = "DC=this,DC=test,DC=com";
			var container = "CN=Users";
			var filter = "(&(objectClass=person))";
			 */

			var retry = true;
			while (retry)
			{
				retry = false;
				try
				{
					if (!config.OnlineControllers.TryPeek(out ldapServerName))
						throw new Exception("No online controllers available");

					results.Total = 0;
					results.Processed = 0;
					results.Valid = 0;
					results.SkippedInvalidUserAccountControl = 0;
					results.SkippedNullSid = 0;

					Logger.Instance.WriteToLog("ServerLogic.GetADUsers start", LogLevel.Debug);

                    var domainName = AdHelper.GetDNfromBaseDN(baseDN);

					var adminsList = new List<string>();
					
					var adminsBaseDN = baseDN;
					if ( ! string.IsNullOrWhiteSpace(config.Configuration.ADAdminBaseDNOverride))
						adminsBaseDN = config.Configuration.ADAdminBaseDNOverride;

					var adminFilter = "CN=Administrators,CN=Builtin";
					if ( ! string.IsNullOrWhiteSpace(config.Configuration.ADAdminFilterOverride) ) {
						adminFilter = config.Configuration.ADAdminFilterOverride;
					} else {
						if (!adminFilter.EndsWith(baseDN))
							adminFilter += "," + baseDN;
						adminFilter = string.Format("(&(objectCategory=user)(memberOf={0}))", adminFilter);
					}
					
					Logger.Instance.WriteToLog(string.Format("Using admins filter: '{0}'", adminFilter), LogLevel.Info);
					
					var adminGroups = new List<string>();

					var ldapRootAdminsGroup = string.Format("LDAP://{0}/{1}", ldapServerName, adminsBaseDN);
					using (DirectoryEntry oRoot = new DirectoryEntry(ldapRootAdminsGroup))
					{
						AddUpdateUsers(config, action, sc, Username, Password, adminsBaseDN, adminFilter, domainName, adminGroups, true, oRoot, new UsersGetterResults());
					}					

					Logger.Instance.WriteToLog(string.Format("Starting users poll loop"), LogLevel.Info);
					var containerAndBaseDN = baseDN;
					if (!string.IsNullOrEmpty(container))
						containerAndBaseDN = string.Join(",", new string[] { container, baseDN });
					Logger.Instance.WriteToLog(string.Format("[Container],BaseDN: {0}", containerAndBaseDN), LogLevel.Info);
					var ldapRoot = string.Format("LDAP://{0}/{1}", ldapServerName, containerAndBaseDN);
					using (DirectoryEntry oRoot = new DirectoryEntry(ldapRoot))
					{
						AddUpdateUsers(config, action, sc, Username, Password, baseDN, filter, domainName, adminGroups, false, oRoot, results);                        
					}
                    
					Logger.Instance.WriteToLog(string.Format("ServerLogic.GetADUsers End"), LogLevel.Info);
				}
				catch (Exception ex)
				{
					var comEx = ex as COMException;
					if (comEx != null && comEx.ErrorCode == -2147016646)
					{
						string downServer;
						if (config.OnlineControllers.TryDequeue(out downServer))
						{
							Logger.Instance.WriteToLog(string.Format("ServerLogic.GetADUsers: Added offline server '{0}'", downServer), LogLevel.Error);
							config.OfflineControllers.Enqueue(downServer);
							retry = true;
							continue;
						}
					}
					Logger.Instance.WriteToLog("ServerLogic.GetADUsers ERROR: " + ex.Message, LogLevel.Error);
					Logger.Instance.WriteToLog("ServerLogic.GetADUsers STACK: " + ex.StackTrace, LogLevel.Debug);
                    Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);

                    throw new ADPollException();
				}
				finally
				{

				}
			}
		}

		private void AddUpdateUsers(UserGettersConfig config, Func<AddFullDetails, RetBase> action, SystemConfiguration sc, string Username, string Password, string baseDN, string filter, string domainName, List<string> admins, bool checkingAdmins, DirectoryEntry oRoot, IUsersGetterResults results)
		{
			oRoot.Username = Username;
			oRoot.Password = Password;
			oRoot.AuthenticationType = AuthenticationTypes.Secure;
			Logger.Instance.WriteToLog(string.Format("AddUpdateUsers->Filter: {0}", filter), LogLevel.Info);

			using (DirectorySearcher oSearcher = new DirectorySearcher(oRoot))
			{

				oSearcher.PageSize = 1000;
				oSearcher.SizeLimit = 0;
				oSearcher.Filter = filter;

				oSearcher.PropertiesToLoad.Add("objectSid");
				oSearcher.PropertiesToLoad.Add("sAMAccountName");
                oSearcher.PropertiesToLoad.Add("userPrincipalName");
				oSearcher.PropertiesToLoad.Add("telephoneNumber");
				oSearcher.PropertiesToLoad.Add("mobile");
				oSearcher.PropertiesToLoad.Add("name");
				oSearcher.PropertiesToLoad.Add("givenName");
				oSearcher.PropertiesToLoad.Add("sn");
				oSearcher.PropertiesToLoad.Add("mail");
				oSearcher.PropertiesToLoad.Add("memberOf");
				oSearcher.PropertiesToLoad.Add("userAccountControl");
				oSearcher.PropertiesToLoad.Add("uSNChanged");

				var exceptionGroups = new List<string>();
				foreach (var deg in sc.AuthEngineDefaultExceptionGroups)
				{
					if (string.IsNullOrWhiteSpace(deg.Name)) continue;
					exceptionGroups.Add(string.Format("CN={0}", deg.Name));
				}

				if (Logger.I.ShouldLog(LogLevel.DebugVerbose))
					Logger.Instance.WriteToLog(string.Format("Exception groups: {0}", string.Join(" | ", exceptionGroups)), LogLevel.DebugVerbose);

				var providerGroups = new Dictionary<Provider, string>();
				foreach (var p in sc.Providers)
					if (!string.IsNullOrEmpty(p.AdGroup))
						providerGroups.Add(p, string.Format("CN={0}", p.AdGroup));

				if (Logger.I.ShouldLog(LogLevel.DebugVerbose))
					Logger.Instance.WriteToLog(string.Format("AuthEngineDefaultEnabled: {0}", sc.AuthEngineDefaultEnabled.ToString()), LogLevel.DebugVerbose);
				
				oSearcher.CacheResults = false;
				using (SearchResultCollection oResults = oSearcher.FindAll())
				{
					foreach (SearchResult oResult in oResults)
					{
						results.Total++;
						
						if (oResult.Properties["objectSid"] == null || oResult.Properties["objectSid"].Count == 0 || oResult.Properties["objectSid"][0] == null)
						{
							results.SkippedNullSid++;
							results.Processed++;
							continue;
						}

						var exceptionChecked = false;
							
						var add = new AddFullDetails();
						add.AuthEnabled = true;
						add.Email = getValue<string>(oResult.Properties["mail"]);
						add.Fullname = getValue<string>(oResult.Properties["name"]);
						add.LastName = getValue<string>(oResult.Properties["sn"]);
						add.FirstName = getValue<string>(oResult.Properties["givenName"]);
						add.Mobile = getStringValue(oResult.Properties["mobile"]);
						add.Org = domainName;
						add.OrgType = "DOMAIN";
						add.Phone = getStringValue(oResult.Properties["telephoneNumber"]);
						add.uSNChanged = ConvertADSLargeInteger(oResult.Properties["uSNChanged"]);
						if (oResult.Properties["objectSid"].Count == 0 || oResult.Properties["objectSid"][0] == null)
						{
							results.SkippedNullSid++;
							results.Processed++;
							continue;
						}

						add.Sid = new SecurityIdentifier((byte[])(oResult.Properties["objectSid"][0]), 0).ToString();
						add.User = getValue<string>(oResult.Properties["sAMAccountName"]);
                        add.UPN = getValue<string>(oResult.Properties["userPrincipalName"]);
						
						if (! checkingAdmins && admins.Contains(add.Sid)) {
							if (Logger.I.ShouldLog(LogLevel.Debug))
								Logger.I.WriteToLog(string.Format("SKIPPED user {0}, it's on administrators group", add.User), LogLevel.Debug);
							continue;
						}
						
						if (Logger.I.ShouldLog(LogLevel.DebugVerbose))
							Logger.Instance.WriteToLog(string.Format("User: {0}", add.User), LogLevel.DebugVerbose);

						var memberOfProperties = oResult.Properties["memberOf"];

						add.AuthEnabled = (sc.AuthEngineDefaultEnabled);
						add.UserType = (checkingAdmins) 
							? UserType.Administrator 
							: UserType.User;

						addProvidersFromConfiguration(config.Configuration, add);

						// userAccountControl cannot be null, so me must have a permission problem
						if (oResult.Properties["userAccountControl"].Count == 0 || oResult.Properties["userAccountControl"][0] == null)
						{
							Logger.I.WriteToLog(string.Format(
									@"Can't read Active Directory 'userAccountControl' property for user '{0}',
please check the permissions of the user making the query or the user being queried.", add.User), LogLevel.Info);
							results.SkippedInvalidUserAccountControl++;
							results.Processed++;
							continue;
						}

						if ( checkingAdmins ) {
							if (Logger.I.ShouldLog(LogLevel.Debug))
								Logger.I.WriteToLog(string.Format("Added user {0} to administrators group", add.User), LogLevel.Debug);
							admins.Add(add.Sid);
						}

						var flags = (int)oResult.Properties["userAccountControl"][0];
						var accountEnabled = !Convert.ToBoolean(flags & 0x00000002);
						var normalAccount = Convert.ToBoolean(flags & 0x00000200);
						
						
						add.Enabled = accountEnabled; // !ACCOUNTDISABLE
						add.Enabled = add.Enabled.Value && normalAccount; // NORMAL_ACCOUNT 
						if (Logger.I.ShouldLog(LogLevel.DebugVerbose) && !add.Enabled.Value)
							Logger.Instance.WriteToLog(string.Format("User.Enabled (!ACCOUNTDISABLED) {0} (NORMAL_ACCOUNT) {1}", accountEnabled.ToString(), normalAccount.ToString()), LogLevel.DebugVerbose);

						var groups = new List<string>();

						for (var i = 0; i < memberOfProperties.Count; i++)
						{
							var group = memberOfProperties[i].ToString();
							groups.Add(group);
						}
							
						if (Logger.I.ShouldLog(LogLevel.DebugVerbose))
							Logger.Instance.WriteToLog(string.Format("User belongs to groups: {0}", string.Join(" | ", groups)), LogLevel.DebugVerbose);
						
						foreach (var group in groups)
						{
							if (!exceptionChecked && isAnyItemInFQN(group, exceptionGroups, baseDN))
							{
								add.AuthEnabled = !add.AuthEnabled;
								exceptionChecked = true;
								if (Logger.I.ShouldLog(LogLevel.DebugVerbose))
									Logger.Instance.WriteToLog(string.Format("User.AuthEnabled (exceptionGroup): {0}", add.AuthEnabled.ToString()), LogLevel.DebugVerbose);
							}

						}

						// If provider groups were specified, check that the user
						// belongs to it.
						if (providerGroups.Count > 0)
						{
							foreach (var addp in add.Providers)
							{
								addp.Enabled = addp.Enabled
									&& (!providerGroups.ContainsKey(addp.Provider)
										|| (providerGroups[addp.Provider] != null
										&& isItemInAnyFQN(groups, providerGroups[addp.Provider], baseDN)));
							}
						}

						action(add);

						results.Processed++;
						results.Valid++;
					}
					oResults.Dispose();
				}
				Logger.Instance.WriteToLog(string.Format("AddUpdateUsers->Processed entries: {0} - Valid: {1} - Skipped (objectSid null): {2} - Skipped (unable to read userAccountControl property): {3}"
				                                         , results.Processed.ToString(), results.Valid.ToString(), results.SkippedNullSid.ToString(), results.SkippedInvalidUserAccountControl.ToString()), LogLevel.Info);
			}
		}
		
		public static string ConvertADSLargeInteger(ResultPropertyValueCollection val)
		{
			if ( val == null || val.Count == 0 )
				return string.Empty;
			if (val[0] is Int64) {
				return Convert.ToString(val[0]);
			}
			return Convert.ToString(ConvertADSLargeIntegerToInt64(val[0]));
		}
		
		public static Int64 ConvertADSLargeIntegerToInt64(object adsLargeInteger)
		{
			var highPart = (Int32)adsLargeInteger.GetType().InvokeMember("HighPart", System.Reflection.BindingFlags.GetProperty, null, adsLargeInteger, null);
			var lowPart  = (Int32)adsLargeInteger.GetType().InvokeMember("LowPart",  System.Reflection.BindingFlags.GetProperty, null, adsLargeInteger, null);
			return highPart * ((Int64)UInt32.MaxValue + 1) + lowPart;
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

		T getValue<T>(ResultPropertyValueCollection val, int index = 0)
		{
			if (val == null || val.Count == 0)
				return default(T);
			return (T)val[index];
		}

		string getStringValue(ResultPropertyValueCollection val, int index = 0)
		{
			if (val == null || val.Count == 0)
				return string.Empty;
			return (string)val[index];
		}

		protected void addProvidersFromConfiguration(SystemConfiguration sc, AddFullDetails add)
		{
			var addedProviders = new List<string>();
			foreach (var provider in sc.Providers)
			{
				add.Providers.Add(new UserProvider()
				                  {
				                  	Name = provider.Name,
				                  	Enabled = provider.Enabled,
				                  	Provider = provider
				                  });
				addedProviders.Add(string.Format("{0} ({1}) ", provider.Name, provider.Id));
			}
			if (Logger.I.ShouldLog(LogLevel.DebugVerbose))
				Logger.Instance.WriteToLog(string.Format("ServerLogic.checkUserProviders: Added Providers {0}", string.Join(",", addedProviders)), LogLevel.DebugVerbose);
		}
	}

	public class LocalUsersGetter : IUsersGetter
	{
		protected UserGettersConfig config;

        public void GetDomainAndReplacements(UserGettersConfig config, out string domain, AdReplacements replacements)
        {
            domain = "";
        }
		public void GetUsers(UserGettersConfig config, Func<AddFullDetails, RetBase> action, IUsersGetterResults results)
		{
			this.config = config;
			String server = Environment.MachineName;
			String currentUser = Environment.UserName;
			Logger.Instance.WriteToLog("ServerLogic.GetLocalUsers Start", LogLevel.Debug);
			foreach (var afd in GetLocalUsers())
			{
				action(afd);
			}
		}
		protected void checkUserProviders(AddFullDetails add)
		{
			var addedProviders = new List<string>();
			var sc = config.Configuration;
			foreach (var provider in sc.Providers)
			{
				add.Providers.Add(new UserProvider()
				                  {
				                  	Name = provider.Name,
				                  	Enabled = provider.Enabled,
				                  	Provider = provider
				                  });
				addedProviders.Add(string.Format("{0} ({1}) ", provider.Name, provider.Id));
			}
			Logger.Instance.WriteToLog(string.Format("ServerLogic.checkUserProviders: Added Providers {0}", string.Join(",", addedProviders)), LogLevel.Debug);
		}

		protected void checkUserGroups(Dictionary<Provider, GroupPrincipal> providerGroups, UserPrincipal up, AddFullDetails add)
		{
			foreach (var provider in add.Providers)
			{
				provider.Enabled = provider.Enabled
					&& providerGroups != null
					&& (!providerGroups.ContainsKey(provider.Provider)
					    || (providerGroups[provider.Provider] != null
					        && (up != null && up.IsMemberOf(providerGroups[provider.Provider])))
					   );
			}
		}

		protected virtual List<AddFullDetails> GetLocalUsers()
		{
			var sc = config.Configuration;
			var ret = new List<AddFullDetails>();
			try
			{
				PrincipalContext pc = new PrincipalContext(ContextType.Machine);

				string builtinAdministratorsName = new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null).Value;
				GroupPrincipal gp = GroupPrincipal.FindByIdentity(pc, builtinAdministratorsName);

				Dictionary<Provider, GroupPrincipal> providerGroups = new Dictionary<Provider, GroupPrincipal>();
				foreach (var p in sc.Providers)
					if (!string.IsNullOrEmpty(p.AdGroup))
						providerGroups.Add(p, GroupPrincipal.FindByIdentity(pc, p.AdGroup));

				List<GroupPrincipal> defaultExceptionGroups = new List<GroupPrincipal>();
				foreach (var groupEntry in sc.AuthEngineDefaultExceptionGroups)
				{
					var groupPrin = GroupPrincipal.FindByIdentity(pc, groupEntry.Name);
					if (groupPrin != null)
						defaultExceptionGroups.Add(groupPrin);
					else
						Logger.Instance.WriteToLog(string.Format("GetLocalUsers.Group '{0}' not found in machine", groupEntry.Name), LogLevel.Info);
				}

				UserPrincipal user = new UserPrincipal(pc);
				user.Name = "*";
				PrincipalSearcher pS = new PrincipalSearcher();
				pS.QueryFilter = user;
				PrincipalSearchResult<Principal> results = pS.FindAll();
				foreach (UserPrincipal result in results)
				{
					using (UserPrincipal up = (UserPrincipal)result)
					{
						AddFullDetails add = new AddFullDetails();
						add.Sid = up.Sid.Value;
						add.User = up.Name;
						add.Phone = (string.IsNullOrEmpty(up.VoiceTelephoneNumber)) ? string.Empty : up.VoiceTelephoneNumber;
						add.Mobile = (string.IsNullOrEmpty(up.VoiceTelephoneNumber)) ? string.Empty : up.VoiceTelephoneNumber;
						add.Fullname = up.UserPrincipalName;
						add.Email = (string.IsNullOrEmpty(up.EmailAddress)) ? string.Empty : up.EmailAddress;
						if (add.Fullname == null)
							add.Fullname = string.Empty;
						if (gp != null && up.IsMemberOf(gp))
							add.UserType = UserType.Administrator;
						else
							add.UserType = UserType.User;
						checkUserProviders(add);
						checkUserGroups(providerGroups, up, add);

						add.AuthEnabled = (sc.AuthEngineDefaultEnabled);
						foreach (var group in defaultExceptionGroups)
						{
							if (up.IsMemberOf(group))
							{
								add.AuthEnabled = !add.AuthEnabled;
								break;
							}
						}
						add.Org = Environment.MachineName;
						add.OrgType = "WORKGROUP";
						add.Enabled = up.Enabled;
						ret.Add(add);
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Instance.WriteToLog("ServerLogic.GetLocalUsers ERROR: " + ex.Message, LogLevel.Error);
				Logger.Instance.WriteToLog("ServerLogic.GetLocalUsers STACK: " + ex.StackTrace, LogLevel.Debug);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
			}
			return ret;
		}
	}
}
