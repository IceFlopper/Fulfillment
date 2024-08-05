using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Reflection;
using Verse;

namespace FulfillmentModNamespace
{
    [StaticConstructorOnStartup]
    public static class FulfillmentMod
    {
        static FulfillmentMod()
        {
            var harmony = new Harmony("com.Lewkah0.fulfillmentmod");
            harmony.PatchAll();
        }
    }

    [HarmonyPatch(typeof(Pawn_NeedsTracker), "AllNeeds", MethodType.Getter)]
    public static class Pawn_NeedsTracker_AllNeeds_Patch
    {
        static void Postfix(Pawn_NeedsTracker __instance, ref List<Need> __result)
        {
            Pawn pawn = (Pawn)typeof(Pawn_NeedsTracker)
                .GetField("pawn", BindingFlags.Instance | BindingFlags.NonPublic)
                .GetValue(__instance);

            if (pawn != null && pawn.RaceProps.Humanlike && !__result.Exists(n => n is Need_Fulfillment))
            {
                __result.Add(new Need_Fulfillment(pawn));
            }
            else if (pawn == null)
            {
                Log.Error("Pawn_NeedsTracker_AllNeeds_Patch: Pawn is null");
            }
        }
    }

    [HarmonyPatch(typeof(Pawn_NeedsTracker), "ShouldHaveNeed")]
    public static class Pawn_NeedsTracker_ShouldHaveNeed_Patch
    {
        static void Postfix(Pawn_NeedsTracker __instance, NeedDef nd, ref bool __result)
        {
            Pawn pawn = (Pawn)typeof(Pawn_NeedsTracker)
                .GetField("pawn", BindingFlags.Instance | BindingFlags.NonPublic)
                .GetValue(__instance);

            if (pawn != null && nd.defName == "Fulfillment")
            {
                __result = pawn.RaceProps.Humanlike;
            }
            else if (pawn == null)
            {
                Log.Error("Pawn_NeedsTracker_ShouldHaveNeed_Patch: Pawn is null");
            }
        }
    }
}
