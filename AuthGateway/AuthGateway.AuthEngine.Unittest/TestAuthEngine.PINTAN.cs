using System;
using AuthGateway.AuthEngine.Logic.DAL;
using AuthGateway.AuthEngine.Logic.Helpers;
using AuthGateway.Shared;
using AuthGateway.Shared.XmlMessages;
using AuthGateway.Shared.XmlMessages.Request.Command.AuthEngine;
using AuthGateway.Shared.XmlMessages.Response.Ret.AuthEngine;
using System.Collections.Generic;
using AuthGateway.AuthEngine.Logic.ProviderLogic;

using NUnit.Framework;

namespace AuthGateway.AuthEngine.Unittest
{
	public abstract class TestPINTANBase : TestWithServers
	{
		public DBQueries dbAccess;
		public AddFullDetails user;

		[SetUp]
		public new void SetupTest()
		{
			base.SetupTest();
			//this.dbAccess = new DBQueries(sc.GetSQLConnectionString());
			this.dbAccess = DBQueriesProvider.Get();
			//this.dbAccess.BeginTransaction();
		}

		[TearDown]
		public new void CleanTest()
		{
			//this.dbAccess.RollbackTransaction();
			base.CleanTest();
		}

		protected void setupPintanProvider(AddFullDetails user)
		{
			UserProvider up = new UserProvider()
			{
				Name = "PINTAN",
				Config = ","
					+ ""
					+ ","
			};
			SetUserProviderRet ret = (SetUserProviderRet)(aeServerLogic.SetUserProvider(
				new SetUserProvider() { User = user.User, Org = user.Org, Provider = up }));
			Assert.IsTrue(string.IsNullOrEmpty(ret.Error), "Error response is not empty: " + ret.Error);
			Assert.AreEqual(1, ret.Out);
		}

		public UserSheet setupUserSheet()
		{
			sc.AuthEngineLogLevel = Shared.Log.LogLevel.All;
			sc.AuthEngineDefaultEnabled = true;
			sc.AuthEngineUseEncryption = false;
			startAuthEngineServer();

			user = insertUser(string.Empty);

			int maxSheets = sc.PinTanMaxSheets;
			setupTestUserWithoutPincode(user);
			setupPintanProvider(user);
			var userId = dbAccess.GetUserId(user.User, user.Org);
			var userSheet = new UserSheet(dbAccess, userId);
			return userSheet;
		}


		public class TestUserSheets
		{
			protected TestPINTANBase testbase;
			protected UserSheet userSheet;
			protected SystemConfiguration sc;

			public TestUserSheets(SystemConfiguration sc, TestPINTANBase testBase)
			{
				this.sc = sc;
				this.testbase = testBase;
				this.userSheet = testBase.setupUserSheet();
			}

			public AddFullDetails User
			{
				get
				{
					return testbase.user;
				}
			}

			public void generateSheets()
			{
				userSheet.GenerateSheets(sc.PinTanMaxSheets, sc.PinTanRows,
				                         sc.PinTanColumns, sc.PintanPasscodeLength);
			}

			public string getCode(int col, int row) {
				return testbase.getUserSheetCodeColumn<string>("code", userSheet.GetCurrentSheetId(), col, row);
			}
		}
		
		public T getUserSheetCodeColumn<T>(string dbcolumn, long sheetId, int col, int row)
		{
			return dbAccess.QueryScalar<T>(
				string.Format(
					"SELECT {3} FROM " + DBQueries.USER_SHEET_CODE + " WHERE userSheetId='{0}' AND col='{1}' AND  row='{2}' ",
					sheetId, col, row, dbcolumn
				)
			);
		}
	}
	
	[TestFixture]
	public class TestAuthEnginePINTAN : TestPINTANBase
	{
		[Test]
		public void TestGenerateSheetsShouldGenerateAll()
		{
			Assert.AreEqual(sc.PinTanMaxSheets, setupUserSheet().GenerateSheets(
				sc.PinTanMaxSheets, sc.PinTanRows, sc.PinTanColumns, sc.PintanPasscodeLength
			).Count);
		}

		[Test]
		public void TestGenerateSheetsShouldGenerateNoneIfallWereGenerated()
		{
			Assert.AreEqual(sc.PinTanMaxSheets, setupUserSheet().GenerateSheets(
				sc.PinTanMaxSheets, sc.PinTanRows, sc.PinTanColumns, sc.PintanPasscodeLength).Count);
			Assert.AreEqual(0, setupUserSheet().GenerateSheets(
				sc.PinTanMaxSheets, sc.PinTanRows, sc.PinTanColumns, sc.PintanPasscodeLength).Count);
		}

