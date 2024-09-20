using UnityEngine;

[CreateAssetMenu(menuName = "Items/NRGBar")]
public class NRGBar : Item
{
    [SerializeField] private int baseChance;
    [SerializeField] private int stackChance;

    public override void Initialize()
    {
        base.Initialize();
        itemStats.nrgBarChance += baseChance;
    }

    public override void IncrementStack()
    {
        base.IncrementStack();
        itemStats.nrgBarChance += stackChance;
    }
}