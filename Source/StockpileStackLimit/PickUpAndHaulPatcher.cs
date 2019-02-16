using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PickUpAndHaul;
using RimWorld;
using Verse;
using Verse.AI;

namespace StockpileStackLimit
{
	//Adds combatibility for the mod PickUpAndHaul
	class PickUpAndHaulPatcher : Patch
	{
		protected override Desc GetDesc() {
			int myIndex = -1;
			int pickupIndex = -1;

			int i = 0;
			foreach (var name in LoadedModManager.RunningModsListForReading.Select(x => x.Name))
			{
				if (name == "Stockpile Stack Limit") myIndex = i;
				else if (name == "Pick Up And Haul") pickupIndex = i;

				if (myIndex != -1 && pickupIndex != -1)
					break;

				i++;
			}

			if (pickupIndex == -1)//Mod is not in the load order, so don't do anything
			{
				return null;
			}
			else if (pickupIndex > myIndex)//It will be loaded after this mod, so warn and don't try to patch.
			{
				Log.Warning("You have Pick Up And Haul installed, but it is loaded after Stockpile Stack Limit. Make sure it is loaded before");
				return null;
			}
			else
			{
				//All good
				return new Desc(Get_WorkGiver_HaulToInventory_Type(), "JobOnThing");
			}
		}

		//Crashes if you do this in GetDesc
		private static Type Get_WorkGiver_HaulToInventory_Type()
		{
			return typeof(PickUpAndHaul.WorkGiver_HaulToInventory);
		}
		

		public static bool Prefix(Pawn pawn, Thing thing, ref Job __result)
		{
			StoragePriority currentPriority = StoreUtility.CurrentStoragePriorityOf(thing);
			if (StoreUtility.TryFindBestBetterStoreCellFor(thing, pawn, pawn.Map, currentPriority, pawn.Faction, out IntVec3 storeCell, true))
			{
				SlotGroup slotGroup = StoreUtility.GetSlotGroup(storeCell, thing.Map);
				if (Limits.HasLimit(slotGroup.Settings))
				{
					__result = HaulAIUtility.HaulToStorageJob(pawn, thing);
					return false;
				}
			}

			return true;
		}
	}

	class PickUpAndHaulPatcher2 : Patch
	{
		protected override Desc GetDesc()
		{
			int myIndex = -1;
			int pickupIndex = -1;

			int i = 0;
			foreach (var name in LoadedModManager.RunningModsListForReading.Select(x => x.Name))
			{
				if (name == "Stockpile Stack Limit") myIndex = i;
				else if (name == "Pick Up And Haul") pickupIndex = i;

				if (myIndex != -1 && pickupIndex != -1)
					break;

				i++;
			}

			if (pickupIndex == -1)//Mod is not in the load order, so don't do anything
			{
				return null;
			}
			else if (pickupIndex > myIndex)//It will be loaded after this mod, so warn and don't try to patch.
			{
				return null;
			}
			else
			{
				//All good
				return new Desc(Get_WorkGiver_HaulToInventory_Type(), "HasJobOnThing");
			}
		}

		//Crashes if you do this in GetDesc
		private static Type Get_WorkGiver_HaulToInventory_Type()
		{
			return typeof(PickUpAndHaul.WorkGiver_HaulToInventory);
		}


		public static bool Prefix(Pawn pawn, Thing thing, WorkGiver_HaulToInventory __instance, bool forced, ref bool __result)
		{
			#region PickUpAndHaul code
			//bulky gear (power armor + minigun) so don't bother.
			if (MassUtility.GearMass(pawn) / MassUtility.Capacity(pawn) >= 0.8f) return false;

			if (!WorkGiver_HaulToInventory.GoodThingToHaul(thing, pawn) || !HaulAIUtility.PawnCanAutomaticallyHaulFast(pawn, thing, forced))
				return false;

			StoragePriority currentPriority = StoreUtility.CurrentStoragePriorityOf(thing);
			bool foundCell = StoreUtility.TryFindBestBetterStoreCellFor(thing, pawn, pawn.Map, currentPriority, pawn.Faction, out IntVec3 storeCell, true);
			#endregion

			if (!foundCell)
			{
				__result = false;
			}
			else
			{
				SlotGroup slotGroup = pawn.Map.haulDestinationManager.SlotGroupAt(storeCell);
				__result =  !(slotGroup != null && Limits.HasLimit(slotGroup.Settings) && Limits.GetLimit(slotGroup.Settings) >= slotGroup.TotalPrecalculatedItemsStack());
			}

			return false;
		}
	}
}
