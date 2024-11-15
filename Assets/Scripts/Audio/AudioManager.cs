using UnityEngine.Audio;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public AudioMixer audioMixer;

    public Sound[] sounds;
    public Sound[] walkingSounds;

    private int walkIndex;

    public static AudioManager Instance;

    void Awake()
    {
        Instance = this;
        InitSoundList(sounds);
        InitSoundList(walkingSounds);

        // Play BGM for the level
        LoadBGM();
    }

    private void InitSoundList(Sound[] sounds)
    {
        foreach (Sound s in sounds)
        {
            if (!s.createSource)
                continue;

            s.source = gameObject.AddComponent<AudioSource>();
            InitAudioSource(s.source, s);

            if (s.playOnAwake)
                s.source.Play();
        }
    }

    public void InitAudioSource(AudioSource source, Sound sound)
    {
        source.clip = sound.clip;
        source.outputAudioMixerGroup = sound.mixerGroup;
        source.pitch = sound.pitch;
        source.volume = sound.volume;
        source.loop = sound.loop;
    }

    public void PlayRandomWalkingSound()
    {
        walkingSounds[walkIndex].source.pitch = Random.Range(0.7f, 1.3f);
        walkingSounds[walkIndex].source.PlayOneShot(walkingSounds[walkIndex].clip);
        walkIndex++;
        if (walkIndex >= walkingSounds.Length)
            walkIndex = 0;
    }

    public void RandomiseAudioPitch(Sound.SoundName sound, float minPitch, float maxPitch)
    {
        Sound s = FindSound(sound);
        s.source.pitch = Random.Range(minPitch, maxPitch);
    }

    public Sound FindSound(Sound.SoundName name)
    {
        foreach (Sound s in sounds)
        {
            if (s.name.Equals(name))
                return s;
        }
        return null;
    }

    public void Play(Sound.SoundName sound)
    {
        Sound s = FindSound(sound);
        ResetVolumeOfSound(s);
        s.source.Play();
    }

    public void PlayOneShot(Sound.SoundName sound)
    {
        Sound s = FindSound(sound);
        ResetVolumeOfSound(s);
        s.source.PlayOneShot(s.clip);
    }

    public void PlayOneShot(AudioSource source, Sound.SoundName sound)
    {
        Sound s = FindSound(sound);
        InitAudioSource(source, s);
        source.PlayOneShot(s.clip);
    }

    public void OnlyPlayAfterSoundEnds(Sound.SoundName sound)
    {
        Sound s = FindSound(sound);
        if (s.name.Equals(sound) && !s.source.isPlaying)
            s.source.Play();
    }

    public void Stop(Sound.SoundName sound)
    {
        FindSound(sound).source.Stop();
    }

    public void StopAllSounds()
    {
        foreach (Sound s in sounds)
            s.source.Stop();
    }

    public void Pause(Sound.SoundName sound)
    {
        FindSound(sound).source.Pause();
    }

    public void Unpause(Sound.SoundName sound)
    {
        FindSound(sound).source.UnPause();
    }

    public void PauseAllSounds()
    {
        foreach (Sound s in sounds)
        {
            if (s.source != null)
                s.source.Pause();
        }
    }

    public void UnpauseAllSounds()
    {
        foreach (Sound s in sounds)
        {
            if (s.source != null)
                s.source.UnPause();
        }
    }

    public bool CheckIfSoundPlaying(Sound.SoundName sound)
    {
        return FindSound(sound).source.isPlaying;
    }

    public void FadeAllSound(bool fadeIn, float duration, float targetVolume)
    {
        foreach (Sound s in sounds)
        {
            if (s.doNotFade)
                continue;

            StopFadeRoutine(s.name);
            s.fadeRoutine = StartCoroutine(s.FadeSoundRoutine(fadeIn, duration, targetVolume));
        }
    }

    public void FadeSound(bool fadeIn, Sound.SoundName sound, float duration, float targetVolume)
    {
        Sound s = FindSound(sound);
        StopFadeRoutine(sound);
        s.fadeRoutine = StartCoroutine(s.FadeSoundRoutine(fadeIn, duration, targetVolume));
    }

    public void StopFadeRoutine(Sound.SoundName sound)
    {
        Sound s = FindSound(sound);

        if (s.fadeRoutine != null)
        {
            StopCoroutine(s.fadeRoutine);
            s.fadeRoutine = null;
            ResetVolumeOfSound(s);
        }
    }

    private void ResetVolumeOfSound(Sound sound)
    {
        if (sound.source == null)
            return;

        FindSound(sound.name).source.volume = sound.volume;
    }

    public void PlayAfterDelay(float delay, Sound.SoundName sound)
    {
        FindSound(sound).source.PlayDelayed(delay);
    }

    public void SetPitch(Sound.SoundName sound, float newPitch)
    {
        Sound s = FindSound(sound);
        s.source.pitch = newPitch;
    }

    public void LoadBGM()
    {
        StartCoroutine(LoadBGMAudioRoutine());
    }
    private IEnumerator LoadBGMAudioRoutine()
    {
        if (GameData.Instance == null)
            yield break;

        string audioURL = "file://" + Application.persistentDataPath + "/" + GameData.Instance.currentLevel + ".wav";

        Debug.Log("Image URL: " + audioURL);

        using (UnityWebRequest webRequest = UnityWebRequestMultimedia.GetAudioClip(audioURL, AudioType.UNKNOWN))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                // Get the downloaded texture
                AudioClip bgm = ((DownloadHandlerAudioClip)webRequest.downloadHandler).audioClip;
                if (bgm != null)
                {
                    Sound s = new Sound();
                    s.source = gameObject.AddComponent<AudioSource>();
                    s.clip = bgm;
                    s.loop = true;
                    InitAudioSource(s.source, s);
                    s.source.Play();

                    Debug.Log("BGM LOADED!");
                }
            }
            else
            {
                // Get the downloaded texture
                AudioClip bgm = Resources.Load<AudioClip>(GameData.Instance.currentLevel);
                if (bgm != null)
                {
                    Sound s = new Sound();
                    s.source = gameObject.AddComponent<AudioSource>();
                    s.clip = bgm;
                    s.loop = true;
                    InitAudioSource(s.source, s);
                    s.source.Play();

                    Debug.Log("BGM LOADED THROUGH RESOURCES!");
                }
            }
        }
    }
}