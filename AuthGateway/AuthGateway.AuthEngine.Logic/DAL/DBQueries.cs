using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

using AuthGateway.Shared;
using AuthGateway.Shared.Tracking;
using AuthGateway.Shared.XmlMessages;

namespace AuthGateway.AuthEngine.Logic.DAL
{
	public class DBQueryParm
	{
		string name;
		object value;
		public DBQueryParm(string name, object value)
		{
			this.name = name;
			this.value = value;
		}
		public string Name
		{
			get { return name; }
		}
		public object Value
		{
			get { return value; }
		}
	}
	public class DBQueries : IDisposable
	{
		private SqlConnection connection;
		private Stack<SqlTransaction> transactions;

		public const string SMS_CONTACT = "SMS_CONTACT";
		public const string SETTINGS = "SETTINGS";
		public const string USER_SHEET = "UserSheet";
		public const string USER_SHEET_CODE = "UserSheetCode";
		public const string MESSAGE = "Message";
        public const string DOMAINS = "DOMAINS";
        public const string ALIASES = "ALIASES";
        public const string AUTH_IMAGE_THUMBNAILS = "AUTH_IMAGE_THUMBNAILS";
        public const string AUTH_IMAGE_CATEGORIES = "AUTH_IMAGE_CATEGORIES";
        public const string AUTH_IMAGES = "AUTH_IMAGES";
        public const string AE_HEARTBEAT = "AE_HEARTBEAT";
        public const string AE_COMMANDS = "AE_COMMANDS";
        public const string REPORTING = "AE_REPORTING";
        public const string USER_PROVIDERS = "UserProviders";
        public const string AE_POLL_FAILURES = "AE_POLL_FAILURES";
        public const string AE_KEY_TEST = "AE_KEY_TEST";

		public DBQueries(SqlConnection connection)
		{
			this.connection = connection;
			if (this.connection.State != ConnectionState.Open)
				this.connection.Open();
			this.transactions = new Stack<SqlTransaction>();
		}

		public DBQueries(string connectionString) : this(new SqlConnection(connectionString))
		{
			
		}

		public bool DiposeThis = true;

		private void UseTransactionIfNecessary(SqlCommand command) {
			if (this.transactions.Count >= 1)
			{
				command.Transaction = this.transactions.Peek();
			}
		}

		private void AddParamaters(SqlCommand command, List<DBQueryParm> commandParameters)
		{
			foreach (var parameter in commandParameters)
				command.Parameters.AddWithValue("@" + parameter.Name, parameter.Value);
		}

		public void BeginTransaction(IsolationLevel level)
		{
			if (this.transactions.Count == 0)
				this.transactions.Push(this.connection.BeginTransaction(level));
			else
				this.transactions.Push(this.transactions.Peek());
		}

		public void BeginTransaction()
		{
			if (this.transactions.Count == 0)
				this.transactions.Push(this.connection.BeginTransaction());
			else
				this.transactions.Push(this.transactions.Peek());
		}

		public void CommitTransaction()
		{
			if (this.transactions.Count == 0)
				return;
			var transaction = this.transactions.Pop();
			if (this.transactions.Count == 0)
				transaction.Commit();
		}

		public void RollbackTransaction()
		{
			if (this.transactions.Count == 0)
				return;
			var transaction = this.transactions.Pop();
			if (this.transactions.Count == 0)
				transaction.Rollback();
		}

		public DataTable Query(string commandSql, List<DBQueryParm> commandParameters)
		{
			DataTable dt = new DataTable();
			#if DEBUG && LOGQUERIES
			Logger.Instance.WriteToLog("Query: " + commandSql, LogLevel.Debug);
			#endif
            try {
                using (var command = new SqlCommand(commandSql, this.connection)) {
                    UseTransactionIfNecessary(command);
                    AddParamaters(command, commandParameters);
                    using (var da = new SqlDataAdapter(command)) {
                        da.Fill(dt);
                    }
                }
            }
            catch (Exception ex){
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
            }
			return dt;
		}

		public DataTable Query(string commandSql, params DBQueryParm[] commandParameters)
		{
			var parms = new List<DBQueryParm>();
			foreach(var parm in commandParameters)
				parms.Add(parm);
			return Query(commandSql, parms);
		}

		public DataTable Query(string commandSql)
		{
			return Query(commandSql, new List<DBQueryParm>());
		}

		public T QueryScalar<T>(string commandSql, List<DBQueryParm> commandParameters)
		{
			using (var command = new SqlCommand(commandSql, this.connection))
			{
				UseTransactionIfNecessary(command);
				AddParamaters(command, commandParameters);
				#if DEBUG && LOGQUERIES
				Logger.Instance.WriteToLog("QueryScalar: " + commandSql, LogLevel.Debug);
				#endif
				object result = command.ExecuteScalar();
				return (T)result;
			}
		}

