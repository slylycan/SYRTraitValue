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
            LongEventHandler.ExecuteWhenFinished(() => badColorButton = SolidColorMaterials.NewSolidColorTexture(TraitValueSettings.badTraitColor));
        }

        public override string SettingsCategory() => "TraitValueSettingsCategory".Translate();

        private Vector2 scrollPosition = new Vector2(0f, 0f);
        public static int traitCount = 0;
        public static Texture2D badColorButton;
        public override void DoSettingsWindowContents(Rect inRect)
        {
            checked
            {
                Listing_Standard listing_Standard = new Listing_Standard();
                listing_Standard.Begin(inRect);
                listing_Standard.CheckboxLabeled("SyrTraitValue_EnableColors".Translate(), ref TraitValueSettings.enableColors, "SyrTraitValue_EnableColorsTooltip".Translate());
                listing_Standard.Gap(12);

                listing_Standard.Label("SyrTraitValue_BadColor", -1, "SyrTraitValue_BadColorTooltip");
                if (listing_Standard.ButtonImage(badColorButton, 128f, 24f))
                {
                    Find.WindowStack.Add(new Dialog_ColorPicker(TraitValueSettings.badTraitColor, delegate(Color color) { TraitValueSettings.badTraitColor = color; badColorButton = SolidColorMaterials.NewSolidColorTexture(TraitValueSettings.badTraitColor); badColorButton.Apply(false); TraitValueUtility.ColorTraitLabels(); } ));
                }
                listing_Standard.Gap(12);

                Rect outRect = inRect;
                outRect.height -= 120f;
                outRect.y += 60f;
                Rect viewRect = inRect.GetInnerRect();
                viewRect.height = 27f * traitCount * 0.34f;
                Rect rowRect1 = new Rect(viewRect.x, viewRect.y, viewRect.width * 0.33f, 24f);
                Rect rowRect2 = new Rect(viewRect.x + viewRect.width * 0.33f, viewRect.y, viewRect.width * 0.33f, 24f);
                Rect rowRect3 = new Rect(viewRect.x + viewRect.width * 0.67f, viewRect.y, viewRect.width * 0.33f, 24f);
                Widgets.BeginScrollView(outRect, ref scrollPosition, viewRect);
                int i = 0;
                int threshold = Mathf.CeilToInt(traitCount * 0.333f);
                Dictionary<TraitDegreeData, string> buffers = new Dictionary<TraitDegreeData, string>();
                foreach (TraitDef t in TraitValueUtility.allTraits)
                {
                    foreach (TraitDegreeData tdd in t.degreeDatas)
                    {
                        i++;
                        buffers.TryGetValue(tdd, out string buffer);
                        if (i <= threshold)
                        {
                            TextFieldNumericLabeled(rowRect1, tdd.label, ref t.GetModExtension<TraitValueExtension>().traitValues.Find(dv => dv.degree == tdd.degree).value, ref buffer, -99, 99);
                            rowRect1.y = rowRect1.yMax + 2f;
                        }
                        else if (i <= threshold * 2)
                        {
                            TextFieldNumericLabeled(rowRect2, tdd.label, ref t.GetModExtension<TraitValueExtension>().traitValues.Find(dv => dv.degree == tdd.degree).value, ref buffer, -99, 99);
                            rowRect2.y = rowRect2.yMax + 2f;
                        }
                        else
                        {
                            TextFieldNumericLabeled(rowRect3, tdd.label, ref t.GetModExtension<TraitValueExtension>().traitValues.Find(dv => dv.degree == tdd.degree).value, ref buffer, -99, 99);
                            rowRect3.y = rowRect3.yMax + 2f;
                        }
                    }
                }
                Widgets.EndScrollView();

                listing_Standard.Gap(inRect.height - 120f);
                listing_Standard.GapLine(30f);
                Rect rectDefaultSettings = listing_Standard.GetRect(30f);
                TooltipHandler.TipRegion(rectDefaultSettings, "RestoreToDefaultSettings".Translate());
                if (Widgets.ButtonText(rectDefaultSettings, "ResetBinding".Translate(), true, true, true))
                {
                    TraitValueSettings.enableColors = true;
                    TraitValueUtility.LoadSavedValues(true);
                    TraitValueSettings.changedTraitValues.Clear();
                }
                listing_Standard.End();
                settings.Write();
            }
        }

        public static void TextFieldNumericLabeled<T>(Rect rect, string label, ref T val, ref string buffer, float min = 0f, float max = 1E+09f) where T : struct
        {
            Rect rectLabel = new Rect(rect.x, rect.y, rect.width * 0.77f, rect.height);
            Rect rectField = new Rect(rect.x + rect.width * 0.8f, rect.y, rect.width * 0.2f, rect.height);
            TextAnchor anchor = Text.Anchor;
            Text.Anchor = TextAnchor.MiddleRight;
            Widgets.Label(rectLabel, label);
            Text.Anchor = anchor;
            Widgets.TextFieldNumeric<T>(rectField, ref val, ref buffer, min, max);
        }

        public override void WriteSettings()
        {
            base.WriteSettings();
            if (!TraitValueSettings.enableColors)
            {
                TraitValueUtility.UncolorTraitLabels();
            }
            else
            {
                TraitValueUtility.ColorTraitLabels();
            }
            foreach (TraitDef t in TraitValueUtility.allTraits)
            {
                foreach (TraitDegreeData tdd in t.degreeDatas)
                {
                    TraitValueExtension modExtension = t.GetModExtension<TraitValueExtension>();
                    if (modExtension != null)
                    {
                        string key = t.defName + ", " + tdd.degree;
                        if (newTraitValues.ContainsKey(key))
                        {
                            newTraitValues[key] = modExtension.traitValues.Find(dv => dv.degree == tdd.degree).value;
                        }
                        else
                        {
                            newTraitValues.Add(t.defName + ", " + tdd.degree, modExtension.traitValues.Find(dv => dv.degree == tdd.degree).value);
                        }
                    }
                }
            }
            TraitValueSettings.changedTraitValues = newTraitValues.Where(kvp => TraitValueUtility.originalTraitValues[kvp.Key] != kvp.Value).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            //Log.Message("new Trait Values: " + newTraitValues.Keys.Count.ToString());
            //Log.Message("original Trait Values: " + TraitValueUtility.originalTraitValues.Keys.Count.ToString());
            //Log.Message("changed Trait Values: " + TraitValueSettings.changedTraitValues.Keys.Count.ToString());
        }
        public static Dictionary<string, int> newTraitValues = new Dictionary<string, int>();
    }

    public class TraitValueSettings : ModSettings
    {
        public static bool enableColors = true;
        public static Color bestTraitColor = Color.cyan;
        public static Color goodTraitColor = Color.green;
        public static Color neutralTraitColor = Color.yellow;
        public static Color badTraitColor = Color.red;
        public static Dictionary<string, int> changedTraitValues = new Dictionary<string, int>();

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<bool>(ref enableColors, "SyrTraitValue_enableColors", true, false);
            Scribe_Values.Look<Color>(ref bestTraitColor, "SyrTraitValue_bestTraitColor", Color.cyan, false);
            Scribe_Values.Look<Color>(ref goodTraitColor, "SyrTraitValue_goodTraitColor", Color.green, false);
            Scribe_Values.Look<Color>(ref neutralTraitColor, "SyrTraitValue_neutralTraitColor", Color.yellow, false);
            Scribe_Values.Look<Color>(ref badTraitColor, "SyrTraitValue_redTraitColor", Color.red, false);
            Scribe_Collections.Look<string, int>(ref changedTraitValues, "SyrTraitValue_changedTraitValues", LookMode.Value, LookMode.Value);
        }
    }
}
