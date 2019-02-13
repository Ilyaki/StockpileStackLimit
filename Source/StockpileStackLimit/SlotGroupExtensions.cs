using RimWorld;
using System.Linq;

namespace StockpileStackLimit
{
	static class SlotGroupExtensions
	{
		public static int TotalHeldItemsStack(this SlotGroup slotGroup)
		{
			if (slotGroup == null) return 0;
			
			return slotGroup.HeldThings.Where(x => StoreUtility.IsInValidStorage(x)).Sum(y => y.stackCount);
		}

		public static int TotalPrecalculatedItemsStack(this SlotGroup slotGroup, bool usePending = true)
		{
			if (slotGroup == null) return 0;

			int pending = usePending ? PendingHaulJobsTracker.GetPendingStack(slotGroup) : 0;
			int inSlotGroup = HeldItemsCounter.GetTotalItemsStack(slotGroup);

			return pending + inSlotGroup;
		}
	}
}
