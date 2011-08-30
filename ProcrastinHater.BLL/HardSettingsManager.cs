
using System;
using System.Linq;

using ProcrastinHater.POCOEntities;

namespace ProcrastinHater.BLL
{
	/// <summary>
	/// Manages database-stored settings.
	/// </summary>
	internal static class HardSettingsManager
	{
		public static int GetAndAdvanceNextChecklistElementKey(ProcrastinHaterEntities context)
		{
			
			HardSettings settings = GetHardSettings(context);
			
			
			if (settings == null)
				return -1;
			else
			{
				int ret = settings.NextChecklistElementsKey;
				
				++settings.NextChecklistElementsKey; //committed through context.SaveChanges by caller
				
				return ret;
			}
			
		}
		
		
		public static int GetAndAdvanceNextTimedTaskSettingsKey(ProcrastinHaterEntities context)
		{
			
			HardSettings settings = GetHardSettings(context);
			
			
			if (settings == null)
				return -1;
			else
			{
				int ret = settings.NextTimedTaskSettingsKey;
				
				++settings.NextTimedTaskSettingsKey; //committed through context.SaveChanges by caller
				
				return ret;
			}
			
		}
		
		private static HardSettings GetHardSettings(ProcrastinHaterEntities context)
		{
			HardSettings settings = (from s in context.HardSettingsSet
			                         where s.Check == true
			                         select s).SingleOrDefault();
			
			return settings;
		}
	}
}
