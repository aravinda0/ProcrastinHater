
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
			
			GroupVM dummyVmRootNode = (CreateVmTree(dummyBllRootNode, null) as GroupVM);
			dummyVmRootNode.CurrentIndex = -1;
			ChecklistTreeRoot = dummyVmRootNode;
		}
		
		#endregion ctor
		
		
		public GroupVM ChecklistTreeRoot
		{
			get {return _checklistTreeRoot;}
			set
			{
				if (_checklistTreeRoot == value)
					return;
				
				_checklistTreeRoot = value;
				this.OnPropertyChanged("ChecklistTreeRoot");
			}
		}		
		
		public ChecklistElementVM CurrentItem
		{
			get {return _currentItem;}
			set
			{
				if (_currentItem != null && _currentItem.ParentGroup != value.ParentGroup)
				{
					//de-select old item
					_currentItem.ParentGroup.CurrentIndex = -1;
				}
				
				_currentItem = value;
//				this.OnPropertyChanged("CurrentItem");
			}
		}
		
		
		
		private ChecklistElementVM CreateVmTree(ChecklistElementBLL bllNode, GroupVM parentGroup)
		{
			ChecklistElementVM ret = null;
			
			if (bllNode is TaskBLL)
			{
				ret = EntityConverter.TaskBllToTaskVm(bllNode as TaskBLL);
				ret.ParentGroup = parentGroup;
			}
			else if (bllNode is GroupBLL)
			{
				ret = EntityConverter.GroupBllToGroupVm(bllNode as GroupBLL);
				GroupVM grp = ret as GroupVM;
				
				grp.ParentGroup = parentGroup;
				grp.Items = new ObservableCollection<ChecklistElementVM>();
				
				foreach (ChecklistElementBLL ce in (bllNode as GroupBLL).Items)
					grp.Items.Add(CreateVmTree(ce, grp));
				
			}
			
			return ret;
		}
		
		
		#region private fields
		
		ITasksManager _tasksManager;
		IGroupsManager _groupsManager;
		IChecklistElementOrganizer _ceOrganizer;
		
		
		GroupVM _checklistTreeRoot;
		
		ChecklistElementVM _currentItem;
		
		#endregion private fields
	}
}
