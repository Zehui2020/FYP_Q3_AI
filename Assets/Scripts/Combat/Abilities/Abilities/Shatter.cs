using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Shatter")]
public class Shatter : BaseAbility
{
    public override void OnAbilityUse(BaseStats self, BaseStats target)
    {
        // deal damage
        float damageDealt = 
            (abilityStrength / 100) * target.CalculateDamageDealt(target, BaseStats.Damage.DamageSource.Shatter, 
            out bool isCrit, 
            out DamagePopup.DamageType damageType);

        if (target.isFrozen)
            damageDealt *= 2;

        target.TakeDamage(PlayerController.Instance, new BaseStats.Damage(
            BaseStats.Damage.DamageSource.Shatter, damageDealt), 
            isCrit, target.transform.position, damageType);
    }

    public override void OnAbilityEnd(BaseStats self, BaseStats target)
    {
    }
}
