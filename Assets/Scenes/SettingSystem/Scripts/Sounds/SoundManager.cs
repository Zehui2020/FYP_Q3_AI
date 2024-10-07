//----------------------------------------------------------------------
// SoundManager
//
// Class that manages all sounds
//
// Date: 1/10/2024
// Author: Shimba Sakai
//----------------------------------------------------------------------

using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [Header("AudioSource for BGM")]
    public AudioSource m_BGMAudioSource;

    [Header("AudioSource for SFX")]
    public AudioSource m_SFXAudioSource;

    [Header("AudioClips for BGM")]
    public AudioClip[] m_BGMAudioClips;

    [Header("AudioClips for SFX")]
    public AudioClip[] m_SFXAudioClips;

    // Instance of the sound manager
    private static SoundManager m_instance;

    // Instance of the sound manager
    public static SoundManager Instance
    {
        get
        {
            // Handle the case where the instance of the sound manager is not assigned
            if (m_instance == null)
            {
                // Assign the sound manager
                m_instance = FindObjectOfType<SoundManager>();

                // Handle the case where the sound manager is not assigned
                if (m_instance == null)
                {
                    // Create a new sound manager
                    GameObject soundManagerObject = new GameObject("SoundManager");
                    // Assign the created sound manager to the instance
                    m_instance = soundManagerObject.AddComponent<SoundManager>();
                }
            }
            // Return the instance of the sound manager
            return m_instance;
        }
    }

    void Awake()
    {
        // Handle the case where the instance of the sound manager is not assigned
        if (m_instance == null)
        {
            // Assign the sound manager
            m_instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Set the BGM volume
    public void SetBGMVolume(float volume)
    {
        // Handle the case where BGM is assigned
        if (m_BGMAudioSource != null)
        {
            // Set the BGM volume
            m_BGMAudioSource.volume = volume;
        }
    }

    // Set the SFX volume
    public void SetSFXVolume(float volume)
    {
        // Handle the case where SFX is assigned
        if (m_SFXAudioSource != null)
        {
            // Set the SFX volume
            m_SFXAudioSource.volume = volume;
        }
    }

    // Play the BGM (managed by index)
    public void PlayBGM(int index)
    {
        if (index >= 0 && index < m_BGMAudioClips.Length)
        {
            if (m_BGMAudioSource != null)
            {
                // Set the AudioClip for BGM
                m_BGMAudioSource.clip = m_BGMAudioClips[index];
                // Play the BGM
                m_BGMAudioSource.Play();
            }
        }
        else
        {
            Debug.LogWarning("BGM index is out of range: " + index);
        }
    }

    // Play the SFX
    public void PlaySFX(int index)
    {
        if (index >= 0 && index < m_SFXAudioClips.Length)
        {
            if (m_SFXAudioSource != null)
            {
                // Set the AudioClip for SFX
                m_SFXAudioSource.clip = m_SFXAudioClips[index];
                // Play the SFX
                m_SFXAudioSource.Play();
            }
        }
        else
        {
            Debug.LogWarning("SFX index is out of range: " + index);
        }
    }

    // Stop the BGM for the specified index
    public void StopBGM(int index)
    {
        // Handle the case where the BGM index is within range
        if (index >= 0 && index < m_BGMAudioClips.Length)
        {
            // Handle the case where BGM is assigned and the current index matches the specified one
            if (m_BGMAudioSource != null && m_BGMAudioSource.clip == m_BGMAudioClips[index])
            {
                // Stop the specified BGM
                m_BGMAudioSource.Stop();
            }
        }
    }

    // Stop the specified SFX (not currently used)
    public void StopSFX()
    {
        // Handle the case where SFX is assigned and playing
        if (m_SFXAudioSource != null && m_SFXAudioSource.isPlaying)
        {
            // Stop the SFX
            m_SFXAudioSource.Stop();
        }
    }

    // Stop all sounds
    public void StopAllSounds()
    {
        // Handle the case where BGM is assigned and playing
        if (m_BGMAudioSource != null && m_BGMAudioSource.isPlaying)
        {
            // Stop the BGM
            m_BGMAudioSource.Stop();
        }

        // Handle the case where SFX is assigned and playing
        if (m_SFXAudioSource != null && m_SFXAudioSource.isPlaying)
        {
            // Stop the SFX
            m_SFXAudioSource.Stop();
        }
    }
}
