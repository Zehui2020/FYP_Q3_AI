using UnityEngine;

[CreateAssetMenu(menuName = "Items/BlackCard")]
public class BlackCard : Item
{
    [SerializeField] private int baseChestAmount;
    [SerializeField] private int stackChestAmount;

    public override void Initialize()
    {
        base.Initialize();
        itemStats.blackCardChestAmount += baseChestAmount;
    }

    public override void IncrementStack()
    {
        base.IncrementStack();
        itemStats.blackCardChestAmount += stackChestAmount;
    }
}