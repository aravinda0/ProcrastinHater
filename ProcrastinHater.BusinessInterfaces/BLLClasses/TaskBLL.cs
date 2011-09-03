
using System;
using ProcrastinHater.BusinessInterfaces.CrudHelpers;

namespace ProcrastinHater.BusinessInterfaces.BLLClasses
{
	/// <summary>
	/// BLL version of Task.
	/// </summary>
	public class TaskBLL : ChecklistElementBLL
	{

		
		public TaskBLL(int id, int? parentGroupId, TaskInfo tInfo, DateTime? resolveTime,
		              TimedTaskSettingsInfo timingInfo)
			:base(id, parentGroupId, tInfo, resolveTime)
		{
			Status = tInfo.Status;
			Details = tInfo.Details;
			
			TimingInfo = new TimedTaskSettingsInfo();
			TimingInfo.DueTime = timingInfo.DueTime;
			TimingInfo.TimeoutAction = timingInfo.TimeoutAction;
		}
		
		public string Details
		{
			get;
			private set;
		}
		
		public TaskStatuses Status
		{
			get;
			private set;
		}
		
		public TimedTaskSettingsInfo TimingInfo
		{
			get;
			private set;
		}
	}
}
