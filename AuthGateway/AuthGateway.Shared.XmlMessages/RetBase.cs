using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace AuthGateway.Shared.XmlMessages
{
    [XmlTypeAttribute(AnonymousType = true)]
    [XmlRootAttribute(Namespace = "", IsNullable = true)]
    public abstract class RetBase
    {
        public string Error { get; set; }
    }
}
