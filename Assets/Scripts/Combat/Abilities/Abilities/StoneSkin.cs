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
        // stop player movement
        PlayerController.Instance.ChangeState(PlayerController.PlayerStates.Ability);
        PlayerController.Instance.abilityController.TriggerOverlayAnim(1, "StoneSkin");
        // immune
        singleTarget.ApplyImmune(abilityDuration, BaseStats.ImmuneType.StoneSkin);
        PlayerController.Instance.GetComponent<SpriteRenderer>().enabled = false;
    }

    public override void OnAbilityEnd(BaseStats singleTarget, List<BaseStats> targetList)
    {
        PlayerController.Instance.ChangeState(PlayerController.PlayerStates.Movement);
        PlayerController.Instance.GetComponent<SpriteRenderer>().enabled = true;
        PlayerController.Instance.abilityController.abilityOverlayAnimator.gameObject.SetActive(false);
    }
}
