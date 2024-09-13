using UnityEngine;

[CreateAssetMenu(menuName = "Items/GasolineTank")]
public class GasolineTank : Item
{
    [SerializeField] private int gasolineRadius;
    [SerializeField] private float gasolineDamageModifier;
    [SerializeField] private int gasolineBurnStack;

    [SerializeField] private int stackGasolineRadius;
    [SerializeField] private float stakGasolineDamageModifier;
    [SerializeField] private int stackGasolineBurnStack;

    public override void Initialize()
    {
        base.Initialize();
        itemStats.gasolineRadius += gasolineRadius;
        itemStats.gasolineDamageModifier += gasolineDamageModifier;
        itemStats.gasolineBurnStacks += gasolineBurnStack;
    }

    public override void IncrementStack()
    {
        base.IncrementStack();
        itemStats.gasolineRadius += stackGasolineRadius;
        itemStats.gasolineDamageModifier += stakGasolineDamageModifier;
        itemStats.gasolineBurnStacks += stackGasolineBurnStack;
    }
}