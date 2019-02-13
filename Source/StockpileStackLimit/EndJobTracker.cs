using RimWorld;
using System;
using Verse;
using Verse.AI;

namespace StockpileStackLimit
{
	class EndJobTracker : Patch
	{
		protected override Desc GetDesc() => new Desc(typeof(Pawn_JobTracker), "StartJob");

		public static void Postfix(Job newJob, Pawn ___pawn, Pawn_JobTracker __instance)
		{
			if (newJob.def == JobDefOf.HaulToCell)//TODO: HaulToContainer aswell?
			{
				if (newJob.targetA.HasThing)
				{
					Thing thing = newJob.targetA.Thing;
					SlotGroup slotGroup = ___pawn.Map.haulDestinationManager.SlotGroupAt(newJob.targetB.Cell);

					int stackCount = newJob.targetA.Thing.stackCount;
					if (stackCount < 1) stackCount = int.MaxValue;
					int amountCarrying = Math.Min(newJob.count, stackCount);
					amountCarrying = Math.Min(amountCarrying, ___pawn.carryTracker.AvailableStackSpace(newJob.targetA.Thing.def));

					//int amountCarrying = newJob.count < 0 ? newJob.targetA.Thing.stackCount : newJob.count;
#if DEBUG
					Log.Message($"{___pawn} is hauling {newJob.targetA}, count = {amountCarrying}");
#endif

					PendingHaulJobsTracker.AddNewJob(___pawn, amountCarrying, slotGroup);
				}
			}
			// Seems to never be called TODO: REMOVE
			  else if (newJob.def == JobDefOf.HaulToContainer)
			{
				Log.Message($"{___pawn} is hauling (to container) {newJob.targetA}");

				if (newJob.targetA.HasThing)
				{
					Thing thing = newJob.targetA.Thing;
					SlotGroup slotGroup = newJob.targetB.Thing.GetSlotGroup();

					int stackCount = newJob.targetA.Thing.stackCount;
					if (stackCount < 1) stackCount = int.MaxValue;
					int amountCarrying = Math.Min(newJob.count, stackCount);
					amountCarrying = Math.Min(amountCarrying, ___pawn.carryTracker.AvailableStackSpace(newJob.targetA.Thing.def));

					//int amountCarrying = newJob.count < 0 ? newJob.targetA.Thing.stackCount : newJob.count;
#if DEBUG
					Log.Message($"{___pawn} is hauling (to container) {newJob.targetA}, count = {amountCarrying}");
#endif

					PendingHaulJobsTracker.AddNewJob(___pawn, amountCarrying, slotGroup);
				}
			}
			else
			{
				PendingHaulJobsTracker.ClearJobForPawn(___pawn);
			}
		}
	}

	class EndJobTracker2 : Patch
	{
		protected override Desc GetDesc() => new Desc(typeof(Pawn_JobTracker), "EndCurrentJob");

		public static void Postfix(bool startNewJob, Pawn ___pawn)
		{
			if (!startNewJob)
			{
				PendingHaulJobsTracker.ClearJobForPawn(___pawn);
			}
		}
	}
}
