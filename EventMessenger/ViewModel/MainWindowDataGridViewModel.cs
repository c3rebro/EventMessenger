/*
 * Created by SharpDevelop.
 * Date: 02/12/2017
 * Time: 23:43
 * 
 */
using EventMessenger.ViewModel;
using EventMessenger.Model;

using GalaSoft.MvvmLight;
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
	/// Description of ActionReactionDataGridViewModel.
	/// </summary>
	public class MainWindowDataGridViewModel : ViewModelBase
	{
		private readonly ObservableCollection<MenuItem> rowContextMenuItems;
		private readonly ObservableCollection<MenuItem> emptySpaceContextMenuItems;
		
		private readonly RelayCommand _cmdDeleteEntry;
		private readonly RelayCommand _cmdAddEntry;
		private readonly RelayCommand _cmdAddNewEvent;
		
		public MainWindowDataGridViewModel()
		{
			
			_cmdDeleteEntry = new RelayCommand(DeleteRow);
			_cmdAddEntry = new RelayCommand(AddItem);
			_cmdAddNewEvent = new RelayCommand(AddNewEvent);
			
			rowContextMenuItems = new ObservableCollection<MenuItem>();
			emptySpaceContextMenuItems = new ObservableCollection<MenuItem>();
			
			emptySpaceContextMenuItems.Add(new MenuItem()
			                               {
			                               	Header = "Add new Event",
			                               	Command = _cmdAddNewEvent
			                               });
			
			rowContextMenuItems.Add(new MenuItem()
			                        {
			                        	Header = "Add new Item",
			                        	Command = _cmdAddEntry
			                        });
			
			rowContextMenuItems.Add(new MenuItem()
			                        {
			                        	Header = "Delete",
			                        	Command = _cmdDeleteEntry
			                        });
			
			//dataGridSource = new ObservableCollection<DataGridModel>();
			
		}

		private void DeleteRow()
		{
			//dataGridSource.Remove(selectedDataGridItem);
		}
		
		private void AddItem()
		{

		}

		public void AddNewEvent()
		{
			Messenger.Default.Send<NotificationMessage<string>>(
				new NotificationMessage<string>(this, "MainWindowDataGrid", "AddNewEvent")
			);
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
	}
}
