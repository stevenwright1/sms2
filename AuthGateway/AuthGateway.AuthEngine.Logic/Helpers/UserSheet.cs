using System;
using System.Collections.Generic;
using System.Data;
using AuthGateway.AuthEngine.Logic.DAL;
using AuthGateway.AuthEngine.Logic.ProviderLogic;
using AuthGateway.Shared;
using AuthGateway.Shared.Log;
using AuthGateway.Shared.Tracking;

namespace AuthGateway.AuthEngine.Logic.Helpers
{
	public class UserSheet
	{
		private DBQueries dbAccess;
		private long userId;
		public UserSheet(DBQueries dbAccess, long userId)
		{
			this.dbAccess = dbAccess;
			this.userId = userId;
		}

		public SheetAndCodes GetNextSheetAndCodes()
		{
			long sheetId = GetCurrentSheetId();
			Logger.Instance.WriteToLog(string.Format("UserId: {0} Current Sheet: {1}", userId, sheetId), LogLevel.Debug);
			InvalidatePreviousDisplayedCodes(sheetId);

			DataTable dt = GetAvailableSheetCodes(sheetId);

			if (dt.Rows.Count < 2)
			{
				InvalidateSheet(sheetId);
				Logger.Instance.WriteToLog(string.Format("UserId: {0} Invalidated Sheet: {1}", userId, sheetId), LogLevel.Debug);
				sheetId = GetCurrentSheetId();
				InvalidatePreviousDisplayedCodes(sheetId);
				dt = GetAvailableSheetCodes(sheetId);
				if (dt.Rows.Count < 2)
					throw new PINTANException("Invalid amount of valid codes.");
			}

			int codeToShowRowIndex = CryptoRandom.Instance().Next(dt.Rows.Count);
			int codeToAskRowIndex = CryptoRandom.Instance().Next(dt.Rows.Count);
			while (codeToAskRowIndex == codeToShowRowIndex)
				codeToAskRowIndex = CryptoRandom.Instance().Next(dt.Rows.Count);

			var codeToShowRow = dt.Rows[codeToShowRowIndex];
			var codeToRequestRow = dt.Rows[codeToAskRowIndex];
			SetUserSheetCodeDisplayedAndUsed(Convert.ToInt32(codeToShowRow["id"]));
			SetUserSheetCodeDisplayed(Convert.ToInt32(codeToRequestRow["id"]));

			return new SheetAndCodes()
			{
				SheetId = sheetId,
				CodeShownAtCol = Convert.ToInt32(codeToShowRow["col"]),
				CodeShownAtRow = Convert.ToInt32(codeToShowRow["row"]),
				CodeShownAtIsCode = codeToShowRow["code"].ToString(),
				CodeRequestedAtCol = Convert.ToInt32(codeToRequestRow["col"]),
				CodeRequestedAtRow = Convert.ToInt32(codeToRequestRow["row"]),
			};
		}

		public long GetCurrentSheetId()
		{
			var parms = new List<DBQueryParm>();
			parms.Add(new DBQueryParm("userId", userId));

			long? sheetId = dbAccess.QueryScalar<long?>(
				"SELECT TOP 1 ID FROM "
				+ DBQueries.USER_SHEET
				+ " WHERE userId = @userId AND active=1 ORDER BY ID ASC",
				parms
				);
			if (sheetId == null || !sheetId.HasValue)
				throw new PINTANException("User has no active sheets.");
			return sheetId.Value;
		}

		private void InvalidatePreviousDisplayedCodes(long sheetId)
		{
			var parms = new List<DBQueryParm>();
			parms.Add(new DBQueryParm("sheetId", sheetId));

			dbAccess.NonQuery(
				" UPDATE "
				+ DBQueries.USER_SHEET_CODE
				+ " SET used=1 "
				+ " WHERE userSheetId=@sheetId AND displayed=1 AND used=0 ",
				parms
				);
		}

