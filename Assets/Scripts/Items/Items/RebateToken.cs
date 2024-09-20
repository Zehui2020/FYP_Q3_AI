using UnityEngine;

[CreateAssetMenu(menuName = "Items/RebateToken")]
public class RebateToken : Item
{
    [SerializeField] private int baseGoldAmount;
    [SerializeField] private int stackGoldAmount;

    public override void Initialize()
    {
        base.Initialize();
        itemStats.rebateGold += baseGoldAmount;
    }

    public override void IncrementStack()
    {
        base.IncrementStack();
        itemStats.rebateGold += stackGoldAmount;
    }
}