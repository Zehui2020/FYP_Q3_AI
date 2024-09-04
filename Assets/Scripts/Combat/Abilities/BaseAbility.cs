using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BaseAbility : ScriptableObject
{
    public enum AbilityName
    {
        Rage,
        Heal
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

    public enum AbilityEffectStat
    {
        attack,
        health
    }
    public AbilityEffectStat abilityEffectStat;

    public enum AbilityEffectValueType
    {
        Flat,
        Percentage
    }
    public AbilityEffectValueType abilityEffectValueType;

    public float abilityEffectValue;

    public float abilityDuration;
    public float abilityCooldown;
}
