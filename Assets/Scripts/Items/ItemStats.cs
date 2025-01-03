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

    public float chestplateDamageModifier;
    public int chestplatePoisonStacks;

    public float transceiverBuffDuration;
    public float transceiverBuffMultiplier;

    public float minPlungeDist;
    public float maxPlungeDist;
    public float minPlungeMultiplier;
    public float maxPlungeMultiplier;

    public float dynamightTotalDamageMultiplier;
    public float dynamightRadius;

    public float gavelThreshold;
    public float gavelDamageMultiplier;
    public int gavelStacks;
    public float gavelCooldown;

    public float bottleRadius;
    public int bottleStacks;

    public int voucherRewardCount;

    public int interestChance;

    public int rebateGold;

    public int nrgBarChance;

    public float dazeGrenadeRadius;

    public float defibrillatorHealDelay;
    public float defibrillatorHealMultiplier;

    public float stakeHealAmount;

    public int fungiHealAmount;

    public int algaeHealingMultiplier;

    public int blackCardChestAmount;

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

        chestplateDamageModifier = 0;
        chestplatePoisonStacks = 0;

        transceiverBuffDuration = 0;
        transceiverBuffMultiplier = 0;

        minPlungeDist = 0;
        maxPlungeDist = 0;
        minPlungeMultiplier = 0;
        maxPlungeMultiplier = 0;

        dynamightTotalDamageMultiplier = 0;
        dynamightRadius = 0;

        gavelThreshold = 0;
        gavelDamageMultiplier = 0;
        gavelStacks = 0;
        gavelCooldown = 0;

        bottleRadius = 0;
        bottleStacks = 0;

        voucherRewardCount = 0;

        interestChance = 0;

        rebateGold = 0;

        nrgBarChance = 0;

        dazeGrenadeRadius = 0;

        defibrillatorHealDelay = 0;
        defibrillatorHealMultiplier = 0;

        stakeHealAmount = 0;

        fungiHealAmount = 0;

        algaeHealingMultiplier = 1;

        blackCardChestAmount = 0;
    }
}