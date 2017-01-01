using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using AuthGateway.Setup.Shared;
using AuthGateway.Setup.Shared.Helpers;
using AuthGateway.Setup.SQLDB;
using AuthGateway.Shared;
using Microsoft.Deployment.WindowsInstaller;
using Microsoft.Win32;

namespace AuthGateway.Wix.ServiceConfig
{
	public class CustomActionsHandler
	{
		private string ConvertToBase64(string str)
		{
			if (string.IsNullOrEmpty(str) || str == "0")
				return str;
			return Convert.ToBase64String(Encoding.UTF8.GetBytes(str));
		}

		private string ConvertFromBase64(string str)
		{
			if (string.IsNullOrEmpty(str) || str == "0")
				return str;
			return Encoding.UTF8.GetString(Convert.FromBase64String(str));
		}

		public ActionResult WrightCCS_CA_Before_InstallFinalize_Rollback(CASession session)
		{
			session.Log("Begin WrightCCS_CA_Before_InstallFinalize_Rollback");
			return ActionResult.Success;
		}

		public ActionResult WrightCCS_CA_Before_InstallFinalize(CASession session)
		{
			session.Log("Begin WrightCCS_CA_Before_InstallFinalize");
			return ActionResult.Success;
		}

		private static void saveConfigurationXml(string configurationXml, string installDir)
		{
			var file = Path.Combine(installDir, "Settings", "Configuration.xml");
			File.WriteAllText(file, configurationXml, Encoding.UTF8);
		}

		private static void saveClientConfigurationXml(SystemConfiguration sc, string installDir)
		{
			sc.WriteClientCredentials(Path.Combine(installDir, "SettingsPublic", "Configuration.xml"));
		}

		private static void saveLicenseXml(string licenseXml, string installDir)
		{
			if (!string.IsNullOrEmpty(licenseXml) && licenseXml != "0")
			{
				var file = Path.Combine(installDir, "Settings", "License.xml");
				File.WriteAllText(file, licenseXml, Encoding.UTF8);
			}
		}

		public static void executeDatabaseHandler(CASession session, SystemConfiguration sc)
		{
			var dbHandler = new DbHandler(sc, session);
			dbHandler.Execute();
		}

		public ActionResult WrightCCS_CA_RemoveOldResidual(CASession session)
		{
			session.Log("Begin WrightCCS_CA_RemoveOldResidual");
			var key = @"HKEY_LOCAL_MACHINE\System\CurrentControlSet\Services\AuthSrv\Parameters";
			var regStrings = (string[])Microsoft.Win32.Registry.GetValue(key, "ExtensionDLLs", new string[] { });
			var newStrings = new List<string>();
			var modified = false;
			var searchEntry = "Ias.RADIUS.C.dll".ToLowerInvariant();
			foreach (var regString in regStrings)
			{
				if (!string.IsNullOrEmpty(regString) && regString.ToLowerInvariant().Contains(searchEntry) )
					modified = true;
				else
					newStrings.Add(regString);
			}
			if (modified)
				Microsoft.Win32.Registry.SetValue(key, "ExtensionDLLs", newStrings.ToArray());

			return ActionResult.Success;
		}

		public ActionResult WrightCCS_CA_After_InstallFiles_Rollback(CASession session)
		{
			session.Log("Begin WrightCCS_CA_After_InstallFiles_Rollback");
			return ActionResult.Success;
		}

