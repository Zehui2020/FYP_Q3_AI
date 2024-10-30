using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Rabid Execution")]
public class ContagiousHaze : BaseAbility
{
    [SerializeField] private LayerMask targetLayer;
    private BaseStats target;

    public override void InitAbility()
    {
    }

    public override void OnAbilityUse(BaseStats singleTarget, List<BaseStats> targetList)
    {
        target = targetList[0];
        for (int i = 1; i < targetList.Count; i++)
        {
            if (targetList[i].health <= 0)
                continue;

            if (Vector3.Distance(targetList[i].transform.position, PlayerController.Instance.transform.position) <
                Vector3.Distance(target.transform.position, PlayerController.Instance.transform.position))
            {
                target = targetList[i];
            }
        }

        PlayerController.Instance.transform.position = target.transform.position;
        PlayerController.Instance.particleVFXManager.OnPoison();

        // deal damage
        float damageDealt = GetDamage();
        target.TakeDamage(
            PlayerController.Instance,
            new BaseStats.Damage(BaseStats.Damage.DamageSource.ContagiousHaze, damageDealt),
            isCrit,
            target.transform.position,
            damageType
            );

        // spread poison
        if (target.health <= 0)
        {
            // get all target objects in area
            Collider2D[] targetColliders = Physics2D.OverlapCircleAll(target.transform.position, 10, targetLayer);

            foreach (Collider2D col in targetColliders)
            {
                BaseStats targetInArea = col.GetComponent<BaseStats>();
                if (target != null)
                    targetInArea.ApplyStatusEffect(
                        new StatusEffect.StatusType(
                            StatusEffect.StatusType.Type.Debuff,
                            StatusEffect.StatusType.Status.Poison
                            ),
                        abilityStats.rabidExecutionStacks
                        );
            }

            abilityStats.rabidExecutionStacks = 0;
        }
    }

    public override void OnAbilityEnd(BaseStats singleTarget, List<BaseStats> targetList)
    {
        PlayerController.Instance.particleVFXManager.StopPoison();
    }
}