		public T QueryScalar<T>(string commandSql, params DBQueryParm[] commandParameters)
		{
			var parms = new List<DBQueryParm>();
			foreach (var parm in commandParameters)
				parms.Add(parm);
			return QueryScalar<T>(commandSql, parms);
		}

		public T QueryScalar<T>(string commandSql)
		{
			return QueryScalar<T>(commandSql, new List<DBQueryParm>());
		}

		public T? QueryScalarNullable<T>(string commandSql, List<DBQueryParm> commandParameters) where T : struct
		{
			using (var command = new SqlCommand(commandSql, this.connection))
			{
				UseTransactionIfNecessary(command);
				AddParamaters(command, commandParameters);
				#if DEBUG && LOGQUERIES
				Logger.Instance.WriteToLog("QueryScalar: " + commandSql, LogLevel.Debug);
				#endif
				object result = command.ExecuteScalar();
				if (result == null)
					return new T?();
				return (T)result;
			}
		}

		public int NonQuery(string commandSql, List<DBQueryParm> commandParameters)
		{
			using (var command = new SqlCommand(commandSql, this.connection))
			{
				UseTransactionIfNecessary(command);
				AddParamaters(command, commandParameters);
				#if DEBUG && LOGQUERIES
				Logger.Instance.WriteToLog("NonQuery: " + commandSql, LogLevel.Debug);
				#endif
				return command.ExecuteNonQuery();
			}
		}

		public int NonQuery(string commandSql, params DBQueryParm[] commandParameters)
		{
			var parms = new List<DBQueryParm>();
			foreach (var parm in commandParameters)
				parms.Add(parm);
			return NonQuery(commandSql, parms);
		}

		public int NonQuery(string commandSql)
		{
			return this.NonQuery(commandSql, new List<DBQueryParm>());
		}

		public void Dispose()
		{
			if (!DiposeThis)
				return;
			//if (this.connection.State == ConnectionState.Open)
			//{
			//  //if (this.transactions.Count > 0)
			//  //{
			//  //  this.transactions.Peek().Rollback();
			//  //  throw new DALException("A current transaction was open, rollback.");
			//  //}
			//  //this.connection.Close();
			//}
			if (this.connection != null)
			{
				this.connection.Dispose();
				//Logger.Instance.WriteToLog("DBQUERIES DISPOSED", LogLevel.Debug);
			}
		}

		public string GetSetting(string key, string component = "")
		{
			if (string.IsNullOrEmpty(component)) {
				return QueryScalar<string>(@"SELECT [VALUE] FROM [SETTINGS] WHERE [SETTING]=@KEY", new DBQueryParm("KEY", key));
			}
			return QueryScalar<string>(@"SELECT [VALUE] FROM [SETTINGS] WHERE [SETTING]=@KEY AND [OBJECT]=@COMPONENT"
			                           , new DBQueryParm("KEY", key), new DBQueryParm("COMPONENT", component));
		}
		
		public int SetSetting(string key, string value, string component = "")
		{
			return NonQuery(@"
BEGIN TRAN
   UPDATE [SETTINGS] WITH (serializable) SET [VALUE] = @VALUE
   	WHERE [SETTING]=@KEY AND ((LEN(@COMPONENT) = 0) OR [OBJECT]=@COMPONENT)
   IF @@rowcount = 0
   BEGIN
      INSERT [SETTINGS] ([SETTING],[VALUE],[OBJECT]) VALUES (@KEY, @VALUE, @COMPONENT)
   END
COMMIT TRAN
"
			         , new DBQueryParm("KEY", key), new DBQueryParm("VALUE", value), new DBQueryParm("COMPONENT", component));
			
		}
		
		public long GetUserId(string user, string org)
		{
			var parms = new List<DBQueryParm>();
			parms.Add(new DBQueryParm("USER", user));
			parms.Add(new DBQueryParm("ON", org));
			long userId = this.QueryScalar<long>(
				"SELECT TOP 1 ID FROM "
				+ DBQueries.SMS_CONTACT
				+ " WHERE AD_USERNAME = @USER AND ORG_NAME=@ON ORDER BY ID ASC",
				parms
			);
			return userId;
		}

		static string getUserIdQuery = string.Format(
			@"SELECT TOP 1 ID FROM [{0}] WHERE AD_USERNAME = @USER AND ORG_NAME=@ON AND SID=@SID"
			, DBQueries.SMS_CONTACT);
		public long GetUserId(string user, string org, string sid)
		{
			var parms = new List<DBQueryParm>();
			parms.Add(new DBQueryParm("USER", user));
			parms.Add(new DBQueryParm("ON", org));
			parms.Add(new DBQueryParm("SID", sid));
			var userId = QueryScalarNullable<long>( getUserIdQuery, parms );
			if (!userId.HasValue)
				return 0;
			return userId.Value;
		}

