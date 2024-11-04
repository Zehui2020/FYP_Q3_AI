using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Shatter")]
public class Shatter : BaseAbility
{
    public override void InitAbility()
    {
    }

    public override void OnAbilityUse(BaseStats singleTarget, List<BaseStats> targetList)
    {
        for (int i = 0; i < targetList.Count; i++)
        {
            BaseStats target = targetList[i];
            // deal damage
            float damageDealt = GetDamage();

            if (target.isFrozen)
                damageDealt *= 2;

            target.TakeDamage(
                PlayerController.Instance, 
                new BaseStats.Damage(BaseStats.Damage.DamageSource.Shatter, damageDealt),
                isCrit, 
                target.transform.position, 
                damageType
                );

            target.particleVFXManager.OnFrozen();
            AudioManager.Instance.PlayOneShot(Sound.SoundName.Shatter);
        }
    }

    public override void OnAbilityEnd(BaseStats singleTarget, List<BaseStats> targetList)
    {
    }
}
