using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Threading;
using AuthGateway.AuthEngine.Logic;
using AuthGateway.AuthEngine.Logic.DAL;
using AuthGateway.AuthEngine.Logic.Helpers;
using Newtonsoft.Json;
using AuthGateway.Setup.SQLDB;
using AuthGateway.Shared;
using AuthGateway.Shared.Identity;
using AuthGateway.Shared.Log;
using AuthGateway.Shared.Log.Loggers;
using AuthGateway.Shared.XmlMessages;
using AuthGateway.Shared.XmlMessages.Request.Command.AuthEngine;
using AuthGateway.Shared.XmlMessages.Response.Ret.AuthEngine;
using AuthGateway.OATH;

using NUnit.Framework;
using AuthGateway.AuthEngine.Helpers;

namespace AuthGateway.AuthEngine.Unittest
{
	//[TestFixture]
	public abstract class TestWithServers
	{
		private static bool ran = false;
		private static bool testInTransaction = true;

		protected SystemConfiguration sc;
		protected string path;

		protected Registry registry;

		protected Thread aeServerThread = null;
		protected ServerAccess aeServer;
		protected ServerLogic aeServerLogic;

		protected Thread csServerThread = null;
		protected SMSService.Server csServer;
		protected SMSService.ServerLogic csServerLogic;

		protected Thread oathServerThread = null;
		protected AsyncServer oathServer;

		protected CounterLogger counterLogger;

		protected IIdentity CurrentIdentity;
		protected string Username;
		protected string Org;
		protected string OrgUsername;

		const string TEST_DB_NAME = "SMS_DB_unittest";
		
		#region "Unit test setup"
		[SetUp]
		public void SetupTest()
		{
			Logger.Instance.WriteToLog("SetupTest - Start", LogLevel.Info);
			Console.WriteLine("SetupTest - Start");
			CleanTest();
			Logger.Instance.SetFlushOnWrite(true);
			Logger.Instance.SetLogLevel(LogLevel.All);
			counterLogger = new CounterLogger();
			Logger.Instance.AddLogger(new ConsoleLogger(), LogLevel.All);
			Logger.Instance.AddLogger(counterLogger, LogLevel.Error);

			string path = Assembly.GetExecutingAssembly().Location;
			path = Directory.GetParent(Directory.GetParent(path).FullName).FullName;
			path = path.TrimEnd(Path.DirectorySeparatorChar);
			this.path = path;
			var saver = new LicenseCreator.LicenseHandler();
			saver.Save(path + @"\Settings\license.xml", DateTime.UtcNow.AddDays(1));

			CurrentIdentity = System.Security.Principal.WindowsIdentity.GetCurrent();
			Username = ((IIdentity)CurrentIdentity).GetLogin();
			Org = ((IIdentity)CurrentIdentity).GetDomain();
			OrgUsername = ((IIdentity)CurrentIdentity).Name;

			registry = new Registry();
			
			sc = new SystemConfiguration();
			sc.AuthEngineDefaultEnabled = true;
			sc.AuthEngineDefaultExceptionGroups = new AuthEngineDefaultExceptionGroups() { };
			sc.SetDBData(@"(local)", "", "", "9999", true, "", true, TEST_DB_NAME);
			sc.setServicesData("127.0.0.1", "9061", "127.0.0.1", "9071");
			sc.BaseSendTokenTestMode = true;
			sc.StopServiceOnException = true;

			/*sc.ADServerAddress = "192.168.1.147";
			sc.ADUsername = "Administrator";
			sc.ADPassword = "Password0$";
			sc.ADContainer = "CN=Users";
			sc.ADBaseDN = "DC=pasargentina,DC=corp";

			sc.ManualDomainReplacements.Add(new DomainReplacement() { Name = "NET7DEV" });
			sc.ManualDomainReplacements.Add(new DomainReplacement() { Name = "Guillermo2013" });*/

            sc.ADServerAddress = "192.168.1.219";
            sc.ADUsername = "Administrator";
            sc.ADPassword = "Samson29$";
            sc.ADContainer = "CN=Users";
            sc.ADBaseDN = "DC=mycoolcompany,DC=local";

            sc.ManualDomainReplacements.Add(new DomainReplacement() { Name = "alias1" });
            sc.ManualDomainReplacements.Add(new DomainReplacement() { Name = "alias2" });
            sc.ManualDomainReplacements.Add(new DomainReplacement() { Name = "mycoolcompany" });
			
			sc.AuthEngineUseEncryption = true;
			sc.AuthEnginePinCode = PinCodeOption.False;
			sc.CloudSMSUseEncryption = true;
			sc.Providers = new AuthProviders() {
								new Provider() { Name="CloudSMS", AdGroup="", Enabled=true, Default=true }
								,new Provider() { Name="OATHCalc", AdGroup="", Enabled=true, Config="HOTP,,1" }
								,new Provider() { Name="PINTAN", AdGroup="", Enabled=true, Config="" }
								,new Provider() { Name="Email", AdGroup="", Enabled=true, Config="" }
								,new Provider() { Name="NHS", AdGroup="", Enabled=true, Config="" }
								,new Provider() { Name="OneTime", FriendlyName="XenMobile-Enrolment", AdGroup="", Enabled=true, Config="" }
								,new Provider() { Name="Passthrough", FriendlyName="Passthrough", AdGroup="", Enabled=true, Config="" }
						};
			sc.OATHCalcServerIp = IPAddress.Parse("127.0.0.1");
			sc.OATHCalcServerPort = 9992;
			sc.OATHCalcUseEncryption = false;
			sc.AuthEngineChallengeResponse = true;

			if (!ran || !testInTransaction)
			{
				DBQueriesProvider.SetQueriesProvider(new DBQueriesProviderNew(sc.GetSQLConnectionString()));
				RecreateDatabase(true, true);
				ran = true;
			}
			if (testInTransaction)
			{
				DBQueriesProvider.SetQueriesProvider(new DBQueriesProviderSame(sc.GetSQLConnectionString()));
				DBQueriesProvider.Get().BeginTransaction();
			}
			
			Logger.Instance.WriteToLog("SetupTest - End", LogLevel.Info);
			Console.WriteLine("SetupTest - End");
		}

