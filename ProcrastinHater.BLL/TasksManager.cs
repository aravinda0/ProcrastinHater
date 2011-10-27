
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
				bllTask = BLLUtility.CreateTaskBll(dalTask);
			
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
        	taskToAdd.StatusID = 100;
			taskToAdd.ParentGroupID = parentGroupId;
			
			bool taskIsValid = true;
			bool timingInfoIsValid = true;
			
			string taskValidationErrs = "";
			taskIsValid = ValidateTask(taskToAdd, out taskValidationErrs);
			
			
			TimedTaskSettings newTTS = null;
			
			string ttsValidationErrs = "";
			if (timingInfo != null)
			{
				newTTS = new TimedTaskSettings();
				TTSInfoToTTS(timingInfo, newTTS);
				
				timingInfoIsValid = ValidateTimedTaskSettings(newTTS, out ttsValidationErrs);
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
		
		#region Update

		/// <summary>
		/// Update task's information. For changing task's 'Status', use ChangeStatus().
		/// </summary>
		public bool UpdateTaskDescriptors(int id, TaskInfo taskInfo, out string errors)
		{
			/*
				If task did not have 'PositionInformation' and new 'BeginTime'
				is within position-tracked days, add a PosInfo to it. 
				Pos = end of parent group (take care of top lvl task, parent == null).
				
				Besides that, just update properties.
			*/
			//TODO: Ponder about the need to allow changes on ResolveTime (via another method...  not this one)
			
			errors = "";
			
			Task taskToUpdate = _context.ChecklistElements.OfType<Task>()
				.SingleOrDefault(t => t.ItemID == id);
			
			if (taskToUpdate != null)
			{
				HardSettings settings = _context.HardSettingsSet.SingleOrDefault(hs => hs.Check == true);
				
				if (settings != null)
				{
					TaskInfoToTask(taskInfo, taskToUpdate);
					
					if (ValidateTask(taskToUpdate, out errors))
					{
						CheckIfNewBeginTimeAffectsPosInfo(taskToUpdate, settings);
						
						_context.SaveChanges();
						return true;
					}
					else
						return false;
					
				}
				else
				{
					errors = "The database is corrupt. Settings info could not be retreived.";
					return false;
				}
			}
			else
			{
				errors = "No Task with the specified ItemID exists.";
				return false;				
			}
			
		}
		
		private void CheckIfNewBeginTimeAffectsPosInfo(Task taskToUpdate, HardSettings settings)
		{
			int numPosTrackedDays = settings.DaysOfHistoryToShow;
			DateTime posTrackingStartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month,
			                                             DateTime.Now.Day - numPosTrackedDays);
		
			if (taskToUpdate.PositionInformation == null)
			{
				//BeginTime changed from non-pos-tracking timestamp to pos-tracking timestamp
				
				if (taskToUpdate.BeginTime >= posTrackingStartDate && taskToUpdate.BeginTime <= DateTime.Now)
				{
					//need to add PositionInfo. Add task to end of Group.
					PositionInformation pi = new PositionInformation();
					
					ChecklistElement lastItemOfGroup = taskToUpdate.ParentGroup.ChecklistElements
						.SingleOrDefault(ce => ce.PositionInformation != null && ce.PositionInformation.NextItem == null);
					
					if (lastItemOfGroup != null)
					{
						lastItemOfGroup.PositionInformation.NextItem = taskToUpdate;
						pi.PreviousItem = lastItemOfGroup; 
					}
					//else the group is currently empty
					
					taskToUpdate.PositionInformation = pi;
				}
			}
			else
			{
				//BeginTime changed from pos-tracking timestamp to non-pos-tracking timestamp
				//just remove PosInfo.
				
				if (taskToUpdate.BeginTime < posTrackingStartDate || taskToUpdate.BeginTime > DateTime.Now)
				{
					RemovePosInfoFromLinkedList(taskToUpdate);
				}
			}
		}
		
		private void RemovePosInfoFromLinkedList(ChecklistElement ce)
		{
			ChecklistElement prevItem = ce.PositionInformation.PreviousItem;
			ChecklistElement nextItem = ce.PositionInformation.NextItem;
			if (prevItem != null)
				prevItem.PositionInformation.NextItem = nextItem;
			if (nextItem != null)
				nextItem.PositionInformation.PreviousItem = prevItem;
				
			_context.PositionInformationSet.DeleteObject(ce.PositionInformation);			
		}

		#endregion Update
		
		#endregion CRUD
		
		

		#region private helpers
		
		#region Validation
		private bool ValidateTask(Task task, out string errors)
		{
			errors = "";
			
			//using += so as to get all problems with provided Task object in one go.
			
			string checkListValiErrs;
			if (!BLLUtility.ValidateChecklistElement(_context, task, out checkListValiErrs))
				errors += checkListValiErrs + "\n";
			
			string statusIdErr = ValidateStatusId(task.StatusID);
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
		
		private string ValidateStatusId(int statusId)
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
		
		
		
		private bool ValidateTimedTaskSettings(TimedTaskSettings tts, out string errors)
		{
			errors = "";
			
			string dueTimeErr = ValidateDueTime(tts.DueTime);
			if (dueTimeErr != null)
				errors += dueTimeErr + "\n";
			
			string timeoutActionErr = ValidateTimeoutAction(tts.TimeoutActionID);
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
		
		private string ValidateTimeoutAction(int timeoutActionId)
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
