using UnityEngine;

[CreateAssetMenu(menuName = "Items/MetalBat")]
public class MetalBat : Item
{
    [SerializeField] private int staticChance;
    [SerializeField] private int staticStacks;

    [SerializeField] private int stackStaticChance;
    [SerializeField] private int stackStaticStacks;

    public override void Initialize()
    {
        base.Initialize();
        itemStats.metalBatChance += staticChance;
        itemStats.metalBatStacks += staticStacks;
    }

    public override void IncrementStack()
    {
        base.IncrementStack();
        itemStats.metalBatChance += stackStaticChance;
        itemStats.metalBatStacks += stackStaticStacks;
    }
}