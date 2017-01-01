using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using AuthGateway.Shared;
using SMSService.Modules;
using AuthGateway.Shared.Log;

namespace AuthGateway.GSM.Gui
{
	public partial class Main : Form, ILogger
	{
		string lastCom = string.Empty;
		int lastBaud = -1;
		SerialPort sp;

		public Main()
		{
			InitializeComponent();
			Logger.Instance.AddLogger(this, LogLevel.All);
			Logger.Instance.SetLogLevel(LogLevel.All);
			Logger.Instance.SetFlushOnWrite(true);
		}

		private void init()
		{
			Logger.Instance.AddLogger(this, LogLevel.All);
			var c = new CloudSMSModuleConfig();
			c.TypeName = "GSM";
			c.ModuleParameters = new ModuleParameters()
			{
				new ModuleParameter() {
					Name = "input1", Value = "atz"
				},
				new ModuleParameter() {
					Name = "input2", Value = "at+cfun=1"
				},
				new ModuleParameter() {
					Name = "input3", Value = "at+cops?"
				},
				new ModuleParameter() {
					Name = "input4", Value = "at+cmgf=1"
				},
				new ModuleParameter() {
					Name = "input5", Value = "at+cmgs=\"07525218010\""
				},
				new ModuleParameter() {
					Name = "input6", Value = "{message}" + (char)26
				},
				new ModuleParameter() {
					Name = "expected1", Value = "OK"
				},
			};
		}

		private void openPortIfNecesary()
		{
			int baud = Convert.ToInt32(numericUpDown1.Value);
			if (sp == null || lastBaud != baud || lastCom != tbCOM.Text)
			{
				closePort();
				sp = new SerialPort(tbCOM.Text, baud, Parity.None, 8, StopBits.One);
				lastCom = tbCOM.Text;
				lastBaud = baud;
			}
			if (!sp.IsOpen)
				sp.Open();
		}

		private void closePort()
		{
			if (sp == null)
				return;
			sp.Dispose();
		}

		private void btnSend_Click(object sender, EventArgs e)
		{
			openPortIfNecesary();
			sp.Write(tbBuffer.Text + "\r\n");
			tbBuffer.Text = string.Empty;
		}

		private void button1_Click(object sender, EventArgs e)
		{
			openPortIfNecesary();
			var sb = new StringBuilder();
			var sb2 = new StringBuilder();
			while (sp.BytesToRead > 0)
			{
				sb.Append(sp.ReadExisting());
			}
			tbBuffer.Text = sb.ToString();
		}

		private void button2_Click(object sender, EventArgs e)
		{
			openPortIfNecesary();
			sp.Write((char)26 + "\r\n");
		}

		private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
		{

		}

		public void Write(LogEntry message)
		{
			tbBuffer.AppendText(message.Message + Environment.NewLine);
		}

		private void button1_Click_1(object sender, EventArgs e)
		{
			var commands = new List<GsmCommand>()
			{
				new GsmCommand() { Send = "atz", Expect = "OK" },
				new GsmCommand() { Send = "at^curc=0", Expect = "OK" },
				new GsmCommand() { Send = "at+cfun=1", Expect = "OK" },
				new GsmCommand() { Send = "at+cops?", Expect = "OK" },
				new GsmCommand() { Send = "at+cmgf=1", Expect = "OK" },
				new GsmCommand() { Send = "at+csmp=17,167,0,16", Expect = "OK" },
				new GsmCommand() { Send = "at+cmgs=\"07525218010\"", Expect = ">", Delay = 200 },
				new GsmCommand() { Send = "this is gsm gui test message" + (char)26, Expect = @"\+CMG?S", Delay = 500, ReceiveTries = 120 },
			};
			var s = new GsmRealSender("testgui");
			s.Send(tbCOM.Text.Trim(), Convert.ToInt32(numericUpDown1.Value), commands);
		}
	}
}
