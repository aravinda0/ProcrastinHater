
using System;
using System.Collections.Generic;
using ProcrastinHater.POCOEntities;

namespace ProcrastinHater.BLL
{
	/// <summary>
	/// Description of Bridge.
	/// </summary>
	public class Bridge
	{
		public Bridge()
		{
			_context = new ProcrastinHaterEntities();
			CEMgr = new ChecklistElementOrganizer(_context);
			GroupsMgr = new GroupsManager(_context);
			TasksMgr = new TasksManager(_context);
		}
		
		public ChecklistElementOrganizer CEMgr
		{
			get;
			private set;
		}
		
		public GroupsManager GroupsMgr
		{
			get;
			private set;
		}
		
		public TasksManager TasksMgr
		{
			get;
			private set;
		}
		
		private ProcrastinHaterEntities _context;
	}
}
