using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Playables;
using UnityEngine;
using UnityEngine.UI;

public class AbilityController : MonoBehaviour
{
    [SerializeField] public List<BaseAbility> abilities;
    [SerializeField] private List<AbilityUIController> abilityUI;
    [SerializeField] private GameObject abilityUIPrefab;
    [SerializeField] private Transform abilityUIParent;
    [SerializeField] LayerMask targetLayer;
    [SerializeField] LayerMask groundLayer;

    private PlayerController player;
    private List<Coroutine> abilityCooldownRoutines = new List<Coroutine> { null, null };
    private List<int> charges = new List<int>();
    private List<int> maxCharges = new List<int>();

    public void InitializeAbilityController()
    {
        player = GetComponent<PlayerController>();
        for (int i = 0; i < abilities.Count; i++)
        {
            abilityUI[i].SetIcon(abilities[i].abilityIcon);
            charges.Add(abilities[i].abilityCharges);
            maxCharges.Add(abilities[i].abilityMaxCharges);
        }
        InitializeAbility(-1);
    }

    private void AddAbility(BaseAbility newAbility)
    {
        // add ui
        GameObject obj = Instantiate(abilityUIPrefab, abilityUIParent);
        abilityUI.Add(obj.GetComponent<AbilityUIController>());
        abilityUI[abilityUI.Count - 1].SetIcon(newAbility.abilityIcon);
        // add ability
        abilities.Add(newAbility);
        abilityCooldownRoutines.Add(null);
        charges.Add(newAbility.abilityCharges);
        maxCharges.Add(newAbility.abilityMaxCharges);
        // init ability
        InitializeAbility(abilities.Count - 1);
        // debug
        Debug.Log("Added: " + newAbility.abilityName);
        string text = "";
        for (int i = 0; i < abilities.Count; i++)
        {
            text = text + abilities[i].abilityName + ", ";
        }
        Debug.Log("Abilities: " + text);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            AddAbility(abilities[0]);
        }
    }

    private void InitializeAbility(int abilityNo)
    {
        if (abilityNo < 0)
        {
            for (int i = 0; i < abilities.Count; i++)
            {
                if (charges[i] < maxCharges[i])
                    abilityCooldownRoutines[i] = StartCoroutine(AbilityCooldownRoutine(i, abilities[i]));
            }
        }
        else
        {
            if (charges[abilityNo] < maxCharges[abilityNo])
                abilityCooldownRoutines[abilityNo] = StartCoroutine(AbilityCooldownRoutine(abilityNo, abilities[abilityNo]));
        }
    }

    public void HandleAbility(int abilityNo)
    {
        if (charges[abilityNo] <= 0)
            return;

        BaseAbility ability = abilities[abilityNo];

        if (ability == null)
            return;

        // Emergency Transceiver
        player.ApplyTransceiverBuff();

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

        charges[abilityNo]--;
        if (abilityCooldownRoutines[abilityNo] == null)
            abilityCooldownRoutines[abilityNo] = StartCoroutine(AbilityCooldownRoutine(abilityNo, ability));
    }

    public IEnumerator AbilityCooldownRoutine(int abilityNo, BaseAbility ability)
    {
        // track cooldown
        float timer = ability.abilityCooldown;
        while (timer > 0)
        {
            float fill = timer / ability.abilityCooldown;
            abilityUI[abilityNo].SetCooldown(fill);
            timer -= Time.deltaTime;
            yield return null;
        }

        charges[abilityNo]++;
        abilityUI[abilityNo].SetCooldown(0);
        if (charges[abilityNo] < maxCharges[abilityNo])
            abilityCooldownRoutines[abilityNo] = StartCoroutine(AbilityCooldownRoutine(abilityNo, ability));
        else
            abilityCooldownRoutines[abilityNo] = null;
    }

    public void ResetAbilityCooldowns()
    {
        for (int i = 0; i < abilityCooldownRoutines.Count; i++)
        {
            if (abilityCooldownRoutines[i] != null)
                continue;

            StopCoroutine(abilityCooldownRoutines[i]);

            charges[i]++;
            abilityUI[i].SetCooldown(0);
            if (charges[i] < maxCharges[i])
                abilityCooldownRoutines[i] = StartCoroutine(AbilityCooldownRoutine(i, abilities[i]));
            else
                abilityCooldownRoutines[i] = null;
        }
    }

    public void SetAbility(int abilityNo, BaseAbility ability)
    {
        abilities[abilityNo] = ability;
        abilityUI[abilityNo].SetIcon(abilities[abilityNo].abilityIcon);
    }
}
