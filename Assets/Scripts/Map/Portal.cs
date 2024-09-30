using UnityEngine;

public class Portal : Interactable
{
    public bool isActivated = false;

    public override void OnEnterRange()
    {
        base.OnEnterRange();
        isActivated = true;
    }

    public override void OnLeaveRange()
    {
        base.OnLeaveRange();
        isActivated = true;
    }
}