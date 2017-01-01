using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using AuthGateway.Shared.Identity;

namespace AuthGateway.AuthEngine.Logic.Helpers
{
	public class DomainUsernameNotFoundException : ServerLogicException
	{
		public DomainUsernameNotFoundException()
			: base(@"No domain\username found.")
		{

		}

		public DomainUsernameNotFoundException(string username)
			: base(string.Format(@"No domain\username found in '{0}'.",username))
		{

		}
	}

	public class DefaultDomainNotSetException : ServerLogicException
	{
		public DefaultDomainNotSetException()
			: base(@"No default domain set.")
		{

		}

		public DefaultDomainNotSetException(string username)
			: base(string.Format(@"No default domain set for '{0}'.", username))
		{

		}
	}

	public class IdentityHelper : IdentityParser
	{
		public IdentityHelper(List<string> patterns) 
			: base(patterns)
		{
		}
		public IdentityHelper()
			: base()
		{
		}

		public DomainUsername GetDomainUsername(string user)
		{
			var du = GetDomainUserNameOrNull(user);
			if (du == null)
				throw new DomainUsernameNotFoundException(user);
			return du;
		}

		public DomainUsername GetElseDefaultDomainUsername(string defaultDomain, string user)
		{
			var du = GetDomainUserNameOrNull(user);
			if (du == null)
			{
				if (string.IsNullOrEmpty(defaultDomain))
					throw new DefaultDomainNotSetException(user);
				du = new DomainUsername(defaultDomain, user);
			}
			return du;
		}

		public static string GetDomain(string s)
		{
			if (string.IsNullOrEmpty(s))
				return s;
			int stop = s.IndexOf("\\");
			if (stop < 0)
				return string.Empty;
			return (stop > -1) ? s.Substring(0, stop) : string.Empty;
		}
		public static string GetLogin(string s)
		{
			if (string.IsNullOrEmpty(s))
				return s;
			int stop = s.IndexOf("\\");
			if (stop < 0)
				return s;
			return (stop > -1) ? s.Substring(stop + 1, s.Length - stop - 1) : s;
		}
	}
}
