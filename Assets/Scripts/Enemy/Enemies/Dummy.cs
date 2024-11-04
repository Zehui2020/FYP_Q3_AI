using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy : Enemy
{
    private readonly int HurtAnim = Animator.StringToHash("DummyHit");

    public override bool TakeDamage(BaseStats attacker, Damage damage, bool isCrit, Vector3 closestPoint, DamagePopup.DamageType damageType)
    {
        if (health <= 0)
            return false;

        animator.Play(HurtAnim, 0, 0);
        bool tookDamage = base.TakeDamage(attacker, damage, isCrit, closestPoint, damageType);

        if (health <= 0)
        {
            StartCoroutine(DieRoutine());
        }

        return tookDamage;
    }

    private IEnumerator DieRoutine()
    {
        yield return new WaitForSecondsRealtime(3f);

        Heal(maxHealth);
        uiController.SetCanvasActive(true);
    }
}