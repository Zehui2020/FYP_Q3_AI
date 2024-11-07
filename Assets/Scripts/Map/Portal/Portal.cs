using UnityEngine;
using UnityEngine.UI;

public class Portal : MonoBehaviour, IInteractable
{
    [SerializeField] private Animator animator;
    public bool isActivated = false;
    public GameObject button;

    public void OnEnterRange()
    {
        ActivatePortal();
    }

    public bool OnInteract()
    {
        return true;
    }

    public void OnLeaveRange()
    {
        ActivatePortal();
    }

    private void ActivatePortal()
    {
        if (isActivated)
            return;

        animator.SetTrigger("activate");
        AudioManager.Instance.PlayOneShot(Sound.SoundName.PortalActivate);
        isActivated = true;
        button.SetActive(true);
    }
}