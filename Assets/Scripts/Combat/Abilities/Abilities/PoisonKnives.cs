using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Poison Knives")]
public class PoisonKnives : BaseAbility
{
    [SerializeField] GameObject knifePrefab;

    public override void OnUseAbility(BaseStats self, BaseStats target)
    {
        GameObject obj = Instantiate(knifePrefab);
        obj.transform.position = PlayerController.Instance.transform.position;
        Vector3 force = new Vector3(PlayerController.Instance.transform.localScale.x * abilityEffectValue, 0, 0);
        obj.GetComponent<PoisonKnifeProjectile>().LaunchProjectile(force * abilityRange);
    }
}
