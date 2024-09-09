using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Heal")]
public class Heal : BaseAbility
{
    public override void OnUseAbility(BaseStats target)
    {
        target.OnHealthChange(abilityEffectValue, abilityEffectType, abilityEffectValueType, abilityDuration);
    }
}
