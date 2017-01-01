using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.DirectoryServices.AccountManagement;
using System.Security.Principal;
using AuthGateway.Shared.Log;
using System.DirectoryServices.ActiveDirectory;
using System.Windows.Forms;

namespace AuthGateway.MassUserSetup
{
	internal class AdHelper
	{
		public static List<string> GetGroups()
		{
			return GetGroups(null, null, null);
		}
		public static List<string> GetGroups(string ldapServer, string username, string password)
		{
			var ret = new List<string>();
			try
			{
				Logger.Instance.WriteToLog(".GetGroups start", LogLevel.Debug);
				PrincipalContext pc = null;
				try
				{
					if (string.IsNullOrEmpty(ldapServer) || string.IsNullOrEmpty(username) || password == null)
						pc = new PrincipalContext(ContextType.Domain);
					else
						pc = new PrincipalContext(ContextType.Domain, ldapServer, username, password);

					using (GroupPrincipal gp = new GroupPrincipal(pc))
					{
						gp.Name = "*";
						using (PrincipalSearcher pS = new PrincipalSearcher())
						{
							pS.QueryFilter = gp;
							foreach (Principal result in pS.FindAll())
							{
								if (result is GroupPrincipal)
								{
									ret.Add(result.SamAccountName);
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
			catch (Exception ex)
			{
				Logger.Instance.WriteToLog(".GetGroups ERROR: " + ex.Message, LogLevel.Error);
				Logger.Instance.WriteToLog(".GetGroups STACK: " + ex.StackTrace, LogLevel.Debug);
			}
			return ret;
		}

		public static List<AdUser> GetUsers(string groupname)
		{
			return GetUsers(null, groupname, null, null);
		}

		public static List<AdUser> GetUsers(string ldapServer, string groupname, string username, string password)
		{
			return GetUsers(ldapServer, groupname, username, password, true);
		}

		public static List<AdUser> GetUsers(string ldapServer, string groupname, string username, string password, bool filterDisabled)
		{
			var ret = new List<AdUser>();

			try
			{
				Logger.Instance.WriteToLog(".GetGroups start", LogLevel.Debug);
				PrincipalContext pc = null;
				try
				{
					if (string.IsNullOrEmpty(ldapServer) || string.IsNullOrEmpty(username) || password == null)
					{
						pc = new PrincipalContext(ContextType.Domain);
					}
					else
					{
						pc = new PrincipalContext(ContextType.Domain, ldapServer, username, password);
					}

					using(GroupPrincipal gp = GroupPrincipal.FindByIdentity(pc, IdentityType.SamAccountName, groupname)) {
						foreach (Principal result in gp.GetMembers(false))
						{
							if (result is UserPrincipal)
							{
								var rP = UserPrincipal.FindByIdentity(pc, result.SamAccountName);
								if (filterDisabled && (rP.Enabled == null || rP.Enabled == false))
									continue;

								var user = new AdUser();
								user.Email = rP.EmailAddress;
								user.Phone = rP.VoiceTelephoneNumber;
								user.Username = rP.SamAccountName;
								user.Fullname = rP.Name;
								user.Enabled = (rP.Enabled != null && rP.Enabled == true);
								ret.Add(user);
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
			catch (Exception ex)
			{
				Logger.Instance.WriteToLog(".GetGroups ERROR: " + ex.Message, LogLevel.Error);
				Logger.Instance.WriteToLog(".GetGroups STACK: " + ex.StackTrace, LogLevel.Debug);
			}

			return ret;
		}
	}
}
