using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] private SimpleAnimation keycodeUI;
    [SerializeField] private Animator animator;
    [SerializeField] private string nextLevel;

    public void OnEnterRange()
    {
        keycodeUI.Show();
        animator.SetTrigger("activate");
    }

    public bool OnInteract()
    {
        SceneLoader.Instance.LoadScene(nextLevel);

        keycodeUI.Hide();
        GameData.Instance.levelCount++;
        return true;
    }

    public void OnLeaveRange()
    {
        keycodeUI.Hide();
    }
}