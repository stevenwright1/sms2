using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text.RegularExpressions;
using AuthGateway.Shared.Log;
using AuthGateway.Shared.Tracking;
using AuthGateway.Shared.XmlMessages.Request.Command.CloudSms;
using AuthGateway.Shared.XmlMessages.Response.Ret.CloudSms;
using System.Text;
using System.Collections.Concurrent;

namespace SMSService.Modules
{
	public class Gsm : BaseModule
	{
		public override string TypeName { get { return "Gsm"; } }

		public Gsm()
		{

		}

		public override SendSmsRet SendSMSMessage(SendSms cmd)
		{
			cmd.Message = replaceCodeAndFullname(cmd.Message, cmd.Code);

			var ret = new SendSmsRet();
			ret.Error = "An unexpected error occurred.";

			send(cmd);

			return ret;
		}

		private void send(SendSms cmd)
		{
			var queueMessage = new QueuedGsm()
			{
				Message = cmd.Message,
				Destination = cmd.Destination,
				Tries = 0,
			};

			var currentMsg = queueMessage;
			try
			{
				currentMsg.Tries++;
				if (currentMsg.Tries > 3)
				{
					Logger.Instance.WriteToLog(this.TypeName + " sendGsmSMS.Dropping Message : "
						+ string.Format("({1}) - {0}", currentMsg.Message, currentMsg.Destination), LogLevel.Info);
					//continue;
				}

				var commands = new List<GsmCommand>();

				var baud = (string)this.getModuleParameterValueOrDefault("baudrate", string.Empty);
				var comname = (string)this.getModuleParameterValueOrDefault("comport", string.Empty);

				for (var i = 1; ; i++) // It breaks when no parameter is found (var input == "")
				{
					var istring = i.ToString();
					var input = (string)this.getModuleParameterValueOrDefault("input" + istring, string.Empty);
					if (input == string.Empty)
						break;

					var delay = this.getModuleParameterValueOrDefault("delay" + istring, "100");
					int delayMilliseconds;
					if (!Int32.TryParse(delay, out delayMilliseconds))
						delayMilliseconds = 100;

					var receiveTriesParameter = this.getModuleParameterValueOrDefault("receiveTries" + istring, "20");
					int receiveTries;
					if (!Int32.TryParse(receiveTriesParameter, out receiveTries))
						receiveTries = 20;

					input = input
						.Replace("{destination}", cmd.Destination)
						.Replace("{message}", cmd.Message + " " + (char)26)
						.Replace("{guid}", Guid.NewGuid().ToString("N"));
					var expectedParameter = this.mc.ModuleParameters.GetByName("expected" + istring);
					var expectedValue = string.Empty;
					var expectedCapture = false;
					if (expectedParameter != null)
					{
						expectedValue = expectedParameter.Value;
						expectedCapture = expectedParameter.Output;
					}

					var gsmCommand = new GsmCommand()
					{
						Send = input,
						Expect = expectedValue,
						Capture = expectedCapture,
						Delay = delayMilliseconds,
						ReceiveTries = receiveTries
					};

					commands.Add(gsmCommand);
				}

				var gsmQueueItem = new GsmQueueItem()
				{
					ComName = comname,
					BaudRate = Int32.Parse(baud),
					Commands = commands
				};
				GsmProcessor.Instance.Process(gsmQueueItem);
			}
			catch (Exception ex)
			{
				Logger.Instance.WriteToLog(this.TypeName + " sendGsmSMS.ERROR: " + ex.Message, LogLevel.Error);
				Logger.Instance.WriteToLog(this.TypeName + " sendGsmSMS.STACK: " + ex.StackTrace, LogLevel.Debug);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
			}
		}
	}

	public class GsmProcessor
	{
		private static GsmProcessor instance = new GsmProcessor();

		private ConcurrentQueue<GsmQueueItem> queue;
		private bool init;
		private IGsmSender sender;

		private GsmProcessor()
		{

		}

		public static GsmProcessor Instance
		{
			get
			{
				if (!instance.init)
					instance.Init();
				return instance;
			}
		}

		public void Init()
		{
			instance.init = true;
			instance.sender = SenderFactory.Get();
			instance.queue = new ConcurrentQueue<GsmQueueItem>();
		}


		public void SetSender(IGsmSender sender)
		{
			this.sender = sender;
		}

		public IGsmSender GetSender()
		{
			return this.sender;
		}

