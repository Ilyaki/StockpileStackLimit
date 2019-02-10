using RimWorld;
using System;
using System.Reflection;
using Verse;
using Verse.AI;

namespace StockpileStackLimit
{
	class HaulPatcher : Patch
	{
		protected override Desc GetDesc() => new Desc(typeof(HaulAIUtility), "HaulToStorageJob");//HaulToCellStorageJob

		public static void Postfix(Pawn p, Thing t, ref Job __result)
		{
			if (__result == null || p == null || t == null) return;
			
			IntVec3 storeCell = __result.targetB.Cell;
			
			SetCountLimit(p, t, ref __result, storeCell);
		}
		
		private static void SetCountLimit(Pawn p, Thing t, ref Job __result, IntVec3 storeCell)
		{
			SlotGroup toSlotGroup = p.Map.haulDestinationManager.SlotGroupAt(storeCell);
			
			if (toSlotGroup == null)
			{
				//It is a haul destination without a SlotGroup, e.g. a grave
				return;
			}

			int toLimit = Limits.GetLimit(toSlotGroup.Settings);

			int currentStack = __result.count < 0 ? t.stackCount : __result.count;
			
			bool hasSetFirstLimit = false;

			SlotGroup fromSlotGroup = StoreUtility.GetSlotGroup(t.Position, t.Map);
			if (fromSlotGroup != null)
			{
				int fromLimit = Limits.GetLimit(fromSlotGroup.Settings);
				
				if (fromLimit > 0 && StoreUtility.CurrentStoragePriorityOf(t) > StoreUtility.StoragePriorityAtFor(storeCell, t))
				{
					//Hauling from limited high priority to low priority. Only haul the minimum necessary to go to exact limit.
					int stockpileStack1 = fromSlotGroup.TotalItemsStack();
					__result.count = stockpileStack1 - fromLimit;
					hasSetFirstLimit = true;
				}
			}
			
			if (toLimit < 0)
			{
				return;
			}
			
			int stockpileStack = toSlotGroup.TotalItemsStack();
			
			if (stockpileStack >= toLimit)
			{
				//Say no spot availible
				JobFailReason.Is(GetNoEmptyPlaceLowerTransString(), null);
				__result = null;
			}
			else if (stockpileStack + currentStack > toLimit)//It will go over the limit
			{
				int newLimit = toLimit - stockpileStack;
				
				__result.count = hasSetFirstLimit ? Math.Min(newLimit, __result.count < 0 ? int.MaxValue : __result.count) : newLimit;
			}

			if (__result.count == 0)
			{
				JobFailReason.Is(GetNoEmptyPlaceLowerTransString(), null);
				__result = null;
			}
			
		}

		private static string GetNoEmptyPlaceLowerTransString()
		{
			string noEmptyPlaceLowerTrans = "";

			try
			{
				Type type = typeof(HaulAIUtility);
				FieldInfo fi = type.GetField("NoEmptyPlaceLowerTrans", BindingFlags.NonPublic | BindingFlags.Static);
				noEmptyPlaceLowerTrans = (string)fi.GetValue(null);

			}
			catch (Exception)
			{
				noEmptyPlaceLowerTrans = "Cannot haul item (no empty, accessible spot configured to store it)";
			}

			return noEmptyPlaceLowerTrans;
		}
	}
}
