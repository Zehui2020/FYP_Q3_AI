using DesignPatterns.ObjectPool;
using UnityEngine;

public class AbilityPickUp : PooledObject, IInteractable
{
    [SerializeField] private SimpleAnimation keycodeUI;
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