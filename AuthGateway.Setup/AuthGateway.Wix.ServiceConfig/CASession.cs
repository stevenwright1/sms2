using System;
using System.Collections.Generic;
using Microsoft.Deployment.WindowsInstaller;

namespace AuthGateway.Wix.ServiceConfig
{
	public abstract class CASession
	{
		abstract public string this[string property] { get; set; }

		abstract public IDictionary<string, string> Data { get; }

		public virtual void Log(string msg)
		{
			Console.WriteLine(msg);
		}

		public virtual void Log(string format, params object[] args)
		{
			Console.WriteLine(format, args);
		}

		public virtual void Message(string message)
		{
			Console.WriteLine(message);
		}

		public virtual void MessageBox(string message)
		{
			System.Windows.Forms.MessageBox.Show(message);
		}
	}

	public class CASessionTest : CASession
	{
		private Dictionary<string, string> data = new Dictionary<string, string>();
		private Dictionary<string, string> cadata = new Dictionary<string, string>();

		public override string this[string property]
		{
			get
			{
				if (property.StartsWith("%"))
					return Environment.GetEnvironmentVariable(property.Substring(1));
				if (!data.ContainsKey(property))
					return null;
				return data[property];
			}
			set
			{
				if (data.ContainsKey(property))
					data[property] = value;
				else
					data.Add(property, value);
			}
		}

		public override IDictionary<string, string> Data
		{
			get { return this.cadata; }
		}
	}

	public class CASessionAdapter : CASession
	{
		protected Session session;

		public CASessionAdapter(Session session)
		{
			this.session = session;
		}

		public override string this[string property]
		{
			get
			{
				return session[property];
			}
			set
			{
				session[property] = value;
			}
		}

		public override IDictionary<string, string> Data
		{
			get { return this.session.CustomActionData; }
		}

		public override void Log(string msg)
		{
			this.session.Log(msg);
		}

		public override void Log(string format, params object[] args)
		{
			this.session.Log(format, args);
		}

		public override void Message(string message)
		{
			Record record = new Record(0);
			record[0] = message;
			this.session.Message(InstallMessage.Info, record);
		}

		public override void MessageBox(string message)
		{
			Record record = new Record(0);
			record[0] = message;
			this.session.Message(InstallMessage.Warning, record);
		}
	}
}
