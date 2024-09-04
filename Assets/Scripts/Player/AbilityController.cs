using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AbilityController : MonoBehaviour
{
    [SerializeField] private List<BaseAbility> abilities;
    [SerializeField] private List<AbilityUIController> abilityUI;

    private List<Coroutine> abilityDurationRoutines = new List<Coroutine> { null, null };
    private int baseAttack, baseHealth;
    private List<float> statChanges = new List<float> { 0, 0 };

    public void InitializeAbilityController()
    {
        baseAttack = GetComponent<PlayerController>().attack;
        baseHealth = GetComponent<PlayerController>().health;
        for (int i = 0; i < abilities.Count; i++)
        {
            abilityUI[i].SetIcon(abilities[i].abilityIcon);
        }
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
                    stats.health = Mathf.Clamp(stats.health, 0, baseHealth);
                    break;
            }
        }
        abilityDurationRoutines[abilityNo] = StartCoroutine(AbilityDurationRoutine(stats, abilityNo, ability));
    }

    public IEnumerator AbilityDurationRoutine(PlayerController stats, int abilityNo, BaseAbility ability)
    {
        float timer = ability.abilityDuration;
        abilityUI[abilityNo].SetDurationText(((int)timer).ToString(), true);
        while (timer > 0)
        {
            abilityUI[abilityNo].SetDurationText(((int)timer).ToString(), true);
            timer -= Time.deltaTime;
            yield return null;
        }
        timer = 0;
        abilityUI[abilityNo].SetDurationText(((int)timer).ToString(), false);

        if (ability.abilityDuration > 0)
            switch (ability.abilityEffectStat)
            {
                case BaseAbility.AbilityEffectStat.attack:
                    stats.attack -= (int)ResetStat(abilityNo);
                    break;
                case BaseAbility.AbilityEffectStat.health:
                    stats.health -= (int)ResetStat(abilityNo);
                    break;
            }

        timer = ability.abilityCooldown;
        while (timer > 0)
        {
            float fill = timer / ability.abilityCooldown;
            abilityUI[abilityNo].SetCooldown(fill);
            timer -= Time.deltaTime;
            yield return null;
        }
        abilityUI[abilityNo].SetCooldown(0);

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

    public void SetAbility(int abilityNo, BaseAbility ability)
    {
        abilities[abilityNo] = ability;
        abilityUI[abilityNo].SetIcon(abilities[abilityNo].abilityIcon);
    }
}
