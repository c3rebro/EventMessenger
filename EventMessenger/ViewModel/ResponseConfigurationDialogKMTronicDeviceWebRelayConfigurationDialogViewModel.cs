/*
 * Created by SharpDevelop.
 * Date: 27.09.2017
 * Time: 21:37
 * 
 */
using EventMessenger.Model;
using EventMessenger.ViewModel;
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
	/// Description of ResponseConfigurationDialogKMTronicDeviceWebRelayConfigurationDialogViewModel.
	/// </summary>
	public class ResponseConfigurationDialogKMTronicDeviceWebRelayConfigurationDialogViewModel : ViewModelBase, IUserDialogViewModel
	{
		#region fields
		private readonly ResourceLoader resLoader;
		private int timeAsInteger;
		private int relayNumberAsInteger;
		#endregion
		
		#region contructors
		
		public ResponseConfigurationDialogKMTronicDeviceWebRelayConfigurationDialogViewModel(RelayContact _responseObject)
		{
			if(_responseObject != null)
			{
				Behavior = _responseObject.Behavior;
				Time = _responseObject.Time.ToString();

				RelayNumber = _responseObject.RelayNumber.ToString();
				
			}
			else
			{
				RelayNumber = "1";
				Time = "0";
				Behavior = KMTronicRelayContactBehavior.RelayContactIsStaticOn;
			}
			
			IsModal = true;
		}
		
		#endregion
		
		#region Dialogs
		
		private ObservableCollection<IDialogViewModel> dialogs = new ObservableCollection<IDialogViewModel>();
		public ObservableCollection<IDialogViewModel> Dialogs { get { return dialogs; } }
		
		#endregion
		
		#region dependency properties
		public KMTronicRelayContactBehavior Behavior
		{
			get	{ return behavior;	}
			
			set
			{
				Time = "5";
				behavior = value;
				RaisePropertyChanged("ContactHasTime");
				RaisePropertyChanged("Behavior");
			}
		} private KMTronicRelayContactBehavior behavior;
		
		public string RelayNumber
		{
			get { return relayNumber; }
			set
			{
				relayNumber = value;
				RaisePropertyChanged("IsRelayNumberValidInteger");
				RaisePropertyChanged("RelayNumber");
			}
		} private string relayNumber;
		
		public string Time
		{
			get { return time; }
			set {
				time = value;
				RaisePropertyChanged("IsTimeValidInteger");
				RaisePropertyChanged("Time");
			}
		} private string time;
		
		public bool ContactHasTime
		{
			get { return Behavior == KMTronicRelayContactBehavior.RelayContactIsTimed; }
		}
		
		public bool IsTimeValidInteger
		{
			get { return int.TryParse(Time, out timeAsInteger);	}
		}
		
		public bool IsRelayNumberValidInteger
		{
			get { return int.TryParse(RelayNumber, out relayNumberAsInteger);	}
		}
		#endregion
		
		#region Localization
		public string Caption
		{
			get { using (var resMan = new ResourceLoader()) { return resMan.getResource("windowCaptionResponseConfigurationDialogAddKMTronicWebRelayContact"); } }
		}
		
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
		
		public Action<ResponseConfigurationDialogKMTronicDeviceWebRelayConfigurationDialogViewModel> OnOk { get; set; }
		public Action<ResponseConfigurationDialogKMTronicDeviceWebRelayConfigurationDialogViewModel> OnCancel { get; set; }
		public Action<ResponseConfigurationDialogKMTronicDeviceWebRelayConfigurationDialogViewModel> OnCloseRequest { get; set; }

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
