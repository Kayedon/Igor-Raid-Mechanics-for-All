using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace IgorRaidMechanics
{
    public class IgorRaidMechanicsSettings : ModSettings
    {
        public float damageMultiplier;
        public int disableThreatsAtPopulationCount;
        public List<string> goodIncidents = new List<string>();
        public bool firstTimeInit = true;
        public bool enableRaidWarning;
        public int raidWarningInterval;

        public List<string> baseGoodIncidents = new List<string>
        {
            "ResourcePodCrash",
            "PsychicSoothe",
            "SelfTame",
            "AmbrosiaSprout",
            "FarmAnimalsWanderIn",
            "WandererJoin",
            "RefugeePodCrash",
            "ThrumboPasses",
            "MeteoriteImpact",
            "WildManWandersIn"
        };
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref firstTimeInit, "firstTimeInit");
            Scribe_Values.Look(ref damageMultiplier, "damageMultiplier");
            Scribe_Values.Look(ref disableThreatsAtPopulationCount, "disableThreatsAtPopulationCount");
            Scribe_Values.Look(ref raidWarningInterval, "raidWarningInterval");
            Scribe_Values.Look(ref enableRaidWarning, "enableRaidWarning");
            Scribe_Collections.Look(ref goodIncidents, "goodIncidents");
        }

        public void DoSettingsWindowContents(Rect inRect)
        {
            var goodIncidentsLocal = goodIncidents.Where(x => DefDatabase<IncidentDef>.GetNamed(x) != null).OrderBy(x => DefDatabase<IncidentDef>.GetNamed(x).label).ToHashSet();
            goodIncidentsLocal.AddRange(baseGoodIncidents);
            var allIncidents = DefDatabase<IncidentDef>.AllDefs.Where(x => !goodIncidents.Contains(x.defName)).OrderBy(x => x.label).ToList();
            Rect rect = new Rect(inRect.x, inRect.y, inRect.width, inRect.height);
            Rect rect2 = new Rect(0f, 0f, inRect.width - 30f, 280 + (goodIncidentsLocal.Count * 24) + (allIncidents.Count * 24));
            Widgets.BeginScrollView(rect, ref scrollPosition, rect2, true);
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(rect2);
            if (listingStandard.ButtonText("Reset".Translate()))
            {
                firstTimeInit = true;
                DefsAlterer.DoDefsAlter();
            }
            listingStandard.SliderLabeled("Igor.AdjustDamageMultiplier".Translate(), ref damageMultiplier, damageMultiplier.ToStringDecimalIfSmall(), 0.1f, 5f);
            listingStandard.SliderLabeled("Igor.AdjustMinPopulationForThreats".Translate(), ref disableThreatsAtPopulationCount, disableThreatsAtPopulationCount.ToString(), 0, 99);
            listingStandard.CheckboxLabeled("Igor.EnableRaidWarning".Translate(), ref enableRaidWarning);
            if (enableRaidWarning)
            {
                listingStandard.SliderLabeled("Igor.AdjustRaidWarningInterval".Translate(), ref raidWarningInterval, raidWarningInterval.ToString(), 0, 24);
            }
            listingStandard.Label("Igor.PickGoodIncidentToFire".Translate());
            listingStandard.GapLine();
            foreach (var incident in goodIncidentsLocal)
            {
                bool test = goodIncidents.Contains(incident);
                listingStandard.CheckboxLabeled(DefDatabase<IncidentDef>.GetNamed(incident).label, ref test);
                if (!test)
                {
                    goodIncidents.Remove(incident);
                }
                else if (!goodIncidents.Contains(incident))
                {
                    goodIncidents.Add(incident);
                }
            }
            listingStandard.Gap();
            foreach (var incident in allIncidents)
            {
                bool test = false;
                listingStandard.CheckboxLabeled(incident.label, ref test);
                if (test)
                {
                    goodIncidents.Add(incident.defName);
                }
            }


            listingStandard.End();
            Widgets.EndScrollView();
            base.Write();
        }
        private static Vector2 scrollPosition = Vector2.zero;
    }
}