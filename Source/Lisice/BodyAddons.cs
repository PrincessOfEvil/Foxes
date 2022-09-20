using AlienRace;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Lisice
    {
    [StaticConstructorOnStartup]
    public static class HarmonyLoader
        {
        static HarmonyLoader() 
            {
            var harmony = new Harmony("princess.Lisice");
            Log.Message("Lisice patching...");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            var type = AccessTools.TypeByName("rjw.JobDriver_SexBaseInitiator");
            if (type != null)
                harmony.Patch(AccessTools.Method(type, "End"), postfix: new HarmonyMethod(typeof(Lisice_RJW_Support), "Postfix"));
            }
        }
    [HarmonyPatch(typeof(HarmonyPatches), "DrawAddonsFinalHook")]
    public static class Lisice_HAR_DrawAddonsFinalHook 
        {
        static void Postfix(Pawn pawn, AlienPartGenerator.BodyAddon addon, ref float angle, ref Vector3 offsetVector) 
            {
            AlienPartGenerator.RotationOffset? rotOffset = null;
            switch (pawn.Rotation.AsByte) 
                {
                case 0:
                    rotOffset = addon.offsets.north;
                    break;
                case 1:
                    rotOffset = addon.offsets.east;
                    break;
                case 2:
                    rotOffset = addon.offsets.south;
                    break;
                case 3:
                    rotOffset = addon.offsets.west;
                    break;
                }
            if (rotOffset != null && rotOffset is RotationOffset offset)
                angle += offset.angle + (pawn.ageTracker.AgeBiologicalYears < 800 && pawn.ageTracker.AgeBiologicalYears / 100 % 2 == 1 && !pawn.Rotation.IsHorizontal ? 13.5f : 0f);
            }
        }
    public class RotationOffset : AlienRace.AlienPartGenerator.RotationOffset
        {
        public float angle = 0f;
        }

    public class BodyAddon : AlienRace.AlienPartGenerator.BodyAddon
        {
        public int minAge = 0;

        public override bool CanDrawAddon(Pawn pawn) => pawn.ageTracker.AgeBiologicalYears >= minAge && base.CanDrawAddon(pawn);
        }
    }
