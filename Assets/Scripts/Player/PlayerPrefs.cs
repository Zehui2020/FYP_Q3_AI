using UnityEngine;

[CreateAssetMenu(fileName = "PlayerPrefs")]
public class PlayerPrefs : ScriptableObject
{
    public bool experiencedTutorial = false;
    public bool detailedDescription = false;

    public void SetExperienceTutrial(bool experiencedTutorial)
    {
        this.experiencedTutorial = experiencedTutorial;
    }
    public void ResetPrefs()
    {

    }
}