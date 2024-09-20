using UnityEngine;

[CreateAssetMenu(menuName = "Items/BloodBag")]
public class BloodBag : Item
{
    [SerializeField] private float healthIncreaseMultiplier;

    public override void Initialize()
    {
        base.Initialize();
        PlayerController.Instance.maxHealth += Mathf.CeilToInt(PlayerController.Instance.maxHealth * healthIncreaseMultiplier);
        PlayerController.Instance.health = PlayerController.Instance.maxHealth;
    }

    public override void IncrementStack()
    {
        base.IncrementStack();
        PlayerController.Instance.maxHealth += Mathf.CeilToInt(PlayerController.Instance.maxHealth * healthIncreaseMultiplier);
        PlayerController.Instance.health = PlayerController.Instance.maxHealth;
    }
}