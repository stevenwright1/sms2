using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

using AuthGateway.OATH;
using AuthGateway.OATH.XmlProcessor;
using AuthGateway.Shared;
using AuthGateway.Shared.Client;
using AuthGateway.Shared.Log;
using AuthGateway.Shared.Log.Loggers;
using AuthGateway.Shared.Serializer;

using NUnit.Framework;

[TestFixture]
public class AsyncTest
{
	public class StateC
	{
		public bool done = false;
		public int id;
		public string xml = string.Empty;
		public DateTime startTime;
	}

	private List<StateC> doneStates = new List<StateC>();
	private AsyncServer server;

	private SystemConfiguration sc;

	public class OathCalcSender : ReqSender<tokenquery, tokenresponse>
	{

		public OathCalcSender(SystemConfiguration sc)
			: base(sc.OATHCalcServerIp, sc.OATHCalcServerPort, sc.OATHCalcUseEncryption, sc.OATHCalcPublicKey)
		{
			
		}

		public override string SendData(tokenquery req)
		{
			return base.SendData(req);
		}

		protected override string Name
		{
			get { return "OathCalcSender"; }
		}
	}

	[Test]
	public void TestAsyncServerEncrypt()
	{
		var serverThread = new Thread(new ParameterizedThreadStart(startServer));
		try
		{
			Logger.Instance.AddLogger(new ConsoleLogger(), LogLevel.All);
			Logger.Instance.SetLogLevel(LogLevel.All);
			Logger.Instance.SetFlushOnWrite(true);
			sc = new SystemConfiguration();
			sc.OATHCalcServerIp = IPAddress.Parse("127.0.0.1");
			sc.OATHCalcServerPort = 9992;
			sc.OATHCalcUseEncryption = true;
			serverThread.Start(sc);

			while(!serverStarted) {
				System.Threading.Thread.Sleep(50);
			}

			var xml = @"<tokenquery>
														<key>123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890</key>
														<hash>totp</hash>
														<hexhash>0</hexhash>
														<counter>0</counter>
														<digits>6</digits>
														<aftervalues>1</aftervalues>
														<beforevalues>2</beforevalues>
														<window>30</window>
													</tokenquery>";

			var c = new OathCalcSender(sc);
			var tq = Generic.Deserialize<tokenquery>(xml);
			var rawResponse = c.SendData(tq);
			var response = Generic.Deserialize<tokenresponse>(rawResponse);
		}
		finally
		{
			try {
				stopServer();
				serverThread.Join();
			}
			finally{
				Logger.Instance.Clear();
				Logger.Instance.EmptyLoggers();
			}
		}
	}

	[Test]
	public void TestAsyncServerThreads()
	{
		var xml = @"<tokenquery>
													<key>123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890</key>
													<hash>totp</hash>
													<hexhash>0</hexhash>
													<counter>0</counter>
													<digits>6</digits>
													<aftervalues>1</aftervalues>
													<beforevalues>2</beforevalues>
													<window>30</window>
												</tokenquery>";
		var serverThread = new Thread(new ParameterizedThreadStart(startServer));
		serverThread.Start(null);
		doneStates.Clear();
		var startTime = DateTime.UtcNow;
		for(int i = 0; i<1000; i++) {
			var t = new Thread(new ParameterizedThreadStart(sendToServer));
			var s = new StateC();
			s.xml = xml;
			s.id = i;
			t.Start(s);
		}
		while (doneStates.Count < 1000)
			Thread.Sleep(100);
		
		while (!doneStates[doneStates.Count - 1].done)
			Thread.Sleep(100);
		
		Console.WriteLine(String.Format("Whole test took: {0} milliseconds", (DateTime.UtcNow - startTime).TotalMilliseconds));
		stopServer();
		Assert.AreEqual(1000, doneStates.Count, "Threads should've finished");
		foreach(var state in doneStates)
			Assert.IsTrue(state.done);
	}
		
	private bool serverStarted = false;
	private void startServer(object scObj)
	{
		SystemConfiguration sc = new SystemConfiguration();
		sc.OATHCalcServerIp = IPAddress.Parse("127.0.0.1");
		sc.OATHCalcServerPort = 9992;
		sc.OATHCalcUseEncryption = false;
		sc.StopServiceOnException = true;
	
		if (scObj != null) {
			sc = (SystemConfiguration)scObj;
		}
		server = new AsyncServer();
		serverStarted = true;
		server.StartServer(sc, 250);
	}
	
	private void stopServer()
	{
		if (((server != null))) {
			server.StopServer();
		}
		serverStarted = false;
	}

	private void sendToServer(object stateObj)
	{
		var state = (StateC)stateObj;
		string server = "127.0.0.1";
		int port = 9992;
		TcpClient tcpc = new TcpClient();
		state.startTime = System.DateTime.UtcNow;
		try {
			Console.WriteLine(string.Format("{2} | Connecting to {0}:{1}", server, port, state.id));
			tcpc.Connect(server, port);
		} catch (SocketException ex) {
			Console.WriteLine(string.Format("{2} | Couldn't connect to {0}:{1} - Error: {3}", server, port, state.id, ex.Message));
			return;
		}
		NetworkStream netStream = tcpc.GetStream();
		string xmlContent = state.xml;
		//Console.WriteLine(String.Format("{1} | XML Content to send {0}", xmlContent, state.id))
		byte[] byteContent = Encoding.UTF8.GetBytes(xmlContent);
		Console.WriteLine(string.Format("{1} | Sending {0} bytes", byteContent.Length, state.id));
		netStream.Write(byteContent, 0, byteContent.Length);
		byteContent = null;
		Console.WriteLine(string.Format("{0} | Sent, waiting for answer", state.id));
		string outputXml = string.Empty;
		byte[] bytes = new Byte[1024];
		try {
			do {
				int bread = netStream.Read(bytes, 0, bytes.Length);
				Console.WriteLine(string.Format("{1} | Read {0} bytes", bread, state.id));
				outputXml += TrimAll(Encoding.UTF8.GetString(bytes));
			} while (netStream.DataAvailable);
		//Console.Write(outputXml)
		} catch (IOException ex) {
			Console.WriteLine(string.Format("{0} | Connection closed unexpectedly. Error: {1}", state.id, ex.Message));
		}
		tcpc.Close();
		Console.WriteLine(string.Format("{1} | Finished in: {0}", (System.DateTime.UtcNow - state.startTime).TotalMilliseconds, state.id));
		state.done = true;
		doneStates.Add(state);
	}
	
	public string TrimAll(string TextIn, string TrimChar = " ", string CtrlChar = "\0")
	{
		return TextIn.TrimEnd(new char[] { Convert.ToChar("\0") }).Trim();
	}
}