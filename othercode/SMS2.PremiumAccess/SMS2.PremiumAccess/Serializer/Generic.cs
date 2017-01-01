using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Xml.Serialization;

namespace SMS2.Shared.Serializer
{
	public interface IXmlSerializationCallback
	{
		void OnXmlDeserialized();
		void OnXmlSerializing();
		void OnXmlSerialized();
	}

	public class GenericXmlSerializer : XmlSerializer
	{
		public GenericXmlSerializer(Type type)
			: base(type)
		{

		}

		public GenericXmlSerializer(Type type, Type[] extraTypes)
			: base(type, extraTypes)
		{
		}
		protected override object Deserialize(XmlSerializationReader reader)
		{
			var result = base.Deserialize(reader);

			var deserializedCallback = result as IXmlSerializationCallback;
			if (deserializedCallback != null)
			{
				deserializedCallback.OnXmlDeserialized();
			}

			return result;
		}
		public new object Deserialize(TextReader textReader)
		{
			var result = base.Deserialize(textReader);

			var deserializedCallback = result as IXmlSerializationCallback;
			if (deserializedCallback != null)
			{
				deserializedCallback.OnXmlDeserialized();
			}

			return result;
		}

		protected override void Serialize(object o, XmlSerializationWriter writer)
		{
			var serializingCallBack = o as IXmlSerializationCallback;
			if (serializingCallBack != null)
			{
				serializingCallBack.OnXmlSerializing();
			}
			base.Serialize(o, writer);
			if (serializingCallBack != null)
			{
				serializingCallBack.OnXmlSerialized();
			}
		}

		public new void Serialize(XmlWriter xmlWriter, object o, XmlSerializerNamespaces namespaces)
		{
			var serializingCallBack = o as IXmlSerializationCallback;
			if (serializingCallBack != null)
			{
				serializingCallBack.OnXmlSerializing();
			}
			base.Serialize(xmlWriter, o, namespaces);
			if (serializingCallBack != null)
			{
				serializingCallBack.OnXmlSerialized();
			}
		}
	}

	public class Generic
	{
		/// <summary>
		/// Serializes the given object.
		/// </summary>
		/// <typeparam name="T">The type of the object to be serialized.</typeparam>
		/// <param name="obj">The object to be serialized.</param>
		/// <returns>String representation of the serialized object.</returns>
		public static string Serialize<T>(T obj)
		{
			using (Writer writer = new Writer())
			{
				GenericXmlSerializer xs = new GenericXmlSerializer(typeof(T));
				XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
				ns.Add("", "");
				xs.Serialize(writer.GetWriter(), obj, ns);
				return writer.GetString();
			}
		}

		/// <summary>
		/// Serializes the given object.
		/// </summary>
		/// <typeparam name="T">The type of the object to be serialized.</typeparam>
		/// <param name="obj">The object to be serialized.</param>
		/// <returns>String representation of the serialized object.</returns>
		public static string SerializeIndented<T>(T obj)
		{
			using (IndentedWriter writer = new IndentedWriter())
			{
				GenericXmlSerializer xs = new GenericXmlSerializer(typeof(T));
				XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
				ns.Add("", "");
				xs.Serialize(writer.GetWriter(), obj, ns);
				return writer.GetString();
			}
		}

