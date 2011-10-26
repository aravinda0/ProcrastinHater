
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
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
			
			DateTime today = DateTime.Now;
			_currentDate = new DateTime(today.Year, today.Month, today.Day);
			
			GetVmTreeForDate(_currentDate);
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
		
		public string CurrentDateString
		{
			get
			{
				return string.Format("{0} {1} {2}", _currentDate.Day.ToString(), 
				                     _currentDate.Month.ToString(), _currentDate.Year.ToString());
			}
		}
		
		public ICommand NextDayCommand
		{
			get
			{
				if (_nextDayCmd == null)
					_nextDayCmd = new RelayCommand((o) =>
					                               {
					                               	_currentDate = _currentDate.AddDays(1);
					                               	this.GetVmTreeForDate(_currentDate);
					                               });
				
				return _nextDayCmd;
			}
		}
		
		public ICommand PreviousDayCommand
		{
			get
			{
				if (_previousDayCmd == null)
					_previousDayCmd = new RelayCommand((o) =>
					                               {
					                               	_currentDate = _currentDate.AddDays(-1);
					                               	this.GetVmTreeForDate(_currentDate);
					                               });
				
				return _previousDayCmd;
			}
		}		
		
		
		#region private helpers
		
		private void GetVmTreeForDate(DateTime date)
		{
			GroupBLL dummyBllRootNode = new GroupBLL(0,0, new GroupInfo(), null);
			dummyBllRootNode.Items = _ceOrganizer.GetChecklistElementTreeForDate(date);
			
			GroupVM dummyVmRootNode = (ConstructVmTree(dummyBllRootNode, null) as GroupVM);
			dummyVmRootNode.CurrentIndex = -1;
			ChecklistTreeRoot = dummyVmRootNode;
			
		}		
		
		private ChecklistElementVM ConstructVmTree(ChecklistElementBLL bllNode, GroupVM parentGroup)
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
					grp.Items.Add(ConstructVmTree(ce, grp));
				
			}
			
			return ret;
		}
		
		#endregion private helpers
		
		#region private fields
		
		ITasksManager _tasksManager;
		IGroupsManager _groupsManager;
		IChecklistElementOrganizer _ceOrganizer;
		
		
		GroupVM _checklistTreeRoot;
		
		ChecklistElementVM _currentItem;
		DateTime _currentDate;
		
		RelayCommand _nextDayCmd;
		RelayCommand _previousDayCmd;
		
		
		#endregion private fields
	}
}
