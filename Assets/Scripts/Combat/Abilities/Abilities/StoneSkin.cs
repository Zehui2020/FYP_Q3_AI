using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Stone Skin")]
public class StoneSkin : BaseAbility
{
    public override void OnAbilityUse(BaseStats self, BaseStats target)
    {
        // stop player movement
        // immune
        self.ApplyImmune(abilityDuration, BaseStats.ImmuneType.StoneSkin);
    }

    public override void OnAbilityEnd(BaseStats self, BaseStats target)
    {
    }
}