		static string getUserDataQuery =string.Format(
			@"SELECT TOP 1 ID, SID, [userStatus], [uSNChanged] FROM [{0}] WHERE AD_USERNAME = @USER AND ORG_NAME=@ON"
			, DBQueries.SMS_CONTACT );
		public UserData GetUserData(string user, string org)
		{
			var parms = new List<DBQueryParm>();
			parms.Add(new DBQueryParm("USER", user));
			parms.Add(new DBQueryParm("ON", org));
			var idAndSid = Query( getUserDataQuery, parms );
			var ret = new UserData();            
			if (idAndSid.Rows.Count == 0)
				return ret;
			ret.Id = idAndSid.Rows[0].Field<long>(0);
			ret.Sid = idAndSid.Rows[0].Field<string>(1);
			ret.UserStatus = idAndSid.Rows[0].Field<byte>(2);
			ret.uSNChanged = idAndSid.Rows[0].Field<string>(3);
			return ret;
		}

        static string getLoginDataByMobileNumberQuery = string.Format(@"SELECT TOP 1 AD_USERNAME, ORG_NAME FROM [{0}] WHERE MOBILE_NUMBER = @MOBILE", DBQueries.SMS_CONTACT);
        public LoginData GetLoginDataByMobileNumber(string mobileNumber)
        {
            var parms = new List<DBQueryParm>();
            parms.Add(new DBQueryParm("MOBILE", mobileNumber));
            var userDomainPair = Query(getLoginDataByMobileNumberQuery, parms);
            var ret = new LoginData();
            if (userDomainPair.Rows.Count > 0)
            {
                ret.Username = userDomainPair.Rows[0].Field<string>(0);
                ret.DomainName = userDomainPair.Rows[0].Field<string>(1);
            }
            return ret;
        }

        static string getLoginDataByUPNQuery = string.Format(@"SELECT TOP 1 AD_USERNAME, ORG_NAME FROM [{0}] WHERE UPN = @UPN", DBQueries.SMS_CONTACT);
        public LoginData GetLoginDataByUPN(string upn)
        {
            var parms = new List<DBQueryParm>();
            parms.Add(new DBQueryParm("UPN", upn));
            var userDomainPair = Query(getLoginDataByUPNQuery, parms);
            var ret = new LoginData();
            if (userDomainPair.Rows.Count > 0)
            {                
                ret.Username = userDomainPair.Rows[0].Field<string>(0);
                ret.DomainName = userDomainPair.Rows[0].Field<string>(1);
            }
            return ret;
        }

        static string getLoginDataByEmailQuery = string.Format(@"SELECT TOP 1 AD_USERNAME, ORG_NAME FROM [{0}] WHERE email = @EMAIL", DBQueries.SMS_CONTACT);
        public LoginData GetLoginDataByEmail(string email)
        {
            var parms = new List<DBQueryParm>();
            parms.Add(new DBQueryParm("EMAIL", email));
            var userDomainPair = Query(getLoginDataByEmailQuery, parms);
            var ret = new LoginData();
            if (userDomainPair.Rows.Count > 0)
            {
                ret.Username = userDomainPair.Rows[0].Field<string>(0);
                ret.DomainName = userDomainPair.Rows[0].Field<string>(1);
            }
            return ret;
        }

        static string getMobileNumbersQuery = string.Format(@"SELECT MOBILE_NUMBER FROM [{0}]", DBQueries.SMS_CONTACT);
        public List<string> GetMobileNumbers()
        {
            var parms = new List<DBQueryParm>();            
            var mobileNumbers = Query(getMobileNumbersQuery, parms);
            var ret = new List<string>();

            foreach (DataRow row in mobileNumbers.Rows)           
            {
                ret.Add(row.Field<string>(0));                
            }
            return ret;
        }

        static string getUPNsQuery = string.Format(@"SELECT UPN FROM [{0}]", DBQueries.SMS_CONTACT);
        public List<string> GetUPNs()
        {
            var parms = new List<DBQueryParm>();
            var UPNs = Query(getUPNsQuery, parms);
            var ret = new List<string>();

            foreach (DataRow row in UPNs.Rows)
            {
                ret.Add(row.Field<string>(0));
            }
            return ret;
        }

        static string getDomainsQuery = string.Format(@"SELECT DOMAIN FROM [{0}]", DBQueries.DOMAINS);
        public List<string> GetDomains()
        {
            var parms = new List<DBQueryParm>();
            var domains = Query(getDomainsQuery, parms);
            var ret = new List<string>();

            foreach (DataRow row in domains.Rows)
            {
                ret.Add(row.Field<string>(0));
            }
            return ret;
        }

