
using System;

namespace ProcrastinHater.BusinessInterfaces.CrudHelpers
{
	/// <summary>
	/// The valid statuses that a Task may be in.
	/// </summary>
	public enum TaskStatuses
	{
		PENDING = 100,
		COMPLETED = 101,
		FAILED = 102,
		OVERDUE = 103,
		COMPLETED_LATE = 104
	}
	
	/// <summary>
	/// Timeout actions for Tasks with time limits.
	/// </summary>
	public enum TaskTimeoutActions
	{
		SHOW_OVERDUE = 100,
		SHOW_FAILED = 101
	}
}