		private DataTable GetAvailableSheetCodes(long sheetId)
		{
			var parms = new List<DBQueryParm>();
			parms.Add(new DBQueryParm("sheetId", sheetId));

			var dt = dbAccess.Query(
				" SELECT * FROM "
				+ DBQueries.USER_SHEET_CODE
				+ " WHERE userSheetId=@sheetId AND displayed=0 AND used=0 ",
				parms
				);
			return dt;
		}

		public IList<long> GetActiveSheetIds()
		{
			var parms = new List<DBQueryParm>();
			parms.Add(new DBQueryParm("userId", userId));

			var ids = new List<long>();

			var dt = dbAccess.Query(
					"SELECT id FROM " + DBQueries.USER_SHEET + " WHERE userId = @userId AND active=1",
					parms
					);

			foreach (DataRow row in dt.Rows)
				ids.Add(Convert.ToInt64(row["ID"]));

			return ids;
		}

		public IList<long> InvalidateAllUserSheets()
		{
			var parms = new List<DBQueryParm>();
			parms.Add(new DBQueryParm("userId", userId));

			try
			{
				dbAccess.BeginTransaction();

				var ids = GetActiveSheetIds();

				var updates = dbAccess.NonQuery(
					string.Format(" UPDATE {0} SET active=0  WHERE userId=@userId AND active=1", DBQueries.USER_SHEET)
					, parms
					);

				dbAccess.CommitTransaction();

				return ids;
			}
			catch (Exception ex)
			{
				dbAccess.RollbackTransaction();
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
				return new List<long>();
			}
		}

		private void InvalidateSheet(long sheetId)
		{
			var parms = new List<DBQueryParm>();
			parms.Add(new DBQueryParm("id", sheetId));

			if (dbAccess.NonQuery(
				" UPDATE "
				+ DBQueries.USER_SHEET
				+ " SET active=0 "
				+ " WHERE id=@id ",
				parms
				) != 1)
				throw new Exception("Could not set sheet to inactive.");
		}

		private void SetUserSheetCodeDisplayedAndUsed(long userSheetCodeId)
		{
			var parms = new List<DBQueryParm>();
			parms.Add(new DBQueryParm("id", userSheetCodeId));

			dbAccess.NonQuery(
				" UPDATE "
				+ DBQueries.USER_SHEET_CODE
				+ " SET used=1, displayed=1 "
				+ " WHERE id=@id ",
				parms
				);
		}

		private void SetUserSheetCodeDisplayed(long userSheetCodeId)
		{
			var parms = new List<DBQueryParm>();
			parms.Add(new DBQueryParm("id", userSheetCodeId));

			dbAccess.NonQuery(
				" UPDATE "
				+ DBQueries.USER_SHEET_CODE
				+ " SET displayed=1 "
				+ " WHERE id=@id ",
				parms
				);
		}

		public List<SheetData> GenerateSheets(int maxSheets, int rows, int columns, int passcodeLen)
		{
			var ret = new List<SheetData>();
			try
			{
				//dbAccess.BeginTransaction();

				var currentSheets = GetCurrentSheetsCount();
				for (var i = currentSheets; i < maxSheets; i++)
				{
					var sheetId = insertUserSheet();
					ret.Add(GenerateUserSheetCodes(sheetId, rows, columns, passcodeLen));
				}
				//dbAccess.CommitTransaction();
				//return maxSheets - currentSheets;
				return ret;
			}
			catch (Exception ex)
			{
				Logger.Instance.WriteToLog(string.Format("GenerateSheet ERROR: {0}", ex.Message), LogLevel.Error);
				Logger.Instance.WriteToLog(string.Format("GenerateSheet STACK: {0}", ex.StackTrace), LogLevel.Debug);
				//dbAccess.RollbackTransaction();
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
				throw new PINTANException("Error generating sheet");
			}
		}

