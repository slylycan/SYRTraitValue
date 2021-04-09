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
            TraitValueUtility.CountTraits();
            if (TraitValueSettings.enableColors)
            {
                TraitValueUtility.ColorTraits();
            }
        }
    }
}