		private void RecreateDatabase(bool loadusers = true, bool dropifexists = false)
		{
			DatabaseHandler dbHandler = new DatabaseHandler();
			DatabaseStatus dbStatus =
					dbHandler.DoStatus(sc.GetSQLConnectionString(false)
					, sc.GetSQLConnectionString(true), TEST_DB_NAME);
			if (dropifexists && dbStatus.Exists)
			{
				dbHandler.DoDrop(sc.GetSQLConnectionString(false), TEST_DB_NAME);
				dbStatus.Exists = false;
				dbStatus.Version = string.Empty;
			}
			if (!dbStatus.Exists)
				dbHandler.DoCreate(sc.GetSQLConnectionString(false), TEST_DB_NAME);
			dbHandler.DoSchema(sc.GetSQLConnectionString(true), sc.DatabaseName, dbStatus.Version);
			if (loadusers)
			{
				var aeServerLogic = new ServerLogic(sc, registry);
				aeServerLogic.Init();
				aeServerLogic.getUsers();
				aeServerLogic.Stop();
			}
		}

		protected void startAuthEngineServer()
		{
			this.startAuthEngineServer(registry);
		}

		protected void startAuthEngineServer(Registry registry)
		{
			if (aeServerThread != null && 
				(aeServerThread.ThreadState == ThreadState.Running || aeServerThread.ThreadState == ThreadState.Background ) )
				return;
			aeServerThread = new Thread(new ParameterizedThreadStart(aeServerStart));
			aeServerThread.Start(registry);
			while (aeServer == null || !aeServer.Listening || aeServer.GettingUsers)
			{
				Thread.Sleep(500);
			}
			aeServerLogic = new ServerLogic(sc, registry);
			aeServerLogic.Init();
			using (var queries = DBQueriesProvider.Get())
				queries.Query(string.Format(@"UPDATE [SMS_CONTACT] SET [MOBILE_NUMBER] = '1234' WHERE [AD_USERNAME] = '{0}'", this.Username));
		}

		protected void startCloudSMSServer()
		{
			if (csServerThread != null && 
				(csServerThread.ThreadState == ThreadState.Running || csServerThread.ThreadState == ThreadState.Background ) )
				return;
			csServerThread = new Thread(csServerStart);
			csServerThread.Start();
			while (csServer == null || !csServer.Listening)
				Thread.Sleep(100);
			csServerLogic = csServer.getServerLogic();
		}

		protected void startOATHServer()
		{
			if (oathServerThread != null)
				return;
			oathServerThread = new Thread(oathServerStart);
			oathServerThread.Start();
		}

