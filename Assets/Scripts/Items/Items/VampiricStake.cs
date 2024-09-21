using UnityEngine;

[CreateAssetMenu(menuName = "Items/VampiricStake")]
public class VampiricStake : Item
{
    [SerializeField] private float firstPickupCrit;

    [SerializeField] protected float baseHealMultiplier;
    [SerializeField] protected float stackHealMultiplier;

    public override void Initialize()
    {
        base.Initialize();
        PlayerController.Instance.critRate.AddModifier(firstPickupCrit);
        itemStats.stakeHealAmount += baseHealMultiplier;
    }

    public override void IncrementStack()
    {
        base.IncrementStack();
        itemStats.stakeHealAmount += stackHealMultiplier;
    }
}