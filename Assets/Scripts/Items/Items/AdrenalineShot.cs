using UnityEngine;

[CreateAssetMenu(menuName = "Items/AdrenalineShot")]
public class AdrenalineShot : Item
{
    [SerializeField] private float attackSpeedIncrease;

    public override void Initialize()
    {
        base.Initialize();
        PlayerController.Instance.attackSpeedMultiplier.AddModifier(attackSpeedIncrease);
    }

    public override void IncrementStack()
    {
        base.IncrementStack();
        PlayerController.Instance.attackSpeedMultiplier.AddModifier(attackSpeedIncrease);
    }
}