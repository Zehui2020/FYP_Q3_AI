using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonKnifeProjectile : AbilityProjectile
{
    protected override void OnHit(BaseStats target)
    {
        target.ApplyStatusEffect(new StatusEffect.StatusType(StatusEffect.StatusType.Type.Debuff, StatusEffect.StatusType.Status.Poison), 1);
        base.OnHit(target);
    }
}
