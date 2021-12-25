using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using VanillaStorytellersExpanded;
using Verse;

namespace IgorRaidMechanics
{
    public class IgorRaidMechanicsMod : Mod
    {
        public static IgorRaidMechanicsSettings settings;
        public IgorRaidMechanicsMod(ModContentPack pack) : base(pack)
        {
            settings = GetSettings<IgorRaidMechanicsSettings>();
        }
        public override void DoSettingsWindowContents(Rect inRect)
        {
            base.DoSettingsWindowContents(inRect);
            settings.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return "[KYD] Igor Raid Mechanics for All Storytellers";
        }

        public override void WriteSettings()
        {
            base.WriteSettings();
            DefsAlterer.DoDefsAlter();
        }
    }
    [StaticConstructorOnStartup]
    public static class DefsAlterer
    {
        static DefsAlterer()
        {
            DoDefsAlter();
        }
        public static void DoDefsAlter()
        {
            if (IgorRaidMechanicsMod.settings.firstTimeInit)
            {
                IgorRaidMechanicsMod.settings.damageMultiplier = 2f;
                IgorRaidMechanicsMod.settings.disableThreatsAtPopulationCount = 1;
                IgorRaidMechanicsMod.settings.goodIncidents = IgorRaidMechanicsMod.settings.baseGoodIncidents.ListFullCopy();
                IgorRaidMechanicsMod.settings.firstTimeInit = false;
                IgorRaidMechanicsMod.settings.enableRaidWarning = false;
            }

            foreach (var storytellerDef in DefDatabase<StorytellerDef>.AllDefs)
            {
                var modExtension = storytellerDef.GetModExtension<StorytellerDefExtension>();
                if (modExtension is null)
                {
                    modExtension = new StorytellerDefExtension();
                    if (storytellerDef.modExtensions is null) 
                        storytellerDef.modExtensions = new List<DefModExtension>();
                    storytellerDef.modExtensions.Add(modExtension);
                }
                AssignValues(ref modExtension);
            }
        }

        private static void AssignValues(ref StorytellerDefExtension modExtension)
        {
            modExtension.storytellerThreat = new StorytellerThreat();
            modExtension.storytellerThreat.allDamagesMultiplier = IgorRaidMechanicsMod.settings.damageMultiplier;
            modExtension.storytellerThreat.disableThreatsAtPopulationCount = IgorRaidMechanicsMod.settings.disableThreatsAtPopulationCount;
            modExtension.storytellerThreat.goodIncidents = IgorRaidMechanicsMod.settings.goodIncidents;
            if (IgorRaidMechanicsMod.settings.enableRaidWarning)
            {
                var hour = IgorRaidMechanicsMod.settings.raidWarningInterval * 2500;
                modExtension.storytellerThreat.raidWarningRange = new IntRange(hour, hour);
            }
            else
            {
                modExtension.storytellerThreat.raidWarningRange = null;
            }
        }
    }
}