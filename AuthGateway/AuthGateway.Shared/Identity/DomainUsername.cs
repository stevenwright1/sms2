using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AuthGateway.Shared.Identity
{
	public class DomainUsername
	{
		public DomainUsername(string domain, string username)
		{
			this.Domain = domain;
			this.Username = username;
		}
		public string Domain
		{
			get;
			private set;
		}
		public string Username
		{
			get;
			private set;
		}
		public string GetDomainUsername()
		{
			return this.Domain + "\\" + this.Username;
		}

        public static DomainUsername FromDomainUsername(string domainUsername)
        {
            if (domainUsername.IndexOf('\\') == -1)
                throw new Exception("Unexpected format of a domain username: " + domainUsername);
            string[] du = domainUsername.Split('\\');
            return new DomainUsername(du[0], du[1]);
        }
	}
}
