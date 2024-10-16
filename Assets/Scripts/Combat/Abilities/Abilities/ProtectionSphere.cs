using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Protection Sphere")]
public class ProtectionSphere : BaseAbility
{
    public override void InitAbility()
    {
    }

    public override void OnAbilityUse(BaseStats self, BaseStats target)
    {
        // push targets away
        float dir = target.transform.position.x < PlayerController.Instance.transform.position.x ? -1 : 1;
        target.GetComponent<Rigidbody2D>().velocity = new Vector3(dir * abilityStrength, abilityStrength, 0);
        target.particleVFXManager.OnStunned();
    }

    public override void OnAbilityEnd(BaseStats self, BaseStats target)
    {
    }
}