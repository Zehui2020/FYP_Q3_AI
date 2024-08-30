using UnityEngine;

[CreateAssetMenu(menuName = "Items/HDHUD")]
public class HDHUD : Item
{
    [SerializeField] private int critRateIncrease;
    [SerializeField] private int critDamageIncrease;

    public override void Initialize()
    {
        base.Initialize();
        itemStats.critRate += critRateIncrease;
        itemStats.critDamage += critDamageIncrease;
    }

    public override void IncrementStack()
    {
        base.IncrementStack();
        itemStats.critRate += critRateIncrease;
        itemStats.critDamage += critDamageIncrease;
    }
}