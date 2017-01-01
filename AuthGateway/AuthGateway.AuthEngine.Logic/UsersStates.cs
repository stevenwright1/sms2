using System;
using System.Collections.Generic;
using AuthGateway.Shared.Log;

namespace AuthGateway.AuthEngine.Logic
{
	public class UserStateDetail
	{
		public UserStateDetail()
		{
			this.Date = DateTime.UtcNow;
			this.PinCodeValidated = false;
			this.SetInfo = false;
			this.SkipCheck = false;
		}

		public DateTime Date { get; set; }
		public bool PinCodeValidated { get; set; }
		public bool Panic { get; set; }
		public bool SetInfo { get; set; }
		public bool SkipCheck { get; set; }

		public bool Expired()
		{
			return (DateTime.UtcNow - this.Date).TotalSeconds < 300;
		}
	}

	public class UserState
	{
		private object lockobj = new object();
		private Dictionary<string, UserStateDetail> list;

		public UserState()
		{
			list = new Dictionary<string, UserStateDetail>();
		}

		public bool hasState(string state)
		{
			return list.ContainsKey(state);
		}

		public void remove(string state)
		{
			lock (lockobj)
			{
				if (this.hasState(state))
					this.list.Remove(state);
			}
		} 

		public UserStateDetail add(string state)
		{
			lock (lockobj)
			{
				if (!this.hasState(state))
					this.list.Add(state, new UserStateDetail());
				else
					throw new Exception("State already exists.");
			}
			return this.list[state];
		}

		public UserStateDetail getByState(string state)
		{
			if (!list.ContainsKey(state))
				throw new Exception("State not found.");
			return list[state];
		}
	}

	public class UsersStates
	{
		private static UsersStates _instance = new UsersStates();

		private object lockobj = new object();
		private Dictionary<string, UserState> list;

		private UsersStates() {
			this.list = new Dictionary<string, UserState>();
		}

		public static UsersStates Instance
		{
			get { 
				return _instance; 
			}
		}

		public bool hasUsername(string username)
		{
			return this.list.ContainsKey(username);
		}

		public bool hasUsernameAndState(string username, string state)
		{
			return this.hasUsername(username) && list[username].hasState(state);
		}

		private UserState addGet(string username)
		{
			lock (lockobj)
			{
				if (!this.hasUsername(username))
					this.list.Add(username, new UserState());
			}
			return this.list[username];
		}

		public UserStateDetail addUsernameAndState(string username, string state)
		{
			Logger.Instance.WriteToLog(string.Format("addUsernameAndState Username: '{0}' State: '{1}'", username, state), LogLevel.Debug);
			return this.addGet(username).add(state);
		}

		public void remove(string username)
		{
			lock (lockobj)
			{
				if (this.hasUsername(username))
					this.list.Remove(username);
			}
		} 

		public void removeUsernameAndState(string username, string state)
		{
			lock (lockobj)
			{
				if (this.hasUsername(username))
					this.list[username].remove(state);
				this.remove(username);
			}
		}

		public UserStateDetail getByUsernameAndState(string username, string state)
		{
			Logger.Instance.WriteToLog(string.Format("getByUsernameAndState Username: '{0}' State: '{1}'", username, state), LogLevel.Debug);
			if (!this.hasUsernameAndState(username, state))
				throw new LogicException(string.Format("Username '{0}' and state '{1}' not found.",username,state));
			return this.list[username].getByState(state);
		}
	}
}
