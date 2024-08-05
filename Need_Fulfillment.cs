using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace FulfillmentModNamespace
{
    public class Need_Fulfillment : Need
    {
        public Need_Fulfillment(Pawn pawn) : base(pawn)
        {
            try
            {
                //Log.Message("Need_Fulfillment: Trying to get NeedDef 'Fulfillment_Fulfillment'");
                this.def = DefDatabase<NeedDef>.GetNamed("Fulfillment_Fulfillment", true);
                //Log.Message("Need_Fulfillment: Successfully got NeedDef 'Fulfillment_Fulfillment'");
            }
            catch (Exception ex)
            {
                Log.Error($"Need_Fulfillment constructor: {ex.Message}");
            }
            this.threshPercents = new List<float>
            {
                0.05f, 0.20f, 0.30f, 0.60f, 0.75f, 0.90f // Example thresholds
            };
        }

        private float FulfillmentRate
        {
            get
            {
                if (this.pawn != null)
                {
                    float comfort = this.pawn.GetStatValue(StatDefOf.Comfort, true, -1);
                    //Log.Message($"FulfillmentRate: Pawn comfort value = {comfort}");
                    return comfort > 0 ? comfort : 1f; // Ensure comfort is not zero to avoid zero FallPerTick
                }
                Log.Error("FulfillmentRate: Pawn is null");
                return 1f; // Default fallback value
            }
        }

        public FulfillmentCategory CurCategory
        {
            get
            {
                if (this.CurLevel <= 0.05f)
                {
                    return FulfillmentCategory.VeryLow;
                }
                if (this.CurLevel <= 0.20f)
                {
                    return FulfillmentCategory.Low;
                }
                if (this.CurLevel <= 0.30f)
                {
                    return FulfillmentCategory.MediumLow;
                }
                if (this.CurLevel <= 0.60f)
                {
                    return FulfillmentCategory.Medium;
                }
                if (this.CurLevel <= 0.75f)
                {
                    return FulfillmentCategory.MediumHigh;
                }
                return FulfillmentCategory.High;
            }
        }

        private float FallPerTick
        {
            get
            {
                float baseRate = 0.0000001f * this.FulfillmentRate; // Base rate for depletion
                if (this.CurLevel >= 0.90f)
                {
                    return baseRate * 2; // Faster depletion when at extreme ends
                }
                return baseRate;
            }
        }

        private bool ReceivingFulfillment
        {
            get
            {
                return Find.TickManager.TicksGame < this.lastGainTick + 10;
            }
        }

        public override int GUIChangeArrow
        {
            get
            {
                if (this.ReceivingFulfillment)
                {
                    return 1;
                }
                return -1;
            }
        }

        public void GainFulfillment()
        {
            float gainRate = 0.000015f; // Base rate for gain
            if (this.CurLevel <= 0.05f)
            {
                gainRate *= 2; // Faster gain when at extreme low
            }
            this.lastGainTick = Find.TickManager.TicksGame;
            this.CurLevel = Mathf.Min(this.CurLevel + gainRate, 1f); // Reduced gain rate
            //Log.Message($"GainFulfillment called: CurLevel={this.CurLevel}");
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref this.ticksAtZero, "ticksAtZero", 0, false);
        }

        public override void SetInitialLevel()
        {
            if (DebugSettings.godMode)
            {
                this.CurLevel = 0.2f; // Start at 20% in god mode
                return;
            }
            this.CurLevel = 0.2f; // Start at 20% normally
        }

        public override void NeedInterval()
        {
            //Log.Message($"NeedInterval called: CurLevel={this.CurLevel}, FallPerTick={this.FallPerTick}");
            float oldCurLevel = this.CurLevel;
            this.CurLevel -= this.FallPerTick * 150f;
            //Log.Message($"NeedInterval updated: oldCurLevel={oldCurLevel}, newCurLevel={this.CurLevel}");

            if (this.CurLevel < 0f)
            {
                this.CurLevel = 0f;
            }
            ApplyMoodEffects();

            // Check if there are any unexpected conditions
            //Log.Message($"NeedInterval after ApplyMoodEffects: CurLevel={this.CurLevel}");

            if (this.CurCategory <= FulfillmentCategory.Low && this.pawn != null && this.pawn.CurrentBed() != null && this.pawn.health.capacities.CanBeAwake)
            {
                this.CurLevel = 1f;
                //Log.Message("NeedInterval: Reset CurLevel to 1f due to low category and bed condition");
                return;
            }

            //Log.Message($"NeedInterval before ticksAtZero update: CurCategory={this.CurCategory}, ticksAtZero={this.ticksAtZero}");

            if (this.CurCategory <= FulfillmentCategory.Low)
            {
                this.ticksAtZero += 150;
            }
            else
            {
                this.ticksAtZero = 0;
            }

            //Log.Message($"NeedInterval end: CurLevel={this.CurLevel}, ticksAtZero={this.ticksAtZero}");
        }

        private void ApplyMoodEffects()
        {
            if (this.pawn.needs.mood == null) return;

            if (this.CurLevel < 0.05f)
            {
                ApplyHediffOrThought(CustomThoughtDefOf.FulfillmentVeryLow);
            }
            else if (this.CurLevel < 0.20f)
            {
                ApplyHediffOrThought(CustomThoughtDefOf.FulfillmentLow);
            }
            else if (this.CurLevel < 0.30f)
            {
                ApplyHediffOrThought(CustomThoughtDefOf.FulfillmentMediumLow);
            }
            else if (this.CurLevel < 0.60f)
            {
                ApplyHediffOrThought(CustomThoughtDefOf.FulfillmentMedium);
            }
            else if (this.CurLevel < 0.75f)
            {
                ApplyHediffOrThought(CustomThoughtDefOf.FulfillmentMediumHigh);
            }
            else if (this.CurLevel >= 0.90f)
            {
                ApplyHediffOrThought(CustomThoughtDefOf.FulfillmentHigh);
            }
        }

        private void ApplyHediffOrThought(ThoughtDef thoughtDef)
        {
            if (!this.pawn.needs.mood.thoughts.memories.Memories.Exists(m => m.def == thoughtDef))
            {
                this.pawn.needs.mood.thoughts.memories.TryGainMemory(thoughtDef);
            }
        }

        private int lastGainTick = -999;
        public int ticksAtZero;
    }

    public enum FulfillmentCategory
    {
        VeryLow,
        Low,
        MediumLow,
        Medium,
        MediumHigh,
        High
    }
}