        static string getAliasesQuery = string.Format(@"SELECT ALIAS FROM [{0}] JOIN [{1}] ON [{1}].ID =[{0}].DOMAIN WHERE [{1}].DOMAIN = @DOMAIN_NAME", DBQueries.ALIASES, DBQueries.DOMAINS);
        public List<string> GetAliases(string domain)
        {
            var parms = new List<DBQueryParm>();
            parms.Add(new DBQueryParm("DOMAIN_NAME", domain));
            var aliases = Query(getAliasesQuery, parms);
            var ret = new List<string>();

            foreach (DataRow row in aliases.Rows) {
                ret.Add(row.Field<string>(0));
            }
            return ret;
        }

        static string addDomainQuery = string.Format(@"IF NOT EXISTS (SELECT * FROM [{0}] WHERE DOMAIN = @DOMAIN) INSERT INTO [{0}] (DOMAIN) OUTPUT INSERTED.ID VALUES (@DOMAIN)", DOMAINS);
        public int AddDomain(string domain)
        {            
            try {
                var parms = new List<DBQueryParm>();        
                parms.Add(new DBQueryParm("DOMAIN", domain));               
                DataTable dt = Query(addDomainQuery, parms);                
                return dt.Rows.Count;
            }
            catch(Exception ex) {
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
                return 0; 
            }
        }


        static string addAliasQuery = string.Format(@"INSERT INTO [{0}] (DOMAIN, ALIAS) VALUES ((SELECT {1}.ID FROM [{1}] WHERE {1}.DOMAIN = @DOMAIN), @ALIAS)", ALIASES, DOMAINS);
        public void AddAlias(string domain, string alias)
        {
            var parms = new List<DBQueryParm>();
            parms.Add(new DBQueryParm("DOMAIN", domain));
            parms.Add(new DBQueryParm("ALIAS", alias));

            Query(addAliasQuery, parms);
        }

        static string removeAliasQuery = string.Format(@"DELETE {0} FROM [{0}] JOIN [{1}] ON {0}.DOMAIN = {1}.ID WHERE {0}.ALIAS = @ALIAS AND {1}.DOMAIN = @DOMAIN", ALIASES, DOMAINS);
        public void RemoveAlias(string domain, string alias)
        {
            var parms = new List<DBQueryParm>();
            parms.Add(new DBQueryParm("DOMAIN", domain));
            parms.Add(new DBQueryParm("ALIAS", alias));

            Query(removeAliasQuery, parms);
        }

        static string setPanicStateQuery = string.Format(@"UPDATE [{0}] SET PANIC_ON = @PANIC WHERE ORG_NAME = @ORG_NAME AND AD_USERNAME = @USERNAME", SMS_CONTACT);
        public void SetPanicState(string user, string domain, bool panic)
        {
            var parms = new List<DBQueryParm>();
            parms.Add(new DBQueryParm("ORG_NAME", domain));
            parms.Add(new DBQueryParm("USERNAME", user));
            parms.Add(new DBQueryParm("PANIC", Convert.ToInt32(panic)));

            Query(setPanicStateQuery, parms);
        }

        static string getPanicStateQuery = string.Format(@"SELECT PANIC_ON FROM [{0}] WHERE AD_USERNAME = @USER AND ORG_NAME = @ORG_NAME", SMS_CONTACT);

        public bool GetPanicState(string user, string domain)
        {
            var parms = new List<DBQueryParm>();
            parms.Add(new DBQueryParm("ORG_NAME", domain));
            parms.Add(new DBQueryParm("USER", user));
            
            var state = Query(getPanicStateQuery, parms);
            if (state.Rows.Count == 0)
                throw new DALException(string.Format("User {0}\\{1} not found.", domain, user));
            return state.Rows[0].Field<bool>(0);
        }

        static string setLeftImageQuery = string.Format(@"UPDATE [{0}] SET LEFT_IMAGE = (SELECT ID FROM [{1}] WHERE [{1}].URL=@LEFT) WHERE ORG_NAME = @ORG_NAME AND AD_USERNAME = @USERNAME", SMS_CONTACT, AUTH_IMAGES);
        public void SetLeftImage(string user, string domain, string leftImage)
        {
            var parms = new List<DBQueryParm>();
            parms.Add(new DBQueryParm("ORG_NAME", domain));
            parms.Add(new DBQueryParm("USERNAME", user));
            parms.Add(new DBQueryParm("LEFT", leftImage));                        

            Query(setLeftImageQuery, parms);
        }

