using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;
using System.Collections;

public class AbilityAnimationController : MonoBehaviour
{
    [SerializeField] public Animator abilityOverlayAnimator;
    [SerializeField] private List<Light2D> spotLight;

    private void Awake()
    {
        for (int i = 0; i < spotLight.Count; i++)
            spotLight[i].enabled = false;
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

    public void ShowAbilityLight(int i)
    {
        bool show = !spotLight[i].enabled;
        StartCoroutine(LightRoutine(i, show));
    }

    private IEnumerator LightRoutine(int i, bool show)
    {
        float timer = 2;
        float a;

        if (show)
        {
            a = 0;
            spotLight[i].enabled = show;
        }
        else
            a = 1;

        spotLight[i].color = new Color(spotLight[i].color.r, spotLight[i].color.g, spotLight[i].color.b, a);

        while (timer > 0)
        {
            if (show)
                a = Mathf.Lerp(a, 1, 0.05f);
            else
                a = Mathf.Lerp(a, 0, 0.05f);

            spotLight[i].color = new Color(spotLight[i].color.r, spotLight[i].color.g, spotLight[i].color.b, a);

            timer -= Time.deltaTime;
            yield return null;
        }

        spotLight[i].enabled = show;

        if (!show)
            spotLight[i].color = new Color(spotLight[i].color.r, spotLight[i].color.g, spotLight[i].color.b, 0);
        else
            spotLight[i].color = new Color(spotLight[i].color.r, spotLight[i].color.g, spotLight[i].color.b, 1);
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
