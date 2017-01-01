using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using AuthGateway.AdminGUI.Helpers;

namespace AuthGateway.AdminGUI
{
    public partial class frmImageConfirmation : Form
    {       
        public frmImageConfirmation(string leftImageUrl, string rightImageUrl)
        {
            InitializeComponent();
            //pbLeft.Image = ImagingHelper.GetImageFromUrl(leftImageUrl);
            //pbRight.Image = ImagingHelper.GetImageFromUrl(rightImageUrl);
        }
    }
}
