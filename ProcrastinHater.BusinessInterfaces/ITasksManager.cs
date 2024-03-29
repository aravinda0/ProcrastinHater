﻿
using System;
using System.Collections.Generic;
using ProcrastinHater.BusinessInterfaces.BLLClasses;
using ProcrastinHater.BusinessInterfaces.CrudHelpers;

namespace ProcrastinHater.BusinessInterfaces
{
	/// <summary>
	/// Contract for management of tasks.
	/// </summary>
	public interface ITasksManager 
	{
		TaskBLL GetTaskById(int id);
		bool AddNewTask(TaskInfo taskInfo, int? parentGroupId, out string errors);
		bool AddNewTask(TaskInfo taskInfo, int? parentGroupId, 
		                TimedTaskSettingsInfo timingInfo, out string errors);
		bool UpdateTaskDescriptors(int id, TaskInfo taskInfo, out string errors);
		
	}
}
