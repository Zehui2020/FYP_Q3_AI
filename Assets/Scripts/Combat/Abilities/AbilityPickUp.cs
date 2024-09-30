using DesignPatterns.ObjectPool;
using UnityEngine;

public class AbilityPickUp : PooledObject, IInteractable
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
    }

    public bool OnInteract()
    {
        if (PlayerController.Instance.abilityController.HandleAbilityPickUp(ability))
        {
            Release();
            gameObject.SetActive(false);
        }

        return true;
    }

    public void OnEnterRange()
    {
    }

    public void OnLeaveRange()
    {
    }
}
