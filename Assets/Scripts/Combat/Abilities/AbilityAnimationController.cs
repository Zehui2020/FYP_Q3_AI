using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class AbilityAnimationController : MonoBehaviour
{
    [SerializeField] private Animator abilityOverlayAnimator;
    [SerializeField] private Light2D spotLight;
    public string animName;

    private void Awake()
    {
        spotLight.enabled = false;
    }

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

    public void ShowAbilityLight()
    {
        bool show = !spotLight.enabled;
        StartCoroutine(LightRoutine(show));
    }

    private IEnumerator LightRoutine(bool show)
    {
        float timer = 2;
        float a;

        if (show)
        {
            a = 0;
            spotLight.enabled = show;
        }
        else
            a = 1;

        spotLight.color = new Color(spotLight.color.r, spotLight.color.g, spotLight.color.b, a);

        while (timer > 0)
        {
            if (show)
                a = Mathf.Lerp(a, 1, 0.05f);
            else
                a = Mathf.Lerp(a, 0, 0.05f);

            spotLight.color = new Color(spotLight.color.r, spotLight.color.g, spotLight.color.b, a);

            timer -= Time.deltaTime;
            yield return null;
        }

        spotLight.enabled = show;

        if (!show)
            spotLight.color = new Color(spotLight.color.r, spotLight.color.g, spotLight.color.b, 0);
        else
            spotLight.color = new Color(spotLight.color.r, spotLight.color.g, spotLight.color.b, 1);
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
