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
    [StaticConstructorOnStartup]
    public static class HarmonyPatches
    {
        static HarmonyPatches()
        {
            var harmony = new Harmony("Syrchalis.Rimworld.TraitValue");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
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

    [HarmonyPatch(typeof(Trait), nameof(Trait.Label), MethodType.Getter)]
    public static class TraitLabelPatch
    {
        [HarmonyPostfix]
        public static void TraitLabelPostfix(ref string __result)
        {
            if (__result.Contains("<color=#"))
            {
                __result = __result.Remove(__result.IndexOf("<color=#"), 15).Replace("</color>", "");
            }
        }
    }
}
