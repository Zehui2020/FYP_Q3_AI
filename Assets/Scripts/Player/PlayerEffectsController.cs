using Cinemachine;
using DesignPatterns.ObjectPool;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerEffectsController : MonoBehaviour
{
    public static PlayerEffectsController Instance;

    [SerializeField] private Volume volume;
    [SerializeField] private Animator camAnimator;

    [Header("Camera Effects")]
    [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;
    private CinemachineBasicMultiChannelPerlin cinemachinePerlin;
    private Coroutine shakeRoutine;

    [Header("Particle Systems")]
    [SerializeField] private ParticleSystem airDashPS;
    [SerializeField] private ParticleSystem groundDashPS;
    [SerializeField] private ParticleSystem doubleJumpPS;
    [SerializeField] private ParticleSystem dustPS;
    [SerializeField] private ParticleSystem landPS;

    [Header("Others")]
    [SerializeField] private MoneyPopupCounter moneyPopup;
    [SerializeField] private SpriteRenderer playerSR;
    [SerializeField] private float afterimageSpawnRate;
    [SerializeField] private GameObject shadowBound;

    private Vignette vignette;
    private ChromaticAberration chromaticAberration;

    private Coroutine pulseRoutine;
    private Coroutine afterimageRoutine;

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

    public void SetShadowBound(bool active)
    {
        if (active)
            StartCoroutine(ShadowBoundRoutine());
        else
            shadowBound.SetActive(false);
    }
    private IEnumerator ShadowBoundRoutine()
    { 
        yield return new WaitForSeconds(0.8f);

        shadowBound.SetActive(true);
    }

    public void StartSpawnAfterimage()
    {
        if (afterimageRoutine == null)
            afterimageRoutine = StartCoroutine(SpawnAfterimageRoutine());
    }
    public void StopSpawnAfterimage()
    {
        if (afterimageRoutine != null)
        {
            StopCoroutine(afterimageRoutine);
            afterimageRoutine = null;
        }
    }
    private IEnumerator SpawnAfterimageRoutine()
    {
        while (true)
        {
            PlayerAfterimage afterimage = ObjectPool.Instance.GetPooledObject("Afterimage", true) as PlayerAfterimage;
            afterimage.SetupImage(playerSR.sprite, Mathf.Sign(transform.localScale.x));
            afterimage.transform.position = transform.position;
            yield return new WaitForSeconds(afterimageSpawnRate);
        }
    }

    public void AddMoney(int amount)
    {
        moneyPopup.AddMoney(amount);
    }

    public void ShakeCamera(float intensity, float frequency, float timer)
    {
        if (shakeRoutine != null)
            return;

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

    public void HitStop(float duration, float timeScale, bool lerp, float lerpSpeed)
    {
        StartCoroutine(ApplyHitStop(duration, timeScale, lerp, lerpSpeed));
    }
    private IEnumerator ApplyHitStop(float duration, float timeScale, bool lerp, float lerpSpeed)
    {
        Time.timeScale = timeScale;

        if (lerp)
        {
            while (Time.timeScale <= 0.97f)
            {
                Time.timeScale = Mathf.Lerp(Time.timeScale, 1, Time.deltaTime * lerpSpeed);
                yield return null;
            }

            Time.timeScale = 1;
        }
        else
        {
            yield return new WaitForSecondsRealtime(duration);
            Time.timeScale = 1;
        }
    }

    public void SetCameraTrigger(string trigger)
    {
        camAnimator.SetTrigger(trigger);
    }

    public void BurstDustPS()
    {
        dustPS.Play();
    }

    public void BurstLandPS()
    {
        landPS.Play();
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