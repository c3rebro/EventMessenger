/*
 * Created by SharpDevelop.
 * Date: 03/04/2017
 * Time: 21:45
 * 
 */
using EventMessenger.ViewModel;
using EventMessenger.EventObjects;
using EventMessenger.Model.ResponseObjects;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using MvvmDialogs.ViewModels;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Linq;

namespace EventMessenger.ViewModel
{
	/// <summary>
	/// Description of ResponseConfigurationDialogTelegramMessageResponseViewModel.
	/// </summary>
	public class ResponseConfigurationDialogTelegramMessageResponseViewModel : ViewModelBase, IUserDialogViewModel
	{
		
		#region fields
		
		public ResponseObjectModel ResponseObjectModel { get; set;	}
		private TelegramBotObjectModel telegramBot;
		private IEnumerable<TelegramMessageObjectModel> messagesWithBot;
		
		private ResourceLoader resLoader;
		#endregion

		/// <summary>
		/// 
		/// </summary>
		/// <param name="_staticEvents"></param>
		/// <param name="_responseCollection"></param>
		/// <param name="_responseObjectModel"></param>
		public ResponseConfigurationDialogTelegramMessageResponseViewModel(ObservableCollection<EventObjectModel> _staticEvents, ObservableCollection<ResponseObjectModel> _responseCollection, ResponseObjectModel _responseObjectModel)
		{
			resLoader = new ResourceLoader();
			
			ResponseObjectModel = _responseObjectModel;
			responseCollection = _responseCollection;
			
			StatusMessage = string.Format("Not Connected");
			
			if(ResponseObjectModel != null && ResponseObjectModel.ResponseObject != null)
			{
				if(ResponseObjectModel.ResponseObject is TelegramMessageObjectModel)
				{
					TelegramMessageObject = ResponseObjectModel.ResponseObject as TelegramMessageObjectModel;
					
					if(TelegramMessageObject.Bot2Use.TelegramUsers.Where(x => x.UserStatus == TelegramUserStatus.UserIsAdmin).Select(x => x.UserStatus == TelegramUserStatus.UserIsAdmin).Single())
					{
						StatusMessage = string.Format("Online with ChatID: {0}",TelegramMessageObject.Bot2Use.TelegramUsers.Where(x => x.UserStatus == TelegramUserStatus.UserIsAdmin).Select(x => x.chatID).Single());
						
						TelegramBotObjectCollection = new ObservableCollection<TelegramBotObjectModel>();
						TelegramBotObjectCollection.Add(TelegramMessageObject.Bot2Use);
						SelectedTelegramBotObject = TelegramMessageObject.Bot2Use;
						ResponseObjectModel.HasConfiguration = true;
					}
				}
			}
			else
			{
				telegramBot = new TelegramBotObjectModel();
				TelegramMessageObject = new TelegramMessageObjectModel(telegramBot);
				ResponseObjectModel = new ResponseObjectModel(TelegramMessageObject);
				
				messagesWithBot = from q in responseCollection
					where !string.IsNullOrEmpty((q.ResponseObject as TelegramMessageObjectModel).Bot2Use.BotName)
					select (q.ResponseObject as TelegramMessageObjectModel);
				
				var query
					= (from q in (from p in responseCollection where (p.ResponseObject is TelegramMessageObjectModel) select (p.ResponseObject as TelegramMessageObjectModel).Bot2Use).Select(p => p).Distinct() where q.BotName != null select q);
				
				foreach(EventObjectModel eventObject in _staticEvents)
				{
					foreach(ResponseObjectModel responseObjectModel in from x in eventObject.ResponseCollection where x.ResponseObject is TelegramMessageObjectModel select x)
					{
						if(!query.Contains((responseObjectModel.ResponseObject as TelegramMessageObjectModel).Bot2Use)){
							TelegramBotObjectCollection = new ObservableCollection<TelegramBotObjectModel>(query as IEnumerable<TelegramBotObjectModel>);
							TelegramBotObjectCollection.Add((responseObjectModel.ResponseObject as TelegramMessageObjectModel).Bot2Use);
							SelectedTelegramBotObject = TelegramBotObjectCollection[0];
						}
					}
				}
				
				if(!query.Any() && TelegramBotObjectCollection == null)
					TelegramBotObjectCollection = new ObservableCollection<TelegramBotObjectModel>();
				
				else
				{
					if(TelegramBotObjectCollection == null)
						TelegramBotObjectCollection = new ObservableCollection<TelegramBotObjectModel>(query as IEnumerable<TelegramBotObjectModel>);
					
					if(this.TelegramBotObjectCollection.Count > 0 && SelectedTelegramBotObject == null)
						SelectedTelegramBotObject = this.TelegramBotObjectCollection[0];
					
					if(
						SelectedTelegramBotObject != null &&
						SelectedTelegramBotObject.TelegramUsers != null &&
						SelectedTelegramBotObject.TelegramUsers.Count > 0 &&
						SelectedTelegramBotObject.TelegramUsers.Where(x => x.UserStatus == TelegramUserStatus.UserIsAdmin).Select(x => x.UserStatus == TelegramUserStatus.UserIsAdmin).Single()
					)
					{
						TelegramMessageObject.Bot2Use = SelectedTelegramBotObject;
						ResponseObjectModel.HasConfiguration = true;
						StatusMessage = string.Format("Online with ChatID: {0}",TelegramMessageObject.Bot2Use.TelegramUsers.Where(x => x.UserStatus == TelegramUserStatus.UserIsAdmin).Select(x => x.chatID).Single());
					}
					
				}

				//TelegramBotObjectCollection.Add((objectModel.ResponseObject as TelegramMessageObjectModel).Bot);
				
			}
		}

