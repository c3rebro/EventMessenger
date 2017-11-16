/*
 * Created by SharpDevelop.
 * Date: 03/04/2017
 * Time: 21:20
 * 
 */
using EventMessenger;
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

namespace EventMessenger.ViewModel
{

	/// <summary>
	/// Description of ResponseEditorDialogViewModel.
	/// </summary>
	public class ResponseEditorDialogViewModel : ViewModelBase, IUserDialogViewModel
	{

		private readonly ResourceLoader resLoader;
		
		private readonly ObservableCollection<MenuItem> rowContextMenuItems;
		private readonly ObservableCollection<MenuItem> emptySpaceContextMenuItems;

		private readonly RelayCommand _cmdDeleteEntry;
		private readonly RelayCommand _cmdAddEditResponse;
		private readonly RelayCommand _cmdAddEditSchedule;
		
		#region Dialogs
		
		private ObservableCollection<IDialogViewModel> dialogs = new ObservableCollection<IDialogViewModel>();
		public ObservableCollection<IDialogViewModel> Dialogs { get { return dialogs; } }
		
		#endregion
		
		#region constructors
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="_selectedEventObjectModel"></param>
		/// <param name="_staticEvents"></param>
		public ResponseEditorDialogViewModel(ObservableCollection<EventObjectModel> _staticEvents, EventObjectModel _selectedEventObjectModel)
		{
			//responseObjectModels = objModels;
			//ResponseTypeList  = new ObservableCollection<string>(Constants.responseTypeList);

			StaticEvents = _staticEvents;
			
			resLoader = new ResourceLoader();
			
			_cmdDeleteEntry = new RelayCommand(OnNewDeleteSelectedItemCommand);
			_cmdAddEditResponse = new RelayCommand(OnNewAddOrEditBehaviorCommand);
			_cmdAddEditSchedule = new RelayCommand(OnNewAddOrEditScheduleCommand);
			
			rowContextMenuItems = new ObservableCollection<MenuItem>();
			emptySpaceContextMenuItems = new ObservableCollection<MenuItem>();
			
			emptySpaceContextMenuItems.Add(new MenuItem()
			                               {
			                               	Header = resLoader.getResource("contextMenuItemAddNewResponse"),
			                               	Command = _cmdAddEditResponse
			                               });

			
			rowContextMenuItems.Add(new MenuItem()
			                        {
			                        	Header = resLoader.getResource("contextMenuItemEditResponse"),
			                        	Command = _cmdAddEditResponse
			                        });


			rowContextMenuItems.Add(new MenuItem()
			                        {
			                        	Header = resLoader.getResource("contextMenuItemAddEditSchedule"),
			                        	Command = _cmdAddEditSchedule
			                        });
			
			rowContextMenuItems.Add(new MenuItem()
			                        {
			                        	Header = resLoader.getResource("contextMenuItemDeleteResponse"),
			                        	Command = _cmdDeleteEntry
			                        });
			
			if(_selectedEventObjectModel.ResponseCollection != null && _selectedEventObjectModel.ResponseCollection.Count > 0)
			{
				ResponseCollection = _selectedEventObjectModel.ResponseCollection;
				ResponseObjectModel = _selectedEventObjectModel.ResponseCollection[0];
				SelectedDataGridItem = ResponseObjectModel;
			}
			else
			{
				ResponseCollection = new ObservableCollection<ResponseObjectModel>();
				ResponseObjectModel = new ResponseObjectModel();
				
				ResponseName = resLoader.getResource("textBoxResponseEditorWindowResponseName");;
				ResponseDescription = resLoader.getResource("textBoxResponseEditorWindowResponseDescription");
				
				SelectedResponseType =  Enum.GetName(typeof(GeneralResponseTypes), GeneralResponseTypes.MessageTELEGRAM);
			}
			
			this.IsModal = true;
		}
		#endregion

		#region items sources

		public string Caption
		{
			get { using (var resMan = new ResourceLoader()) { return resMan.getResource("windowCaptionResponseEditorDialog"); } }
		}
		
		public ObservableCollection<MenuItem> RowContextMenu {
			get {
				return rowContextMenuItems;
			}
		}

