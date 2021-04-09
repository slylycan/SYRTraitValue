using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using RimWorld;
using Verse;
using UnityEngine;

namespace SyrTraitValue
{
    public class TraitValueCore : Mod
    {
        public static TraitValueSettings settings;

        public TraitValueCore(ModContentPack content) : base(content)
        {
            settings = GetSettings<TraitValueSettings>();
        }

        public override string SettingsCategory() => "TraitValueSettingsCategory".Translate();

        public override void DoSettingsWindowContents(Rect inRect)
        {
            checked
            {
                Listing_Standard listing_Standard = new Listing_Standard();
                listing_Standard.Begin(inRect);
                listing_Standard.CheckboxLabeled("SyrTraitValue_EnableColors".Translate(), ref TraitValueSettings.enableColors, "SyrTraitValue_EnableColorsTooltip".Translate());
                listing_Standard.GapLine(30);
                Rect rectDefaultSettings = listing_Standard.GetRect(30f);
                TooltipHandler.TipRegion(rectDefaultSettings, "RestoreToDefaultSettings".Translate());
                if (Widgets.ButtonText(rectDefaultSettings, "ResetBinding".Translate(), true, true, true))
                {
                    TraitValueSettings.enableColors = true;
                }
                listing_Standard.End();
                settings.Write();
            }
        }
        public override void WriteSettings()
        {
            base.WriteSettings();
            if (TraitValueSettings.enableColors)
            {
                TraitValueUtility.ColorTraits();
            }
            else
            {
                TraitValueUtility.UncolorTraits();
            }
        }
    }

    public class TraitValueSettings : ModSettings
    {
        public static bool enableColors = true;
        public static Color bestTraitColor = Color.cyan;
        public static Color goodTraitColor = Color.green;
        public static Color neutralTraitColor = Color.yellow;
        public static Color badTraitColor = Color.red;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<bool>(ref enableColors, "SyrTraitValue_enableColors", true, false);
            Scribe_Values.Look<Color>(ref bestTraitColor, "SyrTraitValue_bestTraitColor", Color.red, false);
            Scribe_Values.Look<Color>(ref goodTraitColor, "SyrTraitValue_goodTraitColor", Color.green, false);
            Scribe_Values.Look<Color>(ref neutralTraitColor, "SyrTraitValue_neutralTraitColor", Color.yellow, false);
            Scribe_Values.Look<Color>(ref badTraitColor, "SyrTraitValue_redTraitColor", Color.red, false);
        }
    }
}
