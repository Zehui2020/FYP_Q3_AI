using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTrigger : MonoBehaviour, IInteractable
{
    [SerializeField] private int dir;
    [SerializeField] private RoomController roomController;

    public bool OnInteract()
    {
        roomController.OnTriggerTransition(dir);
        return true;
    }

    public void OnEnterRange()
    {
    }

    public void OnLeaveRange()
    {
    }
}
