using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Freezing Orb")]
public class FreezingOrb : BaseAbility
{
    [SerializeField] GameObject nadePrefab;
    public override void InitAbility()
    {
    }

    public override void OnAbilityUse(BaseStats singleTarget, List<BaseStats> targetList)
    {
        GameObject obj = Instantiate(nadePrefab);
        obj.transform.position = PlayerController.Instance.transform.position;
        Vector3 force = new Vector3(PlayerController.Instance.transform.localScale.x * abilityRange + PlayerController.Instance.GetComponent<Rigidbody2D>().velocity.x, abilityStrength, 0);
        obj.GetComponent<FreezingOrbProjectile>().LaunchProjectile(force);
        AudioManager.Instance.PlayOneShot(Sound.SoundName.ProjectileThrow);
    }

    public override void OnAbilityEnd(BaseStats singleTarget, List<BaseStats> targetList)
    {
    }
}