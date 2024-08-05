using Verse;

namespace FulfillmentModNamespace
{
    public class FulfillmentMoodSettings : ModSettings
    {
        public float moodAmountVeryLow = -10f;
        public float moodAmountLow = -5f;
        public float moodAmountMediumLow = -2f;
        public float moodAmountMedium = 2f;
        public float moodAmountMediumHigh = 5f;
        public float moodAmountHigh = 8f;
        public float moodAmountVeryHigh = 10f;

        public void ResetToDefaults()
        {
            moodAmountVeryLow = -10f;
            moodAmountLow = -5f;
            moodAmountMediumLow = -2f;
            moodAmountMedium = 2f;
            moodAmountMediumHigh = 5f;
            moodAmountHigh = 8f;
            moodAmountVeryHigh = 10f;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref moodAmountVeryLow, "moodAmountVeryLow", -10f);
            Scribe_Values.Look(ref moodAmountLow, "moodAmountLow", -5f);
            Scribe_Values.Look(ref moodAmountMediumLow, "moodAmountMediumLow", -2f);
            Scribe_Values.Look(ref moodAmountMedium, "moodAmountMedium", 2f);
            Scribe_Values.Look(ref moodAmountMediumHigh, "moodAmountMediumHigh", 5f);
            Scribe_Values.Look(ref moodAmountHigh, "moodAmountHigh", 8f);
            Scribe_Values.Look(ref moodAmountVeryHigh, "moodAmountVeryHigh", 10f);
        }
    }

}