		[Test]
		public void TestGenerateSheetsAndDisplayedShouldSetDisplayedAndUsed()
		{
			var userSheet = setupUserSheet();
			userSheet.GenerateSheets(1, sc.PinTanRows, sc.PinTanColumns, sc.PintanPasscodeLength);
			var sheetAndCodes = userSheet.GetNextSheetAndCodes();
			Assert.IsTrue(getUserSheetCodeColumn<bool>("displayed",
			                                           sheetAndCodes.SheetId, sheetAndCodes.CodeShownAtCol, sheetAndCodes.CodeShownAtRow
			                                          ));
			Assert.IsTrue(getUserSheetCodeColumn<bool>("used",
			                                           sheetAndCodes.SheetId, sheetAndCodes.CodeShownAtCol, sheetAndCodes.CodeShownAtRow
			                                          ));
			Assert.IsTrue(getUserSheetCodeColumn<bool>("displayed",
			                                           sheetAndCodes.SheetId, sheetAndCodes.CodeRequestedAtCol, sheetAndCodes.CodeRequestedAtRow
			                                          ));
			Assert.IsFalse(getUserSheetCodeColumn<bool>("used",
			                                            sheetAndCodes.SheetId, sheetAndCodes.CodeRequestedAtCol, sheetAndCodes.CodeRequestedAtRow
			                                           ));
		}

		[Test]
		public void TestDisplayedCodesNotUsedShouldBeInvalidatedOnGetNextSheetAndCodes()
		{
			var userSheet = setupUserSheet();
			userSheet.GenerateSheets(1, sc.PinTanRows, sc.PinTanColumns, sc.PintanPasscodeLength);
			var sheetAndCodes = userSheet.GetNextSheetAndCodes();
			userSheet.GetNextSheetAndCodes();
			Assert.IsTrue(getUserSheetCodeColumn<bool>("used",
			                                           sheetAndCodes.SheetId, sheetAndCodes.CodeRequestedAtCol, sheetAndCodes.CodeRequestedAtRow
			                                          ));
		}

		[Test]
		public void TestValidateUserFailNoSheets()
		{
			var userSheet = setupUserSheet();
			var OrgUser = user.Org + "\\" + user.User;
			ValidateUser vu = new ValidateUser();
			vu.User = OrgUser;
			ValidateUserRet vuret = (ValidateUserRet)aeServerLogic.Actioner.Do(vu);
			Assert.IsFalse(string.IsNullOrEmpty(vuret.Error), "Error response empty");
		}

		[Test]
		public void TestValidateUserFailNoPin()
		{
			var userSheet = setupUserSheet();
			userSheet.GenerateSheets(sc.PinTanMaxSheets, sc.PinTanRows, sc.PinTanColumns, sc.PintanPasscodeLength);
			var OrgUser = user.Org + "\\" + user.User;
			ValidateUser vu = new ValidateUser();
			vu.User = OrgUser;
			ValidateUserRet vuret = (ValidateUserRet)aeServerLogic.Actioner.Do(vu);
			Assert.IsTrue(string.IsNullOrEmpty(vuret.Error), "Error response is not empty: " + vuret.Error);
			Assert.IsFalse(string.IsNullOrEmpty(vuret.Extra));

			ValidatePin vp = new ValidatePin();
			vp.User = OrgUser;
			vp.Pin = string.Empty;
			vp.State = vuret.State;

			ValidatePinRet vpret = (ValidatePinRet)aeServerLogic.Actioner.Do(vp);
			Assert.IsFalse(string.IsNullOrEmpty(vpret.Error), "Error response is empty: ");
			Assert.IsFalse(vpret.Validated, "Validated is true");
		}

		[Test]
		public void TestValidateUser()
		{
			var userSheet = setupUserSheet();
			userSheet.GenerateSheets(sc.PinTanMaxSheets, sc.PinTanRows, sc.PinTanColumns, sc.PintanPasscodeLength);
			var OrgUser = user.Org + "\\" + user.User;

			for (var i = 0; i < 3; i++)
			{

				ValidateUser vu = new ValidateUser();
				vu.User = OrgUser;
				ValidateUserRet vuret = (ValidateUserRet)aeServerLogic.Actioner.Do(vu);
				Assert.IsTrue(string.IsNullOrEmpty(vuret.Error), "Error response is not empty: " + vuret.Error);
				Assert.IsFalse(string.IsNullOrEmpty(vuret.Extra));

				var data = TextHelper.ParsePipeML(vuret.Extra);
				var col = TextHelper.String2Number(data["C"].Substring(0, 1));
				var row = Convert.ToInt32(data["C"].Substring(1));

				ValidatePin vp = new ValidatePin();
				vp.User = OrgUser;
				vp.Pin = getUserSheetCodeColumn<string>("code", userSheet.GetCurrentSheetId(), col, row);
				vp.State = vuret.State;

				ValidatePinRet vpret = (ValidatePinRet)aeServerLogic.Actioner.Do(vp);
				Assert.IsTrue(string.IsNullOrEmpty(vpret.Error), "Error response is empty: ");
				Assert.IsTrue(vpret.Validated, "Validated is true");

			}
		}

