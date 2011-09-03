
using System;

namespace ProcrastinHater.BusinessInterfaces.CrudHelpers
{
	/// <summary>
	/// Helper that aids in specifying properties of a Group object.
	/// </summary>
	public class GroupInfo : ChecklistElementInfo
	{
		public GroupInfo()
			:this(null, "#FF232323", "#FFFFFFFF", "Arial", 14, DateTime.Now,
			      true)
		{
			
		}
		
		public GroupInfo(string title, string bgCol, string fontCol,
		               	string fontName, double fontSize, DateTime beginTime,
		               	bool isExpanded)
			:base(title, bgCol, fontCol, fontName, fontSize, beginTime)
		{
			IsExpanded = isExpanded;
		}
		
		public bool IsExpanded {get; set;}
	}
}
