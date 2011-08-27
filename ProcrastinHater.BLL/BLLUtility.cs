
using System;
using System.Linq;
using System.Text.RegularExpressions;
using ProcrastinHater.POCOEntities;

namespace ProcrastinHater.BLL
{
	/// <summary>
	/// General purpose stuff for use by the BLL assembly.
	/// </summary>
	internal static class BLLUtility
	{
		public static HardSettings GetHardSettings(ProcrastinHaterEntities context,
		                                           out string err)
		{
			err = "";
			
			HardSettings settings = (from s in context.HardSettingsSet
			                         where s.Check == true
			                         select s).SingleOrDefault();
			
			if (settings == null)
				err = "The settings row does not exist in the database!";
			
			return settings;
		}
		
		#region ChecklistElement validation
		
		public static bool ValidateChecklistElement(ChecklistElement item, 
		                                       ProcrastinHaterEntities context,
		                                       out string errors)
		{
			errors = "";
			
			string titleErr = ValidateTitleField(item.Title);
			if (titleErr != null)
				errors += titleErr + "\n";
			
			string fontColErr = ValidateColorString(item.FontColor);
			if (fontColErr != null)
				errors += fontColErr + "\n";
			
			string bgColErr = ValidateColorString(item.BackgroundColor);
			if (bgColErr != null)
				errors += bgColErr + "\n";
			
			string createTimeErr = ValidateCreateTime(item.CreateTime);
			if (createTimeErr != null)
				errors += createTimeErr + "\n";
			
			string resolveTimeErr = ValidateResolveTime(item.CreateTime, item.ResolveTime);
			if (resolveTimeErr != null)
				errors += resolveTimeErr + "\n";
			
			string fontSizeErr = ValidateFontSize(item.FontSize);
			if (fontSizeErr != null)
				errors += fontSizeErr + "\n";
			
			
			if (string.IsNullOrEmpty(errors))
				return true;
			else
				return false;
		}
		
		private static string ValidateTitleField(string title)
		{
			string err = null;
			
			if (string.IsNullOrWhiteSpace(title))
				err = "The item's title cannot be empty.";
			else if (title.Length > 800)
				err = "The item's title is too long. Limit it to 800 characters.";
						
			return err;
		}
		
		private static string ValidateColorString(string color)
		{
			string err = null;
			
			if (color.Length != 9 || Regex.Match(color, @"#[\da-f]{8}$", 
			                                     RegexOptions.IgnoreCase).Success == false)
				err = "The color must be a 9-character string of the format #XXXXXXXX.";

			return err;
		}
		
		private static string ValidateCreateTime(DateTime createTime)
		{
			string err = null;
			
			TimeSpan diff = DateTime.Now - createTime;
			
			//Allow future creation dates to incorporate advance planning of tasks.
			//ALT: Allow past dates?
			if (diff.Days > 0)
				err = "The creation date can be in the future, but not in the past.";
			
			return err;
		}		
		
		private static string ValidateResolveTime(DateTime createTime, DateTime? resolveTime)
		{
			string err = null;
			
			if (resolveTime != null && ((DateTime)resolveTime) < createTime)
				err = "The resolve timestamp cannot be earlier than the creation timestamp.";
			
			return err;
		}
			
		private static string ValidateFontSize(double size)
		{
			string err = null;
			
			if (size <= 0)
				err = "The font size must be a positive value.";
			
			return err;
		}
		
		//TODO: Validate FontName. 

		
		#endregion ChecklistElement validation
	}
}
