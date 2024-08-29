using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    [SerializeField] private int dir;

    private RoomController roomController;

    private void Start()
    {
        roomController = GetComponentInParent<RoomController>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // notify room to trigger transition
            roomController.OnTriggerTransition(dir);
        }
    }
}
