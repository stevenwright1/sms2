using System;
using System.Net;
using System.Windows.Forms;
using AuthGateway.Shared;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Security.Principal;
using System.DirectoryServices.ActiveDirectory;
using System.Data.SqlClient;
using System.DirectoryServices;
using System.Runtime.InteropServices;
using AuthGateway.AuthEngine.Helpers;
using AuthGateway.AuthEngine.Logic.Helpers;

namespace AuthGateway.Wix.ServiceConfig.Dialog
{
	public partial class AuthEngineConfig01 : UserControl, IWizardScreen
	{
		protected SystemConfiguration sc;
		protected Wizard wizard;
		public AuthEngineConfig01()
		{
			InitializeComponent();
		}
		public AuthEngineConfig01(Wizard wizard, SystemConfiguration sc)
			: this()
		{
			this.wizard = wizard;
			this.sc = sc;
		}

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

		private void AuthEngineConfig01_Load(object sender, EventArgs e)
		{
			IPHostEntry iphe = Dns.GetHostEntry(Dns.GetHostName());
			IPAddress[] ips = iphe.AddressList;
			var currentAuthEngineIp = sc.getAuthEngineServerAddress().ToString();
			foreach (IPAddress ip in ips)
				cbAuthEngineAddress.Items.Add(ip.ToString());
			cbAuthEngineAddress.Text = currentAuthEngineIp;
			tbAuthEnginePort.Text = sc.AuthEngineServerPort.ToString();


			tbAdServer.Text = sc.ADServerAddress;
			tbAdUsername.Text = sc.ADUsername;
			tbAdPassword.Text = sc.ADPassword;

			tbLdapFilter.Text = sc.ADFilter;
			tbLdapContainer.Text = sc.ADContainer;

			if (string.IsNullOrEmpty(sc.ADBaseDN))
				tbLdapBaseDN.Text = "DC=" + GetComputerDomainName().Replace(".", ",DC=");
			else
				tbLdapBaseDN.Text = sc.ADBaseDN;
		}

		public bool Store()
		{
			try
			{
				sc.setServicesData(cbAuthEngineAddress.Text, tbAuthEnginePort.Text,
					sc.getSMSServerAddress().ToString(), sc.CloudSMSServerPort.ToString());

				sc.ADServerAddress = tbAdServer.Text;
				sc.ADUsername = tbAdUsername.Text;
				sc.ADPassword = tbAdPassword.Text;

				sc.ADBaseDN = tbLdapBaseDN.Text.Trim();
				sc.ADContainer = tbLdapContainer.Text.Trim();
				sc.ADFilter = tbLdapFilter.Text.Trim();

				sc.AuthEngineDefaultDomain = AdHelper.GetDNfromBaseDN(tbLdapBaseDN.Text);

				return true;
			}
			catch (Exception ex)
			{
				wizard.ShowError(ex.Message);
			}
			return false;
		}

		public bool SkipNext()
		{
			return false;
		}

		public bool SkipPrevious()
		{
			return false;
		}

		public UserControl GetControl()
		{
			return this;
		}

		public IWizardScreen GetWizardScreen()
		{
			return this;
		}

		private void btnTestADConfig_Click(object sender, EventArgs e)
		{
			var scADServerAddress = tbAdServer.Text;
			var scADUsername = tbAdUsername.Text;
			var scADPassword = tbAdPassword.Text;
			var scADBaseDN = tbLdapBaseDN.Text.Trim();
			var scADContainer = tbLdapContainer.Text.Trim();
			var scADFilter = tbLdapFilter.Text.Trim();

			PrincipalContext pc = null;

			try
			{
				if (string.Compare("autodetect", scADServerAddress, true) == 0) {
					var domainName = AdHelper.GetDNfromBaseDN(scADBaseDN);
					var detected = false;
					var nsquery = string.Format(@"_ldap._tcp.dc._msdcs.{0}.", domainName);
					foreach (var server in NsLookup.GetSRVRecords(nsquery))
					{
						scADServerAddress = server;
						detected = true;
						wizard.ShowInfo(string.Format("Using first autodetected AD/LDAP server: '{0}'", server));
						break;
					}
					if (!detected)
						throw new Exception(string.Format("Could not autodetect AD/LDAP servers with NsLookup query type SRV: '{0}'", nsquery));
				}
				var get = new AdOrLocalUsersGetter();
				var users = get.GetADUsers(scADServerAddress, scADUsername, scADPassword, scADBaseDN, scADContainer, scADFilter);
				wizard.ShowInfo(string.Format("Test querying AD/LDAP server '{1}' successful. Returned {0} users.", users, scADServerAddress));
			}
			catch(Exception ex)
			{
				wizard.ShowError(string.Format("An error occurred querying AD/LDAP server '{0}'. Error: {1}", scADServerAddress, ex.Message));
			}
			finally
			{
				if (pc != null)
					pc.Dispose();
			}
		}

		private void tbDefaultDomain_TextChanged(object sender, EventArgs e)
		{

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

		public int GetADUsers(string ldapServerName, string username, string password, string baseDN, string container, string filter)
		{

			/*
			var baseDN = "DC=this,DC=test,DC=com";
			var container = "CN=Users";
			var filter = "(&(objectClass=person))";
			*/

			var retry = true;
			int users = 0;
			while (retry)
			{
				retry = false;
				users = 0;
				PrincipalContext pc = null;
				try
				{
					
					string domainName = baseDN.Replace(",DC=", ".").Replace("DC=","");
					
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
			return users;
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
