
using System;
using System.Collections.Generic;

using ElementalMvvm;
using ProcrastinHater.BusinessInterfaces;

namespace ProcrastinHater.ViewModels
{
	/// <summary>
	/// The main window's viewmodel.
	/// </summary>
	public class MainWindowVM : CloseableViewModel
	{
		public MainWindowVM(ITasksManager tasksManager, IGroupsManager groupsManager,
		                   IChecklistElementOrganizer ceOrganizer)
		{
			_tasksManager = tasksManager;
			_groupsManager = groupsManager;
			_ceOrganizer = ceOrganizer;
		}
		
		
		#region private fields
		
		ITasksManager _tasksManager;
		IGroupsManager _groupsManager;
		IChecklistElementOrganizer _ceOrganizer;
		
		#endregion private fields
	}
}
