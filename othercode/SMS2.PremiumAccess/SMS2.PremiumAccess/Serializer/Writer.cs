using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

namespace SMS2.Shared.Serializer
{
	public class Writer : MemoryStream
	{
		private XmlWriter writer;
		public Writer()
		{
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.OmitXmlDeclaration = true;
			settings.Encoding = new UTF8Encoding(false);
			writer = XmlWriter.Create(this, settings);
		}

		public XmlWriter GetWriter()
		{
			return writer;
		}

		public string GetString()
		{
			writer.Flush();
			this.Flush();
			this.Position = 0;
			return Encoding.UTF8.GetString(this.ToArray());
		}

		public new void Dispose()
		{
			try
			{
				Dispose(true);
			}
			finally
			{
				base.Dispose();
			}
		}

		~Writer()
		{
			Dispose(false);
		}

		protected override void Dispose(Boolean freeManagedObjectsAlso)
		{
			try
			{
				if (freeManagedObjectsAlso)
				{
					if (this.writer != null)
					{
						this.writer.Close();
						this.writer = null;
					}
				}
			}
			finally
			{
				base.Dispose(freeManagedObjectsAlso);
			}
		}
	}

	public class IndentedWriter : MemoryStream
	{
		private XmlWriter writer;
		public IndentedWriter()
		{
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.OmitXmlDeclaration = true;
			settings.Indent = true;
			settings.Encoding = new UTF8Encoding(false);
			writer = XmlWriter.Create(this, settings);
		}

		public XmlWriter GetWriter()
		{
			return writer;
		}

		public string GetString()
		{
			writer.Flush();
			this.Flush();
			this.Position = 0;
			return Encoding.UTF8.GetString(this.ToArray());
		}

		public new void Dispose()
		{
			try
			{
				Dispose(true);
			}
			finally
			{
				base.Dispose();
			}
		}

		~IndentedWriter()
		{
			Dispose(false);
		}

		protected override void Dispose(Boolean freeManagedObjectsAlso)
		{
			try
			{
				if (freeManagedObjectsAlso)
				{
					if (this.writer != null)
					{
						this.writer.Close();
						this.writer = null;
					}
				}
			}
			finally
			{
				base.Dispose(freeManagedObjectsAlso);
			}
		}
	}
}
