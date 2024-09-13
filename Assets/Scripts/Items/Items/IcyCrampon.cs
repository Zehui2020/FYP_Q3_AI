using UnityEngine;

[CreateAssetMenu(menuName = "Items/IcyCrampon")]
public class IcyCrampon : Item
{
    [SerializeField] private int cramponChance;
    [SerializeField] private int cramponFreezeStack;
    [SerializeField] private float cramponTrueDamage;

    [SerializeField] private int stackCramponChance;
    [SerializeField] private int stackCramponFreezeStack;
    [SerializeField] private float stackCramponTrueDamage;

    public override void Initialize()
    {
        base.Initialize();
        itemStats.cramponChance += cramponChance;
        itemStats.cramponFreezeStacks += cramponFreezeStack;
        itemStats.cramponDamageModifier += cramponTrueDamage;
    }

    public override void IncrementStack()
    {
        base.IncrementStack();
        itemStats.cramponChance += stackCramponChance;
        itemStats.cramponFreezeStacks += stackCramponFreezeStack;
        itemStats.cramponDamageModifier += stackCramponTrueDamage;
    }
}