using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] private SimpleAnimation keycodeUI;

    public void OnEnterRange()
    {
        keycodeUI.Show();
    }

    public bool OnInteract()
    {
        SceneLoader.Instance.LoadScene("LevelImageGeneration");
        keycodeUI.Hide();
        return true;
    }

    public void OnLeaveRange()
    {
        keycodeUI.Hide();
    }
}