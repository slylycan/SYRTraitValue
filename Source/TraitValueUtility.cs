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
        public static int GetTraitValue(TraitDef traitDef, int degree)
        {
            TraitValueExtension modExtension = traitDef.GetModExtension<TraitValueExtension>();
            return modExtension.traitValues.Find((DegreeValue dv) => dv.degree == degree).value;
        }
    }
}
