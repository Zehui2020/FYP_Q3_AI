using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AbilityController : MonoBehaviour
{
    [SerializeField] private List<BaseAbility> abilities;

    private List<Coroutine> abilityDurationRoutines = new List<Coroutine> { null, null };
    private int baseAttack, baseHealth;
    private List<float> statChanges = new List<float> { 0, 0 };

    public void InitializeAbilityController()
    {
        baseAttack = GetComponent<PlayerController>().attack;
        baseHealth = GetComponent<PlayerController>().health;
    }

    public void HandleAbility(PlayerController stats, int abilityNo)
    {
        if (abilityDurationRoutines[abilityNo] != null)
            return;

        BaseAbility ability = abilities[abilityNo];

        if (ability == null)
            return;

        // if ability for self
        if (ability.abilityUseType == BaseAbility.AbilityUseType.Self)
        {
            switch (ability.abilityEffectStat)
            {
                case BaseAbility.AbilityEffectStat.attack:
                    statChanges[abilityNo] = ChangeStat(
                        baseAttack,
                        ability.abilityEffectValue,
                        ability.abilityEffectType,
                        ability.abilityEffectValueType
                        );
                    stats.attack += (int)statChanges[abilityNo];
                    break;
                case BaseAbility.AbilityEffectStat.health:
                    statChanges[abilityNo] = ChangeStat(
                        baseHealth,
                        ability.abilityEffectValue,
                        ability.abilityEffectType,
                        ability.abilityEffectValueType
                        );
                    stats.health += (int)statChanges[abilityNo];
                    break;
            }
        }
        if (ability.abilityDuration > 0)
            abilityDurationRoutines[abilityNo] = StartCoroutine(AbilityDurationRoutine(stats, abilityNo, ability));
    }

    public IEnumerator AbilityDurationRoutine(PlayerController stats, int abilityNo, BaseAbility ability)
    {
        yield return new WaitForSeconds(ability.abilityDuration);

        switch (ability.abilityEffectStat)
        {
            case BaseAbility.AbilityEffectStat.attack:
                stats.attack -= (int)ResetStat(abilityNo);
                break;
            case BaseAbility.AbilityEffectStat.health:
                stats.health -= (int)ResetStat(abilityNo);
                break;
        }

        yield return new WaitForSeconds(ability.abilityCooldown);

        abilityDurationRoutines[abilityNo] = null;
    }

    private float ChangeStat(float stat, float change, BaseAbility.AbilityEffectType effectType, BaseAbility.AbilityEffectValueType valueType)
    {
        if (valueType == BaseAbility.AbilityEffectValueType.Flat)
        {
            if (effectType == BaseAbility.AbilityEffectType.Increase)
                return change;
            else if (effectType == BaseAbility.AbilityEffectType.Decrease)
                return -change;
        }
        else if (valueType == BaseAbility.AbilityEffectValueType.Percentage)
        {
            if (effectType == BaseAbility.AbilityEffectType.Increase)
                return stat * change / 100;
            else if (effectType == BaseAbility.AbilityEffectType.Decrease)
                return -stat * change / 100;
        }
        return 0;
    }

    private float ResetStat(int abilityNo)
    {
        return statChanges[abilityNo];
    }
}