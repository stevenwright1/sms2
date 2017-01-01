using System;

namespace AuthGateway.AuthEngine.Logic.DAL
{
	class DALException : Exception
	{
		public DALException(string message) : base(message)
		{

		}

		public DALException()
			: base("An error occurred when communicating with the database.")
		{

		}
	}
}
