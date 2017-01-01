using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AuthGateway.Shared;
using AuthGateway.Setup.SQLDB;
using AuthGateway.Shared.Identity;

namespace AuthGateway.Wix.ServiceConfig
{
	class DbHandler
	{
		private SystemConfiguration sc;
		private CASession session;

		public DbHandler(SystemConfiguration sc, CASession session)
		{
			this.sc = sc;
			this.session = session;
		}

		internal void Execute()
		{
			ImpersonatedUser impersonatedUser = null;
			Impersonation impersonation = null;
			if (sc.DbUseIntegratedSecurity)
			{
				var parser = new IdentityParser();
				var username = session.Data["AeUsername"];
				var password = session.Data["AePassword"];
				var domainUsername = parser.GetDomainUserNameOrNull(username);
				try
				{
					impersonatedUser = new ImpersonatedUser(domainUsername.Username, domainUsername.Domain, password);
				}
				catch (NotImpersonableException)
				{
					if (impersonatedUser != null)
					{
						impersonatedUser.Dispose();
						impersonatedUser = null;
					}
					var builtinUser = ImpersonatedUser.GetBuiltIn(domainUsername.Username, domainUsername.Domain);
					if (builtinUser != BuiltinUser.None)
						impersonation = new Impersonation(builtinUser);
				}
				catch
				{
					throw new Exception(string.Format("Error impersonating user '{0}' domain '{1}'", domainUsername.Username, domainUsername.Domain));
				}
			}
			try
			{
				var cb = new DatabaseHandler();
				var ds = cb.DoStatus(sc.GetSQLConnectionString(false), sc.GetSQLConnectionString(true), sc.DatabaseName);
				var create = false;
				var upgrade = true;
				if (!ds.Exists)
				{
					session.Log("Database does not exist, it will be created.");
					create = true;
				}
				else
				{
					if (ds.Empty)
					{
						session.Log("Database is empty, schema will be generated.");
					}
					else if (ds.Version != cb.getLastPath())
					{
						if (!string.IsNullOrEmpty(ds.Version))
						{
							session.Log("Database version: " + ds.Version + "\r\nThe schema will be upgraded to " + cb.getLastPath());
						}
						else
						{
							session.Log("Unknown database status.");
							upgrade = false;
						}
					}
					else
					{
						session.Log("Database version: " + ds.Version + "\r\nNo action needed on database.");
						upgrade = false;
					}
				}
				if (create)
					cb.DoCreate(sc.GetSQLConnectionString(false), sc.DatabaseName);
				if (upgrade)
					cb.DoSchema(sc.GetSQLConnectionString(true), sc.DatabaseName, ds.Version);
			}
			finally
			{
				if (impersonatedUser != null)
				{
					impersonatedUser.Dispose();
					impersonatedUser = null;
				}
				if (impersonation != null)
				{
					impersonation.Dispose();
					impersonation = null;
				}
			}
		}
	}
}
