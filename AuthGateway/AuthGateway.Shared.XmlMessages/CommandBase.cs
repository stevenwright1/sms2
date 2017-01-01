using System;
using System.Xml.Serialization;
using System.Security.Principal;

namespace AuthGateway.Shared.XmlMessages
{
    [XmlTypeAttribute(AnonymousType = true)]
    [XmlRootAttribute(Namespace = "", IsNullable = true)]
    public abstract class CommandBase : IDisposable
    {
        private bool _authenticated = false;

        [XmlIgnore]
        public bool Authenticated { get { return _authenticated; } set { _authenticated = value; } }
        [XmlIgnore]
        public IIdentity Identity { get; set; }
        [XmlIgnore]
        public bool External { get; set; }

				public void Dispose()
				{
					if (Identity != null)
					{
						var asWI = Identity as WindowsIdentity;
						if (asWI != null)
							asWI.Dispose();
					}
				}
		}

    [XmlTypeAttribute(AnonymousType = true)]
    [XmlRootAttribute(Namespace = "", IsNullable = true)]
    public class UserProvider
    {
        [XmlIgnore]
        public bool Enabled { get; set; }
        [XmlIgnore]
        public Provider Provider { get; set; }
        public string Name { get; set; }
		public string FName { get; set; }
		public string Config { get; set; }
        public bool Selected { get; set; }
    }
    
    [XmlTypeAttribute(AnonymousType = true)]
    [XmlRootAttribute(Namespace = "", IsNullable = true)]
    public class TemplateMessage
    {
    	public string Label { get; set; }
    	public string Title { get; set; }
    	public string Message { get; set; }
    	public string Legend { get; set; }
    	public int Order { get; set; }
    	
		public override string ToString()
		{
			return Label;
		}
    }
    
    [XmlTypeAttribute(AnonymousType = true)]
    [XmlRootAttribute(Namespace = "", IsNullable = true)]
    public class Setting
    {
    	public Setting() {
    		Name = string.Empty;
    		Value = string.Empty;
    		Object = string.Empty;
    	}
    	public string Name { get; set; }
    	public string Value { get; set; }
    	public string Object { get; set; }
		public override string ToString()
		{
			return string.Format("{0}: {1}", Name, Value);
		}
    }
}
