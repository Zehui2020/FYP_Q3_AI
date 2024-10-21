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
        PlayerController.Instance.abilityController.TriggerOverlayAnim(1, "StoneSkin");
        // immune
        self.ApplyImmune(abilityDuration, BaseStats.ImmuneType.StoneSkin);
        PlayerController.Instance.GetComponent<SpriteRenderer>().enabled = false;
    }

    public override void OnAbilityEnd(BaseStats self, BaseStats target)
    {
        PlayerController.Instance.ChangeState(PlayerController.PlayerStates.Movement);
        PlayerController.Instance.GetComponent<SpriteRenderer>().enabled = true;
        PlayerController.Instance.abilityController.abilityOverlayAnimator.gameObject.SetActive(false);
    }
}
