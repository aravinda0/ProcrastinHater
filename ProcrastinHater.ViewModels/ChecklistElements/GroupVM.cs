
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
		}
		
		public GroupVM(int id, GroupInfo groupInfo, DateTime? resolveTime, GroupVM parentGroup)
			:base(id, groupInfo, resolveTime, parentGroup)
		{
			IsExpanded = groupInfo.IsExpanded;
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
		
		
		
		public ObservableCollection<ChecklistElementVM> Items
		{
			get;
			set;
		}
		
		#region private fields
		
		bool _isExpanded;
		
		#endregion private fields
		
	}
}
