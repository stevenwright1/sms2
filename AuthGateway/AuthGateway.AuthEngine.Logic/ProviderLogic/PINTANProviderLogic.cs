using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using AuthGateway.AuthEngine.Logic.DAL;
using AuthGateway.AuthEngine.Logic.Helpers;
using AuthGateway.AuthEngine.ProviderLogic;
using AuthGateway.Shared;
using AuthGateway.Shared.Log;
using AuthGateway.Shared.Tracking;
using AuthGateway.Shared.XmlMessages;
using AuthGateway.Shared.XmlMessages.Response.Ret.AuthEngine;

namespace AuthGateway.AuthEngine.Logic.ProviderLogic
{
	public class PINTANException : Exception
	{
		public PINTANException(string message)
			: base(message)
		{

		}
	}

	public class PINTANProviderLogic : IProviderLogic
	{
		private SystemConfiguration sc;
		private ServerLogic serverLogic;
		private string config;
		private Provider provider;

		public const string ACTION_INVALIDATEALLSHEETS = "InvalidateAllSheets";
		public const string ACTION_INVALIDATESHEETS = "InvalidateSheets";
		public const string ACTION_GETACTIVESHEETS = "GetActiveSheets";
		public const string ACTION_GENERATESHEETS = "GenerateSheets";

		public SetInfoRet CheckMissingUserInfo(string user, string org) { return new SetInfoRet(); }
		public bool SetMissingUserInfo(string user, string org, string field, string fieldValue) { return false; }
		public void ClearUserInfo(string user, string org) { }

		public string Name
		{
			get { return "PINTANProviderLogic"; }
		}

		public string RemoveSecretsFromConfig(string config)
		{
			return config;
		}

		public ValidateUserRet ValidateUser(string state, string user, string org)
		{
            Tracker.Instance.TrackEvent("User Validation Attempt with " + Name, Tracker.Instance.DefaultEventCategory);
			ValidateUserRet ret = new ValidateUserRet();
			ret.PName = this.Name;
			ret.CreditsRemaining = "0";

			using (var queries = DBQueriesProvider.Get())
			{
				try
				{
					var parms = new List<KeyValuePair<string, object>>();
					parms.Add(new KeyValuePair<string, object>("USER", user));
					parms.Add(new KeyValuePair<string, object>("ON", org));

					queries.BeginTransaction();
					long userId = queries.GetUserId(user, org);

					var userSheet = new UserSheet(queries, userId);

					var sheetAndCodes = userSheet.GetNextSheetAndCodes();

					var codeShownAt = string.Format("{0}{1}",
						TextHelper.Number2String(sheetAndCodes.CodeShownAtCol),
						sheetAndCodes.CodeShownAtRow.ToString()
						);
					var codeShownAtIs = sheetAndCodes.CodeShownAtIsCode;
					var codeRequestedAt = string.Format("{0}{1}",
						TextHelper.Number2String(sheetAndCodes.CodeRequestedAtCol),
						sheetAndCodes.CodeRequestedAtRow.ToString()
						);

					ret.CreditsRemaining = "1";
					ret.Extra = string.Format("|S|{0}|/S||A|{1}|/A||B|{2}|/B||C|{3}|/C|",
						sheetAndCodes.SheetId,
						codeShownAt,
						codeShownAtIs,
						codeRequestedAt
						);
					Logger.Instance.WriteToLog(this.Name + ".ValidateUser Extra: " + ret.Extra, LogLevel.Debug);
					queries.CommitTransaction();
				}
				catch (PINTANException ex)
				{
					Logger.Instance.WriteToLog(this.Name + ".ValidateUser Logic Error: " + ex.Message, LogLevel.Debug);
					ret.Error = ex.Message;
					queries.CommitTransaction();
                    Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
				}
				catch (Exception ex)
				{
					Logger.Instance.WriteToLog(this.Name + ".ValidateUser ERROR: " + ex.Message, LogLevel.Error);
					Logger.Instance.WriteToLog(this.Name + ".ValidateUser STACK: " + ex.StackTrace, LogLevel.Debug);
					ret.Error = "Error validating user";
					queries.RollbackTransaction();
                    Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
				}
			}

            if (string.IsNullOrEmpty(ret.Error)) {
                Tracker.Instance.TrackCustomEvent("User Validation Success with " + Name, Tracker.Instance.DefaultEventCategory, MACAddress.Get());
            }

			return ret;
		}

