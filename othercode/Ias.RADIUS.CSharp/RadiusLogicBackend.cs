using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using AuthGateway.Shared;
using AuthGateway.Shared.Log;
using AuthGateway.Shared.Serializer;
using AuthGateway.Shared.Tracking;
using AuthGateway.Shared.XmlMessages;
using AuthGateway.Shared.XmlMessages.Request;
using AuthGateway.Shared.XmlMessages.Request.Command.AuthEngine;
using AuthGateway.Shared.XmlMessages.Response;
using AuthGateway.Shared.XmlMessages.Response.Ret.AuthEngine;
using Mindscape.Raygun4Net;
namespace Ias.RADIUS.CSharp
{
	public class RadiusLogicBackend
	{
		public const int REJECT = 0;

		public const int CHALLENGE = 1;

		public const int ACCEPT = 2;

		public const int ACCEPTNOSTATE = 4;

		public const int FLOOD = 3;

        private const string ChallengeMessage = "Message challenge";

		private ConcurrentDictionary<string, StateInfo> states;

		protected SystemConfiguration sc;

		private readonly object lockStatesObj = new object();

		// states lock
		private RaygunClient raygun = null;

		public RadiusLogicBackend(SystemConfiguration sc, RaygunClient raygun) : this(sc)
		{
			this.raygun = raygun;
		}

		public RadiusLogicBackend(SystemConfiguration sc)
		{
			this.sc = sc;
			sc.LoadSettings();
			states = new ConcurrentDictionary<string, StateInfo>();
			Logger.Instance.SetFlushOnWrite(true);
			Logger.Instance.SetLogLevel(LogLevel.All);
			if (!sc.RadiusShowPin) {
				Logger.Instance.SetLogLevel(LogLevel.Info);
			}
			if (Logger.I.ShouldLog(LogLevel.Info))
				Logger.Instance.WriteToLog(string.Format("Started Radius Logic Backend - LogLevel set to '{0}'", Logger.Instance.GetLogLevel().ToString()), LogLevel.Info);
		}

		private AuthEngineResponse sendCmd(CommandBase cmd)
		{
			try {
				var tcpClient = TimeOutSocket.Connect(new IPEndPoint(sc.getAuthEngineServerAddress(), sc.AuthEngineServerPort), 5000);
				tcpClient.SendTimeout = 10000;
				tcpClient.ReceiveTimeout = 5000;
				#if DEBUG
				tcpClient.SendTimeout = 100000;
				tcpClient.ReceiveTimeout = 500000;
				#endif
				var ns = tcpClient.GetStream();
				var req = new AuthEngineRequest();
				req.Commands.Add(cmd);
				var tmpstring = Generic.Serialize<AuthEngineRequest>(req);
				if (Logger.I.ShouldLog(LogLevel.Debug))
					Logger.Instance.WriteToLog("Sent: " + tmpstring, LogLevel.Debug);
				var sendBytes = Encoding.UTF8.GetBytes(tmpstring);
				ns.Write(sendBytes, 0, sendBytes.Length);
				MemoryStream retData = new MemoryStream();
				var output = new Byte[tcpClient.ReceiveBufferSize];
				do {
					int bread = ns.Read(output, 0, output.Length);
					retData.Write(output, 0, bread);
				}
				while (ns.DataAvailable);
				retData.Position = 0;
				using (StreamReader sr = new StreamReader(retData))
					tmpstring = sr.ReadToEnd();
				tmpstring = tmpstring.TrimEnd(new char[] {
					(char)0
				}).Trim();
				if (Logger.I.ShouldLog(LogLevel.Debug))
					Logger.Instance.WriteToLog("Received: " + tmpstring, LogLevel.Debug);
				if (string.IsNullOrEmpty(tmpstring))
					return null;
				return Generic.Deserialize<AuthEngineResponse>(tmpstring);
			}
			catch (Exception e) {
				Logger.Instance.WriteToLog(string.Format("Error sendCmd ERROR: {0}\nStack: {1}", e.Message, e.StackTrace), LogLevel.Error);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, e);
				throw e;
			}
		}

