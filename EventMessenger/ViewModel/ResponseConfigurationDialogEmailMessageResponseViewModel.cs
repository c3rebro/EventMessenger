/*
 * Created by SharpDevelop.
 * Date: 03/09/2017
 * Time: 22:05
 * 
 */
using EventMessenger.EventObjects;
using EventMessenger.Model.ResponseObjects;

using MvvmDialogs.ViewModels;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Mail;
using System.Windows.Input;
using System.Windows;



namespace EventMessenger.ViewModel
{
	/// <summary>
	/// Description of ResponseConfigurationDialogEmailMessageResponseViewModel.
	/// </summary>
	public class ResponseConfigurationDialogEMailMessageResponseViewModel : ViewModelBase, IUserDialogViewModel
	{
		private EMailResponseObjectModel emailObject;
		private NetworkCredential smtpCredential;
		private bool hasSMTPConfig;
		
		/// <summary>
		/// 
		/// </summary>
		public ResponseObjectModel ResponseObjectModel { get; set;	}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="_responseCollection"></param>
		/// <param name="_selectedItem"></param>
		public ResponseConfigurationDialogEMailMessageResponseViewModel(ObservableCollection<ResponseObjectModel> _responseCollection, ResponseObjectModel _selectedItem)
		{

			
			IsModal = true;
			
			if(_selectedItem != null)
			{
				ResponseObjectModel = _selectedItem;
				FromAddress = (_selectedItem.ResponseObject as EMailResponseObjectModel).FromAddress;
				ToAddress = (_selectedItem.ResponseObject as EMailResponseObjectModel).ToAddress;
				Subject = (_selectedItem.ResponseObject as EMailResponseObjectModel).Subject;
				
				emailObject = (_selectedItem.ResponseObject as EMailResponseObjectModel);

				if(
					!string.IsNullOrEmpty(emailObject.SMTPServerName) && 
					emailObject.PortNumber > 0 && 
					(emailObject.UseDefaultCredentials || !string.IsNullOrWhiteSpace(emailObject.SMTPCredentials.UserName)))
				{
					hasSMTPConfig = true;
				}
			}
			else
			{
				hasSMTPConfig = false;
				
				emailObject = new EMailResponseObjectModel();
				ResponseObjectModel = new ResponseObjectModel(emailObject);
			}

		}
		
		#region Dialogs
		
		/// <summary>
		/// 
		/// </summary>
		public ObservableCollection<IDialogViewModel> Dialogs { get { return dialogs; } }
		private ObservableCollection<IDialogViewModel> dialogs = new ObservableCollection<IDialogViewModel>();
		
		#endregion
		
		#region Commands
		
		/// <summary>
		/// 
		/// </summary>
		public ICommand SMTPConfigurationCommand { get { return new RelayCommand(OnNewSMTPConfigurationCommand); }}
		private void OnNewSMTPConfigurationCommand()
		{
			this.Dialogs.Add(new ResponseConfigurationDialogEMailResponseSMTPSettingsViewModel(SelectedEmail)
			                 {
			                 	OnOk = (sender) =>
			                 	{
			                 		if(!sender.UseDefaultCredentials && sender.Password.Length > 0 && !string.IsNullOrWhiteSpace(sender.ServerName))
			                 		{
			                 			smtpCredential = new NetworkCredential(sender.Username, sender.Password);
			                 			smtpCredential.Domain = sender.ServerName;
			                 			emailObject.SMTPCredentials = smtpCredential;
			                 		}

			                 		emailObject.PortNumber = Convert.ToInt32(sender.PortNumber);
			                 		emailObject.SMTPServerName = sender.ServerName;
			                 		emailObject.IsSSLEnabled = sender.SSLEnabled;
			                 		emailObject.UseDefaultCredentials = sender.UseDefaultCredentials;
			                 		if(!string.IsNullOrEmpty(sender.ServerName) && !string.IsNullOrEmpty(sender.PortNumber))
			                 		{
			                 			hasSMTPConfig = true;
			                 		}

			                 		else
			                 			hasSMTPConfig = false;
			                 		sender.Close();
			                 	},

			                 	OnCancel = (sender) =>
			                 	{
			                 		sender.Close();
			                 	},
			                 	
			                 	OnCloseRequest = (sender) => {
			                 		sender.Close();
			                 	}
			                 });
		}
		
