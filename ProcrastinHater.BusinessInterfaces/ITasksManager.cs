
using System;
using System.Collections.Generic;

using ProcrastinHater.POCOEntities;
using ProcrastinHater.BusinessInterfaces.CrudHelpers;

namespace ProcrastinHater.BusinessInterfaces
{
	/// <summary>
	/// Contract for management of 'Task' objects.
	/// </summary>
	public interface ITasksManager 
	{
		Task GetTaskById(int id);
		bool AddNewTask(TaskInfo taskInfo, int? parentGroupId, out string errors);
		bool AddNewTask(TaskInfo taskInfo, int? parentGroupId, 
		                TimedTaskSettingsInfo timingInfo, out string errors);
		
		List<Task> GetTasksForDate(DateTime date);
	}
}
