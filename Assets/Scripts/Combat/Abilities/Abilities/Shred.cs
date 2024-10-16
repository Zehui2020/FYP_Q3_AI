using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Shred")]
public class Shred : BaseAbility
{
    public int damageCount;
    private int count;
    private BaseStats main;

    public override void InitAbility()
    {
        count = damageCount;
    }

    public override void OnAbilityUse(BaseStats self, BaseStats target)
    {
        if (target.GetComponent<Rigidbody2D>().velocity.y == 0)
        {
            count += damageCount;
            // deal damage
            float damageDealt = (abilityStrength / 100) * target.CalculateDamageDealt(target,BaseStats.Damage.DamageSource.Normal, out bool isCrit, out DamagePopup.DamageType damageType);
            target.TakeTrueDamage(new BaseStats.Damage(damageDealt));
            // push targets away
            float dir = target.transform.position.x < PlayerController.Instance.transform.position.x ? -1 : 1;
            target.GetComponent<Rigidbody2D>().velocity = new Vector3(dir * abilityRange, abilityRange, 0);
            target.particleVFXManager.OnStunned();
        }
    }

    private void OnAbilityLoop(BaseStats self, BaseStats target)
    {
        PlayerController.Instance.abilityController.HandleAbilityDuration(this, self, target);
    }

    public override void OnAbilityEnd(BaseStats self, BaseStats target)
    {
        if (count <= 0)
            return;

        // deal damage
        float damageDealt = ((abilityStrength / 10) / 100) * target.CalculateDamageDealt(target, BaseStats.Damage.DamageSource.Normal, out bool isCrit, out DamagePopup.DamageType damageType);
        target.TakeTrueDamage(new BaseStats.Damage(damageDealt));
        // push targets away
        float dir = Random.Range(0, 2) == 0 ? -1 : 1;
        target.GetComponent<Rigidbody2D>().velocity = new Vector3(dir * abilityRange / 2, abilityRange / 2, 0);
        target.particleVFXManager.OnStunned();

        count--;
        OnAbilityLoop(self, target);
    }
}