		private void showError(string message)
		{
			MessageBox.Show(message, "SMS2", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		public ActionResult WrightCCS_CA_After_InstallFiles(CASession session)
		{
			session.Log("Begin WrightCCS_CA_After_InstallFiles");
			try
			{
				// Write Configuration.xml
				// Execute SQL create/upgrade

				session.Log("writeConfigurationXmlAndUpgradeDB");
				var configurationXml = ConvertFromBase64(session.Data["ConfigurationXml"]);
				session.Log("ConfigurationXml");
				var configurationClientXml = ConvertFromBase64(session.Data["ConfigurationClientXml"]);
				session.Log("ConfigurationClientXml");
				var licenseXml = ConvertFromBase64(session.Data["LicenseXml"]);
				session.Log("LicenseXml");
				var installDir = session.Data["InstallDir"];
				session.Log("InstallDir");

				if (!string.IsNullOrEmpty(configurationXml) && configurationXml != "0")
				{
					var sc = new SystemConfiguration();
					try
					{
						saveConfigurationXml(configurationXml, installDir);
						sc.LoadSettings(configurationXml);
					}
					catch
					{
						showError("Error saving Configuration file.");
						throw;
					}

					try
					{
						executeDatabaseHandler(session, sc);
					}
					catch (DatabaseHandlerException ex)
					{
						showError(ex.Message);
						throw;
					}
					catch (Exception ex)
					{
						showError(string.Format("Error creating or upgrading database. Please check your SQL credentials. {0}\n{1}", ex.Message, ex.StackTrace));
						throw;
					}
				}

				if (!string.IsNullOrEmpty(configurationClientXml) && configurationClientXml != "0")
				{
					try
					{
						var sc = new SystemConfiguration();
						sc.LoadSettings(configurationClientXml);
						saveClientConfigurationXml(sc, installDir);
					}
					catch
					{
						showError("Error saving Client Configuration file.");
						throw;
					}
				}

				try
				{
					saveLicenseXml(licenseXml, installDir);
				}
				catch
				{
					showError("Error saving License file.");
					throw;
				}


				return ActionResult.Success;
			}
			catch (Exception ex)
			{
				session.Log("EXCEPTION EXCEPTION");
				session.Log("WrightCCS_CA_After_InstallFiles ERROR: " + ex.Message);
				session.Log("WrightCCS_CA_After_InstallFiles STACK: " + ex.StackTrace);
				return ActionResult.Failure;
			}
		}

		public ActionResult WrightCCS_CA_ConfigureAuthEngine(CASession session)
		{
			try
			{
				session.Log("Begin WrightCCS_CA_ConfigureAuthEngine");
				var configurationXml = ConvertFromBase64(session["CONFIGURATIONXML"]);
				var installDir = session["INSTALLLOCATION"];
				var wizard = new Dialog.Wizard(configurationXml, installDir);

				wizard.SessionValues.Add("LICENSEXML", ConvertFromBase64(session["LICENSEXML"]));
				wizard.SessionValues.Add("AEUSERNAME", session["AEUSERNAME"]);
				wizard.SessionValues.Add("AEPASSWORD", session["AEPASSWORD"]);
				wizard.SessionValues.Add("WIX_ACCOUNT_LOCALSYSTEM", session["WIX_ACCOUNT_LOCALSYSTEM"]);
				wizard.SessionValues.Add("WIX_ACCOUNT_LOCALSERVICE", session["WIX_ACCOUNT_LOCALSERVICE"]);
				wizard.SessionValues.Add("WIX_ACCOUNT_NETWORKSERVICE", session["WIX_ACCOUNT_NETWORKSERVICE"]);

				var screens = new List<IWizardScreen>() {
					new Dialog.SelectOrInputLicense(wizard, wizard.sc),
					new Dialog.AuthEngineConfigServiceUser(wizard, wizard.sc),
					new Dialog.AuthEngineConfig01(wizard, wizard.sc),
					new Dialog.AuthEngineConfig03(wizard, wizard.sc),
					new Dialog.AuthEngineConfig02(wizard, wizard.sc),
                    new Dialog.EncryptionKey(wizard, wizard.sc),
					//new Dialog.ViewConfigurationXml(wizard, wizard.sc),
				};
				wizard.SetScreens(screens);
				wizard.SetCurrentScreenIndex(0);
				session["AUTHENGINE_CONFIGURATION_VALID"] = "0";

				WindowHandleWrapper wrapper = null;
				try { wrapper = new WindowHandleWrapper("SMS2 Setup"); }
				catch { }

				DialogResult dr = (wrapper == null) ? wizard.ShowDialog() : wizard.ShowDialog(wrapper);

				if (dr == DialogResult.OK)
				{
					session["AEUSERNAME"] = wizard.SessionValues["AEUSERNAME"];
					session["AEPASSWORD"] = wizard.SessionValues["AEPASSWORD"];
					wizard.SessionValues.Remove("AEUSERNAME");
					wizard.SessionValues.Remove("AEPASSWORD");
					wizard.SessionValues.Remove("WIX_ACCOUNT_LOCALSYSTEM");
					wizard.SessionValues.Remove("WIX_ACCOUNT_LOCALSERVICE");
					wizard.SessionValues.Remove("WIX_ACCOUNT_NETWORKSERVICE");

					session["CONFIGURATIONXML"] = ConvertToBase64(wizard.sc.WriteXMLCredentialsToString());
					foreach (var kv in wizard.SessionValues)
						session[kv.Key] = ConvertToBase64(kv.Value);
					session["AUTHENGINE_CONFIGURATION_VALID"] = "1";
				}
				return ActionResult.Success;
			}
			catch (Exception ex)
			{
				session.Log("EXCEPTION EXCEPTION");
				session.Log(ex.Message);
				session.Log(ex.StackTrace);
			}
			return ActionResult.Failure;
		}

		public ActionResult WrightCCS_CA_ConfigureCloudSMS(CASession session)
		{
			session.Log("Begin WrightCCS_CA_ConfigureCloudSMS");
			var configurationXml = ConvertFromBase64(session["CONFIGURATIONXML"]);
			var installDir = session["INSTALLLOCATION"];
			var wizard = new Dialog.Wizard(configurationXml, installDir);

			wizard.SessionValues.Add("CSMSUSERNAME", session["CSMSUSERNAME"]);
			wizard.SessionValues.Add("CSMSPASSWORD", session["CSMSPASSWORD"]);
			wizard.SessionValues.Add("WIX_ACCOUNT_LOCALSYSTEM", session["WIX_ACCOUNT_LOCALSYSTEM"]);
			wizard.SessionValues.Add("WIX_ACCOUNT_LOCALSERVICE", session["WIX_ACCOUNT_LOCALSERVICE"]);
			wizard.SessionValues.Add("WIX_ACCOUNT_NETWORKSERVICE", session["WIX_ACCOUNT_NETWORKSERVICE"]);

			var screens = new List<IWizardScreen>() {
				new Dialog.CloudSMS.ConfigServiceUser(wizard, wizard.sc),
				new Dialog.CloudSMSConfig01(wizard, wizard.sc),
				//new Dialog.CloudSMSConfig02(wizard, wizard.sc),
					 //new Dialog.ViewConfigurationXml(wizard, wizard.sc),
			};
			wizard.SetScreens(screens);
			wizard.SetCurrentScreenIndex(0);
			session["CLOUDSMS_CONFIGURATION_VALID"] = "0";
			if (wizard.ShowDialog() == DialogResult.OK)
			{
				session["CSMSUSERNAME"] = wizard.SessionValues["CSMSUSERNAME"];
				session["CSMSPASSWORD"] = wizard.SessionValues["CSMSPASSWORD"];
				wizard.SessionValues.Remove("CSMSUSERNAME");
				wizard.SessionValues.Remove("CSMSPASSWORD");
				wizard.SessionValues.Remove("WIX_ACCOUNT_LOCALSYSTEM");
				wizard.SessionValues.Remove("WIX_ACCOUNT_LOCALSERVICE");
				wizard.SessionValues.Remove("WIX_ACCOUNT_NETWORKSERVICE");

				session["CONFIGURATIONXML"] = ConvertToBase64(wizard.sc.WriteXMLCredentialsToString());
				foreach (var kv in wizard.SessionValues)
					session[kv.Key] = ConvertToBase64(kv.Value);
				session["CLOUDSMS_CONFIGURATION_VALID"] = "1";
			}
			return ActionResult.Success;
		}

        public ActionResult WrightCCS_CA_ConfigureMutualAuthImages(CASession session)
        {
            session.Log("Begin WrightCCS_CA_ConfigureMutualAuthImages");
            var configurationXml = ConvertFromBase64(session["CONFIGURATIONXML"]);
            var installDir = session["INSTALLLOCATION"];
            var wizard = new Dialog.Wizard(configurationXml, installDir);

            wizard.SessionValues.Add("MAIUSERNAME", session["MAIUSERNAME"]);
            wizard.SessionValues.Add("MAIPASSWORD", session["MAIPASSWORD"]);
            wizard.SessionValues.Add("WIX_ACCOUNT_LOCALSYSTEM", session["WIX_ACCOUNT_LOCALSYSTEM"]);
            wizard.SessionValues.Add("WIX_ACCOUNT_LOCALSERVICE", session["WIX_ACCOUNT_LOCALSERVICE"]);
            wizard.SessionValues.Add("WIX_ACCOUNT_NETWORKSERVICE", session["WIX_ACCOUNT_NETWORKSERVICE"]);

            var screens = new List<IWizardScreen>() {
				new Dialog.MutualAuthImages.ConfigServiceUser(wizard, wizard.sc)				
			};
            wizard.SetScreens(screens);
            wizard.SetCurrentScreenIndex(0);
            session["MAI_CONFIGURATION_VALID"] = "0";
            if (wizard.ShowDialog() == DialogResult.OK) {
                session["MAIUSERNAME"] = wizard.SessionValues["MAIUSERNAME"];
                session["MAIPASSWORD"] = wizard.SessionValues["MAIPASSWORD"];
                wizard.SessionValues.Remove("MAIUSERNAME");
                wizard.SessionValues.Remove("MAIPASSWORD");
                wizard.SessionValues.Remove("WIX_ACCOUNT_LOCALSYSTEM");
                wizard.SessionValues.Remove("WIX_ACCOUNT_LOCALSERVICE");
                wizard.SessionValues.Remove("WIX_ACCOUNT_NETWORKSERVICE");

                session["CONFIGURATIONXML"] = ConvertToBase64(wizard.sc.WriteXMLCredentialsToString());
                foreach (var kv in wizard.SessionValues)
                    session[kv.Key] = ConvertToBase64(kv.Value);
                session["MAI_CONFIGURATION_VALID"] = "1";
            }
            return ActionResult.Success;
        }

		public ActionResult WrightCCS_CA_ConfigureOATHCalc(CASession session)
		{
			session.Log("Begin WrightCCS_CA_ConfigureOATHCalc");
			var configurationXml = ConvertFromBase64(session["CONFIGURATIONXML"]);
			var installDir = session["INSTALLLOCATION"];
			var wizard = new Dialog.Wizard(configurationXml, installDir);

			wizard.SessionValues.Add("OATHUSERNAME", session["OATHUSERNAME"]);
			wizard.SessionValues.Add("OATHPASSWORD", session["OATHPASSWORD"]);
			wizard.SessionValues.Add("WIX_ACCOUNT_LOCALSYSTEM", session["WIX_ACCOUNT_LOCALSYSTEM"]);
			wizard.SessionValues.Add("WIX_ACCOUNT_LOCALSERVICE", session["WIX_ACCOUNT_LOCALSERVICE"]);
			wizard.SessionValues.Add("WIX_ACCOUNT_NETWORKSERVICE", session["WIX_ACCOUNT_NETWORKSERVICE"]);

			var screens = new List<IWizardScreen>() {
				new Dialog.OathCalc.ConfigServiceUser(wizard, wizard.sc),
				new Dialog.OATHCalcConfig01(wizard, wizard.sc),
					 //new Dialog.ViewConfigurationXml(wizard, wizard.sc),
			};
			wizard.SetScreens(screens);
			wizard.SetCurrentScreenIndex(0);
			session["OATHCALC_CONFIGURATION_VALID"] = "0";
			if (wizard.ShowDialog() == DialogResult.OK)
			{
				session["OATHUSERNAME"] = wizard.SessionValues["OATHUSERNAME"];
				session["OATHPASSWORD"] = wizard.SessionValues["OATHPASSWORD"];
				wizard.SessionValues.Remove("OATHUSERNAME");
				wizard.SessionValues.Remove("OATHPASSWORD");
				wizard.SessionValues.Remove("WIX_ACCOUNT_LOCALSYSTEM");
				wizard.SessionValues.Remove("WIX_ACCOUNT_LOCALSERVICE");
				wizard.SessionValues.Remove("WIX_ACCOUNT_NETWORKSERVICE");

				session["CONFIGURATIONXML"] = ConvertToBase64(wizard.sc.WriteXMLCredentialsToString());
				foreach (var kv in wizard.SessionValues)
					session[kv.Key] = ConvertToBase64(kv.Value);
				session["OATHCALC_CONFIGURATION_VALID"] = "1";
			}
			return ActionResult.Success;
		}

		public ActionResult WrightCCS_CA_ConfigureClients(CASession session)
		{
			session.Log("Begin WrightCCS_CA_ConfigureClients");
			var configurationXml = ConvertFromBase64(session["CONFIGURATIONXML"]);
			var configurationClientXml = ConvertFromBase64(session["CONFIGURATIONCLIENTXML"]);
			var installDir = session["INSTALLLOCATION"];
			var wizard = new Dialog.Wizard(configurationClientXml, installDir);

			var serverConfig = new SystemConfiguration();
			try
			{
				serverConfig.LoadSettings(configurationXml);
			}
			 catch { }

			var screens = new List<IWizardScreen>() {
				new Dialog.ClientsAuthEngine(wizard, wizard.sc, serverConfig),
			};
			wizard.SetScreens(screens);
			wizard.SetCurrentScreenIndex(0);
			session["CLIENTS_CONFIGURATION_VALID"] = "0";
			if (wizard.ShowDialog() == DialogResult.OK)
			{
				session["CONFIGURATIONCLIENTXML"] = ConvertToBase64(wizard.sc.WriteXMLCredentialsToString());
				foreach (var kv in wizard.SessionValues)
					session[kv.Key] = ConvertToBase64(kv.Value);
				session["CLIENTS_CONFIGURATION_VALID"] = "1";
			}
			return ActionResult.Success;
		}

        public ActionResult WrightCCS_CA_ConfigureCommonProperties(CASession session)
        {
            session.Log("Begin WrightCCS_CA_ConfigureCommonProperties");
            var configurationXml = ConvertFromBase64(session["CONFIGURATIONXML"]);
            var installDir = session["INSTALLLOCATION"];
            var wizard = new Dialog.Wizard(configurationXml, installDir);            

            var screens = new List<IWizardScreen>() {
				new Dialog.CommonPropertiesConfig01(wizard, wizard.sc)				
			};
            wizard.SetScreens(screens);
            wizard.SetCurrentScreenIndex(0);
            session["COMMON_PROPERTIES_CONFIGURATION_VALID"] = "0";

            if (wizard.ShowDialog() == DialogResult.OK) {
                session["CONFIGURATIONXML"] = ConvertToBase64(wizard.sc.WriteXMLCredentialsToString());
                foreach (var kv in wizard.SessionValues)
                    session[kv.Key] = ConvertToBase64(kv.Value);
                session["COMMON_PROPERTIES_CONFIGURATION_VALID"] = "1";
            }

            return ActionResult.Success;
        }

		public ActionResult WrightCSS_CA_CheckIAS(CASession session)
		{
			var sm = new ServiceManager("IAS", 7500);
			if (sm.ValidService())
				session["IASSERVICEINSTALLED"] = "1";
			return ActionResult.Success;
		}
	}
}
