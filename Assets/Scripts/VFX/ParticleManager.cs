using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ParticleManager : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private ParticleSystem[] allPSs;
    [SerializeField] private LightFlicker[] lights;
    [SerializeField] private AudioProxy audioProxy;
    [SerializeField] private Sound.SoundName soundToPlay;

    public void PlayAll()
    {
        foreach (ParticleSystem ps in allPSs)
        {
            if (ps.isPlaying)
                continue;

            ps.Play();
        }

        foreach (LightFlicker light in lights)
            light.StartFlicker();

        if (audioProxy != null)
            audioProxy.PlayAudioOneShot(soundToPlay);
    }

    public void StopAll()
    {
        foreach (ParticleSystem ps in allPSs)
            ps.Stop();

        foreach (LightFlicker light in lights)
            light.StopFlicker();

        if (audioProxy != null)
            audioProxy.StopAudio(soundToPlay);
    }

    public void SetAllEmissionRate(float rate)
    {
        rate = Mathf.Clamp(rate, 10, 20);

        foreach (ParticleSystem ps in allPSs)
        {
            var emission = ps.emission;
            var rateOverTime = emission.rateOverTime;

            rateOverTime.constant = rate;
            emission.rateOverTime = rateOverTime;
        }
    }
}