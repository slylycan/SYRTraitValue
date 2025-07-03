using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace SyrTraitValue;

public class TraitValueSettings : ModSettings
{
    public Color badTraitColor = Color.red;

    public Color bestTraitColor = Color.cyan;

    public Dictionary<string, int> changedTraitValues = new();
    public bool enableColors = true;

    public Color goodTraitColor = Color.green;

    public Color neutralTraitColor = Color.yellow;
    public bool noNotice;

    public bool useBestColor = true;

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref enableColors, "SyrTraitValue_enableColors", true);
        Scribe_Values.Look(ref useBestColor, "SyrTraitValue_useBestColor", true);
        Scribe_Values.Look(ref noNotice, "SyrTraitValue_noNotice");
        Scribe_Values.Look(ref bestTraitColor, "SyrTraitValue_bestTraitColor", Color.cyan);
        Scribe_Values.Look(ref goodTraitColor, "SyrTraitValue_goodTraitColor", Color.green);
        Scribe_Values.Look(ref neutralTraitColor, "SyrTraitValue_neutralTraitColor", Color.yellow);
        Scribe_Values.Look(ref badTraitColor, "SyrTraitValue_badTraitColor", Color.red);
        Scribe_Collections.Look(ref changedTraitValues, "SyrTraitValue_changedTraitValues", LookMode.Value,
            LookMode.Value);
    }
}