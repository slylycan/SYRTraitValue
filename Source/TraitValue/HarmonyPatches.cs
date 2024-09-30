using System.Reflection;
using HarmonyLib;
using Verse;

namespace SyrTraitValue;

[StaticConstructorOnStartup]
public static class HarmonyPatches
{
    static HarmonyPatches()
    {
        new Harmony("Syrchalis.Rimworld.TraitValue").PatchAll(Assembly.GetExecutingAssembly());
        TraitValueUtility.CacheTraitDefs();
        TraitValueUtility.CountTraits();
        TraitValueUtility.CacheXMLValues();
        TraitValueUtility.LoadSavedValues(false);
        if (TraitValueSettings.enableColors)
        {
            TraitValueUtility.ColorTraitLabels();
        }
    }
}