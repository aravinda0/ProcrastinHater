
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ProcrastinHater.BusinessInterfaces.CrudHelpers;

namespace ProcrastinHater.ViewModels.ChecklistElements
{
	/// <summary>
	/// Screen-presentable Group.
	/// </summary>
	public class GroupVM : ChecklistElementVM
	{
		public GroupVM()
		{
			IsExpanded = true;
			CurrentIndex = -1;
		}
		
		public GroupVM(int id, GroupInfo groupInfo, DateTime? resolveTime, GroupVM parentGroup = null)
			:base(id, groupInfo, resolveTime, parentGroup)
		{
			IsExpanded = groupInfo.IsExpanded;
			CurrentIndex = -1;
		}
		
		#region Mapped Group properties
		
		public bool IsExpanded
		{
			get {return _isExpanded;}
			set
			{
				if (_isExpanded == value)
					return;
				
				_isExpanded = value;
				this.OnPropertyChanged("IsExpanded");
			}
		}
		
		#endregion Mapped Group properties
		
		#region Presentation related properties
		
		public int CurrentIndex
		{
			get {return _currentIndex;}
			set
			{
				if (_currentIndex == value)
					return;

				_currentIndex = value;
				this.OnPropertyChanged("CurrentIndex");
			}
		}
		
		#endregion		
		
		public ObservableCollection<ChecklistElementVM> Items
		{
			get;
			set;
		}
		
		#region private fields
		
		bool _isExpanded;
		
		int _currentIndex;
		
		#endregion private fields
		
	}
}
