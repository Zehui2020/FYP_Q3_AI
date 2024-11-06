using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Poison Knives")]
public class PoisonKnives : BaseAbility
{
    [SerializeField] GameObject knifePrefab;
    public override void InitAbility()
    {
    }

    public override void OnAbilityUse(BaseStats singleTarget, List<BaseStats> targetList)
    {
        GameObject obj = Instantiate(knifePrefab);
        obj.transform.position = PlayerController.Instance.transform.position;
        obj.transform.localScale = new Vector3(PlayerController.Instance.transform.localScale.x, 1, 1);
        Vector3 force = new Vector3(PlayerController.Instance.transform.localScale.x * abilityRange + PlayerController.Instance.GetComponent<Rigidbody2D>().velocity.x, abilityStrength, 0);
        obj.GetComponent<PoisonKnifeProjectile>().LaunchProjectile(force);
        AudioManager.Instance.PlayOneShot(Sound.SoundName.PoisonKnives);
        AudioManager.Instance.PlayOneShot(Sound.SoundName.ProjectileThrow);
    }

    public override void OnAbilityEnd(BaseStats singleTarget, List<BaseStats> targetList)
    {
    }
}