		protected void StopAuthEngineAndCloudSMS()
		{
			if (aeServer != null)
				aeServer.StopService();
			if (csServer != null)
				csServer.StopService();
			if (aeServerLogic != null)
				aeServerLogic.Stop();
			if (csServerLogic != null)
				csServerLogic.Stop();
			if (counterLogger != null)
				counterLogger.Clear();
			if (aeServerThread != null && !(aeServerThread.ThreadState == ThreadState.Running || aeServerThread.ThreadState == ThreadState.Background))
				aeServerThread.Abort();
			if (csServerThread != null && !(csServerThread.ThreadState == ThreadState.Running || csServerThread.ThreadState == ThreadState.Background))
				csServerThread.Abort();
			aeServer = null;
			csServer = null;
			aeServerLogic = null;
			csServerLogic = null;
			aeServerThread = null;
			csServerThread = null;
		}

		[TearDown]
		public void CleanTest()
		{
			Logger.Instance.WriteToLog("CleanTest - Start", LogLevel.Info);
			Console.WriteLine("CleanTest - Start");
			StopAuthEngineAndCloudSMS();
			if (counterLogger != null)
				counterLogger.Clear();
			if (oathServer != null)
				oathServer.StopServer();
			oathServer = null;
			if (oathServerThread != null && !(oathServerThread.ThreadState == ThreadState.Running || oathServerThread.ThreadState == ThreadState.Background))
				oathServerThread.Abort();
			oathServerThread = null;
			Logger.Instance.EmptyLoggers();
			Logger.Instance.Clear();
			try
			{
				if (testInTransaction)
				{
					if (DBQueriesProvider.Get() != null)
						DBQueriesProvider.Get().RollbackTransaction();
				}
			}
			catch
			{

			}			
			
			Logger.Instance.WriteToLog("CleanTest - End", LogLevel.Info);
			Console.WriteLine("CleanTest - End");
		}

		private void csServerStart()
		{
			csServer = new SMSService.Server();
			csServer.StartService(new object[] { sc });
		}

		private void aeServerStart(object registry)
		{
			aeServer = new ServerAccess();
			aeServer.StartService(new object[] { sc, path, (Registry)registry });
		}

		private void oathServerStart()
		{
			oathServer = new AsyncServer();
			oathServer.StartServer(sc, 200);
		}

		#endregion

		public AddFullDetails getInsertAdminData(string user = "test.admin")
		{
			return new AddFullDetails
			{
				Fullname = "Test Admin",
				User = user,
				Org = "testdomain",
				UserType = UserType.Administrator,
				Enabled = true,
				Mobile = "",
				OrgType = "DOMAIN",
				Phone = "",
				Sid = "123",
				Providers = new List<UserProvider> { 
						new UserProvider() { Name = "CloudSMS", Enabled = true }
				},
				AuthEnabled = true
			};
		}
		
		protected AddFullDetails insertAdminUser()
		{
			var afd = getInsertAdminData();
			if (!string.IsNullOrEmpty(aeServerLogic.AddFullUser(afd).Error))
				throw new Exception("Error inserting user.");
			return afd;
		}

		public AddFullDetails getInsertUserData(string pincode = "", string username = "", string org = "")
		{
			if (string.IsNullOrWhiteSpace(username))
				username = "test.user";
			if (string.IsNullOrWhiteSpace(org))
				org = "testdomain";
			var afd = new AddFullDetails()
			{
				Fullname = "Test User",
				User = username,
				Org = org,
				UserType = UserType.User,
				Enabled = true,
				//Mobile = "+447525218010",
				Mobile = "00541169531549",
				OrgType = "DOMAIN",
				Phone = "",
				Sid = "123",
				Email = "steven@wrightccs.com",
				PinCode = pincode,
				Providers = new List<UserProvider>() { 
					new UserProvider() { Name = "CloudSMS", Enabled = true },
					new UserProvider()
					{
						Name = "OATHCalc",
						Config = "HOTP,"
								+ HexConversion.ToString(Encoding.UTF8.GetBytes("12345678901234567890"))
								+ ",0",
								Enabled = true
					}, 
					new UserProvider() { Name = "PINTAN", Enabled = true } ,
					new UserProvider() { Name = "OneTime", Enabled = true } ,
				},
				AuthEnabled = sc.AuthEngineDefaultEnabled == true,
			};
			return afd;
		}

		protected AddFullDetails insertUser(string pincode = "", string username = "", string org = "")
		{
			var afd = getInsertUserData(pincode, username, org);
			if (aeServerLogic == null) aeServerLogic = new ServerLogic(sc);
			if (!string.IsNullOrEmpty(aeServerLogic.AddFullUser(afd).Error))
				throw new Exception("Error inserting user.");
			return afd;
		}

