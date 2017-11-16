using EventMessenger.ViewModel;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EventMessenger.View
{
	/// <summary>
	/// Interaction logic for DataGridView.xaml
	/// </summary>
	public partial class MainWindowDataGrid : UserControl
	{
		public MainWindowDataGrid()
		{
			InitializeComponent();
		}
		
		private void MainWindowDataGridControlMouseButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (sender != null)
			{
				DataGrid grid = sender as DataGrid;
				
				DependencyObject dep = (DependencyObject)e.OriginalSource;
				while ((dep != null) && !(dep is DataGridCell))
				{
					dep = VisualTreeHelper.GetParent(dep);
				}
				if (dep == null) 
				{
					MainWindowDataGridControl.SelectedItem = null;
					return;
				}

				if (dep is DataGridCell)
				{
					DataGridCell cell = dep as DataGridCell;
					cell.Focus();

					while ((dep != null) && !(dep is DataGridRow))
					{
						dep = VisualTreeHelper.GetParent(dep);
					}
					DataGridRow row = dep as DataGridRow;
					MainWindowDataGridControl.SelectedItem = row.DataContext;
					row.IsSelected = true;
				}
				
				/* if (grid != null && grid.SelectedItems != null && grid.SelectedItems.Count >= 1 && grid.SelectedCells.Count >= 1)
				{
					List<object> temp = new List<object>();
					
					temp = grid.SelectedItems.Cast<object>().ToList();
					
					foreach(object item in temp)
					{
						DataGridRow dgr = grid.ItemContainerGenerator.ContainerFromItem(item) as DataGridRow;
						if(dgr == null)
							grid .SelectedItem = null;
						
						else if (dgr != null && !dgr.IsMouseOver)
						{
							(dgr as DataGridRow).IsSelected = false;
						}
					}
				}*/
			}
		}
	}
}
