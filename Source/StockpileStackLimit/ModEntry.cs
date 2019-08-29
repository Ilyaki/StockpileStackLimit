using Harmony;
using Verse;

namespace StockpileStackLimit
{
    [StaticConstructorOnStartup]
    public static class ModEntry
    {

        static ModEntry()
        {
            var harmony = HarmonyInstance.Create("Ilyaki.StockpileStackLimit");
#if DEBUG
            HarmonyInstance.DEBUG = true;
#endif
            Patch.PatchAll(harmony);
            Log.Message("StockpileStackLimit loaded");
        }
    }
}

