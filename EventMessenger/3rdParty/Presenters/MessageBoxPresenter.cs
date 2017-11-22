using EventMessenger.ViewModel;
using MvvmDialogs.Presenters;
using System;
using System.Linq;
using System.Windows;

namespace EventMessenger.Presenters
{
	public class MessageBoxPresenter : IDialogBoxPresenter<MessageBoxViewModel>
	{
		public void Show(MessageBoxViewModel vm)
		{
			vm.Result = MessageBox.Show(vm.Message, vm.Caption, vm.Buttons, vm.Image);
		}
	}
}
