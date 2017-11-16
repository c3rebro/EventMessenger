using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace EventMessenger
{
	
	//	public delegate void TreeViewNodeMouseAction(object sender, TreeNodeMouseClickEventArgs e);
	

	
	
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	/// 
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow
	{
		public MainWindow()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
		}
		

		private void WindowMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			DragMove();
		}
	}
	
}