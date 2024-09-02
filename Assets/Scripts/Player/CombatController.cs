using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatController : MonoBehaviour
{
    [SerializeField] private WeaponData wData;

    private AnimationManager animationManager;

    private int attackComboCount;

    private Coroutine attackRoutine;
    private Coroutine attackAnimRoutine;

    public void InitializeCombatController()
    {
        animationManager = GetComponent<AnimationManager>();
        attackComboCount = 0;
    }
    public void HandleAttack()
    {
        if (attackAnimRoutine == null)
        {
            if (attackRoutine != null)
            {
                attackComboCount++;
                if (attackComboCount >= wData.animations.Count)
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
        animationManager.SetAttackAnimationClip(Animator.StringToHash(wData.animations[attackComboCount].name));
        animationManager.ChangeAnimation(animationManager.GetAttackAnimation(), 0, 0, true);
        attackAnimRoutine = StartCoroutine(AttackAnimRoutine());

        yield return new WaitForSeconds(wData.attackSpeed + wData.animations[attackComboCount].length);

        attackRoutine = null;
    }

    private IEnumerator AttackAnimRoutine()
    {
        yield return new WaitForSeconds(wData.animations[attackComboCount].length);

        attackAnimRoutine = null;
    }

    public bool CheckAttacking()
    {
        if (attackAnimRoutine == null)
        {
            return false;
        }
        return true;
    }
}
