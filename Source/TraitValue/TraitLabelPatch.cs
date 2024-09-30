using HarmonyLib;
using RimWorld;

namespace SyrTraitValue;

[HarmonyPatch(typeof(Trait), nameof(Trait.Label), MethodType.Getter)]
public static class TraitLabelPatch
{
    [HarmonyPostfix]
    public static void TraitLabelPostfix(ref string __result)
    {
        if (__result.Contains("<color=#"))
        {
            // ReSharper disable once StringIndexOfIsCultureSpecific.1
            __result = __result.Remove(__result.IndexOf("<color=#"), 15).Replace("</color>", "");
        }
    }
}