		[Test]
		[ExpectedException(typeof(PINTANException))]
		public void TestConsumingAllCodeShouldThrowNoActiveSheet()
		{
			var cols = sc.PinTanColumns;
			var rows = sc.PinTanRows;
			var sheets = sc.PinTanMaxSheets;
			var userSheet = setupUserSheet();
			userSheet.GenerateSheets(sheets, sc.PinTanRows, sc.PinTanColumns, sc.PintanPasscodeLength);
			var roundsToAsk = (sheets * (cols * rows)) + 2;
			for (var i = 1; i <= roundsToAsk; i = i + 2)
			{
				userSheet.GetNextSheetAndCodes();
			}
		}

		[Test]
		public void TestResyncPINTANGenerateSheetShouldCreateAll()
		{
			var userSheet = setupUserSheet();

			ResyncHotp resync = new ResyncHotp();
			ResyncHotpRet resyncRet = null;
			resync.User = user.User;
			resync.Org = user.Org;

			resync.Action = PINTANProviderLogic.ACTION_GENERATESHEETS;
			resyncRet = (ResyncHotpRet)aeServerLogic.Actioner.Do(resync);
			Assert.AreEqual(sc.PinTanMaxSheets, resyncRet.Out);
		}

		[Test]
		public void TestResyncPINTANGenerateSheetShouldCreateRemaining()
		{
			var userSheet = setupUserSheet();
			var startSheets = sc.PinTanMaxSheets - 2;
			userSheet.GenerateSheets(startSheets, sc.PinTanRows, sc.PinTanColumns, sc.PintanPasscodeLength);

			ResyncHotp resync = new ResyncHotp();
			ResyncHotpRet resyncRet = null;
			resync.User = user.User;
			resync.Org = user.Org;

			resync.Action = PINTANProviderLogic.ACTION_GENERATESHEETS;
			resyncRet = (ResyncHotpRet)aeServerLogic.Actioner.Do(resync);
			Assert.AreEqual(sc.PinTanMaxSheets, startSheets + resyncRet.Out);
		}

		[Test]
		public void TestResyncPINTANGenerateSheetShouldCreateNoneIfAllCreated()
		{
			var userSheet = setupUserSheet();
			userSheet.GenerateSheets(sc.PinTanMaxSheets, sc.PinTanRows, sc.PinTanColumns, sc.PintanPasscodeLength);

			ResyncHotp resync = new ResyncHotp();
			ResyncHotpRet resyncRet = null;
			resync.User = user.User;
			resync.Org = user.Org;

			resync.Action = PINTANProviderLogic.ACTION_GENERATESHEETS;
			resyncRet = (ResyncHotpRet)aeServerLogic.Actioner.Do(resync);
			Assert.AreEqual(0, resyncRet.Out);
		}

		[Test]
		public void TestResyncPINTANGenerateSheetRegenerateInvalidSheetsToPINTAXMASSHEETS()
		{
			var userSheet = setupUserSheet();
			userSheet.GenerateSheets(sc.PinTanMaxSheets, sc.PinTanRows, sc.PinTanColumns, sc.PintanPasscodeLength);

			ResyncHotp resync = new ResyncHotp();
			ResyncHotpRet resyncRet = null;
			resync.User = user.User;
			resync.Org = user.Org;

			resync.Action = PINTANProviderLogic.ACTION_INVALIDATEALLSHEETS;
			resyncRet = (ResyncHotpRet)aeServerLogic.Actioner.Do(resync);
			Assert.AreEqual(sc.PinTanMaxSheets, resyncRet.Out);

			resync.Action = PINTANProviderLogic.ACTION_GENERATESHEETS;
			resyncRet = (ResyncHotpRet)aeServerLogic.Actioner.Do(resync);
			Assert.AreEqual(sc.PinTanMaxSheets, resyncRet.Out);
		}

		[Test]
		public void TestGetPINTANGeneratedSheetsInResyncExtra()
		{
			var userSheet = setupUserSheet();
			
			ResyncHotp resync = new ResyncHotp();
			ResyncHotpRet resyncRet = null;
			resync.User = user.User;
			resync.Org = user.Org;

			resync.Action = PINTANProviderLogic.ACTION_GENERATESHEETS;
			resyncRet = (ResyncHotpRet)aeServerLogic.Actioner.Do(resync);
			Assert.AreEqual(sc.PinTanMaxSheets, resyncRet.Out);
			Assert.IsFalse(string.IsNullOrEmpty(resyncRet.Extra));
			var data = TextHelper.ParsePipeML(resyncRet.Extra);
			Assert.AreEqual(sc.PinTanRows.ToString(), data["R"]);
			Assert.AreEqual(sc.PinTanColumns.ToString(), data["C"]);
			var sheets = 0;
			foreach (var kp in data)
			{
				if (kp.Key == "R" || kp.Key == "C")
					continue;
				Assert.IsFalse(string.IsNullOrEmpty(kp.Value));
				sheets++;
			}
			Assert.AreEqual(sc.PinTanMaxSheets, sheets);
		}
	}
}
