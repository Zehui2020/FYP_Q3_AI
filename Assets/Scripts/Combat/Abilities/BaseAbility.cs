using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAbility : ScriptableObject
{
    public enum AbilityName
    {
        Heal,
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

    public Sprite abilityIcon;

    public enum AbilityUseType
    {
        Projectile,
        Self,
        Area
    }
    public AbilityUseType abilityUseType;

    public float abilityRange;

    public enum AbilityEffectType
    {
        Increase,
        Decrease
    }
    public AbilityEffectType abilityEffectType;

    public enum AbilityEffectValueType
    {
        Flat,
        Percentage
    }
    public AbilityEffectValueType abilityEffectValueType;

    public float abilityEffectValue;

    public float abilityDuration;
    public float abilityCooldown;

    public abstract void OnUseAbility(BaseStats self, BaseStats target);
}
