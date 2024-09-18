using UnityEngine;

[CreateAssetMenu(menuName = "Items/SpikedChestplate")]
public class SpikedChestplate : Item
{
    [SerializeField] private float baseDamageMultiplier;
    [SerializeField] private int basePoisonStack;

    [SerializeField] private float stackDamageMultiplier;
    [SerializeField] private int stackPoisonStack;

    public override void Initialize()
    {
        base.Initialize();
        itemStats.chestplateDamageModifier += baseDamageMultiplier;
        itemStats.chestplatePoisonStacks += basePoisonStack;
    }

    public override void IncrementStack()
    {
        base.IncrementStack();
        itemStats.chestplateDamageModifier += stackDamageMultiplier;
        itemStats.chestplatePoisonStacks += stackPoisonStack;
    }
}