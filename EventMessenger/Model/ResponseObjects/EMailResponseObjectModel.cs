/*
 * Created by SharpDevelop.
 * Date: 04.03.2017
 * Time: 21:13
 * 
 */
using System;
using System.Text;
using System.Net;
using System.Net.Mail;
using System.Security;
using System.Xml.Serialization;

namespace EventMessenger.Model.ResponseObjects
{
	/// <summary>
	/// Description of EMailInterface.
	/// </summary>
	public class EMailResponseObjectModel
	{
		private SmtpClient smtpClient;
		
		private string fromAddress;
		public string FromAddress
		{
			get { return fromAddress; }
			set {
				fromAddress = value;
				//mailMessageToSend.From.Address = value;
			}
		}
		
		private string toAddress;
		public string ToAddress
		{
			get { return toAddress; }
			set {
				toAddress = value;
				//mailMessageToSend.To[0].Address = value;
			}
		}
		
		private string subject;
		public string Subject
		{
			get { return subject; }
			set {
				subject = value;
				//mailMessageToSend.Subject = value;
			}
		}
		
		private string messageBody;
		public string MessageBody
		{
			get { return messageBody; }
			set {
				messageBody = value;
			}
		}
		
		
		private string smtpServerName;
		public string SMTPServerName
		{
			get { return smtpServerName; }
			set { smtpServerName = value; }
		}
		
		private int portNumber;
		public int PortNumber
		{
			get { return portNumber; }
			set { portNumber = value; }
		}
		
		private bool isSSLEnabled;
		public bool IsSSLEnabled
		{
			get { return isSSLEnabled; }
			set { isSSLEnabled = value; }
		}
		
		private bool useDefaultCredentials;
		public bool UseDefaultCredentials
		{
			get { return useDefaultCredentials; }
			set { useDefaultCredentials = value; }
		}
		
		public string UserName
		{
			get { return SMTPCredentials.UserName; }
			set {
				if(SMTPCredentials != null)
					SMTPCredentials.UserName = value;
				else
				{
					SMTPCredentials = new NetworkCredential();
					SMTPCredentials.UserName = value;
				}
				
			}
		}
		
		public string Password
		{
			get { return Security.SimpleEncryptWithPassword(SMTPCredentials.Password, "1q2w3e4r5t6z7u"); }
			set {
				SecureString pwd = new SecureString();
				
				Array.ForEach(Security.SimpleDecryptWithPassword(value, "1q2w3e4r5t6z7u").ToCharArray(), pwd.AppendChar);
				
				if(SMTPCredentials == null)
					SMTPCredentials = new NetworkCredential();
				
				SMTPCredentials.SecurePassword = pwd;
				
				

			}
		}
		
		private NetworkCredential smtpCredentials;
		[XmlIgnore]
		public NetworkCredential SMTPCredentials
		{
			get { return smtpCredentials; }
			set { smtpCredentials = value; }
		}
		
//		private MailMessage mailMessageToSend;
//		public MailMessage MailMessageToSend
//		{
//			get { return mailMessageToSend; }
//			private set {
//				FromAddress = (value as MailMessage).From.Address;
//				ToAddress = (value as MailMessage).To[0].Address;
//				Subject = (value as MailMessage).Subject;
//				MessageBody = (value as MailMessage).Body;
//
//				mailMessageToSend = value;
//			}
//		}
		
