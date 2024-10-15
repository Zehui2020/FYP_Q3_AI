using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Stone Skin")]
public class StoneSkin : BaseAbility
{
    public override void InitAbility()
    {
    }

    public override void OnAbilityUse(BaseStats self, BaseStats target)
    {
        // stop player movement
        PlayerController.Instance.ChangeState(PlayerController.PlayerStates.Ability);
        // immune
        self.ApplyImmune(abilityDuration, BaseStats.ImmuneType.StoneSkin);
    }

    public override void OnAbilityEnd(BaseStats self, BaseStats target)
    {
        PlayerController.Instance.ChangeState(PlayerController.PlayerStates.Movement);
    }
}
