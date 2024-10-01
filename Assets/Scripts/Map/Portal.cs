using UnityEngine;
using UnityEngine.UI;

public class Portal : MonoBehaviour, IInteractable
{
    [SerializeField] private SimpleAnimation keycodeUI;
    public bool isActivated = false;

    public void OnEnterRange()
    {
        keycodeUI.Show();
        isActivated = true;
    }

    public bool OnInteract()
    {
        return true;
    }

    public void OnLeaveRange()
    {
        keycodeUI.Hide();
        isActivated = true;
    }
}