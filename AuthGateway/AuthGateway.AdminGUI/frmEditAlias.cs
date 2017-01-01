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
    public partial class frmEditAlias : Form
    {
        private string domain;        
        private List<string> aliases;
        private string oldAlias;
        public string EditedAlias { get; private set; }
        public frmEditAlias(string domain, string alias, List<string> aliases)
        {
            InitializeComponent();
            this.domain = domain;            
            this.aliases = aliases;
            this.Text = string.Format("Edit Alias for Domain '{0}'", domain);
            oldAlias = alias;
            txtAlias.Text = oldAlias;
        }

        private void txtAlias_TextChanged(object sender, EventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            btnOk.Enabled = textBox.Text != String.Empty && textBox.Text != oldAlias;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            string alias = txtAlias.Text;
            if (!aliases.Contains(alias))
            {
                EditedAlias = alias;
            }
            else
            {
                MessageBox.Show(this, string.Format("Domain '{0}' already has alias '{1}'", domain, alias));
                this.DialogResult = System.Windows.Forms.DialogResult.None;
            }
        }
    }
}