		protected AddFullDetails insertUser2()
		{
			AddFullDetails afd = new AddFullDetails()
			{
				Fullname = "Test User 2",
				User = "test.user.2",
				Org = "testdomain",
				UserType = UserType.User,
				Enabled = true,
				Mobile = "",
				OrgType = "DOMAIN",
				Phone = "",
				Sid = "123",
				Providers = new List<UserProvider>() { new UserProvider() { Name = "CloudSMS", Enabled = true } }
			};
			if (aeServerLogic == null) aeServerLogic = new ServerLogic(sc);
			if (!string.IsNullOrEmpty(aeServerLogic.AddFullUser(afd).Error))
				throw new Exception("Error inserting user.");
			return afd;
		}        

        protected void insertUser3(AddFullDetails userDetails)
        {
            if (aeServerLogic == null) aeServerLogic = new ServerLogic(sc);
            if (!string.IsNullOrEmpty(aeServerLogic.AddFullUser(userDetails).Error))
                throw new Exception("Error inserting user.");
        }

		protected AddFullDetails insertUserCachoDomain()
		{
			AddFullDetails afd = new AddFullDetails()
			{
				Fullname = "Test User",
				User = "test.user",
				Org = "testcacho",
				UserType = UserType.User,
				Enabled = true,
				Mobile = "",
				OrgType = "DOMAIN",
				Phone = "",
				Sid = "123",
				Providers = new List<UserProvider>() { new UserProvider() { Name = "CloudSMS", Enabled = true } }
			};
			if (!string.IsNullOrEmpty(aeServerLogic.AddFullUser(afd).Error))
				throw new Exception("Error inserting user.");
			return afd;
		}

		protected string getOrgOrBaseDN()
		{
			var org = this.Org;
			foreach (var rep in sc.ManualDomainReplacements)
			{
				if (string.Compare(rep.Name, org, true) == 0)
					org = AdHelper.GetDNfromBaseDN(sc.ADBaseDN);
			}
			return org;
		}

		public int RunSQL(string command)
		{
			using (var queries = DBQueriesProvider.Get())
				return queries.NonQuery(command);
		}

		public List<object[]> QueryValSQL(string command)
		{
			var ret = new List<object[]>();
			using (var queries = DBQueriesProvider.Get())
			{
				var table = queries.Query(command);
				if (table.Rows.Count == 0)
					return ret;
				foreach (DataRow row in table.Rows)
					ret.Add(row.ItemArray);
			}
			return ret;
		}

		protected UpdateFullDetailsRet setupTestUserWithPincode(AddFullDetails user, string pincode)
		{
			UserProvider up = new UserProvider()
			{
				Name = "OATHCalc",
				Config = "HOTP,"
						+ HexConversion.ToString(Encoding.UTF8.GetBytes("12345678901234567890"))
						+ ",0"
			};
			SetUserProviderRet ret = (SetUserProviderRet)(aeServerLogic.SetUserProvider(
					new SetUserProvider() { User = user.User, Org = user.Org, Provider = up }));
			Assert.IsTrue(string.IsNullOrEmpty(ret.Error), "Error response is not empty: " + ret.Error);
			Assert.AreEqual(1, ret.Out);

			var adminUser = insertAdminUser();

			UpdateFullDetails ufd = new UpdateFullDetails();
			ufd.Identity = new FakeIdentity(adminUser);
			ufd.Authenticated = true;
			ufd.AuthEnabled = true;
			ufd.Fullname = user.Fullname;
			ufd.Mobile = user.Mobile;
			ufd.Phone = user.Phone;
			ufd.User = user.User;
			ufd.Org = user.Org;
			ufd.UserType = user.UserType;
			ufd.PinCode = pincode;
			ufd.PCChange = true;
            ufd.UPN = string.Format("{0}@{1}", user.User, user.Org);
			return (UpdateFullDetailsRet)aeServerLogic.Actioner.Do(ufd);
		}

		protected UpdateFullDetailsRet setupTestUserWithoutPincode(AddFullDetails user)
		{
			return setupTestUserWithPincode(user, null);
		}

		protected void insertXUsers(object usersObj, int quantity, string orgName = "domainName20K")
		{
			var users = (List<AddFullDetails>)usersObj;
			for (var i = 0; i < quantity; i++)
			{
				AddFullDetails add = new AddFullDetails();
				add.Sid = "up.Sid.Value" + i;
				add.User = "up.SamAccountName" + i;
				add.Phone = string.Empty;
				add.Mobile = string.Empty;
				add.Fullname = "up.Name" + i;
				add.Email = string.Empty;

				add.AuthEnabled = (sc.AuthEngineDefaultEnabled);
				add.UserType = UserType.User;
				foreach (var provider in sc.Providers)
				{
					add.Providers.Add(new UserProvider()
					{
						Name = provider.Name
						,
						Enabled = provider.Enabled
						,
						Provider = provider
					});
				}
				add.Org = orgName;
				add.OrgType = "DOMAIN";
				add.Enabled = true;
				users.Add(add);
			}
		}
	}

