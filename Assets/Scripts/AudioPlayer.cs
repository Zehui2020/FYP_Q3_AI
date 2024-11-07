using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    [SerializeField] private List<Sound.SoundName> soundsToPlay = new();

    public void PlayOneShot()
    {
        foreach (Sound.SoundName sound in soundsToPlay)
            AudioManager.Instance.PlayOneShot(sound);
    }

    public void Play()
    {
        foreach (Sound.SoundName sound in soundsToPlay)
            AudioManager.Instance.Play(sound);
    }
}