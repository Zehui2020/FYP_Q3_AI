//----------------------------------------------------------------------
// SFXVolumeController
//
// Class that manages the volume of SFX
//
// Date: 1/10/2024
// Author: Shimba Sakai
//----------------------------------------------------------------------

using UnityEngine;

public class SFXVolumeController : MonoBehaviour
{
    // Adjust the SFX volume
    public void SetSFXVolume(float volume)
    {
        // Set the SFX volume through the SoundManager
        SoundManager.Instance.SetSFXVolume(volume);
    }

    // Play the decision sound effect
    public void PlaySampleDecisionSFX()
    {
        // Play the decision sound (set to track 0)
        SoundManager.Instance.PlaySFX(0);
    }

    // Play the return sound effect
    public void PlaySampleReturnSFX()
    {
        // Play the return sound (set to track 1)
        SoundManager.Instance.PlaySFX(1);
    }
}
