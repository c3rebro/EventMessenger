/*
 * Created by SharpDevelop.
 * Date: 03/09/2017
 * Time: 22:41
 * 
 */
using GalaSoft.MvvmLight.Messaging;


using System;
using System.Security;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace EventMessenger.View
{
	/// <summary>
	/// Interaction logic for ResponseConfigurationDialogEmailResponseSMTPSettings.xaml
	/// </summary>
	public partial class ResponseConfigurationDialogEMailResponseSMTPSettings : Window
	{
		public ResponseConfigurationDialogEMailResponseSMTPSettings()
		{
			InitializeComponent();
			
		}
		
		public void PasswordChanged(object sender, RoutedEventArgs e)
		{
			Messenger.Default.Send<NotificationMessage<SecureString>>(
				new NotificationMessage<SecureString>(smtpPasswordBox.SecurePassword, "SMTPPassword")
			);
		}
	}
}