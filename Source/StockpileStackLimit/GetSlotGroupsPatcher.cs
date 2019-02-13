using RimWorld;
using System.Collections.Generic;
using System.Linq;

namespace StockpileStackLimit
{
	class GetSlotGroupsPatcher : Patch
	{
		protected override Desc GetDesc() => new Desc(typeof(HaulDestinationManager), "get_AllGroupsListInPriorityOrder");

		public static void Postfix(ref List<SlotGroup> __result)
		{
			__result = __result.Where(x =>
			{
				if (!Limits.HasLimit(x.Settings)) return true;

				int limit = Limits.GetLimit(x.Settings);
				if (limit < 0) limit = int.MaxValue;

				return x.TotalPrecalculatedItemsStack() < limit;
			})
			.OrderByDescending(x => x.Settings.Priority)
			.ThenByDescending(x =>
			{
				if (!Limits.HasLimit(x.Settings)) return 0;

				int limit = Limits.GetLimit(x.Settings);
				if (limit < 0) limit = int.MaxValue;

				return limit - x.TotalPrecalculatedItemsStack();
			})
			/*.ThenByDescending(x =>
			{
				int limit = Limits.GetLimit(x.Settings);
				if (limit < 0)
					return int.MaxValue;
				else
				{
					int heldItems = x.TotalItemsStack();
					int spaceLeft = limit - heldItems;
					return spaceLeft;
				}
			})*/
			.ToList();
		}
	}

	class GetSlotGroupsPatcher2 : Patch
	{
		protected override Desc GetDesc() => new Desc(typeof(HaulDestinationManager), "get_AllHaulDestinationsListInPriorityOrder");

		public static void Postfix(ref List<IHaulDestination> __result)
		{

			int HeldItems(IHaulDestination x) => StoreUtility.GetSlotGroup(x.Position, x.Map).TotalPrecalculatedItemsStack();

			__result = __result.Where(x =>
			{
				if (!Limits.HasLimit(x.GetStoreSettings())) return true;

				int limit = Limits.GetLimit(x.GetStoreSettings());
				if (limit < 0) limit = int.MaxValue;

				return HeldItems(x) < limit;
			})
			.OrderByDescending(x => x.GetStoreSettings().Priority)
			.ThenByDescending(x =>
			{
				if (!Limits.HasLimit(x.GetStoreSettings())) return 0;

				int limit = Limits.GetLimit(x.GetStoreSettings());
				if (limit < 0) limit = int.MaxValue;

				return limit - HeldItems(x);
			})
			/*.ThenByDescending(x =>
			{
				int limit = Limits.GetLimit(x.GetStoreSettings());
				if (limit < 0)
					return int.MaxValue;
				else
				{
					int heldItems = HeldItems(x);
					int spaceLeft = limit - heldItems;
					return spaceLeft;
				}
			})*/
			.ToList();
		}
	}
}
