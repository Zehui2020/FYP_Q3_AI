using UnityEngine;

[CreateAssetMenu(fileName = "PlayerPrefs")]
public class PlayerPrefs : ScriptableObject
{
    public bool experiencedTutorial = false;
    public bool detailedDescription = false;
    public bool developerMode = false;

    public float masterVolume = 1.0f;
    public float bgmVolume = 1.0f;
    public float sfxVolume = 1.0f;

    public void SetExperienceTutrial(bool experiencedTutorial)
    {
        this.experiencedTutorial = experiencedTutorial;
    }

    public void SetDetailedDescrpition(bool detailed)
    {
        detailedDescription = detailed;
    }
    public void SetDeveloperMode(bool devMode)
    {
        developerMode = devMode;
    }

    public void ResetPrefs()
    {

    }

    public void ResetVolume()
    {
        masterVolume = 1.0f;
        bgmVolume = 1.0f;
        sfxVolume = 1.0f;
    }
}