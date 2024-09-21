using UnityEngine;

[CreateAssetMenu(menuName = "Items/BloodFungi")]
public class BloodFungi : Item
{
    [SerializeField] private int baseHealingAmount;
    [SerializeField] private int stackHealingAmount;

    public override void Initialize()
    {
        base.Initialize();
        itemStats.fungiHealAmount += baseHealingAmount;
    }

    public override void IncrementStack()
    {
        base.IncrementStack();
        itemStats.fungiHealAmount += stackHealingAmount;
    }
}