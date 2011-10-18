﻿
using System;
using System.Collections.Generic;
using System.Linq;
using ProcrastinHater.BusinessInterfaces;
using ProcrastinHater.BusinessInterfaces.BLLClasses;
using ProcrastinHater.BusinessInterfaces.CrudHelpers;
using ProcrastinHater.POCOEntities;

namespace ProcrastinHater.BLL
{
	/// <summary>
	/// Manages tasks.
	/// </summary>
	public class TasksManager : ITasksManager
	{
		internal TasksManager(ProcrastinHaterEntities context)
		{
			_context = context;
		}
		
		
		#region CRUD
		
		public TaskBLL GetTaskById(int id)
		{
			TaskBLL bllTask = null;
			
			Task dalTask = (from t in _context.ChecklistElements.OfType<Task>().Include("TimedTaskSettings")
			             where t.ItemID == id select t).SingleOrDefault();
			
			if (dalTask != null)
				BLLUtility.CreateTaskBll(dalTask);
			
			return bllTask;
			
		}
		
		#region Add
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
			
			Task taskToAdd = new Task();
			TaskInfoToTask(taskInfo, taskToAdd);
			taskToAdd.ParentGroupID = parentGroupId;
			
			bool taskIsValid = true;
			bool timingInfoIsValid = true;
			
			string taskValidationErrs = "";
			taskIsValid = ValidateTask(_context, taskToAdd, out taskValidationErrs);
			
			
			TimedTaskSettings newTTS = null;
			
			string ttsValidationErrs = "";
			if (timingInfo != null)
			{
				newTTS = new TimedTaskSettings();
				TTSInfoToTTS(timingInfo, newTTS);
				
				timingInfoIsValid = ValidateTimedTaskSettings(_context, newTTS, out ttsValidationErrs);
			}
			
			
			if (!taskIsValid || !timingInfoIsValid)
				errors += taskValidationErrs + ttsValidationErrs;
			else
			{
				
				int newTaskKey = HardSettingsManager.GetAndAdvanceNextChecklistElementKey(_context);
				
				
				if (newTaskKey != -1)
				{
					if (newTTS != null)
					{
						//sigh.. validate this too?
						newTTS.TimedTaskSettingsID = HardSettingsManager.GetAndAdvanceNextTimedTaskSettingsKey(_context);
						
						taskToAdd.TimedTaskSettings = newTTS;
					}
					
					taskToAdd.ItemID = newTaskKey;
					BLLUtility.AddPositionInfo(_context, taskToAdd, parentGroupId);
					
					_context.ChecklistElements.AddObject(taskToAdd);
					_context.SaveChanges();
					
					return true;
				}
				else
					errors += "The next-key information could not be retreived from the database.\n";
			}
			
			

			return false;
		}
		
		#endregion Add

		
		#endregion CRUD
		
		

		#region private helpers
		
		#region Validation
		private bool ValidateTask(ProcrastinHaterEntities _context, Task task,
		                          out string errors)
		{
			errors = "";
			
			//using += so as to get all problems with provided Task object in one go.
			
			string checkListValiErrs;
			if (!BLLUtility.ValidateChecklistElement(_context, task, out checkListValiErrs))
				errors += checkListValiErrs + "\n";
			
			string statusIdErr = ValidateStatusId(_context, task.StatusID);
			if (statusIdErr != null)
				errors += statusIdErr + "\n";
			
			string detailsErr = ValidateTaskDetails(task.Details);
			if (detailsErr != null)
				errors += detailsErr + "\n";

			if (string.IsNullOrEmpty(errors))
				return true;
			else 
				return false;
			
		}
		
		private string ValidateStatusId(ProcrastinHaterEntities _context,
		                                int statusId)
		{
            string err = null;
            
            if (!(_context.Status.Any((st) => st.StatusID == statusId)))
            	err = "StatusId: " + statusId.ToString() + " is not a valid status.\n";
            
            return err;
		}
		
		
		private string ValidateTaskDetails(string details)
		{
			string err = null;
			
			if (details != null && details.Length > 2000)
				err = "The task details must be limited to up to 2000 characters." +
					" There are currently " + details.Length.ToString() + " characters.";
			
			return err;
		}
		
		
		
		private bool ValidateTimedTaskSettings(ProcrastinHaterEntities _context,
		                                       TimedTaskSettings tts, out string errors)
		{
			errors = "";
			
			string dueTimeErr = ValidateDueTime(tts.DueTime);
			if (dueTimeErr != null)
				errors += dueTimeErr + "\n";
			
			string timeoutActionErr = ValidateTimeoutAction(_context, tts.TimeoutActionID);
			if (timeoutActionErr != null)
				errors += timeoutActionErr + "\n";
			
			if (string.IsNullOrEmpty(errors))
				return true;
			else 
				return false;
			
		}
		
		private string ValidateDueTime(DateTime dueTime)
		{
			string err = null;
			
			TimeSpan ts = dueTime - DateTime.Now;
			if (ts.Seconds < 0)
				err = "A task's due time cannot be in the past.";
			    
			return err;
		}
		
		private string ValidateTimeoutAction(ProcrastinHaterEntities _context,
		                                     int timeoutActionId)
		{
			string err = null;
			
			if (!(_context.TimeoutActions.Any((o) => o.TimeoutActionID == timeoutActionId)))
			    err = "An invalid timeout action has been specified.";
			
			return err;
		}
        #endregion Validation
        
        #region Helper to entity class conversion
        private void TaskInfoToTask(TaskInfo ti, Task t)
        {
        	BLLUtility.ChecklistElementInfoToChecklistElement(ti, t);
        	
        	t.Details = ti.Details;
        	t.StatusID = (int)ti.Status;
        }
        
        private void TTSInfoToTTS(TimedTaskSettingsInfo ttsi, TimedTaskSettings tts)
        {
        	tts.DueTime = ttsi.DueTime;
        	tts.TimeoutActionID = (int)ttsi.TimeoutAction;
        }
		
        #endregion Helper to entity class conversion
        

		#endregion private helpers
		
		
		private ProcrastinHaterEntities _context;
	}
}
