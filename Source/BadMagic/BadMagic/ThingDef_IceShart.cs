using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace BadMagic
{
    public class ThingDef_IceShart : ThingDef
    {
        public float AddInfectionChance = 0.5f;
        public HediffDef InfectionHediff = HediffDefOf.WoundInfection;
        
        public float AddHyphothermiaChance = 0.3f;
        public HediffDef HypothermiaHediff = HediffDefOf.Hypothermia;
    }
}
