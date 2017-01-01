using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AuthGateway.Shared
{
	public class Registry
	{
		private Dictionary<Type, object> items = new Dictionary<Type, object>();

		public void AddOrSet<T>(object obj)
		{
			var t = typeof(T);
			if (!items.ContainsKey(t))
				items.Add(t, obj);
			else
				items[t] = obj;
		}

		public void AddIfNotSet<T>(object obj)
		{
			var t = typeof(T);
			if (!items.ContainsKey(t))
				items.Add(t, obj);
		}

		public void Remove<T>()
		{
			items.Remove(typeof(T));
		}

		public T Get<T>()
		{
			var t = typeof(T);
			if (!items.ContainsKey(t))
				return default(T);
			return (T)items[t];
		}
	}
}
