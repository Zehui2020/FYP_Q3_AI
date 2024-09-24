using DesignPatterns.ObjectPool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatController : MonoBehaviour
{
    [SerializeField] private WeaponData wData;
    [SerializeField] private ItemStats itemStats;
    [SerializeField] private Animator weaponEffectAnimator;
    [SerializeField] private float perfectParryCooldown;

    private AnimationManager animationManager;
    private CombatCollisionController collisionController;
    private BaseStats player;

    private int attackComboCount;
    private bool isInParry = false;
    private bool canAttack = true;
    public bool isAttacking = false;

    private Coroutine parryCooldownRoutine;
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

    public void GiveParryImmune()
    {
        player.ApplyImmune(wData.parryAnimation.length, BaseStats.ImmuneType.Parry);
    }

    public bool HandleParry()
    {
        if (isInParry || parryCooldownRoutine != null)
            return false;

        isInParry = true;
        animationManager.SetAttackAnimationClip(Animator.StringToHash(wData.parryAnimation.name), player.attackSpeedMultiplier.GetTotalModifier());
        animationManager.ChangeAnimation(animationManager.GetAttackAnimation(), 0, 0, AnimationManager.AnimType.ResetIfSame);

        return true;
    }

    private IEnumerator ParryCooldownRoutine()
    {
        yield return new WaitForSeconds(perfectParryCooldown);
        parryCooldownRoutine = null;
    }

    public void OnParryEnd()
    {
        player.RemoveImmune();
        isInParry = false;
        OnAnimationEnd();
        parryCooldownRoutine = StartCoroutine(ParryCooldownRoutine());
    }

    public void HandleAttack()
    {
        if (!canAttack)
            return;
            
        canAttack = false;

        player.comboDamageMultipler.ReplaceAllModifiers(wData.attackMultipliers[attackComboCount]);
        animationManager.SetAttackAnimationClip(Animator.StringToHash(wData.attackAnimations[attackComboCount].name), player.attackSpeedMultiplier.GetTotalModifier());
        animationManager.ChangeAnimation(animationManager.GetAttackAnimation(), 0, 0, AnimationManager.AnimType.ResetIfSame);

        weaponEffectAnimator.speed = player.attackSpeedMultiplier.GetTotalModifier();
        weaponEffectAnimator.Play(Animator.StringToHash(wData.attackEffectAnimations[attackComboCount].name), -1, 0);

        if (attackComboCount - 1 >= 0)
            player.breachDamageMultiplier.RemoveModifier(wData.breachMultipliers[attackComboCount - 1]);
        else
            player.breachDamageMultiplier.RemoveModifier(wData.breachMultipliers[wData.breachMultipliers.Count - 1]);

        player.breachDamageMultiplier.AddModifier(wData.breachMultipliers[attackComboCount]);

        attackComboCount++;
        if (attackComboCount >= wData.attackAnimations.Count)
            attackComboCount = 0;

        // Hidden Kunai
        int randNum = Random.Range(0, 100);
        if (randNum < itemStats.kunaiChance)
        {
            Kunai kunai = ObjectPool.Instance.GetPooledObject("Kunai", false) as Kunai;
            kunai.transform.position = transform.position;
            kunai.SetupKunai(player, transform.localScale.x < 0 ? Vector2.left : Vector2.right);
        }
    }

    public void ResetComboInstantly()
    {
        attackComboCount = 0;
        canAttack = true;
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
        if (canAttack && !isInParry)
        {
            return false;
        }
        return true;
    }

    public void OnPlungeStart()
    {
        animationManager.SetAttackAnimationClip(Animator.StringToHash(wData.plungeAttackAnimation.name), player.attackSpeedMultiplier.GetTotalModifier());
        animationManager.ChangeAnimation(animationManager.GetAttackAnimation(), 0, 0, AnimationManager.AnimType.ResetIfSame);

        weaponEffectAnimator.Play(Animator.StringToHash(wData.plungeEffectAnimation.name), -1, 0);
    }

    public void HandlePlungeAttack()
    {
        player.comboDamageMultipler.ReplaceAllModifiers(wData.plungeAttackMultiplier);
        animationManager.SetAttackAnimationClip(Animator.StringToHash(wData.plungeSlamAnimation.name), player.attackSpeedMultiplier.GetTotalModifier());
        animationManager.ChangeAnimation(animationManager.GetAttackAnimation(), 0, 0, AnimationManager.AnimType.ResetIfSame);

        weaponEffectAnimator.Play(Animator.StringToHash(wData.plungeSlamEffectAnimation.name), -1, 0);
    }

    public void CancelPlunge()
    {
        weaponEffectAnimator.SetTrigger("cancelPlunge");
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
