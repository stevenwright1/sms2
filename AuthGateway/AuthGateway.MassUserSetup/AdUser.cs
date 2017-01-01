using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AuthGateway.MassUserSetup
{
	public abstract class SMSUser
	{
		public string Username { get; set; }
		public string Fullname { get; set; }
		public string Email { get; set; }
		public string Phone { get; set; }
		public bool Enabled { get; set; }
		public abstract string OrgType { get; }
	}

	public class AdUser : SMSUser
	{
		public override string OrgType
		{
			get { return "DOMAIN"; }
		}
	}

	public class LocalUser : SMSUser
	{
		public override string OrgType
		{
			get { return "WORKGROUP"; }
		}
	}
}
