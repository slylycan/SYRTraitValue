using HarmonyLib;
using RimWorld;

namespace SyrTraitValue;

[HarmonyPatch(typeof(Trait), nameof(Trait.Label), MethodType.Getter)]
public static class Trait_Label
{
    public static void Postfix(ref string __result)
    {
        if (__result.Contains("<color=#"))
        {
            // ReSharper disable once StringIndexOfIsCultureSpecific.1
            __result = __result.Remove(__result.IndexOf("<color=#"), 15).Replace("</color>", "");
        }
    }
}