
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
			
			PurgeOldPositionInfo();
			
			
			List<ChecklistElementBLL> topLvlNodes = new List<ChecklistElementBLL>();
			
			using (ProcrastinHaterEntities context = new ProcrastinHaterEntities())
			{
				
//                int daysOfPositionHistory = context.HardSettingsSet.Where(o => o.Check == true).Single().DaysOfHistoryToShow;
//
//				DateTime dateLowerLimit = date.AddDays(-daysOfPositionHistory);
//				dateLowerLimit = new DateTime(dateLowerLimit.Year, dateLowerLimit.Month, dateLowerLimit.Day);
//                
//				Func<ChecklistElement, bool> fc = (ce) => ((ce is Task) && ((ce.ResolveTime == null) || ((dateLowerLimit >= ce.BeginTime && dateLowerLimit <= (DateTime)ce.ResolveTime))));
//
//				var st = (from ce in context.ChecklistElements
//				          where (((ce is Task) && fc(ce)) ||
//				                 ((ce is Group) && (ce as Group).ChecklistElements.Any(ce2 => fc(ce2))))
//				          select ce).ToList();
//          		
//
//				
//				return null;
//				
//
//				//first get tasks(leaf nodes) only.
//				var allTasksForDateGroupedByParents = (from t in context.ChecklistElements.OfType<Task>()
//				                                       where ((t.ResolveTime == null) || 
//				                                              ((dateLowerLimit >= t.BeginTime && dateLowerLimit <= (DateTime)t.ResolveTime)))
//				                                       group t by t.ParentGroup into bucket
//				                                       select new {Group = bucket.Key, Items = bucket}).ToList();				
//				

//				var v = context.ChecklistElements.First();
//				int x = v.ItemID;
////				int x = 3;
//				
////				int i = 0;
////				foreach (var x in v)
////					i = 2 + 4;
//				
//				return (x > 0)?null:new List<ChecklistElementBLL>();
				
				#region old try

				#region commenting helper
                int daysOfPositionHistory = context.HardSettingsSet.Where(o => o.Check == true).Single().DaysOfHistoryToShow;
				
				
				DateTime dateLowerLimit = date.AddDays(-daysOfPositionHistory);
				dateLowerLimit = new DateTime(dateLowerLimit.Year, dateLowerLimit.Month, dateLowerLimit.Day);
				

				//first get tasks(leaf nodes) only.
				var allTasksForDateGroupedByParents = (from t in context.ChecklistElements.OfType<Task>()
				                                       where ((t.ResolveTime == null) || 
				                                              ((dateLowerLimit >= t.BeginTime && dateLowerLimit <= (DateTime)t.ResolveTime)))
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
				
				//FIXME: MASSIVE Bottleneck here. Soln: cache all posinfo in memory.
//				SortTreeByPositionInfo(context, groupToMembersMap);
				
				return groupToMembersMap[-1];
				#endregion

				#endregion
				
			}
			
		}
		
		private void PurgeOldPositionInfo()
		{
			
			using (ProcrastinHaterEntities context = new ProcrastinHaterEntities())
			{
				
				HardSettings settings = context.HardSettingsSet.Single(hs => hs.Check == true);
	            DateTime lastPurgeTime = settings.LastPosInfoPurgeTime;
	
	            //purge items when a day or more has elapsed since the last purge.
	            if ((DateTime.Now - lastPurgeTime).Days > 0)
	            {
                	int daysOfPositionHistory = settings.DaysOfHistoryToShow;
	            	DateTime dateLowerLimit = DateTime.Now.AddDays(-daysOfPositionHistory);
	            	
	                var itemsToStripPosInfoFrom = (from ce in context.ChecklistElements
	                                               where ((ce.PositionInformation != null) && ((DateTime)ce.ResolveTime < dateLowerLimit))
	                                               select ce);
	
	                foreach (var item in itemsToStripPosInfoFrom)
	                	BLLUtility.DeletePositionInfo(context, item);
	                
	                settings.LastPosInfoPurgeTime = DateTime.Now;
	                
	                //Commit purge.
	                context.SaveChanges();

            	}
	            
			}
		}
		
		private void SortTreeByPositionInfo(ProcrastinHaterEntities context, Dictionary<int, List<ChecklistElementBLL>> tree)
		{
			ChecklistElementPositionPrioritizedComparer comparer = new ChecklistElementOrganizer.ChecklistElementPositionPrioritizedComparer(context);
			
			foreach(List<ChecklistElementBLL> groupMemberSet in tree.Values)
			{
				groupMemberSet.Sort(comparer);
			}
		}
		
		
		/// <summary>
		/// Sorts CEBLL items based on PositionInfo stored in database, if available.
		/// Items with PosInfo occur before items without PosInfo.
		/// Items without PosInfo are sorted as per BeginTime, latest items appearing first.
		/// </summary>
		private class ChecklistElementPositionPrioritizedComparer : IComparer<ChecklistElementBLL>
		{
			
			public ChecklistElementPositionPrioritizedComparer(ProcrastinHaterEntities context)
			{
				_context = context;
			}
			
			public int Compare(ChecklistElementBLL x, ChecklistElementBLL y)
			{
				if (x == y)
					return 0;

				ChecklistElement ceX = _context.ChecklistElements.Where((ce) => ce.ItemID == x.ItemID).Single();
				ChecklistElement ceY = _context.ChecklistElements.Where((ce) => ce.ItemID == y.ItemID).Single();

				if (ceX.PositionInformation == null && ceY.PositionInformation == null)
				{
					return DateTime.Compare(ceX.BeginTime, ceY.BeginTime);
				}
				else if (ceX.PositionInformation != null && ceY.PositionInformation == null)
					return -1;
				else if (ceX.PositionInformation == null && ceY.PositionInformation != null)
					return 1;
				else if (ceX.PositionInformation != null && ceY.PositionInformation != null)
				{
					//compare database stored position info
					
					ChecklistElement temp = ceX;//.PositionInformation.NextItem;
					while (temp != null)
					{
						if (temp == ceY)
							break;
						temp = temp.PositionInformation.NextItem;
					}
					
					if (temp == ceY)
						return -1;
					else
					{
						temp = ceX;
						while (temp != null)
						{
							if (temp == ceY)
								break;
							temp = temp.PositionInformation.PreviousItem;
						}
						
						if (temp == ceY)
							return 1;
						else //HACK: Implement a proper exception.
							throw new Exception(string.Format("The database PositionInfo table has badly formed data. Failure caused by items with respective ItemIDs {0} and {1}", ceX.ItemID.ToString(), ceY.ItemID.ToString()));
					}
					
				}
				
				
				throw new Exception("The universe just broke. The code never should reach here"); 
				
			}
			
			private ProcrastinHaterEntities _context;
			
		}
	}
}
