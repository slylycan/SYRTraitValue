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
    public static class TraitValueUtility
    {
        public static void ColorTraits()
        {
            foreach (TraitDef t in DefDatabase<TraitDef>.AllDefs.Where((TraitDef td) => td?.degreeDatas != null))
            {
                TraitValueExtension modExtension = t.GetModExtension<TraitValueExtension>();
                if (modExtension != null)
                {
                    foreach (TraitDegreeData traitDegreeData in t.degreeDatas)
                    {
                        if (!modExtension.traitValues.NullOrEmpty() && !traitDegreeData.label.Contains("<color=#"))
                        {
                            traitDegreeData.label = ValueColor(modExtension.traitValues.FirstOrFallback((DegreeValue dv) => dv.degree == traitDegreeData.degree).value) + traitDegreeData.label.CapitalizeFirst() + "</color>";
                        }
                    }
                }
            }
        }
        public static void UncolorTraits()
        {
            foreach (TraitDef t in DefDatabase<TraitDef>.AllDefs.Where((TraitDef td) => td?.degreeDatas != null))
            {
                foreach (TraitDegreeData traitDegreeData in t.degreeDatas)
                {
                    string label = traitDegreeData.label;
                    if (label.Contains("<color=#"))
                    {
                        traitDegreeData.label = label.Remove(label.IndexOf("<color=#"), 15).Replace("</color>", "");
                    }
                }
            }
        }

        public static string ValueColor(int value)
        {
            if (value >= bestTraitValue * 0.9)
            {
                float t = (float)value / bestTraitValue;
                return ColorToRichText(Color.Lerp(TraitValueSettings.goodTraitColor, TraitValueSettings.bestTraitColor, t));
            }
            if (value >= 0)
            {
                float t = (float)value / (bestTraitValue * 0.9f);
                return ColorToRichText(Color.Lerp(TraitValueSettings.neutralTraitColor, TraitValueSettings.goodTraitColor, t));
            }
            if (value < 0)
            {
                float t = (float)value / worstTraitValue;
                return ColorToRichText(Color.Lerp(TraitValueSettings.neutralTraitColor, TraitValueSettings.badTraitColor, t));
            }
            return "<color=#FFFFFF>";
        }

        public static string ColorToRichText(Color color)
        {
            return $"<color=#{Mathf.RoundToInt(color.r * 255):X2}{Mathf.RoundToInt(color.g * 255):X2}{Mathf.RoundToInt(color.b * 255):X2}>";
        }

        public static void CountTraits()
        {
            int traitsInTotal = 0;
            int traitsWithValue = 0;
            foreach (TraitDef t in DefDatabase<TraitDef>.AllDefs.Where((TraitDef td) => td?.degreeDatas != null))
            {
                int degrees = t.degreeDatas.Count;
                traitsInTotal += degrees;
                TraitValueExtension modExtension = t.GetModExtension<TraitValueExtension>();
                if (modExtension != null)
                {
                    int traitValues = modExtension.traitValues.Count();
                    traitsWithValue += traitValues;
                    if (degrees != traitValues)
                    {
                        Log.Warning("WARNING: Trait " + t.defName.ToString() + " from the mod " + t.modContentPack.Name + " has " + degrees + " degrees, but " + traitValues + " values. You should configure one value per trait degree data.");
                        modExtension.traitValues.AddRange(t.degreeDatas.Where(tdd => modExtension.traitValues.FirstOrDefault(tv => tv.degree == tdd.degree) == null).Select(tdd => new DegreeValue(tdd.degree, 0)));
                    }
                    foreach (DegreeValue degreeValue in modExtension.traitValues)
                    {
                        int currentTraitValue = degreeValue.value;
                        if (currentTraitValue > bestTraitValue) bestTraitValue = currentTraitValue;
                        if (currentTraitValue < worstTraitValue) worstTraitValue = currentTraitValue;
                    }
                }
                else
                {
                    //Adds mod extension, creates a new list if no mod extensions present, then creates an extension and for each degree a DegreeValue
                    Log.Message(t.defName + " from the mod " + t.modContentPack.Name + " does not have trait values set up for integration with [SYR] Trait Values");
                    t.modExtensions = (t.modExtensions ?? new List<DefModExtension>()).Append(new TraitValueExtension { traitValues = t.degreeDatas.Select(tdd => new DegreeValue(tdd.degree, 0)).ToList() }).ToList();
                }
            }

            Log.Message("[SYR] Trait Value Analysis || Traits: " + traitsInTotal + " - Traits with an assigned value: " + traitsWithValue);
        }
        public static int bestTraitValue = 0;
        public static int worstTraitValue = 0;
    }
}
