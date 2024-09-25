using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Stone Skin")]
public class StoneSkin : BaseAbility
{
    public override void OnAbilityUse(BaseStats self, BaseStats target)
    {
        // stop player movement

        // immune
        self.ApplyImmune(5, BaseStats.ImmuneType.Block);
    }

    public override void OnAbilityEnd(BaseStats self, BaseStats target)
    {
    }
}
