using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Quake")]
public class Quake : BaseAbility
{
    public override void OnAbilityUse(BaseStats self, BaseStats target)
    {
        if (target.GetComponent<Rigidbody2D>().velocity.y == 0)
        {
            // deal damage
            float damageDealt = (abilityStrength / 100) * target.CalculateDamageDealt(target,BaseStats.Damage.DamageSource.Normal, out bool isCrit, out DamagePopup.DamageType damageType);
            target.TakeDamage(PlayerController.Instance, new BaseStats.Damage(damageDealt), isCrit, target.transform.position, damageType);
            // push targets away
            Vector3 force = (target.transform.position - PlayerController.Instance.transform.position).normalized;
            force = new Vector3(force.x, 10, 0);
            target.GetComponent<Rigidbody2D>().AddForce(force, ForceMode2D.Impulse);

            target.particleVFXManager.OnStunned();
        }
    }

    public override void OnAbilityEnd(BaseStats self, BaseStats target)
    {
    }
}