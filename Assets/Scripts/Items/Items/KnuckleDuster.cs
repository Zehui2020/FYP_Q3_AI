using UnityEngine;

[CreateAssetMenu(menuName = "Items/KnuckleDuster")]
public class KnuckleDuster : Item
{
    [SerializeField] private float damageModifier;
    [SerializeField] private float stackDamageModifier;

    public override void Initialize()
    {
        base.Initialize();
        itemStats.knuckleDusterDamageModifier += damageModifier;
    }

    public override void IncrementStack()
    {
        base.IncrementStack();
        itemStats.knuckleDusterDamageModifier += stackDamageModifier;
    }
}