		public void ValidatePin(ValidatePinRet ret, string state, string user, string org, string pin)
		{
            Tracker.Instance.TrackEvent("PIN Validation Attempt with" + Name, Tracker.Instance.DefaultEventCategory);

			ret.Validated = false;
			using (var queries = DBQueriesProvider.Get())
			{
				try
				{
					var parms = new List<KeyValuePair<string, object>>();
					parms.Add(new KeyValuePair<string, object>("USER", user));
					parms.Add(new KeyValuePair<string, object>("ON", org));

					queries.BeginTransaction();
					long userId = queries.GetUserId(user, org);

					var userSheet = new UserSheet(queries, userId);

					userSheet.CheckCode(pin);
					ret.Validated = true;
					queries.CommitTransaction();
				}
				catch (PINTANException ex)
				{
					Logger.Instance.WriteToLog(this.Name + ".ValidateUser Logic Error: " + ex.Message, LogLevel.Debug);
					ret.Error = ex.Message;
					queries.CommitTransaction();
                    Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
				}
				catch (Exception ex)
				{
					Logger.Instance.WriteToLog(this.Name + ".ValidatePin ERROR: " + ex.Message, LogLevel.Error);
					Logger.Instance.WriteToLog(this.Name + ".ValidatePin STACK: " + ex.StackTrace, LogLevel.Debug);
					ret.Error = "Error validating pin";
					queries.RollbackTransaction();
                    Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
				}
			}

            if (ret.Validated) {
                Tracker.Instance.TrackCustomEvent("PIN Validation Success with" + Name, Tracker.Instance.DefaultEventCategory, MACAddress.Get());
            }
		}

		public RetBase Resync(string user, string org, string action, string parameters, string token1, string token2)
		{
			var ret = new ResyncHotpRet();
			ret.Out = 0;
			try
			{
				using (var queries = DBQueriesProvider.Get())
				{
					var userId = queries.GetUserId(user, org);
					var userSheet = new UserSheet(queries, userId);

					switch (action)
					{
						case ACTION_INVALIDATEALLSHEETS:
							var invalidatedAllSheets = userSheet.InvalidateAllUserSheets();
							ret.Extra = String.Join(",", invalidatedAllSheets);
							ret.Out = invalidatedAllSheets.Count;
							break;
						case ACTION_INVALIDATESHEETS:
							var sheetsToInvalidateIdsString = parameters.Split(new char[] { ',' });
							var sheetsToInvalidateIds = new List<long>();
							foreach (var idString in sheetsToInvalidateIdsString)
								sheetsToInvalidateIds.Add(Convert.ToInt64(idString));
							ret.Out = userSheet.InvalidateUserSheets(sheetsToInvalidateIds);
							break;
						case ACTION_GETACTIVESHEETS:
							var activeSheetIds = userSheet.GetActiveSheetIds();
							ret.Extra = String.Join(",", activeSheetIds);
							ret.Out = activeSheetIds.Count;
							break;
						case ACTION_GENERATESHEETS:
							var newSheets = userSheet.GenerateSheets(sc.PinTanMaxSheets, sc.PinTanRows, sc.PinTanColumns, sc.PintanPasscodeLength);
							var dic = new Dictionary<string, string>();
							dic.Add("R", sc.PinTanRows.ToString());
							dic.Add("C", sc.PinTanColumns.ToString());
							foreach (var sd in newSheets)
								dic.Add(sd.SheetId.ToString(), string.Join(",", sd.Codes));
							ret.Extra = TextHelper.GeneratePipeML(dic);
							ret.Out = newSheets.Count;
							break;
						default:
							throw new PINTANException(string.Format("Unsopported action '{0}'.", action));
					}
				}
			}
			catch (PINTANException ex)
			{
				ret.Error = ex.Message;
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
			}
			catch (Exception ex)
			{
				Logger.Instance.WriteToLog(this.Name + ".Resync ERROR Message: " + ex.Message, LogLevel.Error);
				Logger.Instance.WriteToLog(this.Name + ".Resync ERROR Stack: " + ex.StackTrace, LogLevel.Error);
				ret.Error = "Error " + this.Name + ".Resync";
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
			}
			return ret;
		}

