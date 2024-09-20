using UnityEngine;

[CreateAssetMenu(menuName = "Items/Dynamight")]
public class Dynamight : Item
{
    [SerializeField] private float totalDamageModifier;
    [SerializeField] private float explodeRadius;

    [SerializeField] private float stackTotalDamageModifier;
    [SerializeField] private float stackExplodeRadius;

    public override void Initialize()
    {
        base.Initialize();
        itemStats.dynamightTotalDamageMultiplier += totalDamageModifier;
        itemStats.dynamightRadius += explodeRadius;
    }

    public override void IncrementStack()
    {
        base.IncrementStack();
        itemStats.dynamightTotalDamageMultiplier += stackTotalDamageModifier;
        itemStats.dynamightRadius += stackExplodeRadius;
    }
}