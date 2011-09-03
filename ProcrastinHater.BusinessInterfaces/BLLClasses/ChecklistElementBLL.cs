
using System;
using System.Collections.Generic;
using ProcrastinHater.BusinessInterfaces.CrudHelpers;

namespace ProcrastinHater.BusinessInterfaces.BLLClasses
{
	/// <summary>
	/// Abstract BLL class for a ChecklistElement.
	/// </summary>
	public abstract class ChecklistElementBLL
	{

		
		public ChecklistElementBLL(int id, int? parentGroupId, ChecklistElementInfo ceInfo,
		                           DateTime? resolveTime)
		{
			ItemID = id;
			
			Title = ceInfo.Title;
			BackgroundColor = ceInfo.BackgroundColor;
			FontColor = ceInfo.FontColor;
			FontName = ceInfo.FontName;
			FontSize = ceInfo.FontSize;
			BeginTime = ceInfo.BeginTime;
			ResolveTime = resolveTime;
		}
		
		public int ItemID
		{
			get;
			private set;
		}
		
		public string Title
		{
			get;
			private set;
		}
		
		public string FontName
		{
			get;
			private set;
		}
		
		public string FontColor
		{
			get;
			private set;
		}
		
		public double FontSize
		{
			get;
			private set;
		}
		
		public string BackgroundColor
		{
			get;
			private set;
		}
		
		public DateTime BeginTime
		{
			get;
			private set;
		}
		
		public DateTime? ResolveTime
		{
			get;
			private set;
		}
		
		public int? ParentGroupID
		{
			get;
			private set;
		}
		
	}
}
