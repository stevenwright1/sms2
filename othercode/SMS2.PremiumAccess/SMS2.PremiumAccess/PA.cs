using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using SMS2.Shared.Serializer;

namespace SMS2.PremiumAccess
{
	public class PA
	{
		public virtual string GetConfigFile()
		{
			return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SMS2.PremiumAccess", "Clients.xml");
		}

		public ClientsList LoadClients()
		{
			return LoadClients(GetConfigFile());
		}

		public ClientsList LoadClients(string file)
		{
			if (!File.Exists(file))
			{
				var clients = new ClientsList();

				clients.Items.Add(
					new Client() { ID = "Test TOTP 1", SecurityCode = "", SharedSecret = "secret1", Digits = 6, Config = "30", Type = OAuthType.TOTP }
				);
				clients.Items.Add(
					new Client() { ID = "Test HOTP 1", SecurityCode = "", SharedSecret = "secret2", Digits = 6, Config = "3", Type = OAuthType.HOTP }
				);

				return clients;
				;
			}
			return Generic.Open<ClientsList>(file);
		}

		public void SaveClients(ClientsList clients)
		{
			SaveClients(GetConfigFile(), clients);
		}

		public void SaveClients(string file, ClientsList clients)
		{
			var enc = new UTF8Encoding(false);
			Generic.SaveAs<ClientsList>(clients, file, enc, new Type[] { typeof(ClientsList) });
		}

		public class ClientsList : IXmlSerializationCallback
		{
			public ClientsList() 
			{
				this.Items = new List<Client>();
			}

			public int SelectedIndex { get; set; }

			[XmlArrayItem(typeof(Client))]
			public List<Client> Items { get; set; }

			public void OnXmlDeserialized()
			{
				this.Items.ForEach(delegate(Client item) { item.OnXmlDeserialized(); });
			}

			public void OnXmlSerializing()
			{
				this.Items.ForEach(delegate(Client item) { item.OnXmlSerializing(); });
			}

			public void OnXmlSerialized()
			{
				this.Items.ForEach(delegate(Client item) { item.OnXmlSerialized(); });
			}
		}
	}
}
