
namespace AuthGateway.Shared
{
	public abstract class EncryptCheckResponse
	{
		public string CHK { get; set; }
	}

	public abstract class EncryptableRequest
	{
		public string CKey { get; set; }
		public string CIV { get; set; }
		public virtual void Nullify() { }
	}
}
