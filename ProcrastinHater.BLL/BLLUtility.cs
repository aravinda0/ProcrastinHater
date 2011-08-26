
using System;
using System.Linq;

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
	}
}
