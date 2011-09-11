
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ElementalMvvm;
using ProcrastinHater.BusinessInterfaces;
using ProcrastinHater.BusinessInterfaces.BLLClasses;
using ProcrastinHater.BusinessInterfaces.CrudHelpers;
using ProcrastinHater.ViewModels.ChecklistElements;
using ProcrastinHater.ViewModels.Utility;

namespace ProcrastinHater.ViewModels
{
	/// <summary>
	/// The main window's viewmodel.
	/// </summary>
	public class MainWindowVM : CloseableViewModel
	{
		
		#region ctor
		public MainWindowVM(ITasksManager tasksManager, IGroupsManager groupsManager,
		                   IChecklistElementOrganizer ceOrganizer)
		{
			_tasksManager = tasksManager;
			_groupsManager = groupsManager;
			_ceOrganizer = ceOrganizer;
			
			SetupVmTree();
		}
		
	
		
		private void SetupVmTree()
		{
			GroupBLL dummyBllRootNode = new GroupBLL(0,0, new GroupInfo(), null);
			dummyBllRootNode.Items = _ceOrganizer.GetChecklistElementTreeForDate(DateTime.Now);
			
			ChecklistElementVM dummyVmRootNode = CreateVmTree(dummyBllRootNode);
			ChecklistTree = (dummyVmRootNode as GroupVM).Items;
		}
		
		#endregion ctor
		
		
		public ObservableCollection<ChecklistElementVM> ChecklistTree
		{
			get {return _checklistTree;}
			set
			{
				if (_checklistTree == value)
					return;
				
				_checklistTree = value;
				this.OnPropertyChanged("ChecklistTree");
			}
		}		
		
		
		
		
		
		private ChecklistElementVM CreateVmTree(ChecklistElementBLL bllNode)
		{
			ChecklistElementVM ret = null;
			
			if (bllNode is TaskBLL)
				ret = EntityConverter.TaskBllToTaskVm(bllNode as TaskBLL);
			else if (bllNode is GroupBLL)
			{
				ret = EntityConverter.GroupBllToGroupVm(bllNode as GroupBLL);
				GroupVM group = ret as GroupVM;
				
				group.Items = new ObservableCollection<ChecklistElementVM>();
				foreach (ChecklistElementBLL ce in (bllNode as GroupBLL).Items)
					group.Items.Add(CreateVmTree(ce));
				
			}
			
			return ret;
		}
		
		
		#region private fields
		
		ITasksManager _tasksManager;
		IGroupsManager _groupsManager;
		IChecklistElementOrganizer _ceOrganizer;
		
		
		ObservableCollection<ChecklistElementVM> _checklistTree;
		
		#endregion private fields
	}
}
