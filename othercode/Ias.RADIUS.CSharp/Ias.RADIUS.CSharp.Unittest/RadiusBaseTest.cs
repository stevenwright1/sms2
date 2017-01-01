using System;
using AuthGateway.AuthEngine;
using AuthGateway.AuthEngine.Unittest;
using AuthGateway.Shared;
using AuthGateway.Shared.XmlMessages.Request.Command.AuthEngine;
using AuthGateway.Shared.XmlMessages.Response.Ret.AuthEngine;
using NUnit.Framework;

namespace Ias.RADIUS.CSharp.Unittest
{
	public abstract class RadiusBaseTest : TestPINTANBase
	{
		protected RadiusLogicBackend backend;

		public void SetupRadiusLogicTest(bool start = true)
		{
			Console.WriteLine("SetupRadiusLogicTest");
			
			sc.AuthEngineAskMissingInfo = true;
			ServerLogic.setSetting("AESETTING", "AuthEngineAskMissingInfo", sc.AuthEngineAskMissingInfo.ToString());
			sc.AuthEngineAskPin = true;
			ServerLogic.setSetting("AESETTING", "AuthEngineAskPin", sc.AuthEngineAskPin.ToString());
			sc.AuthEngineAskProviderInfo = true;
			ServerLogic.setSetting("AESETTING", "AuthEngineAskProviderInfo", sc.AuthEngineAskProviderInfo.ToString());
			
			sc.AuthEngineUseEncryption = false;
			sc.CloudSMSUseEncryption = false;
			sc.OATHCalcUseEncryption = false;
			sc.AuthEnginePinCode = PinCodeOption.False;
			ServerLogic.setSetting("AESETTING", "AuthEnginePinCode", sc.AuthEnginePinCode.ToString());
			sc.BaseSendTokenTestMode = true;
			sc.RadiusShowPin = true;
			sc.StopServiceOnException = true;
			if (start) {
				RadiusStartAuthEngine();
			}
		}

		public void RadiusStartAuthEngine()
		{
			startAuthEngineServer(registry);
			startCloudSMSServer();
			startOATHServer();
			
			backend = new RadiusLogicBackend(sc);
		}
		
		protected string sendToken(AddFullDetails adfUser)
			{
				var cmd = new SendToken();
				cmd.Identity = new FakeIdentity(adfUser);
				cmd.User = adfUser.User;
				cmd.Org = adfUser.Org;

				var cmdret = (SendTokenRet)aeServerLogic.Actioner.Do(cmd);

				var token = 0;
				try
				{
					token = Convert.ToInt32(cmdret.Error);
				}
				catch
				{

				}
				Assert.AreNotEqual(0, token, "We should have received new token in .Error property");
				return cmdret.Error;
			}

	}
}


