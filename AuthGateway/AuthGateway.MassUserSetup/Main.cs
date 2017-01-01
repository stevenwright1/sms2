using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using AuthGateway.MassUserSetup.Steps;
using AuthGateway.Shared;

namespace AuthGateway.MassUserSetup
{
	public partial class Main : Form
	{
		private List<StepBase> steps;
		private StepBase currentStep = null;
		private StepBase priorStep = null;

		public SystemConfiguration Config { get; private set; }
		public List<AdUser> Users { get; private set; }
		public EmailTemplate EmailTemplate { get; private set; }

		public Main()
		{
			InitializeComponent();

			this.Config = new SystemConfiguration();
			this.Users = new List<AdUser>();
			this.EmailTemplate = new EmailTemplate()
			{
				Subject = "SMS2 - Token access",
				Content = @"Mr./Ms. {$fullname},

A new authentication step was added to our systems in order to provide you with better security measures.

Your current username is: {$username}

Please, follow the next instructions:
1.",
				IsHtml = false
			};

			steps = new List<StepBase>();
			steps.Add(new ConfigureAuthEngine(this));
			steps.Add(new ConfigureAD(this));
			steps.Add(new ConfigureEmail(this));
			steps.Add(new ConfigureTemplate(this));
			steps.Add(new ConfigureAuth(this));
			steps.Add(new EmailTest(this));
			steps.Add(new EmailSend(this));
			steps.Add(new ReviewResult());

			this.progressBar1.Value = 1;
			this.progressBar1.Maximum = steps.Count;
			this.progressBar1.Minimum = 1;
			NextStep();
		}

		private int currentStepIndex() {
			return steps.IndexOf(currentStep);
		}


		public void PreviousStep()
		{
			if (priorStep == null)
				return;
			if (currentStep != null)
			{
				currentStep.Hide();
				tlp.Controls.Remove(currentStep);
			}
			if (priorStep != null)
			{
				priorStep.Hide();
				currentStep = priorStep;
			}
			else
				return;
			var currentIndex = currentStepIndex();
			if (currentIndex == 0)
			{
				priorStep = null;
				btnBack.Enabled = false;
			}
			else
			{
				priorStep = steps[currentIndex - 1];
				btnBack.Enabled = true;
			}
			btnNext.Text = "Next";
			
			showNextStep();
			
			this.progressBar1.Value = currentStepIndex() + 1;
		}

		public void NextStep()
		{
			if (currentStep != null)
				currentStep.DoNext();
			priorStep = currentStep;
			if (currentStep == null)
			{
				currentStep = steps[0];
				btnBack.Enabled = false;
			}
			else
			{
				var currentIndex = currentStepIndex();
				btnBack.Enabled = true;
				if (currentIndex + 1 < steps.Count)
				{
					currentStep = steps[currentIndex + 1];
					if (currentStepIndex() == steps.Count - 1)
						btnNext.Text = "Finish";
					else
						btnNext.Text = "Next";
				}
				else
				{
					this.Close();
					return;
				}
			}
			if (priorStep != null)
			{
				priorStep.Hide();
				tlp.Controls.Remove(priorStep);
			}
			
			showNextStep();

			this.progressBar1.Value = currentStepIndex() + 1;
		}

		private void showNextStep()
		{
			currentStep.Dock = DockStyle.Fill;
			tlp.Controls.Add(currentStep, 0, 0);
			tlp.SetColumnSpan(currentStep, 2);
			currentStep.Show();
		}

		public void EnableNext(bool enable) {
			btnNext.Enabled = enable;
		}

		public void EnableBack(bool enable)
		{
			btnBack.Enabled = enable;
		}

		private void btnBack_Click(object sender, EventArgs e)
		{
			PreviousStep();
		}

		private void btnNext_Click(object sender, EventArgs e)
		{
			if (currentStep.CanDoNext())
				NextStep();
		}

		public void ShowErrors(List<string> errors)
		{
			this.InvokeIfRequired(() =>
			{
				var errorsAppend = new StringBuilder();
				errorsAppend.AppendLine("An error ocurred:");
				foreach (var error in errors)
				{
					errorsAppend.AppendFormat("{0}" + Environment.NewLine, error);
				}
				MessageBox.Show(this, errorsAppend.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, (MessageBoxOptions)8192);
			});
		}

		private void btnQuit_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		public IConfigurator Configurator { get; set; }
	}

	public class EmailTemplate
	{
		public string Subject { get; set; }
		public string Content { get; set; }
		public bool IsHtml { get; set; }
	}
}
