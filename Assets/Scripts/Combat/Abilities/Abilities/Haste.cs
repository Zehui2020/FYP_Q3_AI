using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Haste")]
public class Haste : BaseAbility
{
    public override void InitAbility()
    {
    }

    public override void OnAbilityUse(BaseStats self, BaseStats target)
    {
        self.movementSpeedMultiplier.AddModifier(abilityStrength / 100);
    }

    public override void OnAbilityEnd(BaseStats self, BaseStats target)
    {
        self.movementSpeedMultiplier.RemoveModifier(abilityStrength / 100);
    }
}
