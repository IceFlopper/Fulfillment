using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;
using System.Linq;

namespace FulfillmentModNamespace
{
    [HarmonyPatch(typeof(JobDriver), "DriverTick")]
    public static class JobDriver_DriverTick_Patch
    {
        static void Postfix(JobDriver __instance)
        {
            if (__instance.pawn == null) return;
            if (__instance.pawn.needs == null) return;
            Need_Fulfillment need = __instance.pawn.needs.TryGetNeed<Need_Fulfillment>();
            if (need == null) return;

            if (__instance.job != null && __instance.job.workGiverDef != null)
            {
                var relevantSkillDefs = __instance.job.workGiverDef.workType.relevantSkills;
                foreach (var skillDef in relevantSkillDefs)
                {
                    SkillRecord skill = __instance.pawn.skills.GetSkill(skillDef);
                    if (skill != null && skill.passion > Passion.None)
                    {
                        need.GainFulfillment();
                        break; // Gain fulfillment only once per tick
                    }
                }
            }
        }
    }
}
