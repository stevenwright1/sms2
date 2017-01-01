
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

using AuthGateway.AuthEngine.Helpers;
using AuthGateway.AuthEngine.Logic;
using AuthGateway.Shared;
using AuthGateway.Shared.Log;
using AuthGateway.Shared.XmlMessages.Request.Command.AuthEngine;
using AuthGateway.Shared.XmlMessages.Response.Ret.AuthEngine;
using Caliburn.Micro;
using Microsoft.Win32;

namespace AuthGateway.Tester
{
	public class SckLogger : ILogger {
		private IObservableCollection<string> logs;
		private Action<LogEntry> writeProc;
		
		public SckLogger(IObservableCollection<string> logs, Action<LogEntry> writeProc)
		{
			this.writeProc = writeProc;
			this.logs = logs;
		}
		
		public void Write(LogEntry message)
		{
			writeProc(message);
		}
	}
	
	public class ProgressPageModel : PropertyChangedBase
	{
		private string adPassword = string.Empty;
		public ProgressPageModel(ConfigPageModel cpm) {
			this.cpm = cpm;
			adPassword = cpm.AdPassword;
			this.Users = new BindableCollection<AddFullDetails>();
			this.LoggerLogs = new BindableCollection<string>();
		}
		
		private ConfigPageModel cpm { get; set; }
		
		public IObservableCollection<AddFullDetails> Users { get; set; }
		
		public IObservableCollection<string> LoggerLogs { get; set; }
		
		public int polled { get; set; }
		public int polledProcessed { get; set; }
		public int polledActive { get; set; }
		public int polledInactive { get; set; }
		public int polledInDb { get; set; }
		public int polledSkipped { get; set; }
		public int dbUsers { get; set; }
		public int dbUsersActive { get; set; }

		private int _toProcess = 0;
		public int ToProcess
		{
			get { return this._toProcess; }
			set
			{
				if (this._toProcess != value)
				{
					this._toProcess = value;
					NotifyOfPropertyChange(() => ToProcess);
				}
			}
		}

		private int _progress;
		public int Progress
		{
			get { return this._progress; }
			set
			{
				if (this._progress != value)
				{
					this._progress = value;
					NotifyOfPropertyChange(() => Progress);
				}
			}
		}
		
		private bool _nextEnabled = false;
		public bool NextEnabled
		{
			get { return _nextEnabled; }
			set
			{
				if (this._nextEnabled != value)
				{
					this._nextEnabled = value;
					NotifyOfPropertyChange(() => NextEnabled);
				}
			}
		}
		
		private string _message;
		public string Message {
			get { return _message; }
			set { _message = value; NotifyOfPropertyChange(() => Message); }
		}

