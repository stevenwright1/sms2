using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Text.RegularExpressions;
using AuthGateway.Shared;

namespace AuthGateway.MassUserSetup
{
	public class SmtpMailSender
	{
		private EmailConfig emailConfig;
		private EmailTemplate emailTemplate;

		public SmtpMailSender(EmailConfig emailConfig, EmailTemplate emailTemplate)
		{
			this.emailConfig = emailConfig;
			this.emailTemplate = emailTemplate;
		}

		public void Send(ToUser toUser)
		{
			Send(toUser, new Dictionary<string, Stream>());
		}

		public void Send(ToUser toUser, Dictionary<string, Stream> attachmentStreams)
		{
			var content = getContent(new Dictionary<string, string>()
			{
				{ "{$fullname}", toUser.Fullname },
				{ "{$username}", toUser.Username },
				{ "{$email}", toUser.Email }
			});

			var subject = emailTemplate.Subject;

			MailMessage mail = new MailMessage(emailConfig.From, toUser.Email);
			mail.IsBodyHtml = emailTemplate.IsHtml;
			mail.Subject = emailTemplate.Subject;
			mail.Body = content;

			foreach (var attachmentNameAndStream in attachmentStreams)
			{
				mail.Attachments.Add(new Attachment(
						attachmentNameAndStream.Value,
						attachmentNameAndStream.Key
					));
			}

			SmtpClient client = new SmtpClient();
			client.Host = emailConfig.Server;
			client.Port = emailConfig.Port;
			client.EnableSsl = emailConfig.EnableSSL;
			client.DeliveryMethod = SmtpDeliveryMethod.Network;
			client.UseDefaultCredentials = emailConfig.UseAuth;
			if (emailConfig.UseAuth)
			{
				client.Credentials = new System.Net.NetworkCredential(
					emailConfig.Username, 
					emailConfig.Password);
			}
			client.Send(mail);
		}

		private string getContent(Dictionary<string, string> replaces)
		{
			var content = this.emailTemplate.Content;
			foreach (var kp in replaces)
			{
				content = Regex.Replace(content,
					Regex.Escape(kp.Key),
					Regex.Escape(kp.Value),
					RegexOptions.IgnoreCase);
			}
			return content;
		}
	}
}
