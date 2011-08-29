
using System;

namespace ProcrastinHater.BusinessInterfaces.CrudHelpers
{
	/// <summary>
	/// Aids in the specification of ChecklistElements for the BLL to process.
	/// </summary>
	public abstract class ChecklistElementInfo
	{
		public ChecklistElementInfo()
		{
		}
		
		public string Title {get; set;}
		
		public string BackgroundColor { get; set; }
		
		public string FontColor { get; set; }
		
		public string FontName { get; set; }
		
		public double FontSize { get; set; }
		
		public DateTime BeginTime { get; set; }
	}
}
