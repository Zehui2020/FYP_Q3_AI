using UnityEngine;

[CreateAssetMenu(fileName = "PlayerPrefs")]
public class PlayerPrefs : ScriptableObject
{
    public bool experiencedTutorial = false;

    public void ResetPrefs()
    {

    }
}