		#region Dialogs
		
		private ObservableCollection<IDialogViewModel> dialogs = new ObservableCollection<IDialogViewModel>();
		public ObservableCollection<IDialogViewModel> Dialogs { get { return dialogs; } }
		
		#endregion
		
		#region dependency properties

		/// <summary>
		/// 
		/// </summary>
		public TelegramMessageObjectModel TelegramMessageObject { get; set; }
		
		/// <summary>
		/// 
		/// </summary>
		public ObservableCollection<ResponseObjectModel> ResponseCollection
		{
			get { return responseCollection; }
			set {
				responseCollection = value;
				RaisePropertyChanged("ResponseCollection");
			}
		} private ObservableCollection<ResponseObjectModel> responseCollection;
		/// <summary>
		/// 
		/// </summary>
		public ObservableCollection<TelegramBotObjectModel> TelegramBotObjectCollection
		{
			get { return telegramBotObjectCollection;}
			set { telegramBotObjectCollection = value; RaisePropertyChanged("TelegramBotObjectCollection"); } //telegramBotObjectCollection = value;
		} private ObservableCollection<TelegramBotObjectModel> telegramBotObjectCollection;
		
		/// <summary>
		/// 
		/// </summary>
		public TelegramBotObjectModel SelectedTelegramBotObject
		{
			get { return selectedTelegramBotObject; }
			set
			{
				selectedTelegramBotObject = value;
				RaisePropertyChanged("SelectedTelegramBotObject");
			}
		} private TelegramBotObjectModel selectedTelegramBotObject;
		
		/// <summary>
		/// 
		/// </summary>
		public string TelegramMessage
		{
			get { return TelegramMessageObject.MessageToSend; }
			set {
				TelegramMessageObject.MessageToSend = value;
				RaisePropertyChanged("TelegramMessage");
			}
		}
		
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
		
		#endregion
		
		#region Localization
		
		public string Caption
		{
			get { using (var resMan = new ResourceLoader()) { return resMan.getResource("windowCaptionResponseEditorDialog"); } }
		}
		
		public string LocalizationResourceSet { get; set; }
		
		#endregion
		
		#region Commands
		