		public void Fill()
		{
			this.Users.Clear();
			this.LoggerLogs.Clear();
			var syncContext = SynchronizationContext.Current;
			
			var onlineServers = new ConcurrentQueue<string>();
			onlineServers.Enqueue(this.cpm.AdServer);
			
			var offlineServers = new ConcurrentQueue<string>();
			cpm.sc.ADPassword = adPassword;
			var getterConfig = new UserGettersConfig(cpm.sc, onlineServers, offlineServers);
			
			var replacements = new AdReplacements();
			if (!string.IsNullOrWhiteSpace(cpm.sc.ADBaseDN)) {
				var domainName = AdHelper.GetDNfromBaseDN(cpm.sc.ADBaseDN);
				foreach (var rep in cpm.sc.ManualDomainReplacements)
				{
					if (string.IsNullOrWhiteSpace(rep.Name))
						continue;
					replacements.Add(rep.Name, domainName);
				}
			}
			
			
			var getter = new AdOrLocalUsersGetter();
			new Thread(new ThreadStart(
				() => {
					Logger.I.EmptyLoggers();
					Logger.I.SetLogLevel(LogLevel.DebugVerbose);
					Logger.I.SetFlushOnWrite(true);
					Logger.I.AddLogger(new SckLogger(
						this.LoggerLogs,
						message => {
							syncContext.Post(p => {
							                 	this.LoggerLogs.Insert(0, string.Format(
							                 		//this.LoggerLogs.Add(string.Format(
							                 		"{0}-{1} | {2} | {3}", message.LogDate, message.LogTime, message.Level.ToString(),
							                 		message.Message
							                 	));
							                 }, null);
						}), LogLevel.DebugVerbose);

					var results = new CrazyUsersResults(
						x => { syncContext.Post(p => { this.ToProcess = x; this.polled = x; }, null); },
						x => { syncContext.Post(p => { this.Progress = x; }, null); }
					);
					var connString = this.cpm.sc.GetSQLConnectionString(true);
					
					try {
						syncContext.Post(p => { this.Message = "Starting process..."; }, null);
						using(var dbQuery = new DBQuery(connString)) {

							var usersInDb = dbQuery.Query("SELECT COUNT(*) FROM SMS_CONTACT");
							if (usersInDb.Rows.Count > 0) {
								syncContext.Post(p => { this.dbUsers = usersInDb.Rows[0].Field<int>(0); }, null);
							}

							var usersInDbActive = dbQuery.Query("SELECT COUNT(*) FROM SMS_CONTACT WHERE [userStatus] <> 0");
							if (usersInDbActive.Rows.Count > 0) {
								syncContext.Post(p => { this.dbUsersActive = usersInDbActive.Rows[0].Field<int>(0); }, null);
							}
						}

                        string domain = "";
                        getter.GetDomainAndReplacements(getterConfig, out domain, replacements);
						
						getter.GetUsers(getterConfig, cmd => {
						                	var ret = new AddFullDetailsRet();
						                	if (results.Total == 0)
						                		return ret;
						                	
						                	var cmdUserOrg = cmd as UserOrgCommand;
						                	if (cmdUserOrg != null)
						                		cmdUserOrg.Org = replacements.ReplacementFor(cmdUserOrg.Org);
						                	
						                	DataTable userIsInDb;
						                	using(var dbQuery = new DBQuery(connString)) {
						                		userIsInDb = dbQuery.Query(
						                			"SELECT TOP 1 ID, SID FROM SMS_CONTACT WHERE AD_USERNAME = @USER AND ORG_NAME=@ON",
						                			new DBQueryParm("USER", cmdUserOrg.User),
						                			new DBQueryParm("ON", cmdUserOrg.Org)
						                		);
						                	}
						                	
						                	syncContext.Post(p => {
						                	                 	this.Users.Add(cmd);
						                	                 	
						                	                 	if (cmd.Enabled != null && cmd.Enabled.Value)
						                	                 		this.polledActive++;
						                	                 	else
						                	                 		this.polledInactive++;

						                	                 	if (userIsInDb.Rows.Count > 0) {
						                	                 		this.polledInDb++;
						                	                 	}
						                	                 	
						                	                 	this.Message = string.Format("Processed {0}... ({1})", cmd.User, results.Processed);
						                	                 }, null);
						                	return ret;
						                }, results);
						
						syncContext.Post(p => { this.NextEnabled = true; }, null);
						this.polledProcessed = results.Valid;
						this.polledSkipped = results.SkippedInvalidUserAccountControl + results.SkippedNullSid;
					} catch(Exception ex) {
						syncContext.Post(p => {
						                 	this.Message = string.Format("Error, exception thrown in Wright CCS tester");
						                 	var i = 1;
						                 	var curex = ex;
						                 	this.LoggerLogs.Insert(0, string.Format("ERROR {0}: {1}", i, curex.Message));
						                 	while(curex.InnerException != null) {
						                 		i++;
						                 		curex = ex.InnerException;
						                 		this.LoggerLogs.Insert(0, string.Format("ERROR {0}: {1}", i, curex.Message));
						                 	}

						                 	
						                 	
						                 	i++;
						                 }, null);
						
					} finally {
						Logger.I.EmptyLoggers();
					}
				}
			)).Start();
		}
		
		private void GetUsersWork() {
			
		}
	}

	public class CrazyUsersResults : IUsersGetterResults
	{
		public CrazyUsersResults(Action<int> totalCb, Action<int> processedCb)
		{
			this.totalCb = totalCb;
			this.processedCb = processedCb;
		}

		private Action<int> totalCb;
		private Action<int> processedCb;

		private int _total;
		private int _processed;
		private int _valid;
		private int _skippedNullSid;
		private int _skippedInvalidUAC;

		public int Total
		{
			get
			{
				return _total;
			}
			set
			{
				_total = value;
				if (totalCb != null)
					totalCb(value);
			}
		}

		public int Processed
		{
			get
			{
				return _processed;
			}
			set
			{
				_processed = value;
				if (processedCb != null)
					processedCb(value);
			}
		}

		public int Valid
		{
			get
			{
				return _valid;
			}
			set
			{
				_valid = value;
			}
		}

