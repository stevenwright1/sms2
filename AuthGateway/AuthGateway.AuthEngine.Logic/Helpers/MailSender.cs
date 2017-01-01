using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using AuthGateway.Shared;

namespace AuthGateway.AuthEngine.Logic.Helpers
{
	public interface IMailSender
	{
		void Send(EmailConfig emailConfig, string email, string subject, string messageTemplate, string password);
		void Send(EmailConfig emailConfig, string password, string email, string subject, string message, string attachmentName, MemoryStream attachmentStream);
		bool IsValidAddress(string emailaddress);
	}
	
	public class MailSender : IMailSender
	{
		public void Send(EmailConfig emailConfig, string email, string subject, string messageTemplate, string password)
		{
			var mailMessage = new MailMessage(emailConfig.From, email, subject, messageTemplate);
			Send(emailConfig, mailMessage, password);
		}
		
		public void Send(EmailConfig emailConfig, string password, string email, string subject, string message, string attachmentName, MemoryStream attachmentStream)
		{
			var mailMessage = new MailMessage(emailConfig.From, email);
			mailMessage.Subject = subject;
			
			var attachment = new Attachment(attachmentStream, attachmentName);
			attachment.ContentId = Path.GetFileName(attachmentName).Replace(".", "");
			attachment.ContentDisposition.Inline = true;
			attachment.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
			
			mailMessage.Attachments.Add(attachment);
			mailMessage.IsBodyHtml = true;
			mailMessage.Body = message.Replace("{attachment}", attachment.ContentId);
			
			Send(emailConfig, mailMessage, password);
		}
		
		void Send(EmailConfig emailConfig, MailMessage mailMessage, string password) {
			using (var client = new SmtpClient(emailConfig.Server, emailConfig.Port))
			{
				client.UseDefaultCredentials = false;
				if (emailConfig.UseAuth)
					client.Credentials = new NetworkCredential(emailConfig.Username, password);
				client.EnableSsl = emailConfig.EnableSSL;
				client.Timeout = 5000;
				
				try {
					client.Send(mailMessage);
				} catch( SmtpException smtpException ) {                    
					if ( smtpException.InnerException == null ) {
						throw new CommunicationException(
							string.Format("E-mail could not be sent: ({0}) {1}", smtpException.StatusCode, smtpException.Message)
							, smtpException);
					}
					Exception innerException;
					do {
						innerException = smtpException.InnerException;
					} while (innerException.InnerException != null);				
					throw new CommunicationException(
						string.Format("MailSender Error - {0}", innerException.Message)
						, smtpException);
				}
			}
		}

		public bool IsValidAddress(string emailaddress)
		{
			try
			{
				MailAddress m = new MailAddress(emailaddress);
				return true;
			}
			catch (FormatException)
			{
				return false;
			}
		}
	}
}
