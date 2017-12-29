using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace BadMagic
{
    class Projectile_IceShart : Bullet
    {
        #region Properties
        //
        public ThingDef_IceShart Def
        {
            get
            {
                return this.def as ThingDef_IceShart;
            }
        }
        #endregion Properties

        #region Overrides
        protected override void Impact(Thing hitThing)
        {
            base.Impact(hitThing);

            if (Def != null && hitThing != null && hitThing is Pawn hitPawn)
            {
                var rand = Rand.Value;
                if (rand <= Def.AddHyphothermiaChance)
                {
                    Messages.Message("BM_IceShart_Hypothermia_SuccessMessage".Translate(new object[] {
                        this.launcher.Label, hitPawn.Label
                    }), MessageTypeDefOf.NegativeHealthEvent);

                    var pawnHypothermiaLevel = hitPawn?.health?.hediffSet?.GetFirstHediffOfDef(Def.HypothermiaHediff);
                    var randomSeverity = Rand.Range(0.10f, 0.30f);
                    if (pawnHypothermiaLevel != null)
                    {
                        pawnHypothermiaLevel.Severity += randomSeverity;
                    }
                    else
                    {
                        Hediff hediff = HediffMaker.MakeHediff(Def.HypothermiaHediff, hitPawn, null);
                        hediff.Severity = randomSeverity;
                        hitPawn.health.AddHediff(hediff, null, null);
                    }

                }
                else
                {
                    //MoteMaker.ThrowText(hitThing.PositionHeld.ToVector3(), hitThing.MapHeld, "BM_IceShart_Hypothermia_FailureMessage".Translate(Def.AddHyphothermiaChance), 3f);
                }

                /*
                rand = Rand.Value;
                if (rand <= Def.AddInfectionChance)
                {
                    var randomSeverity = Rand.Range(0.10f, 0.20f);

                    //IEnumerable<Hediff_Injury> injuries = hitPawn?.health?.hediffSet?.GetInjuriesTendable();
                    //Hediff_Injury injury = injuries.RandomElement<Hediff_Injury>();

                    //BodyPartRecord part = injury.Part;

                    IEnumerable<BodyPartRecord> parts = hitPawn?.health?.hediffSet?.GetInjuredParts();
                    if (parts != null)
                    {
                        BodyPartRecord part = parts.RandomElement<BodyPartRecord>();
                        //hitPawn.health.hediffSet.GetFirstHediffOfDef(Def.InfectionHediff).Part.parent.
                        //IEnumerable<Def.InfectionHediff> infectedParts = hitPawn?.health?.hediffSet?.GetHediffs<Def.InfectionHediff>();
                        if (part != null)
                        {
                            hitPawn.health.AddHediff(Def.InfectionHediff, part, null);
                        }
                        else
                        {
                            foreach (BodyPartRecord p in parts)
                            {
                                Messages.Message(p.ToString(), MessageTypeDefOf.NegativeEvent);
                            }
                        }
                    }
                }
                */
            }
        }
        #endregion Overrides

    }
}
