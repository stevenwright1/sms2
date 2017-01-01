using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using System.ComponentModel;

namespace AuthGateway.AdminGUI.Controls
{
    public class ComboBoxEx : ComboBox
    {
        public event CancelEventHandler SelectedIndexChanging;

        [Browsable(false)]
        public int LastAcceptedSelectedIndex { get; private set; }

        public ComboBoxEx()
        {
            LastAcceptedSelectedIndex = -1;
        }

        protected void OnSelectedIndexChanging(CancelEventArgs e)
        {
            var selectedIndexChanging = SelectedIndexChanging;
            if (selectedIndexChanging != null)
                selectedIndexChanging(this, e);
        }


        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            if (LastAcceptedSelectedIndex != SelectedIndex)
            {
                var cancelEventArgs = new CancelEventArgs();
                OnSelectedIndexChanging(cancelEventArgs);

                if (!cancelEventArgs.Cancel)
                {
                    LastAcceptedSelectedIndex = SelectedIndex;
                    base.OnSelectedIndexChanged(e);
                }
                else
                    SelectedIndex = LastAcceptedSelectedIndex;
            }
        }

    }
}
