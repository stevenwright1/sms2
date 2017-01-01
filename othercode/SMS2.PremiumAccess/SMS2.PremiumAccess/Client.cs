using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;
using SMS2.Shared.Oath;
using SMS2.Shared.Serializer;
using SMS2.PremiumAccess.Encryption;

namespace SMS2.PremiumAccess
{
	public enum OAuthType
	{
		HOTP = 1,
		TOTP
	}

	public interface IClientActioner
	{

		void Refresh(Client client);

		void ValidateConfig(string config);
	}

	public class TotpClientActioner : IClientActioner
	{
		public void Refresh(Client client)
		{
			var secret = client.SharedSecret;
			var digits = client.Digits;

			var window = Convert.ToInt32(client.Config);

			client.SecurityCode = HotpTotp.TOTP(secret, digits, window);
		}

		public void ValidateConfig(string config)
		{
			long window = 0;
			if (Int64.TryParse(config, out window))
			{
				return;
			}
			throw new ArgumentException("Invalid window setting, it must be a number.");
		}
	}

	public class HotpClientActioner : IClientActioner
	{
		public void Refresh(Client client)
		{
			var secret = client.SharedSecret;
			var digits = client.Digits;

			var counter = Convert.ToInt32(client.Config);

			client.SecurityCode = HotpTotp.HOTP(secret, counter, digits);
			counter++;
			client.Config = Convert.ToString(counter);
		}

		public void ValidateConfig(string config)
		{
			long counter = 0;
			if (Int64.TryParse(config, out counter))
			{
				return;
			}
			throw new ArgumentException("Invalid counter setting, it must be a number.");
		}
	}

	public class ClientActionerFactory
	{
		public static IClientActioner Get(Client client)
		{
			switch (client.Type)
			{
				case OAuthType.TOTP:
					return new TotpClientActioner();
				case OAuthType.HOTP:
					return new HotpClientActioner();
				default:
					throw new Exception(string.Format("Actioner not found for type '{0}'", client.Type.ToString()));
			}
		}
	}

	public class Client : INotifyPropertyChanged, IXmlSerializationCallback
	{
		private string id;
		public string ID
		{
			get { return id; }
			set { id = value; NotifyPropertyChanged("ID"); }
		}

		private string securityCode;

		[XmlIgnore]
		public string SecurityCode
		{
			get { return securityCode; }
			set
			{
				if (value == securityCode) return;
				securityCode = value;
				NotifyPropertyChanged("SecurityCode");
			}
		}

		private OAuthType type;
		public OAuthType Type
		{
			get { return type; }
			set { type = value; NotifyPropertyChanged("Type"); }
		}

		private int digits;
		public int Digits
		{
			get { return digits; }
			set { digits = value; NotifyPropertyChanged("Digits"); }
		}

		private string sharedSecret;
		public string SharedSecret
		{
			get { return sharedSecret; }
			set { sharedSecret = value; NotifyPropertyChanged("SharedSecret"); }
		}

		private string config;
		public string Config
		{
			get { return config; }
			set { config = value; NotifyPropertyChanged("Config"); }
		}

		public event PropertyChangedEventHandler PropertyChanged;
		private void NotifyPropertyChanged(string propertyName)
		{
			if (PropertyChanged == null) return;
			PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		public void OnXmlDeserialized()
		{
			if (string.IsNullOrWhiteSpace(this.sharedSecret)) return;
			try
			{
				this.sharedSecret = CryptoHelper.DecryptSettingIfNecessary(this.sharedSecret, "sms2premiumclient");
			}
			catch { }
		}

		public void OnXmlSerializing()
		{
			if (string.IsNullOrWhiteSpace(this.sharedSecret)) return;
			var currentValue = this.SharedSecret;
			try
			{
				this.sharedSecret = CryptoHelper.EncryptSettingIfNecessary(this.sharedSecret, "sms2premiumclient");
			}
			catch
			{
				this.sharedSecret = currentValue;
			}
		}

		public void OnXmlSerialized()
		{
			if (string.IsNullOrWhiteSpace(this.sharedSecret)) return;
			try
			{
				this.sharedSecret = CryptoHelper.DecryptSettingIfNecessary(this.sharedSecret, "sms2premiumclient");
			}
			catch { }
		}
	}

	public class ClientViewModel
	{
		public ClientViewModel()
			: this(new SMS2.PremiumAccess.PA.ClientsList())
		{
		}

		public ClientViewModel(SMS2.PremiumAccess.PA.ClientsList startClients)
		{
			var index = startClients.SelectedIndex;
			if (index >= startClients.Items.Count) 
				index = 0;
			this.SelectedClient = startClients.Items[index];
			this.clientsView = new ObservableCollection<Client>(startClients.Items);
		}

		private readonly ObservableCollection<Client> clientsView;

		public ObservableCollection<Client> ListOfClients
		{
			get { return clientsView; }
		}

		public Client SelectedClient
		{
			get;
			set;
		}

		public void Remove(Client client)
		{
			clientsView.Remove(client);
		}

		public IEnumerable<OAuthType> OAuthTypeValues
		{
			get
			{
				return Enum.GetValues(typeof(OAuthType)).Cast<OAuthType>();
			}
		}
	}
}
