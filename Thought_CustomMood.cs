using RimWorld;
using Verse;
namespace FulfillmentModNamespace
{
    public class Thought_CustomMood : Thought_Memory
    {
        public override float MoodOffset()
        {
            switch (def.defName)
            {
                case "Fulfillment_FulfillmentVeryLow":
                    return FulfillmentMoodMod.settings.moodAmountVeryLow;
                case "Fulfillment_FulfillmentLow":
                    return FulfillmentMoodMod.settings.moodAmountLow;
                case "Fulfillment_FulfillmentMediumLow":
                    return FulfillmentMoodMod.settings.moodAmountMediumLow;
                case "Fulfillment_FulfillmentMedium":
                    return FulfillmentMoodMod.settings.moodAmountMedium;
                case "Fulfillment_FulfillmentMediumHigh":
                    return FulfillmentMoodMod.settings.moodAmountMediumHigh;
                case "Fulfillment_FulfillmentHigh":
                    return FulfillmentMoodMod.settings.moodAmountHigh;
                case "Fulfillment_FulfillmentVeryHigh":
                    return FulfillmentMoodMod.settings.moodAmountVeryHigh;
                default:
                    return base.MoodOffset();
            }
        }
    }



}