        static string setRightImageQuery = string.Format(@"UPDATE [{0}] SET RIGHT_IMAGE = (SELECT ID FROM [{1}] WHERE [{1}].URL=@RIGHT) WHERE ORG_NAME = @ORG_NAME AND AD_USERNAME = @USERNAME", SMS_CONTACT, AUTH_IMAGES);
        public void SetRightImage(string user, string domain, string rightImage)
        {
            var parms = new List<DBQueryParm>();
            parms.Add(new DBQueryParm("ORG_NAME", domain));
            parms.Add(new DBQueryParm("USERNAME", user));            
            parms.Add(new DBQueryParm("RIGHT", rightImage));

            Query(setRightImageQuery, parms);
        }

        static string getLeftImageQuery = string.Format(@"SELECT [{0}].THUMBNAIL, [{1}].ID FROM [{2}] INNER JOIN [{1}] ON [{2}].LEFT_IMAGE = [{1}].ID INNER JOIN [{0}] ON [{1}].ID = [{0}].IMAGE_ID WHERE [{2}].AD_USERNAME=@USERNAME AND [{2}].ORG_NAME=@ORG_NAME", AUTH_IMAGE_THUMBNAILS, AUTH_IMAGES, SMS_CONTACT);
        static string getRightImageQuery = string.Format(@"SELECT [{0}].THUMBNAIL, [{1}].ID FROM [{2}] INNER JOIN [{1}] ON [{2}].RIGHT_IMAGE = [{1}].ID INNER JOIN [{0}] ON [{1}].ID = [{0}].IMAGE_ID WHERE [{2}].AD_USERNAME=@USERNAME AND [{2}].ORG_NAME=@ORG_NAME", AUTH_IMAGE_THUMBNAILS, AUTH_IMAGES, SMS_CONTACT);

        public void GetImages(string user, string domain, out byte[] leftImageBytes, out byte[] rightImageBytes, out long leftImageId, out long rightImageId)
        {
            leftImageBytes = null;
            rightImageBytes = null;
            leftImageId = 0;
            rightImageId = 0;

            var parms = new List<DBQueryParm>();
            parms.Add(new DBQueryParm("ORG_NAME", domain));
            parms.Add(new DBQueryParm("USERNAME", user));            

            DataTable resultLeft = Query(getLeftImageQuery, parms);

            if (resultLeft.Rows.Count > 0) {
                leftImageBytes = resultLeft.Rows[0].Field<byte[]>(0);
                leftImageId = resultLeft.Rows[0].Field<long>(1);
            }

            DataTable resultRight = Query(getRightImageQuery, parms);

            if (resultRight.Rows.Count > 0) {
                rightImageBytes = resultRight.Rows[0].Field<byte[]>(0);
                rightImageId = resultRight.Rows[0].Field<long>(1);
            }
        }

        static string storeThumbnailQuery = string.Format(@"INSERT INTO [{0}] (
[IMAGE_ID]
,[THUMBNAIL]
) VALUES (
@ID
,@BUFFER
)", AUTH_IMAGE_THUMBNAILS);

        static string storeImageQuery = string.Format(@"IF NOT EXISTS (SELECT * FROM [{0}] WHERE URL = @URL)INSERT INTO [{0}] (URL, CATEGORY_ID) OUTPUT INSERTED.ID VALUES (@URL, (SELECT ID FROM [{1}] WHERE CATEGORY=@CATEGORY))", AUTH_IMAGES, AUTH_IMAGE_CATEGORIES);
        public void StoreImage(string url, string category, byte[] imageBytes)
        {
            var parmsMain = new List<DBQueryParm>();
            parmsMain.Add(new DBQueryParm("URL", url));
            parmsMain.Add(new DBQueryParm("CATEGORY", category));

            DataTable dt = Query(storeImageQuery, parmsMain);

            if (dt.Rows.Count > 0) {
                var parms = new List<DBQueryParm>();
                parms.Add(new DBQueryParm("ID", dt.Rows[0].Field<long>("ID")));
                parms.Add(new DBQueryParm("BUFFER", imageBytes));

                Query(storeThumbnailQuery, parms);
            }
        }

        static string getThumbnailQuery = string.Format(@"SELECT THUMBNAIL FROM [{0}] WHERE IMAGE_ID = (SELECT ID FROM [{1}] WHERE [{1}].URL=@URL) ", AUTH_IMAGE_THUMBNAILS, AUTH_IMAGES);

        public byte[] GetImage(string url)
        {
            var parms = new List<DBQueryParm>();
            parms.Add(new DBQueryParm("URL", url));            

            var result = Query(getThumbnailQuery, parms);

            if (result.Rows.Count == 0)
                throw new DALException(string.Format("Image not found."));
            return result.Rows[0].Field<byte[]>(0);
        }

        static string getImageCategoriesQuery = string.Format(@"SELECT CATEGORY FROM [{0}]", AUTH_IMAGE_CATEGORIES);
        public string[] GetImageCategories()
        {
            var parms = new List<DBQueryParm>();            
            var result = Query(getImageCategoriesQuery, parms);
            List<string> categories = new List<string>();
            foreach (DataRow row in result.Rows) {
                categories.Add(row.Field<string>(0));
            }

            return categories.ToArray();
            
        }

