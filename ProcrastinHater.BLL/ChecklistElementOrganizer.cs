
using System;
using System.Collections.Generic;
using System.Linq;

using ProcrastinHater.BusinessInterfaces;
using ProcrastinHater.BusinessInterfaces.BLLClasses;
using ProcrastinHater.POCOEntities;

namespace ProcrastinHater.BLL
{
	/// <summary>
	/// Handles ordering & positioning of ChecklistElementBLLs.
	/// </summary>
	public class ChecklistElementOrganizer : IChecklistElementOrganizer
	{
		public ChecklistElementOrganizer()
		{
		}
		
		

		#region Get tree from position information
		
		/// <summary>
		/// Get the latest set of elements, which happen to be position-tracked
		/// in the database.
		/// </summary>
		/// <returns>Returns a tree of ChecklistElements with items organized 
		/// as per the user's' ordering, stored in the database.</returns>
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
		
				
	}
}
