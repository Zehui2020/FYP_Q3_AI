using DesignPatterns.ObjectPool;
using System.Collections;
using UnityEngine;

public class AbilityPickUp : PooledObject, IInteractable
{
    [SerializeField] private SimpleAnimation keycodeUI;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private BaseAbility ability;
    [SerializeField] private bool initAbility;
    private int charges;

    private void Start()
    {
        if (initAbility)
            StartCoroutine(InitRoutine());
    }

    public void InitPickup(BaseAbility newAbility, int charges)
    {
        ability = newAbility;
        spriteRenderer.sprite = ability.spriteIcon;
        this.charges = charges;
    }

    private IEnumerator InitRoutine()
    {
        yield return new WaitForSeconds(0.1f);

        InitPickup(ability, ability.abilityCharges);
    }

    public bool OnInteract()
    {
        if (PlayerController.Instance.abilityController.HandleAbilityPickUp(ability, charges))
            Destroy(gameObject);

        keycodeUI.Hide();

        return true;
    }

    public void OnEnterRange()
    {
        keycodeUI.Show();
    }

    public void OnLeaveRange()
    {
        keycodeUI.Hide();
    }
}