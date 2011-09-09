
using System;
using System.Collections.Generic;

using ProcrastinHater.BusinessInterfaces.BLLClasses;

namespace ProcrastinHater.BusinessInterfaces
{
	/// <summary>
	/// Contract for management of position & ordering of ChecklistElementBLLs.
	/// </summary>
	public interface IChecklistElementOrganizer
	{
		List<ChecklistElementBLL> GetChecklistElementTreeForDate(DateTime date);
		
	}
}
