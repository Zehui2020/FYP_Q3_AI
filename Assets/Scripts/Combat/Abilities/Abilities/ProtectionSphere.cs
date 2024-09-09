using System.Collections;
using System.Collections.Generic;
using UnityEditor.Playables;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Protection Sphere")]
public class ProtectionSphere : BaseAbility
{
    public override void OnUseAbility(BaseStats self, BaseStats target)
    {
        // push targets away
        Vector3 force = (target.transform.position - PlayerController.Instance.transform.position).normalized;
        force = new Vector3(force.x, 1, 0);
        target.GetComponent<Rigidbody2D>().AddForce(force * abilityDuration, ForceMode2D.Impulse);
    }
}