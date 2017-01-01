using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AuthGateway.Shared;
using AuthGateway.Shared.XmlMessages.Response;
using AuthGateway.Shared.XmlMessages;
using AuthGateway.Shared.XmlMessages.Request;
using AuthGateway.Shared.Serializer;
using AuthGateway.Shared.Log;
using System.IO;
using System.Net;
using AuthGateway.Shared.XmlMessages.Request.Command.AuthEngine;
using AuthGateway.Shared.XmlMessages.Response.Ret.AuthEngine;

namespace AuthGateway.GUIClient
{
	public partial class frmClient : Form
	{
		private SystemConfiguration sc;
		public frmClient()
		{
			InitializeComponent();
		}

		private string sendCmd(CommandBase cmd)
		{
			var tcpClient = TimeOutSocket.Connect(
				new IPEndPoint(sc.getAuthEngineServerAddress(), sc.AuthEngineServerPort),
				2000
				);
			tcpClient.SendTimeout = 5000;
			tcpClient.ReceiveTimeout = 5000;
			var ns = tcpClient.GetStream();

			var req = new AuthEngineRequest();
			req.Commands.Add(cmd);
			var tmpstring = Generic.Serialize<AuthEngineRequest>(req);
			Logger.Instance.WriteToLog("Sent: " + tmpstring, LogLevel.Debug);
			var sendBytes = Encoding.UTF8.GetBytes(tmpstring);
			ns.Write(sendBytes, 0, sendBytes.Length);

			MemoryStream retData = new MemoryStream();
			var output = new Byte[tcpClient.ReceiveBufferSize];
			do
			{
				int bread = ns.Read(output, 0, output.Length);
				retData.Write(output, 0, bread);
			} while (ns.DataAvailable);
			retData.Position = 0;

			using (StreamReader sr = new StreamReader(retData))
				tmpstring = sr.ReadToEnd();
			tmpstring = tmpstring.TrimEnd(new char[] { (char)0 }).Trim();
			Logger.Instance.WriteToLog("Received: " + tmpstring, LogLevel.Debug);

			if (string.IsNullOrEmpty(tmpstring))
				throw new Exception("Server response was empty.");
			return tmpstring;
		}

		private void frmClient_Load(object sender, EventArgs e)
		{
			sc = new SystemConfiguration(Application.StartupPath, "SettingsPublic");
			try
			{
				sc.LoadSettings();
			}
			catch (SystemConfigurationParseError ex)
			{
				MessageBox.Show(ex.Message, "WrightCCS - Configuration", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
			tbUsername.Text = Environment.MachineName + "\\" + Environment.UserName;
		}

		private void appendText(string text)
		{
			text = DateTime.Now + " - " + text;
			tbResponse.AppendText(text);
		}

		private void btnValidateUser_Click(object sender, EventArgs e)
		{
			var cmd = new ValidateUser();
			cmd.User = tbUsername.Text;
			cmd.PinCode = tbPincode.Text;
			try
			{
				var ret = sendCmd(cmd);
				tbResponse.Clear();
				appendText(ret);
				appendText("\n");
				var aer = Generic.Deserialize<AuthEngineResponse>(ret);
				var vu = (ValidateUserRet)aer.Responses[0];
				if (!string.IsNullOrEmpty(vu.Error))
					return;
				if (!string.IsNullOrEmpty(vu.State))
					tbState.Text = vu.State;
				tbPin.Text = string.Empty;
				tbPincode.Text = string.Empty;
			}
			catch (Exception ex)
			{
				appendText(ex.Message + Environment.NewLine 
					//+ ex.StackTrace
					);
			}
		}

		private void btnValidatePin_Click(object sender, EventArgs e)
		{
			var cmd = new ValidatePin();
			cmd.User = tbUsername.Text;
			cmd.PinCode = tbPincode.Text;
			cmd.State = tbState.Text;
			cmd.Pin = tbPin.Text;
			try
			{
				var ret = sendCmd(cmd);
				tbResponse.Clear();
				appendText(ret);
				appendText("\n");
			
				var aer = Generic.Deserialize<AuthEngineResponse>(ret);
				var vp = (ValidatePinRet)aer.Responses[0];
				if (!string.IsNullOrEmpty(vp.Error))
					return;
				tbState.Text = string.Empty;
				tbPin.Text = string.Empty;
				tbPincode.Text = string.Empty;
			}
			catch (Exception ex)
			{
				appendText(ex.Message + Environment.NewLine 
					//+ ex.StackTrace
					);
			}
		}
	}
}
