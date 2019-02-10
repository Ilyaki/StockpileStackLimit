using RimWorld;
using Verse;

namespace StockpileStackLimit
{
	class StorageSettingsExposeDataPatcher : Patch
	{
		protected override Desc GetDesc() => new Desc(typeof(StorageSettings), "ExposeData");

		public static void Postfix(StorageSettings __instance)
		{			
			Scribe_Values.Look<int>(ref Limits.GetLimit(__instance), "stacklimit", -1, false);
		}
	}
}
