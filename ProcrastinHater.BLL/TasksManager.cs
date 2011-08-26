
using System;
using System.Collections.Generic;
using System.Linq;

using ProcrastinHater.BusinessInterfaces;
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
		
		public Task GetItemById(int id)
		{
			Task task = null;
			
			using (ProcrastinHaterEntities context = new ProcrastinHaterEntities())
			{
				task = (from t in context.ChecklistElements.OfType<Task>()
				             where t.ItemID == id select t).SingleOrDefault();
				
			}
			
			return task;
			
		}
		
		public bool AddItem(Task newItem, out string err)
		{
			throw new NotImplementedException();
		}
		
		public bool DeleteItem(int id, out string err)
		{
			throw new NotImplementedException();
		}
		
		public bool UpdateItem(int id, Task newItem, out string err)
		{
			throw new NotImplementedException();
		}
		
		#endregion CRUD
		
		public List<Task> GetTasksForDate(DateTime date)
		{
			throw new NotImplementedException();
		}		
	}
}