        static string getImageUrlsByCategoryQuery = string.Format(@"SELECT [{0}].URL FROM [{0}] INNER JOIN [{1}] ON [{0}].CATEGORY_ID = [{1}].ID WHERE ([{1}].CATEGORY = @CATEGORY)", AUTH_IMAGES, AUTH_IMAGE_CATEGORIES);
        public string[] GetImageUrlsByCategory(string category)
        {
            var parms = new List<DBQueryParm>();  
            parms.Add(new DBQueryParm("CATEGORY", category));            
            var result = Query(getImageUrlsByCategoryQuery, parms);
            List<string> urls = new List<string>();
            foreach (DataRow row in result.Rows) {
                urls.Add(row.Field<string>(0));
            }

            return urls.ToArray();
        }

        static string heartbeatQuery = string.Format(@"INSERT INTO [{0}] VALUES (GETDATE(), @INSTANCE, @PREFERENCE, @IS_MASTER, @IMAGES_PREFERENCE, @IS_IMAGES_MASTER)", AE_HEARTBEAT);
        public void WriteHeartbeat(string instanceName, int preference, bool isMaster, int imagesPreference, bool isImagesMaster)
        {
            var parms = new List<DBQueryParm>();
            parms.Add(new DBQueryParm("INSTANCE", instanceName));
            parms.Add(new DBQueryParm("PREFERENCE", preference));
            parms.Add(new DBQueryParm("IS_MASTER", isMaster));
            parms.Add(new DBQueryParm("IMAGES_PREFERENCE", imagesPreference));
            parms.Add(new DBQueryParm("IS_IMAGES_MASTER", isImagesMaster));

            Query(heartbeatQuery, parms);
        }

        static string deleteHeartbeatsQuery = string.Format(@"DELETE FROM [{0}] WHERE DATETIME < DATEADD(second, -@SECONDS, GETDATE())", AE_HEARTBEAT);
        public void DeleteOldHeartbeats(int seconds)
        {            
            var parms = new List<DBQueryParm>();
            parms.Add(new DBQueryParm("SECONDS", seconds));
            Query(deleteHeartbeatsQuery, parms);
        }

