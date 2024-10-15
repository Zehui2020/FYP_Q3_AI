using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] private SimpleAnimation keycodeUI;
    [SerializeField] private Animator animator;

    public void OnEnterRange()
    {
        keycodeUI.Show();
        animator.SetTrigger("activate");
    }

    public bool OnInteract()
    {
        SceneLoader.Instance.LoadScene("LevelImageGeneration");
        keycodeUI.Hide();
        GameData.Instance.currentLevel++;
        return true;
    }

    public void OnLeaveRange()
    {
        keycodeUI.Hide();
    }
}