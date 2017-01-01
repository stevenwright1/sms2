using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AuthGateway.AuthEngine.Logic
{
	public class ServerAccess
	{
		Server server;
		public ServerAccess()
		{
			server = new Server();
		}

		public void StartService(object data)
		{
			server.StartService(data);
		}

		public bool getUsers()
		{
			return server.getUsers();
		}

		public void StopService()
		{
			server.StopService();
		}

		public bool Listening { get { return (server == null) ? false : server.Listening; } }
		public bool GettingUsers { get { return (server == null) ? false : server.GettingUsers; } }
	}
}
