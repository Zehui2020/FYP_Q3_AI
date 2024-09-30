using UnityEngine;

public class Door : Interactable
{
    public override bool OnInteract()
    {
        SceneLoader.Instance.LoadScene("LevelImageGeneration");
        return true;
    }
}