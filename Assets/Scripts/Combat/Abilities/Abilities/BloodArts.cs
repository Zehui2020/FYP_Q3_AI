using UnityEngine;


[CreateAssetMenu(menuName = "Abilities/Blood Arts")]
public class BloodArts : BaseAbility
{
    public override void InitAbility()
    {
    }

    public override void OnAbilityUse(BaseStats self, BaseStats target)
    {
        // -50% health
        self.TakeTrueDamage(new BaseStats.Damage(BaseStats.Damage.DamageSource.BloodArts, self.health * 0.5f));
        // 25% bleed chance
        abilityStats.bloodArtsBleedChance += 25;
        // 50% life steal from attacks
        abilityStats.bloodArtsLifestealMultiplier += 0.5f;

        self.particleVFXManager.OnBloodLoss();
    }

    public override void OnAbilityEnd(BaseStats self, BaseStats target)
    {
        abilityStats.bloodArtsBleedChance -= 25;
        abilityStats.bloodArtsLifestealMultiplier -= 0.5f;
    }
}
