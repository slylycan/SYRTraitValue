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
    public static class TraitCounter
    {
        static TraitCounter()
        {
            CountTraits();
        }

        public static void CountTraits()
        {
            int traitsInTotal = 0;
            int traitsWithValue = 0;
            foreach (TraitDef t in DefDatabase<TraitDef>.AllDefs)
            {
                int degrees = t.degreeDatas.Count;
                traitsInTotal += degrees;
                TraitValueExtension modExtension = t.GetModExtension<TraitValueExtension>();
                if (modExtension != null)
                {
                    int traitValues = modExtension.traitValues.Count();
                    traitsWithValue += traitValues;
                    if (degrees != modExtension.traitValues.Count())
                    {
                        Log.Warning("WARNING: Trait " + t.label.ToString() + " has " + degrees + " degrees, but " + traitValues + " values. You should configure one value per trait degree data.");
                    }
                }
            }

            Log.Message("[SYR]Trait Value Analysis || Traits: " + traitsInTotal + " - Traits with an assigned value: " + traitsWithValue);
        }
    }
}
