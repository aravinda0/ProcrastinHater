
using System;
using ProcrastinHater.BusinessInterfaces.BLLClasses;
using ProcrastinHater.BusinessInterfaces.CrudHelpers;
using ProcrastinHater.ViewModels.ChecklistElements;

namespace ProcrastinHater.ViewModels.Utility
{
	/// <summary>
	/// Helps convert between entities of different layers.
	/// </summary>
	public static class EntityConverter
	{
		/// <summary>
		/// Maps properties from TaskBLL to TaskVM. 
		/// Does not set ParentGroup property.
		/// </summary>
		public static TaskVM TaskBllToTaskVm(TaskBLL bllTask)
		{
			TaskInfo ti = new TaskInfo(bllTask.Title, bllTask.BackgroundColor, bllTask.FontColor,
			                           bllTask.FontName, bllTask.FontSize, bllTask.BeginTime, bllTask.Details,
			                           bllTask.Status);
			
			return new TaskVM(bllTask.ItemID, ti, bllTask.ResolveTime);
		}
		
		public static GroupVM GroupBllToGroupVm(GroupBLL bllGroup)
		{
			GroupInfo gi = new GroupInfo(bllGroup.Title, bllGroup.BackgroundColor, bllGroup.FontColor,
			                             bllGroup.FontName, bllGroup.FontSize, bllGroup.BeginTime, bllGroup.IsExpanded);
			
			return new GroupVM(bllGroup.ItemID, gi, bllGroup.ResolveTime);
		}
	}
}
