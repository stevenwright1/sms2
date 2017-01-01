using System;
using System.Windows.Forms;

namespace AuthGateway.AdminGUI.Controls
{
	/**
	 * Author: esskar
	 * URL: http://stackoverflow.com/a/8001842
	 * 
	 */
	public class DelayedTextBox : TextBox
	{
		Timer m_delayedTextChangedTimer;

		public event EventHandler DelayedTextChanged;

		public DelayedTextBox()
		{
			this.DelayedTextChangedTimeout = 500; // 1/2 second
		}

		protected override void Dispose(bool disposing)
		{
			if (m_delayedTextChangedTimer != null)
			{
				m_delayedTextChangedTimer.Stop();
				if (disposing)
					m_delayedTextChangedTimer.Dispose();
			}

			base.Dispose(disposing);
		}

		public int DelayedTextChangedTimeout { get; set; }

		protected virtual void OnDelayedTextChanged(EventArgs e)
		{
			if (DelayedTextChanged != null)
				DelayedTextChanged(this, e);
		}

		protected override void OnTextChanged(EventArgs e)
		{
			InitializeDelayedTextChangedEvent();
			base.OnTextChanged(e);
		}

		void InitializeDelayedTextChangedEvent()
		{
			if (m_delayedTextChangedTimer != null)
				m_delayedTextChangedTimer.Stop();

			if (m_delayedTextChangedTimer == null || m_delayedTextChangedTimer.Interval != this.DelayedTextChangedTimeout)
			{
				m_delayedTextChangedTimer = new Timer();
				m_delayedTextChangedTimer.Tick += HandleDelayedTextChangedTimerTick;
				m_delayedTextChangedTimer.Interval = DelayedTextChangedTimeout;
			}

			m_delayedTextChangedTimer.Start();
		}

		void HandleDelayedTextChangedTimerTick(object sender, EventArgs e)
		{
			Timer timer = sender as Timer;
			timer.Stop();

			OnDelayedTextChanged(EventArgs.Empty);
		}

	}

}