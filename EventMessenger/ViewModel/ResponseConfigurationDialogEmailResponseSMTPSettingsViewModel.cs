/*
 * Created by SharpDevelop.
 * Date: 03/09/2017
 * Time: 22:43
 * 
 */
using EventMessenger.Model.ResponseObjects;

using GalaSoft;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

using MvvmDialogs.ViewModels;

using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows.Input;
using System.Security;

namespace EventMessenger.ViewModel
{
	/// <summary>
	/// Description of ResponseConfigurationDialogEmailResponseSMTPSettingsViewModel.
	/// </summary>
	public class ResponseConfigurationDialogEMailResponseSMTPSettingsViewModel : ViewModelBase, IUserDialogViewModel
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="model"></param>
		public ResponseConfigurationDialogEMailResponseSMTPSettingsViewModel(EMailResponseObjectModel model)
		{
			if(model == null)
			{
				IsModal = true;
				portNumber = 587; // initialize default SMTP Port
				sslEnabled = true;
			}
			else
			{
				IsModal = true;
				portNumber = model.PortNumber;
				sslEnabled = model.IsSSLEnabled;
				ServerName = model.SMTPServerName;
				UseDefaultCredentials = model.UseDefaultCredentials;
				Username = model.UserName;
			}

			Messenger.Default.Register<NotificationMessage<SecureString>>(
				this, nm => {
					if(nm.Notification == "SMTPPassword")
						password = nm.Content;
				});
			
		}
		
		#region Localization
		
		public string Caption
		{
			get {
				using (var resMan = new ResourceLoader())
				{
					return resMan.getResource("windowCaptionResponseConfigurationDialogEMailResponse");
				}
			}
		}
		
		public string LocalizationResourceSet { get; set; }
		
		#endregion
		
		private string serverName;
		public string ServerName
		{
			get { return serverName; }
			set { serverName = value; }
		}
		
		private int portNumber;
		public string PortNumber
		{
			get { return Convert.ToString(portNumber); }
			set { portNumber = Convert.ToInt32(value); }
		}
		
		private bool sslEnabled;
		public bool SSLEnabled
		{
			get { return sslEnabled; }
			set { sslEnabled = value; }
		}
		
		private bool useDefaultCredentials;
		public bool UseDefaultCredentials
		{
			get { return useDefaultCredentials; }
			set { useDefaultCredentials = value; }
		}
		
		private string username;
		public string Username
		{
			get { return username; }
			set { username = value; }
		}
		
		private SecureString password;
		public SecureString Password
		{
			get { return password; }
			set { password = value; }
		}
		
		#region IUserDialogViewModel Implementation

		public bool IsModal { get; private set; }
		public virtual void RequestClose()
		{
			if (this.OnCloseRequest != null)
				OnCloseRequest(this);
			else
				Close();
		}
		public event EventHandler DialogClosing;

		public ICommand OKCommand { get { return new RelayCommand(Ok); } }
		protected virtual void Ok()
		{
			if (this.OnOk != null)
				this.OnOk(this);
			else
				Close();
		}
		
		public ICommand CancelCommand { get { return new RelayCommand(Cancel); } }
		protected virtual void Cancel()
		{
			if (this.OnCancel != null)
				this.OnCancel(this);
			else
				Close();
		}
		
		public Action<ResponseConfigurationDialogEMailResponseSMTPSettingsViewModel> OnOk { get; set; }
		public Action<ResponseConfigurationDialogEMailResponseSMTPSettingsViewModel> OnCancel { get; set; }
		public Action<ResponseConfigurationDialogEMailResponseSMTPSettingsViewModel> OnCloseRequest { get; set; }

		public void Close()
		{
			if (this.DialogClosing != null)
				this.DialogClosing(this, new EventArgs());
		}

		public void Show(IList<IDialogViewModel> collection)
		{
			collection.Add(this);
		}
		
		#endregion IUserDialogViewModel Implementation
		
	}
}
