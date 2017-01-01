using System;
namespace AuthGateway.AuthEngine.Logic
{
	public abstract class BaseAuthEngineException : Exception 
	{
		protected BaseAuthEngineException(string message, Exception innerException) : base(message, innerException) {
			
		}
	}
	
	public class InvalidSerialException : Exception
	{
		public InvalidSerialException() : base("Invalid serial number.")
		{
			
		}
	}
	
	public class ServerLogicException : Exception
	{
		public ServerLogicException(string message)
			: base(message)
		{

		}
	}

	public class PermissionException : Exception
	{
		public PermissionException(string message)
			: base(message)
		{

		}
	}

	public class BaseAuthException : Exception
	{
		public BaseAuthException(string message)
			: base(message)
		{

		}
	}
	public class LogicException : BaseAuthException
	{
		public LogicException(string message)
			: base(message)
		{

		}
	}
	public class ValidationException : BaseAuthException
	{
		public ValidationException(string message) : base(message)
		{

		}
	}
	public class ValidateUserException : BaseAuthException
	{
		public string State { get; private set; }
		public ValidateUserException(string message, string state)
			: base(message)
		{
			this.State = state;
		}
	}
	
	public class CommunicationException : BaseAuthEngineException {
		public CommunicationException(string message, Exception innerException) : base(message, innerException) {
			
		}
	}
}
