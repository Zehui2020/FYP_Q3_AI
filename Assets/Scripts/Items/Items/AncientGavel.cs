using UnityEngine;

[CreateAssetMenu(menuName = "Items/AncientGavel")]
public class AncientGavel : Item
{
    [SerializeField] private float gavelThreshold;

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
    }

    public override void IncrementStack()
    {
        base.IncrementStack();
        itemStats.gavelDamageMultiplier += gavelStackDamageMultiplier;
        itemStats.gavelStacks += gavelStackStacks;
    }
}