		public IProviderLogic Using(Shared.SystemConfiguration sc)
		{
			this.sc = sc;
			return this;
		}

		public IProviderLogic Using(ServerLogic serverLogic)
		{
			this.serverLogic = serverLogic;
			return this;
		}

		public IProviderLogic Using(Provider provider)
		{
			this.provider = provider;
			return this;
		}

		public IProviderLogic UsingConfig(string config)
		{
			this.config = config;
			return this;
		}

		public bool UsesPincode()
		{
			return true;
		}
		public int GetPasscodeLen()
		{
			return sc.PintanPasscodeLength;
		}
		public void SendToken(string user, string org)
		{

		}
		
		public void PostSelect(string user, string org, long userId, int authProviderId, bool manuallySet)
		{
			if (manuallySet) {
				if (Logger.I.ShouldLog(LogLevel.DebugVerbose))
						Logger.I.WriteToLog(Name + ".PostSelect skipped: manually set", LogLevel.DebugVerbose);
				return;
			}

			if (!provider.AutoSetup) {
				if (Logger.I.ShouldLog(LogLevel.DebugVerbose))
					Logger.I.WriteToLog(Name + ".PostSelect skipped: !autoSetup", LogLevel.DebugVerbose);
				return;
			}
			
			if (Logger.I.ShouldLog(LogLevel.Debug))
				Logger.Instance.WriteToLog(Name + ".PostSelect", LogLevel.Debug);

			var tm = new TemplateMessage {
				Title = "WrightCCS - SMS2 - Authentication sheets",
				Message = @"
These are your SMS2 authentication sheets
"
			};
			
			var email = string.Empty;
			using (var queries = DBQueriesProvider.Get())
			{
				const string emailQuery = @"SELECT [Email] FROM [SMS_CONTACT] WHERE ID=@UserId";
				using (var dtGet = queries.Query(emailQuery,
	                     new DBQueryParm(@"UserId", userId)
	                    ))
				{
					if (dtGet.Rows.Count == 0)
					{
						if (Logger.I.ShouldLog(LogLevel.DebugVerbose))
							Logger.I.WriteToLog("PostSelect skipped: E-mail not found", LogLevel.DebugVerbose);
						return; // No email found
					}
						email = Convert.ToString(dtGet.Rows[0][0]);
				}
				if (string.IsNullOrEmpty(email)) {
					if (Logger.I.ShouldLog(LogLevel.DebugVerbose))
						Logger.I.WriteToLog("PostSelect skipped: E-mail empty or not found", LogLevel.DebugVerbose);
					return;
				}
				
				tm = queries.GetTemplateMessage("PIN/TAN Setup E-mail");
			}
			
			var resyncRet = (ResyncHotpRet)Resync(user, org, ACTION_GENERATESHEETS, string.Empty, string.Empty, string.Empty);
			try {
				if (!string.IsNullOrWhiteSpace(resyncRet.Error))
					throw new Exception(resyncRet.Error);

				if (resyncRet.Out == 0) {
					if (Logger.I.ShouldLog(LogLevel.DebugVerbose))
						Logger.I.WriteToLog("PostSelect skipped: No sheets were generated", LogLevel.DebugVerbose);
					return; 
				}
				
				Dictionary<string, string> data = TextHelper.ParsePipeML(resyncRet.Extra);
				Document doc = ProcessPINTANData(data);
				
				var password = sc.EmailConfig.Password;
				var pdfRenderer = new PdfDocumentRenderer(true);
				pdfRenderer.Document = doc;
				pdfRenderer.RenderDocument();
				using (var attachment = new MemoryStream())
				{
					pdfRenderer.Save(attachment, false);
					attachment.Flush();
					attachment.Position = 0;
					serverLogic.Registry.Get<IMailSender>().Send(sc.EmailConfig, password, email, tm.Title, tm.Message, "sheets.pdf", attachment);
				}
			} catch(Exception ex) {
				Logger.Instance.WriteToLog(Name + ".PostSelect ERROR Message: " + ex.Message, LogLevel.Error);
				Logger.Instance.WriteToLog(Name + ".PostSelect ERROR Stack: " + ex.StackTrace, LogLevel.Error);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
			}
			//renderer.Save(ofd.FileName);
			//serverLogic.Registry.Get<IMailSender>().Send(sc.EmailConfig, destiny, messageTemplate, password);
		}
		
