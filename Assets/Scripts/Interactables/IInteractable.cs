using UnityEngine;

public class Interactable : MonoBehaviour
{
    [SerializeField] private SimpleAnimation keycodeUI;

    public virtual bool OnInteract() { return true; }
    public virtual void OnEnterRange() { keycodeUI.Show(); }
    public virtual void OnLeaveRange() { keycodeUI.Hide(); }
}