
using System;

namespace ProcrastinHater.BusinessInterfaces.CrudHelpers
{
	/// <summary>
	/// Helper that aids in specifying properties of a Task object.
	/// </summary>
	public class TaskInfo : ChecklistElementInfo
	{
		public TaskInfo()
			:this(null, "#FF232323", "#FFFFFFFF", "Arial", 14, DateTime.Now,
			      null, TaskStatuses.PENDING)
		{
			
		}
		
		public TaskInfo(string title, string bgCol, string fontCol,
		               	string fontName, double fontSize, DateTime beginTime,
		               	string details, TaskStatuses status)
			:base(title, bgCol, fontCol, fontName, fontSize, beginTime)
		{
			Details = details;
			Status = status;
		}
		
		public string Details { get; set; }
		
		public TaskStatuses Status {get; set;}
		
	}
}
