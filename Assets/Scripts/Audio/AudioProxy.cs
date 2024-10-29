using UnityEngine;

public class AudioProxy : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

    public void PlayAudioOneShot(Sound.SoundName name)
    {
        if (audioSource == null)
            AudioManager.Instance.PlayOneShot(name);
        else
            AudioManager.Instance.PlayOneShot(audioSource, name);
    }

    public void StopAudio(Sound.SoundName name)
    {
        AudioManager.Instance.FadeSound(false, name, 0.1f, 0);
    }

    public void StopAudioInstantly(Sound.SoundName name)
    {
        if (audioSource == null)
            AudioManager.Instance.Stop(name);
        else
            audioSource.Stop();
    }

    public void PlayRandomWalkingSound()
    {
        AudioManager.Instance.PlayRandomWalkingSound();
    }
}