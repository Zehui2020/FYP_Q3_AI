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
        Shred,
        Haste,
        PoisonKnives,
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

    public virtual void InitAbility()
    {
    }
    public virtual void OnAbilityUse(BaseStats singleTarget, List<BaseStats> targetList)
    {
    }
    public void OnAbilityLoop(BaseStats singleTarget, List<BaseStats> targetList)
    {
        PlayerController.Instance.abilityController.HandleAbilityDuration(this, singleTarget, targetList);
    }
    public virtual void OnAbilityEnd(BaseStats singleTarget, List<BaseStats> targetList)
    {
    }

    protected bool isCrit;
    protected DamagePopup.DamageType damageType;
    public float GetDamage()
    {
        float damage = (abilityStrength / 100) * PlayerController.Instance.attack;

        return damage;
    }
}
