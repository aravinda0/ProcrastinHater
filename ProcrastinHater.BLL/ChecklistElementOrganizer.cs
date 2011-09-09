
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
		
		/// <summary>
		/// Get tree of items that are active for the give date.
		/// </summary>
		/// <param name="date"></param>
		/// <returns></returns>
		public List<ChecklistElementBLL> GetChecklistElementTreeForDate(DateTime date)
		{
			List<ChecklistElementBLL> topLvlNodes = new List<ChecklistElementBLL>();
			
			using (ProcrastinHaterEntities context = new ProcrastinHaterEntities())
			{
				
				//first get tasks(leaf nodes) only.
				var allTasksForDateGroupedByParents = (from t in context.ChecklistElements.OfType<Task>()
				                                       where date >= t.BeginTime && (t.ResolveTime == null || date < (DateTime)t.ResolveTime)
				                                       group t by t.ParentGroup into bucket
				                                       select new {Group = bucket.Key, Items = bucket});
				
				//group id to its members
				Dictionary<int, List<ChecklistElementBLL>> groupToMembersMap = new Dictionary<int, List<ChecklistElementBLL>>();
				Queue<Group> unconvertedGroupsQ = new Queue<Group>();
					
				//So far we have tasks only. Put each set of tasks in appropriate ChklistBLL collections.
				//And populate queue to begin work on organizing 'Group' objects..
				foreach (var bucket in allTasksForDateGroupedByParents)
				{					
					int key = (bucket.Group == null)?(-1):(bucket.Group.ItemID);
					
					List<ChecklistElementBLL> coll = new List<ChecklistElementBLL>();
					
					//add PARENT GROUP of current task set
					if (bucket.Group != null)
						unconvertedGroupsQ.Enqueue(bucket.Group);
					
					foreach (Task dalTask in bucket.Items)
						coll.Add(BLLUtility.CreateTaskBll(dalTask));
					
					
					groupToMembersMap[key] = coll;
				}
				
				//Now process Groups only, ie. the remaining non-leaf nodes
				while (unconvertedGroupsQ.Count > 0)
				{
					Group dalGroup = unconvertedGroupsQ.Dequeue();
					
					GroupBLL bllGroup = BLLUtility.CreateGroupBll(dalGroup);
					bllGroup.Items = groupToMembersMap[dalGroup.ItemID];
					
					//check if parent group has mapping in dict. If not, create.
					//Either case, add to dict[parent].
					if (dalGroup.ParentGroup != null)
					{
						List<ChecklistElementBLL> memberItems;
						if (groupToMembersMap.TryGetValue((int)dalGroup.ParentGroupID, out memberItems))
							memberItems.Add(bllGroup);
						else
						{
							memberItems = new List<ChecklistElementBLL>();
							memberItems.Add(bllGroup);
							groupToMembersMap[(int)dalGroup.ParentGroupID] = memberItems;
							
							unconvertedGroupsQ.Enqueue(dalGroup.ParentGroup);
						}
					}
					else
					{
						if (!groupToMembersMap.ContainsKey(-1))
							groupToMembersMap[-1] = new List<ChecklistElementBLL>();
							
						groupToMembersMap[-1].Add(bllGroup);
					}
				}
				
				return groupToMembersMap[-1];
				
			}
			
		}
		
		
	}
}
