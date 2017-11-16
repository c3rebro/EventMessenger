/*
 * Created by SharpDevelop.
 * Date: 03/04/2017
 * Time: 21:24
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace EventMessenger.View
{
	/// <summary>
	/// Interaction logic for ResponseEditorDialogDataGrid.xaml
	/// </summary>
	public partial class ResponseEditorDialogDataGrid : UserControl
	{
		public ResponseEditorDialogDataGrid()
		{
			InitializeComponent();
		}
		
		
		private void ResponseEditorDataGrid_MouseButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (sender != null)
			{
				DataGrid grid = sender as DataGrid;
				if (grid != null && grid.SelectedItems != null && grid.SelectedItems.Count >= 1 && grid.SelectedCells.Count >= 1)
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
				}
			}
		}
	}
}