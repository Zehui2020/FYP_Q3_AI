using UnityEngine;

[CreateAssetMenu(menuName = "Items/SentientAlgae")]
public class SentientAlgae : Item
{
    [SerializeField] private int healingMutliplier;

    public override void Initialize()
    {
        base.Initialize();
        itemStats.algaeHealingMultiplier *= healingMutliplier;
    }

    public override void IncrementStack()
    {
        base.IncrementStack();
        itemStats.algaeHealingMultiplier *= healingMutliplier;
    }
}