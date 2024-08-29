using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class MapData : ScriptableObject
{
    public GameObject startRoom;
    public GameObject endRoom;
    public GameObject shopRoom;
    public List<GameObject> roomType0;
    public List<GameObject> roomType1;
    public List<GameObject> roomType2;
    public List<GameObject> roomType3;
    public int roomSpacing;
    public Vector2 mapSize;
    public Vector2 minMaxRoomAmt;
}