		public Document ProcessPINTANData(Dictionary<string, string> data)
		{
			var doc = new Document();
			var sec = doc.AddSection();
			doc.DefaultPageSetup.PageFormat = PageFormat.A4;
			PageSetup ps = sec.PageSetup;
			int rows = 0;
			int columns = 0;
			var sheetsWithCodes = new List<SheetData>();

			foreach (KeyValuePair<string, string> kp in data) {
				if (kp.Key == "R") {
					rows = Convert.ToInt32(kp.Value);
				} else if (kp.Key == "C") {
					columns = Convert.ToInt32(kp.Value);
				} else {
					SheetData sd = new SheetData();
					sd.SheetId = Convert.ToInt64(kp.Key);
					sd.Codes.AddRange(kp.Value.Split(new char[] { ',' }));
					sheetsWithCodes.Add(sd);
				}
			}

			foreach (SheetData sd in sheetsWithCodes) {
				// Page header
				Paragraph p = sec.AddParagraph();
				ParagraphFormat pf = new ParagraphFormat();
				pf.Alignment = ParagraphAlignment.Center;
				p.AddText("Sheet ID: " + sd.SheetId.ToString());

				// Codes table
				var table = sec.AddTable();

				for (int i = 0; i <= columns; i++) {
					//table.AddColumn(Unit.FromMillimeter(190 / (columns + 1)))
					Column col = table.AddColumn();
					col.Format = new ParagraphFormat {
						SpaceAfter = Unit.FromPoint(5),
						SpaceBefore = Unit.FromPoint(5)
					};
				}

				// Table Header
				Row headerRow = table.AddRow();
				headerRow.HeadingFormat = true;
				headerRow.Format.Alignment = ParagraphAlignment.Center;
				headerRow.Format.Font.Bold = true;
				headerRow.Cells[0].AddParagraph("Rows/Columns");
				headerRow.Cells[0].Format.Font.Bold = false;
				headerRow.Cells[0].Format.Alignment = ParagraphAlignment.Center;
				headerRow.Cells[0].VerticalAlignment = VerticalAlignment.Center;
				for (int c = 1; c <= columns; c++) {
					headerRow.Cells[c].AddParagraph(TextHelper.Number2String(c));
					headerRow.Cells[c].Format.Font.Bold = false;
					headerRow.Cells[c].Format.Alignment = ParagraphAlignment.Center;
					headerRow.Cells[c].VerticalAlignment = VerticalAlignment.Center;
				}

				int currentColumnIndex = 0;
				int currentRowIndex = 0;
				Row row = null;
				for (int i = 0; i <= sd.Codes.Count - 1; i++) {
					if (currentColumnIndex == 0) {
						currentRowIndex = currentRowIndex + 1;
						row = table.AddRow();
						row.HeadingFormat = true;
						row.Format.Alignment = ParagraphAlignment.Center;
						row.Format.Font.Bold = true;
						row.Cells[0].AddParagraph(currentRowIndex.ToString());
						row.Cells[0].Format.Font.Bold = true;
						row.Cells[0].Format.Alignment = ParagraphAlignment.Center;
						row.Cells[0].VerticalAlignment = VerticalAlignment.Center;
						currentColumnIndex = currentColumnIndex + 1;
					}
					row.Cells[currentColumnIndex].AddParagraph(sd.Codes[i]);
					row.Cells[currentColumnIndex].Format.Font.Bold = false;
					row.Cells[currentColumnIndex].Format.Alignment = ParagraphAlignment.Center;
					row.Cells[currentColumnIndex].VerticalAlignment = VerticalAlignment.Center;
					currentColumnIndex = currentColumnIndex + 1;
					if (currentColumnIndex > columns) {
						currentColumnIndex = 0;
					}
				}
				sec.AddPageBreak();
			}
			return doc;
		}
	}
}
