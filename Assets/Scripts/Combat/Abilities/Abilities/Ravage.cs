using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Ravage")]
public class Ravage : BaseAbility
{
    public override void OnAbilityUse(BaseStats self, BaseStats target)
    {
        target.ApplyStatusEffect(new StatusEffect.StatusType(StatusEffect.StatusType.Type.Debuff, StatusEffect.StatusType.Status.Static), (int)abilityStrength);
    }

    public override void OnAbilityEnd(BaseStats self, BaseStats target)
    {
    }
}