		/// <summary>
		/// 
		/// </summary>
		public ICommand AddMessageCommand { get { return new RelayCommand(OnNewAddMessageCommand); }}
		private void OnNewAddMessageCommand()
		{
			try
			{
				if(!hasSMTPConfig)
					throw new MissingMemberException();
				
				if(EMailCollection == null)
					EMailCollection = new ObservableCollection<EMailResponseObjectModel>();
				
				emailObject.FromAddress = FromAddress;
				emailObject.ToAddress = ToAddress;
				emailObject.MessageBody = MessageBody;
				emailObject.Subject = Subject;
				
				if(!EMailCollection.Contains(emailObject))
				{
					EMailCollection.Add(emailObject);
					ResponseObjectModel.HasConfiguration = true;
				}
			}
			
			catch(Exception mailAddEx)
			{
				if(mailAddEx.GetType() == typeof(MissingMemberException))
				{
					if (new MessageBoxViewModel {
					    	Caption = "No SMTP Server configured",
					    	Message = "Are your sure to proceed without any smtp configuration? 'localhost' will be used instead",
					    	Buttons = MessageBoxButton.YesNo,
					    	Image = MessageBoxImage.Question
					    }
					    .Show(this.Dialogs) == MessageBoxResult.Yes)
					{
						emailObject.SMTPServerName = "localhost";
						emailObject.UseDefaultCredentials = true;
						hasSMTPConfig = true;
					}

				}
				
				else
				{
					
					new MessageBoxViewModel {
						Caption = "Unconfigured Response",
						Message = string.Format("{0} \n{1}",mailAddEx.Message, mailAddEx.InnerException == null ? "" : mailAddEx.InnerException.ToString()),
						Buttons = MessageBoxButton.OK,
						Image = MessageBoxImage.Exclamation
					}
					.Show(this.Dialogs);
				}
			}
			Ok();
		}
		#endregion
		
		#region Localization
		
		/// <summary>
		/// 
		/// </summary>
		public string Caption
		{
			get {
				using (var resMan = new ResourceLoader())
				{
					return resMan.getResource("windowCaptionResponseConfigurationDialogEMailResponse");
				}
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		public string LocalizationResourceSet { get; set; }
		
		#endregion
		
		#region ItemsCollections
		
		/// <summary>
		/// 
		/// </summary>
		public EMailResponseObjectModel SelectedEmail
		{
			get { return selectedEmail; }
			set {
				selectedEmail = value;
				RaisePropertyChanged("SelectedEmail");
			}
		} private EMailResponseObjectModel selectedEmail;
		
		/// <summary>
		/// 
		/// </summary>
		public ObservableCollection<EMailResponseObjectModel> EMailCollection
		{
			get { return emailCollection; }
			set { emailCollection = value; RaisePropertyChanged("EMailCollection"); }
		} private ObservableCollection<EMailResponseObjectModel> emailCollection;

		/// <summary>
		/// 
		/// </summary>
		public string FromAddress
		{
			get { return fromAddress; }
			set { fromAddress = value; }
		} private string fromAddress;
		
		/// <summary>
		/// 
		/// </summary>
		public string ToAddress
		{
			get { return toAddress; }
			set { toAddress = value; }
		} private string toAddress;
		
		/// <summary>
		/// 
		/// </summary>
		public string Subject
		{
			get { return subject; }
			set { subject = value; }
		} private string subject;
		
		/// <summary>
		/// 
		/// </summary>
		public string MessageBody
		{
			get { return messageBody; }
			set { messageBody = value; }
		} private string messageBody;
		
		#endregion
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
		
		/// <summary>
		/// 
		/// </summary>
		public ICommand CancelCommand { get { return new RelayCommand(Cancel); } }
		protected virtual void Cancel()
		{
			if (this.OnCancel != null)
				this.OnCancel(this);
			else
				Close();
		}
		
		/// <summary>
		/// 
		/// </summary>
		public Action<ResponseConfigurationDialogEMailMessageResponseViewModel> OnOk { get; set; }
		
		/// <summary>
		/// 
		/// </summary>
		public Action<ResponseConfigurationDialogEMailMessageResponseViewModel> OnCancel { get; set; }
		
		/// <summary>
		/// 
		/// </summary>
		public Action<ResponseConfigurationDialogEMailMessageResponseViewModel> OnCloseRequest { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public void Close()
		{
			if (this.DialogClosing != null)
				this.DialogClosing(this, new EventArgs());
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="collection"></param>
		public void Show(IList<IDialogViewModel> collection)
		{
			collection.Add(this);
		}
		
		#endregion IUserDialogViewModel Implementation
	}
}
