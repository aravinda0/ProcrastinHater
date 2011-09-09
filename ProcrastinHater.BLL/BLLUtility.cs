
using System;
using System.Linq;
using ProcrastinHater.BusinessInterfaces.BLLClasses;
using ProcrastinHater.BusinessInterfaces.CrudHelpers;
using ProcrastinHater.POCOEntities;

namespace ProcrastinHater.BLL
{
	/// <summary>
	/// General purpose stuff for use by the BLL assembly.
	/// </summary>
	internal static class BLLUtility
	{
		
		public static void ChecklistElementInfoToChecklistElement(ChecklistElementInfo cei,
		                                                          ChecklistElement ce)
		{
			ce.Title = cei.Title;
			ce.BackgroundColor = cei.BackgroundColor;
			ce.FontColor = cei.FontColor;
			ce.FontName = cei.FontName;
			ce.FontSize = cei.FontSize;
			ce.BeginTime = cei.BeginTime;
			
		}
		

		//For now, add item to end of list
		public static void AddPositionInfo(ProcrastinHaterEntities context, 
		                                   ChecklistElement item, int? parentGroupId)
		{
			item.PositionInformation = new PositionInformation();
			
			var lastItemOfGroup = (from pos in context.PositionInformations
			                       where ((pos.ChecklistElement.ParentGroupID == parentGroupId) && (pos.NextItemID == null))
			                       select pos.ChecklistElement).SingleOrDefault();
			
			if (lastItemOfGroup != null)
			{
				lastItemOfGroup.PositionInformation.NextItemID = item.ItemID;
				item.PositionInformation.PreviousItemID = lastItemOfGroup.ItemID;
				item.PositionInformation.NextItemID = null;
			}
		}
		
        #region Entity class to BLL class conversion
        
        public static TaskBLL CreateTaskBll(Task dalTask)
        {
        	
        	TimedTaskSettingsInfo ttsi = null;
        	if (dalTask.TimedTaskSettings != null)
        	{
        		ttsi = new TimedTaskSettingsInfo();
        		ttsi.DueTime = dalTask.TimedTaskSettings.DueTime;
        		ttsi.TimeoutAction = (TaskTimeoutActions)dalTask.TimedTaskSettings.TimeoutActionID;
        	}
        	
        	return  new TaskBLL(dalTask.ItemID, dalTask.ParentGroupID, 
        	                          new TaskInfo(dalTask.Title, dalTask.BackgroundColor, dalTask.FontColor, dalTask.FontName, dalTask.FontSize, dalTask.BeginTime, dalTask.Details, (TaskStatuses)dalTask.StatusID),
        	                          dalTask.ResolveTime, ttsi);
        }
        
        public static GroupBLL CreateGroupBll(Group dalGroup)
        {
        	return  new GroupBLL(dalGroup.ItemID, dalGroup.ParentGroupID, 
        	                     new GroupInfo(dalGroup.Title, dalGroup.BackgroundColor, dalGroup.FontColor, dalGroup.FontName, dalGroup.FontSize, dalGroup.BeginTime, dalGroup.IsExpanded),
        	                     dalGroup.ResolveTime);
        }
        
        #endregion Entity class to BLL class conversion		
		
		#region ChecklistElement validation
		
		public static bool ValidateChecklistElement(ProcrastinHaterEntities context,
		                                            ChecklistElement item,
		                                            out string errors)
		{
			errors = "";
			
			string titleErr = ValidateTitleField(item.Title);
			if (titleErr != null)
				errors += titleErr + "\n";
			
			string fontColErr = ValidateColorString(item.FontColor);
			if (fontColErr != null)
				errors += fontColErr + "\n";
			
			string bgColErr = ValidateColorString(item.BackgroundColor);
			if (bgColErr != null)
				errors += bgColErr + "\n";
			
			string createTimeErr = ValidateBeginTime(item.BeginTime);
			if (createTimeErr != null)
				errors += createTimeErr + "\n";
			
			string resolveTimeErr = ValidateResolveTime(item.BeginTime, item.ResolveTime);
			if (resolveTimeErr != null)
				errors += resolveTimeErr + "\n";
			
			string fontSizeErr = ValidateFontSize(item.FontSize);
			if (fontSizeErr != null)
				errors += fontSizeErr + "\n";
			
			string parentGroupIdErr = ValidateParentGroupId(context, item.ParentGroupID);
			if (parentGroupIdErr != null)
				errors += parentGroupIdErr + "\n";
			
			
			if (string.IsNullOrEmpty(errors))
				return true;
			else
				return false;
		}
		
		private static string ValidateTitleField(string title)
		{
			string err = null;
			
			if (string.IsNullOrWhiteSpace(title))
				err = "The item's title cannot be empty.";
			else if (title.Length > 800)
				err = "The item's title is too long. Limit it to 800 characters.";
						
			return err;
		}
		
		private static string ValidateColorString(string color)
		{
			string err = null;
			
			//Full namespace specified cuz of ambiguity of type 'Group' used in ValidateParentGroupId() 
			if (color.Length != 9 || System.Text.RegularExpressions.Regex.Match(color, @"#[\da-f]{8}$", 
			                                     System.Text.RegularExpressions.RegexOptions.IgnoreCase).Success == false)
				err = "The color must be a 9-character string of the format #XXXXXXXX.";

			return err;
		}
		
		private static string ValidateBeginTime(DateTime createTime)
		{
			string err = null;
			
			TimeSpan diff = DateTime.Now - createTime;
			
			//Allow future creation dates to incorporate advance planning of tasks.
			//ALT: Allow past dates?
			if (diff.Days > 0)
				err = "The creation date can be in the future, but not in the past.";
			
			return err;
		}		
		
		private static string ValidateResolveTime(DateTime createTime, DateTime? resolveTime)
		{
			string err = null;
			
			if (resolveTime != null && ((DateTime)resolveTime) < createTime)
				err = "The resolve timestamp cannot be earlier than the creation timestamp.";
			
			return err;
		}
			
		private static string ValidateFontSize(double size)
		{
			string err = null;
			
			if (size <= 0)
				err = "The font size must be a positive value.";
			
			return err;
		}
		
		private static string ValidateParentGroupId(ProcrastinHaterEntities context,
		                                            int? id)
		{
			string err = null;
			
			if (id != null)
			{
				Group parent = (from g in context.ChecklistElements.OfType<Group>()
				                where g.ItemID == id
				                select g).SingleOrDefault();
				
				if (parent == null)
					err = "No group exists with the provided ParentGroupID.\n";
			}			
			
			return err;
		}
		//TODO: Validate FontName. 

		
		#endregion ChecklistElement validation
	}
}