		public ActOnRet ActOn(string username, string password, string state)
		{
			try {
				var newsi = new StateInfo();
				var si = states.GetOrAdd(username, newsi);
				if (string.IsNullOrEmpty(state) || (!state.Equals(si.State)))
					si.NewStateSupplied = true;
				else
					si.NewStateSupplied = false;
				// The state is valid, this is a valid Challenge Response
				if (!String.IsNullOrEmpty(state) && !String.IsNullOrEmpty(si.State) && state == si.State) {
					if (Logger.I.ShouldLog(LogLevel.Debug))
						Logger.Instance.WriteToLog(string.Format("Existing state, Challenge request '{0}' '{1}'", state, si.State), LogLevel.Debug);
				}
				// The state provided is invalid, add new State and respond accordingly
				else {
					if (Logger.I.ShouldLog(LogLevel.Debug))
						Logger.Instance.WriteToLog(string.Format("Existing invalid state, Accept request '{0}' '{1}'", state, si.State), LogLevel.Debug);
				}
				if (!si.FirstAccess && (DateTime.Now - si.CreatedTime).TotalMilliseconds < sc.MinTimeBetweenRadiusRequestsPerUser && (string.IsNullOrEmpty(state) || (!state.Equals(si.State)))) {
					lock (si.Lock) {
						if (!si.FirstAccess && (DateTime.Now - si.CreatedTime).TotalMilliseconds < sc.MinTimeBetweenRadiusRequestsPerUser && (string.IsNullOrEmpty(state) || (!state.Equals(si.State)))) {
							if (Logger.I.ShouldLog(LogLevel.Debug))
								Logger.Instance.WriteToLog(string.Format("Flood for: {0} - {1}", username, password), LogLevel.Debug);
							return new ActOnRet() {
								status = si.Status,
								state = si.State,
								message = si.StatusMessage
							};
						}
					}
				}
				lock (si.Lock) {
					si.FirstAccess = false;
					if (si.NewStateSupplied) {
						return actOnNewStateSupplied(si, username, password, state);
					}
					else {
						return actOnExistingState(si, username, password, state);
					}
				}
			}
			catch (Exception ex) {
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
				if (raygun != null) {
					try {
						raygun.SendInBackground(ex, new List<string>() {
							"Ias.RADIUS.CSharp",
							"ActOn"
						});
					}
					catch {
						Logger.Instance.WriteToLog(ex, new List<string>() {
							"Raygun"
						});
					}
				}
				Logger.Instance.WriteToLog(string.Format("Error outside main logic: Username '{0}' \nMessage: {1}\nStack: {2}", username, ex.Message, ex.StackTrace), LogLevel.Error);
				if (Logger.I.ShouldLog(LogLevel.Debug))
					Logger.Instance.WriteToLog(string.Format("Error outside main logic: Username '{0}' Password '{1}' State '{2}' \nMessage: {3}\nStack: {4}", username, password, state, ex.Message, ex.StackTrace), LogLevel.Debug);
				return new ActOnRet() {
					status = REJECT,
					state = string.Empty,
					message = "Radius error: " + ex.Message
				};
			}
		}

