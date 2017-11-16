/*
 * Created by SharpDevelop.
 * Date: 03/18/2017
 * Time: 12:08
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
	/// Description of EventConfigurationDialogInputEventViewModel.
	/// </summary>
	public class EventConfigurationDialogInputEventViewModel : ViewModelBase, IUserDialogViewModel
	{
		/// <summary>
		/// 
		/// </summary>
		public EventConfigurationDialogInputEventViewModel(EventObjectModel om)
		{
			if(om != null)
			{
				InputModel = om.InputModel;
				
				SelectedInputName = om.InputModel.InputName;
				SelectedInputTriggerType = om.InputModel.InputTriggerType;
				
				LockNodeAddress = om.InputModel.Address;
			}
			else
			{
				InputModel = new InputModel();
				
				SelectedInputName = Enum.GetName(typeof(InputsByName), InputsByName.Input1);
				SelectedInputTriggerType = Enum.GetName(typeof(InputEdgeTriggerType), InputEdgeTriggerType.RisingEdge);
				
				LockNodeAddress = "0x0000";
			}

			

			
		}
		
		#region Dialogs
		
		private ObservableCollection<IDialogViewModel> dialogs = new ObservableCollection<IDialogViewModel>();
		/// <summary>
		/// 
		/// </summary>
		public ObservableCollection<IDialogViewModel> Dialogs { get { return dialogs; } }
		
		#endregion
		
		#region Selected Items
		
		public string Caption
		{
			get { using (var resMan = new ResourceLoader()) { return resMan.getResource("windowCaptionEventConfigurationDialogInputEvent"); } }
		}
		
		private string selectedInputName;
		public string SelectedInputName
		{
			get { return selectedInputName; }
			set	{
				selectedInputName = value;
				InputModel.InputName = selectedInputName;
				RaisePropertyChanged("SelectedInputName");
			}
		}
		
		private string selectedInputTriggerType;
		public string SelectedInputTriggerType
		{
			get { return selectedInputTriggerType; }
			set	{
				selectedInputTriggerType = value;
				InputModel.InputTriggerType = selectedInputTriggerType;
				RaisePropertyChanged("SelectedInputTriggerType");
			}
		}
		
		#endregion
		
		#region Items Sources
		
		public InputModel InputModel { get; set; }
		
		private string lockNodeAddress;
		public string LockNodeAddress
		{
			get { return lockNodeAddress; }
			set {
				
				try{
					if(CustomConverter.InHexFormat(value.Substring(2)) && (value.Length == 6) && (value.StartsWith("0x")) )
					{
						lockNodeAddress = value;
						InputModel.Address = lockNodeAddress;
						
						IsValidAddress = true;
					}
					else
					{
						lockNodeAddress = value;
						IsValidAddress = false;
					}
					RaisePropertyChanged("LockNodeAddress");
					
				} catch (Exception)
				{
					LockNodeAddress = "0x";
				}
			}
		}
		
		#endregion
		
		#region Localization
		
		public string LocalizationResourceSet { get; set; }
		
		#endregion
		
		#region View Switches
		
		private bool? isValidAddress;
		public bool? IsValidAddress
		{
			get { return isValidAddress; }
			set { isValidAddress = value; RaisePropertyChanged("IsValidAddress"); }
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
		
		public Action<EventConfigurationDialogInputEventViewModel> OnOk { get; set; }
		public Action<EventConfigurationDialogInputEventViewModel> OnCancel { get; set; }
		public Action<EventConfigurationDialogInputEventViewModel> OnCloseRequest { get; set; }

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
