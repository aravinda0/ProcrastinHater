
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
	/// Manages 'Task' entities.
	/// </summary>
	public class TasksManager : ITasksManager
	{
		public TasksManager()
		{
			
		}
		
		
		#region CRUD
		
		public TaskBLL GetTaskById(int id)
		{
			TaskBLL bllTask = null;
			
			using (ProcrastinHaterEntities context = new ProcrastinHaterEntities())
			{
				Task dalTask = (from t in context.ChecklistElements.OfType<Task>().Include("TimedTaskSettings")
				             where t.ItemID == id select t).SingleOrDefault();
				
				if (dalTask != null)
					BLLUtility.CreateTaskBll(dalTask);
			}
			
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
			
			using (ProcrastinHaterEntities context = new ProcrastinHaterEntities())
			{
				bool taskIsValid = true;
				bool timingInfoIsValid = true;
				
				string taskValidationErrs = "";
				taskIsValid = ValidateTask(context, taskToAdd, out taskValidationErrs);
				
				
				TimedTaskSettings newTTS = null;
				
				string ttsValidationErrs = "";
				if (timingInfo != null)
				{
					newTTS = new TimedTaskSettings();
					TTSInfoToTTS(timingInfo, newTTS);
					
					timingInfoIsValid = ValidateTimedTaskSettings(context, newTTS, out ttsValidationErrs);
				}
				
				
				if (!taskIsValid || !timingInfoIsValid)
					errors += taskValidationErrs + ttsValidationErrs;
				else
				{
					
					int newTaskKey = HardSettingsManager.GetAndAdvanceNextChecklistElementKey(context);
					
					
					if (newTaskKey != -1)
					{
						if (newTTS != null)
						{
							//sigh.. validate this too?
							newTTS.TimedTaskSettingsID = HardSettingsManager.GetAndAdvanceNextTimedTaskSettingsKey(context);
							
							taskToAdd.TimedTaskSettings = newTTS;
						}
						
						taskToAdd.ItemID = newTaskKey;
						
						context.ChecklistElements.AddObject(taskToAdd);
						context.SaveChanges();
						
						return true;
					}
					else
						errors += "The next-key information could not be retreived from the database.\n";
				}
			}
			
			

			return false;
		}
		
		#endregion Add

		
		#endregion CRUD
		
		

		#region Get tree from position information
		
		/// <summary>
		/// Get the latest set of elements, which happen to be position-tracked
		/// in the database.
		/// </summary>
		/// <returns>Returns a tree of ChecklistElements with items organized 
		/// as per the order stored in the database.</returns>
		public List<ChecklistElementBLL> GetChecklistElementTreeFromPositionInfo()
		{
			List<ChecklistElementBLL> tree = null;
			
			using (ProcrastinHaterEntities context = new ProcrastinHaterEntities())
			{
				
				if (context.PositionInformations.Count() > 0)
				{
					//ALT: use Distinct() with custom comparer?
					var firstItemsOfEveryGroup = (from pi in context.PositionInformations
					                             where pi.PreviousItemID == null
					                             select pi.ChecklistElement);
					
					var v = firstItemsOfEveryGroup.ToList();
					
					
					Dictionary<int, List<ChecklistElementBLL>> groupIdToGroupItemsMap = new Dictionary<int, List<ChecklistElementBLL>>();
					
					foreach (ChecklistElement fi in firstItemsOfEveryGroup)
					{
						List<ChecklistElementBLL> orderedGroupMembers = GetOrderedConvertedSiblings(context,fi);
						
						if (fi.ParentGroupID == null)
							groupIdToGroupItemsMap[-1] = orderedGroupMembers; //top level elements with ParentGroupID == null
						else
							groupIdToGroupItemsMap[(int)fi.ParentGroupID] = orderedGroupMembers;
					}
					
					tree = groupIdToGroupItemsMap[-1];
					
					var topLevelGroups = (from ce in tree 
					                      where ce is GroupBLL
					                      select ce as GroupBLL);
					
					Queue<GroupBLL> childlessUnprocessedGroupsQ = new Queue<GroupBLL>(topLevelGroups);
					
					while (childlessUnprocessedGroupsQ.Count > 0)
					{
						GroupBLL g = childlessUnprocessedGroupsQ.Dequeue();
						g.Items = groupIdToGroupItemsMap[g.ItemID];
						
						var groups = (from ce in g.Items 
				                      where ce is GroupBLL
				                      select ce as GroupBLL);
						
						
						foreach (GroupBLL gg in groups)
							childlessUnprocessedGroupsQ.Enqueue(gg);
					}
					
					return tree;
					
				}
				
				
			}
			
			return null;
		}
		
		private List<ChecklistElementBLL> GetOrderedConvertedSiblings(ProcrastinHaterEntities context, ChecklistElement firstGroupItem)
		{
			//param firstGroup will never be null, so no check needed.
			
			List<ChecklistElementBLL> ret = new List<ChecklistElementBLL>();
			
			
			ChecklistElement ce = firstGroupItem;
			do
			{
				ChecklistElementBLL bllCE = null;
				if (ce is Task)
				{
					Task t = ce as Task;
//					context.LoadProperty(t, o => o.TimedTaskSettings); // Is this needed?
					bllCE = BLLUtility.CreateTaskBll(t);
				}
				else if (ce is Group)
				{
					bllCE = BLLUtility.CreateGroupBll(ce as Group);		
				}
				
				ret.Add(bllCE);
				
			} while ((ce = ce.PositionInformation.NextItem) != null);
			
			return ret;
		}
		
		#endregion Get tree from position information
		
		
		#region private helpers
		
		#region Validation
		private bool ValidateTask(ProcrastinHaterEntities context, Task task,
		                          out string errors)
		{
			errors = "";
			
			//using += so as to get all problems with provided Task object in one go.
			
			string checkListValiErrs;
			if (!BLLUtility.ValidateChecklistElement(task, context, out checkListValiErrs))
				errors += checkListValiErrs + "\n";
			
			string statusIdErr = ValidateStatusId(context, task.StatusID);
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
		
		private string ValidateStatusId(ProcrastinHaterEntities context,
		                                int statusId)
		{
            string err = null;
            
            if (!(context.Status.Any((st) => st.StatusID == statusId)))
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
		
		
		
		private bool ValidateTimedTaskSettings(ProcrastinHaterEntities context,
		                                       TimedTaskSettings tts, out string errors)
		{
			errors = "";
			
			string dueTimeErr = ValidateDueTime(tts.DueTime);
			if (dueTimeErr != null)
				errors += dueTimeErr + "\n";
			
			string timeoutActionErr = ValidateTimeoutAction(context, tts.TimeoutActionID);
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
		
		private string ValidateTimeoutAction(ProcrastinHaterEntities context,
		                                     int timeoutActionId)
		{
			string err = null;
			
			if (!(context.TimeoutActions.Any((o) => o.TimeoutActionID == timeoutActionId)))
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
	}
}
