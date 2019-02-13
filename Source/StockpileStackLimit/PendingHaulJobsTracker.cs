using RimWorld;
using System.Collections.Generic;
using Verse;
using ListTuple = System.Collections.Generic.List<StockpileStackLimit.Tuple<int, Verse.Pawn>>;
using System.Linq;

namespace StockpileStackLimit
{
	class PendingHaulJobsTracker
	{
		//int is the stack size of the job
		private static Dictionary<SlotGroup, ListTuple> pendingHaulJobs = new Dictionary<SlotGroup, ListTuple>();
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
						x += s.Value.Sum(y => y.Var1);
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
				Tuple<int, Pawn>? matchingTuple = null;

				foreach (var x in pendingHaulJobs)
				{
					foreach (var y in x.Value)
					{
						if (y.Var2 == pawn)
						{
							stack = y.Var1;
							slotGroup = x.Key;
							matchingTuple = y;
							break;
						}
					}
				}

				if (slotGroup != null)
				{
					pendingHaulJobs[slotGroup].Remove(matchingTuple.Value);

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
			if (clearPreviousJob)
				ClearJobForPawn(pawn);

			if (slotGroup == null) return;

			//pendingHaulJobs.Remove(slotGroup);
			ListTuple list;
			if (pendingHaulJobs.ContainsKey(slotGroup))
				list = pendingHaulJobs[slotGroup];
			else
			{
				list = new ListTuple();
				pendingHaulJobs[slotGroup] = list;
			}

			list.Add(Tuple.Create(stackSize, pawn));

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
			else
				Log.Warning($"Already found job for {pawn}, adding new one anyway.");

			HeldItemsCounter.UpdateSlotGroup(slotGroup.parent.Map, slotGroup);
		}

		public static void RecalculatePrecalc()
		{
			//CleanupUnusedPendingJobs() ???
			precalculatedSlotGroupTotalPendingStack.Clear();
			foreach (var x in pendingHaulJobs)
			{
				precalculatedSlotGroupTotalPendingStack[x.Key] = x.Value.Sum(y => y.Var1);
			}
		}

		public static void CleanupUnusedPendingJobs()
		{
			foreach(Pawn pawn in pawnsWithHaulingJobs)
			{
				if (pawn.CurJob == null || pawn.CurJob.def != JobDefOf.HaulToCell)
				{
					ClearJobForPawn(pawn);
				}
			}
		}
	}
}
