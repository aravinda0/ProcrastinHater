
using System;
using ProcrastinHater.BusinessInterfaces.CrudHelpers;

namespace ProcrastinHater.ViewModels.ChecklistElements
{
	/// <summary>
	/// Screen-presentable Task.
	/// </summary>
	public class TaskVM : ChecklistElementVM
	{
		public TaskVM()
		{
			Status = TaskStatuses.PENDING;
		}
		
		public TaskVM(int id, TaskInfo taskInfo, DateTime? resolveTime, GroupVM parentGroup = null)
			:base(id, taskInfo, resolveTime, parentGroup)
		{
			Details = taskInfo.Details;
			Status = taskInfo.Status;
		}
		
		#region Mapped Task properties
		
		public string Details
		{
			get {return _details;}
			set
			{
				if (_details == value)
					return;
				
				_details = value;
				this.OnPropertyChanged("Details");
			}
		}
		
		public TaskStatuses Status
		{
			get {return _status;}
			set
			{
				if (_status == value)
					return;
				
				_status = value;
				this.OnPropertyChanged("Status");
			}
		}
		
		#endregion Mapped Task properties
		
		
		#region private fields
		
		string _details;
		TaskStatuses _status;
		
		#endregion private fields
	}
}
