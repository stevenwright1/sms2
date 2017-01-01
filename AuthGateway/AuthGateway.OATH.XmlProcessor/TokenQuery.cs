using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;
using AuthGateway.Shared;
namespace AuthGateway.OATH.XmlProcessor
{

	[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
	[System.SerializableAttribute()]
	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[XmlTypeAttribute(AnonymousType = true)]
	[XmlRootAttribute(Namespace = "", IsNullable = false)]
	public partial class tokenquery : EncryptableRequest
	{


		private string keyField;

		private string hashField = "hotp";

		private string hexhashField = "0";

		private string counterField = "0";

		private string digitsField = "6";

		private string aftervaluesField = "0";

		private string beforevaluesField = "0";

		private string windowField = "30";

		private string idField;

		private value[] resynctokensField;
		/// <remarks/>
		[XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
		public string key {
			get { return this.keyField; }
			set { this.keyField = value; }
		}

		/// <remarks/>
		[XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
		public string hash {
			get { return this.hashField; }
			set { this.hashField = value; }
		}

		/// <remarks/>
		[XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
		public string hexhash {
			get { return this.hexhashField; }
			set { this.hexhashField = value; }
		}

		/// <remarks/>
		[XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
		public string counter {
			get { return this.counterField; }
			set { this.counterField = value; }
		}

		/// <remarks/>
		[XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
		public string digits {
			get { return this.digitsField; }
			set { this.digitsField = value; }
		}

		/// <remarks/>
		[XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
		public string aftervalues {
			get { return this.aftervaluesField; }
			set { this.aftervaluesField = value; }
		}

		/// <remarks/>
		[XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
		public string beforevalues {
			get { return this.beforevaluesField; }
			set { this.beforevaluesField = value; }
		}

		/// <remarks/>
		[XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
		public string window {
			get { return this.windowField; }
			set { this.windowField = value; }
		}

		/// <remarks/>
		[XmlAttributeAttribute()]
		public string id {
			get { return this.idField; }
			set { this.idField = value; }
		}

		/// <remarks/>
		[XmlArrayAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
		[XmlArrayItemAttribute("value", typeof(value))]
		public value[] resynctokens {
			get { return this.resynctokensField; }
			set { this.resynctokensField = value; }
		}

		public override void Nullify()
		{
			this.idField = null;
			this.aftervaluesField = null;
			this.beforevaluesField = null;
			this.counterField = null;
			this.digitsField = null;
			this.hashField = null;
			this.hexhashField = null;
			this.idField = null;
			this.keyField = null;
			this.windowField = null;
			this.resynctokensField = null;
		}
	}
}
