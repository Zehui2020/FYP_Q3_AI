using UnityEngine;

[CreateAssetMenu(menuName = "Items/PocketBarrier")]
public class PocketBarrier : Item
{
    [SerializeField] private float baseDamageReduction;
    [SerializeField] private float stackDamageReduction;

    public override void Initialize()
    {
        base.Initialize();
        PlayerController.Instance.damageReduction.AddModifier(baseDamageReduction);
    }

    public override void IncrementStack()
    {
        base.IncrementStack();
        PlayerController.Instance.damageReduction.AddModifier(stackDamageReduction);
    }
}