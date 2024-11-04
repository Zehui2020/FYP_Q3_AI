using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Stone Skin")]
public class StoneSkin : BaseAbility
{
    public override void InitAbility()
    {
    }

    public override void OnAbilityUse(BaseStats singleTarget, List<BaseStats> targetList)
    {
        PlayerController.Instance.abilityController.abilityOverlayAnimator.TriggerOverlayAnim(1, "StoneSkin");
        // stop player movement
        PlayerController.Instance.ChangeState(PlayerController.PlayerStates.Ability);
        // immune
        singleTarget.ApplyImmune(abilityDuration + 0.6f, BaseStats.ImmuneType.StoneSkin);
    }

    public override void OnAbilityEnd(BaseStats singleTarget, List<BaseStats> targetList)
    {
        PlayerController.Instance.abilityController.abilityOverlayAnimator.TriggerOverlayAnim(1, "StoneSkinOff");
    }
}
