using System;
using System.Collections.Generic;

namespace AuthGateway.Shared.XmlMessages
{
		public class ActionerInstance
		{
			private Dictionary<Type, CommandAction> actions;

			public delegate RetBase CommandAction(CommandBase command);

			public ActionerInstance()
			{
				this.actions = new Dictionary<Type, CommandAction>();
			}

			public void Add<T>(CommandAction action)
			{
				if (!actions.ContainsKey(typeof(T)))
					actions.Add(typeof(T), action);
			}

			public void Clear()
			{
				actions.Clear();
			}

			public RetBase Do(CommandBase cmd)
			{
				Type type = cmd.GetType();
				if (actions.ContainsKey(type))
					return actions[type](cmd);
				throw new ArgumentException("Invalid CommandAction");
			}
		}
}
