using RimWorld;
using System.Linq;
using Verse;

namespace StockpileStackLimit
{
	class FindBestBetterStoreCellPatcher : Patch
	{
		protected override Desc GetDesc() => new Desc(typeof(StoreUtility), "TryFindBestBetterStorageFor");

		public static void Prefix(Thing t, Pawn carrier, Map map, ref StoragePriority currentPriority, Faction faction)
		{
			var slotGroup0 = t.GetSlotGroup();

			if (slotGroup0 != null && Limits.HasLimit(slotGroup0.Settings))
			{
				int stockpileMax = Limits.GetLimit(slotGroup0.Settings);
								
				if (slotGroup0.TotalPrecalculatedItemsStack(false) > stockpileMax)
				{
					currentPriority = StoragePriority.Unstored;
				}
			}
		}
	}
}
