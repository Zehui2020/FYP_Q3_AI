using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityController : MonoBehaviour
{
    [SerializeField] private ItemPickupAlert itemPickupAlert;
    [SerializeField] public List<BaseAbility> abilities;
    [SerializeField] private List<AbilityUIController> abilityUI;
    [SerializeField] private GameObject abilityUIPrefab;
    [SerializeField] private Transform abilityUIParent;
    [SerializeField] private AbilityPickUp abilityPickUpPrefab;
    [SerializeField] LayerMask targetLayer;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] private int maxAbilitySlots;

    public int currAbilitySlots = 0;
    public bool swappingAbility = false;

    private BaseAbility swapAbility;
    private int swapAbilityCharges;
    private PlayerController player;
    private List<Coroutine> abilityCooldownRoutines = new List<Coroutine> { null, null };
    private List<int> charges = new List<int>();
    private List<int> maxCharges = new List<int>();

    public void InitializeAbilityController()
    {
        player = GetComponent<PlayerController>();
        AddAbilitySlot(2);
    }

    private void AddAbilitySlot(int count)
    {
        for (int i = 0; i < count; i++)
        {
            if (currAbilitySlots >= maxAbilitySlots)
                return;

            // add ui
            GameObject obj = Instantiate(abilityUIPrefab, abilityUIParent);
            abilityUI.Add(obj.GetComponent<AbilityUIController>());
            if (abilityUI.Count == 10)
                abilityUI[abilityUI.Count - 1].InitAbilityUI("[ 0 ]");
            else
                abilityUI[abilityUI.Count - 1].InitAbilityUI("[ " + abilityUI.Count.ToString() + " ]");
            currAbilitySlots++;
        }
    }

    public bool HandleAbilityPickUp(BaseAbility newAbility, int charges)
    {
        itemPickupAlert.DisplayAlert(newAbility);

        if (abilities.Count >= currAbilitySlots)
        {
            swappingAbility = true;
            swapAbility = newAbility;
            swapAbilityCharges = charges;
            return true;
        }

        // add ability
        abilities.Add(newAbility);
        abilityCooldownRoutines.Add(null);
        this.charges.Add(charges);
        maxCharges.Add(newAbility.abilityMaxCharges);
        if (abilityUI.Count == 10)
            abilityUI[abilities.Count - 1].InitAbilityUI(newAbility, "[ 0 ]");
        else
            abilityUI[abilities.Count - 1].InitAbilityUI(newAbility, "[ " + abilities.Count.ToString() + " ]");
        // init ability
        InitializeAbility(abilities.Count - 1);
        return true;
    }

    public void SwapAbility()
    {
        // throw back out ability
        AbilityPickUp ability = Instantiate(abilityPickUpPrefab);
        ability.transform.position = transform.position;
        ability.InitPickup(swapAbility, swapAbility.abilityCharges);

        swappingAbility = false;
        swapAbility = null;
    }

    public void SwapAbility(int i)
    {
        // throw out old ability
        AbilityPickUp ability = Instantiate(abilityPickUpPrefab);
        ability.transform.position = transform.position;
        ability.InitPickup(abilities[i], charges[i]);
        // add ability
        abilities[i] = swapAbility;
        charges[i] = swapAbilityCharges;
        maxCharges[i] = swapAbility.abilityMaxCharges;
        abilityUI[i].InitAbilityUI(swapAbility, "[ " + (i + 1).ToString() + " ]");

        if (abilityCooldownRoutines[i] != null)
        {
            StopCoroutine(abilityCooldownRoutines[i]);
            abilityCooldownRoutines[i] = null;
            abilityUI[i].SetCooldown(0, charges[i]);
        }
        // init ability
        InitializeAbility(i);

        swappingAbility = false;
        swapAbility = null;
    }

    private void RemoveAbility(int i)
    {
        // remove ability
        abilities.RemoveAt(i);
        charges.RemoveAt(i);
        maxCharges.RemoveAt(i);
        int count = 0;
        // re-initialize all abilities
        for (int j = 0; j < abilityUI.Count; j++)
        {
            if (j < abilities.Count)
                abilityUI[j].InitAbilityUI(abilities[j], "[ " + (j + 1).ToString() + " ]");
            else
                abilityUI[j].InitAbilityUI("[ " + (j + 1).ToString() + " ]");

            if (abilityCooldownRoutines[i] != null)
            {
                StopCoroutine(abilityCooldownRoutines[i]);
                abilityCooldownRoutines[i] = null;
                abilityUI[i].SetCooldown(0, charges[i]);
            }

            if (count > 10)
            {
                Debug.Log("Shit Broke :)");
                break;
            }
            count++;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
            AddAbilitySlot(1);
    }

    public void SpawnAbilityPickUp(BaseAbility newAbility, Transform chest)
    {
        AbilityPickUp ability = Instantiate(abilityPickUpPrefab);
        ability.transform.position = chest.position;
        ability.InitPickup(newAbility, newAbility.abilityCharges);
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
            if (targetsInArea.Count == 0)
                return;
            for (int i = 0; i < targetsInArea.Count; i++)
            {
                abilities[abilityNo].OnAbilityUse(player, targetsInArea[i]);
                StartCoroutine(AbilityDurationRoutine(ability, player, targetsInArea[i]));
            }
        }
        // if ability for self or projectile
        else
        {
            abilities[abilityNo].OnAbilityUse(player, player);
            StartCoroutine(AbilityDurationRoutine(ability, player, player));
        }

        charges[abilityNo]--;

        if (ability.isConsumable && charges[abilityNo] == 0)
        {
            RemoveAbility(abilityNo);
            return;
        }

        if (abilityCooldownRoutines[abilityNo] == null)
            abilityCooldownRoutines[abilityNo] = StartCoroutine(AbilityCooldownRoutine(abilityNo, ability));
    }

    private IEnumerator AbilityDurationRoutine(BaseAbility ability, BaseStats self, BaseStats target)
    {
        yield return new WaitForSeconds(ability.abilityDuration);

        ability.OnAbilityEnd(self, target);
    }

    public void HandleAbilityDuration(BaseAbility ability, BaseStats self, BaseStats target)
    {
        StartCoroutine(AbilityDurationRoutine(ability, self, target));
    }

    private IEnumerator AbilityCooldownRoutine(int abilityNo, BaseAbility ability)
    {
        // track cooldown
        float timer = ability.abilityCooldown;
        while (timer > 0)
        {
            float fill = timer / ability.abilityCooldown;
            abilityUI[abilityNo].SetCooldown(fill, charges[abilityNo]);
            timer -= Time.deltaTime;
            yield return null;
        }

        if (ability.isConsumable)
        {
            abilityUI[abilityNo].SetCooldown(0, charges[abilityNo]);
            abilityCooldownRoutines[abilityNo] = null;
        }
        else
        {
            charges[abilityNo]++;
            abilityUI[abilityNo].SetCooldown(0, charges[abilityNo]);
            if (charges[abilityNo] < maxCharges[abilityNo])
                abilityCooldownRoutines[abilityNo] = StartCoroutine(AbilityCooldownRoutine(abilityNo, ability));
            else
                abilityCooldownRoutines[abilityNo] = null;
        }
    }

    public void ResetAbilityCooldowns()
    {
        for (int i = 0; i < abilityCooldownRoutines.Count; i++)
        {
            if (abilityCooldownRoutines[i] == null)
                continue;

            StopCoroutine(abilityCooldownRoutines[i]);

            charges[i]++;
            abilityUI[i].SetCooldown(0, charges[i]);
            if (charges[i] < maxCharges[i])
                abilityCooldownRoutines[i] = StartCoroutine(AbilityCooldownRoutine(i, abilities[i]));
            else
                abilityCooldownRoutines[i] = null;
        }
    }

    public void AddAbilityMaxCharges(int amt)
    {
        for(int i = 0; i < maxCharges.Count; i++)
        {
            maxCharges[i] += amt;
            if (charges[i] < maxCharges[i])
                abilityCooldownRoutines[i] = StartCoroutine(AbilityCooldownRoutine(i, abilities[i]));
        }
    }

    public void AddAbilityMaxCharges(int amt, int abilityNo)
    {
        if (abilities[abilityNo] == null)
            return;

        maxCharges[abilityNo] += amt;
        if (charges[abilityNo] < maxCharges[abilityNo])
            abilityCooldownRoutines[abilityNo] = StartCoroutine(AbilityCooldownRoutine(abilityNo, abilities[abilityNo]));
    }

    public void AddAbilityMaxCharges(int amt, int numOfAbilities, bool isRandom)
    {
        for (int i = 0; i < numOfAbilities; i++)
        {
            int randomIndex = i;

            if (isRandom)
                randomIndex = Random.Range(0, abilities.Count);

            if (abilities[randomIndex] == null)
                return;

            maxCharges[randomIndex] += amt;
            if (charges[randomIndex] < maxCharges[randomIndex])
                abilityCooldownRoutines[randomIndex] = StartCoroutine(AbilityCooldownRoutine(randomIndex, abilities[randomIndex]));
        }
    }
}