		public void Process(GsmQueueItem newItem)
		{
			queue.Enqueue(newItem);
			while (queue.Count > 0)
			{
				GsmQueueItem item;
				if (queue.TryDequeue(out item))
				{
					sender.Send(item.ComName, item.BaudRate, item.Commands);
				}
			}
		}
	}

	public class SenderFactory
	{
		public static bool Testing { get; set; }
		public static IGsmSender Get()
		{
			if (Testing)
				return new GsmTestSender();
			return new GsmRealSender("Gsm");
		}
	}

	public interface IGsmSender
	{
		void Send(string comName, int baud, List<GsmCommand> commands);
	}

	public class GsmUnexpectedAnswerException : Exception
	{

	}

	public class GsmRealSender : IGsmSender
	{
		private string module;
		private StringBuilder buffer;

		public GsmRealSender(string module)
		{
			this.module = module;
			this.buffer = new StringBuilder();
		}


		public void Send(string comName, int baud, List<GsmCommand> commands)
		{
			try
			{
				using (var sp = new SerialPort(comName, baud, Parity.None, 8, StopBits.One))
				{
					sp.Open();
					sp.ReadTimeout = 5000;
					sp.WriteTimeout = 5000;
					sp.NewLine = "\r\n";

					Logger.Instance.WriteToLog(string.Format("GsmRealSender.Sending '{0}' ({1}) total of {2} commands", comName, baud, commands.Count), LogLevel.Debug);

					sp.DataReceived += new SerialDataReceivedEventHandler(sp_DataReceived);

					foreach (var command in commands)
					{
						buffer.Clear();
						Logger.Instance.WriteToLog("Sending: " + command.Send, LogLevel.Debug);
						sp.WriteLine(command.Send);

						System.Threading.Thread.Sleep(100);

						if (string.IsNullOrEmpty(command.Expect))
						{
							Logger.Instance.WriteToLog("Excepting nothing", LogLevel.Debug);
							continue;
						}

						var re = new Regex(command.Expect);
						var tries = 0;
						var found = false;

						Logger.Instance.WriteToLog(string.Format("GsmRealSender.Trying '{0}' loop with {1} delay", command.ReceiveTries, command.Delay), LogLevel.Debug);

						while (!found && tries < command.ReceiveTries)
						{
							var buf = buffer.ToString();
							var match = re.Match(buf);
							if (match.Success)
							{
								found = true;
							}
							else
							{
								System.Threading.Thread.Sleep(command.Delay);
							}
							tries++;
						}
						if (found)
							Logger.Instance.WriteToLog("Received: " + buffer.ToString(), LogLevel.Debug);
						else
							throw new GsmUnexpectedAnswerException();

						if (!command.Capture)
							continue;

						//var outparms = sc.CloudSMSModuleConfig.ModuleParameters.GetOutputParameters();
						//var groupNames = re.GetGroupNames();
					}

					Logger.Instance.WriteToLog("GsmRealSender.Done", LogLevel.Debug);
				}
			}
			catch (Exception ex)
			{
				Logger.Instance.WriteToLog("GsmSender.ERROR: " + ex.Message, LogLevel.Error);
				Logger.Instance.WriteToLog("GsmSender.STACK: " + ex.Message, LogLevel.Debug);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
			}
			finally
			{
				buffer.Clear();
			}
		}

		void sp_DataReceived(object sender, SerialDataReceivedEventArgs e)
		{
			var sp = (SerialPort)sender;
			int newBytes = sp.BytesToRead;
			byte[] newBuffer = new byte[newBytes];
			sp.Read(newBuffer, 0, newBytes);
			buffer.Append(Encoding.UTF8.GetString(newBuffer));
		}
	}

	public class GsmTestSender : IGsmSender
	{
		public int Messages { get; set; }


		public void Send(string comName, int baud, List<GsmCommand> commands)
		{
			this.Messages++;
		}
	}

	public class QueuedGsm
	{
		public int Tries { get; set; }
		public string Message { get; set; }
		public string Destination { get; set; }
	}

	public class GsmCommand
	{
		public GsmCommand()
		{
			this.Delay = 100;
			this.ReceiveTries = 20;
		}
		public string Send { get; set; }
		public string Expect { get; set; }
		public bool Capture { get; set; }
		public int Delay { get; set; }
		public int ReceiveTries { get; set; }
	}

	public class GsmQueueItem
	{
		public string ComName { get; set; }
		public int BaudRate { get; set; }
		public List<GsmCommand> Commands { get; set; }
	}
}
