using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    public bool OnInteract()
    {
        SceneLoader.Instance.LoadScene("LevelImageGeneration");
        return true;
    }

    public void OnEnterRange()
    {
    }

    public void OnLeaveRange()
    {
    }
}