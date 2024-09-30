using UnityEngine;
using UnityEngine.UI;

public class Portal : MonoBehaviour, IInteractable
{
    public bool isActivated = false;
    public Button button;

    public void OnEnterRange()
    {
        isActivated = true;
        button.interactable = true;
    }

    public bool OnInteract()
    {
        return true;
    }

    public void OnLeaveRange()
    {
        isActivated = true;
        button.interactable = true;
    }
}
