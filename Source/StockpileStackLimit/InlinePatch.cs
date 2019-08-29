using Harmony;
using RimWorld;
using System.Collections.Generic;
using System.Reflection.Emit;
using Verse;

namespace StockpileStackLimit
{
    class FindBestBetterStoreCellPatcher2 : Patch
    {
        protected override Desc GetDesc() => new Desc(typeof(StoreUtility), "TryFindBestBetterStoreCellFor");

        public static bool Equals(CodeInstruction one, CodeInstruction another)
        {
            return (one.opcode == another.opcode && one.operand == another.operand);
        }
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var iter = instructions.GetEnumerator();
            CodeInstruction current;
            CodeInstruction first = new CodeInstruction(OpCodes.Callvirt, AccessTools.Method("RimWorld.HaulDestinationManager:get_AllGroupsListInPriorityOrder"));
            CodeInstruction inserted = new CodeInstruction(OpCodes.Call, AccessTools.Method("GetSlotGroupsPatcher:Postfix"));
            while (iter.MoveNext())
            {
                current = iter.Current;
                // Log.Message($"{current}, operand type = {current.operand?.GetType()}");
                yield return current;
                if (Equals(current, first))
                {
                    yield return inserted;
#if DEBUG
                    Log.Message("successfully inserted !!!");
#endif
                }
            }
        }
    }
}
