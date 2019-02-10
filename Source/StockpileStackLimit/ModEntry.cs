using System;
using Verse;

namespace StockpileStackLimit
{
	[StaticConstructorOnStartup]
	public class ModEntry
	{
		public static Random random = new Random();
		
		static ModEntry()
		{
			Patch.PatchAll("Ilyaki.StockpileStackLimit");
			Log.Message("StockpileStackLimit loaded");
		}
	}
}

