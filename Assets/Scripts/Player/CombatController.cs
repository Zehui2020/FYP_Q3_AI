using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatController : MonoBehaviour
{
    [SerializeField] private WeaponData wData;

    private AnimationManager animationManager;
    private CombatCollisionController collisionController;
    private BaseStats player;

    private int attackComboCount;
    private bool cancelledPlunge = false;

    private Coroutine attackRoutine;
    private Coroutine attackAnimRoutine;
    private Coroutine attackCooldownRoutine;
    private Coroutine plungeAttackRoutine;

    public void InitializeCombatController(BaseStats baseStats)
    {
        animationManager = GetComponent<AnimationManager>();
        collisionController = GetComponent<CombatCollisionController>();
        player = baseStats;
        attackComboCount = 0;

        collisionController.InitCollisionController(player);
    }

    public void HandleAttack()
    {
        if (attackAnimRoutine != null || attackCooldownRoutine != null)
            return;

        if (attackRoutine != null)
        {
            attackComboCount++;
            if (attackComboCount >= wData.attackAnimations.Count)
            {
                attackComboCount = 0;
                HandleAttackCooldown();

                player.comboDamageMultipler.RemoveModifier(wData.attackMultipliers[wData.attackMultipliers.Count - 1]);
                player.comboDamageMultipler.AddModifier(wData.attackMultipliers[attackComboCount]);
            }
            else
            {
                player.comboDamageMultipler.RemoveModifier(wData.attackMultipliers[attackComboCount - 1]);
                player.comboDamageMultipler.AddModifier(wData.attackMultipliers[attackComboCount]);
            }

            StopCoroutine(attackRoutine);
            attackRoutine = StartCoroutine(AttackRoutine());
        }
        else
        {
            attackComboCount = 0;
            player.comboDamageMultipler.ReplaceAllModifiers(wData.attackMultipliers[attackComboCount]);
            attackRoutine = StartCoroutine(AttackRoutine());
        }
    }

    private IEnumerator AttackRoutine()
    {
        animationManager.SetAttackAnimationClip(Animator.StringToHash(wData.attackAnimations[attackComboCount].name));
        animationManager.ChangeAnimation(animationManager.GetAttackAnimation(), 0, 0, true);
        attackAnimRoutine = StartCoroutine(AttackAnimRoutine());

        yield return new WaitForSeconds(wData.comboCooldown + wData.attackAnimations[attackComboCount].length);

        attackRoutine = null;
    }

    private IEnumerator AttackAnimRoutine()
    {
        yield return new WaitForSeconds(wData.attackAnimations[attackComboCount].length);

        attackAnimRoutine = null;
    }

    public void HandleAttackCooldown()
    {
        if (attackCooldownRoutine == null)
        {
            attackCooldownRoutine = StartCoroutine(AttackCooldownRoutine());
        }
    }

    private IEnumerator AttackCooldownRoutine()
    {
        yield return new WaitForSeconds(wData.attackCooldown + wData.attackAnimations[attackComboCount].length);

        attackCooldownRoutine = null;
    }

    public bool CheckAttacking()
    {
        if (attackAnimRoutine == null && plungeAttackRoutine == null)
        {
            return false;
        }
        return true;
    }

    public bool HandlePlungeAttack()
    {
        if (plungeAttackRoutine == null && !cancelledPlunge)
        {
            player.comboDamageMultipler.ReplaceAllModifiers(wData.plungeAttackMultiplier);
            plungeAttackRoutine = StartCoroutine(PlungeAttackRoutine());
            return true;
        }

        cancelledPlunge = false;

        return false;
    }

    private IEnumerator PlungeAttackRoutine()
    {
        animationManager.SetAttackAnimationClip(Animator.StringToHash(wData.plungeAttackAnimation.name));
        animationManager.ChangeAnimation(animationManager.GetAttackAnimation(), 0, 0, true);

        yield return new WaitForSeconds(wData.plungeAttackAnimation.length);

        plungeAttackRoutine = null;
    }

    public void CancelPlungeAttack()
    {
        cancelledPlunge = true;
    }

    public void OnDamageEventStart(int col)
    {
        collisionController.EnableCollider(col);
    }

    public void OnDamageEventEnd(int col)
    {
        collisionController.DisableCollider(col);
    }
}
