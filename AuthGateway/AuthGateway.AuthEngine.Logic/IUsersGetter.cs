using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using AuthGateway.Shared;
using AuthGateway.Shared.XmlMessages;
using AuthGateway.Shared.XmlMessages.Request.Command.AuthEngine;

namespace AuthGateway.AuthEngine.Logic
{
	public class UserGettersConfig
	{
		public UserGettersConfig(SystemConfiguration configuration, ConcurrentQueue<string> online, ConcurrentQueue<string> offline)
		{
			this.Configuration = configuration;
			this.OnlineControllers = online;
			this.OfflineControllers = offline;
		}

		public SystemConfiguration Configuration { get; private set; }
		public ConcurrentQueue<string> OnlineControllers { get; private set; }
		public ConcurrentQueue<string> OfflineControllers { get; private set; }
	}
	public interface IUsersGetter
	{
        void GetDomainAndReplacements(UserGettersConfig config, out string domain, AdReplacements replacements);
		void GetUsers(UserGettersConfig config, Func<AddFullDetails, RetBase> action, IUsersGetterResults results = null);
	}

	public interface IUsersGetterResults
	{
		int Total { get; set; }
		int Processed { get; set; }
		int Valid { get; set; }
		int SkippedNullSid { get; set; }
		int SkippedInvalidUserAccountControl { get; set; }
	}

	public class UsersGetterResults : IUsersGetterResults
	{
		public int Total { get; set; }
		public int Processed { get; set; }
		public int Valid { get; set; }
		public int SkippedNullSid { get; set; }
		public int SkippedInvalidUserAccountControl { get; set; }
	}
}
