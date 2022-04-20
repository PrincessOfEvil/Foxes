using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Zenko
    {
    public class SpecificApparelRequirement_HairColored : SpecificApparelRequirement
        {
        }

    [HarmonyPatch(typeof(PawnApparelGenerator), "PostProcessApparel")]
    static class Zenko_PawnApparelGenerator_PostProcessApparel
        {
        static void Postfix(ref Apparel apparel, ref Pawn pawn)
            {
            if (pawn?.kindDef?.specificApparelRequirements != null)
                foreach (var req in pawn.kindDef.specificApparelRequirements)
                    {
                    if (req is SpecificApparelRequirement_HairColored) apparel.SetColor(pawn.story.hairColor);
                    }
            }
        }

    [HarmonyPatch(typeof(PawnApparelGenerator), "CanUseStuff")]
    static class Zenko_PawnApparelGenerator_CanUseStuff
        {
        static bool Postfix(bool ret, Pawn pawn, ThingStuffPair pair)
            {
            if (pawn?.kindDef?.specificApparelRequirements != null)
                foreach (var req in pawn.kindDef.specificApparelRequirements)
                    {
                    if (req is SpecificApparelRequirement_HairColored && req.Stuff == pair.stuff) return true;
                    }
            return ret;
            }
        }
    }
