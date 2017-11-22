using System;
using System.Linq;

namespace MvvmDialogs.ViewModels
{
	/// <summary>
	/// IUserDialogViewModel inherit from propertychanged
	/// </summary>
	public interface IUserDialogViewModel : IDialogViewModel
	{
		/// <summary>
		/// if set, the Dialog is Modal
		/// </summary>
		bool IsModal { get; }
		/// <summary>
		/// call dispose
		/// </summary>
		void RequestClose();
		/// <summary>
		/// delegate to requestclose
		/// </summary>
		event EventHandler DialogClosing;
	}
}
