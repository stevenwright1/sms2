using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AuthGateway.AdminGUI
{
    public partial class frmNewAlias : Form
    {
        private string domain;
        private List<string> aliases;
        public string NewAlias { get; private set; }

        public frmNewAlias(string domain, List<string> aliases)
        {
            InitializeComponent();
            this.domain = domain;
            this.aliases = aliases;
            this.Text = string.Format("New Alias for Domain '{0}'", domain);
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            string alias = txtAlias.Text;
            if (!aliases.Contains(alias))
            {
                NewAlias = alias;
                MessageBox.Show(this, string.Format("Domain alias '{0}' will become active after saving and polling AD users", alias));
            }
            else
            {
                MessageBox.Show(this, string.Format("Domain '{0}' already has alias '{1}'", domain, alias));
                this.DialogResult = System.Windows.Forms.DialogResult.None;                
            }
        }

        private void txtAlias_TextChanged(object sender, EventArgs e)
        {            
            TextBox textBox = (TextBox)sender;
            btnOk.Enabled = textBox.Text != String.Empty;
        }
    }
}
