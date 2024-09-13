using UnityEngine;

[CreateAssetMenu(menuName = "Items/JaggedDagger")]
public class JaggedDagger : Item
{
    [SerializeField] private int bleedChance;

    public override void Initialize()
    {
        base.Initialize();
        itemStats.daggerBleedChance += bleedChance;
    }

    public override void IncrementStack()
    {
        base.IncrementStack();
        itemStats.daggerBleedChance += bleedChance;
    }
}