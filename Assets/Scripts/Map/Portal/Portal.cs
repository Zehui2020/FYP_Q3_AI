using UnityEngine;
using UnityEngine.UI;

public class Portal : MonoBehaviour, IInteractable
{
    [SerializeField] private Animator animator;
    public bool isActivated = false;
    public GameObject button;

    public void OnEnterRange()
    {
        animator.SetTrigger("activate");
        isActivated = true;
        button.SetActive(true);
    }

    public bool OnInteract()
    {
        return true;
    }

    public void OnLeaveRange()
    {
        isActivated = true;
        button.SetActive(true);
    }
}