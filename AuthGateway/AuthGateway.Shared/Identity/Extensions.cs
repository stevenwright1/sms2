using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Principal;

namespace AuthGateway.Shared.Identity
{
    public static class Extensions
    {
        public static string GetDomain(string s)
        {
            if (string.IsNullOrEmpty(s))
                return s;
            int stop = s.IndexOf("\\");
            if (stop < 0)
                return string.Empty;
            return (stop > -1) ? s.Substring(0, stop) : string.Empty;
        }
        public static string GetDomain(this IIdentity identity)
        {
            return GetDomain(identity.Name);
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
        public static string GetLogin(this IIdentity identity)
        {
            return GetLogin(identity.Name);
        }
    }
}
