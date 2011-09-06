
using System;
using System.Collections.Generic;
using ProcrastinHater.BusinessInterfaces.BLLClasses;
using ProcrastinHater.BusinessInterfaces.CrudHelpers;

namespace ProcrastinHater.BusinessInterfaces
{
	/// <summary>
	/// Contract for manager of groups.
	/// </summary>
	public interface IGroupsManager
	{
		bool AddNewGroup(GroupInfo gInfo, int? parentGroupId, out string errors);
	}
}
