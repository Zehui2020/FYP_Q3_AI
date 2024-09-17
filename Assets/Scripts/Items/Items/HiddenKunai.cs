using UnityEngine;

[CreateAssetMenu(menuName = "Items/HiddenKunai")]
public class HiddenKunai : Item
{
    [SerializeField] private int kunaiChance;
    [SerializeField] private int kunaiDamage;

    [SerializeField] private int stackKunaiChance;
    [SerializeField] private int stackKunaiDamage;

    public override void Initialize()
    {
        base.Initialize();
        itemStats.kunaiChance += kunaiChance;
        itemStats.kunaiDamageMultiplier += kunaiDamage;
    }

    public override void IncrementStack()
    {
        base.IncrementStack();
        itemStats.kunaiChance += stackKunaiChance;
        itemStats.kunaiDamageMultiplier += stackKunaiDamage;
    }
}