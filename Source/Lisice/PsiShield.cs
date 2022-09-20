using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI.Group;
using Verse.Sound;

namespace Lisice
    {
    [HarmonyPatch(typeof(Thing), "PreApplyDamage")]
    public static class Lisice_Pawn_PreApplyDamage
		{
		static void Postfix(ref DamageInfo dinfo, ref bool absorbed, Thing __instance) 
            {
			var pawn = __instance as Pawn;
			if (pawn == null) return;
			var ability = pawn.abilities?.GetAbility(LisiceDefOf.Lisice_AbilityPsiShield);
			// Log.Message(LisiceDefOf.Lisice_AbilityPsiShield?.defName + ability?.def.defName + ability?.GizmoDisabled(out _).ToString() + pawn.psychicEntropy?.CurrentPsyfocus);
			if (ability != null && ability.CooldownTicksTotal != ability.def.cooldownTicksRange.max && pawn.psychicEntropy.CurrentPsyfocus > 0)
                {
				// I could skip this copypaste with a transpiler, sure, but at what cost?
				Faction homeFaction = pawn.HomeFaction;
				if (dinfo.Instigator != null && homeFaction != null && homeFaction.IsPlayer && !pawn.InAggroMentalState)
					{
					Pawn pawnInst = dinfo.Instigator as Pawn;
					if (dinfo.InstigatorGuilty && pawnInst != null && pawnInst.guilt != null && pawnInst.mindState != null)
						{
						pawnInst.guilt.Notify_Guilty();
						}
					}
				if (pawn.Spawned)
					{
					if (!pawn.Position.Fogged(pawn.Map))
						{
						pawn.mindState.Active = true;
						}
					pawn.GetLord()?.Notify_PawnDamaged(pawn, dinfo);
					if (dinfo.Def.ExternalViolenceFor(pawn))
						{
						GenClamor.DoClamor(pawn, 18f, ClamorDefOf.Harm);
						}
					pawn.jobs.Notify_DamageTaken(dinfo);
					}
				if (homeFaction != null)
					{
					homeFaction.Notify_MemberTookDamage(pawn, dinfo);
					if (Current.ProgramState == ProgramState.Playing && homeFaction == Faction.OfPlayer && dinfo.Def.ExternalViolenceFor(pawn) && pawn.SpawnedOrAnyParentSpawned)
						{
						pawn.MapHeld.dangerWatcher.Notify_ColonistHarmedExternally();
						}
					}

				ability.CompOfType<CompAbilityEffect_FleckOnTarget>()?.Apply(pawn, pawn);

				pawn.psychicEntropy.OffsetPsyfocusDirectly(-dinfo.Amount * ability.def.GetStatValueAbstract(StatDefOf.Ability_PsyfocusCost));
                pawn.psychicEntropy.TryAddEntropy(dinfo.Amount * ability.def.GetStatValueAbstract(StatDefOf.Ability_EntropyGain), overLimit: true);

                absorbed = true;
                }
            }
        }
	public class Command_ActuallyToggle : Command_Ability
		{
		public new static readonly Texture2D BGTex = ContentFinder<Texture2D>.Get("UI/Widgets/AbilityButBG");
		public static readonly Texture2D BGTexOff = ContentFinder<Texture2D>.Get("AbilityButBGOff");
		public bool isOn => ability.CooldownTicksTotal != ability.def.cooldownTicksRange.max;
		public override Texture2D BGTexture => isOn? BGTex : BGTexOff;
		public Command_ActuallyToggle(Ability ability) : base(ability) { }
		public override void ProcessInput(Event ev)
			{
			SoundDefOf.Tick_Tiny.PlayOneShotOnCamera();
			if (ability.CooldownTicksRemaining <= 0)
				{
				if (ability.CooldownTicksTotal == ability.def.cooldownTicksRange.max)
					ability.StartCooldown(ability.def.cooldownTicksRange.min);
				else
					ability.StartCooldown(ability.def.cooldownTicksRange.max);
				}
			}
		public override bool GroupsWith(Gizmo other)
			{
			bool gv = base.GroupsWith(other);
			if (gv)
				return !(other is Command_ActuallyToggle toggle && toggle.isOn != isOn);
			return gv;
			}
		}
	}
