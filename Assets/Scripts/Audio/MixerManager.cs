using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class MixerManager : MonoBehaviour
{
    // Audio
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;

    // Player prefs
    [SerializeField] private PlayerPrefs playerSettings;

    public static MixerManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SetSliders();
        SetMasterVolume(false);
        SetBGMVolume(false);
        SetSFXVolume(false);
    }

    private void OnEnable()
    {
        SetSliders();
        SetMasterVolume(false);
        SetBGMVolume(false);
        SetSFXVolume(false);
    }

    public void SetMasterVolume(bool playSound)
    {
        float volume = masterSlider.value > 0 ? Mathf.Log10(masterSlider.value) * 20 : -80f;
        SetVolume("Master", volume);
        playerSettings.masterVolume = masterSlider.value;
        PlaySlideAudio(playSound);
    }

    public void SetBGMVolume(bool playSound)
    {
        float volume = bgmSlider.value > 0 ? Mathf.Log10(bgmSlider.value) * 20 + 5 : -80f;
        SetVolume("BGM", volume);
        playerSettings.bgmVolume = bgmSlider.value;
        PlaySlideAudio(playSound);
    }

    public void SetSFXVolume(bool playSound)
    {
        float volume = sfxSlider.value > 0 ? Mathf.Log10(sfxSlider.value) * 20 : -80f;
        SetVolume("SFX", volume);
        playerSettings.sfxVolume = sfxSlider.value;
        PlaySlideAudio(playSound);
    }

    public void PlaySlideAudio(bool playSound)
    {
        if (!playSound && AudioManager.Instance.CheckIfSoundPlaying(Sound.SoundName.SettingsSlide))
        {
            AudioManager.Instance.Stop(Sound.SoundName.SettingsSlide);
            AudioManager.Instance.PlayOneShot(Sound.SoundName.MainMenuClick);
        }

        if (!playSound)
            return;

        if (!AudioManager.Instance.CheckIfSoundPlaying(Sound.SoundName.SettingsSlide))
            AudioManager.Instance.Play(Sound.SoundName.SettingsSlide);
    }

    public void ResetVolume()
    {
        playerSettings.ResetVolume();

        masterSlider.value = 1;
        bgmSlider.value = 1;
        sfxSlider.value = 1;

        SetMasterVolume(false);
        SetBGMVolume(false);
        SetSFXVolume(false);
    }

    public void SetSliders()
    {
        masterSlider.value = playerSettings.masterVolume;
        bgmSlider.value = playerSettings.bgmVolume;
        sfxSlider.value = playerSettings.sfxVolume;
    }

    private void SetVolume(string name, float volume)
    {
        audioMixer.SetFloat(name, volume);
    }
}