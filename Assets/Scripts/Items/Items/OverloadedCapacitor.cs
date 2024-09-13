using UnityEngine;

[CreateAssetMenu(menuName = "Items/OverloadedCapacitor")]
public class OverloadedCapacitor : Item
{
    [SerializeField] private float capacitorDamageModifier;

    [SerializeField] private float stackCapacitorDamageModifier;

    public override void Initialize()
    {
        base.Initialize();
        itemStats.capacitorDamageModifier += capacitorDamageModifier;
    }

    public override void IncrementStack()
    {
        base.IncrementStack();
        itemStats.capacitorDamageModifier += stackCapacitorDamageModifier;
    }
}