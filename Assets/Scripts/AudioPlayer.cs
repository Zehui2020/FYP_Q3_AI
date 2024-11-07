using System;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    [SerializeField] private List<Sound.SoundName> soundsToPlay = new();

    public void PlayOneShot(int index)
    {
        AudioManager.Instance.PlayOneShot(soundsToPlay[index]);
    }

    public void Play(int index)
    {
        AudioManager.Instance.Play(soundsToPlay[index]);
    }
}