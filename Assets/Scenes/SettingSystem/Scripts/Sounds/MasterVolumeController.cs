//----------------------------------------------------------------------
// MasterVolumeController
//
// Class that manages the master volume
//
// Date: 1/10/2024
// Author: Shimba Sakai
//----------------------------------------------------------------------

using UnityEngine;

public class MasterVolumeController : MonoBehaviour
{
    [Header("Class that handles the UI of the settings screen")]
    public SettingUIHandler m_settingUIHandler;

    // Instance of the class that manages the master volume
    public static MasterVolumeController m_instance;

    // Class that manages the BGM volume adjustment
    public BGMVolumeController m_BGMVolumeController;

    // Class that manages the SFX volume adjustment
    public SFXVolumeController m_SFXVolumeController;

    // Variable to store the current master volume
    public float m_masterVolume = 0.4f;

    // Initial BGM and SFX ratio (relative to master volume)
    private float m_initialBGMPercentage = 0.7f;
    private float m_initialSFXPercentage = 0.5f;

    void Awake()
    {
        // Handle the case where the instance of the class that manages the master volume is not assigned
        if (m_instance == null)
        {
            m_instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Handle the case where the class that handles the UI of the settings screen is not assigned
        if (m_settingUIHandler == null)
        {
            m_settingUIHandler = FindAnyObjectByType<SettingUIHandler>();
        }

        // Handle the case where the class that manages the BGM volume adjustment is not assigned
        if (m_BGMVolumeController == null)
        {
            m_BGMVolumeController = GetComponent<BGMVolumeController>();
        }

        // Handle the case where the class that manages the SFX volume adjustment is not assigned
        if (m_SFXVolumeController == null)
        {
            m_SFXVolumeController = GetComponent<SFXVolumeController>();
        }

        // Set the slider for master volume adjustment
        m_settingUIHandler.m_masterVolumeSlider.onValueChanged.AddListener(delegate { SetMasterVolume(); });

        // Set the slider for BGM volume adjustment
        m_settingUIHandler.m_musicVolumeSlider.onValueChanged.AddListener(delegate { SetBGM(); });

        // Set the slider for SFX volume adjustment
        m_settingUIHandler.m_soundEffectVolumeSlider.onValueChanged.AddListener(delegate { SetSFX(); });

        // Set the initial volumes
        SetInitialVolumes();
    }

    // Set the initial volumes
    private void SetInitialVolumes()
    {
        // Set the master volume
        m_settingUIHandler.m_masterVolumeSlider.value = m_masterVolume;

        // Set the initial BGM and SFX volumes, considering their ratio relative to the master volume
        m_settingUIHandler.m_musicVolumeSlider.value = m_initialBGMPercentage * m_masterVolume;
        m_settingUIHandler.m_soundEffectVolumeSlider.value = m_initialSFXPercentage * m_masterVolume;

        // Set the volumes based on the master volume
        SetBGM();
        SetSFX();
    }

    // Set the master volume
    public void SetMasterVolume()
    {
        m_masterVolume = m_settingUIHandler.m_masterVolumeSlider.value;
        SetBGM();
        SetSFX();
    }

    // Set the BGM volume
    private void SetBGM()
    {
        float bgmVolume = m_settingUIHandler.m_musicVolumeSlider.value;
        SoundManager.Instance.SetBGMVolume(bgmVolume * m_masterVolume);
    }

    // Set the SFX volume
    private void SetSFX()
    {
        float sfxVolume = m_settingUIHandler.m_soundEffectVolumeSlider.value;
        SoundManager.Instance.SetSFXVolume(sfxVolume * m_masterVolume);
    }

    // Play BGM
    public void PlayBGM(int index)
    {
        SoundManager.Instance.PlayBGM(index);
    }

    // Play SFX
    public void PlaySFX(int index)
    {
        SoundManager.Instance.PlaySFX(index);
    }
}
