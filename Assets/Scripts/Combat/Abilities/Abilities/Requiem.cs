using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Requiem")]
public class Requiem : BaseAbility
{
    private bool isInState = false;

    public override void OnAbilityUse(BaseStats self, BaseStats target)
    {
        if (!isInState)
        {
            isInState = true;
            // increase atk spd
            PlayerController.Instance.attackSpeedMultiplier.AddModifier(0.5f);
            // increase bleed chance
            abilityStats.bloodArtsBleedChance += 25;

            self.particleVFXManager.OnBleeding();
        }
        else if (isInState)
        {
            isInState = false;
            // reset mods
            PlayerController.Instance.attackSpeedMultiplier.RemoveModifier(0.5f);
            abilityStats.bloodArtsBleedChance -= 25;

            self.particleVFXManager.StopBleeding();
        }
    }

    private void OnAbilityLoop(BaseStats self, BaseStats target)
    {
        PlayerController.Instance.abilityController.HandleAbilityDuration(this, self, target);
    }

    public override void OnAbilityEnd(BaseStats self, BaseStats target)
    {
        // -2% max health
        self.TakeTrueDamage(new BaseStats.Damage(BaseStats.Damage.DamageSource.BloodArts, self.maxHealth * 0.02f));

        if (isInState)
            OnAbilityLoop(self, target);
    }
}
