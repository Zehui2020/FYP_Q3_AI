using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonKnifeProjectile : AbilityProjectile
{
    private int hitCount = 3;

    protected override void OnHit(BaseStats target)
    {
        target.ApplyStatusEffect(new StatusEffect.StatusType(StatusEffect.StatusType.Type.Debuff, StatusEffect.StatusType.Status.Poison), 1);
        hitCount--;
        if (hitCount == 0)
        {
            base.OnHit(target);
        }
    }
}