	public class FakeIdentity : IIdentity
	{
		public FakeIdentity()
		{

		}
		public FakeIdentity(string username)
		{
			this.Auth = true;
			this.Username = username;
		}
		public FakeIdentity(AddFullDetails afd)
		{
			this.Auth = true;
			this.Username = afd.Org + "\\" + afd.User;
		}
		public bool Auth { get; set; }
		public string Username { get; set; }

		public string AuthenticationType
		{
			get { return string.Empty; }
		}

		public bool IsAuthenticated
		{
			get { return Auth; }
		}

		public string Name
		{
			get { return Username; }
		}
	}

	public class CounterLogger : ILogger
	{
		private List<LogEntry> entries;
		public CounterLogger()
		{
			entries = new List<LogEntry>();
		}
		public void Write(LogEntry message)
		{
			entries.Add(message);
		}
		public bool HasError()
		{
			return entries.Count > 0;
		}
		public void Clear()
		{
			entries.Clear();
		}
	}
	
	public class FakeUserGetterWithList : IUsersGetter
	{
		public FakeUserGetterWithList() {
			this.UserList = new List<AddFullDetails>();
		}
		
		public IList<AddFullDetails> UserList {
			get;
			set;
		}

        public void GetDomainAndReplacements(UserGettersConfig config, out string domain, AdReplacements replacements)
        {
            domain = "";
        }
		
		public void GetUsers(UserGettersConfig config, Func<AddFullDetails, RetBase> action, IUsersGetterResults results = null)
		{
			foreach(var afd in UserList) {
				action(afd);
			}
		}
	}
	
	public class FakeUsersGets : IUsersGetter
	{
		public void GetUsers(UserGettersConfig config, Func<AddFullDetails, RetBase> action, IUsersGetterResults results = null)
		{
			var sc = config.Configuration;
			for (var i = 0; i < 20000; i++)
			{
				Console.WriteLine("Inserting user " + i.ToString());
				var afd = new AddFullDetails();
				afd.AuthEnabled = true;
				afd.Email = "test" + i + "@mail.com";
				afd.Enabled = true;
				afd.Fullname = "Fullname" + i;
				afd.Mobile = "+00541169531549";
				afd.Org = "Org";
				afd.OrgType = "DOMAIN";
				afd.Phone = "Phone" + i;
				afd.Sid = i.ToString();
				afd.User = "UserFaked" + i;
				afd.UserType = UserType.User;
				foreach (var provider in sc.Providers)
				{
					afd.Providers.Add(new UserProvider()
					{
						Name = provider.Name,
						Enabled = provider.Enabled,
						Provider = provider
					});
				}
				action(afd);
			}
		}

        public void GetDomainAndReplacements(UserGettersConfig config, out string domain, AdReplacements replacements)
        {
            domain = "";
        }
	}
	
	public class FakeMailSent {
		public string Config { get; set; }
		public string Email { get; set; }
		public string Subject { get; set; }
		public string Template { get; set; }
		public string AttachmentName { get; set; }
		public bool HasAttachmentStream { get; set; }
	}
	
	public class FakeMailSender : IMailSender {
		public FakeMailSender() {
			MailsSent = new List<FakeMailSent>();
		}
		public List<FakeMailSent> MailsSent {
			get; set;
		}
		
		#region IMailSender implementation
		public void Send(EmailConfig emailConfig, string email, string messageTemplate, string password)
		{
			Send(emailConfig, email, "from Send", messageTemplate, password);
		}
		
		public void Send(EmailConfig emailConfig, string email, string subject, string messageTemplate, string password)
		{
			MailsSent.Add(new FakeMailSent {
			              	Config = JsonConvert.SerializeObject(emailConfig)
		              		, Email = email
		              		, Subject = subject
		              		, Template = messageTemplate });
		}
		
		public void Send(EmailConfig emailConfig, string password, string email, string subject, string message, string attachmentName, MemoryStream attachmentStream)
		{
			message = message.Replace("{attachment}", "here_should_go_the_attachment_cid");
			
			MailsSent.Add(new FakeMailSent {
			              	Config = JsonConvert.SerializeObject(emailConfig)
		              		, Email = email
		              		, Subject = subject
		              		, Template = message
		              		, AttachmentName = attachmentName
			              	, HasAttachmentStream = attachmentStream != null
			              });
		}
		
		public bool IsValidAddress(string emailaddress)
		{
			return true;
		}
		#endregion
		
	}
}
