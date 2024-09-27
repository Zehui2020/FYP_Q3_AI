using UnityEngine;

public class Portal : MonoBehaviour, IInteractable
{
    public bool isActivated = false;

    public void OnEnterRange()
    {
        isActivated = true;
    }

    public bool OnInteract()
    {
        return true;
    }

    public void OnLeaveRange()
    {
        isActivated = true;
    }
}
