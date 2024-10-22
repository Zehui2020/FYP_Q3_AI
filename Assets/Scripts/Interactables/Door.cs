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
        if (GameData.Instance.currentLevel >= GameData.Instance.maxLevels)
            SceneLoader.Instance.LoadScene("BossLevel");
        else
            SceneLoader.Instance.LoadScene(nextLevel);

        keycodeUI.Hide();
        GameData.Instance.currentLevel++;
        return true;
    }

    public void OnLeaveRange()
    {
        keycodeUI.Hide();
    }
}