		ActOnRet actOnExistingState(StateInfo si, string username, string password, string state)
		{
			if (Logger.I.ShouldLog(LogLevel.Debug))
				Logger.Instance.WriteToLog(string.Format("Get state: {0} - {1} - '{2}'", username, password, state), LogLevel.Debug);
			try {
				if (si.AskingInfo) {
					if (Logger.I.ShouldLog(LogLevel.DebugVerbose))
						Logger.I.WriteToLog(string.Format("Set info {0} - Field: '{1}' Value: '{2}'", username, si.AksingInfoField, password), LogLevel.DebugVerbose);
					var cmd = new SetInfo();
					cmd.User = username;
					cmd.Field = si.AksingInfoField;
					cmd.Value = password;
					cmd.State = si.State;
					var returnResponse = sendCmd(cmd);
					if (returnResponse == null)
						throw new RadiusLogicException("No user found.");
					var retcmd = (SetInfoRet)returnResponse.Responses[0];
					if (retcmd.AI) {
						si.AksingInfoField = retcmd.AIF;
						if (Logger.I.ShouldLog(LogLevel.DebugVerbose))
							Logger.I.WriteToLog(string.Format("Asking more info {0} Field: '{1}'", username, si.AksingInfoField), LogLevel.DebugVerbose);
						return new ActOnRet {
							status = CHALLENGE,
							state = si.State,
							message = retcmd.Extra,
						};
					}
					si.AskingInfo = false;
					if (Logger.I.ShouldLog(LogLevel.DebugVerbose))
						Logger.I.WriteToLog(string.Format("Finished asking info {0} Field: '{1}'", username, si.AksingInfoField), LogLevel.DebugVerbose);
					var retMessage = ChallengeMessage;
					if (!string.IsNullOrEmpty(sc.RadiusChallengeMessage))
						retMessage = sc.RadiusChallengeMessage;
					return actOnNewStateSupplied(si, username, string.Empty, state);
				}
				
				if (si.AskingVault) {
					if (Logger.I.ShouldLog(LogLevel.DebugVerbose))
						Logger.I.WriteToLog(string.Format("Set vault {0} - Value: '{1}'", username, password), LogLevel.DebugVerbose);
					var cmd = new SetUVault();
					cmd.User = username;
					cmd.Value = password;
					cmd.State = si.State;
					var returnResponse = sendCmd(cmd);
					if (returnResponse == null)
						throw new RadiusLogicException("No user found.");
					var retcmd = (SetUVaultRet)returnResponse.Responses[0];
					if (retcmd.ADP) {
						if (Logger.I.ShouldLog(LogLevel.DebugVerbose))
							Logger.I.WriteToLog(string.Format("Asking AD Password (SetUVault) {0}", username), LogLevel.DebugVerbose);
						return new ActOnRet {
							status = CHALLENGE,
							state = si.State,
							message = "Enter your AD password:"
						};
					}
					si.AskingVault = false;
					if (Logger.I.ShouldLog(LogLevel.DebugVerbose))
						Logger.I.WriteToLog(string.Format("Finished setting AD Password {0}", username), LogLevel.DebugVerbose);
					return new ActOnRet {
						status = ACCEPT,
						state = si.State,
						message = "Message approved",
						password = retcmd.ADPASS
					};
				} else {
					var cmd = new ValidatePin();
					cmd.User = username;
					cmd.Pin = password;
					cmd.State = si.State;
					var returnResponse = sendCmd(cmd);
					if (returnResponse == null)
						throw new RadiusLogicException("No user found.");
					var retcmd = (ValidatePinRet)returnResponse.Responses[0];
					if (!string.IsNullOrEmpty(retcmd.Error))
						throw new RadiusLogicException(retcmd.Error);
					
					if (retcmd.ADP) {
						if (Logger.I.ShouldLog(LogLevel.DebugVerbose))
							Logger.I.WriteToLog(string.Format("Asking AD Password (ValidatePin) {0}", username), LogLevel.DebugVerbose);
						si.AskingVault = true;
						return new ActOnRet {
							status = CHALLENGE,
							state = si.State,
							message = "Enter your AD password:"
						};
					}
					
					if (!retcmd.Validated)
						throw new RadiusLogicException("Pin not validated");
					if (Logger.I.ShouldLog(LogLevel.Debug))
						Logger.Instance.WriteToLog(string.Format("Validate state: {0} - {1}", username, password), LogLevel.Debug);
					
					return new ActOnRet {
						status = ACCEPT,
						state = si.State,
						message = "Message approved",
						password = retcmd.ADPASS,
						panic = Convert.ToInt16(retcmd.Panic)
					};
				}
			} catch (RadiusLogicException e) {
				StateInfo siout;
				states.TryRemove(username, out siout);
				var retState = string.Empty;
				if (!string.IsNullOrEmpty(si.State))
					retState = si.State;
				Logger.Instance.WriteToLog(string.Format("Error validating(ValidatePin) Username '{0}' \nMessage: {1}\nStack: {2}", username, e.Message, e.StackTrace), LogLevel.Error);
				if (Logger.I.ShouldLog(LogLevel.Debug))
					Logger.Instance.WriteToLog(string.Format("Error validating(ValidatePin) state: Username '{0}' Password '{1}' State '{2}' \nMessage: {3}\nStack: {4}", username, password, retState, e.Message, e.StackTrace), LogLevel.Debug);
				if (e.InnerException != null) {
					if (Logger.I.ShouldLog(LogLevel.Error))
						Logger.Instance.WriteToLog(string.Format("Message: {0}\nStack: {1}", e.InnerException.Message, e.InnerException.StackTrace), LogLevel.Error);
				}                
				return new ActOnRet {
					status = REJECT,
					state = retState,
					message = "Pin validation failed: " + e.Message
				};
			}
		}

