using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatController : MonoBehaviour
{
    [SerializeField] private WeaponData wData;
    [SerializeField] private Animator weaponEffectAnimator;
    [SerializeField] private float perfectParryDuration;
    [SerializeField] private float perfectParryCooldown;
    [SerializeField] private int parryDmgReduction;

    private AnimationManager animationManager;
    private CombatCollisionController collisionController;
    private BaseStats player;

    private int attackComboCount;
    private bool cancelledPlunge = false;
    private bool isHoldParry = false;
    private bool isInParry = false;
    private bool damageReduced = false;
    private bool canAttack = true;
    public bool isAttacking = false;

    private Coroutine parryRoutine;
    private Coroutine resetComboRoutine;

    public event System.Action OnAttackReset;

    public void InitializeCombatController(BaseStats baseStats)
    {
        animationManager = GetComponent<AnimationManager>();
        collisionController = GetComponent<CombatCollisionController>();
        player = baseStats;
        attackComboCount = 0;

        collisionController.InitCollisionController(player);

        if (wData == null)
            return;

        weaponEffectAnimator.runtimeAnimatorController = wData.effectController;
        player.critRate.AddModifier(wData.critRate);
        player.critDamage.AddModifier(wData.critDamage);
    }

    public void ChangeWeapon(WeaponData newData)
    {
        wData = newData;
        weaponEffectAnimator.runtimeAnimatorController = wData.effectController;
        player.critRate.AddModifier(wData.critRate);
        player.critDamage.AddModifier(wData.critDamage);
    }

    public bool HandleParry()
    {
        if (parryRoutine != null || isHoldParry)
            return false;
        parryRoutine = StartCoroutine(ParryRoutine());
        return true;
    }

    private IEnumerator ParryRoutine()
    {
        isInParry = true;
        isHoldParry = true;
        animationManager.SetAttackAnimationClip(Animator.StringToHash(wData.parryAnimation.name), player.attackSpeedMultiplier.GetTotalModifier());
        animationManager.ChangeAnimation(animationManager.GetAttackAnimation(), 0, 0, true);
        // if player is hit, negate damage
        player.ApplyImmune(perfectParryDuration, BaseStats.ImmuneType.Parry);
        // only check for perfect parry
        yield return new WaitForSeconds(perfectParryDuration);
        OnHoldParry();
        yield return new WaitForSeconds(wData.parryAnimation.length - perfectParryDuration);
        if (!isHoldParry)
            OnReleaseParry();
        // parry cooldown
        isInParry = false;
        yield return new WaitForSeconds(perfectParryCooldown);
        // can parry again
        parryRoutine = null;
    }

    private void OnHoldParry()
    {
        // reduce damage
        damageReduced = true;
        player.damageReduction.AddModifier(parryDmgReduction);
    }
    public void OnReleaseParry()
    {
        if (damageReduced)
        {
            // reset damage reduction
            damageReduced = false;
            player.damageReduction.RemoveModifier(parryDmgReduction);
        }
        isHoldParry = false;
    }

    public void HandleAttack()
    {
        if (!canAttack)
            return;
            
        canAttack = false;

        player.comboDamageMultipler.ReplaceAllModifiers(wData.attackMultipliers[attackComboCount]);
        animationManager.SetAttackAnimationClip(Animator.StringToHash(wData.attackAnimations[attackComboCount].name), player.attackSpeedMultiplier.GetTotalModifier());
        animationManager.ChangeAnimation(animationManager.GetAttackAnimation(), 0, 0, true);

        weaponEffectAnimator.speed = player.attackSpeedMultiplier.GetTotalModifier();
        weaponEffectAnimator.Play(Animator.StringToHash(wData.attackEffectAnimations[attackComboCount].name), -1, 0);

        //player.breachedMultiplier.AddModifier();

        attackComboCount++;
        if (attackComboCount >= wData.attackAnimations.Count)
            attackComboCount = 0;
    }

    public void ResetComboAttack()
    {
        if (resetComboRoutine != null)
            StopCoroutine(resetComboRoutine);
        resetComboRoutine = StartCoroutine(ResetComboRoutine());
    }

    private IEnumerator ResetComboRoutine()
    {
        yield return new WaitForSeconds(wData.comboCooldown);

        attackComboCount = 0;
    }

    public void OnAnimationEnd()
    {
        SetCanAttack(true);
        OnAttackReset?.Invoke();
    }

    public void SetCanAttack(bool can)
    {
        canAttack = can;
    }

    public bool CheckAttacking()
    {
        if (canAttack && !isInParry && !isHoldParry)
        {
            return false;
        }
        return true;
    }

    public bool HandlePlungeAttack()
    {
        if (!cancelledPlunge)
        {
            player.comboDamageMultipler.ReplaceAllModifiers(wData.plungeAttackMultiplier);
            animationManager.SetAttackAnimationClip(Animator.StringToHash(wData.plungeAttackAnimation.name), player.attackSpeedMultiplier.GetTotalModifier());
            animationManager.ChangeAnimation(animationManager.GetAttackAnimation(), 0, 0, true);
            return true;
        }

        cancelledPlunge = false;

        return false;
    }

    public void CancelPlungeAttack()
    {
        cancelledPlunge = true;
        canAttack = true;
    }

    public void OnDamageEventStart(int col)
    {
        collisionController.EnableCollider(col);
    }

    public void OnDamageEventEnd(int col)
    {
        collisionController.DisableCollider(col);
    }

    private void OnDisable()
    {
        OnAttackReset = null;
    }
}
