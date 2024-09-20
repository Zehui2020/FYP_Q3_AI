using UnityEngine;

[CreateAssetMenu(menuName = "Items/InterestContract")]
public class InterestContract : Item
{
    [SerializeField] private int interestChance;

    public override void Initialize()
    {
        base.Initialize();
        itemStats.interestChance += interestChance;
    }

    public override void IncrementStack()
    {
        base.IncrementStack();
        itemStats.interestChance += interestChance;
    }
}