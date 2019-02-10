using RimWorld;
using Verse;
using Verse.AI;

namespace StockpileStackLimit
{
	class EndJobTracker : Patch
	{
		protected override Desc GetDesc() => new Desc(typeof(Pawn_JobTracker), "StartJob");

		public static void Postfix(Job newJob, Pawn ___pawn)
		{
			if (newJob.def == JobDefOf.HaulToCell)//TODO: HaulToContainer aswell?
			{
				if (newJob.targetA.HasThing)
				{
					Thing thing = newJob.targetA.Thing;
					SlotGroup slotGroup = ___pawn.Map.haulDestinationManager.SlotGroupAt(newJob.targetB.Cell);
					PendingHaulJobsTracker.AddNewJob(___pawn, newJob.count, slotGroup);
				}
			}
			else
			{
				PendingHaulJobsTracker.ClearJobForPawn(___pawn);
			}
		}
	}
}
