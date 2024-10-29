using UnityEngine;

[CreateAssetMenu(fileName = "PlayerPrefs")]
public class PlayerPrefs : ScriptableObject
{
    public bool experiencedTutorial = false;
    public bool detailedDescription = false;

    public float masterVolume = 1;
    public float bgmVolume = 1;
    public float sfxVolume = 1;

    public void SetExperienceTutrial(bool experiencedTutorial)
    {
        this.experiencedTutorial = experiencedTutorial;
    }
    public void ResetPrefs()
    {

    }

    public void ResetVolume()
    {

    }
}