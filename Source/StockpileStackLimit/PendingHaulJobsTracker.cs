using RimWorld;
using System.Collections.Generic;
using Verse;

namespace StockpileStackLimit
{
	class PendingHaulJobsTracker
	{
		//int is the stack size of the job
		private static Dictionary<SlotGroup, Tuple<int, Pawn>> pendingHaulJobs = new Dictionary<SlotGroup, Tuple<int, Pawn>>();
		private static readonly HashSet<Pawn> pawnsWithHaulingJobs = new HashSet<Pawn>();

		private static Dictionary<SlotGroup, int> precalculatedSlotGroupTotalPendingStack = new Dictionary<SlotGroup, int>();
		
		public static int GetPendingStack(SlotGroup slotGroup)
		{
			if (!precalculatedSlotGroupTotalPendingStack.ContainsKey(slotGroup))
			{
				int x = 0;
				foreach (var s in pendingHaulJobs)//TODO: is this foreach necessary?
				{
					if (s.Key == slotGroup)
					{
						x += s.Value.Var1;
					}
				}

				precalculatedSlotGroupTotalPendingStack.Add(slotGroup, x);
				return x;
			}
			else
			{
				return precalculatedSlotGroupTotalPendingStack[slotGroup];
			}
		}

		public static void ClearJobForPawn(Pawn pawn)
		{
			if (pawnsWithHaulingJobs.Contains(pawn))
			{
				int stack = 0;
				SlotGroup slotGroup = null;

				foreach (var x in pendingHaulJobs)
				{
					if (x.Value.Var2 == pawn)
					{
						stack = x.Value.Var1;
						slotGroup = x.Key;
						break;
					}
				}

				if (slotGroup != null)
				{
					pendingHaulJobs.Remove(slotGroup);

					if (precalculatedSlotGroupTotalPendingStack.ContainsKey(slotGroup))
					{
						precalculatedSlotGroupTotalPendingStack[slotGroup] -= stack;
					}
					else
					{
						precalculatedSlotGroupTotalPendingStack[slotGroup] = 0;
					}
				}
			}

			pawnsWithHaulingJobs.Remove(pawn);
		}

		public static void AddNewJob(Pawn pawn, int stackSize, SlotGroup slotGroup, bool clearPreviousJob = true)
		{
			if (slotGroup == null) return;

			if (clearPreviousJob)
				ClearJobForPawn(pawn);

			pendingHaulJobs.Remove(slotGroup);
			pendingHaulJobs.Add(slotGroup, Tuple.Create(stackSize, pawn));

			if (precalculatedSlotGroupTotalPendingStack.ContainsKey(slotGroup))
			{
				precalculatedSlotGroupTotalPendingStack[slotGroup] += stackSize;
			}
			else
			{
				precalculatedSlotGroupTotalPendingStack[slotGroup] = stackSize;
			}

			if (!pawnsWithHaulingJobs.Contains(pawn))
				pawnsWithHaulingJobs.Add(pawn);

			HeldItemsCounter.UpdateSlotGroup(slotGroup.parent.Map, slotGroup);
		}
	}
}
