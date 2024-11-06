using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Abilities/Blood Arts")]
public class BloodArts : BaseAbility
{
    public override void InitAbility()
    {
    }

    public override void OnAbilityUse(BaseStats singleTarget, List<BaseStats> targetList)
    {
        // -50% health
        singleTarget.TakeTrueDamage(new BaseStats.Damage(BaseStats.Damage.DamageSource.BloodArts, singleTarget.health * 0.5f));
        // 25% bleed chance
        abilityStats.bloodArtsBleedChance += 25;
        // 50% life steal from attacks
        abilityStats.bloodArtsLifestealMultiplier += 0.5f;

        singleTarget.particleVFXManager.OnBloodLoss();
        AudioManager.Instance.PlayOneShot(Sound.SoundName.BloodArts);
    }

    public override void OnAbilityEnd(BaseStats singleTarget, List<BaseStats> targetList)
    {
        abilityStats.bloodArtsBleedChance -= 25;
        abilityStats.bloodArtsLifestealMultiplier -= 0.5f;
    }
}
