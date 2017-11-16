/*
 * Created by SharpDevelop.
 * Date: 07/31/2017
 * Time: 21:50
 */

using EventMessenger.ViewModel;
using EventMessenger.EventObjects;
using EventMessenger.Model.ResponseObjects;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

using MvvmDialogs.ViewModels;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Linq;

using Telegram.Bot.Types;

namespace EventMessenger.ViewModel
{
	/// <summary>
	/// Description of ResponseConfigurationDialogTelegramBotConfigurationDialogViewModel.
	/// </summary>
	public class ResponseConfigurationDialogTelegramBotConfigurationDialogViewModel : ViewModelBase, IUserDialogViewModel
	{
		public ResponseConfigurationDialogTelegramBotConfigurationDialogViewModel()
		{
			IsModal = true;
		}
		
		#region Dialogs
		
		private ObservableCollection<IDialogViewModel> dialogs = new ObservableCollection<IDialogViewModel>();
		public ObservableCollection<IDialogViewModel> Dialogs { get { return dialogs; } }
		
		#endregion

		/// <summary>
		/// 
		/// </summary>
		public TelegramBotObjectModel TelegramBot {get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string StatusMessage
		{
			get { return statusMessage; }
			set { statusMessage = value;
				RaisePropertyChanged("StatusMessage");
			}
		} private string statusMessage;

		
		#region Commands
		/// <summary>
		/// 
		/// </summary>
		public ICommand ConnectToBot { get { return new RelayCommand(OnNewConnectToBotCommand); } }
		private void OnNewConnectToBotCommand()
		{
			
			try{

				TelegramBot = new TelegramBotObjectModel(TelegramToken);
				
				User bot = TelegramBot.CreateBot();
				
				if(!string.IsNullOrWhiteSpace(bot.Username))
					StatusMessage = string.Format("Connected to User ID: {0} ({1})",
					                              bot.Id.ToString(),bot.Username
					                             );
				else
					StatusMessage = string.Format("Not Connected");

			}

			
			
			catch (Exception e)
			{
				StatusMessage = string.Format("Error");
				new MessageBoxViewModel {
					Caption = "Message Box",
					Message = string.Format("Cant Connect to Telegram: \n{0} \n{1}",e.Message, e.InnerException == null ? "" : e.InnerException.InnerException.Message),
					Image = MessageBoxImage.Information
				}.Show(this.Dialogs);
			}
		}

		
		#endregion

		#region Localization
		
		public string LocalizationResourceSet { get; set; }
		
		#endregion
		
		/// <summary>
		/// 
		/// </summary>
		public string TelegramToken
		{
			get { return telegramToken; }
			set { telegramToken = value; }
		}private string telegramToken;
		
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
		
		public Action<ResponseConfigurationDialogTelegramBotConfigurationDialogViewModel> OnOk { get; set; }
		public Action<ResponseConfigurationDialogTelegramBotConfigurationDialogViewModel> OnCancel { get; set; }
		public Action<ResponseConfigurationDialogTelegramBotConfigurationDialogViewModel> OnCloseRequest { get; set; }

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