		public ObservableCollection<MenuItem> EmptySpaceContextMenu {
			get {
				return emptySpaceContextMenuItems;
			}
		}
		
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
		public ObservableCollection<EventObjectModel> StaticEvents { get; set; }
		
		/// <summary>
		/// 
		/// </summary>
		public ResponseObjectModel ResponseObjectModel
		{
			get { return responseObjectModel; }
			set {
				responseObjectModel = value;
				RaisePropertyChanged("ResponseObjectModel");
				RaisePropertyChanged("ResponseName");
				RaisePropertyChanged("ResponseDescription");
			}
			
		} private ResponseObjectModel responseObjectModel;
		
		/// <summary>
		/// 
		/// </summary>
		public string ResponseName
		{
			get { return ResponseObjectModel != null ? ResponseObjectModel.ResponseName : "N/A"; }
			set {
				ResponseObjectModel.ResponseName = value;
				RaisePropertyChanged("ResponseName");
				RaisePropertyChanged("ResponseObjectModels");
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		public string ResponseDescription
		{
			get { return ResponseObjectModel != null ? ResponseObjectModel.ResponseDescription : "N/A"; }
			set {
				ResponseObjectModel.ResponseDescription = value;
				RaisePropertyChanged("ResponseDescription");
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		public string SelectedResponseType
		{
			get { return ResponseObjectModel != null ? ResponseObjectModel.ResponseType : ""; }
			set {
				ResponseObjectModel.ResponseType = value;
				RaisePropertyChanged("SelectedResponseType");
			}
		}
		
		public ResponseObjectModel SelectedDataGridItem
		{
			get
			{
				if(selectedDataGridItem == null)
					ResponseTypeComboBoxIsEnabled = true;
				else
					ResponseTypeComboBoxIsEnabled = false;
				
				RaisePropertyChanged("SelectedResponseType");
				return selectedDataGridItem;
			}

			set
			{
				selectedDataGridItem = value;
				if(selectedDataGridItem == null)
				{
					ResponseObjectModel = new ResponseObjectModel();
					SelectedResponseType =  Enum.GetName(typeof(GeneralResponseTypes), GeneralResponseTypes.MessageTELEGRAM);
					ResponseName = resLoader.getResource("textBoxResponseEditorWindowResponseName");;
					ResponseDescription = resLoader.getResource("textBoxResponseEditorWindowResponseDescription");
					ResponseObjectModel.ResponseType = SelectedResponseType;
				}
				else
					ResponseObjectModel = selectedDataGridItem;
				RaisePropertyChanged("SelectedDataGridItem");
			}
			
		} private ResponseObjectModel selectedDataGridItem;
		
		/// <summary>
		/// 
		/// </summary>
		public bool ResponseTypeComboBoxIsEnabled
		{
			get { return responseTypeIsReadOnly; }
			set {
				responseTypeIsReadOnly = value;
				RaisePropertyChanged("ResponseTypeComboBoxIsEnabled");
			}

		} private bool responseTypeIsReadOnly;
		
		#endregion
		
		#region item selections

		#endregion
		
		#region Localization

		public string LocalizationResourceSet { get; set; }
		
		#endregion
		
		#region commands
		
		/// <summary>
		/// 
		/// </summary>
		public ICommand DeleteSelectedItem { get { return new RelayCommand(OnNewDeleteSelectedItemCommand); }}
		private void OnNewDeleteSelectedItemCommand()
		{
			if(SelectedDataGridItem != null)
				ResponseCollection.Remove(SelectedDataGridItem);
		}
		
		
		/// <summary>
		/// 
		/// </summary>
		public ICommand ConfigureBehaviorCommand { get { return new GalaSoft.MvvmLight.Command.RelayCommand(OnNewAddOrEditBehaviorCommand); } }
		private void OnNewAddOrEditBehaviorCommand()
		{
			switch(this.SelectedResponseType)
			{
				case "MessageTELEGRAM":
					
					this.Dialogs.Add(new ResponseConfigurationDialogTelegramMessageResponseViewModel(StaticEvents, ResponseCollection, SelectedDataGridItem)
					                 {
					                 	OnOk = (sender) =>
					                 	{
					                 		ResponseObjectModel.ResponseObject = sender.ResponseObjectModel.ResponseObject;
					                 		ResponseObjectModel.HasConfiguration = sender.ResponseObjectModel.HasConfiguration;
					                 		//TODO: Check and Clean if abortion
					                 		
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
					break;
					
				case "MessageEMAIL":
					this.Dialogs.Add(new ResponseConfigurationDialogEMailMessageResponseViewModel(ResponseCollection, SelectedDataGridItem)
					                 {
					                 	OnOk = (sender) =>
					                 	{
					                 		try
					                 		{
					                 			if(sender.EMailCollection != null && sender.EMailCollection.Count > 0)
					                 			{
					                 				ResponseObjectModel.ResponseObject = sender.ResponseObjectModel.ResponseObject;
					                 				ResponseObjectModel.HasConfiguration = sender.ResponseObjectModel.HasConfiguration;
					                 			}
					                 			sender.Close();
					                 		}
					                 		
					                 		catch (Exception e)
					                 		{
					                 			if(e.GetType() == typeof(NullReferenceException))
					                 			{
					                 				if (new MessageBoxViewModel {
					                 				    	Caption = "No Mails attached",
					                 				    	Message = "Are your sure to proceed without any attached mails",
					                 				    	Buttons = MessageBoxButton.YesNo,
					                 				    	Image = MessageBoxImage.Question
					                 				    }
					                 				    .Show(this.Dialogs) == MessageBoxResult.Yes)
					                 					this.Close();
					                 				
					                 			}
					                 			
					                 			else
					                 			{
					                 				if (new MessageBoxViewModel {
					                 				    	Caption = "Error",
					                 				    	Message = string.Format("{0}, {1}",e.Message, e.InnerException == null ? "" : e.InnerException.Message),
					                 				    	Buttons = MessageBoxButton.YesNo,
					                 				    	Image = MessageBoxImage.Question
					                 				    }
					                 				    .Show(this.Dialogs) == MessageBoxResult.Yes)
					                 					this.Close();
					                 			}
					                 		}
					                 	},

					                 	OnCancel = (sender) =>
					                 	{
					                 		sender.Close();
					                 	},
					                 	
					                 	OnCloseRequest = (sender) => {
					                 		sender.Close();
					                 	}
					                 });
					break;
					
				case "DeviceKNX":
					break;
					
				case "DeviceKMTronic":
					this.Dialogs.Add(new ResponseConfigurationDialogKMTronicDeviceConfigurationViewModel(ResponseCollection, SelectedDataGridItem)
					                 {
					                 	OnOk = (sender) =>
					                 	{
					                 		try
					                 		{
					                 			if(sender.RelayContactCollection != null && sender.RelayContactCollection.Count > 0)
					                 			{
					                 				KMTronicModel device = new KMTronicModel(sender.SelectedDevice);
					                 				device.RelayContactCollection = sender.RelayContactCollection;
					                 				device.HostName = sender.HostName;
					                 				device.PortNumber = Convert.ToInt32(sender.PortNumber);
					                 				device.UserName = sender.UserName;
					                 				device.Password = sender.Password;
					                 				
					                 				ResponseObjectModel.ResponseObject = device;
					                 				
					                 				
					                 				ResponseObjectModel.HasConfiguration = true;
					                 			}
					                 			sender.Close();
					                 		}
					                 		
					                 		catch (Exception e)
					                 		{
					                 			if(e.GetType() == typeof(NullReferenceException))
					                 			{
					                 				if (new MessageBoxViewModel {
					                 				    	Caption = "No Mails attached",
					                 				    	Message = "Are your sure to proceed without any attached mails",
					                 				    	Buttons = MessageBoxButton.YesNo,
					                 				    	Image = MessageBoxImage.Question
					                 				    }
					                 				    .Show(this.Dialogs) == MessageBoxResult.Yes)
					                 					this.Close();
					                 				
					                 			}
					                 			
					                 			else
					                 			{
					                 				if (new MessageBoxViewModel {
					                 				    	Caption = "Error",
					                 				    	Message = string.Format("{0}, {1}",e.Message, e.InnerException == null ? "" : e.InnerException.Message),
					                 				    	Buttons = MessageBoxButton.YesNo,
					                 				    	Image = MessageBoxImage.Question
					                 				    }
					                 				    .Show(this.Dialogs) == MessageBoxResult.Yes)
					                 					this.Close();
					                 			}
					                 		}
					                 	},

					                 	OnCancel = (sender) =>
					                 	{
					                 		sender.Close();
					                 	},
					                 	
					                 	OnCloseRequest = (sender) => {
					                 		sender.Close();
					                 	}
					                 });
					break;
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		public ICommand AddResponseCommand { get { return new GalaSoft.MvvmLight.Command.RelayCommand(OnNewAddOrEditResponseCommand); } }
		private void OnNewAddOrEditResponseCommand()
		{
			if(!ResponseCollection.Contains(ResponseObjectModel))
			{
				if(ResponseObjectModel.HasConfiguration == true)
				{
					ResponseCollection.Add(ResponseObjectModel);
				}
				else
					new MessageBoxViewModel {
					Caption = resLoader.getResource("messageBoxMissingConfigCaption"),
					Message = resLoader.getResource("messageBoxMissingConfigMessage"),
					Buttons = MessageBoxButton.OK,
					Image = MessageBoxImage.Information
				}
				.Show(this.Dialogs);
			}

			else
			{
				new MessageBoxViewModel {
					Caption = resLoader.getResource("messageBoxDuplicateObjectCaption"),
					Message = resLoader.getResource("messageBoxDuplicateObjectMessage"),
					Image = MessageBoxImage.Information
				}.Show(this.Dialogs);
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		public ICommand AddOrEditScheduleCommand { get { return new GalaSoft.MvvmLight.Command.RelayCommand(OnNewAddOrEditScheduleCommand); } }
		private void OnNewAddOrEditScheduleCommand()
		{
			this.Dialogs.Add(new ScheduleConfigurationDialogViewModel(ResponseCollection, SelectedDataGridItem) {

			                 	OnOk = (sender) => {
			                 		if(sender.ScheduleCollection != null && sender.ScheduleCollection.Count > 0)
			                 		{
			                 			selectedDataGridItem.HasSchedule = true;
			                 			selectedDataGridItem.Scheduler.Schedule = sender.ScheduleCollection;
			                 		}
			                 		else
			                 			selectedDataGridItem.HasSchedule = false;
			                 		
			                 		RaisePropertyChanged("ResponseCollection");
			                 		sender.Close();
			                 	},

			                 	OnCancel = (sender) => {
			                 		sender.Close();

			                 	},

			                 	OnCloseRequest = (sender) => {
			                 		sender.Close();
			                 	}
			                 });
		}
		#endregion
		
		#region IUserDialogViewModel Implementation

		/// <summary>
		/// 
		/// </summary>
		public bool IsModal { get; private set; }
		
		/// <summary>
		/// 
		/// </summary>
		public virtual void RequestClose()
		{
			if (this.OnCloseRequest != null)
				OnCloseRequest(this);
			else
				Close();
		}
		/// <summary>
		/// 
		/// </summary>
		public event EventHandler DialogClosing;

		/// <summary>
		/// 
		/// </summary>
		public ICommand ApplyCommand { get { return new RelayCommand(Ok); } }
		
		/// <summary>
		/// 
		/// </summary>
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
		/// <summary>
		/// 
		/// </summary>
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
		public ICommand AuthCommand { get { return new RelayCommand(Auth); } }
		/// <summary>
		/// 
		/// </summary>
		protected virtual void Auth()
		{
			if (this.OnAuth != null)
				this.OnAuth(this);
			else
				Close();
		}
		
		/// <summary>
		/// 
		/// </summary>
		public Action<ResponseEditorDialogViewModel> OnOk { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public Action<ResponseEditorDialogViewModel> OnCancel { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public Action<ResponseEditorDialogViewModel> OnAuth { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public Action<ResponseEditorDialogViewModel> OnCloseRequest { get; set; }
		
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
		public void Show(IList<IDialogViewModel> collection)
		{
			collection.Add(this);
		}
		
		#endregion IUserDialogViewModel Implementation
	}
}
