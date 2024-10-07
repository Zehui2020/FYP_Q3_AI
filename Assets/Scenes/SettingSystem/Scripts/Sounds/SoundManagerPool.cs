//----------------------------------------------------------------------
// SoundManagerPool
//
// Class for pooling sound manager instances
//
// Date: 2/10/2024
// Author: Shimba Sakai
//----------------------------------------------------------------------

using UnityEngine;
using System.Collections.Generic;

public class SoundManagerPool : MonoBehaviour
{
    [Header("Prefab of the Sound Manager class")]
    // Prefab of the Sound Manager class
    public GameObject m_soundManagerPrefab;

    [Header("Size of the pool")]
    // Size of the pool
    public int m_poolSize = 2;

    // Manage instances of the sound manager class in a pool
    private List<SoundManager> m_soundManagerPool;

    // Index of the next available instance
    private int nextAvailableIndex = 0;

    void Start()
    {
        // Initialize the sound manager pool
        InitializeSoundManagerPool();
    }

    // Initialize the sound manager pool
    private void InitializeSoundManagerPool()
    {
        // Create the pool list for the sound manager class
        m_soundManagerPool = new List<SoundManager>();

        // Instantiate the sound manager class according to the specified pool size
        for (int i = 0; i < m_poolSize; i++)
        {
            // Instantiate a new sound manager from the prefab
            GameObject newSoundManager = Instantiate(m_soundManagerPrefab);

            // Get the sound manager component from the instantiated object
            SoundManager soundManager = newSoundManager.GetComponent<SoundManager>();

            // Add it to the pool list
            m_soundManagerPool.Add(soundManager);

            // Set the instance to inactive
            newSoundManager.SetActive(false);
        }
    }

    // Get an available sound manager instance from the pool
    public SoundManager GetSoundManagerFromPool()
    {
        // Get the sound manager instance from the current index
        SoundManager soundManager = m_soundManagerPool[nextAvailableIndex];

        // If BGM is playing, stop it
        StopActiveBGM(soundManager);

        // If SFX is playing, stop it
        StopActiveSFX(soundManager);

        // Activate the sound manager instance
        soundManager.gameObject.SetActive(true);

        // Update the next available index
        nextAvailableIndex = (nextAvailableIndex + 1) % m_poolSize;

        return soundManager;
    }

    // Stop BGM if it's currently playing
    private void StopActiveBGM(SoundManager soundManager)
    {
        // If BGM is currently playing
        if (soundManager.m_BGMAudioSource.isPlaying)
        {
            // Get the index of the currently playing BGM
            int bgmIndex = GetCurrentBGMIndex(soundManager);

            // Stop the BGM
            soundManager.StopBGM(bgmIndex);
        }
    }

    // Get the index of the currently playing BGM
    private int GetCurrentBGMIndex(SoundManager soundManager)
    {
        // If a BGM is currently playing
        if (soundManager.m_BGMAudioSource.clip != null)
        {
            // Get the index of the current clip from the m_BGMAudioClips array
            return System.Array.IndexOf(soundManager.m_BGMAudioClips, soundManager.m_BGMAudioSource.clip);
        }

        // If no BGM is playing, return -1
        return -1;
    }

    // Stop SFX if it's currently playing
    private void StopActiveSFX(SoundManager soundManager)
    {
        // If SFX is currently playing
        if (soundManager.m_SFXAudioSource.isPlaying)
        {
            // Stop the SFX
            soundManager.m_SFXAudioSource.Stop();
        }
    }
}
