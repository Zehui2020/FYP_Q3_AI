using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Ravage")]
public class Ravage : BaseAbility
{
    public override void OnAbilityUse(BaseStats self, BaseStats target)
    {
        target.TakeTrueShieldDamage(new BaseStats.Damage(target.maxShield * 5 / 100));
        target.ApplyStatusEffect(new StatusEffect.StatusType(StatusEffect.StatusType.Type.Debuff, StatusEffect.StatusType.Status.Static), 5);
    }

    public override void OnAbilityEnd(BaseStats self, BaseStats target)
    {
    }
}
