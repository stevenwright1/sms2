using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using AuthGateway.Shared;
using Ionic.Zip;
using AuthGateway.Shared.Log;
using AuthGateway.Shared.Log.Loggers;

namespace AuthGateway.Setup.CitrixIntegration
{
		public partial class SetupForm : Form
		{
				SystemConfiguration sc;

				private string sourcePath;

				public SetupForm()
					: this(
					(
					new Uri(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase))
					.LocalPath
					)
					.TrimEnd(new char[] { Path.DirectorySeparatorChar }))
				{
					
				}

				public SetupForm(string path)
				{
						this.sourcePath = path;
						sc = new SystemConfiguration(path);
						try
						{
							sc.LoadSettings();
						}
						catch (SystemConfigurationParseError ex)
						{
							MessageBox.Show(ex.Message, "WrightCCS - Configuration", MessageBoxButtons.OK, MessageBoxIcon.Warning);
						}
						Logger.Instance
								.SetLogLevel(LogLevel.All)
								.SetFlushOnWrite(true)
								.AddLogger(
										new FileLogger(sourcePath + @"Install"
												, "CitrixIntegration.log")
								, LogLevel.All);
						InitializeComponent();
				}

				private void button1_Click(object sender, EventArgs e)
				{
						string sourcePath = sc.GetWrightPath();
						sourcePath = sourcePath.TrimEnd(new char[]{Path.DirectorySeparatorChar});
						sourcePath = sourcePath + Path.DirectorySeparatorChar.ToString();
						DirectoryInfo source = new DirectoryInfo(sourcePath 
								+ @"Install" + Path.DirectorySeparatorChar 
								+ @"CitrixMod" + Path.DirectorySeparatorChar);

						Logger.Instance.WriteToLog(string.Format("SourceDir is '{0}'.",source.FullName), LogLevel.Info);
						if (!cbBackup.Checked && !cbInstall.Checked)
						{
								Logger.Instance.WriteToLog("Nothing to be done.",LogLevel.Info);
								this.DialogResult = DialogResult.OK;
								this.Close();
								return;
						}
						try
						{
								string targetDir = tbCitrixPath.Text.Trim();

								if (string.IsNullOrEmpty(targetDir))
										throw new Exception("Target path is invalid.");
								DirectoryInfo target = new DirectoryInfo(targetDir);
								Logger.Instance.WriteToLog(string.Format("TargetDir is '{0}'.", target.FullName), LogLevel.Info);
								if (!source.Exists)
										throw new Exception("Source path is invalid.");
								if (!target.Exists)
										throw new Exception("Target path is invalid.");

								string backupFile = source.Parent.FullName+Path.DirectorySeparatorChar
												+DateTime.Now.ToString("backup-yyyyMMdd.HHmmss")
												+".zip";

								if (cbBackup.Checked)
								{
										Logger.Instance.WriteToLog("Creating backup.", LogLevel.Info);
										makeBackup(target, backupFile);
										Logger.Instance.WriteToLog("Backup created in: " + backupFile, LogLevel.Info);
								}

								if (cbInstall.Checked)
								{
										Logger.Instance.WriteToLog("Copying customization files to: "+target.FullName, LogLevel.Info);
										CopyAll(source, target);
										CreateConfig(target);
										ReplaceConfigPath(target);
										Logger.Instance.WriteToLog("Customization files copied.", LogLevel.Info);
								}

								Logger.Instance.WriteToLog("Install finished.", LogLevel.Info);
								MessageBox.Show("Install successful.", "Citrix Wi customizations", MessageBoxButtons.OK, MessageBoxIcon.Information);
								this.DialogResult = DialogResult.OK;
								this.Close();
						}
						catch (Exception ex)
						{
								MessageBox.Show(ex.Message, "Citrix Wi customizations", MessageBoxButtons.OK, MessageBoxIcon.Error);
								Logger.Instance.WriteToLog("ERROR Message: " + ex.Message, LogLevel.Error);
								Logger.Instance.WriteToLog("ERROR Stack: " + ex.StackTrace,LogLevel.Error);
						}
				}

				public void ReplaceConfigPath(DirectoryInfo target)
				{
						string search = "confPath = \"C:/inetpub/wwwroot/Citrix/DesktopWeb/conf/\";";
						string replace = ("confPath = \""+Path.Combine(target.FullName,"conf")+Path.DirectorySeparatorChar+"\";")
								.Replace("\\","\\\\");

						string fileToReplace = Path.Combine(target.FullName, "app_code", "PagesJava", "custom", "auth", "TcpClients.java");
						string fileContent = File.ReadAllText(fileToReplace);
						Logger.Instance.WriteToLog(
										string.Format("Replacing path in '{0}' to {1}.", fileToReplace, replace)
										, LogLevel.Info);
						fileContent = fileContent.Replace(search, replace);
						File.WriteAllText(fileToReplace, fileContent);
				}

				public void CreateConfig(DirectoryInfo target)
				{
						Logger.Instance.WriteToLog(
										string.Format("Creating config.")
										, LogLevel.Info);
						DirectoryInfo nextTargetDir = new DirectoryInfo(target.FullName + Path.DirectorySeparatorChar + "conf");
						if (!nextTargetDir.Exists)
								nextTargetDir = target.CreateSubdirectory("conf");
						sc.WriteClientCredentials(nextTargetDir.FullName + Path.DirectorySeparatorChar + "Configuration.xml");
						Logger.Instance.WriteToLog(
										string.Format("Created config in '{0}'.", nextTargetDir.FullName + Path.DirectorySeparatorChar + "Configuration.xml")
										, LogLevel.Info);
				}

				public void CopyAll(DirectoryInfo source, DirectoryInfo target)
				{
						if (Directory.Exists(target.FullName) == false)
						{
								Logger.Instance.WriteToLog(
										string.Format("Creating directory '{0}'", target.FullName)
										, LogLevel.Info);
								Directory.CreateDirectory(target.FullName);
						}

						foreach (FileInfo fi in source.GetFiles())
						{
								string targetFile = Path.Combine(target.ToString(), fi.Name);
								Logger.Instance.WriteToLog(
										string.Format("Copying file '{0}' to '{1}'.",fi.FullName,targetFile)
										, LogLevel.Info);
								fi.CopyTo(targetFile, true);
						}

						foreach (DirectoryInfo diSourceDir in source.GetDirectories())
						{
								DirectoryInfo nextTargetDir = new DirectoryInfo(target.FullName + Path.DirectorySeparatorChar + diSourceDir.Name);
								if (!nextTargetDir.Exists)
								{
										Logger.Instance.WriteToLog(
												string.Format("Creating directory '{0}'", target.FullName + Path.DirectorySeparatorChar + diSourceDir.Name)
												, LogLevel.Info);
										nextTargetDir = target.CreateSubdirectory(diSourceDir.Name);
								}
								CopyAll(diSourceDir, nextTargetDir);
						}
				}

				public void makeBackup(DirectoryInfo source, string target)
				{
						using (ZipFile zip = new ZipFile())
						{
								Logger.Instance.WriteToLog(
										string.Format("Adding '{0}' to backup zip file.", source.FullName)
										, LogLevel.Info);
								zip.AddDirectory(source.FullName);
								Logger.Instance.WriteToLog(
										string.Format("Saving backup zip file to '{0}'.", target)
										, LogLevel.Info);
								zip.Save(target);
						}
				}

				public string SourceDir { get; set; }

				private void btnBrowse_Click(object sender, EventArgs e)
				{
						if (fbDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
						{
								tbCitrixPath.Text = fbDialog.SelectedPath;
						}
				}
		}
}
