using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace BadMagic
{
    class DamageWorker_IceShart : DamageWorker_AddInjury
    {
        protected override BodyPartRecord ChooseHitPart(DamageInfo dinfo, Pawn pawn)
        {
            return pawn.health.hediffSet.GetRandomNotMissingPart(dinfo.Def, dinfo.Height, BodyPartDepth.Outside);
        }

        protected void MaybeInfectWound(Pawn pawn, DamageInfo dinfo)
        {
            float rand = Rand.Value;
            float infectionThreshold = 0.3f;
            //String message = "Random value was: " + rand + ", Infection threshold is: " + infectionThreshold;
            //Messages.Message(message, MessageTypeDefOf.NeutralEvent);
            if (rand <= infectionThreshold)
            {
                //Messages.Message("Added infection hediff", MessageTypeDefOf.NeutralEvent);
                pawn.health.AddHediff(HediffDefOf.WoundInfection, dinfo.HitPart);
            }
        }

        protected void MaybeCauseHypothermia(Pawn pawn, DamageInfo dinfo)
        {
            float rand = Rand.Value;
            float addHypothermiaChance = 0.3f;
            if (rand <= addHypothermiaChance)
            {
                Messages.Message("BM_IceShart_Hypothermia_SuccessMessage".Translate(new object[] {
                        dinfo.Instigator.Label, pawn.Label
                    }), MessageTypeDefOf.NegativeHealthEvent);

                var pawnHypothermiaLevel = pawn?.health?.hediffSet?.GetFirstHediffOfDef(HediffDefOf.Hypothermia);
                var randomSeverity = Rand.Range(0.10f, 0.30f);
                if (pawnHypothermiaLevel != null)
                {
                    pawnHypothermiaLevel.Severity += randomSeverity;
                }
                else
                {
                    Hediff hediff = HediffMaker.MakeHediff(HediffDefOf.Hypothermia, pawn, null);
                    hediff.Severity = randomSeverity;
                    pawn.health.AddHediff(hediff, null, null);
                }

            }
            else
            {
                //MoteMaker.ThrowText(hitThing.PositionHeld.ToVector3(), hitThing.MapHeld, "BM_IceShart_Hypothermia_FailureMessage".Translate(Def.AddHyphothermiaChance), 3f);
            }
        }

        protected override void ApplySpecialEffectsToPart(Pawn pawn, float totalDamage, DamageInfo dinfo, ref DamageWorker.DamageResult result)
        {
            if (dinfo.HitPart.depth == BodyPartDepth.Inside)
            {
                List<BodyPartRecord> list = new List<BodyPartRecord>();
                for (BodyPartRecord bodyPartRecord = dinfo.HitPart; bodyPartRecord != null; bodyPartRecord = bodyPartRecord.parent)
                {
                    list.Add(bodyPartRecord);
                    if (bodyPartRecord.depth == BodyPartDepth.Outside)
                    {
                        break;
                    }
                }

                MaybeCauseHypothermia(pawn, dinfo);

                float num = (float)(list.Count - 1) + 0.5f;
                for (int i = 0; i < list.Count; i++)
                {
                    DamageInfo dinfo2 = dinfo;
                    dinfo2.SetHitPart(list[i]);
                    MaybeInfectWound(pawn, dinfo2);
                    base.FinalizeAndAddInjury(pawn, totalDamage / num * ((i != 0) ? 1f : 0.5f), dinfo2, ref result);
                }
            }
            else
            {
                int num2 = (this.def.cutExtraTargetsCurve == null) ? 0 : GenMath.RoundRandom(this.def.cutExtraTargetsCurve.Evaluate(Rand.Value));
                List<BodyPartRecord> list2;
                if (num2 != 0)
                {
                    IEnumerable<BodyPartRecord> enumerable = dinfo.HitPart.GetDirectChildParts();
                    if (dinfo.HitPart.parent != null)
                    {
                        enumerable = enumerable.Concat(dinfo.HitPart.parent);
                        enumerable = enumerable.Concat(dinfo.HitPart.parent.GetDirectChildParts());
                    }
                    list2 = enumerable.Except(dinfo.HitPart).InRandomOrder(null).Take(num2).ToList<BodyPartRecord>();
                }
                else
                {
                    list2 = new List<BodyPartRecord>();
                }
                list2.Add(dinfo.HitPart);
                float num3 = totalDamage * (1f + this.def.cutCleaveBonus) / ((float)list2.Count + this.def.cutCleaveBonus);
                if (num2 == 0)
                {
                    num3 = base.ReduceDamageToPreserveOutsideParts(num3, dinfo, pawn);
                }

                MaybeCauseHypothermia(pawn, dinfo);

                for (int j = 0; j < list2.Count; j++)
                {
                    DamageInfo dinfo3 = dinfo;
                    dinfo3.SetHitPart(list2[j]);
                    MaybeInfectWound(pawn, dinfo3);
                    base.FinalizeAndAddInjury(pawn, num3, dinfo3, ref result);
                }
            }
        }
    }
}
