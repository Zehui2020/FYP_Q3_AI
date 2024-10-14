using UnityEngine;
using UnityEngine.UI;

public class Portal : MonoBehaviour, IInteractable
{
    [SerializeField] private Color activatedColor;
    public bool isActivated = false;
    public GameObject button;

    public void OnEnterRange()
    {
        isActivated = true;
        GetComponent<SpriteRenderer>().color = activatedColor;
        button.SetActive(true);
    }

    public bool OnInteract()
    {
        return true;
    }

    public void OnLeaveRange()
    {
        isActivated = true;
        GetComponent<SpriteRenderer>().color = activatedColor;
        button.SetActive(true);
    }
}