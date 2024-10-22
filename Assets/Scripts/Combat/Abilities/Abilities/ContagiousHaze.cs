using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Contagious Haze")]
public class ContagiousHaze : BaseAbility
{
    [SerializeField] LayerMask targetLayer;

    public override void InitAbility()
    {
    }

    public override void OnAbilityUse(BaseStats singleTarget, List<BaseStats> targetList)
    {
        BaseStats target = null;
        for (int i = 0; i < targetList.Count; i++)
        {
            if (target == null)
            {
                target = targetList[i];
                continue;
            }

            if (Vector3.Distance(targetList[i].transform.position, PlayerController.Instance.transform.position) <
                Vector3.Distance(target.transform.position, PlayerController.Instance.transform.position))
            {
                target = targetList[i];
            }
        }

        abilityStats.contagiousHazeTarget = target;
        PlayerController.Instance.transform.position = target.transform.position;

        // deal damage
        float damageDealt = GetDamage();
        target.TakeDamage(
            PlayerController.Instance,
            new BaseStats.Damage(BaseStats.Damage.DamageSource.Shatter, damageDealt),
            isCrit,
            target.transform.position,
            damageType
            );

        PlayerController.Instance.particleVFXManager.OnPoison();
        target.particleVFXManager.OnPoison();

        // spread poison
        if (target.health <= 0)
        {
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

    public override void OnAbilityEnd(BaseStats singleTarget, List<BaseStats> targetList)
    {
        PlayerController.Instance.particleVFXManager.StopPoison();
    }
}
