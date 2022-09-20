using RimWorld;
using Verse;

namespace Lisice
    {
    public static class Util
        {
        public static bool HasModExtension<T>(this Pawn pawn) where T : DefModExtension
            {
            return pawn.def.HasModExtension<T>() || pawn.kindDef.HasModExtension<T>();
            }
        public static T GetModExtension<T>(this Pawn pawn) where T : DefModExtension
            {
            T ext = null;
            if (pawn.HasModExtension<T>())
                {
                ext = pawn.kindDef.GetModExtension<T>();
                if (ext == null)
                    {
                    ext = pawn.def.GetModExtension<T>();
                    }
                }
            return ext;
            }
        }
    [DefOf]
    public static class LisiceDefOf
        {
        public static AbilityDef Lisice_AbilityPsiShield;
        }
    }
