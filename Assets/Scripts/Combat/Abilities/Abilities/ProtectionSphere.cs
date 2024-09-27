using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Protection Sphere")]
public class ProtectionSphere : BaseAbility
{
    public override void OnAbilityUse(BaseStats self, BaseStats target)
    {
        // push targets away
        Vector3 force = (target.transform.position - PlayerController.Instance.transform.position).normalized;
        force = new Vector3(force.x, 1, 0);
        target.GetComponent<Rigidbody2D>().AddForce(force * abilityStrength, ForceMode2D.Impulse);
    }

    public override void OnAbilityEnd(BaseStats self, BaseStats target)
    {
    }
}