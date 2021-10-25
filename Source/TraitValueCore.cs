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

        private Vector2 scrollPosition = new Vector2(0f, 0f);
        public static int traitCount = 0;
        public Pawn examplePawn;
        public override void DoSettingsWindowContents(Rect inRect)
        {
            checked
            {
                Listing_Standard listing_Standard = new Listing_Standard();
                listing_Standard.Begin(inRect);
                listing_Standard.CheckboxLabeled("SyrTraitValue_EnableColors".Translate(), ref TraitValueSettings.enableColors, "SyrTraitValue_EnableColorsTooltip".Translate());
                listing_Standard.CheckboxLabeled("SyrTraitValue_UseBestColor".Translate(), ref TraitValueSettings.useBestColor, "SyrTraitValue_UseBestColorTooltip".Translate());

                Rect colorButton1 = new Rect(inRect.x, listing_Standard.CurHeight, inRect.width * 0.25f, 44f);
                Rect colorButton2 = new Rect(inRect.x + inRect.width * 0.25f, listing_Standard.CurHeight, inRect.width * 0.25f, 44f);
                Rect colorButton3 = new Rect(inRect.x + inRect.width * 0.5f, listing_Standard.CurHeight, inRect.width * 0.25f, 44f);
                Rect colorButton4 = new Rect(inRect.x + inRect.width * 0.75f, listing_Standard.CurHeight, inRect.width * 0.25f, 44f);
                if (ButtonImageLabeled(colorButton1, "SyrTraitValue_BestColor".Translate(), "SyrTraitValue_BestColorTooltip".Translate(), Static_Textures.bestColorButton, 128f, 24f))
                {
                    Find.WindowStack.Add(new Dialog_ColorPicker(TraitValueSettings.bestTraitColor, delegate(Color color) { TraitValueSettings.bestTraitColor = color; Static_Textures.bestColorButton = SolidColorMaterials.NewSolidColorTexture(TraitValueSettings.bestTraitColor); Static_Textures.bestColorButton.Apply(false); } ));
                }
                if (ButtonImageLabeled(colorButton2, "SyrTraitValue_GoodColor".Translate(), "SyrTraitValue_GoodColorTooltip".Translate(), Static_Textures.goodColorButton, 128f, 24f))
                {
                    Find.WindowStack.Add(new Dialog_ColorPicker(TraitValueSettings.goodTraitColor, delegate (Color color) { TraitValueSettings.goodTraitColor = color; Static_Textures.goodColorButton = SolidColorMaterials.NewSolidColorTexture(TraitValueSettings.goodTraitColor); Static_Textures.goodColorButton.Apply(false); }));
                }
                if (ButtonImageLabeled(colorButton3, "SyrTraitValue_NeutralColor".Translate(), "SyrTraitValue_NeutralColorTooltip".Translate(), Static_Textures.neutralColorButton, 128f, 24f))
                {
                    Find.WindowStack.Add(new Dialog_ColorPicker(TraitValueSettings.neutralTraitColor, delegate (Color color) { TraitValueSettings.neutralTraitColor = color; Static_Textures.neutralColorButton = SolidColorMaterials.NewSolidColorTexture(TraitValueSettings.neutralTraitColor); Static_Textures.neutralColorButton.Apply(false); }));
                }
                if (ButtonImageLabeled(colorButton4, "SyrTraitValue_BadColor".Translate(), "SyrTraitValue_BadColorTooltip".Translate(), Static_Textures.badColorButton, 128f, 24f))
                {
                    Find.WindowStack.Add(new Dialog_ColorPicker(TraitValueSettings.badTraitColor, delegate (Color color) { TraitValueSettings.badTraitColor = color; Static_Textures.badColorButton = SolidColorMaterials.NewSolidColorTexture(TraitValueSettings.badTraitColor); Static_Textures.badColorButton.Apply(false); }));
                }

                Rect outRect = inRect;
                outRect.height -= 180f;
                outRect.y += 70f;
                Rect viewRect = inRect.GetInnerRect();
                viewRect.height = 27f * traitCount * 0.34f;
                Rect rowRect1 = new Rect(viewRect.x, viewRect.y, viewRect.width * 0.33f, 24f);
                Rect rowRect2 = new Rect(viewRect.x + viewRect.width * 0.33f, viewRect.y, viewRect.width * 0.33f, 24f);
                Rect rowRect3 = new Rect(viewRect.x + viewRect.width * 0.67f, viewRect.y, viewRect.width * 0.33f, 24f);

                Widgets.BeginScrollView(outRect, ref scrollPosition, viewRect);
                int i = 0;
                int threshold = Mathf.CeilToInt(traitCount * 0.333f);
                Dictionary<TraitDegreeData, string> buffers = new Dictionary<TraitDegreeData, string>();
                //if (examplePawn == null) examplePawn = PawnGenerator.GeneratePawn(DefDatabase<PawnKindDef>.AllDefs.First(pkd => pkd == PawnKindDefOf.Drifter), null);
                foreach (TraitDef t in TraitValueUtility.allTraits)
                {
                    foreach (TraitDegreeData tdd in t.degreeDatas)
                    {
                        i++;
                        buffers.TryGetValue(tdd, out string buffer);
                        Trait exampleTrait = new Trait(t, tdd.degree, true);
                        if (i <= threshold)
                        {
                            TextFieldNumericLabeled(rowRect1, tdd.label, TraitDesc(tdd, t), ref t.GetModExtension<TraitValueExtension>().traitValues.Find(dv => dv.degree == tdd.degree).value, ref buffer, -99, 99);
                            rowRect1.y = rowRect1.yMax + 2f;
                        }
                        else if (i <= threshold * 2)
                        {
                            TextFieldNumericLabeled(rowRect2, tdd.label, TraitDesc(tdd, t), ref t.GetModExtension<TraitValueExtension>().traitValues.Find(dv => dv.degree == tdd.degree).value, ref buffer, -99, 99);
                            rowRect2.y = rowRect2.yMax + 2f;
                        }
                        else
                        {
                            TextFieldNumericLabeled(rowRect3, tdd.label, TraitDesc(tdd, t), ref t.GetModExtension<TraitValueExtension>().traitValues.Find(dv => dv.degree == tdd.degree).value, ref buffer, -99, 99);
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
                    TraitValueSettings.useBestColor = true;
                    TraitValueUtility.LoadSavedValues(true);
                    TraitValueSettings.changedTraitValues.Clear();
                    TraitValueSettings.bestTraitColor = Color.cyan;
                    Static_Textures.bestColorButton = SolidColorMaterials.NewSolidColorTexture(Color.cyan);
                    Static_Textures.bestColorButton.Apply(false);
                    TraitValueSettings.goodTraitColor = Color.green;
                    Static_Textures.goodColorButton = SolidColorMaterials.NewSolidColorTexture(Color.green);
                    Static_Textures.goodColorButton.Apply(false);
                    TraitValueSettings.neutralTraitColor = Color.yellow;
                    Static_Textures.neutralColorButton = SolidColorMaterials.NewSolidColorTexture(Color.yellow);
                    Static_Textures.neutralColorButton.Apply(false);
                    TraitValueSettings.badTraitColor = Color.red;
                    Static_Textures.badColorButton = SolidColorMaterials.NewSolidColorTexture(Color.red);
                    Static_Textures.badColorButton.Apply(false);
                }
                listing_Standard.End();
                settings.Write();
            }
        }

        public bool ButtonImageLabeled(Rect rect, string label, string tooltip, Texture2D tex, float width, float height)
        {
            Widgets.Label(rect, label);
            TooltipHandler.TipRegion(rect, tooltip);
            bool result = Widgets.ButtonImage(new Rect(rect.x, rect.y+20f, width, height), tex, true);
            return result;
        }

        public static void TextFieldNumericLabeled<T>(Rect rect, string label, string tooltip, ref T val, ref string buffer, float min = 0f, float max = 1E+09f) where T : struct
        {
            Rect rectLabel = new Rect(rect.x, rect.y, rect.width * 0.77f, rect.height);
            Rect rectField = new Rect(rect.x + rect.width * 0.8f, rect.y, rect.width * 0.2f, rect.height);
            TextAnchor anchor = Text.Anchor;
            Text.Anchor = TextAnchor.MiddleRight;
            if (val is int value && TraitValueSettings.enableColors)
            {
                Color originalColor = GUI.color;
                Color traitColor;
                TraitValueUtility.ValueColor(value, out traitColor);
                GUI.color = traitColor;
                if (label.Contains("<color=#"))
                {
                    label = label.Remove(label.IndexOf("<color=#"), 15).Replace("</color>", "");
                }
                Widgets.Label(rectLabel, label);
                TooltipHandler.TipRegion(rectLabel, tooltip);
                GUI.color = originalColor;
            }
            else
            {
                if (label.Contains("<color=#"))
                {
                    label = label.Remove(label.IndexOf("<color=#"), 15).Replace("</color>", "");
                }
                Widgets.Label(rectLabel, label);
                TooltipHandler.TipRegion(rectLabel, tooltip);
            }
            Text.Anchor = anchor;
            Widgets.TextFieldNumeric<T>(rectField, ref val, ref buffer, min, max);
        }

        public static string TraitDesc(TraitDegreeData tdd, TraitDef t)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(tdd.description);
            IEnumerable<ThoughtDef> traitThoughts = GetPermaThoughts(tdd, t);
            bool flag = tdd.skillGains.Count > 0;
            bool flag2 = traitThoughts.Any();
            bool flag3 = tdd.statOffsets != null;
            bool flag4 = tdd.statFactors != null;
            if (flag || flag2 || flag3 || flag4)
            {
                stringBuilder.AppendLine();
                stringBuilder.AppendLine();
            }
            if (flag)
            {
                foreach (KeyValuePair<SkillDef, int> keyValuePair in tdd.skillGains)
                {
                    if (keyValuePair.Value != 0)
                    {
                        string value = "    " + keyValuePair.Key.skillLabel.CapitalizeFirst() + ":   " + keyValuePair.Value.ToString("+##;-##");
                        stringBuilder.AppendLine(value);
                    }
                }
            }
            if (flag2)
            {
                foreach (ThoughtDef thoughtDef in traitThoughts)
                {
                    stringBuilder.AppendLine("    " + "PermanentMoodEffect".Translate() + " " + thoughtDef.stages[0].baseMoodEffect.ToStringByStyle(ToStringStyle.Integer, ToStringNumberSense.Offset));
                }
            }
            if (flag3)
            {
                for (int i = 0; i < tdd.statOffsets.Count; i++)
                {
                    StatModifier statModifier = tdd.statOffsets[i];
                    string valueToStringAsOffset = statModifier.ValueToStringAsOffset;
                    string value2 = "    " + statModifier.stat.LabelCap + " " + valueToStringAsOffset;
                    stringBuilder.AppendLine(value2);
                }
            }
            if (flag4)
            {
                for (int j = 0; j < tdd.statFactors.Count; j++)
                {
                    StatModifier statModifier2 = tdd.statFactors[j];
                    string toStringAsFactor = statModifier2.ToStringAsFactor;
                    string value3 = "    " + statModifier2.stat.LabelCap + " " + toStringAsFactor;
                    stringBuilder.AppendLine(value3);
                }
            }
            if (tdd.hungerRateFactor != 1f)
            {
                string hunger = tdd.hungerRateFactor.ToStringByStyle(ToStringStyle.PercentOne, ToStringNumberSense.Factor);
                string value4 = "    " + "HungerRate".Translate() + " " + hunger;
                stringBuilder.AppendLine(value4);
            }
            if (ModsConfig.RoyaltyActive)
            {
                List<MeditationFocusDef> allowedMeditationFocusTypes = tdd.allowedMeditationFocusTypes;
                if (!allowedMeditationFocusTypes.NullOrEmpty<MeditationFocusDef>())
                {
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine("EnablesMeditationFocusType".Translate() + ":\n" + (from f in allowedMeditationFocusTypes
                                                                                                 select f.LabelCap.RawText).ToLineList("  - ", false));
                }
            }
            if (stringBuilder.Length > 0 && stringBuilder[stringBuilder.Length - 1] == '\n')
            {
                if (stringBuilder.Length > 1 && stringBuilder[stringBuilder.Length - 2] == '\r')
                {
                    stringBuilder.Remove(stringBuilder.Length - 2, 2);
                }
                else
                {
                    stringBuilder.Remove(stringBuilder.Length - 1, 1);
                }
            }
            return stringBuilder.ToString();
        }

        private static IEnumerable<ThoughtDef> GetPermaThoughts(TraitDegreeData tdd, TraitDef t)
        {
            TraitDegreeData degree = tdd;
            List<ThoughtDef> allThoughts = DefDatabase<ThoughtDef>.AllDefsListForReading;
            int num;
            for (int i = 0; i < allThoughts.Count; i = num + 1)
            {
                if (allThoughts[i].IsSituational && allThoughts[i].Worker is ThoughtWorker_AlwaysActive && allThoughts[i].requiredTraits != null && allThoughts[i].requiredTraits.Contains(t) && (!allThoughts[i].RequiresSpecificTraitsDegree || allThoughts[i].requiredTraitsDegree == degree.degree))
                {
                    yield return allThoughts[i];
                }
                num = i;
            }
            yield break;
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
        public static bool useBestColor = true;
        public static Color bestTraitColor = Color.cyan;
        public static Color goodTraitColor = Color.green;
        public static Color neutralTraitColor = Color.yellow;
        public static Color badTraitColor = Color.red;
        public static Dictionary<string, int> changedTraitValues = new Dictionary<string, int>();

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<bool>(ref enableColors, "SyrTraitValue_enableColors", true, false);
            Scribe_Values.Look<bool>(ref useBestColor, "SyrTraitValue_useBestColor", true, false);
            Scribe_Values.Look<Color>(ref bestTraitColor, "SyrTraitValue_bestTraitColor", Color.cyan, false);
            Scribe_Values.Look<Color>(ref goodTraitColor, "SyrTraitValue_goodTraitColor", Color.green, false);
            Scribe_Values.Look<Color>(ref neutralTraitColor, "SyrTraitValue_neutralTraitColor", Color.yellow, false);
            Scribe_Values.Look<Color>(ref badTraitColor, "SyrTraitValue_badTraitColor", Color.red, false);
            Scribe_Collections.Look<string, int>(ref changedTraitValues, "SyrTraitValue_changedTraitValues", LookMode.Value, LookMode.Value);
        }
    }
}
