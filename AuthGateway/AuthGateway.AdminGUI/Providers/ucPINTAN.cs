using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.Linq;

using AuthGateway.Shared;
using AuthGateway.Shared.Tracking;
using AuthGateway.Shared.XmlMessages.Response.Ret.AuthEngine;
using MigraDoc;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;

namespace AuthGateway.AdminGUI
{

	public partial class ucPINTAN : ProviderConfig
	{

		private ProviderConfigContainer pcContainer;
		public ucPINTAN(ProviderConfigContainer pcCont) : this()
		{
			this.pcContainer = pcCont;
			this.Parent = this.pcContainer.getControl();
			this.pcContainer.getControl().Controls.Add(this);
			this.Dock = DockStyle.Fill;
		}

		public ucPINTAN()
		{
			// This call is required by the designer.
			InitializeComponent();

			// Add any initialization after the InitializeComponent() call.
		}

		public string getConfig()
		{
			return string.Empty;
		}


		public void loadConfig(string config)
		{
		}

		public string getName()
		{
			return "PINTAN";
		}

		private string friendlyName = string.Empty;
		public string getFriendlyName()
		{
			if (string.IsNullOrEmpty(friendlyName))
				return "PIN/TAN";
			return friendlyName;
		}
		public void setFriendlyName(string name)
		{
			friendlyName = name;
		}

		public void ShowConfig()
		{
			this.Show();
		}

		public void HideConfig()
		{
			this.Hide();
		}

		private void btnGenerateSheets_Click(System.Object sender, System.EventArgs e)
		{
			if (!this.pcContainer.isSelectedProvider(this.getName())) {
				this.pcContainer.ShowError("Please activate this provider by clicking Save Configuration before using this functionality");
				return;
			}

			Variables clientLogic = this.pcContainer.getClientLogic();
			ResyncHotpRet ret = clientLogic.ResyncProvider(pcContainer.getUser(), pcContainer.getDomain(), "GenerateSheets", string.Empty, string.Empty, string.Empty);
			if (!string.IsNullOrEmpty(ret.Error)) {
				this.pcContainer.ShowError(ret.Error);
				return;
			}

			if (ret.Out == 0) {
				this.pcContainer.ShowWarning("No new sheets were generated. If needed, invalidate prior ones first.");
				return;
			}

			Dictionary<string, string> data = TextHelper.ParsePipeML(ret.Extra);
			Document doc = ProcessPINTANData(data);

			try {
				PdfDocumentRenderer renderer = this.GetDocumentPdfRenderer(doc);
				SaveFileDialog ofd = new SaveFileDialog();
				ofd.Filter = "PDF files (*.pdf)|*.pdf|All files (*.*)|*.*";
				ofd.AddExtension = false;
				ofd.DefaultExt = "pdf";
				if (ofd.ShowDialog() != DialogResult.OK) {
					MessageBox.Show("Sheets not saved, invalidate them to be able to generate new ones.", Configs.AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					return;
				}
				renderer.Save(ofd.FileName);
			} catch (Exception ex) {
				AuthGateway.Shared.Log.Logger.Instance.WriteToLog(string.Format("GenerateSheets ERROR: {0}", ex.Message), AuthGateway.Shared.Log.LogLevel.Error);
				AuthGateway.Shared.Log.Logger.Instance.WriteToLog(string.Format("GenerateSheets STACK: {0}", ex.StackTrace), AuthGateway.Shared.Log.LogLevel.Debug);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
				throw;
			}
		}


		public void BeforeSave()
		{
		}

		public string PostSaveMessage()
		{
			Variables clientLogic = this.pcContainer.getClientLogic();
			ResyncHotpRet ret = clientLogic.ResyncProvider(pcContainer.getUser(), pcContainer.getDomain(), "GetActiveSheets", "", "", "");
			if (string.IsNullOrEmpty(ret.Error) && string.IsNullOrEmpty(ret.Extra)) {
				MessageBox.Show("You have no active code sheets and will not be able to login." + Environment.NewLine + "Please use Generate Sheets button.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
			return string.Empty;
		}

		private void btnInvalidateAllSheets_Click(System.Object sender, System.EventArgs e)
		{
			if (!this.pcContainer.isSelectedProvider(this.getName())) {
				this.pcContainer.ShowError("Please activate this provider by clicking Save Configuration before using this functionality");
				return;
			}

			Variables clientLogic = this.pcContainer.getClientLogic();
			ResyncHotpRet ret = clientLogic.ResyncProvider(pcContainer.getUser(), pcContainer.getDomain(), "InvalidateAllSheets", string.Empty, string.Empty, string.Empty);
			if (string.IsNullOrEmpty(ret.Error)) {
				string invalidated = string.Empty;
				if (!string.IsNullOrEmpty(ret.Extra)) {
					invalidated = Environment.NewLine;
					string[] sheetIdsStrings = ret.Extra.Split(new char[] { ',' });
					foreach (string sheetIdString in sheetIdsStrings) {
						invalidated = invalidated + string.Format("Sheet Id: {0}" + Environment.NewLine, sheetIdString);
					}
				}
				MessageBox.Show(string.Format("'{0}' active sheets were invalidated." + invalidated, ret.Out), "WrightCCS", MessageBoxButtons.OK, MessageBoxIcon.Information);
			} else {
				MessageBox.Show(ret.Error, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		public Document ProcessPINTANData(Dictionary<string, string> data)
		{
			MigraDoc.DocumentObjectModel.Document doc = new MigraDoc.DocumentObjectModel.Document();
			MigraDoc.DocumentObjectModel.Section sec = doc.AddSection();
			doc.DefaultPageSetup.PageFormat = PageFormat.A4;
			PageSetup ps = sec.PageSetup;
			int rows = 0;
			int columns = 0;
			List<SheetData> sheetsWithCodes = new List<SheetData>();

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
				Table table = sec.AddTable();

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

		public PdfDocumentRenderer GetDocumentPdfRenderer(Document doc)
		{
			PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(true);
			pdfRenderer.Document = doc;
			pdfRenderer.RenderDocument();
			return pdfRenderer;
		}

		private class SheetData
		{
			public long SheetId { get; set; }
			public List<string> Codes { get; set; }
			public SheetData()
			{
				this.Codes = new List<string>();
			}
		}

		private void btnInvalidateSheets_Click(System.Object sender, System.EventArgs e)
		{
			if (!this.pcContainer.isSelectedProvider(this.getName())) {
				this.pcContainer.ShowError("Please activate this provider by clicking Save Configuration before using this functionality");
				return;
			}

			frmInvalidateSheets frm = new frmInvalidateSheets(this.pcContainer);
			frm.ShowDialog();
		}


		public void validateBeforeSave()
		{
		}
	}
}
