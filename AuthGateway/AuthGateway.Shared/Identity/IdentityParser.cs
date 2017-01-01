using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AuthGateway.Shared.Identity
{
	public class IdentityParser
	{
		private List<Regex> regexes = new List<Regex>();
        private Regex mobileNumberRegex = new Regex(@"^\d+$");

		public IdentityParser(List<string> patterns)
		{
			this.SetPatterns(patterns);
		}
		public IdentityParser()
			: this(
				new List<string>() { 
				@"^(?<domain>.+)\\(?<user>.+)$",
				@"^(?<user>.+)@(?<domain>[^\.]+).*$",
			})
		{

		}

		private void SetPatterns(List<string> patterns)
		{
			regexes.Clear();
			foreach (var pattern in patterns)
			{
				regexes.Add(new Regex(pattern));
			}
		}

		public DomainUsername GetDomainUserNameOrNull(string user)
		{
			foreach (var re in regexes)
			{
				var match = re.Match(user);
				if (!match.Success || match.Groups.Count != 3)
					continue;
				var domainGroup = match.Groups["domain"];
				var userGroup = match.Groups["user"];
				return new DomainUsername(domainGroup.Value, userGroup.Value);
			}
			return null;
		}

        public string GetMobileNumberOrNull(string input)
        {            
            var match = mobileNumberRegex.Match(input);

            return match.Success ? input : null;
        }
	}
}
