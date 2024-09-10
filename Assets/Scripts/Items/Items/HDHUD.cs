using UnityEngine;

[CreateAssetMenu(menuName = "Items/HDHUD")]
public class HDHUD : Item
{
    [SerializeField] private float critRateIncrease;
    [SerializeField] private float critDamageIncrease;

    public override void Initialize()
    {
        base.Initialize();
        PlayerController.Instance.critRate.AddModifier(critRateIncrease);
        PlayerController.Instance.critDamage.AddModifier(critDamageIncrease);
    }

    public override void IncrementStack()
    {
        base.IncrementStack();
        PlayerController.Instance.critRate.AddModifier(critRateIncrease);
        PlayerController.Instance.critDamage.AddModifier(critDamageIncrease);
    }
}