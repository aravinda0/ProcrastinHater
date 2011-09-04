
using System;
using System.Collections.Generic;
using ProcrastinHater.BusinessInterfaces.CrudHelpers;

namespace ProcrastinHater.BusinessInterfaces.BLLClasses
{
	/// <summary>
	/// Description of GroupBLL.
	/// </summary>
	public class GroupBLL : ChecklistElementBLL
	{
		public GroupBLL(int id, int? parentGroupId, GroupInfo gInfo, DateTime? resolveTime)
			:base(id, parentGroupId, gInfo, resolveTime)
		{
			IsExpanded = gInfo.IsExpanded;
			Items = new List<ChecklistElementBLL>();
		}
		
		public bool IsExpanded
		{
			get;
			private set;
		}
		
		public List<ChecklistElementBLL> Items
		{
			get;
			set; //HACK: hmm... okay to be settable in an otherwise RO object?
		}
	}
}
