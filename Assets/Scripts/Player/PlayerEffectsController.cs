using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerEffectsController : MonoBehaviour
{
    public static PlayerEffectsController Instance;

    [SerializeField] private Volume volume;

    [Header("Camera Effects")]
    [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;
    private CinemachineBasicMultiChannelPerlin cinemachinePerlin;
    private Coroutine shakeRoutine;

    [Header("Particle Systems")]
    [SerializeField] private ParticleSystem airDashPS;
    [SerializeField] private ParticleSystem groundDashPS;
    [SerializeField] private ParticleSystem doubleJumpPS;

    private Vignette vignette;
    private ChromaticAberration chromaticAberration;

    private Coroutine pulseRoutine;

    private void Awake()
    {
        Instance = this;
    }

    public void InitializePlayerEffectsController()
    {
        cinemachinePerlin = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        volume.profile.TryGet(out vignette);
        volume.profile.TryGet(out chromaticAberration);
    }

    public void ShakeCamera(float intensity, float frequency, float timer)
    {
        if (shakeRoutine == null)
            shakeRoutine = StartCoroutine(StartShakeCamera(intensity, frequency, timer));
    }
    private IEnumerator StartShakeCamera(float intensity, float frequency, float timer)
    {
        float duration = timer;
        float elapsedTime = 0f;

        cinemachinePerlin.m_AmplitudeGain = intensity;
        cinemachinePerlin.m_FrequencyGain = frequency;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float lerpFactor = Mathf.Clamp01(elapsedTime / duration);
            cinemachinePerlin.m_AmplitudeGain = Mathf.Lerp(intensity, 0f, lerpFactor);
            cinemachinePerlin.m_FrequencyGain = Mathf.Lerp(frequency, 0f, lerpFactor);

            yield return null;
        }

        cinemachinePerlin.m_AmplitudeGain = 0f;
        cinemachinePerlin.m_FrequencyGain = 0;
        shakeRoutine = null;
    }

    public void HitStop(float duration)
    {
        StartCoroutine(ApplyHitStop(duration));
    }
    private IEnumerator ApplyHitStop(float duration)
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1;
    }

    public void PlayDashPS(bool isGroundDash)
    {
        if (isGroundDash)
            groundDashPS.Play();
        else
            airDashPS.Play();
    }
    public void StopDashPS()
    {
        groundDashPS.Stop();
        airDashPS.Stop();
    }

    public void PlayDoubleJumpPS()
    {
        doubleJumpPS.Play();
    }
    public void StopDoubleJumpPS()
    {
        doubleJumpPS.Stop();
    }

    public void Pulse(float duration, float speed, float minPulse, float maxPulse, bool pulseOnce)
    {
        if (pulseRoutine != null)
            StopCoroutine(pulseRoutine);

        if (pulseOnce)
            pulseRoutine = StartCoroutine(PulseOnce(duration, minPulse, maxPulse));
        else
            pulseRoutine = StartCoroutine(PulseRoutine(duration, speed, minPulse, maxPulse));
    }

    private IEnumerator PulseRoutine(float duration, float speed, float minPulse, float maxPulse)
    {
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            float t = Mathf.PingPong(Time.unscaledTime * speed, 1);
            vignette.intensity.value = Mathf.Lerp(minPulse, maxPulse, t);
            chromaticAberration.intensity.value = Mathf.Lerp(0, 0.5f, t);

            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        vignette.intensity.value = minPulse;
        chromaticAberration.intensity.value = 0;
    }

    private IEnumerator PulseOnce(float duration, float minPulse, float maxPulse)
    {
        float halfDuration = duration / 2;
        float elapsedTime = 0;

        while (elapsedTime < halfDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float t = elapsedTime / halfDuration;
            vignette.intensity.value = Mathf.Lerp(minPulse, maxPulse, t);
            chromaticAberration.intensity.value = Mathf.Lerp(0, 0.1f, t);
            yield return null;
        }

        elapsedTime = 0;

        while (elapsedTime < halfDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float t = elapsedTime / halfDuration;
            vignette.intensity.value = Mathf.Lerp(maxPulse, minPulse, t);
            chromaticAberration.intensity.value = Mathf.Lerp(0.1f, 0, t);
            yield return null;
        }

        vignette.intensity.value = minPulse;
        pulseRoutine = null;
    }
}