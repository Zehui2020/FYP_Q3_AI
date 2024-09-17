using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemStats")]
public class ItemStats : ScriptableObject
{
    public int ritualBleedChance;
    public int ritualBleedStacks;

    public float knucleDusterThreshold;
    public float knuckleDusterDamageModifier;

    public int daggerBleedChance;

    public float crudeKnifeDamageModifier;
    public float crudeKnifeDistanceCheck;

    public int frazzledWireChance;
    public int frazzledWireStaticStacks;
    public float frazzledWireTotalDamageModifier;
    public float frazzledWireRange;

    public int cramponChance;
    public int cramponFreezeStacks;
    public float cramponDamageModifier;

    public float gasolineRadius;
    public float gasolineDamageModifier;
    public int gasolineBurnStacks;

    public float capacitorDamageModifier;

    public int metalBatChance;
    public int metalBatStacks;

    public int kunaiChance;
    public int kunaiDamageMultiplier;

    public void ResetStats()
    {
        ritualBleedChance = 0;
        ritualBleedStacks = 0;

        knucleDusterThreshold = 0.9f;
        knuckleDusterDamageModifier = 0;

        daggerBleedChance = 0;

        crudeKnifeDamageModifier = 0;
        crudeKnifeDistanceCheck = 3;

        frazzledWireChance = 0;
        frazzledWireStaticStacks = 0;
        frazzledWireTotalDamageModifier = 0.8f;
        frazzledWireRange = 0;

        cramponChance = 0;
        cramponFreezeStacks = 0;
        cramponDamageModifier = 0;

        gasolineRadius = 0;
        gasolineDamageModifier = 0;
        gasolineBurnStacks = 0;

        capacitorDamageModifier = 0;

        metalBatChance = 0;
        metalBatStacks = 0;

        kunaiChance = 0;
        kunaiDamageMultiplier = 0;
    }
}