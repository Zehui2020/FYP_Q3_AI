using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Haste")]
public class Haste : BaseAbility
{
    public override void InitAbility()
    {
    }

    public override void OnAbilityUse(BaseStats singleTarget, List<BaseStats> targetList)
    {
        singleTarget.movementSpeedMultiplier.AddModifier(abilityStrength / 100);
    }

    public override void OnAbilityEnd(BaseStats singleTarget, List<BaseStats> targetList)
    {
        singleTarget.movementSpeedMultiplier.RemoveModifier(abilityStrength / 100);
    }
}
