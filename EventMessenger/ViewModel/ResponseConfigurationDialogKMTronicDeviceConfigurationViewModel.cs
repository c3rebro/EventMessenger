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
	/// Description of ResponseConfigurationDialogKMTronicDeviceConfigurationViewModel.
	/// </summary>
	public class ResponseConfigurationDialogKMTronicDeviceConfigurationViewModel : ViewModelBase, IUserDialogViewModel
	{
		private KMTronicModel kmTronicDevice;
		private int portNumberAsInteger;
		
		private readonly ObservableCollection<MenuItem> rowContextMenuItems;
		private readonly ObservableCollection<MenuItem> emptySpaceContextMenuItems;
		
		private readonly RelayCommand _cmdDeleteEntry;
		private readonly RelayCommand _cmdAddNewRelayContact;
		private readonly RelayCommand _cmdAddNewResponse;

		private readonly ResourceLoader resLoader;
		
		private ResponseObjectModel responseObjectModel;
		
		public ResponseConfigurationDialogKMTronicDeviceConfigurationViewModel(ObservableCollection<ResponseObjectModel> _responseCollection, ResponseObjectModel _responseObject)
		{
			if(_responseObject != null)
			{
				responseObjectModel = _responseObject;
				
				UserName = (responseObjectModel.ResponseObject as KMTronicModel).UserName;
				Password = (responseObjectModel.ResponseObject as KMTronicModel).Password;
				
				HostName = (responseObjectModel.ResponseObject as KMTronicModel).HostName;
				PortNumber = (responseObjectModel.ResponseObject as KMTronicModel).PortNumber.ToString();
				
				RelayContactCollection = (responseObjectModel.ResponseObject as KMTronicModel).RelayContactCollection;
				
				SelectedDevice = (responseObjectModel.ResponseObject as KMTronicModel).DeviceType;
			}
			else
			{
				HostName = "localhost";
				PortNumber = "80";
				
				UserName = "admin";
				Password = "admin";
				
				SelectedDevice = KMTronicDeviceType.W2CR;
				
				kmTronicDevice = new KMTronicModel(SelectedDevice);
				
				responseObjectModel = new ResponseObjectModel(kmTronicDevice);
				
				RelayContactCollection = new ObservableCollection<RelayContact>();
				
			}
			
			resLoader = new ResourceLoader();
			
			_cmdDeleteEntry = new RelayCommand(null);
			_cmdAddNewRelayContact = new RelayCommand(AddRelayContact);
			
			rowContextMenuItems = new ObservableCollection<MenuItem>();
			emptySpaceContextMenuItems = new ObservableCollection<MenuItem>();
			
			emptySpaceContextMenuItems.Add(new MenuItem(){
			                               	Header = resLoader.getResource("contextMenuItemAddNewRelayContact"),
			                               	Command = _cmdAddNewRelayContact
			                               });
			
			rowContextMenuItems.Add(new MenuItem(){
			                        	Header = resLoader.getResource("contextMenuItemEditRelayContact"),
			                        	Command = _cmdAddNewRelayContact
			                        });

			rowContextMenuItems.Add(new MenuItem(){
			                        	Header = resLoader.getResource("contextMenuItemDeleteSelectedItem"),
			                        	Command = _cmdDeleteEntry
			                        });
			IsModal = true;
		}
		
		#region Dialogs
		
		private ObservableCollection<IDialogViewModel> dialogs = new ObservableCollection<IDialogViewModel>();
		public ObservableCollection<IDialogViewModel> Dialogs { get { return dialogs; } }
		
		/// <summary>
		/// expose contextmenu on row click
		/// </summary>
		public ObservableCollection<MenuItem> RowContextMenu {
			get {
				return rowContextMenuItems;
			}
		}

		/// <summary>
		/// expose context menu on blank area click
		/// </summary>
		public ObservableCollection<MenuItem> EmptySpaceContextMenu {
			get {
				return emptySpaceContextMenuItems;
			}
		}
		
		#endregion
		public bool CollectionContainsElements
		{
			get { try { return !RelayContactCollection.Any(); } catch { return false; } }
		}
		
		public bool IsPortNumberValidInteger
		{
			get { return int.TryParse(PortNumber, out portNumberAsInteger); }
		}
		
		public KMTronicDeviceType SelectedDevice
		{
			get { return selectedDevice; }
			set {
				selectedDevice = value;
				RaisePropertyChanged("SelectedDevice");
			}
		} private KMTronicDeviceType selectedDevice;
		
		public string HostName
		{
			get { return hostName; }
			set {
				hostName = value;
				RaisePropertyChanged("HostName");
			}
		} private string hostName;
		
		public string UserName
		{
			get { return userName; }
			set {
				userName = value;
				RaisePropertyChanged("UserName");
			}
		} private string userName;
		
		public string PortNumber
		{
			get { return portNumber; }
			set {
				portNumber = value;
				RaisePropertyChanged("IsPortNumberValidInteger");
				RaisePropertyChanged("PortNumber");
			}
		} private string portNumber;

		public string Password
		{
			get { return password; }
			set {
				password = value;
				RaisePropertyChanged("Password");
			}
		} private string password;
		
		/// <summary>
		/// 
		/// </summary>
		public RelayContact SelectedRelayContact
		{
			get { return selectedRelayContact; }
			set {
				selectedRelayContact = value;
				RaisePropertyChanged("CollectionContainsElements");
				RaisePropertyChanged("SelectedRelayContact");
			}
		} private RelayContact selectedRelayContact;
		
		/// <summary>
		/// 
		/// </summary>
		public ObservableCollection<RelayContact> RelayContactCollection
		{
			get {
				return relayContactCollection;
			}
			set {
				relayContactCollection = value;
				RaisePropertyChanged("RelayContactCollection");
			}
		} private ObservableCollection<RelayContact> relayContactCollection;
		
		#region Commands
		
		private void AddRelayContact()
		{
			this.Dialogs.Add(new ResponseConfigurationDialogKMTronicDeviceWebRelayConfigurationDialogViewModel(SelectedRelayContact)
			                 {
			                 	OnOk = (sender) =>
			                 	{
			                 		if(sender.IsRelayNumberValidInteger && sender.IsTimeValidInteger)
			                 		{
			                 			if(SelectedRelayContact != null)
			                 			{
			                 				SelectedRelayContact.Behavior = sender.Behavior;
			                 				SelectedRelayContact.RelayNumber = Convert.ToInt32(sender.RelayNumber);
			                 				SelectedRelayContact.Time = Convert.ToInt32(sender.Time);
			                 			}
			                 			else{
			                 				RelayContact rel = new RelayContact();
			                 				rel.Behavior = sender.Behavior;
			                 				rel.RelayNumber = Convert.ToInt32(sender.RelayNumber);
			                 				
			                 				if(sender.Behavior == KMTronicRelayContactBehavior.RelayContactIsTimed)
			                 					rel.Time = Convert.ToInt32(sender.Time);
			                 				else
			                 					rel.Time = 0;
			                 				
			                 				if(
			                 					!RelayContactCollection.Where(x => x.RelayNumber == Convert.ToInt32(sender.RelayNumber)).Any()
			                 					&& (SelectedDevice == KMTronicDeviceType.W2CR ?
			                 					    ((RelayContactCollection.Count < 2 ? true : false) && Convert.ToInt32(sender.RelayNumber) <= 2) :
			                 					    (RelayContactCollection.Count < 8 ? true : false) && Convert.ToInt32(sender.RelayNumber) <= 8)
			                 				)
			                 					RelayContactCollection.Add(rel);
			                 				else{
			                 					
			                 				}
			                 				
			                 			}
			                 			
			                 			

			                 			
			                 			RaisePropertyChanged("CollectionContainsElements");
			                 			RaisePropertyChanged("RelayContactCollection");
			                 			
			                 			sender.Close();
			                 		}
			                 		
			                 		else
			                 			
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
		#endregion
		
		#region Localization
		public string Caption
		{
			get { using (var resMan = new ResourceLoader()) { return resMan.getResource("windowCaptionResponseConfigurationDialogAddKMTronicDevice"); } }
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
		
		public Action<ResponseConfigurationDialogKMTronicDeviceConfigurationViewModel> OnOk { get; set; }
		public Action<ResponseConfigurationDialogKMTronicDeviceConfigurationViewModel> OnCancel { get; set; }
		public Action<ResponseConfigurationDialogKMTronicDeviceConfigurationViewModel> OnCloseRequest { get; set; }

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
