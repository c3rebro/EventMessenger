/*
 * Created by SharpDevelop.
 * Date: 02/28/2017
 * Time: 22:53
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
	/// Description of EventConfigurationDialogUnlockingEventViewModel.
	/// </summary>
	public class EventConfigurationDialogUnlockingEventViewModel : ViewModelBase, IUserDialogViewModel
	{
		
		private ResourceLoader resLoader;
		
		#region Contructors
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="om"></param>
		public EventConfigurationDialogUnlockingEventViewModel(EventObjectModel om)
		{
			resLoader = new ResourceLoader();
			
			TransponderModel = new TransponderModel();
			LockModels = new ObservableCollection<LockModel>();
			
			TransponderModel.SelectedTransponderTriggerType = Enum.GetName(typeof(AccessType), AccessType.AccessGranted);;
			
			useDummyLock = false;
			this.IsModal = true;
			
			if(om != null)
			{
				this.TransponderModel = om.TransponderModel;
				//LockModels = om.TransponderModel.LockModels;
			}
		}
		
		#endregion
		
		#region Dialogs
		
		/// <summary>
		/// 
		/// </summary>
		public ObservableCollection<IDialogViewModel> Dialogs { get { return dialogs; } }
		private ObservableCollection<IDialogViewModel> dialogs = new ObservableCollection<IDialogViewModel>();
		
		#endregion
		
		#region Selected Items
		
		/// <summary>
		/// 
		/// </summary>
		public LockModel SelectedLockItem
		{
			get { return selectedLockItem; }
			set {
				selectedLockItem = value;
				
				LockName = selectedLockItem.LockName;
				LockPHI = selectedLockItem.LockPHI;
			}
		} private LockModel selectedLockItem;
		
		#endregion
		
		#region Items Sources
		/// <summary>
		/// 
		/// </summary>
		public string Caption
		{
			get { using (var resMan = new ResourceLoader()) { return resMan.getResource("windowCaptionEventConfigurationDialogUnlockingEvent"); } }
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
			get { return lockName; }
			set {
				lockName = value;
				RaisePropertyChanged("LockName");
			}
		} private string lockName;
		
		/// <summary>
		/// 
		/// </summary>
		public string LockPHI
		{
			get { return lockPHI; }
			set {
				lockPHI = value;
				RaisePropertyChanged("LockPHI");
			}
		} private string lockPHI;
		
		private TransponderModel transponderModel;
		public TransponderModel TransponderModel
		{
			get { return transponderModel; }
			set { transponderModel = value;
				RaisePropertyChanged("TransponderModel");
				RaisePropertyChanged("LockName");
				RaisePropertyChanged("TransponderName");
			}
		}
		
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
		
		#region Localization
		/// <summary>
		/// 
		/// </summary>
		public string LocalizationResourceSet { get; set; }
		
		#endregion
		
		#region Commands
		public ICommand AddLockCommand { get { return new RelayCommand(AddLockToListCommand); } }
		private void AddLockToListCommand()
		{
			if(string.IsNullOrEmpty(lockPHI))
				LockPHI = "$dont care";
			
			if(string.IsNullOrEmpty(lockName))
				LockName = "$dont care";
			
			foreach(LockModel lm in LockModels)
			{
				if(lm.LockName == LockName && lm.LockPHI == LockPHI)
				{
					new MessageBoxViewModel {
						Caption = resLoader.getResource("messageBoxDuplicateLockCaption"),
						Message = resLoader.getResource("messageBoxDuplicateLockMessage"),
						Buttons = MessageBoxButton.OK,
						Image = MessageBoxImage.Information
					}.Show(this.Dialogs);
					return;
				}
			}
			LockModels.Add(new LockModel(LockPHI, LockName));
		}
		
		public ICommand AddTransponderCommand { get { return new RelayCommand(AddTransponderToListCommand); } }
		private void AddTransponderToListCommand()
		{
			if(string.IsNullOrEmpty(TransponderName))
				this.TransponderModel.TransponderName = "$dont care";
			
			if(string.IsNullOrEmpty(TransponderPHI))
				this.TransponderModel.TransponderPHI = "$dont care";

			if(LockModels != null && LockModels.Count > 0 )
			{
				//if(TransponderModel.SelectedTransponderTriggerType == null)
				//	transponderModel = new TransponderModel(TransponderName, TransponderPHI, LockModels);
				//transponderModel.SelectedLockName = transponderModel.LockModelNames[0];
			}
			else
			{
				new MessageBoxViewModel {
					Caption = resLoader.getResource("messageBoxNoLocksCaption"),
					Message = resLoader.getResource("messageBoxNoLocksMessage"),
					Buttons = MessageBoxButton.OK,
					Image = MessageBoxImage.Information
				}.Show(this.Dialogs);
				return;
				

			}

			//LockModels.Clear();
			
			Ok();
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
		
		public Action<EventConfigurationDialogUnlockingEventViewModel> OnOk { get; set; }
		public Action<EventConfigurationDialogUnlockingEventViewModel> OnCancel { get; set; }
		public Action<EventConfigurationDialogUnlockingEventViewModel> OnCloseRequest { get; set; }

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
