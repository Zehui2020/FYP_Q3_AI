using UnityEngine;

[CreateAssetMenu(menuName = "Items/BottleOfSurprises")]
public class BottleOfSurprises : Item
{
    [SerializeField] private float bottleRadius;

    [SerializeField] private int inflictStacks;
    [SerializeField] private int stackInflictStacks;

    public override void Initialize()
    {
        base.Initialize();
        itemStats.bottleRadius = bottleRadius;
        itemStats.bottleStacks += inflictStacks;
    }

    public override void IncrementStack()
    {
        base.IncrementStack();
        itemStats.bottleStacks += stackInflictStacks;
    }
}