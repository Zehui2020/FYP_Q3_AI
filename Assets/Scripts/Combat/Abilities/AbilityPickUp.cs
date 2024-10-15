using DesignPatterns.ObjectPool;
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
            InitPickup(ability, ability.abilityCharges);
    }

    public void InitPickup(BaseAbility newAbility, int charges)
    {
        ability = newAbility;
        spriteRenderer.sprite = ability.spriteIcon;
        this.charges = charges;
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