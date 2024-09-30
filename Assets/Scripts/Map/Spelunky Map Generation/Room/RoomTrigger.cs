using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTrigger : Interactable
{
    [SerializeField] private int dir;
    [SerializeField] private RoomController roomController;

    public override bool OnInteract()
    {
        roomController.OnTriggerTransition(dir);
        return true;
    }
}