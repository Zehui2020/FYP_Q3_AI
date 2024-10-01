using DesignPatterns.ObjectPool;
using UnityEngine;

public class AbilityPickUp : Interactable
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private BaseAbility ability;
    private int charges;

    public void InitPickup(BaseAbility newAbility, int charges)
    {
        ability = newAbility;
        spriteRenderer.sprite = ability.abilityIcon;
        this.charges = charges;
    }

    public override bool OnInteract()
    {
        if (PlayerController.Instance.abilityController.HandleAbilityPickUp(ability, charges))
            Destroy(gameObject);

        return true;
    }
}
