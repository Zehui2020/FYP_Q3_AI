using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonKnifeProjectile : AbilityProjectile
{
    protected override void OnHit(BaseStats target)
    {
        target.ApplyStatusEffect(StatusEffect.StatusType.Poison);
        base.OnHit(target);
    }
}