		public static string Serialize<T>(T obj, Type[] extraTypes)
		{
			using (Writer writer = new Writer())
			{
				GenericXmlSerializer xs = new GenericXmlSerializer(typeof(T), extraTypes);
				XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
				ns.Add("", "");
				xs.Serialize(writer.GetWriter(), obj, ns);
				return writer.GetString();
			}
		}
		/// <summary>
		/// Serializes the given object.
		/// </summary>
		/// <typeparam name="T">The type of the object to be serialized.</typeparam>
		/// <param name="obj">The object to be serialized.</param>
		/// <param name="writer">The writer to be used for output in the serialization.</param>
		public static void Serialize<T>(T obj, XmlWriter writer)
		{
			GenericXmlSerializer xs = new GenericXmlSerializer(typeof(T));
			XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
			ns.Add("", "");
			xs.Serialize(writer, obj, ns);
		}
		/// <summary>
		/// Serializes the given object.
		/// </summary>
		/// <typeparam name="T">The type of the object to be serialized.</typeparam>
		/// <param name="obj">The object to be serialized.</param>
		/// <param name="writer">The writer to be used for output in the serialization.</param>
		/// <param name="extraTypes"><c>Type</c> array
		///       of additional object types to serialize.</param>
		public static void Serialize<T>(T obj, XmlWriter writer, Type[] extraTypes)
		{
			GenericXmlSerializer xs = new GenericXmlSerializer(typeof(T), extraTypes);
			XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
			ns.Add("", "");
			xs.Serialize(writer, obj, ns);
		}
		/// <summary>
		/// Deserializes the given object.
		/// </summary>
		/// <typeparam name="T">The type of the object to be deserialized.</typeparam>
		/// <param name="reader">The reader used to retrieve the serialized object.</param>
		/// <returns>The deserialized object of type T.</returns>
		public static T Deserialize<T>(XmlReader reader)
		{
			GenericXmlSerializer xs = new GenericXmlSerializer(typeof(T));
			return (T)xs.Deserialize(reader);
		}
		/// <summary>
		/// Deserializes the given object.
		/// </summary>
		/// <typeparam name="T">The type of the object to be deserialized.</typeparam>
		/// <param name="reader">The reader used to retrieve the serialized object.</param>
		/// <param name="extraTypes"><c>Type</c> array
		///           of additional object types to deserialize.</param>
		/// <returns>The deserialized object of type T.</returns>
		public static T Deserialize<T>(XmlReader reader, Type[] extraTypes)
		{
			GenericXmlSerializer xs = new GenericXmlSerializer(typeof(T), extraTypes);
			return (T)xs.Deserialize(reader);
		}
		/// <summary>
		/// Deserializes the given object.
		/// </summary>
		/// <typeparam name="T">The type of the object to be deserialized.</typeparam>
		/// <param name="XML">The XML file containing the serialized object.</param>
		/// <returns>The deserialized object of type T.</returns>
		public static T Deserialize<T>(string XML)
		{
			if (XML == null || XML == string.Empty)
				return default(T);
			GenericXmlSerializer xs = null;
			StringReader sr = null;
			try
			{
				xs = new GenericXmlSerializer(typeof(T));
				sr = new StringReader(XML);
				return (T)xs.Deserialize(sr);
			}
			catch (Exception ex)
			{
				throw ex;
			}
			finally
			{
				if (sr != null)
				{
					sr.Close();
					sr.Dispose();
				}
			}
		}
		public static T Deserialize<T>(string XML, Type[] extraTypes)
		{
			if (XML == null || XML == string.Empty)
				return default(T);
			GenericXmlSerializer xs = null;
			StringReader sr = null;
			try
			{
				xs = new GenericXmlSerializer(typeof(T), extraTypes);
				sr = new StringReader(XML);
				return (T)xs.Deserialize(sr);
			}
			catch (Exception ex)
			{
				throw ex;
			}
			finally
			{
				if (sr != null)
				{
					sr.Close();
					sr.Dispose();
				}
			}
		}
		public static void SaveAs<T>(T Obj, string FileName,
											 Encoding encoding, Type[] extraTypes)
		{
			if (File.Exists(FileName))
				File.Delete(FileName);
			DirectoryInfo di = new DirectoryInfo(Path.GetDirectoryName(FileName));
			if (!di.Exists)
				di.Create();
			XmlDocument document = new XmlDocument();
			XmlWriterSettings wSettings = new XmlWriterSettings();
			wSettings.Indent = true;
			wSettings.Encoding = encoding;
			wSettings.CloseOutput = true;
			wSettings.CheckCharacters = false;
			using (XmlWriter writer = XmlWriter.Create(FileName, wSettings))
			{
				if (extraTypes != null)
					Serialize<T>(Obj, writer, extraTypes);
				else
					Serialize<T>(Obj, writer);
				writer.Flush();
				document.Save(writer);
			}
		}
		public static void SaveAs<T>(T Obj, string FileName, Type[] extraTypes)
		{
			SaveAs<T>(Obj, FileName, new UTF8Encoding(false), extraTypes);
		}
		public static void SaveAs<T>(T Obj, string FileName, Encoding encoding)
		{
			SaveAs<T>(Obj, FileName, encoding, null);
		}
		public static void SaveAs<T>(T Obj, string FileName)
		{
			SaveAs<T>(Obj, FileName, new UTF8Encoding(false));
		}
		public static T Open<T>(string FileName, Type[] extraTypes)
		{
			T obj = default(T);
			if (File.Exists(FileName))
			{
				XmlReaderSettings rSettings = new XmlReaderSettings();
				rSettings.CloseInput = true;
				rSettings.CheckCharacters = false;
				var txt = File.ReadAllText(FileName, Encoding.UTF8);
				txt = txt.Trim();
				if (extraTypes != null)
					obj = Deserialize<T>(txt, extraTypes);
				else
					obj = Deserialize<T>(txt);
			}
			return obj;
		}
		public static T Open<T>(string FileName)
		{
			return Open<T>(FileName, null);
		}
	}
}
