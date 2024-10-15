using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Contagious Haze")]
public class ContagiousHaze : BaseAbility
{
    [SerializeField] LayerMask targetLayer;
    public override void InitAbility()
    {
    }

    public override void OnAbilityUse(BaseStats self, BaseStats target)
    {
        if (abilityStats.contagiousHazeTarget != null)
            return;
        abilityStats.contagiousHazeTarget = target;
        // deal damage
        float damageDealt = (abilityStrength / 100) * target.CalculateDamageDealt(target, BaseStats.Damage.DamageSource.ContagiousHaze, out bool isCrit, out DamagePopup.DamageType damageType);
        target.TakeDamage(PlayerController.Instance, new BaseStats.Damage(BaseStats.Damage.DamageSource.ContagiousHaze, damageDealt), isCrit, target.transform.position, damageType);

        self.particleVFXManager.OnPoison();
        target.particleVFXManager.OnPoison();
    }

    public override void OnAbilityEnd(BaseStats self, BaseStats target)
    {
        self.particleVFXManager.StopPoison();
        target.particleVFXManager.StopPoison();

        if (!abilityStats.contagiousHazeHit)
        {
            abilityStats.contagiousHazeTarget = null;
            return;
        }

        // get all target objects in area
        Collider2D[] targetColliders = Physics2D.OverlapCircleAll(abilityStats.contagiousHazeTarget.transform.position, 10, targetLayer);
        abilityStats.contagiousHazeTarget = null;
        int stacksToApply = abilityStats.contagiousHazeStacks;

        foreach (Collider2D col in targetColliders)
        {
            BaseStats targetInArea = col.GetComponent<BaseStats>();
            if (target != null)
            {
                targetInArea.ApplyStatusEffect(new StatusEffect.StatusType(StatusEffect.StatusType.Type.Debuff, StatusEffect.StatusType.Status.Poison), stacksToApply);
                targetInArea.particleVFXManager.OnPoison();
            }
        }

        abilityStats.contagiousHazeHit = false;
        abilityStats.contagiousHazeStacks = 0;
    }
}
