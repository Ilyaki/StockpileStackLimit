using RimWorld;
using System.Linq;
using Verse;

namespace StockpileStackLimit
{
	static class SlotGroupExtensions
	{
		public static int TotalItemsStack(this SlotGroup slotGroup, bool usePrecalculated = true)
		{
			if (slotGroup == null) return 0;

			if (usePrecalculated)
			{
				return HeldItemsCounter.GetTotalItemsStack(slotGroup);
			}
			else
			{
				int pendingSize = PendingHaulJobsTracker.GetPendingStack(slotGroup);

				return pendingSize + slotGroup.HeldThings.Where(x => StoreUtility.IsInValidStorage(x)).Sum(y => y.stackCount);
			}
		}
	}
}
