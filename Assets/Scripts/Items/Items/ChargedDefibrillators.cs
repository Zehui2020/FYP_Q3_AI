using UnityEngine;

[CreateAssetMenu(menuName = "Items/ChargedDefibrillators")]
public class ChargedDefibrillators : Item
{
    [SerializeField] private float healDelay;

    [SerializeField] private float basePercentage;
    [SerializeField] private float stackPercentage;

    public override void Initialize()
    {
        base.Initialize();
        itemStats.defibrillatorHealDelay += healDelay;
        itemStats.defibrillatorHealMultiplier += basePercentage;
    }

    public override void IncrementStack()
    {
        base.IncrementStack();
        itemStats.defibrillatorHealMultiplier += stackPercentage;
    }
}