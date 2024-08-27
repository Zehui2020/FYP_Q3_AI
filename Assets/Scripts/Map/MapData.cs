using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class MapData : ScriptableObject
{
    public List<GameObject> roomTypes;
    public int noOfRooms;
}
