using System;

namespace Ias.RADIUS.CSharp
{
	public class StateInfo
	{
		public StateInfo()
		{
			this.CreatedTime = DateTime.Now;
			this.NewStateSupplied = true;
			this.FirstAccess = true;
			this.AskingInfo = false;
			this.AskingVault = false;
		}

		public string State {
			get;
			set;
		}

		public DateTime CreatedTime {
			get;
			private set;
		}

		public readonly object Lock = new object();

		public bool NewStateSupplied {
			get;
			set;
		}

		public int Status {
			get;
			set;
		}

		public string StatusMessage {
			get;
			set;
		}

		public bool FirstAccess {
			get;
			set;
		}

		public bool AskingInfo {
			get;
			set;
		}
		
		public bool AskingVault {
			get;
			set;
		}

		public string AksingInfoField {
			get;
			set;
		}
	}
}


