using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class MapData : ScriptableObject
{
    public GameObject startRoom;
    public GameObject endRoom;
    public int noOfRooms;
    public int roomSpacing;
    // map gen
    public List<GameObject> roomTypes;
    // procedural map gen
    public List<GameObject> roomType0;
    public List<GameObject> roomType1;
    public List<GameObject> roomType2;
    public List<GameObject> roomType3;
    public Vector2 mapSize;
}
