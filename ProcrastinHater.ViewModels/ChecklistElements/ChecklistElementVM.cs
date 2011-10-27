
using System;
using System.Collections.Generic;
using ElementalMvvm;
using ProcrastinHater.BusinessInterfaces.BLLClasses;
using ProcrastinHater.BusinessInterfaces.CrudHelpers;

namespace ProcrastinHater.ViewModels.ChecklistElements
{
	/// <summary>
	/// Screen-presentable ChecklistElement.
	/// </summary>
	public abstract class ChecklistElementVM : ViewModelBase
	{
		public ChecklistElementVM()
		{
			ItemID = -1;
			
			BackgroundColor = "#FFFFFFFF";
			FontColor = "#FF000000";
			FontSize = 14;
			FontName = "Arial";
			BeginTime = DateTime.Now;
			ResolveTime = null;
		}
		
		public ChecklistElementVM(int id, ChecklistElementInfo ceInfo, 
		                          DateTime? resolveTime, GroupVM parentGroup)
		{
			ItemID = id;

			Title = ceInfo.Title;
			BackgroundColor = ceInfo.BackgroundColor;
			FontColor = ceInfo.FontColor;
			FontSize = ceInfo.FontSize;
			FontName = ceInfo.FontName;
			BeginTime = ceInfo.BeginTime;
			
			ResolveTime = resolveTime;
			
			ParentGroup = parentGroup;
			
		}
		
		#region Mapped properties
		
		public int ItemID
		{
			get;
			private set;
		}
		
		public string Title
		{
			get {return _title;}
			set
			{
				if (_title == value)
					return;
				
				_title = value;
				this.OnPropertyChanged("Title");
			}
		}
		
		
		public string FontName
		{
			get {return _fontName;}
			set
			{
				if (_fontName == value)
					return;
				
				_fontName = value;
				this.OnPropertyChanged("FontName");
			}
		}
		
		
		public string FontColor
		{
			get {return _fontColor;}
			set
			{
				if (_fontColor == value)
					return;
				
				_fontColor = value;
				this.OnPropertyChanged("FontColor");
			}
		}
		
		
		public double FontSize
		{
			get {return _fontSize;}
			set
			{
				if (_fontSize == value)
					return;
				
				_fontSize = value;
				this.OnPropertyChanged("FontSize");
			}
		}
		
		
		public string BackgroundColor
		{
			get {return _backgroundColor;}
			set
			{
				if (_backgroundColor == value)
					return;
				
				_backgroundColor = value;
				this.OnPropertyChanged("FontSize");
			}
		}
		
		
		public DateTime BeginTime
		{
			get;
			private set;
		}
		
		
		
		public DateTime? ResolveTime
		{
			get {return _resolveTime;}
			set
			{
				if (_resolveTime == value)
					return;
				
				_resolveTime = value;
				this.OnPropertyChanged("ResolveTime");
			}
		}
		
		
		public GroupVM ParentGroup
		{
			get {return _parentGroup;}
			set
			{
				if (_parentGroup == value)
					return;
				
				_parentGroup = value;
				this.OnPropertyChanged("ParentGroup");
			}
		}		
		
		#endregion Mapped properties
		

		#region private fields
		
		string _title;
		string _fontName;
		string _fontColor;
		double _fontSize;
		string _backgroundColor;
		DateTime? _resolveTime;
		GroupVM  _parentGroup;
		
		#endregion private fields
		
	}
}
