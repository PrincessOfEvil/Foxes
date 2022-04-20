using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace Zenko
    {
    [HarmonyPatch(typeof(JobDriver_Lovin), "MakeNewToils")]
    public static class Zenko_JobDriver_Lovin_MakeNewToils
        {
        static IEnumerable<Toil> Postfix(IEnumerable<Toil> toils, JobDriver_Lovin __instance)
            {
            foreach (Toil toil in toils) yield return toil;
            var partner = (Pawn)(Thing)__instance.job.GetTarget(TargetIndex.A);
            if (__instance.pawn.HasModExtension<DefExtension_GainsPsyfocusFromLovin>() ^ partner.HasModExtension<DefExtension_GainsPsyfocusFromLovin>())
                yield return new Toil()
                    {
                    initAction = delegate
                        {
                        Pawn pawn, part;
                            if (__instance.pawn.HasModExtension<DefExtension_GainsPsyfocusFromLovin>()) { pawn = __instance.pawn; part = partner; }
                            else { pawn = partner; part = __instance.pawn; }
                        pawn.psychicEntropy.OffsetPsyfocusDirectly(pawn.GetModExtension<DefExtension_GainsPsyfocusFromLovin>().psyfocusGain / 100f);
                        if (part.needs.rest != null)
                            part.needs.rest.CurLevel -= pawn.GetModExtension<DefExtension_GainsPsyfocusFromLovin>().drain / 100;
                        else if (part.needs.food != null)
                            part.needs.food.CurLevel -= pawn.GetModExtension<DefExtension_GainsPsyfocusFromLovin>().drain / 100;
                        },
                    defaultCompleteMode = ToilCompleteMode.Instant
                    };
            }
        }
    public class DefExtension_GainsPsyfocusFromLovin : DefModExtension
        {
        public float psyfocusGain = 5f;
        public float drain = 5f;
        public DefExtension_GainsPsyfocusFromLovin() { }
        }
    }
