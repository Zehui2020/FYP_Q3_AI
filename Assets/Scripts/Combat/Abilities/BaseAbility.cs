using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAbility : ShopItemData
{
    public AbilityStats abilityStats;

    public Item.Rarity abilityRarity;

    public bool isConsumable;

    public enum AbilityName
    {
        HealthPotion,
        DivineBlessing,
        ProtectionSphere,
        Quake,
        Haste,
        PoisonKnifes,
        MolotovCocktail,
        FreezingOrb,
        Ravage,
        BloodArts,
        HeatWave,
        StoneSkin,
        Requiem,
        ContagiousHaze,
        Shatter
    }
    public AbilityName abilityName;

    public enum AbilityUseType
    {
        Projectile,
        Self,
        Area
    }
    public AbilityUseType abilityUseType;

    public float abilityRange;
    public float abilityStrength;
    public float abilityDuration;
    public float abilityCooldown;
    public int abilityCharges;
    public int abilityMaxCharges;

    public abstract void OnAbilityUse(BaseStats self, BaseStats target);
    public abstract void OnAbilityEnd(BaseStats self, BaseStats target);
}
