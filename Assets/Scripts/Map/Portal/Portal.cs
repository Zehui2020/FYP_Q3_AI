using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class Portal : MonoBehaviour, IInteractable
{
    [SerializeField] private Animator animator;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private List<Light2D> glow = new();
    public bool isActivated = false;
    public GameObject button;

    private void Start()
    {
        foreach (Light2D light in glow)
            light.intensity = 0f;
    }

    public void OnEnterRange()
    {
        ActivatePortal();
    }

    public bool OnInteract()
    {
        return true;
    }

    public void OnLeaveRange()
    {
        ActivatePortal();
    }

    private void ActivatePortal()
    {
        if (isActivated)
            return;

        animator.SetTrigger("activate");
        AudioManager.Instance.PlayOneShot(Sound.SoundName.PortalActivate);
        audioSource.Play();
        isActivated = true;
        button.SetActive(true);
        StartCoroutine(ActivateGlow());
    }

    private IEnumerator ActivateGlow()
    {
        float duration = 1f;
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;
            float currentIntensity = Mathf.Lerp(0, 1f, timeElapsed / duration);

            foreach (Light2D light in glow)
                light.intensity = currentIntensity;

            yield return null;
        }

        foreach (Light2D light in glow)
            light.intensity = 1f;
    }
}