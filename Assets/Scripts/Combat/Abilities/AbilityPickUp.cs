using UnityEngine;

public class AbilityPickUp : Interactable
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private BaseAbility ability;

    private void OnEnable()
    {
        InitPickup(ability);
    }

    public void InitPickup(BaseAbility newAbility)
    {
        ability = newAbility;
        spriteRenderer.sprite = ability.abilityIcon;
        //spriteRenderer.material = newAbility.itemOutlineMaterial;
    }

    public override bool OnInteract()
    {
        if (PlayerController.Instance.abilityController.HandleAbilityPickUp(ability))
            Destroy(gameObject);

        return true;
    }
}
