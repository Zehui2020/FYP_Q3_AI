using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTile : MonoBehaviour
{
    [SerializeField] public List<GameObject> availableTilesUp;
    [SerializeField] public List<GameObject> availableTilesDown;
    [SerializeField] public List<GameObject> availableTilesLeft;
    [SerializeField] public List<GameObject> availableTilesRight;

    public Vector2 GetRandomAvailableDirection()
    {
        // find available directions
        List<Vector2> randomizableDirections = new List<Vector2>();

        if (availableTilesUp.Count > 0)
            randomizableDirections.Add(Vector2.up);
        if (availableTilesDown.Count > 0)
            randomizableDirections.Add(Vector2.down);
        if (availableTilesLeft.Count > 0)
            randomizableDirections.Add(Vector2.left);
        if (availableTilesRight.Count > 0)
            randomizableDirections.Add(Vector2.right);
        // return random direction
        if (randomizableDirections.Count > 0)
            return randomizableDirections[Random.Range(0, randomizableDirections.Count)];
        else
            return Vector2.zero;
    }
}