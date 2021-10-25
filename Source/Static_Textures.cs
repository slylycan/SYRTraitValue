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
    public static class Static_Textures
    {
        public static Texture2D bestColorButton;
        public static Texture2D goodColorButton;
        public static Texture2D neutralColorButton;
        public static Texture2D badColorButton;

        static Static_Textures()
        {
            bestColorButton = SolidColorMaterials.NewSolidColorTexture(TraitValueSettings.bestTraitColor);
            goodColorButton = SolidColorMaterials.NewSolidColorTexture(TraitValueSettings.goodTraitColor);
            neutralColorButton = SolidColorMaterials.NewSolidColorTexture(TraitValueSettings.neutralTraitColor);
            badColorButton = SolidColorMaterials.NewSolidColorTexture(TraitValueSettings.badTraitColor);
        }
    }
    
}