		public ICommand RemoveBot { get { return new RelayCommand(OnNewRemoveBotCommand); } }
		private void OnNewRemoveBotCommand()
		{
			try
			{
				SelectedTelegramBotObject.StopBot();
				TelegramBotObjectCollection.Remove(SelectedTelegramBotObject);
				SelectedTelegramBotObject = null;
			}
			catch(Exception e)
			{
				LogWriter.CreateLogEntry(string.Format("{0}\n{1}",e.Message, e.InnerException != null ? e.InnerException.Message : ""));
				return;
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		public ICommand AddBot { get { return new RelayCommand(OnNewAddBotCommand); } }
		private void OnNewAddBotCommand()
		{
			this.Dialogs.Add(new ResponseConfigurationDialogTelegramBotConfigurationDialogViewModel()
			                 {
			                 	OnOk = (sender) =>
			                 	{
			                 		
			                 		if(
			                 			sender.TelegramBot != null &&
			                 			sender.TelegramBot.TelegramUsers != null &&
			                 			sender.TelegramBot.TelegramUsers.Count > 0 &&
			                 			sender.TelegramBot.TelegramUsers.Where(x => x.UserStatus == TelegramUserStatus.UserIsAdmin).Select(x => x.UserStatus == TelegramUserStatus.UserIsAdmin).Single())
			                 		{
			                 			this.TelegramMessageObject.Bot2Use = sender.TelegramBot;
			                 			ResponseObjectModel.HasConfiguration = true;
			                 			
			                 			if(! TelegramBotObjectCollection.Contains(TelegramMessageObject.Bot2Use))
			                 				TelegramBotObjectCollection.Add(TelegramMessageObject.Bot2Use);
			                 			
			                 			if(this.TelegramBotObjectCollection.Count > 0 && SelectedTelegramBotObject == null)
			                 				SelectedTelegramBotObject = TelegramMessageObject.Bot2Use;
			                 			else
			                 				RaisePropertyChanged("SelectedTelegramBotObject");
			                 			
			                 			StatusMessage = string.Format("Online with ChatID: {0}",TelegramMessageObject.Bot2Use.TelegramUsers.Where(x => x.UserStatus == TelegramUserStatus.UserIsAdmin).Select(x => x.chatID).Single());
			                 			
			                 			sender.Close();
			                 		}
			                 		else
			                 		{
			                 			if (new MessageBoxViewModel {
			                 			    	Caption = resLoader.getResource("messageBoxNoKnownTelegramUserCaption"),
			                 			    	Message = resLoader.getResource("messageBoxNoKnownTelegramUserMessage"),
			                 			    	Buttons = MessageBoxButton.YesNo,
			                 			    	Image = MessageBoxImage.Question
			                 			    }
			                 			    .Show(this.Dialogs) == MessageBoxResult.Yes)
			                 			{
			                 				sender.TelegramBot = null;
			                 				ResponseObjectModel.HasConfiguration = false;
			                 				sender.Close();
			                 			}

			                 			ResponseObjectModel.HasConfiguration = false;
			                 		}
			                 	},

			                 	OnCancel = (sender) =>
			                 	{
			                 		sender.Close();
			                 		ResponseObjectModel.HasConfiguration = false;
			                 	},
			                 	
			                 	OnCloseRequest = (sender) => {
			                 		sender.Close();
			                 		ResponseObjectModel.HasConfiguration = false;
			                 	}
			                 });
		}
		
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
		
		public ICommand CancelCommand { get { return new RelayCommand(Cancel); } }
		protected virtual void Cancel()
		{
			if (this.OnCancel != null)
				this.OnCancel(this);
			else
				Close();
		}
		
		public Action<ResponseConfigurationDialogTelegramMessageResponseViewModel> OnOk { get; set; }
		public Action<ResponseConfigurationDialogTelegramMessageResponseViewModel> OnCancel { get; set; }
		public Action<ResponseConfigurationDialogTelegramMessageResponseViewModel> OnCloseRequest { get; set; }

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
