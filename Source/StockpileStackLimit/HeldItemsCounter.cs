using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using System.Reflection;

namespace StockpileStackLimit
{
	class HeldItemsCounter_MapTickListener : Patch
	{
		protected override Desc GetDesc() => new Desc(typeof(Map), "MapPostTick");

		public static void Postfix(Map __instance)
		{
			var hdm = __instance.haulDestinationManager;
			if (hdm != null)
			{
				var slotGroups = (List<SlotGroup>)typeof(HaulDestinationManager).GetField("allGroupsInOrder", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(hdm);

				if (slotGroups != null)
				{
					foreach (SlotGroup slotGroup in slotGroups.Where(x => x != null && Limits.HasLimit(x.Settings)))
					{
						HeldItemsCounter.UpdateSlotGroup(__instance, slotGroup);
					}

					HeldItemsCounter.RemoveUnusedSlotGroups(__instance, slotGroups);
				}
			}

			//HeldItemsCounter.TestOutput();
		}
	}

	class HeldItemsCounter
	{
		private static Dictionary<Map, Dictionary<SlotGroup, int>> heldItems = new Dictionary<Map, Dictionary<SlotGroup, int>>();

		public static void TestOutput()
		{
#if DEBUG
			if (heldItems.Count != 0)
			{
				var x = heldItems.ElementAt(0).Value;
				foreach (var y in x)
				{
					Log.Message($"{y.Key} : {y.Value} held, and an extra {PendingHaulJobsTracker.GetPendingStack(y.Key)} is pending");
				}
			}
#endif
		}

		public static void UpdateSlotGroup(Map map, SlotGroup slotGroup)
		{
			Dictionary<SlotGroup, int> dk = null;

			if (!heldItems.ContainsKey(map))
			{
				dk = new Dictionary<SlotGroup, int>();
				heldItems[map] = dk;
			}
			else
			{
				dk = heldItems[map];
			}

			dk[slotGroup] = slotGroup.TotalHeldItemsStack();
		}

		public static int GetTotalItemsStack(SlotGroup slotGroup)
		{
			var map = slotGroup.parent.Map;

			Dictionary<SlotGroup, int> dk = null;

			if (!heldItems.ContainsKey(map))
			{
				dk = new Dictionary<SlotGroup, int>();
				heldItems[map] = dk;
			}
			else
			{
				dk = heldItems[map];
			}

			if (!dk.ContainsKey(slotGroup))
			{
				int totalStack = slotGroup.TotalHeldItemsStack();
				dk[slotGroup] = totalStack;
				return totalStack;
			}
			else
			{
				return dk[slotGroup];
			}
		}

		public static void RemoveUnusedSlotGroups(Map map, List<SlotGroup> activeSlotGroups)
		{
			if (heldItems.ContainsKey(map))
			{
				var dk = heldItems[map];
				dk.RemoveAll(x => !activeSlotGroups.Contains(x.Key) || !Limits.HasLimit(x.Key.Settings));
			}
		}
	}
}
