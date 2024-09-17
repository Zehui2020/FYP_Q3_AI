using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    public void OnInteract()
    {
        SceneLoader.Instance.LoadScene("LevelImageGeneration");
    }

    public void OnEnterRange()
    {
    }

    public void OnLeaveRange()
    {
    }
}