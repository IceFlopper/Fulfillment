using RimWorld;
using Verse;

namespace FulfillmentModNamespace
{
    [DefOf]
    public static class CustomThoughtDefOf
    {
        static CustomThoughtDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(CustomThoughtDefOf));
        }

        public static ThoughtDef FulfillmentVeryLow;
        public static ThoughtDef FulfillmentLow;
        public static ThoughtDef FulfillmentMediumLow;
        public static ThoughtDef FulfillmentMedium;
        public static ThoughtDef FulfillmentMediumHigh;
        public static ThoughtDef FulfillmentHigh;
    }
}
