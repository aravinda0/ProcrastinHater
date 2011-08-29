
using System;

namespace ProcrastinHater.BusinessInterfaces.CrudHelpers
{
	/// <summary>
	/// Description of TaskInfo.
	/// </summary>
	public class TaskInfo : ChecklistElementInfo
	{
		public TaskInfo()
		{
			Status = TaskStatuses.PENDING;
		}
		
		public string Details { get; set; }
		
		public TaskStatuses Status {get; set;}
		
	}
}
