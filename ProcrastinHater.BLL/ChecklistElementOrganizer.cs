
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
		internal ChecklistElementOrganizer(ProcrastinHaterEntities context)
		{
			_context = context;
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
			
				
//                int daysOfPositionHistory = _context.HardSettingsSet.Where(o => o.Check == true).Single().DaysOfHistoryToShow;
//
//				DateTime dateLowerLimit = date.AddDays(-daysOfPositionHistory);
//				dateLowerLimit = new DateTime(dateLowerLimit.Year, dateLowerLimit.Month, dateLowerLimit.Day);
//                
//				Func<ChecklistElement, bool> fc = (ce) => ((ce is Task) && ((ce.ResolveTime == null) || ((dateLowerLimit >= ce.BeginTime && dateLowerLimit <= (DateTime)ce.ResolveTime))));
//
//				var st = (from ce in _context.ChecklistElements
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
//				var allTasksForDateGroupedByParents = (from t in _context.ChecklistElements.OfType<Task>()
//				                                       where ((t.ResolveTime == null) || 
//				                                              ((dateLowerLimit >= t.BeginTime && dateLowerLimit <= (DateTime)t.ResolveTime)))
//				                                       group t by t.ParentGroup into bucket
//				                                       select new {Group = bucket.Key, Items = bucket}).ToList();				
//				

//				var v = _context.ChecklistElements.First();
//				int x = v.ItemID;
////				int x = 3;
//				
////				int i = 0;
////				foreach (var x in v)
////					i = 2 + 4;
//				
//				return (x > 0)?null:new List<ChecklistElementBLL>();
			
			#region default approach

			#region commenting helper
            int daysOfPositionHistory = _context.HardSettingsSet.Where(o => o.Check == true).Single().DaysOfHistoryToShow;
			
			
			DateTime dateLowerLimit = date.AddDays(-daysOfPositionHistory);
			dateLowerLimit = new DateTime(dateLowerLimit.Year, dateLowerLimit.Month, dateLowerLimit.Day);

			//first get tasks(leaf nodes) only.
			var allTasksForDateGroupedByParents = (from t in _context.ChecklistElements.OfType<Task>()
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
			
			SortTreeByPositionInfo(groupToMembersMap);
			
			return groupToMembersMap[-1];
			#endregion

			#endregion
			
			
		}
		
		private void PurgeOldPositionInfo()
		{
			
			HardSettings settings = _context.HardSettingsSet.Single(hs => hs.Check == true);
            DateTime lastPurgeTime = settings.LastPosInfoPurgeTime;

            //purge items when a day or more has elapsed since the last purge.
            if ((DateTime.Now - lastPurgeTime).Days > 0)
            {
            	int daysOfPositionHistory = settings.DaysOfHistoryToShow;
            	DateTime dateLowerLimit = DateTime.Now.AddDays(-daysOfPositionHistory);
            	
                var itemsToStripPosInfoFrom = (from ce in _context.ChecklistElements
                                               where ((ce.PositionInformation != null) && ((DateTime)ce.ResolveTime < dateLowerLimit))
                                               select ce);

                foreach (var item in itemsToStripPosInfoFrom)
                	BLLUtility.DeletePositionInfo(_context, item);
                
                settings.LastPosInfoPurgeTime = DateTime.Now;
                
                //Commit purge.
                _context.SaveChanges();

			}
		}
		
		private void SortTreeByPositionInfo(Dictionary<int, List<ChecklistElementBLL>> tree)
		{
			ChecklistElementPositionPrioritizedComparer comparer = new ChecklistElementOrganizer.ChecklistElementPositionPrioritizedComparer(_context);
			
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
				
				_positionTrackedItemsCache = (from ce in _context.ChecklistElements
				          where ce.PositionInformation != null
				          select ce).ToList();
			}
			

			List<ChecklistElement> _positionTrackedItemsCache;
			
			public int Compare(ChecklistElementBLL x, ChecklistElementBLL y)
			{
//				Console.WriteLine("{0}\t{1}", x.ItemID, x.Title);
//				Console.WriteLine("{0}\t{1}", y.ItemID, y.Title);
//				Console.WriteLine("\n");
				if (x == y)
					return 0;

				ChecklistElement ceX = _positionTrackedItemsCache.SingleOrDefault((ce) => ce.ItemID == x.ItemID);
				ChecklistElement ceY = _positionTrackedItemsCache.SingleOrDefault((ce) => ce.ItemID == y.ItemID);

				
				if (ceX == null && ceY == null)
				{
					//They are both null => both are absent from _posTrackedItemsCache => their PositionInformation == null
					
					//FIXME: As is, in this situation, 
					//compare btw Group & Task --> Group always wins. Instead, compare
					//with the most recent Task in BLLGroup's current list of items
					return DateTime.Compare(x.BeginTime, y.BeginTime);
				}
				else if (ceX != null && ceY == null)
					return -1;
				else if (ceX == null && ceY != null)
					return 1;
				else if (ceX != null && ceY != null)
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
		
		
		private ProcrastinHaterEntities _context;
	}
}
