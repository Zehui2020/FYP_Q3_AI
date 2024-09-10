using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEffectsController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;
    private CinemachineBasicMultiChannelPerlin cinemachinePerlin;

    private Coroutine shakeRoutine;

    public void InitializePlayerEffectsController()
    {
        cinemachinePerlin = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
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
            elapsedTime += Time.deltaTime;
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
}