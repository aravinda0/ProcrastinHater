
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
	/// Manages groups.
	/// </summary>
	public class GroupsManager : IGroupsManager
	{
		public GroupsManager()
		{
		}
		
		
		public bool AddNewGroup(GroupInfo gInfo, Nullable<int> parentGroupId, out string errors)
		{
			errors = "";
			
			if (gInfo == null)
			{
				errors += "The provided GroupInfo object is null.";
			}
			else
			{
				Group groupToAdd = new Group();
				GroupInfoToGroup(gInfo, groupToAdd);
				groupToAdd.ParentGroupID = parentGroupId;
				
				using (ProcrastinHaterEntities context = new ProcrastinHaterEntities())
				{
					if (ValidateGroup(context, groupToAdd, out errors))
					{
						int newGroupId = HardSettingsManager.GetAndAdvanceNextChecklistElementKey(context);
						
						if (newGroupId != -1)
						{
							groupToAdd.ItemID = newGroupId;
							context.ChecklistElements.AddObject(groupToAdd);
							context.SaveChanges();
						}
						else
							errors = "The next key information could not be retrieved from the database.";
					}
				}
			}
			
			return false;
		}
		
		
		#region Group validation
		private bool ValidateGroup(ProcrastinHaterEntities context, Group group, out string errors)
		{
			errors = "";
			
			string ceValiErrs = null;
			if (!BLLUtility.ValidateChecklistElement(context, group, out ceValiErrs))
				errors += ceValiErrs;
			
			//space reserved for possible future stuff.
			
			if (string.IsNullOrWhiteSpace(errors))
				return true;
			else
				return false;
		}
		
		#endregion Group validation
		
		
		private void GroupInfoToGroup(GroupInfo gi, Group g)
		{
			BLLUtility.ChecklistElementInfoToChecklistElement(gi, g);
			
			g.IsExpanded = gi.IsExpanded;
		}
	}
}
