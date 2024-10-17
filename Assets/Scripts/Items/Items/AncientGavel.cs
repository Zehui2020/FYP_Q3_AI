using UnityEngine;

[CreateAssetMenu(menuName = "Items/AncientGavel")]
public class AncientGavel : Item
{
    [SerializeField] private float gavelThreshold;
    [SerializeField] private float gavelCooldown;

    [SerializeField] private float gavelDamageMultiplier;
    [SerializeField] private int gavelStacks;

    [SerializeField] private float gavelStackDamageMultiplier;
    [SerializeField] private int gavelStackStacks;

    public override void Initialize()
    {
        base.Initialize();
        itemStats.gavelThreshold += gavelThreshold;
        itemStats.gavelDamageMultiplier += gavelDamageMultiplier;
        itemStats.gavelStacks += gavelStacks;
        itemStats.gavelCooldown += gavelCooldown;
    }

    public override void IncrementStack()
    {
        base.IncrementStack();
        itemStats.gavelDamageMultiplier += gavelStackDamageMultiplier;
        itemStats.gavelStacks += gavelStackStacks;
        itemStats.gavelCooldown += gavelCooldown;
    }
}