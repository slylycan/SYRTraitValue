using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace SyrTraitValue;

public class TraitValueSettings : ModSettings
{
    public static bool enableColors = true;

    public static bool useBestColor = true;

    public static Color bestTraitColor = Color.cyan;

    public static Color goodTraitColor = Color.green;

    public static Color neutralTraitColor = Color.yellow;

    public static Color badTraitColor = Color.red;

    public static Dictionary<string, int> changedTraitValues = new Dictionary<string, int>();

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref enableColors, "SyrTraitValue_enableColors", true);
        Scribe_Values.Look(ref useBestColor, "SyrTraitValue_useBestColor", true);
        Scribe_Values.Look(ref bestTraitColor, "SyrTraitValue_bestTraitColor", Color.cyan);
        Scribe_Values.Look(ref goodTraitColor, "SyrTraitValue_goodTraitColor", Color.green);
        Scribe_Values.Look(ref neutralTraitColor, "SyrTraitValue_neutralTraitColor", Color.yellow);
        Scribe_Values.Look(ref badTraitColor, "SyrTraitValue_badTraitColor", Color.red);
        Scribe_Collections.Look(ref changedTraitValues, "SyrTraitValue_changedTraitValues", LookMode.Value,
            LookMode.Value);
    }
}