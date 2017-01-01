
using System;
namespace AuthGateway.Shared.Helper {
	public class HotpTotpModel {
		public const string HOTP = "HOTP";
		public const string TOTP = "TOTP";
		
		public HotpTotpModel() {
			Type = "HOTP";
			Secret = string.Empty;
			CounterSkew = "1";
			Window = "30";
			DeviceName = "Default";
            DeviceType = "Feitian Serial";
		}
		
		public string Type { get; set; }
		public string Secret { get; set; }
		public string CounterSkew { get; set; }
		public string Window { get; set; }
		public string DeviceName { get; set; }

        public string DeviceType { get; set; }
		
		public static string Serialize(HotpTotpModel model) {
			switch(model.Type) {
				case HOTP:
					return string.Format(
						"{0},{1},{2},{3},{4}"
						, model.Type, model.Secret, model.CounterSkew, model.DeviceName, model.DeviceType
	                    );
				case TOTP:
					return string.Format(
						"{0},{1},{2},{3},{4},{5}"
						, model.Type, model.Secret, model.CounterSkew, model.Window, model.DeviceName, model.DeviceType
	                    );
				default:
					throw new Exception(string.Format("Serialize OATH Type '{0}' not recognized", model.Type));
			}
		}
		
		public static HotpTotpModel Unserialize(string config) {
			var model = new HotpTotpModel();
			if (string.IsNullOrWhiteSpace(config))
				return model;
			
			var configSplit = config.Split(new [] { ',' }, StringSplitOptions.None);
			if (configSplit.Length == 3) {
				config += ",Default";
				configSplit = config.Split(new [] { ',' }, StringSplitOptions.None);
			}
			model.Type = configSplit[0];
			model.Secret = configSplit[1];
			model.CounterSkew = configSplit[2];            
			switch (model.Type) {
				case HOTP:
					model.DeviceName = configSplit[3];
                    if (configSplit.Length > 4)
                        model.DeviceType = configSplit[4];
					break;
				case TOTP:
					if (configSplit.Length <= 4) {
						model.Window = "30";
						model.DeviceName = configSplit[3];
                        model.DeviceType = configSplit[4];
					} else {
						model.Window = configSplit[3];
						model.DeviceName = configSplit[4];
                        if (configSplit.Length > 5)
                            model.DeviceType = configSplit[5];
					}
					break;
				default:
					throw new Exception(string.Format("Unserialize OATH Type '{0}' not recognized", model.Type));
			}
			return model;
		}
	}
}