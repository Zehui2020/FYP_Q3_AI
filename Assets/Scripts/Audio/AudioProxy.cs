using UnityEngine;

public class AudioProxy : MonoBehaviour
{
    public void PlayAudioOneShot(Sound.SoundName name)
    {
        AudioManager.Instance.PlayOneShot(name);
    }

    public void StopAudio(Sound.SoundName name)
    {
        AudioManager.Instance.Stop(name);
    }

    public void PlayRandomWalkingSound()
    {
        AudioManager.Instance.PlayRandomWalkingSound();
    }
}