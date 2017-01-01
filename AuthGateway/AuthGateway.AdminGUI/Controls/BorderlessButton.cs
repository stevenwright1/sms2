using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
namespace AuthGateway.AdminGUI.Controls
{
	
	public class BorderlessButton : Button
	{

		private bool mouseOverButton = false;
		private Pen penForeColer;

		private Rectangle offByOneClientRectangle;
		
		public BorderlessButton() : base()
		{
			this.penForeColer = new Pen(this.ForeColor, 1);
			this.offByOneClientRectangle = this.ClientRectangle;
			
			MouseEnter += HandleMouseEnter;
			MouseLeave += HandleMouseLeave;
			LostFocus += HandleLostFocus;
			GotFocus += HandleGotFocus;
			this.FlatAppearance.BorderSize = 0;
		}

		protected override void OnPaint(PaintEventArgs pevent)
		{
			base.OnPaint(pevent);
			if ((this.BackgroundImage != null)) {
				pevent.Graphics.DrawImage(this.BackgroundImage, this.ClientRectangle);
			}
			if (this.mouseOverButton) {
				pevent.Graphics.DrawRectangle(this.penForeColer, this.offByOneClientRectangle);
			}
		}

		protected override bool ShowFocusCues {
			get { return false; }
		}

		private void HandleGotFocus(System.Object sender, System.EventArgs e)
		{
			this.HandleMouseEnter(sender, e);
		}

		private void HandleLostFocus(System.Object sender, System.EventArgs e)
		{
			this.HandleMouseLeave(sender, e);
		}

		private void HandleMouseLeave(System.Object sender, System.EventArgs e)
		{
			mouseOverButton = false;
		}

		private void HandleMouseEnter(System.Object sender, System.EventArgs e)
		{
			this.offByOneClientRectangle = this.ClientRectangle;
			this.offByOneClientRectangle.Width = this.offByOneClientRectangle.Width - 1;
			this.offByOneClientRectangle.Height = this.offByOneClientRectangle.Height - 1;
			this.penForeColer = new Pen(this.ForeColor, 1);
			mouseOverButton = true;
		}
	}
}
