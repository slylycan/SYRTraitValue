using UnityEngine;
using Verse;

namespace SyrTraitValue;

[StaticConstructorOnStartup]
public static class Static_Textures
{
    public static Texture2D bestColorButton;

    public static Texture2D goodColorButton;

    public static Texture2D neutralColorButton;

    public static Texture2D badColorButton;

    static Static_Textures()
    {
        bestColorButton = SolidColorMaterials.NewSolidColorTexture(TraitValueCore.Instance.Settings.bestTraitColor);
        goodColorButton = SolidColorMaterials.NewSolidColorTexture(TraitValueCore.Instance.Settings.goodTraitColor);
        neutralColorButton =
            SolidColorMaterials.NewSolidColorTexture(TraitValueCore.Instance.Settings.neutralTraitColor);
        badColorButton = SolidColorMaterials.NewSolidColorTexture(TraitValueCore.Instance.Settings.badTraitColor);
    }
}