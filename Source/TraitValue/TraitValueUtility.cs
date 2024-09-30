using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace SyrTraitValue;

public static class TraitValueUtility
{
    public static int bestTraitValue;

    public static int worstTraitValue;

    public static readonly List<TraitDef> allTraits = [];

    public static readonly List<TraitDegreeData> allTraitDegreeDatas = [];

    public static readonly Dictionary<string, int> originalTraitValues = new Dictionary<string, int>();

    public static void CacheTraitDefs()
    {
        allTraits.Clear();
        allTraitDegreeDatas.Clear();
        foreach (var item in DefDatabase<TraitDef>.AllDefs.Where(td => td?.degreeDatas != null))
        {
            allTraits.Add(item);
            foreach (var degreeData in item.degreeDatas)
            {
                allTraitDegreeDatas.Add(degreeData);
            }
        }

        TraitValueCore.traitCount = allTraitDegreeDatas.Count;
    }

    public static void CacheXMLValues()
    {
        foreach (var allTrait in allTraits)
        {
            var modExtension = allTrait.GetModExtension<TraitValueExtension>();
            if (modExtension?.traitValues == null)
            {
                continue;
            }

            foreach (var traitDegreeData in allTrait.degreeDatas)
            {
                originalTraitValues.Add($"{allTrait.defName}, {traitDegreeData.degree}",
                    modExtension.traitValues.Find(dv => dv.degree == traitDegreeData.degree).value);
            }
        }
    }

    public static void LoadSavedValues(bool reset)
    {
        if (reset)
        {
            foreach (var originalTraitValue in originalTraitValues)
            {
                var key2 = originalTraitValue.Key.Split(',');
                if (key2.Length != 2)
                {
                    Log.Message($"{originalTraitValue.Key} is not correctly formatted.");
                }
                else
                {
                    var modExtension = allTraits.Find(t => t.defName == key2[0]).GetModExtension<TraitValueExtension>();
                    if (modExtension != null)
                    {
                        modExtension.traitValues.Find(tv => tv.degree == ParseHelper.FromString<int>(key2[1])).value =
                            originalTraitValue.Value;
                    }
                }
            }

            return;
        }

        foreach (var changedTraitValue in TraitValueSettings.changedTraitValues)
        {
            var key = changedTraitValue.Key.Split(',');
            var modExtension2 = allTraits.Find(t => t.defName == key[0]).GetModExtension<TraitValueExtension>();
            if (modExtension2 != null)
            {
                modExtension2.traitValues.Find(tv => tv.degree == ParseHelper.FromString<int>(key[1])).value =
                    changedTraitValue.Value;
            }
        }
    }

    public static void ColorTraitLabels()
    {
        foreach (var allTrait in allTraits)
        {
            var modExtension = allTrait.GetModExtension<TraitValueExtension>();
            if (modExtension == null)
            {
                continue;
            }

            foreach (var traitDegreeData in allTrait.degreeDatas)
            {
                foreach (var traitValue in modExtension.traitValues)
                {
                    var value = traitValue.value;
                    if (value > bestTraitValue)
                    {
                        bestTraitValue = value;
                    }

                    if (value < worstTraitValue)
                    {
                        worstTraitValue = value;
                    }
                }

                if (!modExtension.traitValues.NullOrEmpty() && !traitDegreeData.label.Contains("<color=#"))
                {
                    traitDegreeData.label =
                        $"{ValueColor(modExtension.traitValues.Find(dv => dv.degree == traitDegreeData.degree).value,
                            out _)}{traitDegreeData.label.CapitalizeFirst()}</color>";
                }
            }
        }
    }

    public static void UncolorTraitLabels()
    {
        foreach (var allTrait in allTraits)
        {
            foreach (var degreeData in allTrait.degreeDatas)
            {
                var label = degreeData.label;
                if (label.Contains("<color=#"))
                {
                    // ReSharper disable once StringIndexOfIsCultureSpecific.1
                    degreeData.label = label.Remove(label.IndexOf("<color=#"), 15).Replace("</color>", "");
                }
            }
        }
    }

    public static string ValueColor(int value, out Color color)
    {
        var num = bestTraitValue * 0.9f;
        if (value >= num && TraitValueSettings.useBestColor)
        {
            var t = value / (float)bestTraitValue;
            color = Color.Lerp(TraitValueSettings.goodTraitColor, TraitValueSettings.bestTraitColor, t);
            return ColorToRichText(color);
        }

        if (value >= 0)
        {
            var t2 = value / (TraitValueSettings.useBestColor ? num : bestTraitValue);
            color = Color.Lerp(TraitValueSettings.neutralTraitColor, TraitValueSettings.goodTraitColor, t2);
            return ColorToRichText(color);
        }

        var t3 = value / (float)worstTraitValue;
        color = Color.Lerp(TraitValueSettings.neutralTraitColor, TraitValueSettings.badTraitColor, t3);
        return ColorToRichText(color);
    }

    public static string ColorToRichText(Color color)
    {
        return
            $"<color=#{Mathf.RoundToInt(color.r * 255f):X2}{Mathf.RoundToInt(color.g * 255f):X2}{Mathf.RoundToInt(color.b * 255f):X2}>";
    }

    private static string getTraitsModName(TraitDef t)
    {
        return t.modContentPack == null ? "Unknown mod (trait defined with a patch?)" : t.modContentPack.Name;
    }

    public static void CountTraits()
    {
        var num = 0;
        var num2 = 0;
        foreach (var allTrait in allTraits)
        {
            var count = allTrait.degreeDatas.Count;
            num += count;
            var modExtension = allTrait.GetModExtension<TraitValueExtension>();
            if (modExtension != null)
            {
                var num3 = modExtension.traitValues.Count;
                num2 += num3;
                if (count != num3)
                {
                    Log.Warning(
                        $"{getTraitsModName(allTrait)}: TraitDef {allTrait.defName} has {count} degrees, but {num3} values. You should configure one value per trait degree data.");
                    modExtension.traitValues.AddRange(from tdd in allTrait.degreeDatas
                        where modExtension.traitValues.FirstOrDefault(tv => tv.degree == tdd.degree) == null
                        select new DegreeValue(tdd.degree, 0));
                }

                foreach (var traitValue in modExtension.traitValues)
                {
                    var value = traitValue.value;
                    if (value > bestTraitValue)
                    {
                        bestTraitValue = value;
                    }

                    if (value < worstTraitValue)
                    {
                        worstTraitValue = value;
                    }
                }
            }
            else
            {
                ModLister.GetModWithIdentifier(allTrait.modContentPack.PackageId);
                Log.Warning(
                    $"{getTraitsModName(allTrait)}: TraitDef{allTrait.defName} does not have trait values set up for integration with [SYR] Trait Values. You can ask the mod author politely to add integration, but accept if they don't want to.");
                allTrait.modExtensions = (allTrait.modExtensions ?? []).Append(
                    new TraitValueExtension
                    {
                        traitValues = allTrait.degreeDatas.Select(tdd => new DegreeValue(tdd.degree, 0)).ToList()
                    }).ToList();
            }
        }

        Log.Message($"[SYR] Trait Value Framework || Traits: {num} - Traits with an assigned value: {num2}");
    }
}