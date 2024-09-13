using UnityEngine;

[CreateAssetMenu(menuName = "Items/FrazzledWire")]
public class FrazzledWire : Item
{
    [SerializeField] private int wireChance;
    [SerializeField] private int staticStacks;
    [SerializeField] private int wireRange;

    [SerializeField] private int stackStaticStacks;
    [SerializeField] private int stackWireRange;

    public override void Initialize()
    {
        base.Initialize();
        itemStats.frazzledWireChance = wireChance;
        itemStats.frazzledWireStaticStacks += staticStacks;
        itemStats.frazzledWireRange += wireRange;
    }

    public override void IncrementStack()
    {
        base.IncrementStack();
        itemStats.frazzledWireStaticStacks += staticStacks;
        itemStats.frazzledWireRange += stackWireRange;
    }
}