		/// <summary>
		/// 
		/// </summary>
		public EMailResponseObjectModel()
		{
		}
		
//		public EMailResponseObjectModel(string hostName, int port)
//		{
//			mailMessageToSend = new MailMessage();
//			smtpClient = new SmtpClient(hostName, port);
//		}
		
//		public void SendMessage(MailMessage message)
//		{
//			smtpClient.SendAsync(message, message.GetHashCode().ToString());
//		}

//		public void SendMessage(string from, string to, string subject, string message)
//		{
//			MailAddress fromMailAddr;
//			MailAddress toMailAddr;
//			MailMessage mailMessage;
//
//			if(from.IndexOf(',') != -1)
//			{
//				string fromAddr = from.Remove(from.IndexOf(','),from.Length);
//				string fromAddrDisplayAs = from.Remove(0,from.IndexOf(','));
//				fromMailAddr = new MailAddress(fromAddr,
//				                               fromAddrDisplayAs,
//				                               System.Text.Encoding.UTF8);
//			}
//			else
//			{
//				fromMailAddr = new MailAddress(from, from, System.Text.Encoding.UTF8);
//			}
//
//			toMailAddr = new MailAddress(to);
//
//			mailMessage = new MailMessage(fromMailAddr.Address,toMailAddr.Address, subject, message);
//			mailMessage.BodyEncoding = Encoding.UTF8;
//			mailMessage.SubjectEncoding = Encoding.UTF8;
//
//			smtpClient.UseDefaultCredentials = useDefaultCredentials;
//
//			if(smtpCredentials != null)
//			{
//				smtpClient.Credentials = smtpCredentials;
//			}
//
//			smtpClient.SendAsync(mailMessage, mailMessage.GetHashCode().ToString());
//
//		}
		
		/// <summary>
		/// 
		/// </summary>
		public async void SendMessage()
		{
			try{
				smtpClient = new SmtpClient(this.SMTPServerName, this.PortNumber);
				
				smtpClient.SendCompleted += (s, e) => {
					if(e.Error != null)
						LogWriter.CreateLogEntry(string.Format("{0}\n{1}",e.Error.Message, e.Error.InnerException != null ? e.Error.InnerException.Message : ""));
					//smtpClient.Dispose();
					//mailMessageToSend.Dispose();
				};
				
				MailAddress fromMailAddr;
				MailAddress toMailAddr;
				MailMessage mailMessage = new MailMessage();
				
				if(FromAddress.IndexOf(',') != -1)
				{
					string fromAddr = FromAddress.Remove(FromAddress.IndexOf(','),FromAddress.Length);
					string fromAddrDisplayAs = FromAddress.Remove(0,FromAddress.IndexOf(','));
					fromMailAddr = new MailAddress(fromAddr,
					                               fromAddrDisplayAs,
					                               System.Text.Encoding.UTF8);
				}
				else
				{
					fromMailAddr = new MailAddress(FromAddress, FromAddress, System.Text.Encoding.UTF8);
				}

				mailMessage.From = fromMailAddr;
				
				string[] receipents = ToAddress.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

				if(this.UseDefaultCredentials)
					smtpClient.UseDefaultCredentials = this.UseDefaultCredentials;
				else
				{
					smtpClient.UseDefaultCredentials = this.UseDefaultCredentials;
					smtpClient.Credentials = new NetworkCredential(this.SMTPCredentials.UserName, this.SMTPCredentials.SecurePassword);
				}

				smtpClient.EnableSsl = this.IsSSLEnabled;
				smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

				foreach(string receipent in receipents)
				{
					if(receipent.IndexOf(',') != -1)
					{
						string toAddr = receipent.Remove(receipent.IndexOf(','),receipent.Length);
						string toAddrDisplayAs = receipent.Remove(0,receipent.IndexOf(','));
						toMailAddr = new MailAddress(toAddr,
						                             toAddrDisplayAs,
						                             System.Text.Encoding.UTF8);
					}
					else
					{
						toMailAddr = new MailAddress(receipent, receipent, System.Text.Encoding.UTF8);
					}
					
					mailMessage.To.Add(toMailAddr);
					
				}
				mailMessage.Body = MessageBody;
				mailMessage.Subject = Subject;
				
				mailMessage.BodyEncoding = Encoding.UTF8;
				mailMessage.SubjectEncoding = Encoding.UTF8;
				
				
				await smtpClient.SendMailAsync(mailMessage);
			}
			
			catch(Exception e)
			{
					LogWriter.CreateLogEntry(string.Format("{0}; {1}; {2}",DateTime.Now, e.Message, e.InnerException.Message));
			}
		}
	}
}
