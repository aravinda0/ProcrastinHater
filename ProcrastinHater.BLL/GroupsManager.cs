
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
		internal GroupsManager(ProcrastinHaterEntities context)
		{
			_context = context;
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
				
				if (ValidateGroup(groupToAdd, out errors))
				{
					int newGroupId = HardSettingsManager.GetAndAdvanceNextChecklistElementKey(_context);
					
					if (newGroupId != -1)
					{
						groupToAdd.ItemID = newGroupId;
						BLLUtility.AddPositionInfo(_context, groupToAdd, parentGroupId);
						
						_context.ChecklistElements.AddObject(groupToAdd);
						_context.SaveChanges();
					}
					else
						errors = "The next key information could not be retrieved from the database.";
				}
			}
			
			return false;
		}
		
		/// <summary>
		/// This method is used to update details of existing 'Groups'. To change a 
		/// Group's parent, ie. to move a Group from one containing Group to another,
		/// use the methods in the ChecklistElementOrganizer.
		/// </summary>
		public bool UpdateGroup(int id, GroupInfo gInfo, out string errors)
		{
			errors = "";
			
			Group groupToUpdate = _context.ChecklistElements.OfType<Group>()
				.Where(g => g.ItemID == id).SingleOrDefault();
			
			if (groupToUpdate != null)
			{
				GroupInfoToGroup(gInfo, groupToUpdate);
				
				if (ValidateGroup(groupToUpdate, out errors))
				{
					_context.SaveChanges();
					return true;
				}
				else
					return false;
			}
			else
			{
				errors = "No Group with the specified ItemID exists.";
				return false;
			}
		}
		
		#region Group validation
		private bool ValidateGroup(Group group, out string errors)
		{
			errors = "";
			
			string ceValiErrs = null;
			if (!BLLUtility.ValidateChecklistElement(_context, group, out ceValiErrs))
				errors += ceValiErrs;
			
			//possible future stuff.
			
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
		
		private ProcrastinHaterEntities _context;
	}
}
