using MvvmDialogs.ViewModels;
using System;
using System.Linq;


namespace MvvmDialogs.Presenters
{
	/// <summary>
	/// 
	/// </summary>
	public interface IDialogBoxPresenter<T> where T : IDialogViewModel
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="viewModel"></param>
		void Show(T viewModel);
	}
}
