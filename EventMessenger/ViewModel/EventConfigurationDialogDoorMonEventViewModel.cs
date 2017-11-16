/*
 * Created by SharpDevelop.
 * Date: 20.03.2017
 * Time: 11:32
 * 
 */
using EventMessenger.Model;
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
	/// Description of EventConfigurationDialogDoorMonEventViewModel.
	/// </summary>
	public class EventConfigurationDialogDoorMonEventViewModel : ViewModelBase, IUserDialogViewModel
	{
		private ResourceLoader resLoader;
		/// <summary>
		/// 
		/// </summary>
		/// <param name="om"></param>
		public EventConfigurationDialogDoorMonEventViewModel(EventObjectModel om)
		{
			resLoader = new ResourceLoader();
			
			LockModel = new LockModel();
			TransponderModel = new TransponderModel();
			
			SelectedDoorMonEventType = Enum.GetName(typeof(DoorMonEventType), DoorMonEventType.DoorHasBeenOpened);
			
			this.IsModal = true;
			
			if(om != null)
			{
				this.TransponderModel = om.TransponderModel;
				LockModel = om.TransponderModel.LockModels[0];
			}
		}
		
		#region Dialogs
		
		private ObservableCollection<IDialogViewModel> dialogs = new ObservableCollection<IDialogViewModel>();
		public ObservableCollection<IDialogViewModel> Dialogs { get { return dialogs; } }
		
		#endregion
		
		#region Selected Items
		
		private string selectedDoorMonEventType;
		public string SelectedDoorMonEventType
		{
			get { return selectedDoorMonEventType; }
			set { selectedDoorMonEventType = value;
				RaisePropertyChanged("SelectedDoorMonEventType");
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		public ObservableCollection<string> DoorMonitoringTrigger
		{
			get { return LockModel.DoorMonitoringTrigger; }
			set {
				LockModel.DoorMonitoringTrigger = value;
				RaisePropertyChanged("DoorMonitoringTrigger");
			}
		}
		
		private LockModel selectedLockItem;
		public LockModel SelectedLockItem
		{
			get { return selectedLockItem; }
			set {
				selectedLockItem = value;
				
				LockName = selectedLockItem.LockName;
				LockPHI = selectedLockItem.LockPHI;
				SelectedDoorMonEventType = selectedLockItem.DoorMonitoringTrigger[0];
			}
		}
		
		#endregion
		
		#region Items Sources
		
		public string Caption
		{
			get { using (var resMan = new ResourceLoader()) { return resMan.getResource("windowCaptionEventConfigurationDialogDoorMonitoringEvent"); } }
		}

		/// <summary>
		/// 
		/// </summary>
		public bool UseDummyLock
		{
			get { return useDummyLock; }
			set { useDummyLock = value; }
		} private bool useDummyLock;
		
		/// <summary>
		/// 
		/// </summary>
		public string TransponderName
		{
			get { return this.TransponderModel.TransponderName; }
			set {
				this.TransponderModel.TransponderName = value;
				RaisePropertyChanged("TransponderName");
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		public string TransponderPHI
		{
			get { return this.TransponderModel.TransponderPHI; }
			set {
				this.TransponderModel.TransponderPHI = value;
				RaisePropertyChanged("TransponderPHI");
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		public string LockName
		{
			get { return LockModel.LockName; }
			set {
				LockModel.LockName = value;
				RaisePropertyChanged("LockName");
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		public string LockPHI
		{
			get { return LockModel.LockPHI; }
			set {
				LockModel.LockPHI = value;
				RaisePropertyChanged("LockPHI");
			}
		}
		
		private TransponderModel transponderModel;
		public TransponderModel TransponderModel
		{
			get { return transponderModel; }
			set { transponderModel = value;
				RaisePropertyChanged("TransponderModel");
				RaisePropertyChanged("LockModels");
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		public LockModel LockModel
		{
			get { return lockModel; }
			set { lockModel = value; }
		} private LockModel lockModel;
		
		/// <summary>
		/// 
		/// </summary>
		public ObservableCollection<LockModel> LockModels
		{
			get { return this.TransponderModel.LockModels; }
			set {
				this.TransponderModel.LockModels = value;
				RaisePropertyChanged("LockModels");
			}
		}
		
		#endregion
		
		#region Commands
		public ICommand AddLockCommand { get { return new RelayCommand(AddLockToListCommand); } }
		private void AddLockToListCommand()
		{
			if(string.IsNullOrEmpty(LockPHI))
				LockPHI = "$dont care";
			
			if(string.IsNullOrEmpty(LockName))
				LockName = "$dont care";
			
			if(string.IsNullOrEmpty(TransponderName))
				TransponderName = "$dont care";
			
			if(string.IsNullOrEmpty(TransponderPHI))
				TransponderPHI = "$dont care";
			

			foreach(LockModel lm in LockModels)
			{
				if(lm.LockName == LockName && lm.LockPHI == LockPHI && lm.DoorMonitoringTrigger.Contains(SelectedDoorMonEventType))
				{
					new MessageBoxViewModel {
						Caption = resLoader.getResource("messageBoxDuplicateLockCaption"),
						Message = resLoader.getResource("messageBoxDuplicateLockMessage"),
						Buttons = MessageBoxButton.OK,
						Image = MessageBoxImage.Information
					}.Show(this.Dialogs);
					return;
				}
				else if(lm.LockName == LockName && lm.LockPHI == LockPHI && !lm.DoorMonitoringTrigger.Contains(SelectedDoorMonEventType))
				{
					
					DoorMonitoringTrigger.Add(SelectedDoorMonEventType);
					
					RaisePropertyChanged("DoorMonitoringTrigger");

					return;
				}
			}
			
			
			/*
			 * //TODO: For Now: I do not want to combine door monitoring triggers and transpondernames. maybe later
			 *
			foreach(TransponderModel tm in transponderModels)
			{
				if(tm.TransponderName == TransponderName && tm.TransponderPHI == TransponderPHI)
				{
					new MessageBoxViewModel {
						Caption = "Already in List...",
						Message = "The Transponder you tried to add is already in the List.",
						Buttons = MessageBoxButton.OK,
						Image = MessageBoxImage.Information
					}.Show(this.Dialogs);
					return;
				}

			}
			 */

			if(!LockModel.DoorMonitoringTrigger.Contains(SelectedDoorMonEventType))
				LockModel.DoorMonitoringTrigger.Add(SelectedDoorMonEventType);
			LockModels.Add(LockModel);

			//LockModels.Clear();
			
		}
		
		public ICommand AddTransponderCommand { get { return new RelayCommand(AddTransponderToListCommand); } }
		private void AddTransponderToListCommand()
		{
			if(string.IsNullOrEmpty(TransponderName))
				TransponderName = "$dont care";
			
			if(string.IsNullOrEmpty(TransponderPHI))
				TransponderPHI = "$dont care";

			if(LockModels != null && LockModels.Count > 0)
			{

				TransponderModel = new TransponderModel(TransponderName, TransponderPHI, LockModels);
			}
			else
			{
				new MessageBoxViewModel {
					Caption = resLoader.getResource("messageBoxNoLocksCaption"),
					Message = resLoader.getResource("messageBoxNoLocksMessage"),
					Buttons = MessageBoxButton.OK,
					Image = MessageBoxImage.Information
				}.Show(this.Dialogs);
				

			}

			
			//LockModels.Clear();
		}
		
		#endregion
		
		#region Localization
		
		public string LocalizationResourceSet { get; set; }
		
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
		
		public Action<EventConfigurationDialogDoorMonEventViewModel> OnOk { get; set; }
		public Action<EventConfigurationDialogDoorMonEventViewModel> OnCancel { get; set; }
		public Action<EventConfigurationDialogDoorMonEventViewModel> OnCloseRequest { get; set; }

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
