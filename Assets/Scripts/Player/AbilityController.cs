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
    [SerializeField] LayerMask targetLayer;

    private PlayerController player;
    private List<Coroutine> abilityDurationRoutines = new List<Coroutine> { null, null };

    public void InitializeAbilityController()
    {
        player = GetComponent<PlayerController>();
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
            // update stat
            switch (ability.abilityEffectStat)
            {
                case BaseAbility.AbilityEffectStat.attack:
                    StartCoroutine(player.AttackChangeRoutine(
                        ability.abilityEffectValue,
                        ability.abilityEffectType,
                        ability.abilityEffectValueType,
                        ability.abilityDuration));
                    break;
                case BaseAbility.AbilityEffectStat.health:
                    StartCoroutine(player.HealthChangeRoutine(
                        ability.abilityEffectValue,
                        ability.abilityEffectType,
                        ability.abilityEffectValueType,
                        ability.abilityDuration));
                    break;
            }
        }
        // if ability is Area
        else if (ability.abilityUseType == BaseAbility.AbilityUseType.Area)
        {
            // get all target objects in area
            Collider2D[] targetColliders = Physics2D.OverlapCircleAll(transform.position, ability.abilityRange, targetLayer);
            List<BaseStats> targetsInArea = new List<BaseStats>();
            foreach (Collider2D col in targetColliders)
            {
                if (col.GetComponent<BaseStats>() != null)
                    targetsInArea.Add(col.GetComponent<BaseStats>());
            }
            for (int i = 0; i < targetsInArea.Count; i++)
            {
                // update stat
                switch (ability.abilityEffectStat)
                {
                    case BaseAbility.AbilityEffectStat.attack:
                        StartCoroutine(targetsInArea[i].AttackChangeRoutine(
                            ability.abilityEffectValue,
                            ability.abilityEffectType,
                            ability.abilityEffectValueType,
                            ability.abilityDuration));
                        break;
                    case BaseAbility.AbilityEffectStat.health:
                        StartCoroutine(targetsInArea[i].HealthChangeRoutine(
                            ability.abilityEffectValue,
                            ability.abilityEffectType,
                            ability.abilityEffectValueType,
                            ability.abilityDuration));
                        break;
                }
            }
        }
        abilityDurationRoutines[abilityNo] = StartCoroutine(AbilityDurationRoutine(stats, abilityNo, ability));
    }

    public IEnumerator AbilityDurationRoutine(PlayerController stats, int abilityNo, BaseAbility ability)
    {
        // track duration
        float timer = ability.abilityDuration;
        abilityUI[abilityNo].SetDurationText(((int)timer).ToString(), true);
        abilityUI[abilityNo].SetCooldown(1);
        while (timer > 0)
        {
            abilityUI[abilityNo].SetDurationText(((int)timer).ToString(), true);
            timer -= Time.deltaTime;
            yield return null;
        }
        timer = 0;
        abilityUI[abilityNo].SetDurationText(((int)timer).ToString(), false);
        // track cooldown
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

    public void SetAbility(int abilityNo, BaseAbility ability)
    {
        abilities[abilityNo] = ability;
        abilityUI[abilityNo].SetIcon(abilities[abilityNo].abilityIcon);
    }
}
