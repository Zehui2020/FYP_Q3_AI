using UnityEngine;

[CreateAssetMenu(menuName = "Items/RitualSickle")]
public class RitualSickle : Item
{
    [SerializeField] private int bleedChance;
    [SerializeField] private int bleedStacks;
    [SerializeField] private int critRateIncrease;

    [SerializeField] private int stackBleedChance;
    [SerializeField] private int stackBleedStacks;

    public override void Initialize()
    {
        base.Initialize();
        PlayerController.Instance.critRate.AddModifier(critRateIncrease);
        itemStats.ritualBleedChance += bleedChance;
        itemStats.ritualBleedStacks += bleedStacks;
    }

    public override void IncrementStack()
    {
        base.IncrementStack();
        itemStats.ritualBleedChance += stackBleedChance;
        itemStats.ritualBleedStacks += stackBleedStacks;
    }
}