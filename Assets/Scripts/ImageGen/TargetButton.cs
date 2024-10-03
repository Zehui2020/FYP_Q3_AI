using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TargetButton : Enemy
{
    [SerializeField] private ComfyBGManager bgManager;
    public UnityEvent OnTakeDamage;

    public override bool TakeDamage(BaseStats attacker, Damage damage, bool isCrit, Vector3 closestPoint, DamagePopup.DamageType damageType)
    {
        if (bgManager.StartBGGeneration())
            OnTakeDamage?.Invoke();
        return true;
    }
}