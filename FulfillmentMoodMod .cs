using UnityEngine;
using Verse;

namespace FulfillmentModNamespace
{

    public class FulfillmentMoodMod : Mod
    {
        public static FulfillmentMoodSettings settings;

        public FulfillmentMoodMod(ModContentPack content) : base(content)
        {
            settings = GetSettings<FulfillmentMoodSettings>();
        }

        public override string SettingsCategory() => "Fulfillment Mood Mod";

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);

            listingStandard.Label($"Very unfulfilled!: {settings.moodAmountVeryLow}");
            settings.moodAmountVeryLow = Mathf.Round(listingStandard.Slider(settings.moodAmountVeryLow, -100f, 100f));
            if (listingStandard.ButtonText("Reset to Default"))
            {
                settings.moodAmountVeryLow = -10f;
            }

            listingStandard.Label($"Unfulfilled: {settings.moodAmountLow}");
            settings.moodAmountLow = Mathf.Round(listingStandard.Slider(settings.moodAmountLow, -100f, 100f));
            if (listingStandard.ButtonText("Reset to Default"))
            {
                settings.moodAmountLow = -5f;
            }

            listingStandard.Label($"Somewhat unfulfilled: {settings.moodAmountMediumLow}");
            settings.moodAmountMediumLow = Mathf.Round(listingStandard.Slider(settings.moodAmountMediumLow, -100f, 100f));
            if (listingStandard.ButtonText("Reset to Default"))
            {
                settings.moodAmountMediumLow = -2f;
            }

            listingStandard.Label($"Somewhat fulfilled: {settings.moodAmountMedium}");
            settings.moodAmountMedium = Mathf.Round(listingStandard.Slider(settings.moodAmountMedium, -100f, 100f));
            if (listingStandard.ButtonText("Reset to Default"))
            {
                settings.moodAmountMedium = 2f;
            }

            listingStandard.Label($"Fulfilled: {settings.moodAmountMediumHigh}");
            settings.moodAmountMediumHigh = Mathf.Round(listingStandard.Slider(settings.moodAmountMediumHigh, -100f, 100f));
            if (listingStandard.ButtonText("Reset to Default"))
            {
                settings.moodAmountMediumHigh = 5f;
            }

            listingStandard.Label($"Highly fulfilled!: { settings.moodAmountHigh}");
            settings.moodAmountHigh = Mathf.Round(listingStandard.Slider(settings.moodAmountHigh, -100f, 100f));
            if (listingStandard.ButtonText("Reset to Default"))
            {
                settings.moodAmountHigh = 8f;
            }

            listingStandard.Label($"Extremely fulfilled!: {settings.moodAmountVeryHigh}");
            settings.moodAmountVeryHigh = Mathf.Round(listingStandard.Slider(settings.moodAmountVeryHigh, -100f, 100f));
            if (listingStandard.ButtonText("Reset to Default"))
            {
                settings.moodAmountVeryHigh = 10f;
            }

            if (listingStandard.ButtonText("Reset All to Defaults"))
            {
                settings.ResetToDefaults();
            }

            listingStandard.End();
            base.DoSettingsWindowContents(inRect);
        }
    }

}
