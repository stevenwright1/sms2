using System.Collections.Generic;

namespace AuthGateway.AuthEngine.Logic
{
    public class AdReplacements : IEnumerable<KeyValuePair<string, string>>
	{
		private Dictionary<string, string> replacements;
		private object lockObj = new object();
		public AdReplacements()
		{
			replacements = new Dictionary<string, string>();
		}

		public void Add(string domain, string replacement)
		{
			var domainLower = domain.ToLowerInvariant();
			if (!replacements.ContainsKey(domainLower))
			{
				lock (lockObj)
				{
					if (!replacements.ContainsKey(domainLower))
						replacements.Add(domainLower, replacement);
				}
			}
		}

		public string ReplacementFor(string domain)
		{
			var domainLower = domain.ToLowerInvariant();
			if (!replacements.ContainsKey(domainLower))
				return domain;
			return replacements[domainLower];
		}

		public int Count { get { return replacements.Count; } }

		internal void Remove(string from)
		{
			from = from.ToLowerInvariant();
			if (replacements.ContainsKey(from))
			{
				lock (lockObj)
				{
					if (replacements.ContainsKey(from))
						replacements.Remove(from);
				}
			}
		}

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return replacements.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
	}
}
