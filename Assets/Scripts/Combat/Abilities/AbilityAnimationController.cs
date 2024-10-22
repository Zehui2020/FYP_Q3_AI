using UnityEditor.ShaderGraph.Drawing;
using UnityEngine;

public class AbilityAnimationController : MonoBehaviour
{
    [SerializeField] public Animator abilityOverlayAnimator;

    public void TriggerOverlayAnim(float alpha, string trigger)
    {
        abilityOverlayAnimator.SetTrigger(trigger);
        abilityOverlayAnimator.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, alpha);
    }

    public void ShowAbilityOverlay(int i)
    {
        bool show = i == 0 ? false : true;
        abilityOverlayAnimator.GetComponent<SpriteRenderer>().enabled = show;
    }

    public void ShowPlayer(int i)
    {
        bool show = i == 0 ? false : true;
        PlayerController.Instance.GetComponent<SpriteRenderer>().enabled = show;

        //resume movement
        if (show)
            PlayerController.Instance.ChangeState(PlayerController.PlayerStates.Movement);
    }
}