		ActOnRet actOnNewStateSupplied(StateInfo si, string username, string password, string state)
		{
			si.NewStateSupplied = false;
			try {
				if (Logger.I.ShouldLog(LogLevel.Debug))
					Logger.Instance.WriteToLog(string.Format("NEW: username: '{0}' password: '{1}' state: '{2}'", username, password, state), LogLevel.Debug);
				var cmd = new ValidateUser() {
					User = username,
					PinCode = password
				};
				if (!string.IsNullOrEmpty(state))
					cmd.State = state;
				var returnResponse = sendCmd(cmd);
				if (returnResponse == null)
					throw new RadiusLogicException("No user found.");
				var retcmd = (ValidateUserRet)returnResponse.Responses[0];
				if (!string.IsNullOrEmpty(retcmd.Error)) {
					if (Logger.I.ShouldLog(LogLevel.Debug))
						Logger.Instance.WriteToLog(string.Format("ValidateUser returned .Error: '{0}'", retcmd.Error), LogLevel.Debug);
				}
				if (retcmd.State == null)
					si.State = string.Empty;
				else
					si.State = retcmd.State;
				var retStatus = CHALLENGE;
				var retMessage = ChallengeMessage;
				if (!string.IsNullOrEmpty(retcmd.Error)) {
					if (sc.BaseSendTokenTestMode) {
						try {
							int token;
							if (!Int32.TryParse(retcmd.Error, out token))
								throw new Exception("Expected a token in .Error: " + retcmd.Error);
							retMessage = retcmd.Error;
							retcmd.Error = string.Empty;
						}
						catch {
						}
					}
					if (!string.IsNullOrEmpty(retcmd.Error))
						throw new RadiusLogicException(retcmd.Error);
				}
                else if (!string.IsNullOrEmpty(retcmd.MutualAuthChallengeMessage)) {
                    retMessage = retcmd.MutualAuthChallengeMessage;
                    if (!string.IsNullOrEmpty(sc.RadiusChallengeMessage))
                        retMessage += sc.RadiusChallengeMessage;
                    else
                        retMessage += ChallengeMessage;
                }
				else if (!string.IsNullOrEmpty(sc.RadiusChallengeMessage)) {
						retMessage = sc.RadiusChallengeMessage;
				}
				if (retcmd.PName == "AuthDisabledProviderLogic") {
					//TODO: checkVaulting(username);
					
					StateInfo siout;
					states.TryRemove(username, out siout);
					si.State = "ACCEPT";
					retStatus = ACCEPTNOSTATE;
					retMessage = "Message accepted";
					if (Logger.I.ShouldLog(LogLevel.Info))
						Logger.Instance.WriteToLog(string.Format("User '{0}' Accepted", username), LogLevel.Info);
				} else {
					if (Logger.I.ShouldLog(LogLevel.Info))
						Logger.Instance.WriteToLog(string.Format("User '{0}' Challenged", username), LogLevel.Info);
					if (retcmd.AI) {
						si.AskingInfo = true;
						si.AksingInfoField = retcmd.AIF;
						if (!string.IsNullOrEmpty(retcmd.Extra))
							retMessage = retcmd.Extra;
					} else {
						if (retcmd.PName == "PINTANProviderLogic") {
							var data = TextHelper.ParsePipeML(retcmd.Extra);
							var sheet = data["S"];
							var codeAt = data["A"];
							var codeAtIs = data["B"];
							var codeRequestedAt = data["C"];
							retMessage = string.Format("TAN SHEET '{0}' - '{1}' is '{2}' - Provide value for '{3}'", sheet, codeAt, codeAtIs, codeRequestedAt);
						}
					}
				}
				si.Status = retStatus;
				si.StatusMessage = retMessage;
				var actonret = new ActOnRet {
					status = si.Status,
					state = si.State,
					message = si.StatusMessage,
					panic = Convert.ToInt16(retcmd.Panic),
				};
				if (actonret.status == ACCEPT) {
					if (Logger.I.ShouldLog(LogLevel.Debug))
						Logger.Instance.WriteToLog(string.Format("Accepted state: {0} - {1} - '{2}' - '{3}'", username, password, actonret.state, actonret.message), LogLevel.Debug);
				}
				else
					if (actonret.status == ACCEPTNOSTATE) {
						if (Logger.I.ShouldLog(LogLevel.Debug))
							Logger.Instance.WriteToLog(string.Format("ACCEPTNOSTATE state: {0} - {1} - '{2}' - '{3}'", username, password, actonret.state, actonret.message), LogLevel.Debug);
					}
					else {
						if (Logger.I.ShouldLog(LogLevel.Debug))
							Logger.Instance.WriteToLog(string.Format("Challenged state: {0} - {1} - '{2}' - '{3}'", username, password, actonret.state, actonret.message), LogLevel.Debug);
					}
				return actonret;
			}
			catch (RadiusLogicException e) {
				StateInfo siout;
				states.TryRemove(username, out siout);
				var retState = string.Empty;
				if (!string.IsNullOrEmpty(si.State))
					retState = si.State;
				Logger.Instance.WriteToLog(string.Format("Error adding(ValidateUser) Username '{0}' \nMessage: {1}\nStack: {2}", username, e.Message, e.StackTrace), LogLevel.Error);
				if (Logger.I.ShouldLog(LogLevel.Debug))
					Logger.Instance.WriteToLog(string.Format("Error adding(ValidateUser) state: Username '{0}' Password '{1}' State '{2}'", username, password, retState), LogLevel.Debug);
				if (e.InnerException != null) {
					if (Logger.I.ShouldLog(LogLevel.Error))
						Logger.Instance.WriteToLog(string.Format("Message: {0}\nStack: {1}", e.InnerException.Message, e.InnerException.StackTrace), LogLevel.Error);
				}                
				return new ActOnRet {
					status = REJECT,
					state = retState,
					message = "Auth Failed: " + e.Message
				};
			}
		}		
	}
}


