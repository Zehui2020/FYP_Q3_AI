using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AbilityController : MonoBehaviour
{
    [SerializeField] private List<BaseAbility> abilities;
    [SerializeField] private List<AbilityUIController> abilityUI;
    [SerializeField] LayerMask targetLayer;
    [SerializeField] LayerMask groundLayer;

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

    public void HandleAbility(int abilityNo)
    {
        if (abilityDurationRoutines[abilityNo] != null)
            return;

        BaseAbility ability = abilities[abilityNo];

        if (ability == null)
            return;

        // if ability is Area
        if (ability.abilityUseType == BaseAbility.AbilityUseType.Area)
        {
            // get all target objects in area
            Collider2D[] targetColliders = Physics2D.OverlapCircleAll(transform.position, ability.abilityRange, targetLayer);
            List<BaseStats> targetsInArea = new List<BaseStats>();
            foreach (Collider2D col in targetColliders)
            {
                if (col.GetComponent<BaseStats>() != null)
                {
                    if (!Physics2D.Raycast(
                        player.transform.position,
                        col.transform.position - player.transform.position,
                        Vector3.Distance(player.transform.position, col.transform.position),
                        groundLayer))
                    {
                        targetsInArea.Add(col.GetComponent<BaseStats>());
                    }
                }
            }
            for (int i = 0; i < targetsInArea.Count; i++)
            {
                abilities[abilityNo].OnUseAbility(player, targetsInArea[i]);
            }
        }
        // if ability for self or projectile
        else
        {
            abilities[abilityNo].OnUseAbility(player, player);
        }
        abilityDurationRoutines[abilityNo] = StartCoroutine(AbilityDurationRoutine(abilityNo, ability));
    }

    public IEnumerator AbilityDurationRoutine(int abilityNo, BaseAbility ability)
    {
        // track duration
        float timer = ability.abilityDuration;
        // if ability for self
        if (ability.abilityUseType == BaseAbility.AbilityUseType.Self)
        {
            timer = ability.abilityDuration;
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
        }
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
