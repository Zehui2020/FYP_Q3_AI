using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetButton : Enemy
{
    [SerializeField] private ComfyBGManager bgManager;

    public override bool TakeDamage(BaseStats attacker, Damage damage, bool isCrit, Vector3 closestPoint, DamagePopup.DamageType damageType)
    {
        bgManager.StartBGGeneration();
        return base.TakeDamage(attacker, damage, isCrit, closestPoint, damageType);
    }
}