		private int GetCurrentSheetsCount()
		{
			var parms = new List<DBQueryParm>();
			parms.Add(new DBQueryParm("userId", userId));

			int sheetId = dbAccess.QueryScalar<int>(
				"SELECT COUNT(id) FROM "
				+ DBQueries.USER_SHEET
				+ " WHERE userId = @userId AND active=1",
				parms
				);
			return sheetId;
		}

		private long insertUserSheet()
		{
			var parms = new List<DBQueryParm>();
			parms.Add(new DBQueryParm("active", true));
			parms.Add(new DBQueryParm("userId", userId));

			decimal result = dbAccess.QueryScalar<decimal>(
				" INSERT INTO "
				+ DBQueries.USER_SHEET
				+ " (active,userId) "
				+ " VALUES(@active,@userId); "
				+ " SELECT SCOPE_IDENTITY(); "
				,
				parms
				);
			return Convert.ToInt64(result);
		}

		private SheetData GenerateUserSheetCodes(long sheetId, int rows, int columns, int passcodeLen)
		{
			var sd = new SheetData();
			sd.SheetId = sheetId;

			var maxCols = columns;
			var maxRows = rows;


			for (var row = 1; row <= maxRows; row++)
			{
				for (var col = 1; col <= maxCols; col++)
				{
					var code = RandomKeyGenerator.Generate(passcodeLen, RKGBase.Base32);
					insertUserSheetCode(sheetId, col, row, code);
					sd.Codes.Add(code);
				}
			}

			return sd;
		}

		private void insertUserSheetCode(long sheetId, int col, int row, string code)
		{
			var parms = new List<DBQueryParm>();
			parms.Add(new DBQueryParm("userSheetId", sheetId));
			parms.Add(new DBQueryParm("col", col));
			parms.Add(new DBQueryParm("row", row));
			parms.Add(new DBQueryParm("code", code));
			parms.Add(new DBQueryParm("displayed", false));
			parms.Add(new DBQueryParm("used", false));

			if (dbAccess.NonQuery(
				" INSERT INTO "
				+ DBQueries.USER_SHEET_CODE
				+ " (userSheetId,col,row,code,displayed,used) "
				+ " VALUES(@userSheetId,@col,@row,@code,@displayed,@used); "
				,
				parms
				) == 0)
				throw new Exception("Cannot insert user sheet code.");
		}

		public void CheckCode(string pin)
		{
			long sheetId = GetCurrentSheetId();

			var parms = new List<DBQueryParm>();
			parms.Add(new DBQueryParm("sheetId", sheetId));
			parms.Add(new DBQueryParm("code", pin));

			int? validated = dbAccess.QueryScalar<int?>(
				" SELECT 1 FROM "
				+ DBQueries.USER_SHEET_CODE
				+ " WHERE userSheetId=@sheetId AND displayed=1 AND used=0 AND code=@code ",
				parms
				);
			if (validated == null || !validated.HasValue)
				throw new PINTANException("Invalid pin.");
		}

		public int InvalidateUserSheets(IList<long> ids)
		{
			var parms = new List<DBQueryParm>();
			parms.Add(new DBQueryParm("userId", userId));

			return dbAccess.NonQuery(
				string.Format(
					" UPDATE {0} SET active=0 WHERE userId=@userId AND active=1 AND id IN ({1})"
					, DBQueries.USER_SHEET, String.Join(",", ids)
				)
				, parms
			);
		}
	}

	public class SheetData
	{
		public long SheetId { get; set; }
		public List<string> Codes { get; set; }

		public SheetData()
		{
			this.Codes = new List<string>();
		}
	}

	public class SheetAndCodes
	{
		public long SheetId { get; set; }
		public int CodeShownAtCol { get; set; }
		public int CodeShownAtRow { get; set; }
		public string CodeShownAtIsCode { get; set; }
		public int CodeRequestedAtCol { get; set; }
		public int CodeRequestedAtRow { get; set; }
	}
}
