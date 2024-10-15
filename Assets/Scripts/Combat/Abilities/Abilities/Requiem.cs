using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Requiem")]
public class Requiem : BaseAbility
{
    private bool isInState;

    public override void InitAbility()
    {
        isInState = false;
    }

    public override void OnAbilityUse(BaseStats self, BaseStats target)
    {
        if (!isInState)
        {
            isInState = true;
            // increase atk spd
            PlayerController.Instance.attackSpeedMultiplier.AddModifier(abilityRange / 100);
            // increase bleed chance
            abilityStats.bloodArtsBleedChance += (int)abilityStrength;
        }
        else if (isInState)
        {
            isInState = false;
            // reset mods
            PlayerController.Instance.attackSpeedMultiplier.RemoveModifier(abilityRange / 100);
            abilityStats.bloodArtsBleedChance -= (int)abilityStrength;
        }
    }

    private void OnAbilityLoop(BaseStats self, BaseStats target)
    {
        PlayerController.Instance.abilityController.HandleAbilityDuration(this, self, target);
    }

    public override void OnAbilityEnd(BaseStats self, BaseStats target)
    {
        if (!isInState)
            return;

        // -2% max health
        self.TakeTrueDamage(new BaseStats.Damage(BaseStats.Damage.DamageSource.BloodArts, self.maxHealth * 0.02f));

        self.particleVFXManager.OnBloodLoss();

        OnAbilityLoop(self, target);
    }
}
