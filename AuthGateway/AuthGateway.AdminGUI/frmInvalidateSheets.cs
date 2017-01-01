using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.Linq;

using AuthGateway.Shared.Tracking;
using AuthGateway.Shared.XmlMessages.Response.Ret.AuthEngine;
using AuthGateway.Shared.XmlMessages.Request.Command.AuthEngine;

namespace AuthGateway.AdminGUI
{

	public partial class frmInvalidateSheets
	{
		private ProviderConfigContainer pcContainer;

		private Variables clientLogic;

		public frmInvalidateSheets(ProviderConfigContainer pcConfig)
		{
			Load += frmResyncToken_Load;
			// This call is required by the designer.
			InitializeComponent();

			// Add any initialization after the InitializeComponent() call.
			this.pcContainer = pcConfig;

			this.clientLogic = pcContainer.getClientLogic();
		}

		private void frmResyncToken_Load(System.Object sender, System.EventArgs e)
		{
			ResyncHotpRet ret = this.clientLogic.ResyncProvider(pcContainer.getUser(), pcContainer.getDomain(), "GetActiveSheets", "", "", "");
			if (!string.IsNullOrEmpty(ret.Error)) {
				MessageBox.Show(ret.Error, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				this.Close();
				return;
			}

			if (string.IsNullOrEmpty(ret.Extra)) {
				MessageBox.Show("No active sheets were found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				this.Close();
				return;
			}

			this.clbSheets.Items.AddRange(ret.Extra.Split(new char[] { ',' }));
		}

		private void btnResync_Click(System.Object sender, System.EventArgs e)
		{

			try {
				Variables clientLogic = this.pcContainer.getClientLogic();
				List<string> idsToInvalidate = new List<string>();

				foreach (string sheetId in this.clbSheets.CheckedItems) {
					idsToInvalidate.Add(sheetId);
				}

				if (idsToInvalidate.Count == 0) {
					MessageBox.Show("No sheets were selected, no action was taken.", "WrightCCS", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					return;
				}

				ResyncHotpRet ret = clientLogic.ResyncProvider(pcContainer.getUser(), pcContainer.getDomain(), "InvalidateSheets", string.Join(",", idsToInvalidate), string.Empty, string.Empty);
				if (!string.IsNullOrEmpty(ret.Error)) {
					throw new Exception(ret.Error);
				}
				MessageBox.Show(string.Format("'{0}' active sheets were invalidated.", ret.Out), "WrightCCS", MessageBoxButtons.OK, MessageBoxIcon.Information);
			} catch (Exception ex) {
				MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Tracker.Instance.TrackException(this.GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex);
			} finally {
				this.Close();
			}
		}
	}
}
