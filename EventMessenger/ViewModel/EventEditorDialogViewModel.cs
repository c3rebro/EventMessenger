/*
 * Created by SharpDevelop.
 * Date: 02/26/2017
 * Time: 22:50
 * 
 */
using EventMessenger.ViewModel;
using EventMessenger.EventObjects;

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
	/// Description of responseDataGridViewModel.
	/// </summary>
	public class EventEditorDialogViewModel : ViewModelBase, IUserDialogViewModel
	{
		private ResourceLoader resLoader;
		/// <summary>
		/// 
		/// </summary>
		public ObservableCollection<EventObjectModel> StaticEvents
		{
			get { return staticEvents; }
			set { staticEvents = value; }
		} private ObservableCollection<EventObjectModel> staticEvents;
		
		/// <summary>
		/// 
		/// </summary>
		public EventObjectModel ObjectModel
		{
			get { return objectModel; }
			set { objectModel = value; }
		} private EventObjectModel objectModel;
		
		#region Dialogs
		
		private ObservableCollection<IDialogViewModel> dialogs = new ObservableCollection<IDialogViewModel>();
		public ObservableCollection<IDialogViewModel> Dialogs { get { return dialogs; } }
		
		#endregion

		#region constructors
		public EventEditorDialogViewModel(ObservableCollection<EventObjectModel> _staticEvents, EventObjectModel _selectedItem)
		{
			resLoader = new ResourceLoader();
			
			ObjectModel = new EventObjectModel();
			staticEvents = _staticEvents;
			
			EventTypeComboBoxIsEnabled = false;

			EventTypeList  = new ObservableCollection<string>();
			
			dataGridSource = new ObservableCollection<EventObjectModel>();
			
			this.IsModal = true;
			
			if(_selectedItem != null)
			{
				SelectedDataGridItem = _selectedItem;
				EventName = _selectedItem.EventName;
				EventDescription = _selectedItem.EventDescription;
				HasConfiguration = _selectedItem.HasConfiguration;
				
				using (var resMan = new ResourceLoader())
				{
					SelectedGeneralEventType = Enum.GetName(typeof(GeneralEventTypes), Enum.Parse(typeof(GeneralEventTypes), _selectedItem.EventType));
				}
			}

			
			else
			{
				EventTypeComboBoxIsEnabled = true;
				
				selectedGeneralEventType = Enum.GetName(typeof(GeneralEventTypes), GeneralEventTypes.UnlockingEvent);
				

				EventName = resLoader.getResource("textBoxEventEditorWindowEventName");
				EventDescription = resLoader.getResource("textBoxEventEditorWindowEventDescription");
			}

		}
		
		#endregion
		
		#region items sources
		
		public string Caption
		{
			get { using (var resMan = new ResourceLoader()) { return resMan.getResource("windowCaptionEventEditorWindow"); } }
		}
		
		private string eventName;
		public string EventName
		{
			get {return eventName;}
			set {
				eventName = value;
				objectModel.EventName = eventName;
				RaisePropertyChanged("EventName");
			}
		}

		private string eventDescription;
		public string EventDescription
		{
			get {return eventDescription;}
			set {
				eventDescription = value;
				objectModel.EventDescription = eventDescription;
				RaisePropertyChanged("EventDescription");
			}
		}
		
		public bool HasConfiguration
		{
			get { return hasConfiguration; }
			set {
				hasConfiguration = value;
				if(hasConfiguration)
					EventTypeComboBoxIsEnabled = false;
				RaisePropertyChanged("HasConfiguration");
			}
		} private bool hasConfiguration;
		
		
		private string selectedGeneralEventType;
		public string SelectedGeneralEventType
		{
			get { return selectedGeneralEventType; }
			set {
				selectedGeneralEventType = value;
				objectModel.EventType = selectedGeneralEventType;
				RaisePropertyChanged("SelectedGeneralEventType");
			}
		}
		
		public ObservableCollection<string> EventTypeList { get; set; }
		
		private ObservableCollection<EventObjectModel> dataGridSource;
		public ObservableCollection<EventObjectModel> DataGridSource {
			get {return dataGridSource; }
			set {
				dataGridSource = value;
				RaisePropertyChanged("DataGridSource");
			}
		}
		#endregion
		
		/// <summary>
		/// 
		/// </summary>
		public bool EventTypeComboBoxIsEnabled
		{
			get { return eventTypeComboBoxIsEnabled; }
			set {
				eventTypeComboBoxIsEnabled = value;
				RaisePropertyChanged("EventTypeComboBoxIsEnabled");
			}
		} private bool eventTypeComboBoxIsEnabled;
		
		#region item selections
		private EventObjectModel selectedDataGridItem;
		public EventObjectModel SelectedDataGridItem {
			get { return selectedDataGridItem; }
			set {selectedDataGridItem = value;
				RaisePropertyChanged("SelectedDataGridItem");
				RaisePropertyChanged("ContextMenu");}
		}
		#endregion
		
		#region Localization

		/// <summary>
		/// Act as a proxy between RessourceLoader and View directly.
		/// </summary>
		public string LocalizationResourceSet { get; set; }
		
		#endregion
		
		#region commands
		
		/// <summary>
		/// Command to open a new Dialogbox. Asking the user what event to setup.
		/// </summary>
		public ICommand ConfigureBehaviorCommand { get { return new RelayCommand(OnNewAddOrEditBehaviorCommand); } }
		private void OnNewAddOrEditBehaviorCommand()
		{
			switch(this.SelectedGeneralEventType)
			{
				case "UnlockingEvent":
					this.Dialogs.Add(new EventConfigurationDialogUnlockingEventViewModel(SelectedDataGridItem)
					                 {
					                 	OnOk = (sender) =>
					                 	{
					                 		if(sender.TransponderModel != null)
					                 		{
					                 			objectModel.EventType = this.SelectedGeneralEventType;
					                 			objectModel.TransponderModel = sender.TransponderModel;
					                 			
					                 			HasConfiguration = true;
					                 		}
					                 		
					                 		else
					                 			HasConfiguration = false;
					                 		
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
					
				case "DoorMonitoringEvent":
					this.Dialogs.Add(new EventConfigurationDialogDoorMonEventViewModel(SelectedDataGridItem)
					                 {
					                 	OnOk = (sender) =>
					                 	{
					                 		if(sender.TransponderModel != null && ! string.IsNullOrEmpty(sender.TransponderModel.TransponderName) )
					                 		{
					                 			if(sender.TransponderModel.LockModels != null && sender.TransponderModel.LockModels.Count > 0)
					                 			{
					                 				objectModel.EventType = this.SelectedGeneralEventType;
					                 				objectModel.TransponderModel = sender.TransponderModel;
					                 				
					                 				HasConfiguration = true;
					                 			}
					                 		}

					                 		else
					                 			HasConfiguration = false;
					                 		
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
					
				case "InputEvent":
					this.Dialogs.Add(new EventConfigurationDialogInputEventViewModel(SelectedDataGridItem)
					                 {
					                 	OnOk = (sender) =>
					                 	{
					                 		if(
					                 			!string.IsNullOrEmpty(sender.SelectedInputName) &&
					                 			!string.IsNullOrEmpty(sender.SelectedInputTriggerType) &&
					                 			!string.IsNullOrEmpty(sender.LockNodeAddress) &&
					                 			sender.IsValidAddress == true)
					                 		{
					                 			objectModel.InputModel = sender.InputModel;
					                 			objectModel.EventType = this.SelectedGeneralEventType;
					                 			HasConfiguration = true;
					                 			sender.Close();
					                 		}

					                 		
					                 		else
					                 		{
					                 			new MessageBoxViewModel {
					                 				Caption = resLoader.getResource("messageBoxMissingConfigCaption"),
					                 				Message = resLoader.getResource("messageBoxMissingConfigMessage"),
					                 				Buttons = MessageBoxButton.OK,
					                 				Image = MessageBoxImage.Information
					                 			}
					                 			.Show(this.Dialogs);
					                 			
					                 			HasConfiguration = false;
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
		
		#endregion
		
		#region IUserDialogViewModel Implementation

		/// <summary>
		/// gets; sets whether this dialog is modal or not
		/// </summary>
		public bool IsModal { get; private set; }
		public virtual void RequestClose()
		{
			if (this.OnCloseRequest != null)
				OnCloseRequest(this);
			else
				Close();
		}
		public event EventHandler DialogClosing;

		public ICommand ApplyCommand { get { return new RelayCommand(Ok); } }
		protected virtual void Ok()
		{
			if (this.OnOk != null)
				this.OnOk(this);
			else
				Close();
		}
		
		public ICommand ExitCommand { get { return new RelayCommand(Cancel); } }
		protected virtual void Cancel()
		{
			if (this.OnCancel != null)
				this.OnCancel(this);
			else
				Close();
		}
		
		public ICommand AuthCommand { get { return new RelayCommand(Auth); } }
		protected virtual void Auth()
		{
			if (this.OnAuth != null)
				this.OnAuth(this);
			else
				Close();
		}
		
		public Action<EventEditorDialogViewModel> OnOk { get; set; }
		public Action<EventEditorDialogViewModel> OnCancel { get; set; }
		public Action<EventEditorDialogViewModel> OnAuth { get; set; }
		public Action<EventEditorDialogViewModel> OnCloseRequest { get; set; }

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