		public int SkippedNullSid
		{
			get
			{
				return _skippedNullSid;
			}
			set
			{
				_skippedNullSid = value;
			}
		}

		public int SkippedInvalidUserAccountControl
		{
			get
			{
				return _skippedInvalidUAC;
			}
			set
			{
				_skippedInvalidUAC = value;
			}
		}
	}

	/// <summary>
	/// Interaction logic for ProgressPage.xaml
	/// </summary>
	public partial class ProgressPage : Page
	{
		private ProgressPageModel vmodel;
		
		public ProgressPage(ProgressPageModel ppm)
		{
			InitializeComponent();
			
			this.vmodel = ppm;
			this.DataContext = ppm;
			
			this.Loaded += new RoutedEventHandler(ProgressPage_Loaded);
		}

		void ProgressPage_Loaded(object sender, RoutedEventArgs e)
		{
			this.vmodel.Fill();
		}
		
		void testProgress_Completed(object sender, EventArgs e){
			btnNext.IsEnabled = true;
		}
		
		private void Next_Click(object sender, RoutedEventArgs e)
		{
			var logLines = new List<string>();
			foreach(var line in this.vmodel.LoggerLogs)
				logLines.Insert(0, line);
			
			var nextPage = new ResultsPage(
				new ResultsPageModel(logLines) {
					dbUsers = vmodel.dbUsers,
					dbUsersActive = vmodel.dbUsersActive,
					polled = vmodel.polled,
					polledProcessed = vmodel.polledProcessed,
					polledActive = vmodel.polledActive,
					polledInactive = vmodel.polledInactive,
					polledInDb = vmodel.polledInDb,
					polledSkipped = vmodel.polledSkipped,
				}
			);
			this.NavigationService.Navigate(nextPage);
		}
		
		private void Save_Click(object sender, RoutedEventArgs e)
		{
			var dlg = new SaveFileDialog();
			dlg.DefaultExt = ".txt";
			dlg.Filter = "Text documents (.txt)|*.txt";
			
			var dlgResult = dlg.ShowDialog();
			if (dlgResult == null || dlgResult == false)
				return;
			
			try
			{
				using (var textWriter = new StreamWriter(dlg.OpenFile())) {
					foreach(var msg in this.vmodel.LoggerLogs.Reverse())
						textWriter.WriteLine(msg);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("Error: Could not read export log to disk. Original error: " + ex.Message,
				                "AuthGateway.Tester", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}
	}

	public class DBQuery : IDisposable
	{
		private SqlConnection connection;
		
		public DBQuery(string connString) {
			connection = new SqlConnection(connString);
		}
		
		public List<object[]> QueryValSQL(string commandSql, params DBQueryParm[] commandParameters)
		{
			var parms = new List<DBQueryParm>();
			foreach(var parm in commandParameters)
				parms.Add(parm);
			return QueryValSQL(commandSql, parms);
		}
		
		public List<object[]> QueryValSQL(string commandSql, List<DBQueryParm> commandParameters)
		{
			var ret = new List<object[]>();
			var table = Query(commandSql, commandParameters);
			if (table.Rows.Count == 0)
				return ret;
			foreach (DataRow row in table.Rows)
				ret.Add(row.ItemArray);
			return ret;
		}

		public DataTable Query(string commandSql, params DBQueryParm[] commandParameters)
		{
			var parms = new List<DBQueryParm>();
			foreach(var parm in commandParameters)
				parms.Add(parm);
			return Query(commandSql, parms);
		}
		
		public DataTable Query(string commandSql, List<DBQueryParm> commandParameters)
		{
			if (commandParameters == null)
				commandParameters = new List<DBQueryParm>();
			var table = new DataTable();
			using (var command = new SqlCommand(commandSql, this.connection))
			{
				foreach (var parameter in commandParameters)
					command.Parameters.AddWithValue("@" + parameter.Name, parameter.Value);
				using (var da = new SqlDataAdapter(command))
				{
					da.Fill(table);
				}
				return table;
			}
		}
		
		public void Dispose()
		{
			if (connection != null)
				connection.Dispose();
		}
	}

	public class DBQueryParm
	{
		private string name;
		private object value;
		public DBQueryParm(string name, object value)
		{
			this.name = name;
			this.value = value;
		}
		public string Name
		{
			get { return this.name; }
		}
		public object Value
		{
			get { return this.value; }
		}
	}
}