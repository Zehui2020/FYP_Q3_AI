using UnityEngine;

[CreateAssetMenu(menuName = "Items/TatteredVoucher")]
public class TatteredVoucher : Item
{
    [SerializeField] private int voucherRewardCount;

    public override void Initialize()
    {
        base.Initialize();
        itemStats.voucherRewardCount += voucherRewardCount;
    }

    public override void IncrementStack()
    {
        base.IncrementStack();
        itemStats.voucherRewardCount += voucherRewardCount;
    }
}