
using System;

namespace ProcrastinHater.BusinessInterfaces.CrudHelpers
{
	/// <summary>
	/// Description of TimedTaskSettingsInfo.
	/// </summary>
	public class TimedTaskSettingsInfo
	{
		public TimedTaskSettingsInfo()
		{
		}
		
		public TimedTaskSettingsInfo(DateTime dueTime, TaskTimeoutActions timeoutAction)
		{
			DueTime = dueTime;
			TimeoutAction = timeoutAction;
		}
		
		public DateTime DueTime { get; set; }
		
		public TaskTimeoutActions TimeoutAction {get; set; }
		
	}
}
