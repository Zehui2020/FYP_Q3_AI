using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatController : MonoBehaviour
{
    [SerializeField] private WeaponData wData;

    private AnimationManager animationManager;
    private CombatCollisionController collisionController;

    private int attackComboCount;

    private Coroutine attackRoutine;
    private Coroutine attackAnimRoutine;
    private Coroutine plungeAttackRoutine;

    public void InitializeCombatController()
    {
        animationManager = GetComponent<AnimationManager>();
        collisionController = GetComponent<CombatCollisionController>();
        attackComboCount = 0;
    }
    public void HandleAttack()
    {
        if (attackAnimRoutine == null)
        {
            if (attackRoutine != null)
            {
                attackComboCount++;
                if (attackComboCount >= wData.attackAnimations.Count)
                {
                    attackComboCount = 0;
                }
                StopCoroutine(attackRoutine);
                attackRoutine = StartCoroutine(AttackRoutine());
            }
            else
            {
                attackComboCount = 0;
                attackRoutine = StartCoroutine(AttackRoutine());
            }
        }
    }

    private IEnumerator AttackRoutine()
    {
        animationManager.SetAttackAnimationClip(Animator.StringToHash(wData.attackAnimations[attackComboCount].name));
        animationManager.ChangeAnimation(animationManager.GetAttackAnimation(), 0, 0, true);
        attackAnimRoutine = StartCoroutine(AttackAnimRoutine());

        yield return new WaitForSeconds(wData.attackSpeed + wData.attackAnimations[attackComboCount].length);

        attackRoutine = null;
    }

    private IEnumerator AttackAnimRoutine()
    {
        yield return new WaitForSeconds(wData.attackAnimations[attackComboCount].length);

        attackAnimRoutine = null;
    }

    public bool CheckAttacking()
    {
        if (attackAnimRoutine == null && plungeAttackRoutine == null)
        {
            return false;
        }
        return true;
    }

    public void HandlePlungeAttack()
    {
        if (plungeAttackRoutine == null)
        {
            Debug.Log("Plunge Attack");
            plungeAttackRoutine = StartCoroutine(PlungeAttackRoutine());
        }
    }

    private IEnumerator PlungeAttackRoutine()
    {
        animationManager.SetAttackAnimationClip(Animator.StringToHash(wData.plungeAttackAnimation.name));
        animationManager.ChangeAnimation(animationManager.GetAttackAnimation(), 0, 0, true);

        yield return new WaitForSeconds(wData.plungeAttackAnimation.length);

        plungeAttackRoutine = null;
    }

    public void OnDamageEventStart(int col)
    {
        float damage;

        if (plungeAttackRoutine != null)
            damage = wData.baseAttack * wData.plungeAttackMultiplier;
        else
            damage = wData.baseAttack * wData.attackMultipliers[attackComboCount];

        collisionController.EnableCollider(damage, wData.critRate, wData.critMultiplier, col);
    }

    public void OnDamageEventEnd(int col)
    {
        collisionController.DisableCollider(col);
    }
}