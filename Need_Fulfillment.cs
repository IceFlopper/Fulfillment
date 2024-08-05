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
                this.def = DefDatabase<NeedDef>.GetNamed("Fulfillment_Fulfillment", true);
            }
            catch (Exception ex)
            {
                Log.Error($"Need_Fulfillment constructor: {ex.Message}");
            }
            this.threshPercents = new List<float>
            {
                0.05f, 0.20f, 0.40f, 0.55f, 0.80f, 0.95f
            };
        }

        private float FulfillmentRate
        {
            get
            {
                if (this.pawn != null)
                {
                    float comfort = this.pawn.GetStatValue(StatDefOf.Comfort, true, -1);
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
                if (this.CurLevel < 0.05f)
                {
                    return FulfillmentCategory.VeryLow;
                }
                if (this.CurLevel < 0.20f && this.CurLevel >= 0.05f)
                {
                    return FulfillmentCategory.Low;
                }
                if (this.CurLevel < 0.40f && this.CurLevel >= 0.20f)
                {
                    return FulfillmentCategory.MediumLow;
                }
                if (this.CurLevel >= 0.50f && this.CurLevel < 0.65f)
                {
                    return FulfillmentCategory.Medium;
                }
                if (this.CurLevel >= 0.65f && this.CurLevel < 0.80f)
                {
                    return FulfillmentCategory.MediumHigh;
                }
                return FulfillmentCategory.High; // for CurLevel >= 0.80f
            }
        }

        private float FallPerTick
        {
            get
            {
                float baseRate = 0.0000012f * this.FulfillmentRate; // Base rate for depletion
                if (this.CurLevel >= 0.95f)
                {
                    return baseRate * 1.2f; // Faster depletion above 95%
                }
                if (this.CurLevel >= 0.80f)
                {
                    return baseRate * 1.1f; // Faster depletion above 80%
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

        public void GainFulfillment(SkillRecord skill)
        {
            float gainRate = 0.00001f; // Base rate for gain
            if (this.CurLevel <= 0.05f)
            {
                gainRate *= 1.2f; // Faster gain when at extreme low
            }
            if (skill.passion == Passion.Minor)
            {
                gainRate *= 1f; // Increase gain rate for passion
            }
            else if (skill.passion == Passion.Major)
            {
                gainRate *= 1.25f; // Further increase gain rate for burning passion
            }
            this.lastGainTick = Find.TickManager.TicksGame;
            this.CurLevel = Mathf.Min(this.CurLevel + gainRate, 1f); // Reduced gain rate
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
                this.CurLevel = 0.3f; // Start at 30% in god mode
                return;
            }
            this.CurLevel = 0.3f; // Start at 30% normally
        }

        public override void NeedInterval()
        {
            float oldCurLevel = this.CurLevel;
            this.CurLevel -= this.FallPerTick * 150f;

            if (this.CurLevel < 0f)
            {
                this.CurLevel = 0f;
            }
            ApplyMoodEffects();

            if (this.CurCategory <= FulfillmentCategory.Low && this.pawn != null && this.pawn.CurrentBed() != null && this.pawn.health.capacities.CanBeAwake)
            {
                this.CurLevel = 1f;
                return;
            }

            if (this.CurCategory <= FulfillmentCategory.Low)
            {
                this.ticksAtZero += 150;
            }
            else
            {
                this.ticksAtZero = 0;
            }
        }

        private void ApplyMoodEffects()
        {
            if (this.pawn.needs.mood == null) return;

            ThoughtDef thoughtDef = null;

            if (this.CurLevel < 0.05f)
            {
                thoughtDef = CustomThoughtDefOf.Fulfillment_FulfillmentVeryLow;
            }
            else if (this.CurLevel < 0.20f && this.CurLevel >= 0.05f)
            {
                thoughtDef = CustomThoughtDefOf.Fulfillment_FulfillmentLow;
            }
            else if (this.CurLevel < 0.40f && this.CurLevel >= 0.20f)
            {
                thoughtDef = CustomThoughtDefOf.Fulfillment_FulfillmentMediumLow;
            }
            else if (this.CurLevel >= 0.50f && this.CurLevel < 0.65f)
            {
                thoughtDef = CustomThoughtDefOf.Fulfillment_FulfillmentMedium;
            }
            else if (this.CurLevel >= 0.65f && this.CurLevel < 0.80f)
            {
                thoughtDef = CustomThoughtDefOf.Fulfillment_FulfillmentMediumHigh;
            }
            else if (this.CurLevel >= 0.80f && this.CurLevel < 0.95f)
            {
                thoughtDef = CustomThoughtDefOf.Fulfillment_FulfillmentHigh;
            }
            else if (this.CurLevel >= 0.95f)
            {
                thoughtDef = CustomThoughtDefOf.Fulfillment_FulfillmentVeryHigh;
            }

            if (thoughtDef != null)
            {
                ApplyHediffOrThought(thoughtDef);
            }
        }

        private void ApplyHediffOrThought(ThoughtDef thoughtDef)
        {
            if (this.pawn.needs.mood.thoughts.memories.Memories.Exists(m => m.def == thoughtDef))
            {
                var memory = this.pawn.needs.mood.thoughts.memories.Memories.Find(m => m.def == thoughtDef);
                if (memory != null)
                {
                    memory.Renew();
                }
            }
            else
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
