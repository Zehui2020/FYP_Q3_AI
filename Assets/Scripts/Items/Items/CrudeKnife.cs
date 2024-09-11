using UnityEngine;

[CreateAssetMenu(menuName = "Items/CrudeKnife")]
public class CrudeKnife : Item
{
    [SerializeField] private float damageModifier;

    public override void Initialize()
    {
        base.Initialize();
        itemStats.crudeKnifeDamageModifier += damageModifier;
    }

    public override void IncrementStack()
    {
        base.IncrementStack();
        itemStats.crudeKnifeDamageModifier += damageModifier;
    }
}