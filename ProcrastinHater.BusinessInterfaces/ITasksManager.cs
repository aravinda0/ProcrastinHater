
using System;
using System.Collections.Generic;
using ProcrastinHater.POCOEntities;

namespace ProcrastinHater.BusinessInterfaces
{
	/// <summary>
	/// Contract for management of 'Task' objects.
	/// </summary>
	public interface ITasksManager : ICrudCapable<Task>
	{
		
		List<Task> GetTasksForDate(DateTime date);
	}
}
