using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomData : MonoBehaviour
{
    public Vector2 roomSize;

    private void Start()
    {
        transform.localScale = roomSize;
    }
}
