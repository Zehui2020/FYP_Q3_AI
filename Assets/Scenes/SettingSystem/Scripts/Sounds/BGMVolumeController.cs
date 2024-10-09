//----------------------------------------------------------------------
// BGMVolumeController
//
// Class that manages the volume of the BGM
//
// Date: 1/10/2024
// Author: Shimba Sakai
//----------------------------------------------------------------------

using UnityEngine;

public class BGMVolumeController : MonoBehaviour
{
    // Adjust the BGM volume
    public void SetBGMVolume(float volume)
    {
        // Set the BGM volume through the SoundManager
        SoundManager.Instance.SetBGMVolume(volume);
    }

    // Play a sample BGM
    public void PlaySampleBGM()
    {
        // Play the sample BGM (set to track 0)
        SoundManager.Instance.PlayBGM(0);
    }
}
