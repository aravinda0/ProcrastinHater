
using System;

namespace ProcrastinHater.BusinessInterfaces.CrudHelpers
{
	/// <summary>
	/// Aids in the specification of ChecklistElements for the BLL to process.
	/// </summary>
	public abstract class ChecklistElementInfo
	{
		public ChecklistElementInfo()
			:this(null, "#FF232323", "#FFFFFFFF", "Arial", 14, DateTime.Now)
		{
		}
		
		public ChecklistElementInfo(string title, string bgCol, string fontCol,
		                            string fontName, double fontSize, DateTime beginTime)
		{
			Title = title;
			BackgroundColor = bgCol;
			FontColor = fontCol;
			FontName = fontName;
			FontSize = fontSize;
			BeginTime = beginTime;
		}
		
		public string Title {get; set;}
		
		public string BackgroundColor { get; set; }
		
		public string FontColor { get; set; }
		
		public string FontName { get; set; }
		
		public double FontSize { get; set; }
		
		public DateTime BeginTime { get; set; }
	}
}
