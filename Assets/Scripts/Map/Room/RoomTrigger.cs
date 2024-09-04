using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTrigger : MonoBehaviour, IInteractable
{
    [SerializeField] private int dir;
    [SerializeField] private RoomController roomController;

    public void OnInteract()
    {
        roomController.OnTriggerTransition(dir);
    }

    public void OnEnterRange()
    {
    }

    public void OnLeaveRange()
    {
    }
}
