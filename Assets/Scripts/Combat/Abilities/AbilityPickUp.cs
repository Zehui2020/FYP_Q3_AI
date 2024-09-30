using UnityEngine;

public class AbilityPickUp : MonoBehaviour, IInteractable
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

    public bool OnInteract()
    {
        if (PlayerController.Instance.abilityController.HandleAbilityPickUp(ability))
            Destroy(gameObject);

        return true;
    }

    public void OnEnterRange()
    {
    }

    public void OnLeaveRange()
    {
    }
}
