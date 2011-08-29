
using System;
using System.Collections.Generic;
using System.Linq;
using ProcrastinHater.BusinessInterfaces;
using ProcrastinHater.BusinessInterfaces.CrudHelpers;
using ProcrastinHater.POCOEntities;

namespace ProcrastinHater.BLL
{
	/// <summary>
	/// Manages 'Task' entities.
	/// </summary>
	public class TasksManager : ITasksManager
	{
		public TasksManager()
		{
			
		}
		
		
		#region CRUD
		
		public Task GetTaskById(int id)
		{
			Task task = null;
			
			using (ProcrastinHaterEntities context = new ProcrastinHaterEntities())
			{
				task = (from t in context.ChecklistElements.OfType<Task>()
				             where t.ItemID == id select t).SingleOrDefault();
				
			}
			
			return task;
			
		}
		
		public bool AddNewTask(TaskInfo taskInfo, int? parentGroupId, 
		                       out string errors)
		{
			return AddNewTask(taskInfo, parentGroupId, null, out errors);
		}
		
		public bool AddNewTask(TaskInfo taskInfo, int? parentGroupId,
		                       TimedTaskSettingsInfo timingInfo, out string errors)
		{
			errors = "";
			
			if (taskInfo == null)
			{
				errors = "The provided TaskInfo object is null.";
				return false;
			}			
			
			Task itemToAdd = new Task();
			
			
			return false;
		}
		

		
		#endregion CRUD
		
		public List<Task> GetTasksForDate(DateTime date)
		{
			throw new NotImplementedException();
		}		
		
		#region private helpers
		
		private bool ValidateTask(ProcrastinHaterEntities context, Task task,
		                          out string errors)
		{
			errors = "";
			
			//using += so as to get all problems with provided Task object in one go.
			
			string checkListValiErrs;
			if (!BLLUtility.ValidateChecklistElement(task, context, out checkListValiErrs))
				errors += checkListValiErrs + "\n";

            return false;
			
		}
		
		private string ValidateStatusId(ProcrastinHaterEntities context,
		                                int statusId)
		{
            string err = null;
            
            if (!(context.Status.Any((st) => st.StatusID == statusId)))
            	err = "StatusId: " + statusId.ToString() + " is not a valid status.\n";
            
            return err;
		}
		
        //private 
		
		#endregion
	}
}
