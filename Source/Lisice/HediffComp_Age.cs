using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using UnityEngine;
using Verse;

namespace Lisice
    {
    public class HediffComp_Age : HediffComp
        {
        public HediffCompProperties_Age Props => (HediffCompProperties_Age)base.props;


        public override void CompPostTick(ref float severityAdjustment)
            {
            if ((GenTicks.TicksGame % GenDate.TicksPerDay).isNear(420)) return;

            parent.Severity = Mathf.Clamp01(Pawn.ageTracker.AgeBiologicalYearsFloat / Props.ageDivisor);
            }
        }

    public class HediffCompProperties_Age : HediffCompProperties
        {
        public float ageDivisor = 1f;
        public HediffCompProperties_Age()
            {
            base.compClass = typeof(HediffCompProperties_Age);
            }
        }

    public class HediffGiver_Lisice : HediffGiver
        {
        public override void OnIntervalPassed(Pawn pawn, Hediff cause)
            {
            if ((GenTicks.TicksGame % GenDate.TicksPerDay).isNear(69)) return;

            TryApply(pawn);
            }
        }
    public class HediffGiver_LisicePsi : HediffGiver
        {
        public override void OnIntervalPassed(Pawn pawn, Hediff cause)
            {
            if ((GenTicks.TicksGame % GenDate.TicksPerDay).isNear(69)) return;
            var source = pawn.health.hediffSet.GetNotMissingParts().Where((BodyPartRecord p) => partsToAffect.Contains(p.def)).Where((BodyPartRecord p) => !pawn.health.hediffSet.HasHediff(this.hediff, p) && !pawn.health.hediffSet.PartOrAnyAncestorHasDirectlyAddedParts(p));
            if (!source.Any())
                {
                return;
                }
            BodyPartRecord partRecord = source.RandomElementByWeight<BodyPartRecord>((BodyPartRecord x) => x.coverageAbs);
            Hediff_Psylink hediff = (Hediff_Psylink)HediffMaker.MakeHediff(this.hediff, pawn, partRecord);
            hediff.suppressPostAddLetter = true;
            pawn.health.AddHediff(hediff);
            }
        }

    public static class Extensions
        {
        public static bool isNear(this int baseIn, int against)
            {
            // Chosen by a fair dice roll.
            // Guaranteed to be random.
            return baseIn.isNear(against, 3);
            }
        public static bool isNear(this int baseIn, int against, int nearness)
            {
            return Mathf.Abs(baseIn - against) < nearness;
            }
        }
    }