        static string getAliveServersQuery = string.Format(@"SELECT TOP 1 WITH TIES
    T.AE_INSTANCE, T.PREFERENCE, T.POLLING_MASTER, T.IMAGES_PREFERENCE, T.IMAGES_POLLING_MASTER, T.DATETIME
FROM [{0}] AS T
ORDER BY ROW_NUMBER() OVER (PARTITION BY T.AE_INSTANCE ORDER BY T.DATETIME DESC)", AE_HEARTBEAT);

        public DataTable GetAliveServers()
        {
            return Query(getAliveServersQuery, new List<DBQueryParm>());
        }

        static string commandQuery = string.Format("INSERT INTO [{0}] VALUES (GETDATE(), @INSTANCE, @COMMAND)", AE_COMMANDS);
        public void CommandUsersPollingPreference(string instance, int preference)
        {
            var parms = new List<DBQueryParm>();
            parms.Add(new DBQueryParm("INSTANCE", instance));
            string command = string.Format("PREFERENCE {0}", preference);
            parms.Add(new DBQueryParm("COMMAND", command));
            Query(commandQuery, parms);
        }

        public void CommandImagesPollingPreference(string instance, int preference)
        {
            var parms = new List<DBQueryParm>();
            parms.Add(new DBQueryParm("INSTANCE", instance));
            string command = string.Format("IMAGES_PREFERENCE {0}", preference);
            parms.Add(new DBQueryParm("COMMAND", command));
            Query(commandQuery, parms);
        }

        public void CommandMasterStatus(string instance, bool masterStatus)
        {
            var parms = new List<DBQueryParm>();
            parms.Add(new DBQueryParm("INSTANCE", instance));
            string command = string.Format("MASTER {0}", masterStatus);
            parms.Add(new DBQueryParm("COMMAND", command));
            Query(commandQuery, parms);
        }

        public void CommandImagesMasterStatus(string instance, bool masterStatus)
        {
            var parms = new List<DBQueryParm>();
            parms.Add(new DBQueryParm("INSTANCE", instance));
            string command = string.Format("IMAGES_MASTER {0}", masterStatus);
            parms.Add(new DBQueryParm("COMMAND", command));
            Query(commandQuery, parms);
        }

        static string getMasterServerQuery = string.Format(@"SELECT SERVERS.AE_INSTANCE FROM (SELECT TOP 1 WITH TIES
    T.AE_INSTANCE, T.POLLING_MASTER
FROM [{0}] AS T
ORDER BY ROW_NUMBER() OVER (PARTITION BY T.AE_INSTANCE ORDER BY T.DATETIME DESC) ) SERVERS
WHERE SERVERS.POLLING_MASTER = 'True'", AE_HEARTBEAT);

        public void CommandPoll()
        {
            DataTable dt = Query(getMasterServerQuery, new List<DBQueryParm>());
            if (dt.Rows.Count == 0)
                throw new Exception("Master polling server not found.");

            string instance = dt.Rows[0].Field<string>(0);

            var parms = new List<DBQueryParm>();
            parms.Add(new DBQueryParm("INSTANCE", instance));            
            parms.Add(new DBQueryParm("COMMAND", "POLL"));
            Query(commandQuery, parms);
        }

        static string getCommandsQuery = string.Format(@"SELECT [ID], [COMMAND]  FROM [{0}] WHERE [RECIPIENT_SERVER] = @INSTANCE ORDER BY [DATETIME]", AE_COMMANDS);
        static string deleteCommandQuery = string.Format(@"DELETE FROM [{0}] WHERE ID = @ID", AE_COMMANDS);

        public List<string> FetchCommands(string instance)
        {
            List<string> commands = new List<string>();

            var parms = new List<DBQueryParm>();
            parms.Add(new DBQueryParm("INSTANCE", instance));
            DataTable dt = Query(getCommandsQuery, parms);

            foreach (DataRow row in dt.Rows) {
                long id = row.Field<long>(0);
                string command = row.Field<string>(1);
                commands.Add(command);
                parms = new List<DBQueryParm>();
                parms.Add(new DBQueryParm("ID", id));
                Query(deleteCommandQuery, parms);
            }

            return commands;
        }

        static string storeReportTimeQuery = string.Format(@"INSERT INTO [{0}] (DATETIME, AE_INSTANCE) VALUES (GETDATE(), @AE_INSTANCE)", REPORTING);
        public void StoreReportTime(string aeInstance)
        {
            var parms = new List<DBQueryParm>();
            parms.Add(new DBQueryParm("AE_INSTANCE", aeInstance));                                    
            Query(storeReportTimeQuery, parms);
        }

        static string getLastReportTimeQuery = string.Format(@"SELECT MAX(DATETIME) AS DATETIME FROM [{0}] WHERE AE_INSTANCE = @AE_INSTANCE", REPORTING);
        public DateTime GetLastReportTime(string aeInstance)
        {
            var parms = new List<DBQueryParm>();
            parms.Add(new DBQueryParm("AE_INSTANCE", aeInstance));
            DataTable dt = Query(getLastReportTimeQuery, parms);

            var lastDatetime = DateTime.MaxValue;
            if (dt.Rows.Count > 0) {
            	var datetime = dt.Rows[0].Field<DateTime?>("DATETIME");
            	if (datetime.HasValue)
            		lastDatetime = datetime.Value;
            }

           return lastDatetime;
        }

        static string getOATHConfigsQuery = string.Format(@"SELECT [id], [CONFIG] FROM [{0}] WHERE authProviderId = 2", USER_PROVIDERS);
        static string updateProviderConfig = string.Format(@"UPDATE [{0}] SET config = @CONFIG WHERE ID = @ID", USER_PROVIDERS);
        public void ApplyOATHCalcDefaults(string defaultConfig)
        {
            var parms = new List<DBQueryParm>();            
            DataTable dt = Query(getOATHConfigsQuery, parms);

            foreach (DataRow row in dt.Rows) {
                string currentConfig = row.Field<string>("config");
                string[] deviceConfigs = currentConfig.Split(new string[]{"|%|"}, StringSplitOptions.RemoveEmptyEntries);

                List<string> newConfig = new List<string>();
                foreach (string config in deviceConfigs) {
                    string[] configParams = config.Split(',');
                    string deviceName = configParams[3];

                    string[] newConfigParams = defaultConfig.Split(',');
                    newConfigParams[3] = deviceName;
                    string newDeviceConfig = string.Join(",", newConfigParams);
                    newConfig.Add(newDeviceConfig);                    
                }

                string updatedConfig = string.Join("|%|", newConfig);

                var parms2 = new List<DBQueryParm>();
                    parms2.Add(new DBQueryParm("CONFIG", updatedConfig));
                    parms2.Add(new DBQueryParm("ID", row.Field<int>("id")));
                    Query(updateProviderConfig, parms2);
            }
        }

        static string storePollFailureTimeQuery = string.Format(@"DELETE FROM [{0}] INSERT INTO [{0}] (DATETIME) VALUES (GETDATE())", AE_POLL_FAILURES);
        public void StorePollFailureTime()
        {
            var parms = new List<DBQueryParm>();
            Query(storePollFailureTimeQuery, parms);

        }

        static string getLastPollFailureTimeQuery = string.Format(@"SELECT MAX(DATETIME) AS DATETIME FROM [{0}]", AE_POLL_FAILURES);

        public DateTime GetLastPollFailureTime()
        {
            DateTime datetime = DateTime.MinValue;

            var parms = new List<DBQueryParm>();
            DataTable dt = Query(getLastPollFailureTimeQuery, parms);

            if (dt.Rows.Count > 0) {
                datetime = dt.Rows[0].Field<DateTime>("DATETIME");
            }

            return datetime;
        }

        static string storeEncryptionExampleQuery = string.Format(@"DELETE FROM [{0}] INSERT INTO [{0}] (ENCRYPTED) VALUES (@ENCRYPTED)", AE_KEY_TEST);
        public void StoreEncryptionExample(string encrypted)
        {
            var parms = new List<DBQueryParm>();
            parms.Add(new DBQueryParm("ENCRYPTED", encrypted));
            DataTable dt = Query(storeEncryptionExampleQuery, parms);
        }

        static string getEncryptionExampleQuery = string.Format(@"SELECT TOP 1 [ENCRYPTED] FROM [{0}]", AE_KEY_TEST);

        public string GetEncryptionExample()
        {
            string encrypted = string.Empty;

            var parms = new List<DBQueryParm>();
            DataTable dt = Query(getEncryptionExampleQuery, parms);

            if (dt.Rows.Count > 0) {
                encrypted = dt.Rows[0].Field<string>("ENCRYPTED");
            }

            return encrypted;
        }


        static string getNotEncryptedQuery = string.Format(@"SELECT [ID], [PINCODE], [PANIC_PINCODE] FROM [{0}]  WHERE PINCODE <> '' AND (NOT PINCODE LIKE '{1}%' OR NOT PANIC_PINCODE LIKE 'AESencrypted:%')", SMS_CONTACT, AESEncryption.EncryptionMark);
        static string updatePincodesQuery = string.Format(@"UPDATE [{0}] SET PINCODE = @PIN, PANIC_PINCODE = @PANIC WHERE ID = @ID", SMS_CONTACT);

        public void CheckEncryption(AESEncryption encryption)
        {            
            DataTable dt = Query(getNotEncryptedQuery, new List<DBQueryParm>());
            
            for (int i = 0; i < dt.Rows.Count; i++){
                string pincode = dt.Rows[i].Field<string>("PINCODE");
                string panicPincode = dt.Rows[i].Field<string>("PANIC_PINCODE");
                string encPincode = pincode;
                string encPanicPincode = panicPincode;                 

                if (!pincode.StartsWith(AESEncryption.EncryptionMark)){
                    encPincode = encryption.Encrypt(pincode);
                }

                if (!panicPincode.StartsWith(AESEncryption.EncryptionMark)){
                    encPanicPincode = encryption.Encrypt(panicPincode);
                }

                var parms = new List<DBQueryParm>();
                parms.Add(new DBQueryParm("ID", dt.Rows[i].Field<long>("ID")));
                parms.Add(new DBQueryParm("PIN", encPincode));
                parms.Add(new DBQueryParm("PANIC", encPanicPincode));
                Query(updatePincodesQuery, parms);
            }
            
        }

		public TemplateMessage GetTemplateMessage(string label) {
			const string getTemplateMessage = @"
			SELECT [label], [text], [replacement], [order], [title]
			FROM [{0}] WHERE [label] = @LABEL
";
			var tmRow = Query(string.Format(getTemplateMessage, DBQueries.MESSAGE), new DBQueryParm(@"LABEL", label));
			if (tmRow.Rows.Count > 1)
				throw new DALException("Invalid amount of expected rows.");
			var tm = new TemplateMessage {
				Label = Convert.ToString(tmRow.Rows[0]["label"]),
				Title = Convert.ToString(tmRow.Rows[0]["title"]),
				Message = Convert.ToString(tmRow.Rows[0]["text"]),
				Legend = Convert.ToString(tmRow.Rows[0]["replacement"]),
				Order = Convert.ToInt32(tmRow.Rows[0]["order"])
			};
			if (string.IsNullOrWhiteSpace(tm.Title))
				tm.Title = "WrightCCS - Token";
			return tm;
		}
		
		public class UserData
		{
			public long Id {get;set;}
			public string Sid {get;set;}
			public byte UserStatus {get;set;}
			public string uSNChanged { get; set; }
		}

        public class LoginData
        {
            public string Username { get; set; }
            public string DomainName { get; set; }

            public bool IsEmpty()
            {
                return String.IsNullOrEmpty(Username) && String.IsNullOrEmpty(DomainName);
            }
        }

	}
}
