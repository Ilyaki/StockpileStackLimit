using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

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
				return new Desc(Get_WorkGiver_HaulToInventory_Type(), "CapacityAt");
			}
		}

		//Crashes if you do this in GetDesc
		private static Type Get_WorkGiver_HaulToInventory_Type()
		{
			return typeof(PickUpAndHaul.WorkGiver_HaulToInventory);
		}
		

		public static void Postfix(ThingDef def, IntVec3 storeCell, Map map, ref int __result)
		{
			SlotGroup slotGroup = StoreUtility.GetSlotGroup(storeCell, map);
			
			if (slotGroup != null)
			{
				int limit = Limits.GetLimit(slotGroup.Settings);

				if (limit < 0)
					return;
				else
				{
					//If capacity is zero, it uses the vanilla hauling Job
					__result = 0;
				}
			}
			
		}
	}
}
