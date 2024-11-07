using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Requiem")]
public class Requiem : BaseAbility
{
    private bool isInState;

    public override void InitAbility()
    {
        isInState = false;
    }

    public override void OnAbilityUse(BaseStats singleTarget, List<BaseStats> targetList)
    {
        if (!isInState)
        {
            isInState = true;
            // increase atk spd
            PlayerController.Instance.attackSpeedMultiplier.AddModifier(abilityRange / 100);
            // increase bleed chance
            abilityStats.bloodArtsBleedChance += (int)abilityStrength;

            AudioManager.Instance.PlayOneShot(Sound.SoundName.BloodArts);
            AudioManager.Instance.Play(Sound.SoundName.HeartBeat);
        }
        else if (isInState)
        {
            isInState = false;
            // reset mods
            PlayerController.Instance.attackSpeedMultiplier.RemoveModifier(abilityRange / 100);
            abilityStats.bloodArtsBleedChance -= (int)abilityStrength;

            AudioManager.Instance.Stop(Sound.SoundName.HeartBeat);
        }
    }

    public override void OnAbilityEnd(BaseStats singleTarget, List<BaseStats> targetList)
    {
        if (!isInState)
            return;

        // -2% max health
        singleTarget.TakeTrueDamage(new BaseStats.Damage(BaseStats.Damage.DamageSource.BloodArts, singleTarget.maxHealth * 0.02f));

        singleTarget.particleVFXManager.OnBloodLoss();

        OnAbilityLoop(singleTarget, targetList);
    }
}
