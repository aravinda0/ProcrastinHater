
using System;
using System.Collections.Generic;

using ElementalMvvm;
using ProcrastinHater.BusinessInterfaces;
using ProcrastinHater.POCOEntities;

namespace ProcrastinHater.ViewModels
{
	/// <summary>
	/// The main window's viewmodel.
	/// </summary>
	public class MainWindowVM : CloseableViewModel
	{
		public MainWindowVM(ITasksManager tasksManager)
		{
			_tasksManager = tasksManager;
		}
		
		
		#region private fields
		
		ITasksManager _tasksManager;
		
		#endregion private fields
	}
}
